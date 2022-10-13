FROM ubuntu
RUN wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
#RUN apt install -y git
RUN dpkg -i packages-microsoft-prod.deb
RUN apt install apt-transport-https 
RUN sudo apt update
RUN sudo apt install dotnet-runtime-6.0
COPY . .
RUN cd WishList/WishList && dotnet build