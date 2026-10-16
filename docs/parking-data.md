# Parkeringsdata

**Uppdaterad:** 2026-10-16  
**Omfattning:** JSON-lagring för Prague Parking 2.0 (G), med avgränsningar inför Prague Parking 2.1 (VG).

**Status 2026-10-16:** `ParkingDataService`, DTO:er och `ParkingDataMapper` hanterar JSON-inläsning, återställning och automatisk sparning vid **Park Vehicle**, **Check Out Vehicle** och **Move Vehicle**. Normalflödena har verifierats med omstart. Den 16 oktober verifierades även återställning av två MC på plats 003, avvisning av korrupt och logiskt ogiltig JSON samt rollback i minnet vid simulerat skrivfel i samtliga tre åtgärder. Normal sparning fungerade därefter igen: `ABC123` återfanns på plats 051 efter omstart. **Detta är manuella tester av avgränsade scenarier, inte en garanti för fullständig transaktions- eller kraschsäkerhet.**

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

Tomma parkeringsplatser återskapas utifrån antalet platser i `config.json`. Standardkonfigurationen innehåller **100 parkeringsplatser**, men det faktiska antalet bestäms av den giltiga konfigurationen. Återställning genom `ParkingDataMapper.ToGarage()` har verifierats med sparade bilar och två MC på samma plats efter omstart. Under testning har garaget växlats mellan 100, 200 och 1 000 platser utan att fordon på giltiga platsnummer har tappats bort.

Prisregler och gratisgränser sparas inte i `parking-data.json` utan hämtas från `config.json`. `ParkingFeeResult` och `CheckoutResult` är tillfälliga beräknings- och resultatobjekt, inte permanent parkeringsdata.

## Samband med den dynamiska garageöversikten (2026-10-13)

`GarageOverView` visar platser och fordon från `ParkingGarage`, inte direkt från `parking-data.json`. Den nya kompakta `Grid`-baserade kartan beräknar sitt utseende utifrån garagekapacitet och terminalens storlek, men ändrar inte hur parkeringsdata ska sparas.

- Garagekapaciteten styrs av `config.json` och får inte härledas från antalet poster i `parking-data.json`, eftersom datafilen endast ska innehålla **upptagna** platser.
- Garageöversikten har manuellt testats med **100, 200 och 1 000 konfigurerade platser**, inklusive intern viewport och navigering till sista platsen. Detta visar **inte** att 1 000 upptagna platser har sparats och återställts.
- Vid återställning validerar `ParkingDataMapper.ToGarage()` att ett sparat platsnummer ligger inom det konfigurerade garaget. Test med plats **201 i ett garage med 200 platser** avvisades med ett specifikt `InvalidDataException`-meddelande.
- Därefter använder mappningen `ParkingGarage.ParkVehicleAtSpot()` och `ParkingSpot`-reglerna. Tester med **tre MC på en plats för två** och **Car + MC på samma plats** avvisades under uppstart.
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

`Car` och `MC` har konstruktorsöverlagringar som tar emot befintlig UTC-ankomsttid. `ParkingDataMapper.ToVehicle()` använder dessa, så att en omstart inte sätter om parkeringstiden. Den 16 oktober återställdes `DEF123` och `EDF123` på plats 003 med tidigare sparade ankomsttider (visade lokalt som 15:50 respektive 16:11 den 15 oktober).

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

`ValidateParkingData()` i `ParkingDataService` kontrollerar dokument-/plats-/fordonsobjekt, icke-tomma listor för sparade platser, positiva och unika `SpotNumber`, fordonsidentifierare, dubbletter med `StringComparer.OrdinalIgnoreCase`, fordonstyperna `Car`/`MC` samt `ArrivalTime.Kind == Utc` och icke-standarddatum. Samma interna kontroll anropas vid `Load()` och `Save()`.

Vid återställning validerar `ParkingDataMapper.ToGarage()` också **platsnummer mot aktuell garagekapacitet**. Fel som uppstår när `ParkingGarage.ParkVehicleAtSpot()` inte kan placera ett sparat fordon fångas som `InvalidOperationException` och omvandlas till `InvalidDataException` med fordonsidentifierare och platsnummer. `Program.cs` stoppar då uppstarten kontrollerat och visar ett begripligt felmeddelande.

Följande ytterligare krav gäller innan sparad data accepteras i det färdiga garaget:

- JSON-strukturen och obligatoriska fält finns och går att läsa.
- Varje `SpotNumber` är unikt och ligger inom den aktuella garagekapaciteten.
- Varje fordonsidentifierare är giltig och unik i hela garaget, **utan skillnad mellan stora och små bokstäver**.
- `VehicleType` motsvarar en fordonstyp som den aktuella versionen stöder.
- `ArrivalTime` är en giltig UTC-tidpunkt.
- Fordonen ryms på den angivna platsen enligt gällande kapacitets- och fordonsregler.

Om en befintlig fil är felaktig eller innehåller oförenliga uppgifter ska uppstarten ge ett tydligt fel. Filen får inte tyst skrivas över och ett delvis återställt garage får inte accepteras som giltigt tillstånd. Den 16 oktober verifierades detta för **trasig JSON-syntax, skiftlägesokänsliga dubbletter, platsnummer utanför intervallet, kapacitetsöverskridande samt otillåten Car/MC-blandning**. Den testade ogiltiga filen låg kvar efter nekad uppstart. Det bevisar inte att ett fristående `Save()` i andra körvägar alltid vägrar ersätta en korrupt befintlig fil.

Om `parking-data.json` saknas vid första start ska garaget kunna starta tomt. Det är en annan situation än att en **befintlig** datafil är ogiltig.

## Uppdatering vid parkering

Efter en lyckad incheckning sparas den nya beläggningen i `parking-data.json` genom `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()`. Detta är anslutet till `ParkVehicleView` och verifierat vid vanlig bilparkering den 14 oktober. Vyn visar ett lyckat resultat först efter att sparningen har slutförts utan rapporterat fel.

Det sparade fordonet ska innehålla:

- Dess faktiska parkeringsplats (`SpotNumber` i det omgivande platsobjektet).
- Dess `Identifier` och `VehicleType`.
- Den ursprungliga `ArrivalTime` i UTC.

Användaren får bekräftelse först när `Save()` returnerar utan rapporterat fel. Om skrivningen kastar `IOException` eller `UnauthorizedAccessException` försöker UI ångra incheckningen med `ParkingGarage.RemoveVehicle()`. **Verifierat 2026-10-16:** En mapp med namnet `parking-data.json.tmp` blockerade temporärfilens sökväg och framkallade ett skrivfel. Parkeringen av testbilen `ROLL123` avbröts och fordonet kunde inte längre hittas i garaget. Testet visar att kompensationen i minnet fungerar i detta scenario; fullständig transaktionssäkerhet kan inte utlovas.

## Uppdatering vid utcheckning

**Implementerat och manuellt verifierat i normalflödet 2026-10-15.** `CheckOutVehicleView` använder `CheckoutService` för att beräkna avgift och ta bort ett utcheckat fordon ur garaget. Därefter anropas `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()`. Bekräftelse på slutförd utcheckning visas först efter att sparningen rapporterat framgång.

Om parkeringsplatsen blir tom utelämnas den ur sparade `ParkingSpots`. Ett manuellt test visade att utcheckade `UKB48K` inte återkom efter omstart medan en annan parkerad bil fanns kvar.

Vid `IOException` eller `UnauthorizedAccessException` under sparning försöker vyn lägga tillbaka **samma fordonsobjekt** på dess ursprungliga plats, så att `ArrivalTime` behålls i objektet. **Verifierat 2026-10-16:** När den blockerade temporärfilens sökväg orsakade skrivfel visades ett felmeddelande och `ABC123` återfanns fortfarande på plats 050. Ankomsttiden jämfördes inte separat under just rollback-testet. Kompensationen är ingen fullständig transaktionsgaranti.

Prisberäkningen sker före borttagningen. `ParkingFeeResult` och `CheckoutResult` är tillfälliga runtime-resultat och sparas inte i `parking-data.json`.

## Uppdatering vid förflyttning

**Implementerat och manuellt verifierat i normalflödet 2026-10-15.** `MoveVehicleView` anropar `ParkingGarage.MoveVehicle()`, exporterar sedan garagets nya tillstånd med `ParkingDataMapper.ToDocument()` och sparar via `ParkingDataService.Save()`. Först därefter visas den gröna bekräftelsepanelen.

Domänmetoden letar upp fordonets ursprungsplats och destinationsplats, nekar flytt till samma plats och kontrollerar destinationens kapacitet och fordonstyp **innan fordonet tas bort**. Samma `Vehicle`-objekt flyttas, vilket bevarar `Identifier`, `VehicleType` och den ursprungliga `ArrivalTime`. Om den gamla platsen blir tom utelämnas den ur den sparade filen.

Den 15 oktober flyttades bilen `ABC123` från plats **001 till 050**. Efter omstart kvarstod placeringen på plats 050. Den 16 oktober återställdes också två MC (`DEF123` och `EDF123`) korrekt på plats **003** efter omstart. Efter skrivfelstesterna kunde `ABC123` flyttas från **050 till 051**; vid nästa programstart låg den kvar på **051**, med ett konfigurerat garage på 100 platser.

Vid `IOException` eller `UnauthorizedAccessException` under sparning försöker vyn flytta tillbaka fordonet till ursprungsplatsen. Domänmetoden har även ett återställningsförsök om parkering på destinationsplatsen skulle kasta ett undantag. **Verifierat 2026-10-16 för skrivfel i `Save()`:** En bekräftad flytt av `ABC123` från **050 till 051** kunde inte sparas eftersom temporärfilens sökväg blockerades. Programmet rapporterade återflytt och fordonet fanns kvar på **050**; plats **051** var ledig och beläggningsräknarna visade fortfarande tre fordon på två platser. Den separata domänåterställningen vid ett undantag under själva destinationsparkeringen är däremot inte felinjiceringstestad. Detta är ingen garanti för alla tänkbara fel.

## Ansvar och säker filhantering

JSON-serialisering och filåtkomst hör hemma i det separata projektet `PragueParkingV2.Data`. `ParkingGarage` och `ParkingSpot` ska fortsatt ansvara för parkeringsregler, och UI ska endast presentera resultat och fel.

Principer för implementation och fortsatt verifiering:

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
- `try/finally` försöker rensa en kvarvarande temporärfil. Att städningen alltid lyckas vid alla typer av filfel eller processavbrott är **inte** verifierat.
- Vid det manuella skrivfelstestet skapades en **katalog** med namnet `parking-data.json.tmp` för att blockera `File.WriteAllText()`. Hindret togs därefter bort och normal sparning verifierades. Testet undersökte inte fullständig kraschsäkerhet eller alla fel i `File.Replace()`.

### Manuell teststatus (uppdaterad 2026-10-16)

**Tidigare verifiering, 11–15 oktober:**

- Giltig JSON läses och återställs med rätt fordonstyp, plats och UTC-ankomsttid. Okänd fordonstyp (`Truck`) avvisas.
- Incheckning av `UKB48K`, utcheckning med sparad borttagning och flytt av `ABC123` från 001 till 050 verifierades genom omstart.
- Två MC kunde dela en plats. Flytt till en plats med otillåten fordonstyp eller utan kapacitet avvisades.

**Ytterligare manuella tester den 16 oktober:**

| Testfall | Genomförd kontroll | Resultat |
|---|---|---|
| Två MC efter omstart | `DEF123` och `EDF123` återkom på plats 003 med tidigare ankomsttider; `ABC123` återkom på dåvarande plats 050. | Godkänt |
| Korrupt JSON | Avsiktligt trasig JSON stoppade uppstarten med felkod 1; befintlig fil lämnades orörd. | Godkänt |
| Dubblett i giltig JSON | `ABC123` och `abc123` avvisades som samma identifierare. | Godkänt |
| Plats utanför garaget | Sparad plats 201 avvisades när `config.json` angav 200 platser. Specifikt felmeddelande infördes i mappern. | Godkänt |
| Kapacitetsöverskridande | Tre MC på plats 003 avvisades när kapaciteten var två. | Godkänt |
| Blandade fordonstyper | `Car` och `MC` på plats 003 avvisades vid återställning. | Godkänt |
| Skrivfel – Park Vehicle | Temporärfilens sökväg blockerades; `ROLL123` registrerades inte permanent och kunde inte hittas i minnet efteråt. | Godkänt i testat scenario |
| Skrivfel – Check Out Vehicle | Sparfel visades, utcheckning avbröts och `ABC123` fanns kvar på plats 050. | Godkänt i testat scenario |
| Skrivfel – Move Vehicle | Flytt 050 → 051 kunde inte sparas; `ABC123` återfanns på 050 och plats 051 var ledig. | Godkänt i testat scenario |
| Normal drift efter skrivfel | Tillfälligt skrivhinder togs bort; flytt 050 → 051 lyckades och 051 kvarstod efter omstart. | Godkänt |
| Ändrat antal platser | 100, 200 och 1 000 platser testades; befintliga fordon på giltiga platser återställdes. | Godkänt i testade fall |

Vid dessa negativa uppstartstester kontrollerades att den ogiltiga JSON-filen inte skrevs över. **Skrivfelstesterna visar återställning i minnet vid ett bestämt, tidigt filskrivningsfel**, men innebär inte att alla steg i filersättningen eller varje möjligt kraschscenario är verifierade.

### Kvarstående säkerhet och integration

- Kontrollera JSON med **utelämnade obligatoriska fält** som DTO-standardvärden eventuellt döljer, samt identifierarlängd och vissa normaliseringsfall.
- Pröva ogiltiga platsnummer, kapacitetsändringar och befintliga fordon tillsammans med andra kombinationer av `config.json` och datafilen där det är relevant.
- Kontrollera skydd mot överskrivning av en redan korrupt fil även vid fristående anrop av `Save()` (uppstart med korrupt fil har redan testats).
- Verifiera temporärfilens städning och filersättning i fler fel- och avbrottsscenarier, inklusive eventuellt fel under `File.Replace()` och processkrasch. Inga garantier om fullständig transaktionssäkerhet görs.
- Verifiera konsekvens mellan disk och minne vid andra skrivfel än det testade hindret mot `parking-data.json.tmp`.
- Testa körning på en annan dator och gör slutlig kravavstämning inför den gemensamma inlämningen av 2.0 och 2.1.

**Status 2026-10-16:** De viktigaste normala persistensflödena, återställning av två MC efter omstart, flera ogiltiga JSON-scenarier och rollback vid ett framprovocerat skrivfel har verifierats manuellt. Ytterligare robusthets- och kompatibilitetstester återstår; systemet har inte genomgått fullständig automatisk eller kraschbaserad verifiering.

## Förberedelser inför Prague Parking 2.1 (VG)

Den nuvarande JSON-strukturen ska fungera som utgångspunkt även när `Bicycle` och `Bus` införs.

### Bicycle

`Bicycle` blir en egen fordonstyp. En cykel som saknar registreringsnummer behöver en stabil, unik `Identifier` som kan sparas och återställas på samma sätt som för övriga fordon.

### Bus

En buss motsvarar 16 storleksenheter och behöver **fyra sammanhängande parkeringsplatser**, där en normal plats motsvarar 4 enheter.

- **Lärarens förtydligande enligt utvecklaren:** De första **50 parkeringsplatserna reserveras som bussområde** i stället för att modellera takhöjd separat.
- **Nyparkering:** Fyra sammanhängande lediga platser måste finnas inom detta område, exempelvis 001–004 eller 047–050.
- **Förflyttning:** Bussens fyra platser måste fortfarande vara sammanhängande. Om flytt till platser efter 050 ska tillåtas behöver den regeln fastställas innan implementation; uppgiften och det hittills återgivna lärarbeskedet ger inte en entydig grund för undantaget.
- Bussens identifierare och ursprungliga UTC-ankomsttid ska behållas vid flytt.
- En buss ska behandlas som **ett fordon**, inte som fyra separata fordon.

Eftersom en buss använder flera platser behövs en framtida utökning av domän- och lagringsmodellen som kan bevara sambandet mellan bussen och dess fyra upptagna platser. **Den exakta JSON-representationen för buss är inte beslutad.** Vi lägger därför inte till exempelvis `OccupiedSpotNumbers` i G-versionens filformat redan nu.

**VG-specifikt krav:** Om datafilen saknas ska Prague Parking 2.1 automatiskt skapa den och fylla den med giltiga testdata. Detta är ett separat krav och ändrar inte G-versionens nuvarande beteende, där en saknad fil ger ett tomt garage i minnet.

VG-funktionaliteten implementeras i ett separat projekt i samma solution och GitHub-repository, med en fungerande G-version som utgångspunkt.

## Relaterad dokumentation

- `configuration.md` – inställningar, priser, tidszon och kapacitetsregler.
- `class-diagram.md` – domänmodell och klassernas relationer.
- `design-decisions.md` – särskilt DD-008–DD-011, DD-023, DD-025 och DD-026.
- `requirements.md` – krav och status för G respektive VG.
