## 2026-10-05

### Vad jag gjorde
- Skapade solutionen PragueParkingV2 i .NET 10.
- Skapade separat klassbibliotek `PragueParkingV2.Data`.
- Lade till projektreferens från huvudprojektet till Data-projektet.
- Skapade dokumentationsstruktur med README, loggbok och docs.
- Gick igenom G-kraven och skapade en kravmatris.
- Började skissa klassdiagrammet för Vehicle, Car, MC, ParkingSpot och ParkingGarage.

### Beslut
- UI, domänlogik och dataåtkomst ska hållas separerade.
- Parkeringsdata ska sparas som JSON.
- Prislista och konfiguration ska kombineras i `config.json`.
- Priser ska ligga tidigt i konfigurationsfilen för enklare redigering.
- ParkingSpot identifieras med platsnummer.
- Motorfordon identifieras med registreringsnummer.
- Vehicle ska inte lagra sitt ParkingSpot-nummer eftersom placeringen redan kan bestämmas av vilken ParkingSpot som innehåller fordonet.

### Nästa steg
- Fortsätta utveckla klassdiagrammet.
- Bestämma klassernas properties och kapacitetsmodell.
- Därefter planera beteenden/metoder innan implementation.

## 2026-10-06

### Genomfört

- Installerat och konfigurerat Spectre.Console.
- Skapat domänklasserna `Vehicle`, `Car`, `MC`, `ParkingSpot` och `ParkingGarage`.
- `Vehicle` har fått:
  - unik identifierare
  - automatisk ankomsttid i UTC
  - validering av identifierare
  - stöd för återställning av tidigare ankomsttid
- `Car` och `MC` ärver från `Vehicle`.
- `ParkingSpot` har fått:
  - platsnummer
  - privat lista över parkerade fordon
  - `CanPark()` för kontroll av kapacitet och fordonstyp
  - `ParkVehicle()` för kontrollerad parkering
- `ParkingGarage` skapar antal parkeringsplatser utifrån konfigurationen.
- Skapat `config.json` med priser, garageinställningar, tidszon och fordonsregler.
- Skapat typad konfigurationsmodell i `AppConfiguration`.
- Implementerat `ConfigurationService` med `System.Text.Json`.
- Om `config.json` saknas skapas den automatiskt med standardvärden.
- Befintlig konfiguration valideras innan den används.
- Ogiltiga priser, parkeringsantal, tidszoner och fordonsregler stoppas.
- Skapat central `MessageService` med Spectre.Console-paneler för:
  - Error
  - Success
  - Warning
  - Info
- Startup-fel hanteras kontrollerat i stället för att ge obehandlade exceptions.
- Verifierat att lösningen bygger med två projekt och utan build-fel.

### Tester

- Testat att borttagen `config.json` återskapas automatiskt.
- Testat att standardvärden skrivs korrekt till JSON.
- Testat ogiltigt antal parkeringsplatser (`-100`).
- Verifierat att ogiltig konfiguration stoppas och visas med ett tydligt felmeddelande.
- Verifierat att 100 parkeringsplatser skapas från konfigurationen.

### Nästa steg

- Implementera `ParkingGarage.FindAvailableSpot()`.
- Koppla parkeringsregler från `config.json` till sökningen efter ledig plats.
- Fortsätta med incheckning och kontroll mot dubbelparkering. 