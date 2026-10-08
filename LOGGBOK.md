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

## 2026-10-07

### Genomfört 

- Verifierat med läraren att `Identifier` är rätt generell identifierare för fordon eftersom alla fordonstyper inte har registreringsnummer.
- Verifierat att en Bus i VG-versionen ska kräva fyra sammanhängande parkeringsplatser, motsvarande totalt 16 kapacitetsenheter.

### Reflektion

- `Vehicle` är en generell basklass för alla fordonstyper och använder därför den generella egenskapen `Identifier`.
- `Car` och `MC` ärver från `Vehicle`, och deras `Identifier` motsvarar i praktiken registreringsnumret.
- Den generella benämningen gör modellen mer flexibel eftersom framtida fordonstyper, exempelvis Bicycle, kan använda en annan typ av identifierare.
- 

## 2026-10-08

### Genomfört

- Implementerat `ParkingGarage.RemoveVehicle()`.
- Metoden söker igenom garagets parkeringsplatser och försöker ta bort fordonet från respektive `ParkingSpot`.
- Om fordonet hittas returneras det borttagna `Vehicle`-objektet.
- Om inget fordon med angiven identifierare finns returneras `null`.
- Återanvänt befintlig `ParkingSpot.RemoveVehicle()` i stället för att ge `ParkingGarage` direkt åtkomst till listan över parkerade fordon.
- Behållit inkapslingen genom att `_parkedVehicles` fortfarande endast hanteras av `ParkingSpot`.

### Tester

- Verifierat att ett parkerat Car-fordon kan tas bort via `ParkingGarage.RemoveVehicle()`.
- Verifierat att ett parkerat MC-fordon kan tas bort via samma metod.
- Kontrollerat med `FindVehicle()` efter borttagning att fordonen inte längre finns kvar i garaget.
- Verifierat att det borttagna fordonet returneras från `RemoveVehicle()`.
- Testkörningen avslutades utan fel med exit code `0`.

### Reflektion

- `ParkingGarage.RemoveVehicle()` följer samma ansvarsfördelning som övriga garagefunktioner: garaget söker bland parkeringsplatserna medan varje `ParkingSpot` ansvarar för sina egna parkerade fordon.
- Att returnera `Vehicle?` i stället för endast `bool` gör det möjligt att senare använda det borttagna fordonets information, exempelvis `ArrivalTime`, vid utcheckning och prisberäkning.

### Nästa steg

- Planera det fullständiga utcheckningsflödet.
- Definiera reglerna för prisberäkning:
  - parkering kortare än 10 minuter är gratis
  - därefter debiteras per påbörjad timme
  - Car och MC använder respektive pris från `config.json`
- Implementera prisberäkningen.
- Därefter koppla ihop prisberäkning och borttagning till ett komplett utcheckningsflöde.
- Fortsätta med flytt av fordon och senare persistens i `parking-data.json`.

