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
  - identifierare
  - automatisk ankomsttid i UTC
  - validering av identifierare
  - stöd för återställning av en tidigare sparad ankomsttid
- `Car` och `MC` ärver från `Vehicle`.
- `ParkingSpot` har fått:
  - platsnummer
  - privat lista över parkerade fordon
  - `CanPark()` för kontroll av kapacitet och fordonstyp
  - `ParkVehicle()` för kontrollerad parkering
  - `FindVehicle()` för sökning på den egna parkeringsplatsen
  - `RemoveVehicle()` för borttagning av ett fordon från den egna platsen
- `ParkingGarage`:
  - skapar antal parkeringsplatser utifrån konfigurationen
  - har `FindAvailableSpot()` för att hitta första lämpliga parkeringsplats
  - har `FindVehicle()` för sökning genom hela garaget
  - har `ParkVehicle()` för att hitta en plats och parkera fordonet
  - kontrollerar att samma identifierare inte checkas in flera gånger
- Skapat `config.json` med:
  - priser
  - antal parkeringsplatser
  - tidszon
  - kapacitetsregler för Car och MC
- Skapat typad konfigurationsmodell i `AppConfiguration`.
- Implementerat `ConfigurationService` med `System.Text.Json`.
- Om runtime-versionen av `config.json` saknas skapas den med standardvärden.
- Befintlig konfiguration valideras innan den används.
- Ogiltiga priser, parkeringsantal, tidszoner och fordonsregler stoppas.
- Skapat central `MessageService` med Spectre.Console-paneler för:
  - SUCCESS
  - INFO
  - WARNING
  - ERROR
- Startup-fel hanteras kontrollerat i stället för att ge obehandlade exceptions.
- Verifierat att lösningen bygger med både huvudprojektet och `PragueParkingV2.Data`.
- Uppdaterat projektets dokumentation:
  - kravanalys
  - klassdiagram
  - konfigurationsdokumentation
  - planerad struktur för `parking-data.json`
  - designbeslut
  - projektplan
  - README

### Tester

- Testat att borttagen runtime-`config.json` återskapas automatiskt.
- Testat att standardvärden skrivs korrekt till JSON.
- Testat ogiltigt antal parkeringsplatser (`-100`).
- Verifierat att ogiltig konfiguration stoppas och visas med ett tydligt felmeddelande.
- Verifierat att 100 parkeringsplatser skapas från konfigurationen.
- Testat `FindAvailableSpot()` och `ParkVehicle()` med en bil och tre motorcyklar.
- Verifierat att en bil får en egen parkeringsplats.
- Verifierat att två MC kan dela samma parkeringsplats när `VehiclesPerSpot` är 2.
- Verifierat att en tredje MC automatiskt placeras på nästa lediga parkeringsplats.
- Verifierat att bil och MC inte blandas på samma parkeringsplats.
- Testat att `ABC123` och `abc123` behandlas som samma identifierare.
- Verifierat att dubbel incheckning stoppas innan en ny parkeringsplats tilldelas.
- Verifierat att programmet avslutas kontrollerat vid kritiska startup-fel.

### Problem och lösningar

- Under test av automatisk återskapning av konfiguration råkade projektets ursprungliga `Config/config.json` tas bort i stället för endast runtime-kopian i `bin`.
- Detta gjorde att Visual Studio inte kunde kopiera konfigurationsfilen vid build.
- Ursprungsfilen återställdes och skillnaden mellan projektets konfigurationsfil och runtime-kopian blev tydligare.
- Upptäckte även att flera dokument hade blivit inaktuella i takt med implementationen. Dessa uppdaterades så att dokumentationen åter speglar projektets aktuella tillstånd.

### Nästa steg

- Implementera `ParkingGarage.RemoveVehicle()`.
- Bygga det riktiga utcheckningsflödet.
- Implementera prisberäkning utifrån `ArrivalTime` och konfigurationen.
- Implementera flytt av fordon mellan parkeringsplatser.
- Implementera persistens i `parking-data.json`.