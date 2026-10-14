# Prague Parking V2 – Klassdiagram

**Uppdaterad:** 2026-10-14  
**Omfattning:** Domänmodellen för Prague Parking 2.0 (G), med avgränsningar inför Prague Parking 2.1 (VG).

Diagrammet visar domänklasserna och deras viktigaste relationer. JSON-persistensen och applikationens mapper visas separat nedan och blandas inte med domänmodellen. VG-utökningar ingår inte som implementerade klasser.

UI och konfiguration beskrivs översiktligt i detta dokument. JSON-persistensen har ett separat klassdiagram längre ned. Den 14 oktober tillkom `ParkingDataMapper` i huvudprojektets applikationslager och `ParkingGarage.ParkVehicleAtSpot()` för återställning. `GarageOverView` behöver inte ändras för detta.

## Aktuell domänmodell

```mermaid
classDiagram
    direction TB

    class Vehicle {
        <<abstract>>
        +string Identifier
        +DateTime ArrivalTime
        #Vehicle(string identifier)
        #Vehicle(string identifier, DateTime arrivalTime)
    }

    class Car {
        +Car(string identifier)
        +Car(string identifier, DateTime arrivalTime)
    }

    class MC {
        +MC(string identifier)
        +MC(string identifier, DateTime arrivalTime)
    }

    class ParkingSpot {
        +int SpotNumber
        -List~Vehicle~ _parkedVehicles
        +IReadOnlyList~Vehicle~ ParkedVehicles
        +ParkingSpot(int spotNumber)
        +bool CanPark(Vehicle vehicle, int maxVehiclesPerSpot)
        +void ParkVehicle(Vehicle vehicle, int maxVehiclesPerSpot)
        +Vehicle? FindVehicle(string identifier)
        +Vehicle? RemoveVehicle(string identifier)
    }

    class ParkingGarage {
        +IReadOnlyList~ParkingSpot~ ParkingSpots
        +ParkingGarage(int numberOfParkingSpots)
        +ParkingSpot? FindAvailableSpot(Vehicle vehicle, int maxVehiclesPerSpot)
        +ParkingSpot ParkVehicle(Vehicle vehicle, int maxVehiclesPerSpot)
        +void ParkVehicleAtSpot(Vehicle vehicle, int spotNumber, int maxVehiclesPerSpot)
        +Vehicle? FindVehicle(string identifier)
        +Vehicle? RemoveVehicle(string identifier)
    }

    class ParkingFeeCalculator {
        -decimal _carHourlyPrice
        -decimal _motorcycleHourlyPrice
        -int _freeMinutes
        +UpdatePricing(...)
        +ParkingFeeResult CalculateParkingFee(Vehicle vehicle, DateTime departureTime)
    }

    class ParkingFeeResult {
        +TimeSpan ParkingDuration
        +decimal HourlyPrice
        +int FreeMinutes
        +decimal TotalFee
    }

    class CheckoutService {
        +CheckoutResult CheckOut(string identifier, DateTime departureTime)
    }

    class CheckoutResult {
        +string Identifier
        +DateTime ArrivalTime
        +DateTime DepartureTime
        +TimeSpan ParkingDuration
        +decimal HourlyPrice
        +int FreeMinutes
        +decimal TotalFee
    }

    Vehicle <|-- Car
    Vehicle <|-- MC

    ParkingGarage "1" *-- "1..*" ParkingSpot : contains
    ParkingSpot "1" o-- "0..*" Vehicle : parks

    ParkingFeeCalculator ..> Vehicle : calculates for
    ParkingFeeCalculator ..> ParkingFeeResult : returns

    CheckoutService ..> ParkingGarage : searches/removes
    CheckoutService ..> ParkingFeeCalculator : calculates
    CheckoutService ..> CheckoutResult : returns
```

**Läsanvisning:** `+` markerar publika medlemmar, `-` privata medlemmar och `#` skyddade konstruktorer. `?` markerar att ett returvärde kan vara `null`. Diagrammets prisberäknings- och utcheckningsdelar återger de tidigare dokumenterade metodsignaturerna; de har inte kontrollerats mot nya källkodsversioner i denna dokumentationsuppdatering.

## Modellens ansvar

### Vehicle

`Vehicle` är en abstrakt basklass som innehåller de gemensamma egenskaperna:

- `Identifier`
- `ArrivalTime`

Alla fordon identifieras med en generell identifierare. Detta gör modellen mer flexibel inför framtida fordonstyper som inte nödvändigtvis har registreringsnummer.

Konstruktorn validerar identifieraren och ankomsttiden. Vid nyregistrering används `DateTime.UtcNow`, och ankomsttiden lagras internt i UTC.

Basklassen har även en skyddad konstruktor som tar emot en befintlig UTC-ankomsttid. Den är avsedd att användas när ett parkerat fordon återställs från JSON. Konstruktorsöverlagringar i `Car` och `MC` tar emot en befintlig UTC-ankomsttid och används nu i `ParkingDataMapper.ToVehicle()` vid återställning.

### Car

`Car` ärver från `Vehicle`.

Klassen representerar en bil och använder basklassens validering. Den kan skapas med antingen en ny `identifier` eller en befintlig `identifier` och UTC-ankomsttid för återställning.

### MC

`MC` ärver från `Vehicle`.

Klassen representerar en motorcykel och använder basklassens validering. Den kan skapas med antingen en ny `identifier` eller en befintlig `identifier` och UTC-ankomsttid för återställning.

### ParkingSpot

`ParkingSpot` representerar en individuell parkeringsplats.

Klassen ansvarar för:

- Parkeringsplatsens nummer.
- Intern lagring av parkerade fordon.
- Kontroll av parkeringskapacitet.
- Kontroll av tillåtna fordonstyper.
- Parkering av fordon.
- Sökning efter fordon.
- Borttagning av fordon.

Fordon lagras internt i `_parkedVehicles` och samlingen exponeras genom `IReadOnlyList<Vehicle> ParkedVehicles`.

Det innebär att andra klasser får läsåtkomst genom denna egenskap utan att kunna lägga till eller ta bort fordon via just det gränssnittet. `IReadOnlyList<T>` gör däremot inte den bakomliggande listan automatiskt oföränderlig.

### ParkingGarage

`ParkingGarage` representerar hela parkeringshuset.

Klassen ansvarar för:

- Att skapa parkeringsplatser.
- Att hålla reda på garagets parkeringsplatser.
- Att hitta första lämpliga parkeringsplats.
- Att parkera fordon.
- Att söka efter fordon.
- Att förhindra dubbla fordonsidentifierare.
- Att ta bort fordon.

Garaget använder `ParkingSpot` för att utföra åtgärder på enskilda parkeringsplatser.

`ParkVehicle()` returnerar den parkeringsplats som tilldelades. `FindAvailableSpot()`, `FindVehicle()` och `RemoveVehicle()` kan returnera `null` när ingen träff finns.

Vid vanlig incheckning väljer `ParkVehicle()` första lämpliga platsen. Vid framtida inläsning från JSON måste en separat återställningsväg kunna bevara **ursprungligt platsnummer** i stället för att placera om fordonet automatiskt.

### ParkingFeeCalculator

`ParkingFeeCalculator` ansvarar för prisberäkningen.

Klassen håller reda på aktiva prisvärden för:

- Car.
- MC.
- Gratisminuter.

Prisreglerna kan uppdateras med `UpdatePricing()`. Metoden validerar nya prisvärden innan de aktiveras.

Avgiften beräknas med `CalculateParkingFee()` utifrån:

- Fordonstyp.
- Ankomsttid.
- Avgångstid.
- Aktuellt timpris.
- Gratisgräns.
- Påbörjade timmar.

Resultatet returneras som ett `ParkingFeeResult`.

### ParkingFeeResult

`ParkingFeeResult` är ett resultatobjekt.

Klassen innehåller:

- `ParkingDuration`
- `HourlyPrice`
- `FreeMinutes`
- `TotalFee`

Resultatobjektet gör att prisberäkningen kan hållas separat från användargränssnittet.

### CheckoutService

`CheckoutService` ansvarar för själva utcheckningsprocessen.

Klassen samordnar:

1. Sökning efter fordon.
2. Prisberäkning.
3. Borttagning av fordon.
4. Skapande av ett utcheckningsresultat.

Prisberäkningen genomförs innan fordonet tas bort från parkeringshuset. Det förhindrar att ett fordon tas bort innan avgiften har kunnat beräknas korrekt.

Det fullständiga utcheckningsflödet via UI och sparande efter utcheckning återstår att integrera och verifiera.

### CheckoutResult

`CheckoutResult` innehåller information som behövs efter en lyckad utcheckning:

- Fordonsidentifierare.
- Ankomsttid.
- Avgångstid.
- Total parkeringstid.
- Timpris.
- Gratisminuter.
- Total avgift.

Objektet används för att lämna information från domänlagret till användargränssnittet. Det är inte avsett att lagras som permanent parkeringsdata.

## Relationer mellan klasser

### Arv

`Car` och `MC` ärver från den abstrakta basklassen `Vehicle`.

### ParkingGarage och ParkingSpot

`ParkingGarage` innehåller en samling `ParkingSpot`-objekt.

### ParkingSpot och Vehicle

Varje `ParkingSpot` kan innehålla noll eller flera fordon, inom de kapacitets- och fordonsregler som gäller för platsen.

### ParkingFeeCalculator och ParkingFeeResult

`ParkingFeeCalculator` använder ett fordons uppgifter för att beräkna avgiften och returnerar ett `ParkingFeeResult`.

### CheckoutService och CheckoutResult

`CheckoutService` använder `ParkingGarage` och `ParkingFeeCalculator` för att genomföra utcheckningen.

Efter en lyckad utcheckning returneras ett `CheckoutResult`.

## Arkitektoniska avgränsningar

### Användargränssnitt

Följande klasser tillhör UI och ingår inte i domänklassdiagrammet:

- `AppTheme` – gemensamma färger, menyer och presentationskomponenter.
- `MainMenu` – huvudmenyn och navigering mellan funktioner.
- `ParkVehicleView` – användarflöde för fordonsincheckning.
- `GarageOverView` – dynamisk parkeringskarta, detaljer för vald plats och sammanfattande garagestatus.
- `MessageService` – informations-, varnings-, fel- och bekräftelsemeddelanden.

UI-komponenterna har refaktorerats för att minska duplicerad kod, bland annat genom gemensamma panelkomponenter i `AppTheme`.

**Garageöversiktens aktuella presentationslösning (2026-10-13):** `GarageOverView` använder ett kompakt `Spectre.Console.Grid` inuti en gemensam `Panel` för parkeringskartan i stället för en `Table` eller en egen `Panel` per parkeringsplats. Kartan och `SPOT DETAILS` ligger sida vid sida; målet är ungefär 80/20-fördelning av den tillgängliga bredden, med ett minsta läsbart utrymme för detaljpanelen. `GARAGE STATUS` visas under båda panelerna.

Antalet kolumner och synliga rader beräknas utifrån terminalens storlek och garagets faktiska antal platser. En intern visningsyta används när hela parkeringskartan inte får plats på höjden. Piltangenterna använder samma kolumnberäkning som rutnätet, och `AnsiConsole.Live()` uppdaterar vyn. Parkeringsplatsens bakgrundsfärg uttrycker ledig, delvis upptagen eller full plats enligt konfigurationen; en markerad plats får en avvikande textfärg utan att statusbakgrunden byts ut. Presentationen hämtar information från befintliga `ParkingGarage`, `ParkingSpot` och `Vehicle`, utan att införa nya domänklasser.

**Verifieringsstatus:** Den 14 oktober visades två fulla bilplatser (001 och 002) bland 200 konfigurerade platser, samt korrekt detaljpanel och statusräknare (`FREE 198`, `OCCUPIED 2`, `VEHICLES 2`). Navigeringens gränsfall, orange MC-status, 500–1 000 platser och terminalstorleksändringar återstår att verifiera.

Parkeringslogiken har tidigare testats direkt från `Program.cs`. Incheckning via `ParkVehicleView` har nu testats tillsammans med JSON-sparning och omstart.

### Konfiguration

Konfigurationshanteringen finns i det separata Data-projektet och omfattar bland annat:

- `AppConfiguration`
- `ConfigurationService`

`config.json` styr bland annat antal platser, tidszon, priser och fordonskapacitet. Domänmodellen får relevanta kapacitetsvärden som parametrar i stället för att läsa filen direkt.

### Persistens

Följande klasser är implementerade i det separata klassbiblioteket `PragueParkingV2.Data/Persistence`. Diagrammet nedan visar DTO-/filnivån. `ParkingDataMapper` visas i ett separat diagram eftersom den ligger i huvudprojektets `Application`-lager.

```mermaid
classDiagram
    direction TB

    class ParkingDataDocument {
        +List~ParkingSpotData~ ParkingSpots
    }

    class ParkingSpotData {
        +int SpotNumber
        +List~VehicleData~ Vehicles
    }

    class VehicleData {
        +string Identifier
        +string VehicleType
        +DateTime ArrivalTime
    }

    class ParkingDataService {
        +bool ParkingDataExists()
        +ParkingDataDocument Load()
        +void Save(ParkingDataDocument parkingData)
    }

    ParkingDataDocument "1" *-- "0..*" ParkingSpotData
    ParkingSpotData "1" *-- "0..*" VehicleData
    ParkingDataService ..> ParkingDataDocument : reads/writes
```

`ParkingDataService` läser och sparar `Data/parking-data.json`, validerar den interna dokumentstrukturen och kan skriva indenterad JSON via `WriteIndented = true`. Kontroller av dubbletter använder `HashSet<int>` respektive ett skiftlägesokänsligt `HashSet<string>`.

Grundläggande JSON-inläsning och avvisning av okänd fordonstyp har tidigare testats. Den 14 oktober verifierades dessutom ett komplett normalflöde för bil: ny parkering, JSON-sparning, omstart och återställning till samma plats och ankomsttid. Fullständiga feltester och återställning av två MC på samma plats återstår.

Konfigurationsberoende kontroller hör hemma i återställningsflödet utanför Data-projektet. En befintlig ogiltig datafil får inte automatiskt ersättas med tom data.

### Applikationslager – mappning och startup/incheckning (2026-10-14)

```mermaid
classDiagram
    class ParkingDataMapper {
        <<static>>
        +Vehicle ToVehicle(VehicleData data)
        +VehicleData ToVehicleData(Vehicle vehicle)
        +ParkingDataDocument ToDocument(ParkingGarage garage)
        +ParkingGarage ToGarage(ParkingDataDocument document, AppConfiguration configuration)
    }

    ParkingDataMapper ..> ParkingGarage : reads / constructs
    ParkingDataMapper ..> ParkingDataDocument : maps
    ParkingDataMapper ..> VehicleData : converts
    ParkingDataMapper ..> AppConfiguration : reads capacity
```

`Program.cs` läser konfiguration och DTO-dokument, sedan anropas `ParkingDataMapper.ToGarage()`. `ParkVehicleView` anropar `ToDocument()` följt av `ParkingDataService.Save()` efter incheckning. Två nästlade `foreach`-loopar används avsiktligt i `ToGarage()` eftersom varje fordon skapas och parkeras på en bestämd plats. `ToDocument()` använder LINQ för projektion till DTO:er. Det här valet prioriterar läsbarhet framför att använda LINQ överallt.

**Avgränsning:** Mappern är en översättningsklass, inte en transaktionshanterare. Skrivfelkompensation ligger för närvarande i incheckningsvyn och behöver robusthetstestas och eventuellt flyttas till en central applikationstjänst.

### VG-versionen

Följande tillhör den framtida Prague Parking 2.1-versionen:

- `Bicycle` och `Bus` som nya fordonstyper.
- Minst två interfaces enligt VG-kraven. Användning av `IReadOnlyList<T>` dokumenterar samlingsåtkomst men ersätter inte automatiskt den planerade interfacebaserade domänarkitekturen.
- Storleksbaserad parkeringskapacitet: Bicycle = 1, MC = 2, Car = 4 och Bus = 16 enheter; en normal plats motsvarar 4 enheter.
- Fyra sammanhängande parkeringsplatser för en buss.
- **Vid nyparkering** tilldelas en buss fyra sammanhängande platser inom de första 50 parkeringsplatserna.
- **Vid förflyttning** får en redan parkerad buss flyttas även bortom plats 50, så länge den nya placeringen består av fyra sammanhängande lediga platser.
- Omläsning och validering av hela konfigurationen under körning.

Den planerade persistensen behöver kunna utökas så att en buss kan kopplas till flera upptagna platser utan att samma fordon behandlas som flera separata fordon. Detaljerna för den framtida JSON-representationen är inte fastställda här; formatet i `parking-data.md` gäller för 2.0.

VG-funktionaliteten implementeras först efter att G-versionen har färdigställts och verifierats.
