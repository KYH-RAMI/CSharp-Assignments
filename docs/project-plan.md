# Prague Parking V2 – Projektplan

**Uppdaterad:** 2026-10-15

**Projekt:** Prague Parking V2  
**Utvecklare:** Rami Issa  
**Utvecklingsmiljö:** Visual Studio Community 2026  
**Teknik:** C# / .NET 10 / Spectre.Console

**Arbetsordning:** Prague Parking 2.0 (G) färdigställs och verifieras innan Prague Parking 2.1 (VG) implementeras.

---

## Projektmål

Utveckla ett objektorienterat parkeringssystem som uppfyller G-kraven för Prague Parking 2.0.

Systemet ska innehålla:

- Fordonshantering.
- Parkeringsplatser.
- Kapacitetskontroll.
- Prisberäkning.
- Incheckning och utcheckning.
- Flytt och sökning.
- JSON-konfiguration.
- Permanent datalagring.
- Interaktivt användargränssnitt.

VG-versionen utvecklas separat när G-versionen är stabil. Dokumenterade VG-behov beaktas i arkitekturen, men implementeras inte i förtid.

**Statusprincip:** *Implementerad* betyder att kod har tagits fram. *Testad* eller *verifierad* används endast när ett motsvarande test faktiskt har genomförts. Planerade funktioner räknas inte som färdiga. Statusen omfattar manuellt testade normalflöden för incheckning, utcheckning och fordonsflytt samt JSON-återställning efter omstart den 14–15 oktober. Tester av sparfel och fullständig avstämning mot det lokala källkodsträdet återstår.

---

## Projektfaser

| Fas | Arbetsområde | Status |
|---|---|---|
| 1 | Kravanalys | Genomförd; kravmatrisen hålls uppdaterad |
| 2 | Solution och projektstruktur | Genomförd |
| 3 | Domänmodell | Implementerad; parkering, utcheckning och fordonsflytt manuellt testade i normalflöden |
| 4 | JSON-konfiguration | Implementerad; grundfall och validering manuellt testade |
| 5 | Parkering och sökning i domänlagret | Implementerad; grundfall manuellt testade |
| 6 | Prisberäkning | Implementerad; manuellt testad |
| 7 | CheckoutService | Implementerad; använd via testat Checkout-UI med avgift och sparad borttagning |
| 8 | Spectre.Console och garageöversikt | Kompakt `Grid`-karta implementerad; testad med 200 platser, bil och två MC på delad plats; gränstester återstår |
| 9 | Main Menu och UI-flöden | Pågår; Park, Check Out och Move implementerade och manuellt testade; Find och Administration återstår |
| 10 | Parkeringspersistens | Pågår; incheckning, utcheckning och flytt sparar JSON; bilflytt och utcheckning kontrollerade efter omstart, feltester återstår |
| 11 | Administrationsfunktioner | Ej påbörjad i UI |
| 12 | Integrationstester och dokumentation | Pågår; fullständig G-verifiering återstår |
| 13 | Prague Parking 2.1 / VG | Ej påbörjad |

---

## Milstolpe 1 – Arkitektur och domänmodell

**Status: Genomförd på implementationsnivå**

Domänmodellen har implementerats med:

- `Vehicle`.
- `Car`.
- `MC`.
- `ParkingSpot`.
- `ParkingGarage`.
- `ParkingFeeCalculator`.
- `ParkingFeeResult`.
- `CheckoutService`.
- `CheckoutResult`.

Grundläggande parkering, sökning, borttagning, dubblettkontroll och prisberäkning har testats manuellt. Under den 15 oktober tillkom `ParkingGarage.MoveVehicle()` med kontroller av ursprungsplats, destinationsplats och fordonskapacitet. Parkering, utcheckning och flytt har även prövats via UI med JSON-sparning i lyckade normalflöden. Återställning vid undantag och misslyckad filskrivning är ännu inte felinjiceringstestad.

`Vehicle` innehåller stöd för en befintlig UTC-ankomsttid. Konstruktorsöverlagringar i `Car` och `MC` används nu av `ParkingDataMapper` för återställning med ursprunglig UTC-tid.

---

## Milstolpe 2 – Konfiguration

**Status: Genomförd för grundläggande inläsning och validering**

`ConfigurationService` har implementerats.

Funktionalitet:

- JSON-inläsning.
- Standardkonfiguration när runtime-filen saknas.
- Validering av befintlig konfiguration.
- Garagekapacitet.
- Prisinställningar.
- Tidszon.
- Fordonskapacitet via `VehiclesPerSpot`.

En befintlig ogiltig konfigurationsfil får inte skrivas över automatiskt. Omläsning av prislistan via administrationsmenyn återstår.

---

## Milstolpe 3 – Prisberäkning och Checkout

**Status: Domänlogik implementerad; utcheckning via UI manuellt verifierad i normalflödet**

`ParkingFeeCalculator`, `ParkingFeeResult`, `CheckoutService` och `CheckoutResult` har implementerats.

- Timpriser för `Car` och `MC` samt gratisminuter hämtas från konfigurationen.
- Avgift beräknas enligt reglerna för påbörjade timmar.
- `UpdatePricing()` kan validera och ersätta aktiva prisvärden.
- Utcheckning samordnar sökning, prisberäkning och borttagning.

`CheckOutVehicleView` använder `CheckoutService` och `ParkingFeeCalculator`, visar avgift och begär bekräftelse innan fordonet tas bort. Automatisk JSON-sparning efter utcheckning har testats: ett utcheckat fordon var fortsatt borttaget efter omstart. Återställning vid simulerat skrivfel och övriga gränsfall återstår.

---

## Milstolpe 4 – User Interface

**Status: Pågår**

### Main Menu

En interaktiv huvudmeny har implementerats med:

- Centrering i terminalfönstret.
- Tangentbordsnavigering och markering av aktivt menyval.
- Återgång från `GarageOverView`.
- Exit-funktion.
- Gemensamt färgtema.
- Positionsbaserad uppdatering av menyval för att minska flimmer.

Återanvändbar menynavigering samlas i `AppTheme.ShowMenu()` i stället för att varje vy ska skapa en egen lösning.

### AppTheme

`AppTheme` samlar:

- Blå och gula temafärger.
- Bakgrund och textfärger.
- Panelramar och gemensamma panelinställningar.
- Återanvändbar menypresentation.
- Hjälpmetoder för konsolens visning och återställning.

Temat används av huvudmenyn, garageöversikten samt Park-, Check Out- och Move-vyerna. De vanliga formulärvyerna använder centrerade paneler, normalt 62 tecken breda, medan garageöversikten använder terminalens bredd. Kompatibilitet med andra terminaler behöver fortfarande testas.

### MessageService

`MessageService` används för återanvändbara fel-, varnings-, informations- och bekräftelsemeddelanden. Under den 15 oktober centrerades dess paneler med `AppTheme.WriteCenteredPanel()`. Felmeddelanden i Move-vyn visas utan att varje misslyckat platsval staplar en ny panel på skärmen. Enhetlig tangenthantering och meddelandepresentation i samtliga vyer behöver fortsatt verifieras.

**Viktig regel:** Ett lyckat resultat får inte presenteras som permanent sparat om JSON-skrivningen misslyckas. Denna ordning används nu i Park-, Check Out- och Move-vyerna. Kompensationsförsök vid `IOException` eller `UnauthorizedAccessException` finns, men har inte felinjiceringstestats och utgör ingen fullständig transaktionsgaranti.

### Garage Overview

**Status 2026-10-15: Kompakt layout manuellt testad med 200 platser, bil samt två MC på samma plats och korrekta räknare.**

Efter försök med individuella `Panel`-komponenter och en `Table` med radavgränsare byggdes översikten om för att utnyttja terminalens bredd utan onödig scrollning eller avklippta parkeringsnummer.

Funktionalitet som finns i den aktuella `GarageOverView.cs`:

- En kompakt Spectre.Console-`Grid` för parkeringsplatserna, inuti en gemensam `Panel` utan separata ramar runt varje plats.
- Dynamiskt antal kolumner beräknat utifrån terminalbredden och antalet siffror i platsnumren; ingen fast 5 × 5- eller 10 × 10-begränsning.
- Huvudlayout där `PARKING MAP` använder ungefär 80 % av bredden och `SPOT DETAILS` ungefär 20 %, med en minsta läsbar bredd för detaljpanelen.
- Status visas med grön bakgrund för ledig plats, orange för delvis upptagen och röd för full plats. Markerad plats får guldtext utan att statusbakgrunden ändras.
- Piltangenterna flyttar markeringen enligt samma kolumnantal som kartan; **ESC** går tillbaka. Den tidigare sidnavigeringen med Page Up/Page Down används inte i den aktuella vyn.
- `AnsiConsole.Live()` används för uppdatering av kartan och detaljpanelen vid navigering.
- En intern viewport begränsar antalet synliga kartrader **endast när hela kartan inte får plats på höjden**. Det synliga platsintervallet visas då i sidfoten.
- `GARAGE STATUS` placeras under kartan och detaljpanelen och sträcker sig över deras sammanlagda bredd.
- För smala eller låga terminalfönster visas en uppmaning att förstora fönstret; layouten kräver minst 80 kolumner och 18 rader.

**Visuellt verifierat 2026-10-13–15:** 200 lediga platser visades den 13 oktober utan avklippta nummer eller oönskad scrollning. Den 14 oktober visades två fulla bilplatser med `FREE 198`, `OCCUPIED 2`, `VEHICLES 2`. Den 15 oktober testades bilflytt från plats 001 till 050 och delad MC-plats 003 med två motorcyklar; garageöversikten visade `FREE 198`, `OCCUPIED 2`, `VEHICLES 3`.

**Återstår:** Testa navigation till första och sista platsen, navigering efter fönsterstorleksändring, viewport med 500–1 000 platser, olika terminalstorlekar samt fler gränsfall för orange delbelagd MC-plats.

### Spot Details

Informationspanelen är implementerad för att visa:

- Markerat parkeringsplatsnummer, status och beläggning.
- Tillgänglig kapacitet för `Car` och `MC` när platsen är ledig.
- Varje parkerat fordons typ och identifierare.
- Ankomsttid i den konfigurerade tidszonen.
- Parkeringstid (`Duration`) beräknad från UTC-ankomsttiden.

Panelen ligger längst till höger med mindre utrymme än parkeringskartan. Ledig plats och bil har kontrollerats visuellt, bland annat med `ABC123` och `UKB48K`. Den 15 oktober visades även två MC med separata identifierare och ankomsttider på samma plats (003) samt korrekt beläggning 2/2. Återställning av just denna MC-kombination efter omstart är ännu inte särskilt bekräftad.

---

## Milstolpe 5 – UI-funktionalitet

**Status: Pågår**

Huvudmenyn är implementerad och öppnar nu `ParkVehicleView`, `CheckOutVehicleView` och `MoveVehicleView`. De tre vyerna använder befintlig domänlogik och uppdaterar `parking-data.json` i normala användarflöden. **Find Vehicle** och **Administration** är ännu platshållare i huvudmenyn.

1. **Park Vehicle:** Incheckning av bil med automatisk JSON-sparning och återställning efter programomstart har verifierats. MC-fall har provats, men ytterligare negativa tester återstår.
2. **Check Out Vehicle:** Använder `CheckoutService`, visar parkeringsavgift och ett kvitto samt sparar borttagningen. Ett manuellt omstartstest bekräftade att det utcheckade fordonet inte återkom.
3. **Move Vehicle:** `ParkingGarage.MoveVehicle()` bevarar samma fordonsobjekt och ursprunglig `ArrivalTime`. `MoveVehicleView` använder centrerade paneler, validering före bekräftelse och en `while`-loop för nytt platsval. Bilflytt 001 → 050 sparades och kvarstod efter omstart. MC → MC med ledig kapacitet tilläts; MC → bilplats nekades.
4. **Find Vehicle:** Separat sökvy återstår att implementera.
5. **Administration:** Prisomläsning via synligt menyval återstår att implementera och testa.

**Kvarstående kvalitetsarbete:** Utför kontrollerade tester av fel vid filskrivning och återställning, fler kapacitets- och inmatningsgränser samt konsekvent UI-beteende vid avbrytande. ESC-avbrytning direkt under ett `TextPrompt` är ännu inte implementerad; tomt Enter används för återgång i Move-vyn.

---

## Milstolpe 6 – Persistens

**Status: Pågår – normalt JSON-flöde vid incheckning, utcheckning och flytt manuellt verifierat 2026-10-14–15; särskilda fel- och gränstester återstår**

Parkeringsinformation ska sparas i `parking-data.json` via det separata Data-projektet. Den överenskomna strukturen i `parking-data.md` gäller för Prague Parking 2.0:

```text
ParkingSpots
    └── SpotNumber
        └── Vehicles
            ├── Identifier
            ├── VehicleType
            └── ArrivalTime (UTC)
```

Regler:

- Endast upptagna parkeringsplatser sparas.
- Tomma platser skapas utifrån `config.json` vid uppstart.
- Varje fordon återställs till **samma parkeringsplats** med **samma identifierare, fordonstyp och UTC-ankomsttid**.
- Filen uppdateras efter lyckad incheckning, utcheckning och flytt.
- Inläst data valideras, bland annat avseende dubbletter, platsnummer, kapacitet och giltiga fordonstyper.
- Befintlig felaktig JSON ska inte tyst skrivas över.
- Skrivfel får inte leda till falskt framgångsmeddelande eller förlorat befintligt datainnehåll.
- Filsökvägar får inte vara hårdkodade till en utvecklingsdator.

Den planerade lagringen ska kunna vidareutvecklas för `Bicycle` och `Bus`, men **inga nya obligatoriska VG-fält läggs till i G-formatet nu**. Lösningen för en buss som upptar fyra platser fastställs under VG-implementationen.

### Implementerat och manuellt verifierat 2026-10-11

- `ParkingDataService` med filväg, `Load()`/`Save()` och tre separata datamodeller har implementerats i `PragueParkingV2.Data/Persistence`.
- Valideringen kontrollerar plats- och fordonslistor, positiva/unika platsnummer, icke-tomma identifierare, dubbletter via `HashSet`, G-typerna `Car`/`MC` och UTC-tid.
- `Load()` har läst ett fordon `ABC123` på plats 1 med `ArrivalTime.Kind = Utc`; den ogiltiga typen `Truck` har avvisats via `InvalidDataException`.
- `Save()` använder indenterad JSON samt temporärfil och `File.Move()`/`File.Replace()`. Den 11 oktober hade enbart serialiserad JSON granskats; den 14 oktober verifierades däremot en lyckad verklig sparning till befintlig fil genom omstartstest. Krasch- och feltester återstår.

### Återstående implementation och verifiering

- Komplettera validering av utelämnade fält, identifierarlängd och kapacitet mot `AppConfiguration` utan att införa domänberoende i Data-projektet.
- Skydda en redan korrupt datafil mot överskrivning och verifiera felhantering vid skrivning och städning.
- **Genomfört:** `ParkingDataMapper` återställer fordon med identifierare, plats och UTC-ankomsttid. Parkering, utcheckning och flytt använder `ToDocument()` och `Save()`; lyckade normalflöden har verifierats manuellt.
- **Återstår:** Verifiera att kompensation och felbesked fungerar vid simulerade skrivfel i samtliga tre åtgärder. Inget fullständigt transaktionsskydd är bevisat.
- Testa ny/saknad fil, befintlig korrupt fil, flera fordon på samma plats, kapacitetsfel, avbruten skrivning och städning av temporär fil.

**Verifiering 2026-10-14–15:** `Program.cs` laddar JSON via `ParkingDataMapper.ToGarage()`. Bilen `UKB48K` återkom efter parkering och omstart; vid efterföljande utcheckning var platsen ledig även efter omstart. Den 15 oktober flyttades `ABC123` från 001 till 050, och den nya platsen kvarstod efter omstart. Två MC kunde dela plats 003, men deras gemensamma placering efter en separat omstart har ännu inte rapporterats verifierad. `ParkingDataService` ansvarar fortsatt för JSON och filåtkomst.

---

## Milstolpe 7 – Administration

**Status: Ej påbörjad i UI**

Planerad funktionalitet för G:

- Omläsning av prislistan från `config.json`.
- Validering av samtliga nya prisvärden.
- Tillämpning via befintliga `ParkingFeeCalculator.UpdatePricing()`.
- Bibehållna tidigare giltiga värden om omläsningen misslyckas.

**VG-utökning:** Omläsning av *hela* konfigurationen under körning, inklusive validering mot redan parkerade fordon. Detta ska inte förväxlas med G-versionens prisomläsning.

---

## Milstolpe 8 – Testning

**Status: Pågår**

Tidigare manuella domäntester finns noterade i `LOGGBOK.md`. Den 15 oktober tillkom end-to-end-tester, men **hela G-versionen är inte slutverifierad**.

**Manuellt verifierat hittills:**

- Bilparkering med JSON-sparning och återställning efter omstart.
- Utcheckning via UI med beräknad avgift, sparad borttagning och bekräftelse efter omstart.
- Flytt av `ABC123` från plats 001 till 050, med bevarad fordonsidentitet och sparad placering efter omstart.
- Två MC på samma plats med rätt beläggning, samt nekad flytt av MC till bilplats.
- Olämpliga platsval fångas i Move-vyns `while`-loop och ett nytt platsnummer kan anges utan att börja om.
- `Garage Status` skiljer mellan upptagna platser och totalt antal fordon, exempelvis `FREE 198`, `OCCUPIED 2`, `VEHICLES 3`.

**Återstående tester omfattar:**

- Återstart med två MC på samma plats och verifierat oförändrade UTC-ankomsttider.
- Simulerade filskrivningsfel, misslyckad återställning, korrupt befintlig JSON och skydd mot dataförlust.
- Fler gränsfall för dubbletter, registreringsidentifierare, kapacitet och felaktig inmatning.
- Fullständigt användarflöde för Find Vehicle samt prisomläsning i Administration.
- Garageöversiktens navigering till första/sista platsen, beteende vid ändrad terminalstorlek och statusfärger i fler kombinationer.
- Intern viewport och kartans placering vid 500–1 000 platser samt mindre terminalfönster.
- Körning på en annan dator utan hårdkodade sökvägar.

Slutlig dokumentation och status i `requirements.md` uppdateras utifrån faktiskt genomförda tester.

---

## Milstolpe 9 – Prague Parking 2.1

**Status: Ej påbörjad**

Planerade VG-funktioner:

- Separat projekt för vidareutvecklingen enligt projektplanen.
- Minst två interfaces.
- `Bicycle` och `Bus`.
- Storleksbaserad parkeringskapacitet: Bicycle = 1, MC = 2, Car = 4, Bus = 16; en normal plats = 4 enheter.
- **Nyparkering av Bus:** Fyra sammanhängande lediga parkeringsplatser inom plats 1–50.
- **Flytt av Bus:** Fyra sammanhängande lediga platser även längre ner i garaget, utanför plats 50.
- Bibehållen bussidentifierare och ankomsttid vid flytt.
- Timpriser: Bicycle 5 CZK och Bus 80 CZK.
- Fordonstyper och storlekar konfigureras enligt VG-kraven.
- Omläsning av hela konfigurationen med säker validering mot aktuellt garagetillstånd.
- Personlig reflektion enligt uppgiftskraven.

Bussens framtida representation i `parking-data.json` är **inte beslutad**. Den ska hantera fyra upptagna platser utan att behandla en buss som fyra olika fordon, samtidigt som befintlig G-data kan läsas.

---

## Närmaste utvecklingsmål (uppdaterade 2026-10-15)

### Prioritet 1 – Find Vehicle

Implementera en separat UI-vy som använder befintlig söklogik och visar registrering, fordonstyp, parkeringsplats och relevant information utan att ändra parkeringsdata.

### Prioritet 2 – Administration (G)

Läs om prislistan från `config.json` under körning, validera och uppdatera aktiva priser med `ParkingFeeCalculator.UpdatePricing()`; behåll gamla giltiga priser vid fel. Full konfigurationsomläsning hör till VG.

### Prioritet 3 – Persistens och säkra feltester

Testa två MC på samma plats efter omstart, oförändrade UTC-ankomsttider, korrupt befintlig JSON, simulering av skrivfel och kompensation vid Park, Check Out och Move. Kontrollera också temporärfilens hantering och skydd mot överskrivning.

### Prioritet 4 – UI- och kapacitetsgränser

Verifiera tom/ogiltig inmatning, parkeringsplatser som är fulla eller innehåller fel fordonstyp samt tangenthantering. Testa garageöversiktens navigeringsgränser, 100/500/1 000 platser och små terminalfönster.

### Prioritet 5 – G-avstämning och dokumentation

Jämför varje G-krav mot aktuellt källkodsbeteende och konkreta testresultat. Uppdatera `requirements.md`, `README.md`, `LOGGBOK.md` och övrig dokumentation. Gör en kontrollerad Git-commit efter granskning av ändrade filer.

### Prioritet 6 – Prague Parking 2.1 (VG)

Implementera VG först efter att G-versionen är stabil och har godkända funktionstester. Frivillig distribution och betatestning kommer senare.

---

## Risker

### Console Rendering

Olika terminaler kan hantera ANSI-sekvenser, färger, tangentbordsinmatning och fönsterstorlekar olika. Applikationen ska verifieras i fler terminalmiljöer. Den nya `Grid`-layouten utnyttjar tillgänglig skärmyta bättre än de tidigare alternativen, men behöver särskilt testas vid stor garagekapacitet, liten terminal, fönsterstorleksändring och långa fordonsuppgifter i den smala detaljpanelen.

### Data Persistence

Parkeringsdata kan gå förlorade vid felaktig filskrivning eller återställning. Move-vyn försöker flytta tillbaka fordonet i minnet om `Save()` misslyckas, men den och motsvarande kompensation vid parkering/utcheckning är ännu inte provade med simulerade fel. Särskild försiktighet krävs vid uppstart, skrivfel och samtidiga uppdateringar i en eventuell framtida fleranvändarmiljö.

### Configuration

Ogiltig befintlig konfiguration får inte ersättas automatiskt. Ändringar under körning ska inte lämna prisreglerna i ett delvis uppdaterat tillstånd.

### Scope Management

G-versionen ska färdigställas före VG. Publicering, Docker, Cloudflare och fleranvändarstöd är inte en del av den aktuella G-implementationen.

### Korrekt projektstatus

Att en klass eller vy har kodats innebär inte att den är integrationstestad. Dokumentation och kravmatris får inte markera funktioner som verifierade utan genomförda tester.

---

## Dokumentation

Utvecklingsarbetet dokumenteras löpande i:

- `LOGGBOK.md` – datum, genomfört arbete, tester, problem och nästa steg.
- `requirements.md` – krav, tolkning, berörd komponent och faktisk status.
- `design-decisions.md` – motiverade och numrerade arkitekturbeslut.
- `class-diagram.md` – implementerad domänmodell och avgränsningar.
- `configuration.md` – konfigurationsformat och regler för prisomläsning.
- `parking-data.md` – fastställt G-format, implementerad JSON-persistens och återstående feltester.
- `README.md` – projektöversikt och instruktioner för att köra applikationen.

Dokumenten ska beskriva **samma utvecklingsläge**. Historiska aktiviteter redovisas i loggboken; framtida arbete ska inte framställas som genomfört. Efter avslutad dokumentationsgenomgång jämförs filerna och sammanställs i en gemensam Git-commit. Kontrollera faktiskt `git status` och `git log` före commit; dokumentationen i sig bekräftar inte vad som redan har skickats till GitHub.

---

## Framtida distribution och betatestning

**Status: Idé och planering; inte del av G-kravens aktuella implementation**

När G och VG är stabila kan en körbar Windows-version publiceras för test utan att klasskamraterna behöver bygga källkoden. Det kräver bland annat kontroll av filvägar, konfigurationsfiler och körning på en annan dator.

En möjlig separat DevSecOps-utökning är att köra programmet i en isolerad Docker-miljö på Synology NAS med webbaserad terminal och tillfällig Cloudflare-adress. Klasskamraterna ska i så fall bara nå applikationen, inte NAS-administrationen eller Tailscale-nätet. Alla testare ska se samma parkeringsdata. Detta kräver central samordning av ändringar; flera oberoende processer får inte skriva okontrollerat till samma JSON-fil.

Feedback samlas via Discord. Ingen driftsättning eller fleranvändarlösning är implementerad eller testad i nuläget.
