# Prague Parking V2 – Projektplan

**Uppdaterad:** 2026-10-14

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

**Statusprincip:** *Implementerad* betyder att kod har tagits fram. *Testad* eller *verifierad* används endast när ett motsvarande test faktiskt har genomförts. Planerade funktioner räknas inte som färdiga. Statusen nedan omfattar även manuellt testad JSON-incheckning och återställning den 14 oktober. Slutlig avstämning mot hela det lokala källkodsträdet återstår.

---

## Projektfaser

| Fas | Arbetsområde | Status |
|---|---|---|
| 1 | Kravanalys | Genomförd; kravmatrisen hålls uppdaterad |
| 2 | Solution och projektstruktur | Genomförd |
| 3 | Domänmodell | Implementerad; centrala delar manuellt testade |
| 4 | JSON-konfiguration | Implementerad; grundfall och validering manuellt testade |
| 5 | Parkering och sökning i domänlagret | Implementerad; grundfall manuellt testade |
| 6 | Prisberäkning | Implementerad; manuellt testad |
| 7 | CheckoutService | Implementerad; manuellt testad på domännivå |
| 8 | Spectre.Console och garageöversikt | Kompakt `Grid`-karta implementerad; testad med 200 platser, varav två fulla bilplatser, fler tester återstår |
| 9 | Main Menu och UI-flöden | Pågår; `ParkVehicleView` med automatisk JSON-sparning och omstartstest verifierad, övriga vyer återstår |
| 10 | Parkeringspersistens | Pågår; DTO:er, mapper, `Load()`/`Save()` och incheckningspersistens verifierade i normalflöde |
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

Grundläggande parkering, sökning, borttagning, dubblettkontroll, prisberäkning och utcheckning har tidigare testats manuellt från programkoden. Det är inte samma sak som att motsvarande UI-flöden eller permanent datalagring är färdigtestade.

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

**Status: Genomförd på domännivå**

`ParkingFeeCalculator`, `ParkingFeeResult`, `CheckoutService` och `CheckoutResult` har implementerats.

- Timpriser för `Car` och `MC` samt gratisminuter hämtas från konfigurationen.
- Avgift beräknas enligt reglerna för påbörjade timmar.
- `UpdatePricing()` kan validera och ersätta aktiva prisvärden.
- Utcheckning samordnar sökning, prisberäkning och borttagning.

Funktionerna har testats separat från det slutliga användargränssnittet. **UI-integration och sparning efter utcheckning återstår.**

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

Temat används av huvudmenyn och garageöversikten. Kompatibilitet med andra terminaler behöver fortfarande testas.

### MessageService

`MessageService` används för återanvändbara fel-, varnings-, informations- och bekräftelsemeddelanden. Enhetlig användning i samtliga kommande UI-flöden ska kontrolleras.

**Viktig regel:** Ett lyckat resultat får inte presenteras som permanent sparat om JSON-skrivningen misslyckas. Detta tillämpas i incheckningsflödet, men har ännu inte felinjiceringstestats.

### Garage Overview

**Status 2026-10-14: Kompakt layout manuellt testad med 200 platser, två fulla bilplatser och korrekta räknare.**

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

**Visuellt verifierat 2026-10-13–14:** 200 lediga platser visades den 13 oktober utan avklippta nummer eller oönskad scrollning. Den 14 oktober visades dessutom två fulla bilplatser, `SPOT DETAILS` och korrekt `GARAGE STATUS` (`FREE 198`, `OCCUPIED 2`, `VEHICLES 2`).

**Återstår:** Testa navigation till första och sista platsen, navigering efter fönsterstorleksändring, viewport med 500–1 000 platser, olika terminalstorlekar, orange MC-status och detaljpanelen vid två MC på samma plats.

### Spot Details

Informationspanelen är implementerad för att visa:

- Markerat parkeringsplatsnummer, status och beläggning.
- Tillgänglig kapacitet för `Car` och `MC` när platsen är ledig.
- Varje parkerat fordons typ och identifierare.
- Ankomsttid i den konfigurerade tidszonen.
- Parkeringstid (`Duration`) beräknad från UTC-ankomsttiden.

Panelen ligger längst till höger med mindre utrymme än parkeringskartan. Presentationen av **ledig plats** har kontrollerats visuellt i det nya 200-platsers testet. Presentation av en bil har nu verifierats genom testet med `ABC123` och `UKB48K`. En MC och två MC på samma plats återstår att integrationstesta.

---

## Milstolpe 5 – UI-funktionalitet

**Status: Pågår**

Huvudmenyn är implementerad. `ParkVehicleView` är ansluten till JSON-sparning och har testats med bilparkering samt återställning efter programomstart. Övriga användarflöden återstår.

Följande användarflöden återstår att färdigställa och verifiera:

1. **Park Vehicle:** Vanligt bilflöde med sparning och återställning **verifierat 2026-10-14**; felhantering och MC-fall behöver kompletterande tester.
2. **Check Out Vehicle:** Använd `CheckoutService`, visa avgift och spara borttagningen.
3. **Move Vehicle:** Flytta ett fordon utan att ändra dess ursprungliga ankomsttid och spara den nya placeringen.
4. **Find Vehicle:** Visa rätt fordon och parkeringsplats.
5. **Administration:** Läs om prislistan, validera den och aktivera nya priser utan omstart.

Den praktiska utvecklingsordningen anpassas så att persistensen kommer på plats **innan incheckning och utcheckning betraktas som färdiga UI-flöden**.

---

## Milstolpe 6 – Persistens

**Status: Pågår – normalflödet för incheckning, JSON-sparning och återställning efter omstart är verifierat (2026-10-14); fullständig fel- och åtgärdsverifiering återstår**

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

### Återstående implementation

- Komplettera validering av utelämnade fält, identifierarlängd och kapacitet mot `AppConfiguration` utan att införa domänberoende i Data-projektet.
- Skydda en redan korrupt datafil mot överskrivning och verifiera felhantering vid skrivning och städning.
- **Genomfört:** `ParkingDataMapper` återställer sparade fordon vid uppstart på angiven plats och med ursprunglig UTC-ankomsttid. Automatisk sparning efter incheckning är verifierad i normalt bilflöde.
- **Återstår:** Automatisk och konsekvent sparning efter **utcheckning och flytt** samt verifiering av kompensation och felbesked vid skrivfel.
- Testa ny/saknad fil, befintlig korrupt fil, flera fordon på samma plats, kapacitetsfel, avbruten skrivning och städning av temporär fil.

**Verifiering 2026-10-14:** `Program.cs` laddar JSON genom `ParkingDataMapper.ToGarage()`; `ParkVehicleView` sparar via `ToDocument()` och `Save()`. Testbilen `UKB48K` återkom på plats 002 efter omstart, medan `ABC123` låg kvar på plats 001. Nästa tekniska steg är att koppla Checkout till UI och säker sparning. `ParkingDataService` fortsätter ansvara för JSON och filåtkomst.

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

Tidigare manuella domäntester finns noterade i `LOGGBOK.md`. De utgör inte en fullständig end-to-end-verifiering.

Återstående tester omfattar:

- Incheckning via användargränssnittet.
- Utcheckning med korrekt pris och sparad borttagning.
- Flytt och sökning av parkerade fordon.
- Dubbelregistrering med olika versaler/gemener.
- Kapacitetsgränser för `Car` och `MC`.
- Återstart med oförändrad parkeringsplats och UTC-ankomsttid.
- Felaktig JSON, ogiltiga platsnummer och simulerade skrivfel.
- Omläsning av prislistan och avvisning av ogiltiga priser.
- Spot Details med parkerade fordon, inklusive två MC på samma plats.
- Garageöversiktens navigering till första/sista platsen, korrekt beteende vid ändrad terminalstorlek och statusfärger med faktiskt parkerade fordon.
- Intern viewport och kartans placering vid 500–1 000 platser samt mindre terminalfönster.
- Körning utan hårdkodade sökvägar på en annan dator.

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

## Närmaste utvecklingsmål (uppdaterade 2026-10-14)

### Prioritet 1 – Check Out Vehicle

Koppla befintlig `CheckoutService` och avgiftsberäkning till UI. Efter utcheckning ska JSON uppdateras utan att en misslyckad sparning felaktigt bekräftas som lyckad.

### Prioritet 2 – Move Vehicle

Bygg en säker flyttfunktion som bevarar fordonets ursprungliga `ArrivalTime`, följer kapacitetsreglerna och sparar det ändrade garaget.

### Prioritet 3 – Find Vehicle

Implementera UI-sökning på identifierare och visa relevant plats-/fordonsinformation.

### Prioritet 4 – Administration (G)

Läs om prislistan från `config.json` under körning, validera och uppdatera aktiva priser; behåll gamla giltiga priser vid fel. Full konfigurationsomläsning hör till VG.

### Prioritet 5 – Hårdare tester av persistens och UI

Testa felaktig JSON, skydd mot överskrivning, saknad fil, skrivfel och kompensation; Car/MC-beläggning, två MC på samma plats, navigeringsgränser, 100/500/1 000 platser och liten terminal.

### Prioritet 6 – G-avstämning och dokumentation

Jämför varje G-krav mot faktiskt källkodsbeteende och testresultat; uppdatera `requirements.md`, `README.md` och `LOGGBOK.md` samt gör en kontrollerad Git-commit.

### Prioritet 7 – Prague Parking 2.1 (VG)

Implementera VG först efter godkänd, stabil G-funktionalitet. Frivillig distribution och betatestning kommer senare.

---

## Risker

### Console Rendering

Olika terminaler kan hantera ANSI-sekvenser, färger, tangentbordsinmatning och fönsterstorlekar olika. Applikationen ska verifieras i fler terminalmiljöer. Den nya `Grid`-layouten utnyttjar tillgänglig skärmyta bättre än de tidigare alternativen, men behöver särskilt testas vid stor garagekapacitet, liten terminal, fönsterstorleksändring och långa fordonsuppgifter i den smala detaljpanelen.

### Data Persistence

Parkeringsdata kan gå förlorade vid felaktig filskrivning eller återställning. Särskild försiktighet krävs vid uppstart, skrivfel och samtidiga uppdateringar i en eventuell framtida fleranvändarmiljö.

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
- `parking-data.md` – fastställt G-format och regler för planerad persistens.
- `README.md` – projektöversikt och instruktioner för att köra applikationen.

Dokumenten ska beskriva **samma utvecklingsläge**. Historiska aktiviteter redovisas i loggboken; framtida arbete ska inte framställas som genomfört. Efter avslutad dokumentationsgenomgång jämförs filerna och sammanställs i en gemensam Git-commit. Kontrollera faktiskt `git status` och `git log` före commit; dokumentationen i sig bekräftar inte vad som redan har skickats till GitHub.

---

## Framtida distribution och betatestning

**Status: Idé och planering; inte del av G-kravens aktuella implementation**

När G och VG är stabila kan en körbar Windows-version publiceras för test utan att klasskamraterna behöver bygga källkoden. Det kräver bland annat kontroll av filvägar, konfigurationsfiler och körning på en annan dator.

En möjlig separat DevSecOps-utökning är att köra programmet i en isolerad Docker-miljö på Synology NAS med webbaserad terminal och tillfällig Cloudflare-adress. Klasskamraterna ska i så fall bara nå applikationen, inte NAS-administrationen eller Tailscale-nätet. Alla testare ska se samma parkeringsdata. Detta kräver central samordning av ändringar; flera oberoende processer får inte skriva okontrollerat till samma JSON-fil.

Feedback samlas via Discord. Ingen driftsättning eller fleranvändarlösning är implementerad eller testad i nuläget.
