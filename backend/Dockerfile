# Stage 1: Build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["PennyEcommerce.csproj", "./"]

RUN dotnet restore "PennyEcommerce.csproj"

COPY . .

RUN dotnet publish "PennyEcommerce.csproj" -c Release -o /app/publish --no-restore


# Stage 2: Chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "PennyEcommerce.dll"]