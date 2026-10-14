# Prague Parking V2

Prague Parking V2 är ett objektorienterat parkeringssystem utvecklat i C# och .NET 10.

Applikationen använder Spectre.Console för ett interaktivt konsolgränssnitt med tangentbordsnavigering, parkeringsöversikt och fordonsinformation. Projektet byggs stegvis: först Prague Parking 2.0 (G), därefter Prague Parking 2.1 (VG).

**Author:** Rami Issa  
**Development Environment:** Visual Studio Community 2026  
**Framework:** .NET 10  
**Language:** C#  
**Status:** Under Development  
**Last Updated:** 2026-10-14

> **Aktuellt läge 2026-10-14:** Domänmodell, konfiguration, prisberäkning, utcheckningslogik på domännivå, huvudmeny och interaktiv garageöversikt är implementerade. **Park Vehicle fungerar via UI med automatisk JSON-sparning.** `ParkingDataMapper` återställer domänobjekt från JSON vid uppstart. Ett manuellt end-to-end-test har verifierat att en ny bil (`UKB48K`) sparades och återkom på samma plats med samma ankomsttid efter programomstart; den tidigare testbilen (`ABC123`) återställdes samtidigt. **Check Out, Move, Find och Administration är ännu inte färdiga UI-flöden.** Felscenarier för persistens, komplett kapacitets-/filvalidering och slutlig G-verifiering återstår. Projektet är **inte färdigt för inlämning eller extern betatestning**.

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

Den första versionen, **Prague Parking 2.0**, utvecklas för betygsnivå G. **Prague Parking 2.1** planeras som ett separat projekt i samma solution för VG. Planerade funktioner räknas inte som implementerade.

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

Den dokumenterade strukturen för G-versionen är:

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
│   └── ParkVehicleView.cs      # Integrationstestad med JSON-sparning
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

Strukturen bygger på de filer som har redovisats under utvecklingen, inte en ny fullständig genomgång av det lokala Git-repositoryt.

---

## Getting Started

### Requirements

- Visual Studio Community 2026 med stöd för C# och .NET-utveckling **eller** .NET 10 SDK.
- En terminal som kan visa Spectre.Console och ta emot tangentbordsinmatning. Windows Terminal används vid de hittills dokumenterade UI-testerna.
- Projektets källkod och tillhörande projekt-/solutionfiler.

### Run in Visual Studio

1. Öppna projektets solution i Visual Studio Community 2026.
2. Kontrollera att huvudprojektet `PragueParkingV2` är valt som startprojekt, inte klassbiblioteket `PragueParkingV2.Data`.
3. Bygg lösningen.
4. Starta med **Ctrl+F5** eller **F5**.
5. Använd piltangenterna och Enter för att navigera i menyn.

### Run with the .NET CLI

Öppna en terminal i katalogen med projektets solution eller huvudprojekt. Kör:

```bash
dotnet restore
dotnet build
```

Starta därefter huvudprojektet:

```bash
dotnet run --project <sökväg-till-PragueParkingV2.csproj>
```

Ersätt platshållaren med den faktiska sökvägen i din lokala projektstruktur. Den exakta `.csproj`-sökvägen har inte verifierats i denna dokumentationsgenomgång.

**Konfiguration:** `ConfigurationService` läser och validerar `config.json` vid uppstart. Om runtime-filen saknas kan en standardkonfiguration skapas. En befintlig ogiltig konfigurationsfil ska däremot inte skrivas över automatiskt; uppstarten stoppas kontrollerat. Se [`configuration.md`](../docs/configuration.md) för struktur och värden.

**Runtime-data:** `ParkingDataService` använder `AppContext.BaseDirectory` och söker efter `Data/parking-data.json` relativt programmets körkatalog. Om filen saknas returnerar `Load()` ett tomt datadokument i minnet; filen skapas inte förrän `Save()` anropas. Vid programstart konverteras inläst data med `ParkingDataMapper.ToGarage()`. Efter en lyckad parkering från `ParkVehicleView` används `ToDocument()` och `Save()`. Automatisk sparning vid utcheckning och flytt återstår.

**Begränsning:** Det finns ännu ingen färdig, verifierad paketering för körning på en annan dator utan SDK eller källkod. Detta testas inför leverans, inte nu.

---

## Domain Model

### Vehicle

`Vehicle` är en abstrakt basklass för samtliga fordonstyper. Den innehåller:

- `Identifier` – generell fordonsidentifierare.
- `ArrivalTime` – fordonets ankomsttid i UTC.

Basklassen validerar identifierare och ankomsttid. Den har också en skyddad konstruktor för en befintlig UTC-ankomsttid inför framtida återställning från JSON. `Car` och `MC` har även konstruktorsöverlagringar med en befintlig UTC-ankomsttid och används vid återställning från JSON.

### Car

`Car` representerar en bil och ärver från `Vehicle`.

### MC

`MC` representerar en motorcykel och ärver från `Vehicle`.

### ParkingSpot

`ParkingSpot` representerar en individuell parkeringsplats och ansvarar för:

- Parkeringsplatsnummer.
- En intern lista över parkerade fordon, exponerad via `IReadOnlyList<Vehicle>`.
- Kontroll av kapacitet och tillåtna fordonskombinationer.
- Parkering, sökning och borttagning av fordon på den egna platsen.

### ParkingGarage

`ParkingGarage` representerar hela parkeringshuset och ansvarar för:

- Skapande och hantering av parkeringsplatser.
- Tilldelning av första lämpliga plats vid nyparkering.
- Parkering, sökning och borttagning av fordon.
- Kontroll av dubbla identifierare i hela garaget, utan hänsyn till stora eller små bokstäver.

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

Se [`class-diagram.md`](../docs/class-diagram.md) för klassernas relationer och metodsignaturer.

---

## Configuration

Applikationen använder `config.json` för priser, gratisminuter, garagekapacitet, tidszon och kapacitetsregler för fordonstyper.

### Default Configuration

| Setting | Value |
|---|---|
| Parking Spots | 100 |
| Car Hourly Price | 20 CZK |
| MC Hourly Price | 10 CZK |
| Free Parking | Under 10 minuter; vid exakt 10 minuter debiteras en påbörjad timme |
| Car Capacity | 1 per spot |
| MC Capacity | 2 per spot |
| Time Zone | Europe/Prague |

Konfigurationen läses och valideras vid programstart. **100 platser är standardvärdet**; antalet parkeringsplatser kan ändras i konfigurationen. Garageöversikten har kontrollerats visuellt med 200 konfigurerade platser. Prisberäkningen arbetar med aktiva värden i minnet; `config.json` behöver inte läsas vid varje beräkning.

`UpdatePricing()` finns i domänlagret. **Omläsning av prislistan via administrationsmenyn är ännu inte implementerad.**

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
- **Park Vehicle:** Ansluten till huvudmenyn; parkering, automatisk JSON-sparning och återställning efter omstart har verifierats manuellt för `Car`.
- **Check Out, Move, Find och Administration:** Kompletta användarflöden återstår.

Att ett menyval syns innebär alltså inte att funktionen är färdig.

### Visual Theme

`AppTheme` centraliserar färger och återanvändbara UI-funktioner som `PrepareScreen()`, `RestoreScreen()`, `CreatePanel()` och `ShowMenu()`. Temat är inspirerat av Sveriges flagga.

| Element | Color |
|---|---|
| Background | Dark Blue |
| Primary Accent | Swedish Gold |
| Title Gradient | Light Gold / Gold / Dark Gold |
| Main Text | White |
| Secondary Text | Light Blue Grey |
| Panel Borders | Blue |

`MessageService` används för gemensamma statusmeddelanden: success, information, warning och error. Enhetlig integration i samtliga vyer återstår att kontrollera.

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

Beläggningsstatus beräknas med `GetSpotColor()` utifrån antalet parkerade fordon och den konfigurerade kapaciteten per fordonstyp. Normalt används temats mörka statustext. **Markering ändrar textfärgen, inte statusbakgrunden.** Färgerna hämtas från `AppTheme`.

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

**Visuellt kontrollerat 2026-10-13–14 i Windows Terminal:** 200 konfigurerade platser (001–200) visades samtidigt utan avklippta nummer eller terminalscrollning i det använda fönstret. `SPOT DETAILS` syns till höger och `GARAGE STATUS` under båda panelerna. Den 14 oktober visades både lediga platser (grön) och två fulla bilplatser (röd), med räknarna `FREE 198`, `OCCUPIED 2`, `VEHICLES 2`.

**Återstår att verifiera:** navigeringens samtliga gränsfall, förändrad terminalstorlek, andra konfigurerade garageantal (inklusive 100, 500 och 1 000), viewport vid otillräcklig höjd, samt orange status för en ensam MC och återställning av två MC på samma plats. Testerna utgör ännu inte en fullständig G-verifiering.

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

**Status:** Panelen har nu även verifierats manuellt med parkerade bilar: registreringsnummer, UTC-baserad ankomsttid presenterad lokalt, beläggning och löpande parkeringstid. Test med en MC, två MC på samma plats och olika terminalstorlekar återstår.

---

## Parking Data Persistence

**Status 2026-10-14: Normalflödet för JSON-inläsning, återställning och automatisk sparning vid incheckning är manuellt verifierat.** Persistensen är ännu inte färdig för alla åtgärder och felscenarier.

- `ParkingDataService` i `PragueParkingV2.Data/Persistence` sköter `Load()`/`Save()`, JSON-serialisering, grundläggande datavalidering, `Data/parking-data.json` relativt `AppContext.BaseDirectory` och temporärfil med `File.Move()`/`File.Replace()`.
- `ParkingDataMapper` i `PragueParkingV2/Application` översätter mellan DTO-modellen och `ParkingGarage`/`Vehicle`. `ToGarage()` skapar garaget enligt `config.json`, använder `ParkVehicleAtSpot()` och behåller identifierare, fordonsklass, parkeringsplats och UTC-ankomsttid. `ToDocument()` sparar endast upptagna platser.
- `Program.cs` läser först konfiguration, sedan parkeringsdokument och bygger garaget via mappern.
- `ParkVehicleView` skriver till datafilen efter parkering och visar framgångsmeddelande först när `Save()` slutförts. Vid `IOException` eller `UnauthorizedAccessException` försöker koden ångra tillägget i minnet. **Denna kompensation har inte felinjiceringstestats och är inte en fullständig transaktionsgaranti.**

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

**Återstår:** spara efter utcheckning och flytt; säkerställa hantering av saknade obligatoriska JSON-fält, korrupt befintlig fil, identifierarregler och gränsvärden mot aktuell konfiguration; testa `.tmp`-hantering, avbruten skrivning och misslyckad kompensation samt ytterligare kombinationer som två MC på samma plats. Den verifierade lyckade normalvägen är **inte bevis för allmän kraschsäkerhet eller transaktionssäkerhet**.

Se [`parking-data.md`](../docs/parking-data.md) och [`design-decisions.md`](../docs/design-decisions.md) för detaljer.

---

## Current Development Status

| Component | Status 2026-10-14 |
|---|---|
| Domain model (`Vehicle`, `Car`, `MC`, `ParkingSpot`, `ParkingGarage`) | Implementerad; grundfall manuellt testade, inklusive återställning på bestämd plats |
| `ConfigurationService` / `AppConfiguration` | Implementerade; grundläggande validering testad |
| `ParkingFeeCalculator` / `CheckoutService` | Implementerade och manuellt testade på domännivå; checkout-UI saknas |
| `MainMenu`, `AppTheme`, `MessageService` | Implementerade för aktiva vyer |
| `GarageOverView` och `Spot Details` | Visuellt verifierade med 200 platser, två fulla bilplatser och korrekta räknare; fler gränsfall återstår |
| `ParkVehicleView` | **Manuellt verifierad** för incheckning av bil samt JSON-sparning och återställning efter omstart |
| `ParkingDataService` / JSON DTO:er | Grundläggande `Load()`/`Save()` och validering; lyckat end-to-end-sparflöde verifierat, feltester återstår |
| `ParkingDataMapper` | Implementerad och verifierad för återställning av sparade bilar samt export av nytt garagetillstånd |
| `Check Out`, `Move`, `Find`, `Administration` | Ännu inte färdiga UI-flöden |

**Närmaste arbete:** Check Out med avgift och sparad borttagning → Move med bibehållen ankomsttid och sparning → Find → prisomläsning i Administration → feltester och full G-avstämning. Standardkonfigurationen är **100** platser; det aktuella integrationstestet gjordes med **200** genom konfigurering, inte hårdkodning.

---

## Testing

Manuella domäntester finns noterade i [`LOGGBOK.md`](LOGGBOK.md), bland annat Car/MC-kapacitet, dubblettkontroll, prisberäkning och grundläggande checkout. Äldre tester av `ParkingDataService.Load()` omfattade giltig JSON (`ABC123`) och avvisning av okänd fordonstyp (`Truck`).

**Integrationstest 2026-10-14 (Windows Terminal / Visual Studio):**

1. Startade med befintlig data: `ABC123` visades på plats 001 med äldre ankomsttid.
2. Parkerade `UKB48K` via `Park Vehicle`; fick plats 002 och lyckad bekräftelse (ankomsttid 20:53 lokalt).
3. Avslutade programmet helt och startade om.
4. Garageöversikten visade `UKB48K` kvar på plats 002 med oförändrad ankomsttid; `ABC123` kvar på plats 001; `FREE 198`, `OCCUPIED 2`, `VEHICLES 2`.

Detta verifierar **normalflödet för incheckning → sparning → omstart → återställning**, inte fel under filskrivning, ogiltig JSON, utcheckning eller flytt. Fortsatta tester krävs för MC-placering, återställning av två MC, statusfärg orange, terminalgränser, andra garageantal och säker filersättning.

Testresultat och reflektioner dokumenteras i [`LOGGBOK.md`](LOGGBOK.md); kravstatus i [`requirements.md`](../docs/requirements.md).

---

## Prague Parking 2.1

VG-versionen planeras som ett separat projekt i samma solution, efter att G-versionen har färdigställts och verifierats.

Planerade funktioner:

- Minst två interfaces.
- `Bicycle` och `Bus`.
- Storleksbaserad kapacitet: Bicycle = 1, MC = 2, Car = 4 och Bus = 16 enheter; en normal plats motsvarar 4 enheter.
- Nyparkering av buss på **fyra sammanhängande lediga platser inom plats 1–50**.
- Möjlighet att **flytta en redan parkerad buss** till fyra sammanhängande lediga platser **även efter plats 50**.
- Konfigurerbara fordonsstorlekar och nya priser.
- Omläsning och validering av hela konfigurationen under körning.
- Individuell reflektion.

VG-funktionaliteten är ännu inte implementerad. Hur en buss ska representeras i sparad data bestäms i samband med VG-utvecklingen utan att ändra G-formatet i förtid.

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
| [`parking-data.md`](../docs/parking-data.md) | Fastställt JSON-format, testad incheckningspersistens och återstående skydd |

Länkarna ovan utgår från att ämnesdokumenten ligger i katalogen `docs/`. Om repositoryts faktiska dokumentstruktur skiljer sig behöver länkarna justeras före commit.

Dokumentationen uppdateras när implementation och tester förändrar projektets verifierade status.

---

## GitHub

Projektet versionshanteras med Git och GitHub.

Repositoryt hålls privat under utvecklingen. Det ska göras publikt inför inlämningen enligt uppgiftens krav. Inlämningen ska hänvisa till repositoryts startsida.

Ingen färdig GitHub-länk eller publicerad release anges här eftersom detta inte har verifierats i den aktuella genomgången.

### Future Deployment and Beta Testing

När G- och VG-funktionerna är tillräckligt stabila kan en Windows-version paketeras och testas på en annan dator utan att dela själva källkodsprojektet.

En framtida separat DevSecOps-testmiljö med Docker på Synology NAS, webbaserad terminal och en tillfällig Cloudflare-adress har diskuterats. Detta är **planerat**, inte driftsatt och inte ett extra krav i C#-uppgiften. Om flera användare ska dela parkeringshus krävs dessutom central hantering av samtidiga ändringar och begränsad åtkomst till NAS:en.

Feedback från eventuella testare kan samlas via Discord.

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
