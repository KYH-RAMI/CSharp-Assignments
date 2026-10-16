# Prague Parking V2 – Projektplan

**Uppdaterad:** 2026-10-16

**Projekt:** Prague Parking V2  
**Utvecklare:** Rami Issa  
**Utvecklingsmiljö:** Visual Studio Community 2026  
**Teknik:** C# / .NET 10 / Spectre.Console

**Arbetsordning:** Prague Parking 2.0 (G) färdigställs och verifieras innan Prague Parking 2.1 (VG) implementeras.

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

VG-versionen utvecklas separat när G-versionen är stabil. Dokumenterade VG-behov beaktas i arkitekturen, men implementeras inte i förtid.

**Statusprincip:** *Implementerad* betyder att kod har tagits fram. *Testad* eller *verifierad* används endast när ett motsvarande test faktiskt har genomförts. Planerade funktioner räknas inte som färdiga. Den 16 oktober utökades de manuella testerna med Find Vehicle, Administration, två MC efter omstart, negativ JSON-validering, riktade skrivfel/rollback och garageöversikt med 1 000 platser. **Ingen fullständig granskning av det lokala Git-repositoryt, automatiserad testsvit eller extern körmiljö har verifierats.**

---

## Projektfaser

| Fas | Arbetsområde | Status |
|---|---|---|
| 1 | Kravanalys | Kravanalys genomförd; status uppdaterad 2026-10-16 och VG-testdata kompletterat |
| 2 | Solution och projektstruktur | Genomförd |
| 3 | Domänmodell | Implementerad; grundläggande parkering, sökning, utcheckning och flytt manuellt testade |
| 4 | JSON-konfiguration | Implementerad; läsning, validering, standardvärden och Reload Pricing manuellt testade |
| 5 | Parkering och sökning i domänlagret | Implementerad; sökning via UI och negativa identifierartester manuellt verifierade |
| 6 | Prisberäkning | Implementerad; manuellt testad |
| 7 | CheckoutService | Implementerad; utcheckning, sparning och riktad rollback manuellt testade |
| 8 | Spectre.Console och garageöversikt | Implementerad; Grid/viewport testad med 100, 200 och 1 000 platser; vissa terminalgränser återstår |
| 9 | Main Menu och UI-flöden | Samtliga G-huvudflöden finns och har manuellt testats, inklusive Find och Administration |
| 10 | Parkeringspersistens | Implementerad; omstart, ogiltig JSON och riktade skrivfel/rollback testade; kraschsäkerhet ej generellt verifierad |
| 11 | Administrationsfunktioner | Administration → Reload Pricing implementerad och manuellt verifierad |
| 12 | Integrationstester och dokumentation | Pågår; omfattande manuella tester genomförda, leveransprov och återstående gränsfall kvar |
| 13 | Prague Parking 2.1 / VG | Ej påbörjad; VG-projekt ännu inte skapat |

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

Grundläggande parkering, sökning, borttagning, dubblettkontroll och prisberäkning har testats manuellt. Den 15 oktober tillkom `ParkingGarage.MoveVehicle()` med kontroller av ursprungsplats, destinationsplats och fordonskapacitet. Parkering, utcheckning och flytt har prövats via UI med JSON-sparning, och den 16 oktober verifierades dessutom skiftlägesokänslig dubblettkontroll genom UI samt riktad rollback vid misslyckad sparning. Detta är inte en fullständig transaktionsgaranti.

`Vehicle` innehåller stöd för en befintlig UTC-ankomsttid. Konstruktorsöverlagringar i `Car` och `MC` används nu av `ParkingDataMapper` för återställning med ursprunglig UTC-tid.

---

## Milstolpe 2 – Konfiguration

**Status: Grundläggande konfiguration samt menybaserad prisomläsning implementerad och manuellt verifierad**

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

---

## Milstolpe 3 – Prisberäkning och Checkout

**Status: Implementerad; utcheckning och riktad återställning vid sparfel verifierade manuellt**

`ParkingFeeCalculator`, `ParkingFeeResult`, `CheckoutService` och `CheckoutResult` har implementerats.

- Timpriser för `Car` och `MC` samt gratisminuter hämtas från konfigurationen.
- Avgift beräknas enligt reglerna för påbörjade timmar.
- `UpdatePricing()` kan validera och ersätta aktiva prisvärden.
- Utcheckning samordnar sökning, prisberäkning och borttagning.

`CheckOutVehicleView` använder `CheckoutService` och `ParkingFeeCalculator`, visar avgift och begär bekräftelse innan fordonet tas bort. Automatisk JSON-sparning efter utcheckning har testats: ett utcheckat fordon var fortsatt borttaget efter omstart. Den 16 oktober framkallades ett sparfel; `ABC123` blev kvar på sin ursprungsplats. Ytterligare gränsfall och generell kraschsäkerhet är inte verifierade.

---

## Milstolpe 4 – User Interface

**Status: Samtliga huvudsakliga G-vyer implementerade; flera terminal- och avbrytningsgränsfall kvarstår**

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

**Status 2026-10-16: Kompakt layout verifierad manuellt med 100, 200 och 1 000 platser; intern viewport och navigering till slutet av garaget provade.**

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

**Status: Pågår**

Huvudmenyn öppnar `ParkVehicleView`, `CheckOutVehicleView`, `MoveVehicleView`, `FindVehicleView`, `GarageOverView` och `AdministrationView`. Park, Check Out och Move uppdaterar `parking-data.json` efter lyckade förändringar. **Find Vehicle** visar sökresultat utan att ändra data; **Administration** läser om priser från `config.json` utan omstart.

1. **Park Vehicle:** Incheckning av bil med automatisk JSON-sparning och återställning efter programomstart har verifierats. MC-fall har provats, men ytterligare negativa tester återstår.
2. **Check Out Vehicle:** Använder `CheckoutService`, visar parkeringsavgift och ett kvitto samt sparar borttagningen. Ett manuellt omstartstest bekräftade att det utcheckade fordonet inte återkom.
3. **Move Vehicle:** `ParkingGarage.MoveVehicle()` bevarar samma fordonsobjekt och ursprunglig `ArrivalTime`. `MoveVehicleView` använder centrerade paneler, validering före bekräftelse och en `while`-loop för nytt platsval. Bilflytt 001 → 050 sparades och kvarstod efter omstart. MC → MC med ledig kapacitet tilläts; MC → bilplats nekades.
4. **Find Vehicle:** Implementerad och verifierad med `ABC123`/`DEF123`, skiftlägesokänslig sökning, okänd identifierare och möjlighet att söka igen.
5. **Administration:** Implementerad med `Reload Pricing`; giltiga och ogiltiga prisvärden har testats utan att starta om programmet.

**Kvarstående kvalitetsarbete:** Riktade skrivfelstester och rollback är genomförda, men fler gränsfall (exempelvis avbrutna åtgärder, fullt garage och terminalstorlek) återstår. ESC-avbrytning direkt under ett `TextPrompt` är ännu inte implementerad; tomt Enter används för återgång i Move-vyn.

---

## Milstolpe 6 – Persistens

**Status: Implementerad; normala JSON-flöden verifierade 2026-10-14–15 och riktade negativa tester/rollback genomförda 2026-10-16**

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

---

## Milstolpe 7 – Administration

**Status: Implementerad och verifierad via UI 2026-10-16**

Implementerad funktionalitet för G:

- Omläsning av prislistan från `config.json` via Administration → Reload Pricing.
- Validering av samtliga nya prisvärden.
- Tillämpning via samma `ParkingFeeCalculator.UpdatePricing()`-instans som används av Check Out.
- Bibehållna tidigare giltiga värden om omläsningen misslyckas.

**Manuellt test:** Bilpris 20 → 30 CZK började användas utan omstart. Värdet −10 CZK avvisades och föregående giltiga pris användes fortsatt. Prislistan återställdes till standardvärden.

**VG-utökning:** Omläsning av *hela* konfigurationen under körning, inklusive validering mot redan parkerade fordon. Detta ska inte förväxlas med G-versionens prisomläsning.

---

## Milstolpe 8 – Testning

**Status: Pågår – väsentligt fler manuella testfall genomförda 2026-10-16**

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

Kravmatrisen i `requirements.md` uppdaterades med teststatus 2026-10-16. När ytterligare G- eller VG-tester genomförs ska projektplan och kravmatris hållas samstämmiga.

---

## Milstolpe 9 – Prague Parking 2.1

**Status: Ej påbörjad (planering och kravförtydliganden dokumenterade)**

Planerade VG-funktioner:

- Separat körbart 2.1-projekt i samma solution och samma GitHub-repository som 2.0.
- Minst två interfaces.
- `Bicycle` och `Bus`.
- Storleksbaserad parkeringskapacitet: Bicycle = 1, MC = 2, Car = 4, Bus = 16; en normal plats = 4 enheter.
- **Bus:** Fyra sammanhängande lediga platser; de första 50 platserna reserveras som bussområde enligt lärarens förtydligande, utan separat takhöjdsmodell.
- **Flytt av Bus:** Identitet och ankomsttid bevaras. Det är ännu inte fastställt om flytt till platsnummer över 50 ska tillåtas; den regeln får inte framställas som godkänd.
- Buss representeras som **ett** fordon även när den upptar fyra platser.
- Timpriser: Bicycle 5 CZK och Bus 80 CZK.
- Fordonstyper och storlekar konfigureras enligt VG-kraven.
- Omläsning av hela konfigurationen med säker validering mot aktuellt garagetillstånd.
- Automatiskt skapande **och ifyllning med giltiga testfordon** om parkeringsdatafilen saknas (VG-krav).
- Personlig `reflektion.md` enligt uppgiftskraven, skriven **sist** efter färdig implementation och test av både G och VG.

Bussens framtida representation i `parking-data.json` är **inte beslutad**. Den ska hantera fyra upptagna platser utan att behandla en buss som fyra olika fordon, samtidigt som befintlig G-data kan läsas.

---

## Närmaste utvecklingsmål (uppdaterade 2026-10-16)

### Prioritet 1 – Slutlig G-avstämning

Kontrollera kvarvarande skillnader mot lärarens krav och V1, särskilt gratisgränsen vid exakt 10 minuter. Genomför endast de ännu relevanta gränstesterna: avbrutna åtgärder, fullt garage, ogiltiga obligatoriska JSON-fält och vid behov ändrade terminalstorlekar.

### Prioritet 2 – Förbered separata körbara G- och VG-projekt

Behåll 2.0 fungerande utan att införa VG-specifika ändringar där. Skapa därefter ett separat 2.1-projekt i **samma solution och GitHub-repository**; återanvänd lämplig kod och dokumentera eventuella nödvändiga skillnader.

### Prioritet 3 – Arkitektur och kapacitet för VG

Inför minst två interfaces som faktiskt används av implementationen samt typerna `Bicycle` och `Bus`. Inför kapacitet i storleksenheter: 1/2/4/16 och 4 per vanlig plats. Bus måste uppta fyra sammanhängande platser. Plats 001–050 utgör enligt lärarens besked reserverat bussområde; regeln för flytt utanför detta område behöver fastställas före kodning.

### Prioritet 4 – VG-konfiguration och testdata

Lägg till Bicycle-/Bus-priser, konfigurerbara storlekar och full omläsning/validering av konfiguration under körning, utan att befintliga fordon blir ogiltiga. Om parkeringsdatafilen saknas ska 2.1 automatiskt **skapa och fylla den med giltiga testfordon**.

### Prioritet 5 – Dokumentation, leverans och Git

Samordna `requirements.md`, `project-plan.md`, `README.md`, `LOGGBOK.md` och övriga docs efter faktiska kod- och testresultat. Verifiera körning på annan dator och startinstruktioner. Kontrollera `git status`, `.gitignore` och vilka ändringar som ska ingå **före** en gemensam commit; ingen commit antas vara gjord.

### Prioritet 6 – Personlig reflektion och inlämning

Skriva `reflektion.md` **sist**, efter att både G och VG är färdigutvecklade och testade, med den befintliga loggboken som underlag. Därefter slutkontroll av publikt GitHub-repository och inlämningslänk. Frivillig distribution/betatestning kan ske efter kärnkraven.

---

## Risker

### Console Rendering

Olika terminaler kan hantera ANSI-sekvenser, färger, tangentbordsinmatning och fönsterstorlekar olika. Den kompakta `Grid`-layouten har testats med 1 000 platser och intern viewport, men behöver fortfarande testas vid små terminaler, fönsterstorleksändring och långa fordonsuppgifter i den smala detaljpanelen.

### Data Persistence

Parkeringsdata kan gå förlorade vid felaktig filskrivning eller återställning. Riktad felinjektion den 16 oktober visade fungerande kompensation vid Park, Check Out och Move för ett avsiktligt blockerat `.tmp`-filnamn. Detta är **inte** ett bevis på fullständig atomisk transaktion eller kraschsäkerhet. Försiktighet behövs även vid framtida samtidiga uppdateringar.

### Configuration

Ogiltig befintlig konfiguration får inte ersättas automatiskt. Ändringar under körning ska inte lämna prisreglerna i ett delvis uppdaterat tillstånd.

### Scope Management

G-versionen ska färdigställas före VG. Publicering, Docker, Cloudflare och fleranvändarstöd är inte en del av den aktuella G-implementationen.

### Korrekt projektstatus

Att en klass eller vy har kodats innebär inte att den är integrationstestad. Dokumentation och kravmatris får inte markera funktioner som verifierade utan genomförda tester. Slutsatser om Git-status, körning på annan dator eller fullständig kraschsäkerhet får inte dras från enbart manuella tester i utvecklingsmiljön.

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

Dokumenten ska beskriva **samma utvecklingsläge**. Historiska aktiviteter redovisas i loggboken; framtida arbete ska inte framställas som genomfört. Dokumentationsfilerna levereras **var för sig** och hålls samstämmiga när VG-arbetet fortsätter. En gemensam Git-commit görs först när kod och berörd dokumentation har granskats tillsammans. Kontrollera faktiskt `git status` och `git log` före commit; texten här bekräftar inte att något har skickats till GitHub.

---

## Framtida distribution och betatestning

**Status: Idé och planering; inte del av G-kravens aktuella implementation**

När G och VG är stabila kan en körbar Windows-version publiceras för test utan att klasskamraterna behöver bygga källkoden. Det kräver bland annat kontroll av filvägar, konfigurationsfiler och körning på en annan dator.

En möjlig separat DevSecOps-utökning är att köra programmet i en isolerad Docker-miljö på Synology NAS med webbaserad terminal och tillfällig Cloudflare-adress. Klasskamraterna ska i så fall bara nå applikationen, inte NAS-administrationen eller Tailscale-nätet. Alla testare ska se samma parkeringsdata. Detta kräver central samordning av ändringar; flera oberoende processer får inte skriva okontrollerat till samma JSON-fil.

Feedback samlas via Discord. Ingen driftsättning eller fleranvändarlösning är implementerad eller testad i nuläget.
