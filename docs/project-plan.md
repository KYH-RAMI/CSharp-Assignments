# Prague Parking V2 – Projektplan

**Uppdaterad:** 2026-10-20

**Projekt:** Prague Parking V2  
**Utvecklare:** Rami Issa  
**Utvecklingsmiljö:** Visual Studio Community 2026  
**Teknik:** C# / .NET 10 / Spectre.Console

**Arbetsordning:** Prague Parking 2.0 (G) är färdigutvecklad och manuellt testad. Prague Parking 2.1 (VG) utvecklas nu i separata projekt i samma solution. G-versionens kod och dokumenterade testresultat bevaras som utgångspunkt för regressionstester.

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

| Fas | Arbetsområde | Status 2026-10-20 |
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

## Milstolpe 9 – Prague Parking 2.1

**Status 2026-10-20: Pågår. Separata projekt har skapats och kompilerats; VG-funktionaliteten är inte färdigintegrerad eller funktionstestad.**

### Genomfört hittills

- Skapat `PragueParkingV2_1` och `PragueParkingV2_1.Data` som separata .NET 10-projekt i samma solution och kopierat relevant G-kod som utgångspunkt.
- Bytt namnrymder i V2.1 och justerat programtitlar/menyrubriker så att versionerna går att skilja åt.
- Skapat interfacet `IVehicle` och låtit `Vehicle` implementera det. Skapat `Bicycle` och `Bus`, även för återställning med UTC-ankomsttid.
- Infört `Size` och `SpotCapacity` i den nya konfigurationsmodellen; uppdaterat V2.1:s JSON-inställningar till Bicycle = 1, MC = 2, Car = 4, Bus = 16 och normal platskapacitet = 4.
- Lagt till `ParkingSpot.GetUsedCapacity()` samt storleksbaserade överlagringar av `CanPark()`/`ParkVehicle()`.
- Lagt till motsvarande överlagringar av `FindAvailableSpot()`, `ParkVehicle()` och `ParkVehicleAtSpot()` i `ParkingGarage`.
- Ändrat `ParkVehicleView` för att anropa ny kapacitetsmodell med `Func<IVehicle, int>`; utökat JSON-mapperns typkonvertering med Bicycle/Bus.
- Byggt solutionen och V2.1:s nya metoder utan kompileringsfel. Funktionstester av de nya flödena återstår.

### Återstående delmål

| Område | Avgränsat återstående arbete | Status |
|---|---|---|
| Interfacebaserad modell | Lägg till ett andra meningsfullt interface (t.ex. `IParkingSpot`) och använd gränssnitten där de tillför nytta | Pågår |
| Kapacitet | Migrera `ParkingDataMapper.ToGarage()` och kvarvarande `VehiclesPerSpot`-beroenden; avveckla överflödiga gamla överlagringar **i V2.1** efter test | Pågår |
| Bicycle | Integrera meny, registrering, kapacitet, prissättning, JSON och tester | Pågår |
| Bus – tilldelning | Sök från plats 1 till högst startplats 47 efter **fyra helt lediga sammanhängande platser** inom 1–50; avvisa om ingen grupp finns | Ej implementerad |
| Bus – representation | Registrera en buss som **ett fordon** men reservera fyra platser; sökning, flytt, avgift, utcheckning och återställning ska frigöra/återskapa rätt reservationer | Ej implementerad |
| Priser | Integrera Bicycle 5 och Bus 80 CZK/timme i beräkning, validering och UI | Pågår |
| Konfiguration | Färdigställ standardvärden/validering och säkra fullständig omläsning under drift utan att förstöra befintliga parkeringar | Pågår |
| Testdata | Skapa och fyll saknad V2.1-datafil med giltiga testfordon | Ej implementerad |
| Verifiering | Kör nya VG-tester och återanvänd relevanta G-regressionstester, särskilt JSON/omstart/felfall | Ej genomförd för VG-helheten |
| Dokumentation och reflektion | Uppdatera gemensamma dokument, följ sedan upp en personlig reflektion på cirka 2–3 sidor **när projektet är färdigt** | Pågår |

**Regel för bussområdet:** Endast bussar begränsas till parkeringsplatserna **1–50** vid nyparkering. **Car, MC och Bicycle får också använda dessa platser.** De är alltså inte exklusivt reserverade för bussar. Bus ska vara ett fordon med fyra reservationer, inte fyra duplicerade fordonsobjekt. Flyttreglerna ska stämmas av mot uppgiftens exakta krav innan implementationen fastställs.

**Arkitekturfråga under utredning:** Hur de fyra reservationerna representeras i minnet och i `parking-data.json` utan att skapa dubbletter, dubbeldebitering eller svårigheter vid flytt och utcheckning. Lösningen är inte beslutad eller testad.

---

## Närmaste utvecklingsmål (uppdaterade 2026-10-20)

### Prioritet 1 – Dokumentation och första Git-kontrollpunkten

Gå igenom centrala dokument **en fil i taget** och säkerställ att V2.0 och V2.1 beskrivs korrekt. Kontrollera ändringarna i Visual Studios **Git Changes**, att `bin/`, `obj/`, `.vs/` och genererade runtime-data inte följer med, och skapa en tydlig commit för arbetet hittills. Dagens V2.1-arbete har ännu inte committats eller pushats enligt dokumenterad Git-status.

### Prioritet 2 – Slutför migration till storleksbaserad kapacitet

Börja med `ParkingDataMapper.ToGarage()` och ersätt gamla kapacitetsanrop med `SpotCapacity` och storleksuppslagning via `Func<IVehicle, int>`. Migrera sedan flytt, utcheckning och garageöversikt stegvis. Testa innan äldre överlagringar och `VehiclesPerSpot`-anrop tas bort ur **V2.1**. Lämna G-koden intakt.

### Prioritet 3 – Bicycle och grundläggande VG-regression

Slutför `Bicycle` i meny, domänlogik, prisberäkning, JSON-validering och återställning. Lägg till det andra interfacet där det faktiskt förenklar kodens beroenden. Verifiera att Car/MC fortfarande fungerar med den nya kapacitetsmodellen.

### Prioritet 4 – Bussparkering: fyra sammanhängande platser

Implementera och testa sökning efter fyra helt lediga platser inom **1–50**. Modellera buss som ett logiskt fordon med fyra reserverade platser. Därefter integreras flytt, utcheckning och JSON-sparning/återställning. Undvik delvis genomförda reservationer vid fel och bevara identifierare och UTC-ankomsttid. Tydliggör flyttreglerna innan de låses i kod.

### Prioritet 5 – Full konfigurationsomläsning och testdata

Färdigställ konfigurationsstandardvärden och validering för alla fyra typerna. Lägg till omläsning av hela konfigurationen via UI med kontroll mot aktuella parkeringar och sammanhängande bussreservationer. Implementera också VG-kravet att en saknad parkeringsdatafil ska skapas och fyllas med giltiga testfordon.

### Prioritet 6 – Testning, leverans och personlig reflektion

Kör dokumenterade VG-funktionstester och relevanta G-regressionstester, inklusive omstart och negativa fall. Stäm slutligen av kvarvarande kurskrav för G, exempelvis körning på annan dator och startinstruktioner, utan att förväxla detta med frivillig distribution. Uppdatera dokument och kravstatus; sammanställ den personliga VG-reflektionen från loggbokens **egna** reflektioner först efter färdig implementation och testning. Granska Git-historiken och slutför inlämningen enligt kurskraven.

---

## Risker

### Bussens fyra reservationer

En buss måste kunna lokaliseras, flyttas, checkas ut och återställas som **ett logiskt fordon** samtidigt som fyra sammanhängande parkeringsplatser blockeras. Om endast en av fyra platser ändras vid ett fel kan garagekartan och JSON-data bli inkonsekventa. Sökningen måste hålla sig inom de tillåtna platsnumren och kontrollera att samtliga fyra rutor är helt tomma.

### Pågående migrering och regression

Under övergången finns både `VehiclesPerSpot`-baserade och storleksbaserade metodöverlagringar i V2.1. Risken är att olika UI- och lagringsflöden använder olika regler. Migrera ett anrop i taget, verifiera funktionaliteten och ta först därefter bort gamla V2.1-metoder. Ändra inte G-logiken.

### Visual Studio och lokala inställningar

Visual Studio 2026 kraschade den 20 oktober efter att flera teman hade installerats. Orsakssambandet är inte fastställt, men inställningar och tillägg behövde återställas. Formateringskommandon och tangentbordsprofil gav ytterligare tidsåtgång. Prioritera en stabil utvecklingsmiljö och dokumentera viktig projektkonfiguration oberoende av IDE-teman.

### Console Rendering

Olika terminaler kan hantera ANSI-sekvenser, färger, tangentbordsinmatning och fönsterstorlekar olika. Den kompakta `Grid`-layouten har testats med 1 000 platser och intern viewport, men behöver fortfarande testas vid små terminaler, fönsterstorleksändring och långa fordonsuppgifter i den smala detaljpanelen.

### Data Persistence

Parkeringsdata kan gå förlorade vid felaktig filskrivning eller återställning. Riktad felinjektion den 16 oktober visade fungerande kompensation vid Park, Check Out och Move för ett avsiktligt blockerat `.tmp`-filnamn. Detta är **inte** ett bevis på fullständig atomisk transaktion eller kraschsäkerhet.

### Configuration

Ogiltig befintlig konfiguration får inte ersättas automatiskt. V2.1:s fullständiga omläsning får inte aktivera en konfiguration som gör parkerade fordon, tilldelade platsnummer eller en buss fyra reservationer ogiltiga. Vid fel ska tidigare giltig konfiguration behållas.

### Scope Management

G-versionen hålls stabil medan VG utvecklas separat. Frivilliga funktioner och externa driftsättningsidéer ingår inte i den aktuella arbetsplanen; prioritera kurskrav, begriplig kod, tester och dokumentation.

### Korrekt projektstatus

Att en klass eller vy har kodats eller kompilerat innebär inte att den är integrationstestad. Dokumentation och kravmatris får inte markera VG-funktioner som verifierade utan genomförda tester. V2.1:s nya filer är ännu inte committade enligt `git status --short` den 20 oktober. Slutsatser om körning på annan dator eller fullständig kraschsäkerhet får inte dras från enbart manuella tester i utvecklingsmiljön.

---

## Dokumentation

Utvecklingsarbetet dokumenteras löpande i den befintliga centrala strukturen:

- [`LOGGBOK.md`](../Documentation/LOGGBOK.md) – datum, genomfört arbete, tester, problem och **personliga reflektioner i jag-form**.
- [`requirements.md`](requirements.md) – spårbara G-/VG-krav, tolkning och faktisk status.
- [`design-decisions.md`](design-decisions.md) – motiverade arkitektur- och modellbeslut.
- [`class-diagram.md`](class-diagram.md) – domänmodell, interfaces och klassrelationer.
- [`configuration.md`](configuration.md) – konfigurationsformat, kapacitet och omläsningsregler.
- [`parking-data.md`](parking-data.md) – G:s JSON-format samt VG:s planerade utökningar och felhantering.
- [`README.md`](../Documentation/README.md) – gemensam projektöversikt och instruktioner för att köra respektive version.

De sex ämnesdokumenten ligger kvar i `docs/`. `README.md` och `LOGGBOK.md` ligger i `Documentation/`. Dokumenten ska beskriva **samma utvecklingsläge** utan att tidigare G-testresultat blandas ihop med V2.1:s pågående arbete.

Det aktuella arbetssättet är att granska och uppdatera **en dokumentationsfil i taget**, redovisa vad som ändrats och först därefter granska Git-ändringarna i Visual Studio. Historiska händelser och tidigare idéer i loggboken får finnas kvar som historik, men de ska inte finnas kvar som aktiva projektmål om de inte längre planeras.
