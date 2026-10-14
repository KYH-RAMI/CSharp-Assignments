# Parkeringsdata

**Uppdaterad:** 2026-10-14  
**Omfattning:** JSON-lagring för Prague Parking 2.0 (G), med avgränsningar inför Prague Parking 2.1 (VG).

**Status 2026-10-14:** `ParkingDataService` och DTO:er är implementerade. `ParkingDataMapper` i huvudprojektet konverterar nu mellan JSON och domänobjekt och återställer fordon till deras ursprungliga plats och UTC-ankomsttid. Automatisk sparning är ansluten till **Park Vehicle**. Den 14 oktober verifierades ett normalt end-to-end-flöde med två bilar efter omstart. Säkerhetstester, mer komplett validering och sparning från **Check Out** och **Move** återstår.

## Beslutad struktur

```mermaid
flowchart TD
    A[Parking Data]
    A --> B[ParkingSpots]
    B --> C[ParkingSpot]
    C --> C1[SpotNumber]
    C --> C2[Vehicles]

    C2 --> D[Vehicle]
    D --> D1[Identifier]
    D --> D2[VehicleType]
    D --> D3[ArrivalTime UTC]
```

Diagrammet visar den fastställda strukturen för G-versionen. Vi inför inte extra obligatoriska fält eller en alternativ, platt struktur under implementationen av 2.0.

## Syfte

`parking-data.json` ska innehålla det aktuella tillståndet i parkeringshuset.

Filen ska användas för att återställa parkerade fordon när programmet startas igen. Varje fordon ska återfå **samma parkeringsplats, identifierare, fordonstyp och ursprungliga ankomsttid**.

Konfigurationsdata lagras inte här utan separat i `config.json`.

Endast parkeringsplatser som innehåller fordon ska sparas i `parking-data.json`.

I det färdiga återställningsflödet ska tomma parkeringsplatser återskapas utifrån antalet platser i `config.json`. Standardkonfigurationen innehåller 100 parkeringsplatser, men det faktiska antalet bestäms av den giltiga konfigurationen. Återställning genom `ParkingDataMapper.ToGarage()` är implementerad och testad med sparade bilar.

Prisregler och gratisgränser sparas inte i `parking-data.json` utan hämtas från `config.json`. `ParkingFeeResult` och `CheckoutResult` är tillfälliga beräknings- och resultatobjekt, inte permanent parkeringsdata.

## Samband med den dynamiska garageöversikten (2026-10-13)

`GarageOverView` visar platser och fordon från `ParkingGarage`, inte direkt från `parking-data.json`. Den nya kompakta `Grid`-baserade kartan beräknar sitt utseende utifrån garagekapacitet och terminalens storlek, men ändrar inte hur parkeringsdata ska sparas.

- Garagekapaciteten styrs av `config.json` och får inte härledas från antalet poster i `parking-data.json`, eftersom datafilen endast ska innehålla **upptagna** platser.
- En garageöversikt med **200 platser har kontrollerats visuellt** i utvecklingsmiljön. Detta verifierar **inte** att 200 parkerade platser kan sparas och återställas efter omstart.
- Vid återställning används `ParkingGarage.ParkVehicleAtSpot()`, som avvisar platser som saknas i det konfigurerade garaget och tillämpar `ParkingSpot`-reglerna. Negativa testfall med felaktiga platsnummer återstår.
- Layout, färger och markering av vald plats tillhör UI-lagret och ska inte lägga till fält i G-versionens JSON-format.

## VehicleType

`VehicleType` lagras i JSON för att programmet ska kunna avgöra vilken konkret fordonsklass som ska skapas när data läses in.

Exempel:

```text
"Car" → Car
"MC"  → MC
```

`VehicleType` behöver därför inte vara en property i domänklassen `Vehicle`. Fordonstypen representeras där av den faktiska runtime-klassen.

Vid inläsning ska okända eller ännu inte stödda fordonstyper avvisas kontrollerat. Detta gäller även framtida `Bicycle` och `Bus` tills dessa faktiskt har implementerats.

## ArrivalTime

`ArrivalTime` lagras i UTC med en entydig tidsangivelse, exempelvis `2026-10-05T14:30:00Z`.

Det gör att den sparade tiden kan konverteras till den tidszon som anges i `config.json`, exempelvis `Europe/Prague`, när den visas för användaren.

Vid återställning måste den ursprungliga UTC-tiden bevaras. Ett återställt fordon får **inte** få en ny ankomsttid genom att konstruktorn använder `DateTime.UtcNow`.

`Car` och `MC` har konstruktorsöverlagringar som tar emot befintlig UTC-ankomsttid. `ParkingDataMapper.ToVehicle()` använder dessa, så att en omstart inte sätter om parkeringstiden.

## Exempel på `parking-data.json`

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
    },
    {
      "SpotNumber": 18,
      "Vehicles": [
        {
          "Identifier": "MC123",
          "VehicleType": "MC",
          "ArrivalTime": "2026-10-05T14:35:00Z"
        },
        {
          "Identifier": "MC456",
          "VehicleType": "MC",
          "ArrivalTime": "2026-10-05T14:42:00Z"
        }
      ]
    }
  ]
}
```

Exemplet följer det tidigare beslutade formatet: `ParkingSpots` innehåller `SpotNumber` och `Vehicles`, medan varje fordon innehåller `Identifier`, `VehicleType` och `ArrivalTime`. Endast upptagna platser visas i filen.

Ett tomt parkeringshus kan representeras av:

```json
{
  "ParkingSpots": []
}
```

## Inläsning och återställning vid programstart

Den implementerade ordningen vid normal uppstart är:

1. Läs och validera `config.json`.
2. Läs och validera `parking-data.json` via `ParkingDataService.Load()`; om filen saknas används ett tomt dokument i minnet.
3. Anropa `ParkingDataMapper.ToGarage(document, configuration)`, som skapar `ParkingGarage` enligt konfigurerad kapacitet.
4. För varje sparad plats och fordon: skapa `Car` eller `MC` med ursprunglig UTC-tid, hämta `VehiclesPerSpot` och anropa `ParkVehicleAtSpot()` för **exakt** platsnummer.
5. Visa huvudmenyn först när hela återställningen har slutförts utan undantag.

Återställning ska **inte** använda den ordinarie metoden som automatiskt väljer första lediga parkeringsplats. Den skulle kunna flytta fordon till andra platser vid omstart.

### Validering av sparad data

`ValidateParkingData()` i `ParkingDataService` kontrollerar redan dokument-/plats-/fordonsobjekt, icke-tomma listor för sparade platser, positiva och unika `SpotNumber`, fordonsidentifierare, dubbletter med `StringComparer.OrdinalIgnoreCase`, fordonstyperna `Car`/`MC` samt `ArrivalTime.Kind == Utc` och icke-standarddatum. Samma interna kontroll anropas vid `Load()` och `Save()`.

Följande ytterligare krav gäller innan sparad data accepteras i det färdiga garaget:

- JSON-strukturen och obligatoriska fält finns och går att läsa.
- Varje `SpotNumber` är unikt och ligger inom den aktuella garagekapaciteten.
- Varje fordonsidentifierare är giltig och unik i hela garaget, **utan skillnad mellan stora och små bokstäver**.
- `VehicleType` motsvarar en fordonstyp som den aktuella versionen stöder.
- `ArrivalTime` är en giltig UTC-tidpunkt.
- Fordonen ryms på den angivna platsen enligt gällande kapacitets- och fordonsregler.

Om en befintlig fil är felaktig eller innehåller oförenliga uppgifter ska uppstarten ge ett tydligt fel. Filen får inte tyst skrivas över och ett delvis återställt garage får inte accepteras som giltigt tillstånd.

Om `parking-data.json` saknas vid första start ska garaget kunna starta tomt. Det är en annan situation än att en **befintlig** datafil är ogiltig.

## Uppdatering vid parkering

Efter en lyckad incheckning sparas den nya beläggningen i `parking-data.json` genom `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()`. Detta är anslutet till `ParkVehicleView` och verifierat vid vanlig bilparkering den 14 oktober.

Det sparade fordonet ska innehålla:

- Dess faktiska parkeringsplats (`SpotNumber` i det omgivande platsobjektet).
- Dess `Identifier` och `VehicleType`.
- Den ursprungliga `ArrivalTime` i UTC.

Användaren får bekräftelse först när `Save()` returnerar utan rapporterat fel. Om skrivningen kastar `IOException` eller `UnauthorizedAccessException` försöker UI ångra incheckningen med `ParkingGarage.RemoveVehicle()`. Detta är **inte testat med framprovocerade skrivfel**, och fullständig transaktionssäkerhet kan därför inte utlovas.

## Uppdatering vid utcheckning

När ett fordon checkas ut ska det tas bort från det aktuella parkeringsläget.

Efter en lyckad utcheckning ska `parking-data.json` uppdateras så att det utcheckade fordonet inte längre finns kvar i den persistenta datan. Om platsen därefter är tom ska den inte längre finnas med bland de sparade `ParkingSpots`.

Prisberäkningen ska ske innan fordonet tas bort, enligt ansvaret i `CheckoutService`.

Prisberäkning, `ParkingFeeResult` och `CheckoutResult` är tillfälliga runtime-resultat och ska inte sparas i `parking-data.json`.

En utcheckning får inte presenteras som varaktigt slutförd om sparningen misslyckas.

## Uppdatering vid förflyttning

När ett fordon flyttas ska `parking-data.json` uppdateras så att det inte längre finns på den gamla parkeringsplatsen utan på den nya.

Förflyttningen ska behålla fordonets `Identifier`, `VehicleType` och ursprungliga `ArrivalTime`.

Om den gamla platsen blir tom ska den utelämnas ur den sparade filen. En misslyckad förflyttning eller sparning får inte lämna ett ofullständigt eller motsägelsefullt parkeringsläge.

## Ansvar och säker filhantering

JSON-serialisering och filåtkomst hör hemma i det separata projektet `PragueParkingV2.Data`. `ParkingGarage` och `ParkingSpot` ska fortsatt ansvara för parkeringsregler, och UI ska endast presentera resultat och fel.

Vid implementationen ska vi:

- Använda en portabel, kontrollerad lagringsplats utan hårdkodade sökvägar till en viss dator.
- Spara efter varje **genomförd förändring** av parkeringshuset: incheckning, utcheckning och förflyttning.
- Undvika att en avbruten skrivning förstör huvudfilen, exempelvis genom skrivning till temporär fil följt av säker ersättning.
- Säkerställa konsekvens mellan tillståndet i minnet och den sparade filen om en skrivning misslyckas.
- Aldrig automatiskt skriva över en befintlig, korrupt eller ogiltig datafil med ett tomt parkeringshus.

### Implementerat 2026-10-11

- `ParkingDataService` använder `AppContext.BaseDirectory` och `Path.Combine()` för körmiljöns `Data/parking-data.json`.
- `Load()` kontrollerar om filen finns, läser texten, deserialiserar och validerar dokumentet. Saknad fil ger ett tomt dokument i minnet; den skapas inte vid enbart inläsning.
- `Save()` skapar saknad datakatalog och validerar det inkommande dokumentet innan JSON serialiseras.
- `JsonSerializerOptions { WriteIndented = true }` ger indenterad och människoläsbar JSON.
- Innehållet skrivs först till `parking-data.json.tmp` i samma katalog. `File.Move()` används om huvudfilen saknas och `File.Replace()` om den finns.
- `try/finally` försöker rensa en kvarvarande temporärfil.

### Manuell teststatus (uppdaterad 2026-10-14)

- **Verifierat:** Giltig JSON läses till ett dokument med plats 1, `Car`, `ABC123` och korrekt UTC-ankomsttid.
- **Verifierat:** Okänd typ (`Truck`) avvisas med `InvalidDataException` och fångas av programmets felhantering.
- **Verifierat:** Serialiserad text är indenterad och behåller rätt identifierare, fordonstyp och UTC-tidsmarkering.
- **Verifierat 2026-10-14:** Ny bil `UKB48K` parkerades på plats 002 genom UI och sparades. Programmet avslutades och startades igen; `UKB48K` återkom på samma plats och med ankomsttid 20:53 lokal tid. Tidigare sparad bil `ABC123` återställdes på plats 001. Garaget visade `FREE 198`, `OCCUPIED 2`, `VEHICLES 2`.
- **Fortfarande ej verifierat:** Faktiskt beteende vid `IOException`/`UnauthorizedAccessException`, om `.tmp` alltid städas, samt fullständig filersättningssäkerhet vid krasch eller trasig befintlig data.

### Kvarstående säkerhet och integration

- Kontroll av att obligatoriska JSON-fält faktiskt förekommer, även om DTO:ernas standardvärden annars kan dölja utelämnade fält.
- Kontroll av identifierarlängd, normalisering och garagekapacitet i relation till `config.json`.
- Skydd mot att en redan befintlig korrupt/ogiltig fil skrivs över genom ett fristående `Save()`-anrop. Den normala uppstartssekvensen anropar redan `Load()` och stoppar vid upptäckta ogiltiga data.
- Fullständig verifiering av `Save()` med befintlig respektive saknad fil, inklusive rensning av temporärfil och hantering av skrivfel.
- Säker konsekvens mellan tillståndet i minnet och filen när lagring misslyckas.
- Testa fler återställningsfall (särskilt två MC på samma plats, ogiltiga platsnummer och kapacitetsöverskridande), och integrera automatisk sparning efter **utcheckning och förflyttning**. Incheckningens normalflöde är nu verifierat.

**Status 2026-10-14:** Komplett normalt flöde för **bilincheckning och omstartsåterställning** är verifierat. Fullständig persistens för alla användaråtgärder och felhantering återstår.

## Förberedelser inför Prague Parking 2.1 (VG)

Den nuvarande JSON-strukturen ska fungera som utgångspunkt även när `Bicycle` och `Bus` införs.

### Bicycle

`Bicycle` blir en egen fordonstyp. En cykel som saknar registreringsnummer behöver en stabil, unik `Identifier` som kan sparas och återställas på samma sätt som för övriga fordon.

### Bus

En buss motsvarar 16 storleksenheter och behöver fyra sammanhängande parkeringsplatser, där en normal plats motsvarar 4 enheter.

- **Nyparkering:** Fyra sammanhängande platser inom platsnummer 1–50.
- **Förflyttning:** En redan parkerad buss får flyttas till fyra sammanhängande platser även efter platsnummer 50.
- Bussens identifierare och ursprungliga UTC-ankomsttid ska behållas vid flytt.
- En buss ska behandlas som **ett fordon**, inte som fyra separata fordon.

Eftersom en buss använder flera platser behövs en framtida utökning av domän- och lagringsmodellen som kan bevara sambandet mellan bussen och dess fyra upptagna platser. **Den exakta JSON-representationen för buss är inte beslutad.** Vi lägger därför inte till exempelvis `OccupiedSpotNumbers` i G-versionens filformat redan nu.

VG-funktionaliteten implementeras efter att G-versionens lagring och övriga funktioner har färdigställts och verifierats.

## Relaterad dokumentation

- `configuration.md` – inställningar, priser, tidszon och kapacitetsregler.
- `class-diagram.md` – domänmodell och klassernas relationer.
- `design-decisions.md` – särskilt DD-008–DD-011, DD-023, DD-025 och DD-026.
- `requirements.md` – krav och status för G respektive VG.
