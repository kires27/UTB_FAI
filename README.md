# setup

start docker 
`sudo systemctl start docker`

start existing container
`docker start pw-calendar-db`

to run project; execute inside **Web** directory 
`~/.dotnet/dotnet run`

# dotnet

to call dotnet framework use:
`~/.dotnet/dotnet`

You can invoke the tool from this directory using the following commands: 
`dotnet tool run dotnet-ef`

## EF Migrations

This will create the database schema and apply migrations
```
~/.dotnet/dotnet ef database update \
    --project ./UTB.eshop25.Infrastructure/UTB.eshop25.Infrastructure.csproj \
    --startup-project ./UTB.eshop25.Web/UTB.eshop25.Web.csproj
```

check if migrations are applied
```
~/.dotnet/dotnet ef migrations list \
	--project ./UTB.eshop25.Infrastructure/UTB.eshop25.Infrastructure.csproj \
    --startup-project ./UTB.eshop25.Web/UTB.eshop25.Web.csproj
```

# docker

start docker 
`sudo systemctl start docker`

create container
```
docker run --name pw-calendar-db \
  -e MYSQL_ROOT_PASSWORD=root \
  -e MYSQL_DATABASE=calendar-db \
  -p 3306:3306 \
  -v pw-calendar-db:/var/lib/mysql \
  -d mysql:8.0.29
```
>data stored in: /var/lib/docker/volumes/

start existing container
`docker start pw-calendar-db`

Stop the container
`docker stop pw-calendar-db`

remove the container
`docker rm -v pw-calendar-db`

View running containers
`docker ps [-a]`

Connect to MySQL inside the container
`docker exec -it my-mysql mysql -u root -p`

# database

connect to database using vscode extension