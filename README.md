# Prague Parking V2

Prague Parking V2 är en objektorienterad vidareutveckling av Prague Parking V1.

Projektet utvecklas i C# med .NET 10 och följer en tydlig separation mellan
presentation, domänlogik och dataåtkomst.

## Projektstruktur

- `PragueParkingV2` – huvudapplikation, domänmodell och konsolgränssnitt
- `PragueParkingV2.Data` – separat klassbibliotek för konfiguration och dataåtkomst
- `docs` – kravanalys, klassdiagram, projektplan och designbeslut
- `LOGGBOK.md` – löpande dokumentation av utvecklingsarbetet

## Status

Projektet är under aktiv utveckling.

### Implementerat eller påbörjat

- objektorienterad domänmodell med bland annat:
  - `Vehicle`
  - `Car`
  - `MC`
  - `ParkingSpot`
  - `ParkingGarage`
- konfiguration via `config.json`
- validering av konfiguration vid programstart
- automatisk återskapning av saknad runtime-konfiguration
- skydd mot att en befintlig ogiltig konfiguration skrivs över automatiskt
- stöd för konfigurerbart antal parkeringsplatser
- konfigurerbara kapacitetsregler för Car och MC
- registrering av ankomsttid i UTC
- sökning efter parkerade fordon
- kontroll mot dubbel incheckning
- grundläggande parkeringslogik
- borttagning av fordon via både `ParkingSpot` och `ParkingGarage`
- prisberäkning för Car och MC via `ParkingFeeCalculator`
- debitering per påbörjad timme
- stöd för konfigurerbar gratisgräns
- validering av UTC-baserade tider vid prisberäkning
- validering av att avgångstid inte ligger före ankomsttid
- stöd för kontrollerad uppdatering av aktiva prisregler via `UpdatePricing()`
- `ParkingFeeResult` för strukturerat resultat från prisberäkning
- utcheckningslogik via `CheckoutService`
- `CheckoutResult` för strukturerat resultat från utcheckning
- verifierat checkout-flöde där fordon:
  - hittas
  - får parkeringsavgiften beräknad
  - tas bort från parkeringshuset
  - returnerar ett samlat checkout-resultat
- Spectre.Console-baserad meddelandepresentation

### Verifierade delar

Prisberäkningen har testats med flera gränsfall.

Exempel:

```text
9 min 59 sek  → 0 CZK
10 min        → 1 påbörjad timme
60 min        → 1 påbörjad timme
60 min 1 sek  → 2 påbörjade timmar
```

Ett komplett checkout-test har även genomförts där en bil parkerades,
checkades ut efter 61 minuter och därefter verifierades som borttagen från
parkeringshuset.

Med ett timpris på 20 CZK gav testet:

```text
Parkeringstid: 61 minuter
Påbörjade timmar: 2
Total avgift: 40 CZK
```

### Återstår

Följande återstår bland annat för Prague Parking 2.0:

- fullständigt användarflöde för incheckning
- fullständigt UI-flöde för utcheckning
- omläsning av prislista via meny
- flytt av fordon
- garageöversikt
- visning av ledig och upptagen kapacitet
- persistens av parkeringsdata i `parking-data.json`
- återställning av parkerade fordon vid programstart
- uppdatering av parkeringsdata vid:
  - incheckning
  - utcheckning
  - flytt
- slutlig menystruktur
- sluttest och stabilisering av Prague Parking 2.0

Efter att Prague Parking 2.0 är stabil fortsätter utvecklingen med
VG-funktionaliteten i Prague Parking 2.1.

## Arkitektur

Projektet följer principen:

```text
Presentation
    ↓
Domain
    ↓
Persistence
```

### Presentation

Ansvarar för:

- användarinteraktion
- menyer
- konsolpresentation
- Spectre.Console
- presentation av fel och resultat

### Domain

Ansvarar för:

- fordon
- parkeringsplatser
- parkeringsgarage
- parkeringsregler
- sökning
- kapacitetskontroll
- prisberäkning
- checkout-flöde

### Persistence / Data

Ansvarar för:

- läsning av konfiguration
- JSON-hantering
- framtida lagring och återställning av parkeringsdata

Presentation ska inte innehålla parkeringslogik och domänlagret ska inte
ansvara för konsolutskrift eller filhantering.

## Tidshantering

Ankomsttider lagras internt i UTC.

```text
Vehicle.ArrivalTime
        ↓
       UTC
```

Den konfigurerade tidszonen används senare när tider ska presenteras för
användaren.

Standardkonfigurationen använder:

```text
Europe/Prague
```

Detta gör att beräkningar kan ske med entydiga UTC-tider medan presentation
kan anpassas efter parkeringshusets lokala tidszon.

## Prisberäkning

Prisregler läses från `config.json` vid programstart och skickas till
`ParkingFeeCalculator`.

De aktiva reglerna hålls i minnet under körning.

För Prague Parking 2.0 används:

```text
Car → 20 CZK per påbörjad timme
MC  → 10 CZK per påbörjad timme
```

Parkering kortare än den konfigurerade gratisgränsen är kostnadsfri.

Standardvärdet är:

```text
FreeMinutes = 10
```

`ParkingFeeCalculator` returnerar ett `ParkingFeeResult` med:

- parkeringstid
- timpris
- gratisminuter
- total avgift

## Utcheckning

Utcheckningsflödet samordnas av `CheckoutService`.

```text
Identifier
    ↓
hitta fordon
    ↓
beräkna avgift
    ↓
ta bort fordon
    ↓
CheckoutResult
```

Prisberäkningen sker innan fordonet tas bort från parkeringshuset.

Om prisberäkningen misslyckas ska fordonet därför fortfarande finnas kvar i
garaget.

`CheckoutResult` innehåller:

- fordonsidentifierare
- ankomsttid
- avgångstid
- total parkeringstid
- timpris
- gratisminuter
- total avgift

Det fullständiga meny- och presentationsflödet för checkout återstår.

## Konfiguration och parkeringsdata

Konfiguration och parkeringsdata har separata ansvarsområden.

```text
config.json
    ↓
systemets regler och inställningar

parking-data.json
    ↓
aktuellt tillstånd i parkeringshuset
```

`config.json` innehåller exempelvis:

- priser
- gratisgräns
- antal parkeringsplatser
- tidszon
- kapacitetsregler

`parking-data.json` kommer att innehålla de fordon som faktiskt är
parkerade.

Prisresultat och checkout-resultat ska inte sparas i `parking-data.json`.

## Teknik

- C#
- .NET 10
- Spectre.Console
- System.Text.Json
- Git
- GitHub

## Dokumentation

Projektets dokumentation finns i `docs`:

- `requirements.md` – kravanalys och kravstatus
- `class-diagram.md` – aktuell domänmodell
- `configuration.md` – konfigurationsstruktur och hantering
- `parking-data.md` – planerad struktur för parkeringsdata
- `design-decisions.md` – viktiga arkitektur- och designbeslut
- `project-plan.md` – projektplan och milstolpar

Utvecklingsarbetet dokumenteras löpande i:

- `LOGGBOK.md`

## Versioner

### Prague Parking 2.0

G-versionen.

Fokus ligger på att färdigställa och stabilisera:

- grundläggande parkeringsfunktioner
- JSON-persistens
- konfiguration
- prisberäkning
- konsolgränssnitt
- incheckning
- utcheckning
- flytt
- sökning
- garageöversikt

### Prague Parking 2.1

VG-versionen utvecklas vidare efter att Prague Parking 2.0 fungerar stabilt.

Planerad funktionalitet omfattar bland annat:

- minst två interfaces
- Bicycle
- Bus
- storleksbaserad kapacitetsmodell
- Bicycle = 1 kapacitetsenhet
- MC = 2 kapacitetsenheter
- Car = 4 kapacitetsenheter
- Bus = 16 kapacitetsenheter
- Bus kräver fyra sammanhängande parkeringsplatser
- Bus får endast använda de första 50 parkeringsplatserna
- konfigurerbara storlekar och fordonstyper
- fullständig omläsning och validering av konfiguration under körning