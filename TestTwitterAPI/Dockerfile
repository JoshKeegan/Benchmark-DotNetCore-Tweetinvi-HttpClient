FROM microsoft/dotnet:2.1-sdk

WORKDIR /app

COPY TestTwitterAPI.csproj .
RUN dotnet restore

COPY . ./
RUN dotnet build -o out

WORKDIR /app/out
ENTRYPOINT ["dotnet", "TestTwitterAPI.dll"]