# Parkeringsdata

**Uppdaterad:** 2026-10-10  
**Omfattning:** Planerad persistent lagring för Prague Parking 2.0 (G), med avgränsningar inför Prague Parking 2.1 (VG).

**Status:** JSON-formatet är beslutat och dokumenterat. Inläsning, återställning och automatisk sparning av `parking-data.json` är ännu inte implementerade eller integrationstestade.

## Planerad struktur

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

Tomma parkeringsplatser återskapas från `config.json` när programmet startar. Standardkonfigurationen innehåller 100 parkeringsplatser, men det faktiska antalet bestäms av den giltiga konfigurationen.

Prisregler och gratisgränser sparas inte i `parking-data.json` utan hämtas från `config.json`. `ParkingFeeResult` och `CheckoutResult` är tillfälliga beräknings- och resultatobjekt, inte permanent parkeringsdata.

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

Domänklasserna behöver därför stöd för att skapa `Car` och `MC` med en befintlig, giltig UTC-ankomsttid. Att dessa konstruktorsöverlagringar finns i den aktuella koden återstår att verifiera.

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

Den planerade ordningen är:

1. Läs och validera `config.json`.
2. Skapa garagets parkeringsplatser utifrån det konfigurerade antalet.
3. Läs och validera en befintlig `parking-data.json`.
4. Återskapa varje fordon på det `SpotNumber` som finns i filen, med dess ursprungliga `ArrivalTime`.
5. Aktivera garaget först när hela den sparade beläggningen har validerats.

Återställning ska **inte** använda den ordinarie metoden som automatiskt väljer första lediga parkeringsplats. Den skulle kunna flytta fordon till andra platser vid omstart.

### Validering av sparad data

Innan sparad data accepteras ska systemet bland annat kontrollera att:

- JSON-strukturen och obligatoriska fält finns och går att läsa.
- Varje `SpotNumber` är unikt och ligger inom den aktuella garagekapaciteten.
- Varje fordonsidentifierare är giltig och unik i hela garaget, **utan skillnad mellan stora och små bokstäver**.
- `VehicleType` motsvarar en fordonstyp som den aktuella versionen stöder.
- `ArrivalTime` är en giltig UTC-tidpunkt.
- Fordonen ryms på den angivna platsen enligt gällande kapacitets- och fordonsregler.

Om en befintlig fil är felaktig eller innehåller oförenliga uppgifter ska uppstarten ge ett tydligt fel. Filen får inte tyst skrivas över och ett delvis återställt garage får inte accepteras som giltigt tillstånd.

Om `parking-data.json` saknas vid första start ska garaget kunna starta tomt. Det är en annan situation än att en **befintlig** datafil är ogiltig.

## Uppdatering vid parkering

Efter en lyckad incheckning ska den nya beläggningen sparas i `parking-data.json`.

Det sparade fordonet ska innehålla:

- Dess faktiska parkeringsplats (`SpotNumber` i det omgivande platsobjektet).
- Dess `Identifier` och `VehicleType`.
- Den ursprungliga `ArrivalTime` i UTC.

Användaren ska inte få ett meddelande om varaktigt lyckad parkering om lagringen misslyckas.

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

**Status:** Detta är implementationskrav och designbeslut; filhanteringen är ännu inte färdigbyggd eller verifierad. Exakt strategi för säker filersättning och återställning vid skrivfel bestäms i implementationen.

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
