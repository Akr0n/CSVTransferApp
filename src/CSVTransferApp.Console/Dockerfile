FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and restore/build inside the container
COPY . .
RUN dotnet restore CSVTransferApp.sln \
    && dotnet publish src/CSVTransferApp.Console/CSVTransferApp.Console.csproj \
        -c Release -r linux-x64 --self-contained true \
        /p:PublishSingleFile=true -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["./CSVTransferApp"]
