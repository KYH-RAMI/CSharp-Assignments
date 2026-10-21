# Prague Parking V2

Prague Parking V2 är ett objektorienterat parkeringssystem utvecklat i C# och .NET 10.

Applikationen använder Spectre.Console för ett interaktivt konsolgränssnitt med tangentbordsnavigering, parkeringsöversikt och fordonsinformation. Projektet byggs stegvis: först Prague Parking 2.0 (G), därefter Prague Parking 2.1 (VG).

**Author:** Rami Issa  
**Development Environment:** Visual Studio Community 2026  
**Framework:** .NET 10  
**Language:** C#  
**Status:** V2.0 (G) färdig och testad; V2.1 (VG) under utveckling  
**Last Updated:** 2026-10-21

> **Aktuellt läge 2026-10-20:** **Prague Parking V2.0 (G)** är färdig, manuellt testad och dokumenterad. Testhistoriken längre ned avser den versionen och behålls som referens. **Prague Parking V2.1 (VG)** har skapats som två nya projekt i samma solution och har startat med kopierad, separat kod. Namnrymderna har uppdaterats, `IVehicle`, `Bicycle` och `Bus` har tillkommit och migreringen från `VehiclesPerSpot` till storleksbaserad kapacitet pågår. Flera delsteg har kompilerat utan fel, men **hela V2.1:s nya parkeringsflöden, JSON-återställning och busslogik är ännu inte verifierade**. Den gemensamma inlämningen är därför inte färdig.
> **Aktuellt läge 2026-10-21 – V2.1 (VG):** Projektet körs på den separata Git-branchen `feature/prague-parking-v2.1`. Storleksbaserad parkering har införts för Car/MC/Bicycle i nyparkering, flytt, JSON-återställning och utcheckningens rollback. Bil `UKB48K` flyttades 001 → 003 och hittades där efter omstart; flytt till upptagen 002 nekades. Första Bicycle kunde registreras. `IParkingSpot` har implementerats och bussens fyra-platser-logik (`ParkBus`, `ParkBusAtSpot`, reservation och frigöring) har byggts, men **inte funktionstestats genom ett helt bussflöde**. Garage Overview kraschar med Bicycle och kräver ombyggnad. Cykel-/bussavgifter, full konfigurationsomläsning, bussflytt och testdata vid saknad datafil återstår. Äldre 2026-10-20-status nedan är historik, inte dagens utvecklingsläge.

---

## Project Overview

Projektet vidareutvecklar Prague Parking V1 till ett objektorienterat parkeringssystem.

Systemet ska hantera:

- Incheckning av fordon.
- Utcheckning och parkeringsavgifter.
- Sökning efter parkerade fordon.
- Flytt av fordon mellan parkeringsplatser.
- Visuell presentation av parkeringshuset.
- Konfigurerbara parkeringsregler och priser.
- Permanent lagring av parkeringsinformation.

**Prague Parking 2.0 (G)** är färdigställd och bevaras som en separat, testad version. **Prague Parking 2.1 (VG)** finns nu i samma solution som ett eget konsolprojekt och ett eget dataklassbibliotek. Den utvecklas stegvis utan att G-versionens arkitektur eller tester skrivs om. Kompilerad kod och påbörjade funktioner är inte liktydigt med färdig VG-funktionalitet.

---

## Technologies

| Technology | Usage |
|---|---|
| C# | Programming language and object-oriented domain model |
| .NET 10 | Application framework |
| Spectre.Console | Console user interface |
| System.Text.Json | JSON-konfiguration samt sparning och återställning av parkeringsdata |
| Git | Version control |
| GitHub | Source repository |
| Visual Studio Community 2026 | Development environment |

---

## Solution Structure

Den färdigställda G-versionen innehåller bland annat följande:

```text
PragueParkingV2
│
├── Application
│   └── ParkingDataMapper.cs
│
├── Domain
│   ├── Vehicle.cs
│   ├── Car.cs
│   ├── MC.cs
│   ├── ParkingSpot.cs
│   ├── ParkingGarage.cs
│   ├── ParkingFeeCalculator.cs
│   ├── ParkingFeeResult.cs
│   ├── CheckoutService.cs
│   └── CheckoutResult.cs
│
├── UI
│   ├── MainMenu.cs
│   ├── AppTheme.cs
│   ├── GarageOverView.cs
│   ├── MessageService.cs
│   ├── ParkVehicleView.cs
│   ├── CheckOutVehicleView.cs
│   ├── MoveVehicleView.cs
│   ├── FindVehicleView.cs
│   └── AdministrationView.cs
│
├── Config
│   └── config.json
│
└── Program.cs

PragueParkingV2.Data
│
├── Configuration
│   ├── AppConfiguration.cs
│   └── ConfigurationService.cs
│
└── Persistence
    ├── ParkingDataService.cs
    ├── ParkingDataDocument.cs
    ├── ParkingSpotData.cs
    └── VehicleData.cs
```

`Config/config.json` är konfigurationsfilen i huvudprojektet. Vid körning använder `ParkingDataService` `Data/parking-data.json` under `AppContext.BaseDirectory` (vanligen huvudprojektets `bin/Debug/net10.0/Data/`). Det separata klassbibliotekets källkodsmapp `Persistence` är inte lagringsplats för runtime-filen.

Domänlogik, användargränssnitt och dataåtkomst hålls separerade. Klassbiblioteket `PragueParkingV2.Data` innehåller konfigurationshantering och JSON-persistens. `ParkingDataDocument`, `ParkingSpotData` och `VehicleData` är separata datamodeller. `ParkingDataMapper` i huvudprojektets `Application`-lager kopplar samman dessa DTO:er med domänmodellen utan att Data-projektet beror på domänklasserna.

V2.1 finns sedan 2026-10-20 som **två ytterligare projekt** i `PragueParkingV2.slnx`:

```text
PragueParkingV2.slnx
├── PragueParkingV2/                # G – Console App
├── PragueParkingV2.Data/           # G – Class Library
├── PragueParkingV2_1/              # VG – Console App
│   ├── Application/
│   ├── Config/config.json
│   ├── Domain/
│   │   ├── IVehicle.cs
│   │   ├── Vehicle.cs
│   │   ├── Car.cs
│   │   ├── MC.cs
│   │   ├── Bicycle.cs
│   │   ├── Bus.cs
│   │   ├── ParkingSpot.cs
│   │   └── ParkingGarage.cs
│   ├── UI/
│   └── Program.cs
├── PragueParkingV2_1.Data/         # VG – Class Library
│   ├── Configuration/
│   └── Persistence/
├── Documentation/
│   ├── README.md
│   └── LOGGBOK.md
└── docs/
    ├── class-diagram.md
    ├── configuration.md
    ├── design-decisions.md
    ├── parking-data.md
    ├── project-plan.md
    └── requirements.md
```

De två VG-projekten refererar till varandra via `PragueParkingV2_1` → `PragueParkingV2_1.Data`; huvudprojektet ska inte använda G-versionens dataklassbibliotek. V2.0 och V2.1 har separata körkataloger och därmed separata `Data/parking-data.json` vid vanliga körningar från respektive projekt. **Kontrollera alltid körsökvägen vid andra distributions- eller körsätt.**

Strukturen ovan sammanfattar de filer som hittills har redovisats under utvecklingen. Den är inte en ny fullständig inventering av den lokala arbetsmappen.

---

## Getting Started

### Requirements

- Visual Studio Community 2026 med stöd för C# och .NET-utveckling **eller** .NET 10 SDK.
- En terminal som kan visa Spectre.Console och ta emot tangentbordsinmatning. Windows Terminal används vid de hittills dokumenterade UI-testerna.
- Projektets källkod och tillhörande projekt-/solutionfiler.

### Run in Visual Studio

1. Öppna projektets solution i Visual Studio Community 2026.
2. Välj rätt startprojekt: `PragueParkingV2` för G eller `PragueParkingV2_1` för pågående VG. Välj inte ett `.Data`-klassbibliotek.
3. Bygg lösningen.
4. Starta med **Ctrl+F5** eller **F5**.
5. Använd piltangenterna och Enter för att navigera i menyn. **ESC** lämnar garageöversikten och kan avsluta huvudmenyn. **Obs:** V2.1 är ännu inte färdig för fullständiga funktionstester efter de senaste modelländringarna.

### Run with the .NET CLI

Öppna en terminal i katalogen med projektets solution eller huvudprojekt. Kör:

```bash
dotnet restore
dotnet build
```

Starta därefter valt huvudprojekt (exemplet nedan gäller G):

```bash
dotnet run --project <sökväg-till-PragueParkingV2.csproj>
```

Ersätt platshållaren med den faktiska sökvägen i din lokala projektstruktur. Den exakta `.csproj`-sökvägen har inte verifierats i denna dokumentationsgenomgång.

**Konfiguration (G):** `ConfigurationService` läser och validerar `Config/config.json` relativt `AppContext.BaseDirectory` vid uppstart. Om runtime-filen saknas kan en standardkonfiguration skapas. En befintlig ogiltig konfigurationsfil skrivs inte över automatiskt; uppstarten stoppas kontrollerat. **Observera skillnaden mellan projektfilen `PragueParkingV2/Config/config.json` och runtime-kopian under `bin/Debug/net10.0/Config/`.** Ändrad garagekapacitet kräver omstart i G-versionen. För ändringar av enbart priser kan **Administration → Reload Pricing** användas medan programmet körs. Se [`configuration.md`](../docs/configuration.md) för struktur och värden.

**Runtime-data (G):** `ParkingDataService` använder `AppContext.BaseDirectory` och söker efter `Data/parking-data.json`. Om filen saknas returnerar `Load()` ett tomt datadokument i minnet; filen skapas först när `Save()` anropas. Vid programstart återställs fordon via `ParkingDataMapper.ToGarage()` till sina sparade platser med ursprunglig UTC-ankomsttid. Efter parkering, utcheckning eller flytt sparas det aktuella tillståndet med `ToDocument()` och `Save()`. En befintlig ogiltig datafil avvisas i stället för att skrivas över vid normal uppstart. Vi har manuellt testat utvalda skrivfel och rollback, men inte generell kraschsäkerhet eller fullständig transaktionshantering.

**Avgränsning:** Krav på att kunna bygga eller köra lösningen på annan dator behöver verifieras enligt lärarens instruktioner. Det innebär inte att en fristående distributionslösning eller en beta-/webbmiljö ska utvecklas. Den exakta `.csproj`-sökvägen i kommandot ovan behöver stämmas av mot aktuell lokal lösning.

---

## Domain Model

### Vehicle

`Vehicle` är en abstrakt basklass för samtliga fordonstyper. Den innehåller:

- `Identifier` – generell fordonsidentifierare.
- `ArrivalTime` – fordonets ankomsttid i UTC.

Basklassen validerar identifierare och ankomsttid. Den har också en skyddad konstruktor för en befintlig UTC-ankomsttid vid återställning från JSON. I V2.0 gäller detta `Car` och `MC`. I V2.1 implementerar basklassen dessutom `IVehicle`, vars kontrakt hittills anger `Identifier` och `ArrivalTime`; även `Bicycle` och `Bus` har två konstruktörer för nyregistrering respektive återställning av befintlig ankomsttid.

### Car

`Car` representerar en bil och ärver från `Vehicle`.

### MC

`MC` representerar en motorcykel och ärver från `Vehicle`.

### Bicycle och Bus (V2.1)

`Bicycle` och `Bus` har tillkommit som klasser som ärver från `Vehicle`. De lagrar **inte** hårdkodade storlekar; dessa läses från konfigurationsmodellen. Bussens krav på flera sammanhängande platser är en uppgift för parkeringsgaraget, inte för `Bus`-klassen.

### ParkingSpot

`ParkingSpot` representerar en individuell parkeringsplats och ansvarar för:

- Parkeringsplatsnummer.
- En intern lista över parkerade fordon, exponerad via `IReadOnlyList<Vehicle>`.
- I G: kontroll av maxantal och tillåtna fordonskombinationer. I pågående VG: nya metoder för storleksbaserad kapacitet.
- Parkering, sökning och borttagning av fordon på den egna platsen.

I V2.1 har `GetUsedCapacity(Func<IVehicle, int>)`, en överlagrad `CanPark(...)` och en överlagrad `ParkVehicle(...)` lagts till. De nya metoderna använder konfigurerad fordonsstorlek, men de äldre överlagringarna finns ännu kvar under migreringen. Detta är **inte ett avslutat gränssnittskontrakt** eller ett fullt integrerat nytt parkeringsflöde.

### ParkingGarage

`ParkingGarage` representerar hela parkeringshuset och ansvarar för:

- Skapande och hantering av parkeringsplatser.
- Tilldelning av första lämpliga plats vid nyparkering.
- Parkering, sökning, borttagning och flytt av fordon mellan platser.
- Validering av destination och kapacitet innan en flytt genomförs.
- Kontroll av dubbla identifierare i hela garaget, utan hänsyn till stora eller små bokstäver.

V2.1 har dessutom överlagrade metoder för storleksbaserad sökning, parkering och återställning till bestämd plats. **Bussens sammanhängande fyrplatsreservation, flytt och utcheckning är ännu inte implementerade.**

### ParkingFeeCalculator

`ParkingFeeCalculator` ansvarar för prisregler och beräkning av parkeringsavgifter:

- Timpriser för `Car` och `MC`.
- Debitering per påbörjad timme.
- Konfigurerbar gratisgräns.
- Uppdatering av aktiva prisvärden med `UpdatePricing()` efter validering.

Beräkningen returnerar ett `ParkingFeeResult` med parkeringstid, timpris, gratisminuter och total avgift.

### CheckoutService

`CheckoutService` samordnar utcheckningen:

1. Söker efter fordonet.
2. Beräknar parkeringsavgiften.
3. Tar bort fordonet efter lyckad beräkning.
4. Returnerar ett `CheckoutResult` till det anropande lagret.

`ParkingFeeResult` och `CheckoutResult` är tillfälliga resultatobjekt och ska inte sparas i `parking-data.json`.

Se [`class-diagram.md`](../docs/class-diagram.md) för klassernas relationer och metodsignaturer. Diagrammet behöver uppdateras för V2.1.

---

## Configuration

Applikationen använder `config.json` för priser, gratisminuter, garagekapacitet, tidszon och kapacitetsregler för fordonstyper.

### Default Configuration – V2.0 (G)

| Setting | Value |
|---|---|
| Parking Spots | 100 |
| Car Hourly Price | 20 CZK |
| MC Hourly Price | 10 CZK |
| Free Parking | Nuvarande kod: kortare än 10 minuter gratis; exakt 10 minuter debiteras. Uppgiftens formulering bör slutavstämmas. |
| Car Capacity | 1 per spot |
| MC Capacity | 2 per spot |
| Time Zone | Europe/Prague |

Konfigurationen läses och valideras vid programstart. **100 platser är standardvärdet**; antalet kan ändras i konfigurationen och har manuellt testats med **100, 200 och 1 000 platser**. Aktiva prisvärden hålls i `ParkingFeeCalculator` och används utan en ny filläsning för varje beräkning.

**Administration → Reload Pricing är implementerad och manuellt verifierad.** Ett höjt bilpris från 20 till 30 CZK började användas utan omstart. Ett ogiltigt bilpris på −10 CZK avvisades, och det tidigare giltiga priset behölls. Standardpriserna återställdes efter testet. Detta gäller endast priserna; **fullständig konfigurationsomläsning är ett VG-krav**.

### Pågående konfigurationsmodell – V2.1 (VG)

Det avsedda konfigurationsupplägget för V2.1 innehåller:

| Inställning | Avsett grundvärde | Status |
|---|---:|---|
| `Garage.NumberOfParkingSpots` | 100 | Konfigurerat som standard |
| `Garage.SpotCapacity` | 4 | Egenskap tillagd i `AppConfiguration` |
| `Garage.BusMaxParkingSpot` | 50 | Planerat; C#-hantering ännu inte verifierad |
| `VehicleTypes.Car.Size` | 4 | Pågående migrering |
| `VehicleTypes.MC.Size` | 2 | Pågående migrering |
| `VehicleTypes.Bicycle.Size` | 1 | Pågående migrering |
| `VehicleTypes.Bus.Size` | 16 | Pågående migrering |
| `Pricing.CarHourlyPrice` | 20 CZK/h | G-pris, ska bibehållas |
| `Pricing.MotorcycleHourlyPrice` | 10 CZK/h | G-pris, ska bibehållas |
| `Pricing.BicycleHourlyPrice` | 5 CZK/h | VG-krav; komplett kodstöd återstår |
| `Pricing.BusHourlyPrice` | 80 CZK/h | VG-krav; komplett kodstöd återstår |
| `Pricing.FreeMinutes` | 10 | Befintlig regel |
| `System.TimeZone` | `Europe/Prague` | Befintlig regel |

I V2.1 har `VehicleTypeConfiguration.Size` lagts till och `ConfigurationService` validerar positiva `Size`-värden i en `foreach` över konfigurerade typer. **Standardgenerering, övriga konfigurationsklasser, full validering, UI och omladdning under drift är ännu inte konsekvent migrerade.** Den gamla egenskapen `VehiclesPerSpot` används fortfarande i delar av koden och måste avvecklas innan V2.1 funktionstestas.

Se [`configuration.md`](../docs/configuration.md).

---

## User Interface

Applikationens användargränssnitt utvecklas med Spectre.Console och gemensamma UI-komponenter.

### Main Menu

En interaktiv huvudmeny har implementerats med följande menyval:

1. Park Vehicle
2. Check Out Vehicle
3. Move Vehicle
4. Find Vehicle
5. Garage Overview
6. Administration
7. Exit

`AppTheme.ShowMenu()` används för återanvändbar menypresentation och tangentbordsnavigering, medan `MainMenu` ansvarar för att öppna rätt vy. Piltangenterna flyttar markeringen, Enter väljer funktion och ESC kan avsluta huvudmenyn.

- **Garage Overview:** Ansluten till huvudmenyn.
- **Park Vehicle:** Ansluten till huvudmenyn; parkering, automatisk JSON-sparning och återställning efter omstart har verifierats manuellt.
- **Check Out Vehicle:** Ansluten till huvudmenyn; avgiftsberäkning, kvitto, borttagning och JSON-sparning har testats med omstart.
- **Move Vehicle:** Ansluten till huvudmenyn; validering, bekräftelse, flytt, JSON-sparning och lyckad återställning efter omstart har testats.
- **Find Vehicle:** Implementerad sökvy som kan hitta `Car` och `MC`, oberoende av stora/små bokstäver, och visa plats, lokalt presenterad ankomsttid och parkeringstid. Okänt fordonsnummer ger felmeddelande och möjlighet att söka igen.
- **Administration:** Implementerad prisomläsning under körning med validering och bibehållna tidigare priser vid ogiltig ändring.

Samtliga huvudmenyfunktioner i G-versionen har nu testats i relevanta manuella flöden. Det innebär inte att varje tänkbart gränsfall eller varje terminalmiljö är verifierad.

### Visual Theme – V2.0 (G)

`AppTheme` centraliserar färger och återanvändbara UI-funktioner som `PrepareScreen()`, `RestoreScreen()`, `CreatePanel()` och `ShowMenu()`. Temat är inspirerat av Sveriges flagga.

| Element | Color |
|---|---|
| Background | Dark Blue |
| Primary Accent | Swedish Gold |
| Title Gradient | Light Gold / Gold / Dark Gold |
| Main Text | White |
| Secondary Text | Light Blue Grey |
| Panel Borders | Blue |

För V2.1 har titlarna i `Program.cs`, `MainMenu` och `Administration` ändrats så att versionen framgår som **V2.1**. Något färdigt, separat VG-färgschema har inte verifierats.

`MessageService` används för gemensamma statusmeddelanden: success, information, warning och error. Panelerna centreras via `AppTheme.WriteCenteredPanel()`. **2026-10-16 rättades också vänstermarginalen för `Press any key to continue...`** när `waitForKey` används, så att texten ligger i linje med panelen. Detta är manuellt kontrollerat i felmeddelandet för ett redan parkerat fordon. Parkering, utcheckning och flytt har egna centrerade kvittopaneler; full UI-kompatibilitet mellan terminaler återstår.

### Console Rendering

Huvudmenyn har utvecklats för centrerad presentation och minskat flimmer, bland annat med positionsbaserad rendering. Menylogiken har därefter samlats i `AppTheme.ShowMenu()` för att undvika duplicering mellan vyer.

Terminalstorlek, tangentbordsbeteende, ANSI-sekvenser och återställning av bakgrund behöver fortfarande verifieras i flera terminalmiljöer. En tidigare diskuterad reservlösning med `SelectionPrompt` är **inte bekräftad som implementerad**.

---

## Garage Overview

Garageöversikten (`GarageOverView.cs`) använder ett kompakt Spectre.Console-`Grid` **inuti en gemensam `Panel`** för parkeringskartan. Den tidigare `Table`-lösningen, med tabellramar och radavgränsare, har ersatts eftersom den krävde för mycket utrymme och orsakade radbrutna platsnummer. Separata paneler per parkeringsplats används inte längre.

### Parking Map and Layout

- Parkeringsplatserna sorteras efter `SpotNumber` och visas som numrerade, färgade celler utan egna ramar.
- Antalet parkeringsplatser hämtas från `ParkingGarage` (som skapas med konfigurerat antal); inget fast rutnät med 5 × 5 eller 10 × 10 platser används.
- `GetGarageLayout()` beräknar kartans bredd, detaljpanelens bredd, antal kolumner och synliga rader utifrån Spectre.Consoles rapporterade terminalstorlek.
- `PARKING MAP` får **ungefär 80 % av den tillgängliga bredden** och `SPOT DETAILS` ungefär 20 %, med en minsta bredd på 23 tecken för detaljpanelen. Fördelningen är alltså inte exakt 80/20 i alla terminalstorlekar.
- `SPOT DETAILS` ligger längst till höger. `GARAGE STATUS` visas under båda panelerna och redovisar `FREE`, `OCCUPIED` och `VEHICLES`.
- För att undvika terminalscrollning används en **intern viewport endast när hela parkeringskartan inte ryms på höjden**. När viewporten behövs visar footern vilket intervall av parkeringsplatser som syns.
- Ett terminalfönster under 80 kolumner eller 18 rader får ett meddelande om att fönstret behöver förstoras i stället för att en oläsbar karta renderas.

### Parking Status

| Status | Bakgrund | Text i markerad cell |
|---|---|---|
| FREE | Grön | Guld |
| PARTIALLY OCCUPIED | Orange | Guld |
| FULL | Röd | Guld |

**I G-versionen** beräknas beläggningsstatus med `GetSpotColor()` utifrån antalet parkerade fordon och den konfigurerade kapaciteten per fordonstyp. V2.1:s vy måste senare ändras till storleksenheter och bussreservationer. Normalt används temats mörka statustext. **Markering ändrar textfärgen, inte statusbakgrunden.** Färgerna hämtas från `AppTheme`.

### Navigation

| Key | Action |
|---|---|
| Arrow Up | Flytta markeringen en rutnätsrad uppåt |
| Arrow Down | Flytta markeringen en rutnätsrad nedåt |
| Arrow Left | Flytta markeringen en plats åt vänster inom aktuell rad |
| Arrow Right | Flytta markeringen en plats åt höger inom aktuell rad |
| ESC | Gå tillbaka till huvudmenyn |

Navigationen använder **samma dynamiskt beräknade kolumnantal** som parkeringskartan. Layouten beräknas om vid nästa tangenttryckning, exempelvis efter att terminalen har ändrat storlek. Den tidigare sidnavigeringen med `Page Up`/`Page Down` används inte i den här implementationen. `AnsiConsole.Live()` hanterar uppdateringarna under navigering.

### Verified and Remaining Tests

**Visuellt kontrollerat 2026-10-13–15 i Windows Terminal:** 200 konfigurerade platser (001–200) kunde visas utan avklippta nummer i det använda fönstret. `SPOT DETAILS` och `GARAGE STATUS` visade korrekt information för både två bilar och en bil tillsammans med två MC på plats 003. Med en bil och två MC på två platser visades `FREE 198`, `OCCUPIED 2`, `VEHICLES 3`.

**Ytterligare verifierat 2026-10-16:** Garageöversikten provades med **1 000 parkeringsplatser**, där den interna viewporten visade avgränsade platsintervall. Navigering till plats **1000** och tillbaka mot början fungerade; markeringen stannade på plats 1000 vid försök att navigera förbi slutet. Efter återställning till **100 parkeringsplatser** visades **FREE 98 / OCCUPIED 2 / VEHICLES 3**, med två MC på 003 och en bil på 051. Fordonen fanns kvar efter kapacitetsändringen och omstart.

**Återstår att verifiera separat:** ändrad terminalstorlek under körning, fler smala/låga terminalmiljöer, eventuella återstående navigeringsgränsfall och komplett test mot kravspecifikationen. 500 platser har inte redovisats som ett separat prov.

---

## Spot Details

Informationspanelen för markerad parkeringsplats har vidareutvecklats för att visa:

- Parkeringsplatsnummer och status.
- Beläggning.
- Samtliga parkerade fordon på platsen.
- Fordonstyp och identifierare.
- Ankomsttid i konfigurerad tidszon.
- Aktuell parkeringstid (`Duration`).

Parkeringstiden beräknas från `ArrivalTime` i UTC när panelen renderas, och lagras inte som ett eget fordonsfält.

Exempel på tidsformat:

```text
35m
2h 15m
1d 3h 20m
```

**Status:** Panelen har verifierats manuellt med parkerade bilar och två MC på samma plats, inklusive ett **omstartstest den 16 oktober** där båda MC behöll sina identifierare, placering och ursprungliga ankomsttider. Panelen visade individuella tider, parkeringstid och beläggning `2 / 2`. Andra terminalstorlekar och ytterligare gränsfall återstår.

---

## Parking Data Persistence

**Status för V2.0 (G), 2026-10-16: JSON-inläsning och automatisk sparning vid parkering, utcheckning och flytt är implementerade.** Både lyckade normalflöden och utvalda negativa scenarier har verifierats manuellt, inklusive omstart med två MC och framprovocerade skrivfel. Detta ska inte förväxlas med generell transaktionssäkerhet.

- `ParkingDataService` i `PragueParkingV2.Data/Persistence` sköter `Load()`/`Save()`, JSON-serialisering, grundläggande datavalidering, `Data/parking-data.json` relativt `AppContext.BaseDirectory` och temporärfil med `File.Move()`/`File.Replace()`.
- `ParkingDataMapper` i `PragueParkingV2/Application` översätter mellan DTO-modellen och `ParkingGarage`/`Vehicle`. `ToGarage()` skapar garaget enligt `config.json`, använder `ParkVehicleAtSpot()` och behåller identifierare, fordonsklass, parkeringsplats och UTC-ankomsttid. `ToDocument()` sparar endast upptagna platser.
- `Program.cs` läser först konfiguration, sedan parkeringsdokument och bygger garaget via mappern.
- `ParkVehicleView`, `CheckOutVehicleView` och `MoveVehicleView` sparar ändringar efter en domänoperation och före framgångsmeddelandet. Vid rapporterade filskrivningsfel försöker UI-koden återställa det tidigare tillståndet i minnet. **Detta har felinjiceringstestats för alla tre flöden**, men är inte en fullständig transaktionsgaranti.

JSON-strukturen för G-versionen är oförändrad:

```json
{
  "ParkingSpots": [
    {
      "SpotNumber": 17,
      "Vehicles": [
        {
          "Identifier": "ABC123",
          "VehicleType": "Car",
          "ArrivalTime": "2026-10-05T14:30:00Z"
        }
      ]
    }
  ]
}
```

**Verifierat end-to-end den 14 oktober:** Testbilen `ABC123` lästes in på plats 001. Bilen `UKB48K` parkerades på plats 002 kl. 20:53, programmet avslutades helt och båda bilarna fanns kvar på respektive plats efter omstart. Garageöversikten visade två upptagna av 200 platser.

**Verifierat 2026-10-15:** Utcheckning med sparad borttagning kontrollerades efter omstart. `ABC123` flyttades från plats 001 till 050, och placeringen bevarades efter omstart. Två MC kunde dela samma plats.

**Verifierat 2026-10-16:** `DEF123` och `EDF123` återställdes på plats 003 med sina ursprungliga ankomsttider. Korrupt JSON avvisades utan att skrivas över. Dubbletter (`ABC123`/`abc123`), plats 201 i ett 200-platsersgarage, tre MC på en plats och blandning av Car/MC på samma plats avvisades med kontrollerad uppstartsfelhantering. Vi framkallade sparfel genom att tillfälligt skapa en katalog med namnet `parking-data.json.tmp`: en ny parkering av `ROLL123` avbröts utan att fordonet blev kvar, en utcheckning återställde fordonet och en misslyckad flytt återställde ursprungsplatsen. Efter borttagning av den avsiktligt blockerande katalogen sparades flytten **050 → 051**, och plats 051 kvarstod efter omstart.

**Återstår för G vid eventuell ytterligare testning:** separat kontroll av utelämnade obligatoriska JSON-fält, krasch mitt under filersättning, misslyckad kompensation och fullständig temporärfilsstädning i fler feltyper. De manuella testerna är **inte bevis för allmän kraschsäkerhet eller transaktionssäkerhet**.

**V2.1:** `ParkingDataMapper.ToVehicle()` och `ToVehicleData()` har utökats med `Bicycle` och `Bus`. Däremot använder `ToGarage()` ännu den gamla `VehiclesPerSpot`-modellen, och `ParkingDataService` validerar fortfarande endast `Car` och `MC` i den senast visade implementationen. **Inläsning, återställning och sparning med de nya fordonstyperna är därför ännu inte klara eller testade.** Bussens planerade modell är ett fordon med en identifierare, en ankomsttid och en reservation över fyra sammanhängande platser, men det slutliga JSON-formatet är inte fastställt.

Se [`parking-data.md`](../docs/parking-data.md) och [`design-decisions.md`](../docs/design-decisions.md) för detaljer.

---

## Current Development Status

### V2.0 (G) – verifierad status 2026-10-16

| Component | Status 2026-10-16 |
|---|---|
| Domain model (`Vehicle`, `Car`, `MC`, `ParkingSpot`, `ParkingGarage`) | Implementerad; grundfall manuellt testade, inklusive återställning på bestämd plats |
| `ConfigurationService` / `AppConfiguration` | Implementerade; grundläggande validering testad |
| `ParkingFeeCalculator` / `CheckoutService` | Implementerade; utcheckning har även testats via UI inklusive sparad borttagning |
| `MainMenu`, `AppTheme`, `MessageService` | Implementerade för aktiva vyer |
| `GarageOverView` och `Spot Details` | Manuellt testade med 100, 200 och 1 000 platser, viewport/navigering samt bil och två MC; vissa terminalgränsfall återstår |
| `ParkVehicleView` | **Manuellt verifierad** för incheckning av bil samt JSON-sparning och återställning efter omstart |
| `ParkingDataService` / JSON DTO:er | Inläsning, sparning och flera negativa datatester samt framprovocerade skrivfel manuellt verifierade; ingen generell transaktionsgaranti |
| `ParkingDataMapper` | Implementerad; lyckade återställningar och kontrollerade fel för ogiltigt platsnummer och kapacitetsöverskridande verifierade |
| `CheckOutVehicleView` | Kvitto, avgiftsberäkning, sparad borttagning samt rollback vid framprovocerat sparfel verifierade i manuella tester |
| `MoveVehicleView` | Lyckad flytt och omstart, nekad otillåten destination samt rollback vid framprovocerat sparfel verifierade |
| `FindVehicleView`, `AdministrationView` | Båda implementerade och manuellt verifierade; sökning, priskorrigering och avvisning av ogiltigt pris testade |

**V2.0 är färdigställd enligt projektets nuvarande status.** Tabellen bevarar de manuella kontroller som dokumenterades i oktober. Standardvärdet **100** platser har återställts i G-konfigurationen efter tidigare tester med större garage. Eventuella kompletterande leveranskontroller är separata från fortsatt VG-utveckling.

### V2.1 (VG) – aktuell status 2026-10-20

| Del | Status |
|---|---|
| Nya projekt `PragueParkingV2_1` och `PragueParkingV2_1.Data` | Skapade i samma solution; tidigare byggning och programstart bekräftade |
| Namespaces och projektreferens | Uppdaterade; fyra projekt byggde utan fel efter namnbytet |
| `IVehicle` | Skapat och implementerat av abstrakta `Vehicle` |
| `Bicycle` och `Bus` | Klasser och konstruktorer skapade; kodstödet är ännu inte ett komplett UI-flöde |
| Storleksbaserad kapacitet | `GetUsedCapacity()` och överlagrade metoder i `ParkingSpot`/`ParkingGarage` tillagda; integrering pågår |
| `AppConfiguration` och `ConfigurationService` | `Size` och `SpotCapacity` införda; fullständig standardgenerering och validering återstår |
| `ParkVehicleView` | Anrop till ny modell med `spotCapacity` och `getSize` påbörjat; ingen verifierad end-to-end-parkering efter migreringen |
| `ParkingDataMapper` | Stöd för att konvertera Bicycle/Bus till/från fordonsobjekt; `ToGarage` och JSON-valideringen återstår |
| `IParkingSpot` | Ännu inte skapat; VG kräver minst två interfaces |
| Buss över fyra sammanhängande platser | Designskiss framtagen; ännu inte implementerat |
| Priser, UI, flytt, utcheckning och garageöversikt | Anpassningar för nya storlekar/fordon återstår |
| Testdata vid saknad datafil och full konfigurationsomladdning | Återstår |
| VG-tester och reflektion | Återstår |

**Viktig statusgräns:** Flera delsteg har kompilerat utan fel. Detta verifierar inte att V2.1 kan starta och arbeta korrekt med den nya JSON-konfigurationen eller att de nya kraven uppfylls.

---

## Testing

Manuella domäntester finns noterade i [`LOGGBOK.md`](LOGGBOK.md), bland annat Car/MC-kapacitet, dubblettkontroll, prisberäkning och grundläggande checkout. Äldre tester av `ParkingDataService.Load()` omfattade giltig JSON (`ABC123`) och avvisning av okänd fordonstyp (`Truck`).

**Integrationstest 2026-10-14 (Windows Terminal / Visual Studio):**

1. Startade med befintlig data: `ABC123` visades på plats 001 med äldre ankomsttid.
2. Parkerade `UKB48K` via `Park Vehicle`; fick plats 002 och lyckad bekräftelse (ankomsttid 20:53 lokalt).
3. Avslutade programmet helt och startade om.
4. Garageöversikten visade `UKB48K` kvar på plats 002 med oförändrad ankomsttid; `ABC123` kvar på plats 001; `FREE 198`, `OCCUPIED 2`, `VEHICLES 2`.

Detta verifierade **normalflödet för incheckning → sparning → omstart → återställning** den 14 oktober.

**Integrationstester 2026-10-15 (Windows Terminal / Visual Studio):**

1. Check Out Vehicle testades med beräknad avgift, borttagning av fordon och sparning. Efter omstart var den utcheckade bilen borta medan kvarvarande fordon fanns kvar.
2. Move Vehicle testades för bil `ABC123`: från plats 001 till plats 050. Garageöversikten visade plats 001 ledig och plats 050 upptagen, även efter omstart.
3. Flytt till samma plats och en otillåten/upptagen plats avvisades. Inmatningen förbättrades med `while`-loop och en enda uppdaterad felpanel åt gången.
4. Två MC visades på samma plats 003, med kapacitet `2 / 2`, identifierarna `DEF123` och `EDF123` och egna ankomsttider. Flytt av MC till bilplats nekades. Garageöversikten visade `FREE 198`, `OCCUPIED 2`, `VEHICLES 3`.

**Ytterligare manuella tester 2026-10-16 (Windows Terminal / Visual Studio):**

1. `DEF123` och `EDF123` återställdes efter omstart på plats 003 med bevarade ankomsttider.
2. Korrupt JSON, skiftlägesokänsliga fordonsdubbletter, platsnummer utanför garagekapaciteten, för många MC och blandad Car/MC på samma plats avvisades. Den ursprungliga testfilen återställdes efter varje negativt test.
3. Framprovocerade sparfel testade rollback vid **Park**, **Check Out** och **Move**. Testet använde en temporär blockerande katalog med namnet `parking-data.json.tmp` i körmiljön; den togs bort efter testen.
4. Den därefter lyckade flytten `ABC123` från **050 till 051** fanns kvar efter omstart.
5. **Administration → Reload Pricing:** giltig ändring 20 → 30 CZK fungerade utan omstart; −10 CZK avvisades utan att ersätta giltigt pris.
6. **Find Vehicle:** sökning efter `ABC123` och `def123`, även med små bokstäver, samt kontroll av ett registreringsnummer som saknas. Försök att parkera `abc123` igen avvisades som dubblett.
7. **Garage Overview:** 1 000 platser, intern viewport och navigering till sista platsen testades. Därefter återställdes standardkapaciteten 100 platser, med tre fordon på två platser: `FREE 98`, `OCCUPIED 2`, `VEHICLES 3`.
8. `MessageService` justerades så att väntetexten ligger i linje med den centrerade felpanelen; manuellt verifierat i dubblettfallet.

**Inte slutverifierat:** körning på en annan dator, samtliga terminalstorlekar och felscenarier, krasch under filersättning samt slutlig krav- och leveranskontroll.

Testresultat och reflektioner dokumenteras i [`LOGGBOK.md`](LOGGBOK.md); kravstatus i [`requirements.md`](../docs/requirements.md). **Inga nya end-to-end-testresultat för VG har lagts till i den här uppdateringen.**

---

## Prague Parking 2.1 (VG)

Den separata VG-versionen utvecklas nu i projekten `PragueParkingV2_1` och `PragueParkingV2_1.Data`. Arbetet bedrivs stegvis för att bevara en begriplig kodbas och hålla G-versionen intakt.

### Genomfört hittills

- Skapat de två nya .NET 10-projekten i samma solution och kopierat G-koden som utgångspunkt.
- Uppdaterat V2.1:s namespaces och Spectre.Console-referens (0.57.2).
- Lagt till `IVehicle` samt klasserna `Bicycle` och `Bus`.
- Börjat ersätta `VehiclesPerSpot` med konfigurerbar `Size` och `Garage.SpotCapacity`.
- Lagt till `GetUsedCapacity()` och nya överlagrade metoder för storleksbaserad kontroll, parkering, sökning och återställning.
- Börjat koppla `ParkVehicleView` till nya kapacitetsparametrar.
- Utökat mapperns fordonskonvertering med Bicycle/Bus.
- Ändrat programtitlarna till V2.1.

### VG-krav och återstående integrering

- **Minst två interfaces:** `IVehicle` finns. Ytterligare ett meningsfullt interface samt faktisk användning i applikationen återstår.
- **Storleksbaserad parkering:** grundvärden Bicycle 1, MC 2, Car 4 och Bus 16 enheter, med platskapacitet 4. Storlekar och kapacitet ska hämtas från konfigurationen; kvarvarande `VehiclesPerSpot`-anrop måste migreras.
- **Bussparkering:** en buss ska använda fyra **helt lediga sammanhängande** parkeringsplatser inom plats **1–50**. **Car, MC och Bicycle får också använda platserna 1–50.** Bussens tillåtna område är inte ett exklusivt bussområde.
- **En buss, en fordonsidentitet:** identifierare och ankomsttid ska inte dupliceras som fyra separata fordon. Fyra platser ska reserveras och kunna frigöras vid flytt/utcheckning; sökning, avgift och JSON ska hantera bussen som ett fordon. Reservationernas exakta representation och format beslutas vid implementationen.
- **Priser:** Car 20, MC 10, Bicycle 5 och Bus 80 CZK per timme; gratisgränsen 10 minuter bevaras enligt befintlig implementation tills den avstämts mot kravtexten.
- **Konfiguration:** säkra ändringar och full omladdning under drift utan att radera platser eller fordon som används.
- **Saknad datafil:** skapa filen automatiskt och fyll med giltiga testdata enligt VG-kravet.
- **Kvalitetssäkring:** uppdatera UI, JSON-validering, flytt/utcheckning och avgifter; genomför nya enhets-/integrationstester samt regressionstester från G.
- **Personlig reflektion:** färdigställs sist utifrån egna erfarenheter och faktisk utveckling.

**Ej färdigt:** Den storleksbaserade modellen är under migrering, och det finns ingen verifierad sammanhängande bussparkering eller full konfigurationsomladdning. VG-kod som kompilerar innebär inte att dessa funktioner är testade eller godkända.
---

## Documentation

Projektets dokumentation består av:

| Document | Content |
|---|---|
| [`README.md`](README.md) | Projektöversikt och startinstruktioner |
| [`LOGGBOK.md`](LOGGBOK.md) | Utvecklingshistorik, tester, problem och lärdomar |
| [`requirements.md`](../docs/requirements.md) | Spårbara G- och VG-krav med status |
| [`project-plan.md`](../docs/project-plan.md) | Projektfaser, milstolpar och prioriteringar |
| [`design-decisions.md`](../docs/design-decisions.md) | Arkitektoniska och tekniska beslut |
| [`class-diagram.md`](../docs/class-diagram.md) | Domänmodell och klassrelationer |
| [`configuration.md`](../docs/configuration.md) | `config.json` och konfigurationsregler |
| [`parking-data.md`](../docs/parking-data.md) | JSON-format, persistensflöden och återstående skydd |

Länkarna är relativa till denna fil i `Documentation/`; ämnesdokumenten ligger i syskonkatalogen `../docs/`.

Dokumentationen uppdateras när implementation och tester förändrar projektets verifierade status.

---

## GitHub

Projektet versionshanteras med Git och GitHub.

Repositoryt hålls privat under utvecklingen. Det ska göras publikt inför inlämningen enligt uppgiftens krav. Inlämningen ska hänvisa till repositoryts startsida.

Ingen färdig GitHub-länk eller publicerad release anges här eftersom detta inte har verifierats i den aktuella genomgången.

---

## Academic Project

**Project:** Prague Parking V2  
**Assignment:** C# Programming – Assignment 2  
**School:** KYH  
**Program:** Systemutvecklare DevSecOps  
**Academic Year:** 2026  
**Author:** Rami Issa

This project is being developed as the second individual assignment in the C# programming course at KYH.

The assignment focuses on object-oriented programming, application architecture, data persistence, and console-based user interfaces using C# and .NET 10.

---

## Utvecklingsuppdatering 2026-10-21 – Prague Parking 2.1

**Tekniskt genomfört:** `IParkingSpot`/`ParkingSpot`; `ReservedByBus`, `ReserveForBus`, `ReleaseBusReservation`, `ParkBusAtPrimarySpot`; `ParkingGarage.FindAvailableBusSpots`, `FindAvailableBusSpotsAt`, `ParkBus` och `ParkBusAtSpot`. `RemoveVehicle` frigör på kodnivå alla fyra bussreservationer. `ParkingDataMapper.ToGarage` väljer `ParkBusAtSpot` för Bus; `ParkingDataService` godkänner nu Bicycle och Bus; `CheckOutVehicleView` har särskild återställning för Bus vid sparfel.

**Konfigurationsändring:** V2.1:s standardinställningar ligger i `PragueParkingV2_1/Config/config.template.json`; endast denna fil kopieras till runtime. `ConfigurationService` kopierar den till aktiv `Config/config.json` **endast när den aktiva filen saknas**, läser/validerar sedan den aktiva filen och lämnar en befintlig fil orörd. `config.json` är alltså runtime-/administrationsfilen, inte en mall som skrivs över vid varje build.

**Manuellt verifierat:** V2.1 startar efter mallhantering; bil `UKB48K` kunde inte flyttas till full plats 002, kunde flyttas från 001 till 003 och återställdes på 003 efter omstart; en cykelregistrering lyckades. **Inte verifierat:** fyra cyklar på samma plats, Bicycle efter omstart, full bussparkering/utcheckning/flytt/JSON-omstart eller robust felinjektion i V2.1. Garage Overview har ett observerat körfel för Bicycle och är inte klar för VG.

**Git:** Tidigare commit `273d2c73` verifierades på fjärrbranchen `feature/prague-parking-v2.1`. Ändringarna från den 21 oktober ska granskas och committas separat; ingen ny push av dessa ändringar är verifierad.

Se `LOGGBOK.md` för detaljerad arbetsdag och `docs/requirements.md` för spårbar kravstatus. G-versionen och dess historiska testresultat är separata.
