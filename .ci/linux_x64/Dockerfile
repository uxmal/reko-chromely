FROM debian:11.6

RUN apt-get update \
	&& apt-get install -y wget ca-certificates \
	&& wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
	&& dpkg -i packages-microsoft-prod.deb \
	&& rm packages-microsoft-prod.deb \
	&& apt-get update \
	&& apt-get install -y dotnet-sdk-7.0 dotnet-runtime-7.0 nodejs npm \
	&& apt-get clean \
	&& rm -rf /var/lib/apt/lists/* \
	&& dotnet nuget locals all --clear
	# reserved for cache eviction: 0
