# Prague Parking V2

Prague Parking V2 är ett objektorienterat parkeringssystem utvecklat i C# och .NET 10.

Applikationen använder Spectre.Console för att presentera ett interaktivt konsolgränssnitt med tangentbordsnavigering, parkeringsöversikt och fordonsinformation.

**Author:** Rami Issa

**Development Environment:** Visual Studio Community 2026  
**Framework:** .NET 10  
**Language:** C#  
**Status:** Under Development  
**Last Updated:** 2026-10-10

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

Den första versionen, Prague Parking 2.0, utvecklas för betygsnivå G.

En separat vidareutveckling, Prague Parking 2.1, planeras för VG.

---

## Technologies

| Technology | Usage |
|---|---|
| C# | Programming language |
| .NET 10 | Application framework |
| Spectre.Console | Console user interface |
| System.Text.Json | JSON configuration |
| Git | Version control |
| GitHub | Source repository |
| Visual Studio 2026 | Development environment |

---

## Solution Structure

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
│   └── MessageService.cs
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

Domänlogik, användargränssnitt och dataåtkomst hålls separerade.

---

## Domain Model

### Vehicle

Abstrakt basklass för samtliga fordonstyper.

Innehåller:

- Identifier
- ArrivalTime

Ankomsttiden registreras i UTC.

### Car

Representerar en bil och ärver från Vehicle.

### MC

Representerar en motorcykel och ärver från Vehicle.

### ParkingSpot

Representerar en parkeringsplats.

Ansvarar för:

- Parkeringsplatsnummer.
- Parkerade fordon.
- Kapacitetskontroll.
- Parkering av fordon.
- Sökning efter fordon.
- Borttagning av fordon.

### ParkingGarage

Representerar parkeringshuset.

Ansvarar för:

- Skapande av parkeringsplatser.
- Tilldelning av lämplig plats.
- Parkering.
- Sökning.
- Borttagning.
- Kontroll av dubbla fordonsidentifierare.

### ParkingFeeCalculator

Ansvarar för beräkning av parkeringsavgifter.

Stöder:

- Timpriser för Car och MC.
- Debitering per påbörjad timme.
- Konfigurerbar gratisgräns.
- Uppdatering av aktiva priser.

### CheckoutService

Samordnar utcheckningsprocessen.

Processen består av:

1. Söka efter fordon.
2. Beräkna parkeringsavgift.
3. Ta bort fordonet.
4. Returnera CheckoutResult.

---

## Configuration

Applikationen använder `config.json`.

Konfigurationen innehåller:

- Parkeringspriser.
- Gratisminuter.
- Antal parkeringsplatser.
- Tidszon.
- Kapacitetsregler för fordonstyper.

### Default Configuration

| Setting | Value |
|---|---|
| Parking Spots | 100 |
| Car Hourly Price | 20 CZK |
| MC Hourly Price | 10 CZK |
| Free Parking | Less than 10 minutes |
| Car Capacity | 1 per spot |
| MC Capacity | 2 per spot |
| Time Zone | Europe/Prague |

Konfigurationen läses vid programstart.

En saknad runtime-konfiguration kan återskapas automatiskt med standardvärden.

Befintlig konfiguration valideras innan den används.

---

## User Interface

Applikationens användargränssnitt utvecklas med Spectre.Console.

### Main Menu

En interaktiv huvudmeny har implementerats.

Menyval:

1. Park Vehicle
2. Check Out Vehicle
3. Move Vehicle
4. Find Vehicle
5. Garage Overview
6. Administration
7. Exit

Navigering sker med piltangenterna.

Enter väljer funktion och ESC kan avsluta huvudmenyn.

Garage Overview är kopplad till huvudmenyn.

Övriga menyfunktioner är ännu under utveckling.

### Visual Theme

Applikationen använder ett gemensamt färgtema inspirerat av Sveriges flagga.

Temat hanteras genom `AppTheme`.

Färgpaletten innehåller:

| Element | Color |
|---|---|
| Background | Dark Blue |
| Primary Accent | Swedish Gold |
| Title Gradient | Light Gold / Gold / Dark Gold |
| Main Text | White |
| Secondary Text | Light Blue Grey |
| Panel Borders | Blue |

Färgtemat är avsett att ge hög kontrast och god läsbarhet.

Bakgrundsrenderingen har vidareutvecklats för att minska skillnader mellan terminalens standardfärg och applikationens färgtema.

Kompatibilitet med olika terminaler behöver fortsatt verifieras.

### Console Rendering

Huvudmenyn använder positionsbaserad rendering.

Panelen placeras centralt utifrån terminalens bredd och höjd.

Vid navigering uppdateras endast de menyval som ändras.

Det minskar behovet av att rita om hela konsolen.

En alternativ meny med Spectre.Console SelectionPrompt kan användas om terminalfönstret är för litet för den ordinarie layouten.

---

## Garage Overview

Garageöversikten använder Spectre.Console Grid och Panel.

### Parking Grid

- 25 parkeringsplatser per sida.
- Fem kolumner.
- Fem rader.
- Dynamisk pagination.
- Individuella parkeringspaneler.
- Markering av vald parkeringsplats.
- Färgkodning av beläggning.

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

---

## Spot Details

En informationspanel visar detaljer om den markerade parkeringsplatsen.

Panelen vidareutvecklas för att visa:

- Parkeringsplatsnummer.
- Parkeringsstatus.
- Beläggning.
- Fordonstyp.
- Fordonsidentifierare.
- Ankomsttid.
- Aktuell parkeringstid.

Samtliga parkerade fordon på platsen ska visas.

Det innebär exempelvis att båda motorcyklarna presenteras när två MC delar samma parkeringsplats.

### Duration

Parkeringstiden beräknas från fordonets ArrivalTime.

Exempel:

```text
35m
2h 15m
1d 3h 20m
```

Tidsberäkningen använder UTC.

Ankomsttiden konverteras till den konfigurerade tidszonen vid presentation.

Duration beräknas vid rendering och lagras inte permanent i fordonsobjektet.

Detaljpanelen behöver fortfarande verifieras med parkerade fordon.

---

## Current Development Status

### Implemented or Started

- Object-oriented domain model.
- Vehicle inheritance.
- Parking capacity validation.
- Configuration loading and validation.
- Parking fee calculation.
- Checkout domain service.
- Main menu.
- Shared UI theme.
- Interactive garage overview.
- Keyboard navigation.
- Dynamic pagination.
- Parking status visualization.
- Spot Details development.

### Remaining Work

- Complete Park Vehicle UI.
- Complete Check Out Vehicle UI.
- Implement Move Vehicle.
- Implement Find Vehicle UI.
- Implement Administration menu.
- Implement parking-data.json persistence.
- Implement pricing reload through the menu.
- Verify Spot Details with parked vehicles.
- Test terminal compatibility and rendering.
- Complete integration testing.

---

## Testing

Manuella tester har genomförts för:

- Konfigurationsinläsning.
- Konfigurationsvalidering.
- Parkeringskapacitet.
- Dubblettkontroll.
- Prisberäkning.
- Gratis parkering.
- CheckoutService.
- Grundläggande garageöversikt.
- Huvudmenyns navigering.

Ytterligare tester krävs innan Prague Parking 2.0 kan betraktas som färdig.

---

## Prague Parking 2.1

VG-versionen ska utvecklas som ett separat projekt i samma solution.

Planerade funktioner:

- Minst två interfaces.
- Bicycle.
- Bus.
- Storleksbaserad kapacitet.
- Bussparkering på fyra sammanhängande platser.
- Konfigurerbara fordonsstorlekar.
- Omläsning och validering av hela konfigurationen.
- Individuell reflektion.

VG-versionen är ännu inte implementerad.

---

## Documentation

Projektets dokumentation omfattar:

- README.md
- LOGGBOK.md
- requirements.md
- project-plan.md
- design-decisions.md
- class-diagram.md
- configuration.md
- parking-data.md

Dokumentationen uppdateras löpande efter genomförda utvecklingsmoment.

---

## GitHub

Projektet versionshanteras med Git och GitHub.

Repositoryt hålls privat under utvecklingen.

Det ska göras publikt inför inlämningen enligt uppgiftens krav.

---

## Academic Project

**Project:** Prague Parking V2  
**Assignment:** C# Programming – Assignment 2  
**School:** KYH  
**Program:** Systemutvecklare DevSecOps  
**Academic Year:** 2026  
**Author:** Rami Issa

This project was developed as the second individual assignment
in the C# programming course at KYH.

The assignment focuses on object-oriented programming,
application architecture, data persistence, and console-based
user interfaces using C# and .NET 10.