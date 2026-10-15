# Prague Parking V2 – Designbeslut

**Uppdaterad:** 2026-10-15  
**Omfattning:** Arkitektur- och implementationsbeslut för Prague Parking 2.0 (G), med dokumenterade avgränsningar inför Prague Parking 2.1 (VG).

Besluten nedan beskriver både genomförda arkitekturval och planerade lösningar. Där implementation eller tester återstår anges det uttryckligen. Beslut om ett framtida arbetssätt innebär inte att funktionen redan finns i programmet.

## DD-001 – Separation mellan UI, domän och dataåtkomst

**Beslut:** Presentation, affärsregler och dataåtkomst hålls separerade.

**Motivering:** Gränssnittet kan ändras utan att fordon och parkeringsregler påverkas.

JSON-hantering placeras i det separata Data-projektet. Dataåtkomst ska inte blandas med UI-kod eller domänregler.

## DD-002 – Gemensam abstrakt basklass för fordon

**Beslut:** `Vehicle` är abstrakt och används som basklass för `Car` och `MC`.

**Motivering:** Gemensam identifierare och ankomsttid ska inte dupliceras mellan fordonstyper.

## DD-003 – Generell fordonsidentifierare

**Beslut:** Fordon identifieras med `Identifier` i stället för en egenskap som förutsätter registreringsnummer.

**Motivering:** Framtida fordonstyper, exempelvis `Bicycle`, kan sakna registreringsnummer.

För `Car` och `MC` motsvarar identifieraren normalt registreringsnummer.

## DD-004 – Parkeringsplatsen ansvarar för sina fordon

**Beslut:** `ParkingSpot` har en privat lista över parkerade fordon och exponerar den som `IReadOnlyList<Vehicle>`.

**Motivering:** UI och garageklass ska kunna läsa beläggningen utan att direkt manipulera parkeringsplatsens interna lista.

`IReadOnlyList<T>` begränsar vad en anropare kan göra genom egenskapen. Den interna samlingen hanteras fortsatt av `ParkingSpot`.

## DD-005 – Fordon lagrar inte parkeringsplatsnummer

**Beslut:** `Vehicle` lagrar inte `SpotNumber`.

**Motivering:** Fordonets placering avgörs av vilken `ParkingSpot` som innehåller objektet.

Detta undviker dubbellagrad information i G-versionen. VG-versionens bussar kräver en särskild modell för beläggning av flera platser; det beslutet behandlas i DD-026.

## DD-006 – Garageklassen samordnar placering och sökning

**Beslut:** `ParkingGarage` skapar och söker bland platserna, hittar första lämpliga platsen, parkerar, tar bort och flyttar fordon.

**Motivering:** En parkeringsplats hanterar sina lokala regler medan garaget hanterar hela samlingen.

`ParkingGarage` kontrollerar även om en fordonsidentifierare redan finns innan en ny parkering genomförs. `MoveVehicle()` samordnar validering av ursprungs- och destinationsplats samt själva förflyttningen. Se DD-030.

## DD-007 – Kapacitetsmodell i G-versionen

**Beslut:** Antalet fordon per parkeringsplats konfigureras separat för `Car` och `MC` via `VehiclesPerSpot`.

**Motivering:** G-versionen kan hantera en bil eller två MC per plats utan att införa VG-versionens generella storleksenheter i förtid.

Fordonskapaciteten hämtas från konfigurationen och skickas till parkeringslogiken. Olika fordonstyper blandas inte på samma plats i den nuvarande G-modellen.

## DD-008 – JSON som filformat

**Beslut:** Konfiguration och permanenta parkeringsdata lagras i JSON.

**Motivering:** Formatet är läsbart, kan hanteras med `System.Text.Json` och uppfyller projektets filkrav.

Inläsning, domänåterställning och automatisk JSON-sparning vid incheckning, utcheckning och flytt är implementerade. Normala flöden har manuellt verifierats, medan avsiktligt framprovocerade skrivfel och vissa ogiltiga datafall återstår att testa.

## DD-009 – Separata filer för konfiguration och parkeringsdata

**Beslut:** `config.json` innehåller inställningar och prislista, medan `parking-data.json` ska innehålla parkerade fordon.

**Motivering:** Administrativa inställningar och föränderliga parkeringsdata har olika livscykler.

## DD-010 – Spara endast upptagna platser

**Beslut:** Endast platser med parkerade fordon sparas.

**Motivering:** Tomma parkeringsplatser kan återskapas utifrån konfigurerat garageantal.

**Status:** `ParkingDataMapper.ToDocument()` konverterar ett `ParkingGarage` till ett dokument som endast innehåller upptagna platser. Normal sparning och återställning efter omstart är manuellt verifierade. Se `parking-data.md` och DD-025.

## DD-011 – UTC för tidpunkter

**Beslut:** `ArrivalTime` registreras och används internt i UTC.

**Motivering:** Beräkningarna ska vara konsekventa oberoende av lokala tidsomställningar.

Tidszon för presentation anges i konfigurationen, med `Europe/Prague` som standard. JSON-inläsning med `ArrivalTime.Kind = Utc` och återställning av sparade bilar med oförändrad ankomsttid har testats. Vid flytt används samma fordonsobjekt, vilket bevarar `ArrivalTime`.

## DD-012 – Samlad JSON-konfiguration med priser först

**Beslut:** Prislista, garagekapacitet, tidszon och fordonsregler finns i `config.json`, med prisuppgifterna tidigt i filen.

**Motivering:** Administratören kan ändra priser och inställningar utan att kompilera om programmet.

Nya filvärden börjar inte automatiskt användas av en redan körande instans. Omläsning hanteras separat.

## DD-013 – Skapa saknad runtime-konfiguration

**Beslut:** Om programmets runtime-`config.json` saknas får en standardkonfiguration skapas.

**Motivering:** Första starten kan ske med dokumenterade standardvärden.

Detta gäller runtime-filen, inte en automatisk omskrivning av projektets ursprungliga konfigurationsfil.

## DD-014 – Avvisa felaktig befintlig konfiguration

**Beslut:** Ogiltig befintlig JSON-konfiguration ska inte tyst skrivas över.

**Motivering:** Användaren ska få ett kontrollerat fel i stället för att förlora sina ändringar.

## DD-015 – Separat prisberäkning

**Beslut:** `ParkingFeeCalculator` hanterar prisregler och avgiftsberäkning, inte `ParkingGarage`.

**Motivering:** Ansvarsfördelningen gör prislogiken oberoende av fordonsplaceringen.

## DD-016 – Prisvärden hålls i kalkylatorinstansen

**Beslut:** Aktiva priser och gratisminuter laddas vid start och kan ändras genom `UpdatePricing()` efter validering.

**Motivering:** Nya priser ska kunna tas i bruk utan att bygga om hela domänmodellen.

Menybaserad omläsning av prislistan återstår för G-versionen. Omläsning av **hela** konfigurationen tillhör VG-kraven.

## DD-017 – Read-only resultat för prisberäkning

**Beslut:** `ParkingFeeCalculator.CalculateParkingFee()` returnerar `ParkingFeeResult`.

**Motivering:** UI får ett strukturerat resultat med parkeringstid, timpris, gratisminuter och avgift utan att innehålla prislogik.

## DD-018 – Checkout samordnas av en separat service

**Beslut:** `CheckoutService` ansvarar för sökning, prisberäkning och borttagning, i den ordningen.

**Motivering:** Fordonet ska inte tas bort innan en korrekt avgift har kunnat beräknas.

**Status 2026-10-15:** `CheckOutVehicleView` är integrerad med avgiftsberäkning, bekräftelse och JSON-sparning. Ett normalt utcheckningsflöde har verifierats efter omstart; återställning vid framprovocerat skrivfel återstår att testa. Se DD-029.

## DD-019 – Strukturerat utcheckningsresultat

**Beslut:** `CheckoutService` returnerar `CheckoutResult`.

`CheckoutResult` och `ParkingFeeResult` sparas inte som parkeringsdata.

**Motivering:** Resultaten tillhör det aktuella användarflödet och ska kunna presenteras i UI utan att lagras som permanent garagetillstånd.

---

## DD-020 – Dynamisk garageöversikt med kompakt Grid och intern viewport

**Senast reviderat:** 2026-10-13

### Beslut

`GarageOverView` använder en enda kompakt Spectre.Console-`Grid` **inuti en `Panel`** för parkeringskartan. Parkeringsplatserna visas som färgade nummer, **utan individuella panelramar eller tabellens radavgränsare**. En separat `SPOT DETAILS`-panel placeras längst till höger och `GARAGE STATUS` visas under båda panelerna.

Kartan och detaljpanelen fördelar tillgänglig terminalbredd med **cirka 80/20 som utgångspunkt**. Detaljpanelen får dock minst 23 tecken enligt den aktuella layoutberäkningen; procentfördelningen är därför inte exakt i smalare terminaler. Terminalen behöver vara minst **80 kolumner bred och 18 rader hög** för att denna vy ska öppnas i nuvarande implementation.

### Bakgrund och motivering

Lösningen har ändrats i flera steg under utvecklingen:

- **9–10 oktober:** Ett 5 × 5-rutnät med 25 platser per sida och individuella `Panel`-komponenter användes. Det gav tydliga ramar men utnyttjade skärmytan dåligt.
- **12 oktober:** En mer kompakt `Table` testades för att visa fler platser samtidigt. Tabellens cellpadding, kantlinjer och radavgränsare gjorde det svårt att beräkna bredd och höjd korrekt.
- **13 oktober:** Lösningen byggdes om till en kompakt `Grid` utan en separat `Panel` per parkeringsplats. Målet var att visa så mycket som möjligt av garaget utan att terminalen scrollar, och samtidigt behålla `SPOT DETAILS` fullt synlig.

Den nya presentationen är ett **UI-beslut**, inte en ändring av parkeringsregler eller av domänklasserna.

### Nuvarande implementation

- Platserna hämtas från `ParkingGarage.ParkingSpots` och sorteras efter `SpotNumber`.
- `GetGarageLayout()` beräknar kartbredd, detaljbredd, antal kolumner och synliga rader utifrån terminalens rapporterade bredd/höjd och aktuellt antal parkeringsplatser.
- `BuildGarageGrid()` skapar ett kompakt rutnät där varje plats upptar en liten, obruten cell med ett tresiffrigt eller bredare platsnummer.
- `CreateParkingSpotCell()` använder `Markup` för statusfärgad bakgrund. `GetSpotColor()` avgör färgen med `AppConfiguration.VehicleTypes` och faktisk beläggning: **grön = ledig**, **orange = delvis upptagen**, **röd = full**.
- Vald plats markeras med **guldtext**, medan bakgrundsfärgen fortsätter att visa platsens parkeringsstatus.
- `SPOT DETAILS` visar vald plats, status och beläggning. Vid parkerade fordon visar den dessutom fordonstyp, identifierare, lokalt presenterad ankomsttid och beräknad parkeringstid.
- `GARAGE STATUS` går över hela den sammansatta layoutens bredd och visar totalt antal lediga/upptagna platser och parkerade fordon. När kartan är begränsad till ett synligt intervall anges det intervallet i footern.
- `AnsiConsole.Live()` används för att uppdatera garagevyn vid navigering utan att rensa hela konsolen vid varje tangenttryckning. Detta ska inte förväxlas med huvudmenyns positionsbaserade navigering i `AppTheme.ShowMenu()`.

### Navigation och visningsbegränsning

Piltangenterna använder **samma beräknade `ColumnsPerRow`** som rutnätet. Upp/ned flyttar markeringen en visuell rad; vänster/höger flyttar markeringen inom samma rad. `ESC` återgår från garageöversikten. Den tidigare sidindelningen med `Page Up`/`Page Down` är **inte längre den aktuella navigeringsmodellen**.

**Alla platser visas samtidigt när utrymmet räcker.** Om kartan blir högre än tillgänglig terminalhöjd används en **intern viewport**: ett sammanhängande intervall av rader visas kring den valda platsen. Navigering mellan platser kan då ändra det synliga intervallet utan att användaren behöver scrolla själva terminalen. Detta är inte en fast indelning i sidor med 25 eller 100 platser.

Layouten beräknas om vid tangenttryckningar, även efter en eventuell ändring av terminalstorlek. Den kan däremot inte garantera att ett stort garage, exempelvis 1 000 platser, visas **helt samtidigt** på varje skärm. En kompakt Grid minskar utrymmesbehovet men tar inte bort terminalens fysiska begränsningar.

### Status och återstående tester

Den aktuella vyn har **visuellt kontrollerats med 200 parkeringsplatser** i ett tillräckligt stort terminalfönster. Den 13 oktober visades samtliga platser som lediga utan att kartan kapades. Den 14–15 oktober har översikten också kontrollerats med parkerade bilar och två MC på samma plats. Ett dokumenterat läge visade `FREE 198`, `OCCUPIED 2`, `VEHICLES 3`. Detta är manuella visuella kontroller, inte en fullständig verifiering av samtliga layoutgränsfall.

Följande behöver verifieras innan garageöversikten kan betraktas som färdigtestad:

- Tangentbordsnavigering till första och sista plats samt över radgränser, särskilt i viewport-läge.
- Ytterligare färg- och beläggningsgränsfall för ledig, delvis upptagen och full plats, inklusive efter omstart.
- Korrekt identifierare, ankomsttid och parkeringstid i `SPOT DETAILS` vid fler fordonskombinationer och efter omstart.
- Layout vid flera terminalstorlekar samt 100, 500 och 1 000 platser, inklusive ändrad fönsterstorlek under körning.
- Att det inte uppstår oönskad terminalscrollning, klippning eller flimmer i dessa testfall.

---

## DD-021 – Gemensamt UI-tema med AppTheme

**Datum:** 2026-10-10

### Beslut

Prague Parking V2 använder en gemensam klass, `AppTheme`, för applikationens visuella tema och återanvändbara presentationsfunktioner.

Temat inspireras av svenska flaggans blå och gula färger.

### Motivering

Tidigare använde olika UI-komponenter separata färgdefinitioner.

Detta riskerade att skapa ett inkonsekvent användargränssnitt och duplicerad kod.

Genom att centralisera färgpaletten och gemensamma panel- och menyfunktioner kan flera vyer använda samma presentationsregler.

### Genomförande

`AppTheme` innehåller bland annat:

- Bakgrundsfärg och återställning av konsolpresentation med `PrepareScreen()` och `RestoreScreen()`.
- Primär och sekundär textfärg.
- Guldaccenter, panelramarnas färger och statusfärger.
- `CreatePanel()` och `WriteCenteredPanel()` för gemensamt utformade och centrerade paneler.
- `ShowMenu()` för menyvisning och tangentbordsnavigering.
- Hjälpmetoder för gemensam text- och färgpresentation.

Temat används i huvudmenyn, garageöversikten samt vyerna för parkering, utcheckning och flytt. `MessageService` återanvänder `AppTheme` för centrerade status- och felpaneler; de övriga vyerna använder främst panelbredden 62 tecken. `GarageOverView` behåller sin breda, terminalanpassade layout.

### Fördelar

- Enhetlig grafisk identitet.
- Enklare underhåll.
- Minskad duplicering av färg- och layoutdefinitioner.
- Gemensamt beteende när fler UI-vyer skapas.

### Begränsningar

Alla terminaler stöder inte samma färgdjup eller ANSI-funktioner.

Terminalkompatibilitet och återställning av bakgrunden måste därför verifieras på olika körmiljöer.

---

## DD-022 – Positionsbaserad och återanvändbar menynavigering

**Datum:** 2026-10-10

### Beslut

Menyvisning och tangentbordsnavigering samlas i `AppTheme.ShowMenu()` i stället för att varje UI-vy implementerar ett eget menysystem.

Huvudmenyn använder den gemensamma funktionen och skickar in menyval, aktuell markering och eventuella texter för presentation.

### Bakgrund

Den första implementationen använde Spectre.Console `SelectionPrompt`.

Därefter utvecklades en anpassad meny med individuella färger och paneler.

När hela konsolen ritades om vid varje tangenttryckning uppstod synligt flimmer.

Spectre.Console Live testades också men skapade komplikationer tillsammans med den anpassade bakgrundsrenderingen.

### Genomförande

Den dokumenterade menylösningen använder:

- `Console.ReadKey()` för inmatning.
- Positionsbaserad konsolrendering med `Console.SetCursorPosition()` där det är tillämpligt.
- En markerad menyposition som kan behållas mellan navigeringstillfällen.
- Uppdatering av tidigare och nytt markerat menyval för att minska onödig omritning.
- `AppTheme.ShowMenu()` som gemensam ingång för huvudmenyn och andra vyer.

Den gamla beskrivningen av en separat menyimplementation i `MainMenu` är därmed inte längre arkitekturens avsikt. Den tidigare dokumenterade reservlösningen med `SelectionPrompt` är inte bekräftad i den aktuella koden och anges därför inte som implementerad.

### Fördelar

- Mindre duplicerad kod mellan menyer.
- Mer enhetlig navigering och presentation.
- Mindre onödig konsolrendering.
- Enklare att lägga till nya funktioner i UI.

### Arkitektur

`MainMenu` ansvarar för att välja och öppna funktioner. `AppTheme` ansvarar för återanvändbar menypresentation.

Parkeringsreglerna ligger fortsatt i domänlagret.

### Fortsatt arbete

- Verifiera tangentbordsnavigering och ESC-beteende i samtliga vyer.
- Kontrollera konsolbakgrundens återställning.
- Testa vid ändrad terminalstorlek och på andra terminaler.
- Säkerställa att färdiga UI-funktioner återgår korrekt till huvudmenyn.

---

## DD-023 – Centraliserade statusmeddelanden

**Datum:** 2026-10-10

**Beslut:** `MessageService` används för återkommande meddelandetyper såsom fel, varning, information och lyckade åtgärder. Visuell utformning delas med `AppTheme`.

**Motivering:** UI-vyer ska inte duplicera panelkonstruktion, färgval och väntan på användarinmatning.

**Genomförande:** `ShowError()`, `ShowSuccess()`, `ShowWarning()` och `ShowInfo()` använder `AppTheme.CreatePanel()` och `AppTheme.WriteCenteredPanel()` för att visa centrerade statuspaneler. Flyttvyn har testats med upprepad felaktig platsinmatning. Fortsatt kontroll av väntetext, tangentbuffert och avbrytningsbeteende återstår.

**Viktig regel:** En parkering, flytt eller utcheckning ska inte presenteras som varaktigt sparad om skrivningen till `parking-data.json` misslyckas. De tre UI-flödena visar framgång först efter `Save()` utan rapporterat fel, men felinjektionstester återstår.

---

## DD-024 – Unika identifierare i hela parkeringshuset

**Datum:** 2026-10-10

**Beslut:** `ParkingGarage` kontrollerar om ett fordon redan finns innan nyparkering. Fordonsidentifierare jämförs utan skillnad mellan stora och små bokstäver.

**Motivering:** Exempelvis `ABC123` och `abc123` ska inte kunna registreras som två separata fordon. Samma regel ska även tillämpas när sparad data återställs.

**Status:** Kontroll av dubbletter och skiftlägesokänslig jämförelse har tidigare testats i domänmodellen. `ValidateParkingData()` har nu `HashSet<string>(StringComparer.OrdinalIgnoreCase)` för inlästa fordonsidentifierare; specifika negativa JSON-test med dubbletter återstår.

---

## DD-025 – Fastställd struktur och säker återställning för parking-data.json

**Datum:** 2026-10-10

**Beslut:** Permanent parkeringsdata ska följa den redan beslutade strukturen i `parking-data.md`. Den får inte ersättas av ett platt JSON-format eller få nya obligatoriska fält utan separat beslut.

**Motivering:** Platsnumret, samlingen fordon och fordonens ursprungliga ankomsttider behöver bevaras för att garaget ska kunna återskapas korrekt efter omstart.

### Beslutad struktur för G-versionen

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
    }
  ]
}
```

### Regler för den implementerade persistensen

- Endast upptagna parkeringsplatser sparas.
- Tomma parkeringsplatser återskapas enligt `config.json`.
- Varje fordon återställs till samma plats med samma identifierare, fordonstyp och UTC-ankomsttid.
- Återställning ska kontrollera platsnummer, fordonstyp, kapacitet och dubbletter innan ett inläst tillstånd accepteras.
- En befintlig felaktig eller ofullständig JSON-fil får inte automatiskt ersättas med en tom fil.
- `Save()` skriver till en `.tmp`-fil i samma katalog och använder `File.Move()` vid första sparningen eller `File.Replace()` när originalfilen finns. Ett lyckat normalflöde med befintlig datafil har verifierats 2026-10-14; krasch- och skrivfelsscenarier återstår.
- Dataåtkomst och serialisering ska ske utanför UI och hållas separerade från domänens parkeringsregler.
- Fordonsincheckning, utcheckning och förflyttning ska omfattas av en konsekvent sparrutin; användaren ska få ett korrekt fel om lagring misslyckas.

**Status 2026-10-11:** `ParkingDataService`, de tre DTO-klasserna, validering av grundstrukturen, unika platsnummer/fordonsidentifierare, G-typerna och UTC-tid finns. Inläsning av giltig data och avvisning av typen `Truck` har testats manuellt. Serialiserad JSON med `WriteIndented = true` har granskats. `Save()` har följts till `finally`, men slutlig filersättning, återställning i `ParkingGarage` och automatisk sparning är inte verifierade. Konstruktorsöverlagringar för att återskapa `Car` och `MC` med befintlig UTC-ankomsttid ska bekräftas i källkoden vid integrationen.

**Verifieringsuppdatering 2026-10-14:** `ParkingDataMapper.ToGarage()` och `ToDocument()` har kopplats in. Vanlig bilparkering, sparning och återställning till ursprunglig plats/tid har testats genom omstart.

**Verifieringsuppdatering 2026-10-15:** Automatisk sparning används också vid utcheckning och flytt. Ett utcheckat fordon förblev borttaget efter omstart. En bil flyttades från plats 001 till 050 och placeringen bestod efter omstart. Två MC har visats på samma plats och flytt till bilupptagen plats har nekats. Skrivfel, rollback vid sådana fel och återställning av två MC efter omstart är ännu inte testade.

**Ansvarsgräns:** `ParkingDataService` validerar dokumentets struktur och interna värden. Att sparade fordon ryms på platsnummer och enligt kapacitetsreglerna i aktuell `AppConfiguration` valideras vid återställning till domänmodellen, inte genom att Data-projektet får ett beroende på domänklasser. De fungerande UI-flödena använder denna uppdelning, men konsekvent behandling av skrivfel och skydd mot överskrivning av befintlig korrupt data är inte fullständigt verifierade.

---

## DD-027 – Mapper i applikationslagret för att isolera DTO och domän

**Datum:** 2026-10-14

**Beslut:** `ParkingDataMapper` placeras i huvudprojektets `Application`-mapp, inte i solution-roten eller i `PragueParkingV2.Data`. Den konverterar mellan DTO:er och domänobjekt. `ToDocument()` använder LINQ för att projektera endast upptagna platser, medan `ToGarage()` behåller nästlade `foreach`-loopar för att tydligt skapa och parkera varje fordon med domänreglerna.

**Motivering:** Data-biblioteket behöver inte känna till de interna domänklasserna. Vid återställning skapas exakt konfigurerat antal platser, varpå fordon återskapas med samma UTC-ankomsttid och får samma plats genom `ParkVehicleAtSpot()`.

**Verifiering:** En äldre bil på plats 001 och en ny bil på plats 002 återställdes efter omstart den 14 oktober. LINQ valdes inte för återställningens sidoeffekter då det skulle försvåra läsbarhet och felsökning.

---

## DD-028 – Incheckning sparas innan bekräftelse

**Datum:** 2026-10-14

**Beslut:** `ParkVehicleView` anropar `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()` efter `ParkingGarage.ParkVehicle()`, men före framgångsmeddelandet. Vid `IOException` eller `UnauthorizedAccessException` försöker programmet ångra tillägget med `ParkingGarage.RemoveVehicle()` och visar ett fel.

**Motivering:** En ny incheckning ska inte presenteras som permanent sparad innan filskrivningen har slutförts. Normalflödet verifierades genom att parkera `UKB48K`, avsluta programmet, starta igen och återfinna bilen på samma plats.

**Begränsning:** Rollback-försöket har ännu inte testats vid faktiska skrivfel, och denna implementering utgör ingen formell transaktion. Skydd av befintlig korrupt JSON, atomisk filersättning vid avbrott och centralisering av mutation/sparning är fortfarande öppna förbättringar inför slutlig G-verifiering.

---

## DD-029 – Utcheckning med bekräftelse och sparad borttagning

**Datum:** 2026-10-15

**Beslut:** `CheckOutVehicleView` hämtar ett befintligt fordon, beräknar avgiften via `ParkingFeeCalculator` och visar sammanfattning innan användaren bekräftar. `CheckoutService` tar därefter bort fordonet. `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()` uppdaterar JSON innan det gröna kvittot visas.

**Motivering:** Avgift och parkeringstid ska beräknas innan borttagning. Användaren ska kunna avbryta och inte få ett felaktigt framgångsbesked om sparningen misslyckas.

**Återställning:** Före utcheckningen identifieras ursprungsplatsen. Om `Save()` kastar `IOException` eller `UnauthorizedAccessException` försöker UI återparkera samma fordonsobjekt på ursprungsplatsen med `ParkVehicleAtSpot()`, vilket bevarar `ArrivalTime`.

**Verifiering:** Ett normalt utcheckningsflöde med kvitto och avgift har körts; det borttagna fordonet saknades även efter omstart. Felinjektion och alla möjliga rollback-fel är inte verifierade. Detta är inte en generell transaktionsgaranti.

---

## DD-030 – Flytt av befintligt fordonsobjekt med förkontroller och JSON-sparning

**Datum:** 2026-10-15

**Beslut:** `ParkingGarage.MoveVehicle(identifier, targetSpotNumber, maxVehiclesPerSpot)` hittar fordonet och dess aktuella plats, kontrollerar att målplatsen finns, inte är samma plats och kan ta emot fordonet med `ParkingSpot.CanPark()`. Först därefter tas fordonet bort från ursprungsplatsen och placeras på målet. **Samma `Vehicle`-objekt används**, så `ArrivalTime` ändras inte.

**Motivering:** Kontroller före mutation hindrar att ett vanligt ogiltigt platsval orsakar en förlorad parkering. Domänlagret behåller ansvaret för kapacitetsregler, även om UI gör tidiga kontroller för att ge bättre återkoppling.

**Återställning i domänen:** `MoveVehicle()` kontrollerar resultatet från `RemoveVehicle()` och försöker återparkera fordonet på ursprungsplatsen om `targetSpot.ParkVehicle()` kastar ett undantag. `ParkingSpot.ParkVehicle()` kör `CanPark()` före listans `Add()`. Återställningen är en kompensationsåtgärd, inte en garanterat atomisk transaktion.

**Sparning och återställning i UI:** Efter användarbekräftelse anropas `MoveVehicle()`, följt av `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()`. Om `Save()` kastar `IOException` eller `UnauthorizedAccessException` försöker `MoveVehicleView` flytta tillbaka fordonet till dess tidigare plats och visar felmeddelande. Även detta behöver testas med avsiktligt framprovocerade fel.

**Verifiering:** Bil `ABC123` flyttades från 001 till 050 och stod kvar på 050 efter omstart. Försök att flytta bil till otillgänglig plats och MC till en plats upptagen av bil avvisades. Två MC har kunnat dela samma plats när konfigurationen tillåter det. Återställning av *båda* MC efter omstart återstår att verifiera separat.

---

## DD-031 – Tydlig återkoppling vid platsval utan att börja om

**Datum:** 2026-10-15

**Beslut:** `MoveVehicleView` använder en `while`-loop för val av destinationsplats. Ogiltig text, platsnummer utanför garaget, samma plats och otillräcklig kapacitet leder till ett felmeddelande och ett nytt försök, i stället för omedelbar återgång till huvudmenyn. Tom inmatning och Enter avbryter den aktuella vyn. Bekräftelsen har **No** som förvalt alternativ.

**Motivering:** Användaren ska kunna korrigera ett misstag utan att skriva registreringsnumret på nytt, och ska inte behöva bekräfta en flytt som redan är känd som otillåten. Affärsreglerna finns fortfarande i `ParkingGarage` och `ParkingSpot`.

**UI-genomförande:** Standardvyer använder centrering via `AppTheme`. I flyttvyn sparas startpositionen för platsinmatningen med `Console.CursorTop` och uppdateras vid nytt försök med `Console.SetCursorPosition()` och ANSI-rensning (`\x1b[0J`). Senaste felmeddelandet visas i en centrerad panel via `MessageService`, utan att tidigare felpaneler staplas.

**Verifiering och begränsningar:** Upprepade ogiltiga val har manuellt testats med korrekt omritning och fortsatt inmatning. Omedelbar avbrytning med ESC i `TextPrompt<string>` är **inte** implementerad; tomt fält och Enter används tills vidare. Terminalbeteende vid andra konsoler, storleksändring och scrollning återstår att kontrollera.

---

## DD-026 – VG-utökning med Bicycle och Bus utan att ändra G-formatet i förtid

**Datum:** 2026-10-10

**Beslut:** Prague Parking 2.1 ska utgå från den befintliga domän- och lagringsmodellen men införa storleksbaserad kapacitet och stöd för fordon som kan uppta flera sammanhängande platser.

**Motivering:** Nya fordonstyper ska kunna införas utan att en fungerande G-version behöver byggas om i onödan eller dess befintliga parkeringsdata förloras.

### Regler för VG-versionen

- `Bicycle` = 1, `MC` = 2, `Car` = 4 och `Bus` = 16 storleksenheter.
- En normal parkeringsplats motsvarar 4 storleksenheter.
- En buss behöver fyra sammanhängande parkeringsplatser.
- **Nyparkering av buss:** De fyra platserna ska ligga inom parkeringsplats 1–50.
- **Förflyttning av buss:** En redan parkerad buss får flyttas till fyra sammanhängande lediga platser även efter plats 50.
- Bussens identifierare och ursprungliga UTC-ankomsttid ska bevaras vid flytt.
- Minst två interfaces och fullständig omläsning av konfigurationen ingår också i VG-arbetet.

### Följder för domän och lagring

Nyparkering och flytt av buss kan inte använda exakt samma placeringsbegränsning. Därför ska dessa regler vara åtskilda i den framtida domänlogiken.

En buss får inte räknas som fyra oberoende fordon. En framtida utökning av JSON-modellen måste kunna representera kopplingen mellan en buss och dess fyra platser, men den exakta representationen är **inte beslutad**. G-versionens fastställda format gäller tills vidare.

**Status:** Planerat för VG. Ingen buss-, cykel- eller flerplatslogik anges som implementerad.
