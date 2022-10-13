using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WishList.DAL.Core.Entities;

namespace WishList.DAL.Core
{
    public class WishListContext : IdentityDbContext<User, Role, Guid>
    {
        public WishListContext(DbContextOptions<WishListContext> options)
           : base(options) { }
        //public DbSet<Currency> Currencies { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<WLEvent> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Gift>();
            modelBuilder.Entity<User>(
                typeBuilder =>
            {
                typeBuilder.HasMany(user => user.Events)
                    .WithOne(wlEvent => wlEvent.User);
            });
            modelBuilder.Entity<WLEvent>(
                typeBuilder =>
                {
                    typeBuilder.HasOne(wlEvent => wlEvent.User)
                        .WithMany(user => user.Events)
                        .HasForeignKey(wlEvent => wlEvent.UserId);
                });
        }
    }
}

