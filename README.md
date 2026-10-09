# Prague Parking V2

**Prague Parking 2.0 (G)** – objektorienterat parkeringssystem i C#/.NET 10 med ett konsolgränssnitt byggt med Spectre.Console.

> **Projektstatus 2026-10-09:** Arbete pågår. Domänmodellen, konfigurationsinläsning, prisberäkning och domänlogik för utcheckning finns. En interaktiv garageöversikt är påbörjad. Det fullständiga användarflödet och permanent lagring av parkerade fordon är ännu inte färdiga.

## Översikt

Systemet är tänkt att kunna checka in, söka efter, flytta och checka ut fordon. Bil (`Car`) och motorcykel (`MC`) stöds i G-versionens domänmodell. Antalet parkeringsplatser och antalet fordon per plats hämtas från JSON-konfigurationen.

En motorcykel kan dela plats med en annan motorcykel när `VehiclesPerSpot` är 2. Bilar och motorcyklar blandas inte på samma plats. Garagekapaciteten kan ändras genom konfigurationen.

## Teknik

- C# och .NET 10.
- Visual Studio Community 2026.
- Spectre.Console för konsolpresentation.
- `System.Text.Json` för konfiguration.
- Git och GitHub för versionshantering.

## Lösning och struktur

- `PragueParkingV2` – huvudprojekt med `Program.cs`, domänmodellen och användargränssnittet.
- `PragueParkingV2.Data` – separat klassbibliotek med konfigurationsmodellen och `ConfigurationService`; parkeringspersistens planeras här.
- `PragueParkingV2/Domain` – fordon, parkeringsplatser, garage, avgiftsberäkning och utcheckningslogik.
- `PragueParkingV2/UI` – `MessageService` och `GarageOverView`.
- `PragueParkingV2/Config/config.json` – projektets JSON-konfiguration, som kopieras till körmiljön.
- `docs` – krav, designbeslut, klassdiagram, konfiguration och planerad parkeringsdata.
- `LOGGBOK.md` – löpande utvecklingslogg.

## Implementerat

### Domänmodell

- Abstrakt `Vehicle` med `Identifier` och UTC-baserad `ArrivalTime`.
- `Car` och `MC` ärver från `Vehicle`.
- `ParkingSpot` hanterar fordon på en plats och hindrar ogiltiga kombinationer.
- `ParkingGarage` skapar platser, hittar första lediga platsen, parkerar, söker och tar bort fordon.
- Dubblettkontroll av fordonsidentifierare utan skillnad på versaler/gemener.

### Konfiguration

- `ConfigurationService` läser och validerar `config.json`.
- Saknad runtime-konfiguration återskapas med standardvärden.
- Ogiltig befintlig konfiguration avvisas med felmeddelande.
- Konfiguration innehåller priser, gratisminuter, antal platser, tidszon och fordonskapacitet.

### Prisberäkning och utcheckning

- `ParkingFeeCalculator` beräknar avgifter efter påbörjad timme.
- Parkering kortare än gratisgränsen debiteras inte.
- `UpdatePricing()` kan ändra aktiva priser i befintlig kalkylatorinstans; omläsning från meny återstår.
- `ParkingFeeResult` återger resultatet av prisberäkningen.
- `CheckoutService` samordnar sökning, avgiftsberäkning och borttagning.
- `CheckoutResult` lämnar strukturerade utcheckningsuppgifter till kommande UI-flöde.

### Användargränssnitt

- `MessageService` visar tydliga paneler för information, varningar, fel och lyckade åtgärder.
- `GarageOverView` visar en parkeringskarta med `Grid` och individuella `Panel`-rutor.
- Fem kolumner och högst fem rader per sida, totalt upp till 25 platser per sida.
- Dynamiskt sidantal baserat på faktiskt antal parkeringsplatser.
- Navigering med piltangenter, sidbyte med Page Up/Page Down samt ESC för att lämna vyn.
- Aktiv ruta markeras med `LightGoldenrod1`; övriga ramar är vita.
- `FREE` är grönt, delvis upptagen plats orange och full plats röd enligt presentationslogiken.
- `SPOT DETAILS` visar en grundläggande sammanställning för vald plats.
- Sidfot med sidnummer, platsintervall, totalt antal platser och tangentkommandon.

## Starta projektet

1. Öppna solutionen `PragueParkingV2` i Visual Studio Community 2026 med .NET 10 installerat.
2. Välj konsolprojektet `PragueParkingV2` som startprojekt.
3. Kontrollera att `Config/config.json` ingår i projektet och kopieras till utmatningskatalogen.
4. Bygg lösningen och starta projektet.

Konfigurationsfilen läses från `Config/config.json` under programmets körkatalog (`AppContext.BaseDirectory`). Vid nuvarande start visas garageöversikten; något komplett huvudmenysystem finns ännu inte.

## Grundinställningar

| Inställning | Standardvärde |
| --- | --- |
| Antal parkeringsplatser | 100 |
| Gratis parkering | Kortare än 10 minuter |
| Bil per påbörjad timme | 20 CZK |
| MC per påbörjad timme | 10 CZK |
| Bilar per parkeringsplats | 1 |
| MC per parkeringsplats | 2 |
| Tidszon för presentation | `Europe/Prague` |

Internt används UTC för ankomsttid och prisberäkning. Mer information finns i `docs/configuration.md`.

## Tester och återstående arbete

Manuella tester av domänlogik, konfiguration, prisberäkning och checkout har dokumenterats i `LOGGBOK.md`. Garageöversikten är visuellt kontrollerad med ett tomt garage, men behöver testas med parkerade fordon, särskilt delvis upptagna MC-platser.

Kvar i G-versionen:

- Fullständigt användarflöde för incheckning i UI.
- Fullständigt användarflöde för utcheckning i UI.
- Flyttfunktion och tillhörande UI.
- Permanent sparande och återläsning av parkeringsdata i `parking-data.json`.
- Prisomläsning från `config.json` via ett menyval.
- Utökad detaljpanel i garageöversikten med fordonstyp, identifierare och ankomsttid.
- Sluttester av beläggningsfärger, navigeringsgränser och olika terminalstorlekar.
- Slutlig dokumentation och leverans.

Den befintliga UI-etiketten `Vehicle ID` i detaljpanelen visar för tillfället ett antal och behöver korrigeras när panelen färdigställs.

## VG – separat fortsättning

Efter färdig G-version planeras Prague Parking 2.1 i ett separat projekt i samma solution. Den versionen tillför bland annat minst två interfaces, `Bicycle`, `Bus`, storleksbaserad kapacitet, fyra angränsande platser för buss inom de första 50 platserna samt omläsning och validering av hela konfigurationen. VG är ännu inte implementerad.

## Dokumentation

- `docs/requirements.md` – kravmatris och verifieringsstatus.
- `docs/class-diagram.md` – aktuell domänmodell i Mermaid.
- `docs/design-decisions.md` – dokumenterade designval.
- `docs/project-plan.md` – genomförande och nästa steg.
- `docs/configuration.md` – konfigurationsformat och validering.
- `docs/parking-data.md` – planerad parkeringsdata.
- `LOGGBOK.md` – utvecklingsdagbok med tester och reflektioner.

## GitHub och inlämning

Projektet versionshanteras med Git. Repositoryt hålls privat under utvecklingen och ska vara publikt vid inlämningen enligt uppgiftskraven. Inlämningen ska länka till hela repositoryt.