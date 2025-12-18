# about project
Správa osobního kalendáře s funkcemi 
- [x] plánování událostí, 
- [x] notifikacemi, 
- [x] sdílením mezi uživateli. 

**Obecné podmínky pro projekt:**

- [x] Návrh webové aplikace projektu (funkční požadavky, návrh entit/diagramu tříd/databáze). Můžete vytvořit dokument ve Wordu (nemusíte dělat diagramy v EA).

- [x] Webová aplikace typu ASP.NET Core MVC (verze 9.x nebo vyšší)
	- Projekt i tak musí splňovat podmínky využití MVC, vícevrstvé architektury, objektového programování. Ve webovém projektu se také nesmí objevit ani řádek SQL kódu, aby byl oddělen kód webové stránky a databáze (výjimkou je volání SQL procedur - ujistěte se ale, že víte, co znamená "SQL procedura" a "volání SQL procedury"!)
	- Pokud tyto podmínky nebudou splněny, tak projekt neprojde 7. týden kontrolou a ani obhajobou!

- [x] Vytvoření vícevrstvé aplikace -> minimálně musí být obsaženy vrstvy Presentation, Application, Infrastructure a Domain.
	- Všechna funkcionalita musí být vytvořena a používána pomocí služeb (services).
	- V Presentation vrstvě se nesmí objevit přímé použití Infrastructure vrstvy (kromě konfigurace).
	- V Controllerech nesmí být kód logicky spadající do nižších vrstev.

- [x] Vytvoření databáze technikou Code-First (s migracemi) a její napojení a použití pomocí EntityFrameworkCore (nebo obdobného frameworku).

- [x] Projekt musí obsahovat několik entit, popř. ViewModelů/DTO (minimálně 5 - nepočítají se entity z Entity Framework Core a ani z Identity, ani ErrorViewModel). Musí být definováno aspoň jedno propojení entit skrz cizí klíč.

- [x] Vytvoření Area "Admin", ve které budou uloženy Controllery a View pro správu všech položek v databázi adminem (admin bude moct spravovat i data uživatele, ale nebude jim smět měnit hash hesla).
	- [x] Ve správě položek musí být implementována i editace položek.

- [x] Bude vyřešena hlavně serverová validace (ale nejlépe i ta klientská).
	- [x] včetně vytvoření jednoho vlastního validačního atributu (tzn. takového, který jste vytvořili sami!).
- [x] Bude umožněna registrace a přihlášení uživatele -> celkem aspoň 2 role (admin, zákazník/klient, popř. manager/redaktor apod.).
	- [x] Admin má veškerá práva a může spravovat vše.
	- [x] Funkce Managera je taková, že spravuje systém v oblastech, kde interaguje se zákazníky/klienty (např. v případě e-shopu jsou to produkty, objednávky, faktury apod.), ale nemůže měnit kritické vlastnosti systému (uživatele, role, přístupová práva, nastavení webové aplikace apod.).
	- [x] Zákazník/klient využívá systém pro účely, ke kterým je určen (např. v e-shopu nakupuje produkty, vytváří pro sebe objednávky, prohlíží si jen své vlastní objednávky, generuje své faktury apod.).

- [x] Prvky na stránce by měly být responzivní (pomocí Bootstrap nebo jiné technologie, anebo vlastním řešením). Pro front-end je možné použít jakýkoliv framework (např. Vue, React, Angular, SvelteKit apod.)

https://moodle.utb.cz/mod/assign/view.php?id=769105


# Startup
## start existing project
`sudo systemctl start docker` start docker

`docker start pw_calendar_db` start existing container

`dotnet run --project CalendarApp.Web` to run project
`dotnet watch run --project CalendarApp.Web` realtime changes

others
`dotnet restore CalendarApp.sln`
`dotnet build CalendarApp.sln`

website url
`http://localhost:5292/`

## start new project

# project dependencies
## tools and packages
*entitiy framework CLI*
`dotnet new tool-manifest`
`dotnet tool install dotnet-ef --version 9.0.9`

*Project 'CalendarApp.Web'*
> Microsoft.AspNetCore.Identity.EntityFrameworkCore      9.0.9       9.0.9   
> Microsoft.AspNetCore.Identity.UI                       9.0.9       9.0.9   
> Microsoft.EntityFrameworkCore.Design                   9.0.9       9.0.9   
> Microsoft.EntityFrameworkCore.SqlServer                9.0.9       9.0.9   
> Microsoft.EntityFrameworkCore.Tools                    9.0.9       9.0.9   
> Microsoft.VisualStudio.Web.CodeGeneration.Design       9.0.0       9.0.0   
> System.ComponentModel.Annotations                      5.0.0       5.0.0   

*Project 'CalendarApp.Domain'*
> Microsoft.AspNetCore.Identity.EntityFrameworkCore      9.0.9       9.0.9   

*Project 'CalendarApp.Infrastructure'*
> Microsoft.AspNetCore.Identity.EntityFrameworkCore      9.0.9       9.0.9   
> Microsoft.EntityFrameworkCore                          9.0.9       9.0.9   
> Microsoft.EntityFrameworkCore.Tools                    9.0.9       9.0.9   
> Pomelo.EntityFrameworkCore.MySql                       9.0.0       9.0.0   

*Project 'CalendarApp.Application'*
> Microsoft.EntityFrameworkCore                          9.0.9       9.0.9   
> Microsoft.EntityFrameworkCore.Relational               9.0.9       9.0.9   


## dotnet
`sudo pacman -S dotnet-sdk-9.0`

`dotnet tool run dotnet-ef`
`dotnet tool list` installed tools 
`dotnet tool uninstall dotnet-ef` remove tools
`dotnet list package`
`dotnet add package <package> --version <x>` add package


## EF Migration

*create New Migration*
```bash
dotnet tool run dotnet-ef migrations add CleanCalendarReset \
  --project ./CalendarApp.Infrastructure \
  --startup-project ./CalendarApp.Web
```

*apply Migration to Database*
```bash
dotnet tool run dotnet-ef database update \
  --project ./CalendarApp.Infrastructure \
  --startup-project ./CalendarApp.Web
```

*check if migrations are applied*
```bash
dotnet tool run dotnet-ef migrations list \
	--project ./CalendarApp.Infrastructure/CalendarApp.Infrastructure.csproj \
    --startup-project ./CalendarApp.Web/CalendarApp.Web.csproj
```

### seeding users
*name*			*password*
admin			Admin123!
alexjohnson 	User123!
sarahchen		User123!
mikedavis		User123!

## docker
`sudo systemctl start docker` start docker 

*create container*
```
docker run --name pw_calendar_db \
  -e MYSQL_ROOT_PASSWORD=root \
  -e MYSQL_DATABASE=calendar_db \
  -p 3306:3306 \
  -v pw_calendar_db:/var/lib/mysql \
  -d mysql:8.0.29
```
>data stored in: /var/lib/docker/volumes/

`docker start pw_calendar_db` start existing container
`docker stop pw_calendar_db` Stop the container
`docker rm -v pw_calendar_db` remove the container
`docker ps` View running containers
`docker ps -a` View existing containers

`docker exec -it my-mysql mysql -u root -p` Connect to MySQL inside the container

## database

connect to database using vscode extension
