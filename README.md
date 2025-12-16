# about project
Správa osobního kalendáře s funkcemi 
- [ ] plánování událostí, 
- [ ] notifikacemi, 
- [ ] sdílením mezi uživateli. 
- [ ] Vytvoření Web API pro pohodlnější budoucí rozšíření na mobilní aplikaci.

**Obecné podmínky pro projekt:**

- [x] Návrh webové aplikace projektu (funkční požadavky, návrh entit/diagramu tříd/databáze). Můžete vytvořit dokument ve Wordu (nemusíte dělat diagramy v EA).

- [x] Webová aplikace typu ASP.NET Core MVC (verze 9.x nebo vyšší)
	- Projekt i tak musí splňovat podmínky využití MVC, vícevrstvé architektury, objektového programování. Ve webovém projektu se také nesmí objevit ani řádek SQL kódu, aby byl oddělen kód webové stránky a databáze (výjimkou je volání SQL procedur - ujistěte se ale, že víte, co znamená "SQL procedura" a "volání SQL procedury"!)
	- Pokud tyto podmínky nebudou splněny, tak projekt neprojde 7. týden kontrolou a ani obhajobou!

- [x] Vytvoření vícevrstvé aplikace -> minimálně musí být obsaženy vrstvy Presentation, Application, Infrastructure a Domain.
	- Všechna funkcionalita musí být vytvořena a používána pomocí služeb (services).
	- V Presentation vrstvě se nesmí objevit přímé použití Infrastructure vrstvy (kromě konfigurace).
	- V Controllerech nesmí být kód logicky spadající do nižších vrstev.

- [ ] Vytvoření databáze technikou Code-First (s migracemi) a její napojení a použití pomocí EntityFrameworkCore (nebo obdobného frameworku).

- [ ] Projekt musí obsahovat několik entit, popř. ViewModelů/DTO (minimálně 5 - nepočítají se entity z Entity Framework Core a ani z Identity, ani ErrorViewModel). Musí být definováno aspoň jedno propojení entit skrz cizí klíč.

- [ ] Vytvoření Area "Admin", ve které budou uloženy Controllery a View pro správu všech položek v databázi adminem (admin bude moct spravovat i data uživatele, ale nebude jim smět měnit hash hesla).
	- [ ] Ve správě položek musí být implementována i editace položek.

- [ ] Bude vyřešena hlavně serverová validace (ale nejlépe i ta klientská).
	- [ ] včetně vytvoření jednoho vlastního validačního atributu (tzn. takového, který jste vytvořili sami!).
- [ ] Bude umožněna registrace a přihlášení uživatele -> celkem aspoň 2 role (admin, zákazník/klient, popř. manager/redaktor apod.).
	- [ ] Admin má veškerá práva a může spravovat vše.
	- [ ] Funkce Managera je taková, že spravuje systém v oblastech, kde interaguje se zákazníky/klienty (např. v případě e-shopu jsou to produkty, objednávky, faktury apod.), ale nemůže měnit kritické vlastnosti systému (uživatele, role, přístupová práva, nastavení webové aplikace apod.).
	- [ ] Zákazník/klient využívá systém pro účely, ke kterým je určen (např. v e-shopu nakupuje produkty, vytváří pro sebe objednávky, prohlíží si jen své vlastní objednávky, generuje své faktury apod.).

- [ ] Prvky na stránce by měly být responzivní (pomocí Bootstrap nebo jiné technologie, anebo vlastním řešením). Pro front-end je možné použít jakýkoliv framework (např. Vue, React, Angular, SvelteKit apod.)

https://moodle.utb.cz/mod/assign/view.php?id=769105

# start existing project

start docker 
`sudo systemctl start docker`

start existing container
`docker start pw-calendar-db`

to run project
`dotnet run --project CalendarApp.Web/CalendarApp.Web.csproj`
or, execute inside projects **Web** directory 
`dotnet run`

others
`~/.dotnet/dotnet restore CalendarApp.sln`
`~/.dotnet/dotnet build CalendarApp.sln`

website url
`http://localhost:5292/`

# init project dependencies
## dotnet
> using dotnet binary: https://dotnet.microsoft.com/en-us/download/dotnet/9.0

You can invoke the tool from this directory using the following commands: 
`dotnet tool run dotnet-ef`

installed tools
`dotnet tool list --global`

remove tools
`dotnet tool uninstall --global dotnet-ef`

## docker

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
`docker ps`
View existing containers
`docker ps -a`

Connect to MySQL inside the container
`docker exec -it my-mysql mysql -u root -p`

## init EF Migrations


dotnet ef migrations add NewMigration --project CalendarApp.Infrastructure --startup-project CalendarApp.Web


This will create the database schema and apply migrations
```
dotnet ef database update \
    --project ./CalendarApp.Infrastructure/CalendarApp.Infrastructure.csproj \
    --startup-project ./CalendarApp.Web/CalendarApp.Web.csproj
```

check if migrations are applied
```
~/.dotnet/dotnet ef migrations list \
	--project ./CalendarApp.Infrastructure/CalendarApp.Infrastructure.csproj \
    --startup-project ./CalendarApp.Web/CalendarApp.Web.csproj
```

## database

connect to database using vscode extension