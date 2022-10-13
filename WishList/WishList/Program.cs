using System.Text.Json.Serialization;
using CloudinaryDotNet;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WishList.DAL.Core;
using WishList.DAL.Core.Entities;
using WishList.DAL.Core.UnitOfWork;
using WishList.DAL.Core.Repositories.Interfaces;
using WishList.DAL.Core.Repositories.Implementation;
using WishList.Business;
using WishList.Business.IServices;
using WishList.Business.Implementation;
using WishList.WebApi.Auth;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Mail;
using WishList.Business.ModelsDto;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("WishlistAppCon");

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<WishListContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<WishListContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAutoMapper(typeof(AutoMap));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = false;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidIssuer = builder.Configuration.GetValue<string>("JsonWebToken:Issuer"),
                       ValidateAudience = false,
                       RequireExpirationTime = true,
                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.Zero,
                       RequireSignedTokens = true,
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                           builder.Configuration.GetValue<string>("JsonWebToken:SecretKey")))
                   };
                   options.SaveToken = true;
               });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo() { Title = "WebAPI", Version = "v1" });
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped((serviceProvider) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    return new SmtpClient()
    {
        Host = config.GetValue<string>("Email:Smtp:Host"),
        Port = config.GetValue<int>("Email:Smtp:Port"),
        UseDefaultCredentials = config.GetValue<bool>("Email:Smtp:UseDefaultCredentials"),
        Credentials = new NetworkCredential(
            config.GetValue<string>("Email:Smtp:Username"),
            config.GetValue<string>("Email:Smtp:Password")
        ),
        EnableSsl = config.GetValue<bool>("Email:Smtp:EnableSsl")
    };
});
builder.Services.AddScoped((serviceProvider) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    Account account = new Account(
        config.GetValue<string>("Cloudinary:Account:Cloud"),
        config.GetValue<string>("Cloudinary:Account:ApiKey"),
        config.GetValue<string>("Cloudinary:Account:ApiSecret"));
    var cloudinary = new Cloudinary(account);
    cloudinary.Api.Secure = true;
    return cloudinary;
});
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddCors(c => c.AddPolicy("AllowOrigin", options =>
    options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
    .Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/User/SignIn");
        });

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var serviceScope = app.Services.CreateScope())
{
    await using var dbContext = serviceScope
        .ServiceProvider
        .GetRequiredService<WishListContext>();
    await dbContext.Database.MigrateAsync();
}

using (var serviceScope = app.Services.CreateScope())
{
    using var userManager = serviceScope
        .ServiceProvider
        .GetRequiredService<UserManager<User>>();
    var mapper = serviceScope
        .ServiceProvider
        .GetRequiredService<IMapper>();

    var userDto = new UserDto
    {
        UserName = "admin",
        Email = "arseniy.borsukov@itechart-group.com",
        Role = "Admin"
    };

    var user = mapper.Map<User>(userDto);
    await userManager.CreateAsync(user, "12345678aA@");
    await userManager.AddToRoleAsync(user, userDto.Role);
}

app.UseDeveloperExceptionPage();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapFallback(HandleRedirect);
});

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

static Task HandleRedirect(HttpContext context)
{
    var path = context.Request.Path.ToUriComponent().Trim('/');
    var responseUri = $"/api/Event/{path}";
    context.Response.Redirect(responseUri);
    return Task.CompletedTask;
}
