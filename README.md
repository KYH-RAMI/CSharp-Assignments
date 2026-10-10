# Prague Parking V2

Prague Parking V2 är ett objektorienterat parkeringssystem utvecklat i C# och .NET 10.

Applikationen använder Spectre.Console för ett interaktivt konsolgränssnitt med tangentbordsnavigering, parkeringsöversikt och fordonsinformation. Projektet byggs stegvis: först Prague Parking 2.0 (G), därefter Prague Parking 2.1 (VG).

**Author:** Rami Issa  
**Development Environment:** Visual Studio Community 2026  
**Framework:** .NET 10  
**Language:** C#  
**Status:** Under Development  
**Last Updated:** 2026-10-10

> **Aktuellt läge:** Domänmodell, konfigurationsinläsning, prisberäkning och utcheckningslogik är implementerade och har testats manuellt i grundläggande scenarier. Huvudmeny och garageöversikt finns. Ett incheckningsgränssnitt har tagits fram men behöver integrationstestas. Kompletta UI-flöden och permanent lagring av parkeringsdata återstår. Projektet är därför **inte färdigt för inlämning eller extern betatestning**.

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
| System.Text.Json | JSON configuration; planned parking-data serialization |
| Git | Version control |
| GitHub | Source repository |
| Visual Studio Community 2026 | Development environment |

---

## Solution Structure

Den dokumenterade strukturen för G-versionen är:

```text
PragueParkingV2
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
│   └── ParkVehicleView.cs      # Framtaget; integrationstest återstår
│
├── Config
│   └── config.json
│
└── Program.cs

PragueParkingV2.Data
│
└── Configuration
    ├── AppConfiguration.cs
    └── ConfigurationService.cs
```

Domänlogik, användargränssnitt och dataåtkomst hålls separerade. Det separata klassbiblioteket `PragueParkingV2.Data` hanterar i nuläget konfigurationen. **Parkeringspersistens är planerad men ännu inte tillagd i projektstrukturen ovan.**

Strukturen visar det senast redovisade utvecklingsläget, inte en ny granskning av alla filer i det lokala Git-repositoryt.

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

**Konfiguration:** `ConfigurationService` läser och validerar `config.json` vid uppstart. Om runtime-filen saknas kan en standardkonfiguration skapas. En befintlig ogiltig konfigurationsfil ska däremot inte skrivas över automatiskt; uppstarten stoppas kontrollerat. Se [`configuration.md`](docs/configuration.md) för struktur och värden.

**Begränsning:** Det finns ännu ingen färdig, verifierad paketering för körning på en annan dator utan SDK eller källkod. Detta testas inför leverans, inte nu.

---

## Domain Model

### Vehicle

`Vehicle` är en abstrakt basklass för samtliga fordonstyper. Den innehåller:

- `Identifier` – generell fordonsidentifierare.
- `ArrivalTime` – fordonets ankomsttid i UTC.

Basklassen validerar identifierare och ankomsttid. Den har också en skyddad konstruktor för en befintlig UTC-ankomsttid inför framtida återställning från JSON. Motsvarande konstruktorer i `Car` och `MC` behöver kontrolleras eller kompletteras i samband med persistensen.

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

Se [`class-diagram.md`](docs/class-diagram.md) för klassernas relationer och metodsignaturer.

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

Konfigurationen läses och valideras vid programstart. Prisberäkningen arbetar med aktiva värden i minnet; `config.json` behöver inte läsas vid varje beräkning.

`UpdatePricing()` finns i domänlagret. **Omläsning av prislistan via administrationsmenyn är ännu inte implementerad.**

Se [`configuration.md`](docs/configuration.md).

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
- **Park Vehicle:** `ParkVehicleView` har tagits fram, men full integration och sparning är inte verifierade.
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

Garageöversikten använder Spectre.Console `Grid`, `Panel` och `Columns` för parkeringskarta och platsinformation.

### Parking Grid

- Upp till 25 parkeringsplatser per sida.
- Fem kolumner och fem rader.
- Dynamisk paginering utifrån garagets storlek.
- Separata paneler för parkeringsplatserna.
- Visuell markering av vald parkeringsplats.
- Färgkodning av beläggning enligt konfigurationen.

### Parking Status

| Status | Color |
|---|---|
| FREE | Green |
| PARTIALLY OCCUPIED | Orange |
| FULL | Red |
| SELECTED SPOT | Gold |

### Navigation

| Key | Action |
|---|---|
| Arrow Up | Previous row |
| Arrow Down | Next row |
| Arrow Left | Previous spot |
| Arrow Right | Next spot |
| Page Up | Previous page |
| Page Down | Next page |
| ESC | Return to Main Menu |

Grundläggande visning och sidnavigering har testats manuellt. Gränsfall och presentation med parkerade fordon behöver verifieras ytterligare.

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

**Status:** Kod för den utökade panelen har tagits fram. Den senaste implementationen behöver fortfarande testas med en bil, en MC och två MC på samma plats samt vid olika terminalstorlekar.

---

## Parking Data Persistence

Permanent lagring av parkeringshusets tillstånd i `parking-data.json` är **planerad men ännu inte implementerad**.

Den fastställda G-strukturen behålls:

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

När persistensen implementeras ska:

- Endast upptagna parkeringsplatser sparas.
- Tomma platser återskapas med hjälp av antalet platser i `config.json`.
- Fordon återställas till **samma plats** med **samma ankomsttid i UTC**.
- JSON-filen uppdateras efter lyckad incheckning, utcheckning och flytt.
- Dubbletter, kapacitetsbrott och ogiltiga sparade värden upptäckas vid inläsning.
- Befintlig felaktig JSON inte tyst skrivas över.
- Misslyckad lagring inte rapporteras till användaren som en lyckad åtgärd.

Ingen platt JSON-struktur eller nya obligatoriska fält införs i G-versionen. Se [`parking-data.md`](docs/parking-data.md) och [`design-decisions.md`](docs/design-decisions.md).

---

## Current Development Status

### Implemented or Started

| Component | Status |
|---|---|
| Domain model (`Vehicle`, `Car`, `MC`, `ParkingSpot`, `ParkingGarage`) | Implementerad; centrala scenarier manuellt testade |
| `ConfigurationService` and `AppConfiguration` | Implementerade; grundläggande validering testad |
| `ParkingFeeCalculator` and `ParkingFeeResult` | Implementerade; beräkningsfall testade |
| `CheckoutService` and `CheckoutResult` | Implementerade; domänflöde testat |
| `MainMenu` and `AppTheme` | Implementerade; grundläggande navigering testad |
| `GarageOverView` and `Spot Details` | Framtagna; utökade tester återstår |
| `MessageService` | Finns; enhetlig användning i alla vyer återstår |
| `ParkVehicleView` | Framtaget; integrationstest återstår |

### Remaining Work

- Implementera och verifiera JSON-persistens och återställning vid programstart.
- Koppla incheckning till säker sparning och testa hela `Park Vehicle`-flödet.
- Integrera utcheckning med UI och permanent sparning.
- Implementera flytt och sökning via UI.
- Implementera administrationsmeny och omläsning av prislista.
- Verifiera garageöversikt och platsdetaljer med parkerade fordon.
- Testa ogiltiga filer, navigeringsgränser och terminalkompatibilitet.
- Genomföra fullständiga integrationstester och stämma av samtliga G-krav.

---

## Testing

Följande har tidigare testats manuellt i utvecklingsmiljön:

- Konfigurationsinläsning, standardvärden och hantering av ogiltig konfiguration.
- Parkering av `Car` och `MC`, kapacitetsregler och dubblettkontroll, inklusive `ABC123`/`abc123`.
- Prisberäkning med gratisgränsen, timavrundning och ändring av aktiva priser.
- `CheckoutService` och dess resultatobjekt på domännivå.
- Grundläggande visning av garageöversikt och huvudmenyns navigering.

Detta motsvarar **inte** ett fullständigt end-to-end-test av programmet. Persistens, UI-flöden och körning på en annan dator återstår.

Genomförda tester och upptäckta problem redovisas i [`LOGGBOK.md`](LOGGBOK.md). Kravstatus redovisas i [`requirements.md`](docs/requirements.md).

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
| [`requirements.md`](docs/requirements.md) | Spårbara G- och VG-krav med status |
| [`project-plan.md`](docs/project-plan.md) | Projektfaser, milstolpar och prioriteringar |
| [`design-decisions.md`](docs/design-decisions.md) | Arkitektoniska och tekniska beslut |
| [`class-diagram.md`](docs/class-diagram.md) | Domänmodell och klassrelationer |
| [`configuration.md`](docs/configuration.md) | `config.json` och konfigurationsregler |
| [`parking-data.md`](docs/parking-data.md) | Beslutat JSON-format och planerad persistens |

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
