# Prague Parking V2

**Ett objektorienterat parkeringssystem i C# och .NET 10**, med interaktivt konsolgränssnitt, konfigurerbara parkeringsregler och JSON-baserad datalagring.

Projektet omfattar två färdigutvecklade och testade versioner: **Prague Parking 2.0 (G)** och **Prague Parking 2.1 (VG)**. Båda ligger i samma Visual Studio-lösning, men har egna projekt och separata datafiler. V2.1 vidareutvecklar systemet med fler fordonstyper, storleksbaserad kapacitet, bussparkering och en automatisk livedemonstration.

| Projektinformation | |
|---|---|
| **Utvecklare** | Rami Issa |
| **Utbildning** | Systemutvecklare DevSecOps, KYH |
| **Kursuppgift** | C# Programming – Assignment 2 |
| **Utvecklingsperiod** | **5–22 oktober 2026** (18 kalenderdagar inklusive start- och slutdatum) |
| **Färdigställd** | **22 oktober 2026** |
| **Status** | **V2.0 (G) och V2.1 (VG): färdigutvecklade, testade och kravgenomgångna** |
| **Utvecklingsmiljö** | Visual Studio Community 2026, .NET 10 |

## Översikt

Prague Parking hanterar ett parkeringsgarage från incheckning till utcheckning. Användaren kan parkera, söka efter och flytta fordon, få parkeringsavgiften beräknad och se garagebeläggningen i en interaktiv översikt.

| Funktion | V2.0 (G) | V2.1 (VG) |
|---|:---:|:---:|
| Incheckning, sökning, flytt och utcheckning | ✓ | ✓ |
| Parkeringsavgift och kvitto | ✓ | ✓ |
| Bil (Car) och motorcykel (MC) | ✓ | ✓ |
| Cykel (Bicycle) och buss (Bus) | – | ✓ |
| JSON-lagring och återställning vid omstart | ✓ | ✓ |
| Interaktiv garagekarta | ✓ | ✓ |
| Storleksbaserad kapacitet och blandade fordonstyper | – | ✓ |
| Buss på fyra sammanhängande platser | – | ✓ |
| Två interfaces: `IVehicle`, `IParkingSpot` | – | ✓ |
| Omläsning av prislista (`Reload Pricing`) | ✓ | ✓ |
| Omläsning av hela konfigurationen (`Reload Configuration`) | – | ✓ |
| Automatisk livedemonstration | – | ✓ |

V2.0 har behållits som en separat G-version. Vid utvecklingen av V2.1 har den färdiga G-koden inte skrivits om.

## Kom igång

### Förutsättningar

- **.NET 10 SDK** eller **Visual Studio Community 2026** med stöd för .NET-utveckling.
- En terminal med stöd för interaktiv tangentbordsinmatning och ANSI-färger. Gränssnittet har främst körts och testats i Windows Terminal.

### Starta i Visual Studio

1. Öppna **`PragueParkingV2.slnx`**.
2. Välj **`PragueParkingV2_1`** som startprojekt för VG-versionen, eller **`PragueParkingV2`** för G-versionen.
3. Bygg lösningen med **Ctrl + Shift + B**.
4. Starta med **Ctrl + F5** eller **F5**.
5. Navigera med **piltangenterna** och **Enter**. Använd **ESC** för att lämna en vy där det stöds.

De två `.Data`-projekten är klassbibliotek och ska inte väljas som startprojekt.

### Starta från terminalen

Öppna terminalen i katalogen där `PragueParkingV2.slnx` ligger:

```bash
dotnet restore
dotnet build

# Prague Parking 2.1 (VG)
dotnet run --project PragueParkingV2_1/PragueParkingV2_1.csproj

# Prague Parking 2.0 (G)
dotnet run --project PragueParkingV2/PragueParkingV2.csproj
```

> **Konfiguration och filer:** V2.1 använder `Config/config.template.json` som mall. Om den aktiva `Config/config.json` saknas vid första start kopieras mallen till en ny fil. Aktiva filer läses relativt programmets körkatalog (`AppContext.BaseDirectory`), vanligtvis `bin/Debug/net10.0/`. G- och VG-versionen använder var sin körkatalog.

## Teknik och arkitektur

| Teknik | Användning |
|---|---|
| **C# / .NET 10** | Objektorienterad applikationslogik |
| **Spectre.Console 0.57.2** | Menyer, paneler, parkeringskarta och tangentbordsnavigering |
| **System.Text.Json** | Konfiguration och beständig lagring av parkeringsdata |
| **Git / GitHub** | Versionshantering |
| **Visual Studio Community 2026** | Utveckling, felsökning och byggning |

Lösningen är uppdelad i **fyra projekt**:

```text
PragueParkingV2.slnx
├── PragueParkingV2/                 G – Console App
├── PragueParkingV2.Data/            G – Class Library
├── PragueParkingV2_1/               VG – Console App
│   ├── Application/                 Bl.a. ParkingDataMapper och DemoScenario
│   ├── Domain/                      Fordon, interfaces och parkeringslogik
│   ├── UI/                          Menyer, Garage Overview och MenuInput
│   ├── Config/                      config.template.json
│   └── Program.cs
├── PragueParkingV2_1.Data/          VG – Class Library
│   ├── Configuration/               AppConfiguration och ConfigurationService
│   └── Persistence/                 JSON-datamodeller och ParkingDataService
├── README.md
├── LOGGBOK.md
└── docs/                           Fördjupad teknisk dokumentation
```

**Ansvarsfördelningen** följer en uppdelning mellan domänmodell, användargränssnitt, applikationsflöde och dataåtkomst. `ParkingDataMapper` översätter mellan parkeringsgaragets domänobjekt och de datamodeller (DTO:er) som används vid JSON-lagring. Dataklassbiblioteket behöver därmed inte känna till de konkreta fordonsklasserna.

### Objektorienterad domänmodell

- **`Vehicle`** är en abstrakt basklass med `Identifier` och `ArrivalTime` (UTC). `Car`, `MC`, `Bicycle` och `Bus` ärver från basklassen.
- **`IVehicle`** anger ett gemensamt kontrakt för fordonen i V2.1.
- **`ParkingSpot`** ansvarar för en enskild plats, dess fordon, tillgänglig kapacitet och eventuella bussreservationer.
- **`IParkingSpot`** definierar platsens gemensamma kontrakt i V2.1.
- **`ParkingGarage`** ansvarar för platstilldelning, sökning, flytt, borttagning och bussens sammanhängande platser.
- **`ParkingFeeCalculator`** beräknar avgifter enligt konfigurationen; **`CheckoutService`** samordnar utcheckningen.

Klassrelationer och tekniska motiveringar finns i [klassdiagrammet](docs/class-diagram.md) och [designbesluten](docs/design-decisions.md).

## Parkeringsregler i V2.1

### Storleksbaserad kapacitet

I G-versionen begränsas hur många fordon av en typ som får dela en plats. I VG-versionen använder garaget i stället **storleksenheter**. En vanlig parkeringsplats har som standard kapaciteten **4**.

| Fordonstyp | Storlek | Exempel på användning |
|---|---:|---|
| Bicycle | 1 | Upp till fyra cyklar på en plats |
| MC | 2 | Upp till två MC på en plats |
| Car | 4 | En bil fyller en plats |
| Bus | 16 | Reserverar fyra sammanhängande platser |

En MC och en cykel kan exempelvis dela en plats: **2 + 1 = 3 av 4** utnyttjade enheter. Storlekarna hämtas från konfigurationen, inte från hårdkodade värden i fordonsklasserna.

### Bussens parkeringslogik

En buss hanteras som **ett fordon**, men använder **fyra angränsande parkeringsplatser**. `ParkingGarage` söker efter ett giltigt sammanhängande intervall och skapar reservationer för samma bussobjekt. Vid flytt kontrolleras det nya intervallet, även när det överlappar det gamla. Vid utcheckning frigörs samtliga reservationer.

Standardkonfigurationen begränsar bussparkering till **platserna 001–050** via `BusMaxParkingSpot`. Hela bussens fyrplatsintervall måste ligga inom den tillåtna delen av garaget.

### Parkeringsavgifter

| Fordon | Pris per påbörjad timme |
|---|---:|
| Car | 20 CZK |
| MC | 10 CZK |
| Bicycle | 5 CZK |
| Bus | 80 CZK |

**Standardregel:** Parkering kortare än **10 minuter** är gratis. Vid debitering avrundas parkeringstiden uppåt till påbörjad timme. Exakt 10 minuter omfattas inte av gratisperioden i den implementerade regeln.

## Konfiguration och datalagring

### Konfiguration

I V2.1 innehåller `config.template.json` följande standardvärden:

| Inställning | Värde |
|---|---|
| `Garage.NumberOfParkingSpots` | 100 |
| `Garage.SpotCapacity` | 4 |
| `Garage.BusMaxParkingSpot` | 50 |
| `VehicleTypes` | Car 4, MC 2, Bicycle 1, Bus 16 |
| `Pricing.FreeMinutes` | 10 |
| `System.TimeZone` | Europe/Prague |

`ConfigurationService` läser och validerar inställningarna. Under körning kan **Administration → Reload Configuration** läsa om den aktiva filen och kontrollera att ändringarna är förenliga med fordon som redan står parkerade. En ogiltig ändring aktiveras inte. **Reload Pricing** används för att uppdatera prisreglerna.

Att ändra projektets mall påverkar inte automatiskt en redan skapad aktiv `config.json`. Mer information finns i [configuration.md](docs/configuration.md).

### JSON-persistens

Parkeringsinformationen sparas i **`Data/parking-data.json`** under programmets körkatalog. Vid omstart läses parkerade fordon tillbaka med **identifierare, fordonstyp, parkeringsplats och ursprunglig ankomsttid i UTC**.

I V2.1 sparas en buss som **ett fordon på sin primärplats**; de övriga tre platserna reserveras när garaget återställs. JSON-modellen skiljer sig därmed från hur bussens platser representeras i minnet.

Om V2.1:s datafil saknas vid uppstart skapar `TestDataSeeder` ett startläge med **en buss, en bil, en MC och en cykel** och sparar detta. En befintlig datafil fylls inte på med testfordon. Vanlig incheckning, utcheckning och flytt uppdaterar den permanenta datan.

Se [parking-data.md](docs/parking-data.md) för JSON-format, validering och återställningsflöde.

## Användargränssnitt

Gränssnittet är byggt med Spectre.Console och använder återanvändbara komponenter för menyer, statusmeddelanden, paneler och kvitton.

- **Park Vehicle:** Registrera ett fordon och tilldela en giltig plats.
- **Check Out Vehicle:** Beräkna avgift, visa kvitto och avsluta parkeringen.
- **Move Vehicle:** Flytta ett fordon efter kontroll av destinationen.
- **Find Vehicle:** Sök via identifierare och visa plats, ankomsttid och parkeringstid.
- **Garage Overview:** Visa och navigera mellan parkeringsplatser och platsdetaljer.
- **Administration:** Läs om prislista eller fullständig konfiguration.
- **Live Demonstration (V2.1):** Visa systemets parkeringslogik automatiskt.
- **Exit:** Avsluta applikationen.

**V2.0** använder ett blågult färgtema. **V2.1** har ett separat mörkt **Sunset-tema** för att skilja versionerna åt.

### Garage Overview

Parkeringskartan anpassar antal kolumner och synliga rader efter terminalens storlek. Kartan använder färgade celler utan individuella ramar, en detaljpanel till höger och en statuspanel under kartan. Om alla platser inte ryms samtidigt används en intern viewport.

| Färg | Status |
|---|---|
| Grön | Ledig plats |
| Orange | Delvis upptagen plats |
| Röd | Fullbelagd plats eller reserverad bussplats |

**SPOT DETAILS** visar bland annat fordonsidentifierare, typ, kapacitetsutnyttjande och parkeringstid. Piltangenterna används för navigering och **ESC** återgår till föregående meny.

### Live Demonstration – V2.1

Livedemonstrationen visar hur parkeringsgaraget förändras under ett automatiskt scenario med **Car, MC, Bicycle och Bus**. Den illustrerar incheckning, utcheckning och flytt, inklusive bussens fyra sammanhängande platser och blandad beläggning.

Rubriken **`LIVE DEMONSTRATION | PARKING MAP`** visas i kartpanelen. **`LAST EVENT`** redovisar den senaste händelsen, medan **`SPOT DETAILS`** och **`GARAGE STATUS`** uppdateras under demonstrationen. Scenariot fortsätter tills användaren trycker **ESC**.

Demonstrationen använder **ett separat garage i minnet** och ändrar **inte** den ordinarie `parking-data.json`. Det gör den användbar vid presentation utan att störa parkerade fordon i det vanliga systemet.

## Tester och utvecklingsperiod

**Båda versionerna är färdigutvecklade och manuellt testade. Kravmatrisens G- och VG-punkter har slutligen gåtts igenom den 22 oktober 2026.** Under utvecklingen har jag kontrollerat både normalflöden och utvalda fel- och gränsfall. Följande är en sammanfattning; detaljerade teststeg, observationer och reflektioner finns i [LOGGBOK.md](LOGGBOK.md).

| Datum | Arbete och dokumenterade kontrollpunkter |
|---|---|
| **2026-10-05** | Projektstart: Prague Parking V2. |
| **2026-10-11** | Arbete med JSON-persistens och tidiga valideringskontroller. |
| **2026-10-13** | Garagekartans layout och visning av parkeringsplatser testades under perioden 13–15 oktober. |
| **2026-10-14** | Incheckning, sparning, omstart och återställning av fordon i G-versionen. |
| **2026-10-15** | Utcheckning, kvitto, fordonsflytt och återställning efter omstart. |
| **2026-10-16** | Fördjupade G-tester: felaktig JSON, dubbletter, kapacitetsgränser, framprovocerade sparfel, prisomläsning och garagekarta med upp till 1 000 platser. |
| **2026-10-20** | Separata projekt för V2.1 etablerades i samma solution. |
| **2026-10-21** | VG-tester: bussens incheckning/flytt/utcheckning, blandad kapacitet, avgifter, konfigurationsomläsning, testdata och JSON-återställning. |
| **2026-10-22** | Live Demonstration färdigställd och kontrollerad; äldre kapacitetskod rensades och lösningen byggdes utan fel. **Projektet färdigutvecklat och testat.** |

**Utvecklingstid:** **5–22 oktober 2026** – **17 dygn mellan start- och slutdatum, eller 18 kalenderdagar inklusive båda datumen**. Perioden beskriver projektets start och avslut, inte antalet faktiska arbetstimmar.

Bland genomförda kontroller finns bland annat:

- **G:** Incheckning, utcheckning, flytt, sökning, priskalkyl, JSON-sparning och återställning efter omstart.
- **G:** Avvisning av korrupt JSON, ogiltiga platsnummer, dubbletter och överbeläggning; återställning vid framprovocerade sparfel i parkering, utcheckning och flytt.
- **VG:** MC och Bicycle på samma plats (3/4 kapacitet), bussens överlappande flytt **004–007 → 006–009**, och bussavgift **160 CZK för 1 h 38 min**.
- **VG:** `Reload Configuration` med utökat antal platser och nekad kapacitetsminskning; startdata vid saknad JSON-fil samt bevarade fordon och ankomsttider efter omstart.
- **VG:** Garage Overview och den separata Live Demonstration, inklusive användning av **ESC**.

Dessa manuella tester dokumenterar de genomförda scenarierna; de innebär inte ett generellt bevis på kraschsäkerhet under alla tänkbara filsystemfel eller fullständig kompatibilitet med varje terminalmiljö.

## Dokumentation

README ger en snabb introduktion. För teknisk fördjupning och utvecklingshistorik finns följande dokument:

| Dokument | Innehåll |
|---|---|
| [LOGGBOK.md](LOGGBOK.md) | Utveckling dag för dag, tester, problem och egna reflektioner |
| [requirements.md](docs/requirements.md) | G- och VG-krav samt kravuppföljning |
| [project-plan.md](docs/project-plan.md) | Projektplan, arbetsfaser och milstolpar |
| [design-decisions.md](docs/design-decisions.md) | Arkitekturval och tekniska avvägningar |
| [class-diagram.md](docs/class-diagram.md) | Klassdiagram och relationer mellan klasser |
| [configuration.md](docs/configuration.md) | Inställningar och hantering av JSON-konfiguration |
| [parking-data.md](docs/parking-data.md) | Datalagring, JSON-struktur och återställning |

## Projektinformation

Prague Parking V2 är min **andra individuella inlämningsuppgift i C#** inom utbildningen **Systemutvecklare DevSecOps vid KYH**, läsåret **2026**. Arbetet omfattar objektorienterad programmering, gränssnitt, konfigurationshantering, datalagring och ett interaktivt konsolgränssnitt.

**Utvecklare:** Rami Issa  
**Utvecklingsperiod:** 5–22 oktober 2026  
**Färdigställt:** 22 oktober 2026  
**Versionshantering:** Git och GitHub  
**Repository:** Privat, enligt individuellt undantag godkänt av läraren. Läraren ges åtkomst till projektet inför bedömningen; publik publicering krävs inte.
