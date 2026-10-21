# Prague Parking V2 – Projektplan

**Uppdaterad:** 2026-10-21

**Projekt:** Prague Parking V2  
**Utvecklare:** Rami Issa  
**Utvecklingsmiljö:** Visual Studio Community 2026  
**Teknik:** C# / .NET 10 / Spectre.Console

**Arbetsordning:** Prague Parking 2.0 (G) är färdigutvecklad och manuellt testad. Prague Parking 2.1 (VG) utvecklas nu i separata projekt i samma solution. G-versionens kod och dokumenterade testresultat bevaras som utgångspunkt för regressionstester.

**Historisk lägesuppdatering tidigt 2026-10-21 (ersatt av sammanfattningen nedan):** Feature-branchen `feature/prague-parking-v2.1` innehöll föregående commit `273d2c73` på GitHub; dagens arbete återstår att versionshantera. V2.1 har verifierad bilflytt med JSON-omstart, första lyckade Bicycle-registreringen och en fungerande konfigurationsmall. Bussens sökning/reservation/lagring och borttagning finns nu som kompilerande kod, men end-to-end för buss återstår. Garage Overview har ett känt körfel för Bicycle. Den tidigare milstolpsstatusen 2026-10-20 nedan är historik.

---

## Aktuell projektstatus – 2026-10-21 efter eftermiddagens tester

**G (V2.0):** Oförändrad, färdig med tidigare manuella tester. **VG (V2.1):** Domänstöd, båda interfaces, storleksbaserad logik, fyra bussreservationer, avgiftsberäkning, normal bussflytt/utcheckning, garageöversikt, full config reload och TestDataSeeder är implementerade och delvis funktionstestade. Detta ersätter de tidigare delstegens anteckningar om att bussflödet och Garage Overview ännu inte fungerar.

| Arbetsområde | Aktuellt läge |
|---|---|
| V2.1-arkitektur och interfaces | Implementerat, build grön; extra granskning kvar |
| Bussens parkering/flytt/utcheckning | Normala flöden testade; felinjektion och gränsfall kvar |
| Garage Overview / Sunset | Fungerar för Bus, Bicycle, blandad beläggning, 500-platserstest |
| Administration / full Reload Configuration | 100→200, 500 platser samt felaktig SpotCapacity testade |
| TestDataSeeder (VG-14) | Giltig generering, JSON och omstart testade; små konfigurationer kvar |
| Gemensam dokumentation | Uppdaterad till 2026-10-21; tidigare G-historik bevarad |
| Personlig reflektion | Reflektioner om busslogik och `Action` finns i loggboken; slutlig text återstår |
| Automatisk Live Demo | Önskad extrafunktion, ännu inte byggd |
| Kodstädning och övriga negativa tester | Återstår före slutlig inlämning |

**Efterföljande prioritet:** (1) granska dokumentation och commit, (2) kontrollera återstående kurskrav och säkerhets-/rollbacktester, (3) varsam kodstädning i V2.1, (4) eventuell automatisk demonstrationssekvens i isolerat garage, (5) personlig slutreflektion och inlämning. Endast G/VG-krav och nödvändiga tester är obligatoriska; frivillig Docker/NAS-distribution ingår inte.

---

## Projektmål

Utveckla och leverera ett objektorienterat parkeringssystem med **Prague Parking 2.0 (G)** och **Prague Parking 2.1 (VG)** som **två separata, körbara projekt i samma solution och GitHub-repository**. G-versionen ska förbli testbar för sig; VG byggs vidare från en stabil 2.0 utan att G:s fungerande kod och filformat ändras i förtid.

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

VG-versionen utvecklas sedan den 20 oktober i `PragueParkingV2_1` och `PragueParkingV2_1.Data`. Arbetet bygger vidare på G-versionens struktur men använder egna källfiler, egen konfiguration och egna runtime-data. Den gemensamma dokumentationen redovisar G- och VG-status var för sig.

**Statusprincip:** *Implementerad* betyder att kod har tagits fram. *Testad* eller *verifierad* används endast när ett motsvarande test faktiskt har genomförts. G-versionens manuella tester den 14–16 oktober kvarstår som dokumenterad historik. Den 20 oktober byggdes solutionens fyra projekt och V2.1:s kopierade huvudmeny startades, men **de nya VG-flödena har inte genomgått fullständiga funktionstester eller omstartstester**. Kompilering visar inte att krav är uppfyllda. Ingen automatiserad testsvit eller separat körning på annan dator har redovisats.

---

## Projektfaser

Tabellen nedan visar läget den 20 oktober. Aktuell status och återstående arbete sammanfattas i inledningen.

| Fas | Arbetsområde | Historisk status 2026-10-20 |
|---|---|---|
| 1 | Kravanalys och spårbarhet | G-krav dokumenterade; VG-krav uppdateras mot den pågående implementationen |
| 2 | Solution och projektstruktur | G klar; två separata V2.1-projekt har skapats och byggts |
| 3 | Domänmodell | G implementerad och manuellt testad; VG: `IVehicle`, `Bicycle`, `Bus` och kapacitetsmetoder har tillkommit |
| 4 | JSON-konfiguration | G verifierad; VG-modellen `Size` och `SpotCapacity` påbörjad, full reload återstår |
| 5 | Parkering och sökning | G manuellt verifierad; VG:s storleksbaserade anrop håller på att migreras |
| 6 | Prisberäkning | G manuellt verifierad; Bicycle-/Bus-priser behöver integreras och testas |
| 7 | CheckoutService | G manuellt verifierad; VG:s nya storlekar och bussreservationer behöver hanteras |
| 8 | Spectre.Console och garageöversikt | G manuellt verifierad med 100/200/1 000 platser; VG-översikt behöver anpassas till storleksenheter |
| 9 | Huvudmeny och UI-flöden | G manuellt verifierad; V2.1 startar, men nya fordons- och administrationsflöden återstår |
| 10 | Parkeringspersistens | G manuellt verifierad; VG:s dataåterställning, fordonstyper och busslagring återstår |
| 11 | Administration | G: Reload Pricing verifierad; VG: omläsning av hela konfigurationen ej implementerad |
| 12 | Integrationstester och dokumentation | G-tester dokumenterade; VG-tester återstår och centrala dokument uppdateras fil för fil |
| 13 | Prague Parking 2.1 / VG | **Pågår** – separata projekt, interface och grund för storlekskapacitet; bussparkering ej implementerad |

---

## Milstolpe 1 – Arkitektur och domänmodell

**G-status: Genomförd på implementationsnivå**

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

Grundläggande parkering, sökning, borttagning, dubblettkontroll och prisberäkning har testats manuellt. Den 15 oktober tillkom `ParkingGarage.MoveVehicle()` med kontroller av ursprungsplats, destinationsplats och fordonskapacitet. Parkering, utcheckning och flytt har prövats via UI med JSON-sparning, och den 16 oktober verifierades dessutom skiftlägesokänslig dubblettkontroll genom UI samt riktad rollback vid misslyckad sparning. Detta är inte en fullständig transaktionsgaranti.

`Vehicle` innehåller stöd för en befintlig UTC-ankomsttid. Konstruktorsöverlagringar i `Car` och `MC` används nu av `ParkingDataMapper` för återställning med ursprunglig UTC-tid.

**V2.1 – pågående utbyggnad:** `IVehicle` har skapats med `Identifier` och `ArrivalTime`, och `Vehicle` implementerar gränssnittet. `Bicycle` och `Bus` har tillkommit med varsin konstruktor för nyregistrering respektive återställning. `ParkingSpot` har fått `GetUsedCapacity()` och nya, överlagrade metoder som använder `Func<IVehicle, int>` för att beräkna storlek. `ParkingGarage` har motsvarande nya överlagringar. Ett andra interface, exempelvis `IParkingSpot`, återstår, liksom att migrera de äldre anropen och testa funktionerna tillsammans.

---

## Milstolpe 2 – Konfiguration

**G-status: Grundläggande konfiguration samt menybaserad prisomläsning implementerad och manuellt verifierad**

`ConfigurationService` har implementerats.

Funktionalitet:

- JSON-inläsning.
- Standardkonfiguration när runtime-filen saknas.
- Validering av befintlig konfiguration.
- Garagekapacitet.
- Prisinställningar.
- Tidszon.
- Fordonskapacitet via `VehiclesPerSpot`.

En befintlig ogiltig konfigurationsfil får inte skrivas över automatiskt. **Administration → Reload Pricing fungerar utan omstart:** ändrat bilpris 20 → 30 CZK användes vid avgiftsberäkning; ogiltigt −10 CZK avvisades och tidigare giltiga priser bevarades. Standardvärdena återställdes. Fullständig omläsning av hela konfigurationen ingår först i VG.

**V2.1 – pågår:** `VehicleTypeConfiguration.Size` och `GarageConfiguration.SpotCapacity` har införts som grund för 1/2/4/16-enhetsmodellen. V2.1:s `config.json` har uppdaterats med fyra fordonstyper samt planerade priser. Validering av positiva `Size`-värden har påbörjats. `ConfigurationService` behöver fortfarande genomgås avseende standardvärden, övrig validering och en säker, fullständig omläsning under körning. G-versionens `VehiclesPerSpot` ska inte ändras i efterhand.

---

## Milstolpe 3 – Prisberäkning och Checkout

**G-status: Implementerad; utcheckning och riktad återställning vid sparfel verifierade manuellt**

`ParkingFeeCalculator`, `ParkingFeeResult`, `CheckoutService` och `CheckoutResult` har implementerats.

- Timpriser för `Car` och `MC` samt gratisminuter hämtas från konfigurationen.
- Avgift beräknas enligt reglerna för påbörjade timmar.
- `UpdatePricing()` kan validera och ersätta aktiva prisvärden.
- Utcheckning samordnar sökning, prisberäkning och borttagning.

`CheckOutVehicleView` använder `CheckoutService` och `ParkingFeeCalculator`, visar avgift och begär bekräftelse innan fordonet tas bort. Automatisk JSON-sparning efter utcheckning har testats: ett utcheckat fordon var fortsatt borttaget efter omstart. Den 16 oktober framkallades ett sparfel; `ABC123` blev kvar på sin ursprungsplats. Ytterligare gränsfall och generell kraschsäkerhet är inte verifierade.

**V2.1 – återstår:** Lägg till och testa prisreglerna för `Bicycle` (5 CZK/timme) och `Bus` (80 CZK/timme) enligt konfigurationen. Säkerställ att en buss debiteras som **ett** fordon och att utcheckning frigör alla fyra reserverade platser. G-versionens utcheckningstester är regressionsunderlag, inte VG-testresultat.

---

## Milstolpe 4 – User Interface

**G-status: Samtliga huvudsakliga G-vyer implementerade; flera terminal- och avbrytningsgränsfall kvarstår**

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

Temat används av huvudmenyn, garageöversikten, Park-, Check Out-, Move- och Find-vyerna samt Administration. De vanliga formulärvyerna använder centrerade paneler, normalt 62 tecken breda, medan garageöversikten använder terminalens bredd. Kompatibilitet med andra terminaler behöver fortfarande testas.

### MessageService

`MessageService` används för återanvändbara fel-, varnings-, informations- och bekräftelsemeddelanden. Den 15 oktober centrerades panelerna med `AppTheme.WriteCenteredPanel()`; den 16 oktober justerades även vänstermarginalen för `Press any key to continue...` vid `waitForKey: true` och resultatet kontrollerades visuellt. Felmeddelanden i Move-vyn visas utan att paneler staplas. Fullständig tangenthantering i andra terminaler återstår.

**Viktig regel:** Ett lyckat resultat får inte presenteras som permanent sparat om JSON-skrivningen misslyckas. Denna ordning används i Park-, Check Out- och Move-vyerna. Kompensationsförsök vid `IOException` eller `UnauthorizedAccessException` har **testats i riktade scenarier** den 16 oktober, men de utgör ingen fullständig transaktionsgaranti.

### Garage Overview

**G-status 2026-10-16: Kompakt layout verifierad manuellt med 100, 200 och 1 000 platser; intern viewport och navigering till slutet av garaget provade.**

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

**Verifierat 2026-10-16:** Vid **1 000 platser** visades en intern viewport och navigering till plats 1000, där markeringen stannade vid försök att gå längre. Navigering tillbaka mot början fungerade. Efter återställning till **100 platser** visades `FREE 98 / OCCUPIED 2 / VEHICLES 3` med MC på 003 och Car på 051. **Återstår:** tester vid fönsterstorleksändring under körning, andra terminalmiljöer, separat 500-platsersprov och eventuella återstående gränsfall för statusfärger/navigation.

### Spot Details

Informationspanelen är implementerad för att visa:

- Markerat parkeringsplatsnummer, status och beläggning.
- Tillgänglig kapacitet för `Car` och `MC` när platsen är ledig.
- Varje parkerat fordons typ och identifierare.
- Ankomsttid i den konfigurerade tidszonen.
- Parkeringstid (`Duration`) beräknad från UTC-ankomsttiden.

Panelen ligger längst till höger med mindre utrymme än parkeringskartan. Ledig plats och bil har kontrollerats visuellt, bland annat med `ABC123` och `UKB48K`. Två MC med separata identifierare och ankomsttider visades på samma plats (003) med beläggning 2/2; **båda återställdes efter omstart den 16 oktober**.

---

## Milstolpe 5 – UI-funktionalitet

**G-status: Huvudfunktionerna implementerade och manuellt verifierade; återstående gränsfall redovisas nedan. VG-integration pågår separat.**

Huvudmenyn öppnar `ParkVehicleView`, `CheckOutVehicleView`, `MoveVehicleView`, `FindVehicleView`, `GarageOverView` och `AdministrationView`. Park, Check Out och Move uppdaterar `parking-data.json` efter lyckade förändringar. **Find Vehicle** visar sökresultat utan att ändra data; **Administration** läser om priser från `config.json` utan omstart.

1. **Park Vehicle:** Incheckning av bil med automatisk JSON-sparning och återställning efter programomstart har verifierats. MC-fall har provats, men ytterligare negativa tester återstår.
2. **Check Out Vehicle:** Använder `CheckoutService`, visar parkeringsavgift och ett kvitto samt sparar borttagningen. Ett manuellt omstartstest bekräftade att det utcheckade fordonet inte återkom.
3. **Move Vehicle:** `ParkingGarage.MoveVehicle()` bevarar samma fordonsobjekt och ursprunglig `ArrivalTime`. `MoveVehicleView` använder centrerade paneler, validering före bekräftelse och en `while`-loop för nytt platsval. Bilflytt 001 → 050 sparades och kvarstod efter omstart. MC → MC med ledig kapacitet tilläts; MC → bilplats nekades.
4. **Find Vehicle:** Implementerad och verifierad med `ABC123`/`DEF123`, skiftlägesokänslig sökning, okänd identifierare och möjlighet att söka igen.
5. **Administration:** Implementerad med `Reload Pricing`; giltiga och ogiltiga prisvärden har testats utan att starta om programmet.

**Kvarstående kvalitetsarbete för G:** Riktade skrivfelstester och rollback är genomförda, men fler gränsfall (exempelvis avbrutna åtgärder, fullt garage och terminalstorlek) återstår. ESC-avbrytning direkt under ett `TextPrompt` är ännu inte implementerad; tomt Enter används för återgång i Move-vyn.

**V2.1 – pågår:** Huvudmenyn har startats i den nya projektkopian. `ParkVehicleView` har ändrats till att använda `Garage.SpotCapacity` och en `Func<IVehicle, int>` för fordonsstorlek. Övriga vyer, valbara fordonstyper, flytt/utcheckning och garageöversikt behöver migreras och funktionstestas. V2.1:s färg- och versionsprofil har påbörjats; inget komplett nytt UI-flöde redovisas som verifierat.

---

## Milstolpe 6 – Persistens

**G-status: Implementerad; normala JSON-flöden verifierade 2026-10-14–15 och riktade negativa tester/rollback genomförda 2026-10-16**

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
- `Save()` använder indenterad JSON samt temporärfil och `File.Move()`/`File.Replace()`. Den 11 oktober hade enbart serialiserad JSON granskats; den 14 oktober verifierades däremot en lyckad verklig sparning till befintlig fil genom omstartstest. Vid den tidpunkten återstod krasch- och feltester; riktade skrivfelstester genomfördes den 16 oktober.

### Verifiering och återstående begränsningar

- **Genomfört:** `ParkingDataMapper` återställer fordon med identifierare, plats och UTC-ankomsttid. Parkering, utcheckning och flytt använder `ToDocument()` och `Save()`; normala sparflöden har verifierats med omstart.
- **Genomfört 2026-10-16:** Två MC på samma plats efter omstart, korrupt JSON, dubbletter, felaktiga platsnummer, tre MC på samma plats och blandad Car/MC avvisades eller återställdes kontrollerat.
- **Genomfört 2026-10-16:** Sparfel provocerades fram via blockerande katalog `parking-data.json.tmp`, vilket testade kompensation vid Park, Check Out och Move.
- **Återstår:** Separata fall med saknade obligatoriska JSON-fält, identifierarlängdens gränser, detaljerad `.tmp`-städning och kraschsäker filersättning. Ingen generell transaktionsgaranti är bevisad.

**Historik 2026-10-14–15:** `Program.cs` laddar JSON via `ParkingDataMapper.ToGarage()`. Bilen `UKB48K` återkom efter parkering och omstart; vid efterföljande utcheckning var platsen ledig även efter omstart. Den 15 oktober flyttades `ABC123` från 001 till 050, och den nya platsen kvarstod efter omstart. Två MC kunde dela plats 003. Deras gemensamma placering efter omstart verifierades separat den 16 oktober. `ParkingDataService` ansvarar fortsatt för JSON och filåtkomst.

### V2.1 – återstående persistensarbete

- `ParkingDataMapper.ToVehicle()` och `ToVehicleData()` har utökats med `Bicycle` och `Bus`. Det är typmappning, inte ett verifierat lagringsflöde.
- `ParkingDataMapper.ToGarage()` använder ännu en gammal `VehiclesPerSpot`-baserad väg och ska migreras till `SpotCapacity` + `Size` innan återställningen kan testas med VG-modellen.
- JSON-valideringen i V2.1.Data behöver acceptera och kontrollera de nya fordonstyperna.
- En buss ska sparas som ett logiskt fordon, samtidigt som fyra intilliggande platser kan reserveras och återställas korrekt. Exakt intern-/JSON-representation är ännu inte beslutad.
- Om V2.1:s parkeringsdatafil **saknas** ska filen skapas **och fyllas med giltiga testfordon**. Befintlig felaktig JSON får inte tyst ersättas. Detta är ännu inte implementerat.
- V2.0:s dokumenterade JSON-format och redan testade återställning bevaras oförändrade.

---

## Milstolpe 7 – Administration

**G-status: Implementerad och verifierad via UI 2026-10-16**

Implementerad funktionalitet för G:

- Omläsning av prislistan från `config.json` via Administration → Reload Pricing.
- Validering av samtliga nya prisvärden.
- Tillämpning via samma `ParkingFeeCalculator.UpdatePricing()`-instans som används av Check Out.
- Bibehållna tidigare giltiga värden om omläsningen misslyckas.

**Manuellt test:** Bilpris 20 → 30 CZK började användas utan omstart. Värdet −10 CZK avvisades och föregående giltiga pris användes fortsatt. Prislistan återställdes till standardvärden.

**VG-utökning – ej implementerad:** Ett synligt menyval ska läsa om *hela* konfigurationen under körning, inklusive prisvärden, storlekar, antal platser och fordonsregler. Innan ändringarna aktiveras måste det aktuella garaget kunna behålla alla befintliga fordon och reservationer. Ogiltiga ändringar avvisas och tidigare giltig konfiguration gäller vidare. UI och prisberäkning ska därefter använda samma aktiva konfiguration.

---

## Milstolpe 8 – Testning

**G-status: Manuella tester genomförda 2026-10-16; vissa leverans- och gränsfall återstår**

Tidigare manuella domäntester finns noterade i `LOGGBOK.md`. Under den 14–16 oktober tillkom end-to-end-tester, negativa JSON-fall, riktade skrivfel/rollback och verifiering av 1 000-platsersöversikt. **Alla leverans- och terminalgränsfall är ännu inte slutverifierade.**

**Manuellt verifierat hittills:**

- Bilparkering med JSON-sparning och återställning efter omstart.
- Utcheckning via UI med beräknad avgift, sparad borttagning och bekräftelse efter omstart.
- Flytt av `ABC123` från plats 001 till 050, med bevarad fordonsidentitet och sparad placering efter omstart.
- Två MC på samma plats med rätt beläggning, samt nekad flytt av MC till bilplats.
- Olämpliga platsval fångas i Move-vyns `while`-loop och ett nytt platsnummer kan anges utan att börja om.
- `Garage Status` skiljer mellan upptagna platser och totalt antal fordon, exempelvis `FREE 198`, `OCCUPIED 2`, `VEHICLES 3`.

**Ytterligare verifierat 2026-10-16:**

- Två MC `DEF123` och `EDF123` på plats 003 återställdes efter omstart med bibehållna ankomsttider.
- Negativa JSON-fall: korrupt JSON, dubbletter, plats 201 vid 200 platser, tre MC på samma plats samt blandning av Car/MC.
- Rollback för Park (`ROLL123`), Checkout (`ABC123`) och Move (050 → 051) genom avsiktlig blockering av `.tmp`-sökvägen. Därefter sparades 050 → 051 och återställdes efter omstart.
- Find Vehicle med skiftlägesokänslig sökning, okänt fordon och omförsök. Administration med ändrat och avvisat pris.
- Garagekarta med 100, 200 och 1 000 platser; intern viewport och navigering till plats 1000. Standardkonfigurationen 100 återställdes, med `FREE 98 / OCCUPIED 2 / VEHICLES 3`.
- Centrering av fortsättningstext i `MessageService` korrigerad och kontrollerad.

**Återstående tester omfattar:**

- Avbrytningsval, fullt garage och andra ännu oredovisade UI-gränsfall.
- Vissa JSON-fall (utelämnade fält och gränslängd), `.tmp`-städning och kraschsäkerhet utöver de riktade testerna.
- Garageöversikt vid ändrad och mindre terminalstorlek; eventuella återstående navigeringsgränser och separat 500-platsersprov.
- Körning på en annan dator utan hårdkodade sökvägar samt fullständig G-/V1-kravavstämning.

Kravmatrisen i `requirements.md` uppdaterades med teststatus 2026-10-20. När ytterligare G- eller VG-tester genomförs ska projektplan och kravmatris hållas samstämmiga.

### V2.1 – faktisk teststatus 2026-10-20

- **Genomfört:** Byggning av alla fyra projekt utan kompileringsfel och uppstart av V2.1:s kopierade huvudmeny, före delar av den senare VG-kodningen.
- **Genomfört:** Lyckad kompilering efter de successivt införda interface-, klass-, konfigurations- och metodändringarna.
- **Inte genomfört:** Sammanhängande körtester av Bicycle, Bus, storleksbaserad samparkering, VG-JSON efter omstart, full config reload eller bussflytt/utcheckning.
- **Nästa testområden:** 1+1+1+1 Bicycle, 2 MC, Car som tar 4 enheter, kombinationer av fordon, full plats, gränsfall för fyra sammanhängande bussplatser inom 1–50, konflikter/dubbletter samt sparning, omstart och kontrollerade fel.
- **Regression:** Använd dokumenterade G-fall som referens när motsvarande funktioner har migrerats till V2.1. Ett grönt buildresultat ensamt ger inget godkänt VG-test.

---

## Milstolpe 9 – V2.1 efter dagens verifierade tester (2026-10-21)

**Genomfört:** Storleksbaserad kapacitet; fyra bussplatser; bil/MC/cykel/buss i gemensam modell; fungerande bussflytt/utcheckning; Sunset Garage Overview; Find Vehicle för nya typer; full Reload Configuration; automatisk seeding när datafil saknas; JSON-omstart.

**Testade scenarier:** buss 004–007 → 006–009 och 160 CZK vid checkout, testbuss 001–004 och delad plats MC+Bicycle på 006; garage från 100→200 och 500 platser utan omstart; nekad SpotCapacity 2 för Car på 002; fyra seeder-fordon sparas/läses korrekt efter omstart.

**Obligatoriskt kvarstående före slutleverans:** manuella negativa gränstester (bus rollback, fyra cyklar, ändrade konfigurationsgränser), kontroll av kod/referenser och portabilitet, slutlig individuell reflektion (2–3 sidor), README/kravkontroll samt Git-inlämningsformalia. En automatisk Live Demo i isolerad garagevy är endast **planerad** och valfri.

**Utvecklingsgräns för buss:** enligt lärarens direkta besked får Bus endast använda platserna 001–050 också vid flytt. Den konfigurerbara parametern `BusMaxParkingSpot` hålls 50 i kursinställningen. G-versionens kod ska inte ändras.
