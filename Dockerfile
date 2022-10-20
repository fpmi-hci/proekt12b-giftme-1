FROM ubuntu
RUN apt-get update && \
	apt-get install -y git && \
	apt-get install wget -y && \
	wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb && \
	dpkg -i packages-microsoft-prod.deb 

RUN apt-get install dotnet-sdk-6.0 -y && \ 
	apt-get install apt-transport-https -y && \
	apt-get install dotnet-runtime-6.0 -y
	
RUN git clone https://github.com/fpmi-hci/proekt12b-giftme-1.git && \
	cd proekt12b-giftme-1 && \
	git checkout gcloud-dev

RUN cd proekt12b-giftme-1/WishList && /usr/bin/dotnet build

WORKDIR proekt12b-giftme-1/WishList/WishList

CMD /usr/bin/dotnet run
