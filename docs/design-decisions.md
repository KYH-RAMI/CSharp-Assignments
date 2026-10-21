# Prague Parking V2 – Designbeslut

**Uppdaterad:** 2026-10-21  
**Omfattning:** Arkitektur- och implementationsbeslut för den färdigutvecklade Prague Parking 2.0 (G) och den pågående vidareutvecklingen Prague Parking 2.1 (VG).

Besluten nedan beskriver både genomförda arkitekturval och planerade lösningar. Där implementation eller tester återstår anges det uttryckligen. Beslut om ett framtida arbetssätt innebär inte att funktionen redan finns i programmet.

**Versionsgräns 2026-10-20:** Prague Parking 2.0 har genomförda manuella G-tester och lämnas i huvudsak oförändrad. `PragueParkingV2_1` och `PragueParkingV2_1.Data` har skapats som separata VG-projekt i samma solution. V2.1 har nya domänklasser och delar av en ny kapacitetsmodell, men saknar ännu ett verifierat genomgående VG-flöde. Äldre beslut och G-testresultat nedan gäller fortfarande G-versionen om inget annat anges.

**Aktuell versionsgräns 2026-10-21:** V2.1 har efter 20 oktober fått `IParkingSpot`, JSON-konfigurationsmall, uppdaterade storleksbaserade UI-/mapper-anrop och en implementerad modell för fyra bussreservationer. Bilflytt med återställning efter omstart och första Bicycle-registreringen är manuellt verifierade. Bussens hela flöde, avgiftsberäkning och Garage Overview är inte klara; den senare har ett observerat körfel för Bicycle. Äldre beslutsnoteringar nedan är tidsbundna.

## DD-001 – Separation mellan UI, domän och dataåtkomst

**Beslut:** Presentation, affärsregler och dataåtkomst hålls separerade.

**Motivering:** Gränssnittet kan ändras utan att fordon och parkeringsregler påverkas.

JSON-hantering placeras i det separata Data-projektet. Dataåtkomst ska inte blandas med UI-kod eller domänregler.

## DD-002 – Gemensam abstrakt basklass för fordon

**Beslut:** `Vehicle` är abstrakt och används som basklass för `Car` och `MC`.

**Motivering:** Gemensam identifierare och ankomsttid ska inte dupliceras mellan fordonstyper.

**Tillägg 2026-10-20 (VG):** `Bicycle` och `Bus` är nu skapade som egna klasser som också ärver från `Vehicle`. Båda har konstruktörer för ny parkering och för återställning av befintlig UTC-ankomsttid. Detta innebär inte att deras fullständiga parkeringsflöden är implementerade.

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

Inläsning, domänåterställning och automatisk JSON-sparning vid incheckning, utcheckning och flytt är implementerade och manuellt testade. Den 16 oktober verifierades även avvisning av korrupt och logiskt ogiltig JSON samt kontrollerade sparfel med återställningsförsök för alla tre åtgärderna. Tester av specifika felvägar innebär inte att fullständig transaktions- eller kraschsäkerhet har bevisats.

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

Tidszon för presentation anges i konfigurationen, med `Europe/Prague` som standard. JSON-inläsning med `ArrivalTime.Kind = Utc` och återställning av sparade bilar **och två MC på samma plats** med oförändrade ankomsttider har testats. Vid flytt används samma fordonsobjekt, vilket bevarar `ArrivalTime`.

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

Menybaserad omläsning av prislistan är implementerad och verifierad via **Administration → Reload Pricing**. En ändring av bilpriset från 20 till 30 CZK/timme användes vid avgiftsberäkning utan omstart. Ett ogiltigt pris (`-10`) avvisades, och föregående giltiga pris behölls. Priset återställdes därefter till 20 CZK/timme. Omläsning av **hela** konfigurationen tillhör VG-kraven.

## DD-017 – Read-only resultat för prisberäkning

**Beslut:** `ParkingFeeCalculator.CalculateParkingFee()` returnerar `ParkingFeeResult`.

**Motivering:** UI får ett strukturerat resultat med parkeringstid, timpris, gratisminuter och avgift utan att innehålla prislogik.

## DD-018 – Checkout samordnas av en separat service

**Beslut:** `CheckoutService` ansvarar för sökning, prisberäkning och borttagning, i den ordningen.

**Motivering:** Fordonet ska inte tas bort innan en korrekt avgift har kunnat beräknas.

**Status 2026-10-16:** `CheckOutVehicleView` är integrerad med avgiftsberäkning, bekräftelse och JSON-sparning. Normal utcheckning är verifierad med omstart. Vid ett kontrollerat sparfel visades fel i stället för kvitto, och fordonet fanns kvar i garaget efter återställningsförsöket. Se DD-029.

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

Vyn har **manuellt kontrollerats med 100, 200 och 1 000 parkeringsplatser**. Den 13 oktober visades 200 lediga platser utan att kartan kapades; den 14–15 oktober testades parkerade bilar och två MC på samma plats. Den 16 oktober visade ett garage med 1 000 platser intern viewport (bland annat intervallen 001–234 och 768–1000), korrekt status `FREE 998`, `OCCUPIED 2`, `VEHICLES 3`, navigering till plats 1000 och tillbaka till början. Markeringen gick inte förbi plats 1000. Efter återgång till 100 platser visades `FREE 98`, `OCCUPIED 2`, `VEHICLES 3` och de sparade fordonen låg kvar. Detta täcker inte alla terminal- och layoutgränsfall.

Följande behöver verifieras innan garageöversikten kan betraktas som färdigtestad:

- Ytterligare färg- och beläggningsgränsfall, inklusive andra fordonskombinationer efter omstart.
- Korrekt identifierare, ankomsttid och parkeringstid i `SPOT DETAILS` vid fler kombinationer; de två sparade MC-fordonen har redan kontrollerats efter omstart.
- Fler terminalstorlekar och ändrad fönsterstorlek **under körning**, särskilt nära minimimåtten.
- Att det inte uppstår oönskad scrollning, klippning eller flimmer i ännu oprövade terminalmiljöer.
- Eventuella kvarstående detaljtester av navigeringsgränsen vid plats 001 och radbyten.

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

Temat används i huvudmenyn, garageöversikten samt vyerna för parkering, utcheckning, flytt, sökning och administration. `MessageService` återanvänder `AppTheme` för centrerade status- och felpaneler; vanliga formulärvyer använder främst panelbredden 62 tecken. `GarageOverView` behåller sin breda, terminalanpassade layout.

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

**Genomförande:** `ShowError()`, `ShowSuccess()`, `ShowWarning()` och `ShowInfo()` använder `AppTheme.CreatePanel()` och `AppTheme.WriteCenteredPanel()` för att visa centrerade statuspaneler. Flyttvyn har testats med upprepad felaktig platsinmatning. Den 16 oktober justerades `ShowPanel()` så att `Press any key to continue...` får ett beräknat vänsterindrag utifrån panelbredden 62 tecken och terminalbredden. Resultatet verifierades visuellt vid försök att parkera en redan registrerad bil. Kontroll av tangentbuffert och avbrytningsbeteende i alla terminalmiljöer återstår.

**Viktig regel:** En parkering, flytt eller utcheckning ska inte presenteras som varaktigt sparad om skrivningen till `parking-data.json` misslyckas. De tre UI-flödena visar framgång först efter `Save()` utan rapporterat fel. Kontrollerade felinjektionstester för samtliga tre åtgärder genomfördes den 16 oktober, med korrekt felbesked och återställning av det observerade garagetillståndet.

---

## DD-024 – Unika identifierare i hela parkeringshuset

**Datum:** 2026-10-10

**Beslut:** `ParkingGarage` kontrollerar om ett fordon redan finns innan nyparkering. Fordonsidentifierare jämförs utan skillnad mellan stora och små bokstäver.

**Motivering:** Exempelvis `ABC123` och `abc123` ska inte kunna registreras som två separata fordon. Samma regel ska även tillämpas när sparad data återställs.

**Status 2026-10-16:** Dubblettkontrollen är manuellt verifierad både vid JSON-inläsning (`ABC123` och `abc123` i olika platser avvisades) och via det ordinarie **Park Vehicle**-gränssnittet (försök att parkera `abc123` när `ABC123` redan fanns avvisades). `ValidateParkingData()` använder `HashSet<string>(StringComparer.OrdinalIgnoreCase)` för sparade fordonsidentifierare.

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
- `Save()` skriver till en `.tmp`-fil i samma katalog och använder `File.Move()` vid första sparningen eller `File.Replace()` när originalfilen finns. Lyckad sparning till befintlig datafil har verifierats genom omstart; ett kontrollerat nekad-åtkomst-fel mot temporärfilen har testats. Strömavbrott, abrupt processavslut och samtliga filsystemsavvikelser är inte verifierade.
- Dataåtkomst och serialisering ska ske utanför UI och hållas separerade från domänens parkeringsregler.
- Fordonsincheckning, utcheckning och förflyttning ska omfattas av en konsekvent sparrutin; användaren ska få ett korrekt fel om lagring misslyckas.

**Historik 2026-10-11:** `ParkingDataService`, DTO-klasserna och grundvalideringen infördes. Då var den fullständiga UI-integrationen ännu inte verifierad; detta beskrivs som ett historiskt läge, inte som aktuell status.

**Verifieringsuppdatering 2026-10-14:** `ParkingDataMapper.ToGarage()` och `ToDocument()` har kopplats in. Vanlig bilparkering, sparning och återställning till ursprunglig plats/tid har testats genom omstart.

**Verifieringsuppdatering 2026-10-15:** Automatisk sparning infördes och provades vid utcheckning och flytt. Ett utcheckat fordon förblev borttaget efter omstart. En bil flyttades från plats 001 till 050 och placeringen bestod efter omstart. Två MC kunde dela en plats och flytt till bilupptagen plats nekades.

**Verifieringsuppdatering 2026-10-16:** Efter omstart återställdes två MC på samma plats med bibehållna UTC-tider. Korrupt JSON, dubbletter med skiftlägesvariation, plats utanför garagegränsen, tre MC på samma plats och blandade Car/MC avvisades under kontrollerad uppstart utan att originalfilen skrevs över. Ett avsiktligt skrivfel via en katalog på den förväntade `.tmp`-sökvägen användes för att testa kompensation vid Park, Check Out och Move. Efter borttagning av felinjektionen sparades och återställdes en lyckad flytt till plats 051. Fullständig kraschåterhämtning eller generellt transaktionsskydd är inte fastställt.

**Ansvarsgräns:** `ParkingDataService` validerar dokumentets struktur och interna värden. Att sparade fordon ryms på platsnummer och enligt kapacitetsreglerna i aktuell `AppConfiguration` valideras vid återställning till domänmodellen, inte genom att Data-projektet får ett beroende på domänklasser. Denna uppdelning är manuellt prövad med ogiltiga platsnummer och kapacitetsfall. Vid upptäckt fel under normal uppstart skrivs den felaktiga filen inte över. Ett fristående `Save()`-anrop och alla möjliga filsystemfel är däremot inte fullständigt verifierade.

---

## DD-027 – Mapper i applikationslagret för att isolera DTO och domän

**Datum:** 2026-10-14

**Beslut:** `ParkingDataMapper` placeras i huvudprojektets `Application`-mapp, inte i solution-roten eller i `PragueParkingV2.Data`. Den konverterar mellan DTO:er och domänobjekt. `ToDocument()` använder LINQ för att projektera endast upptagna platser, medan `ToGarage()` behåller nästlade `foreach`-loopar för att tydligt skapa och parkera varje fordon med domänreglerna.

**Motivering:** Data-biblioteket behöver inte känna till de interna domänklasserna. Vid återställning skapas exakt konfigurerat antal platser, varpå fordon återskapas med samma UTC-ankomsttid och får samma plats genom `ParkVehicleAtSpot()`.

**Verifiering:** En äldre bil på plats 001 och en ny bil på plats 002 återställdes efter omstart den 14 oktober. Den 16 oktober återställdes även två MC på plats 003 med bibehållna ankomsttider. Mappern gav specifika fel vid platsnummer 201 när garaget hade 200 platser, vid tre MC på samma plats och vid försök att blanda Car och MC. LINQ valdes inte för återställningens sidoeffekter eftersom det skulle försvåra läsbarhet och felsökning.

---

## DD-028 – Incheckning sparas innan bekräftelse

**Datum:** 2026-10-14

**Beslut:** `ParkVehicleView` anropar `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()` efter `ParkingGarage.ParkVehicle()`, men före framgångsmeddelandet. Vid `IOException` eller `UnauthorizedAccessException` försöker programmet ångra tillägget med `ParkingGarage.RemoveVehicle()` och visar ett fel.

**Motivering:** En ny incheckning ska inte presenteras som permanent sparad innan filskrivningen har slutförts. Normalflödet verifierades genom att parkera `UKB48K`, avsluta programmet, starta igen och återfinna bilen på samma plats.

**Verifiering 2026-10-16:** När en katalog med namnet `parking-data.json.tmp` framkallade nekad åtkomst under sparning, avvisades en parkering av `ROLL123` med felmeddelande. Efteråt hittades fordonet inte i garagets minnestillstånd. Detta visar att kompensationen fungerade i det testade scenariot; det utgör ingen formell transaktion eller garanti vid plötsligt processavbrott.

---

## DD-029 – Utcheckning med bekräftelse och sparad borttagning

**Datum:** 2026-10-15

**Beslut:** `CheckOutVehicleView` hämtar ett befintligt fordon, beräknar avgiften via `ParkingFeeCalculator` och visar sammanfattning innan användaren bekräftar. `CheckoutService` tar därefter bort fordonet. `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()` uppdaterar JSON innan det gröna kvittot visas.

**Motivering:** Avgift och parkeringstid ska beräknas innan borttagning. Användaren ska kunna avbryta och inte få ett felaktigt framgångsbesked om sparningen misslyckas.

**Återställning:** Före utcheckningen identifieras ursprungsplatsen. Om `Save()` kastar `IOException` eller `UnauthorizedAccessException` försöker UI återparkera samma fordonsobjekt på ursprungsplatsen med `ParkVehicleAtSpot()`, vilket bevarar `ArrivalTime`.

**Verifiering 2026-10-16:** Normal utcheckning är testad med kvitto, avgift och omstart. Vid samma kontrollerade `.tmp`-skrivfel avbröts utcheckning av `ABC123` med felmeddelande och bilen kunde fortfarande hittas på ursprungsplatsen. Alla tänkbara rollback-fel är inte verifierade; detta är ingen generell transaktionsgaranti.

---

## DD-030 – Flytt av befintligt fordonsobjekt med förkontroller och JSON-sparning

**Datum:** 2026-10-15

**Beslut:** `ParkingGarage.MoveVehicle(identifier, targetSpotNumber, maxVehiclesPerSpot)` hittar fordonet och dess aktuella plats, kontrollerar att målplatsen finns, inte är samma plats och kan ta emot fordonet med `ParkingSpot.CanPark()`. Först därefter tas fordonet bort från ursprungsplatsen och placeras på målet. **Samma `Vehicle`-objekt används**, så `ArrivalTime` ändras inte.

**Motivering:** Kontroller före mutation hindrar att ett vanligt ogiltigt platsval orsakar en förlorad parkering. Domänlagret behåller ansvaret för kapacitetsregler, även om UI gör tidiga kontroller för att ge bättre återkoppling.

**Återställning i domänen:** `MoveVehicle()` kontrollerar resultatet från `RemoveVehicle()` och försöker återparkera fordonet på ursprungsplatsen om `targetSpot.ParkVehicle()` kastar ett undantag. `ParkingSpot.ParkVehicle()` kör `CanPark()` före listans `Add()`. Återställningen är en kompensationsåtgärd, inte en garanterat atomisk transaktion.

**Sparning och återställning i UI:** Efter användarbekräftelse anropas `MoveVehicle()`, följt av `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()`. Om `Save()` kastar `IOException` eller `UnauthorizedAccessException` försöker `MoveVehicleView` flytta tillbaka fordonet till dess tidigare plats och visar felmeddelande. Återställningen har provats med ett avsiktligt framprovocerat skrivfel.

**Verifiering 2026-10-16:** Bil `ABC123` flyttades tidigare från 001 till 050 och stod kvar där efter omstart. En flytt 050 → 051 under kontrollerat `.tmp`-skrivfel avbröts och bilen fanns kvar på 050; plats 051 var fortsatt ledig. Efter att felinjektionen togs bort kunde flytten 050 → 051 sparas, och bilen återfanns på 051 efter omstart. Två MC på 003 har också återställts efter omstart. Försök att flytta till en plats utan kapacitet avvisades.

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
- **Bussområde:** De första 50 platserna är området där nya bussar får parkeras, som förenkling av takhöjdskravet efter lärarens förtydligande. **Området är inte exklusivt för bussar:** även övriga fordon får använda platserna.
- **Nyparkering av buss:** Fyra sammanhängande lediga platser ska finnas inom platsnummer 1–50.
- **Förflyttning av buss:** Även en flytt ska kontrollera fyra sammanhängande platser. Om flytt utanför bussområdet ska tillåtas är inte fastställt i det förtydligande som redovisats och avgörs innan implementeringen.
- Bussens identifierare och ursprungliga UTC-ankomsttid ska bevaras vid flytt.
- Minst två interfaces och fullständig omläsning av konfigurationen ingår också i VG-arbetet.

### Följder för domän och lagring

Nyparkering och flytt av buss behöver gemensam kontroll av fyra sammanhängande lediga platser. Eventuella olika områdesbegränsningar för nyparkering och flytt beslutas först efter att lärarens förtydligande har preciserats; vi inför inte en oprövad undantagsregel.

En buss får inte räknas som fyra oberoende fordon. En framtida utökning av JSON-modellen måste kunna representera kopplingen mellan en buss och dess fyra platser, men den exakta representationen är **inte beslutad**. G-versionens fastställda format gäller tills vidare.

**Ytterligare VG-krav:** Om `parking-data.json` saknas vid uppstart av 2.1 ska datafilen skapas och fyllas med giltiga testfordon. Detta införs inte som nytt beteende i G-versionen.

**Status 2026-10-20:** `Bicycle` och `Bus` finns som domänklasser, och `ParkingDataMapper` känner igen typerna vid omvandling mellan domänobjekt och DTO:er. Storleksbaserade metodöverlagringar har införts, men de äldre kapacitetsanropen används fortfarande på vissa ställen. Cykelns kompletta UI-/persistensflöde, bussens fyra reserverade platser, fullständigt konfigurationsstöd och testdatagenerering är inte klara eller end-to-end-verifierade. G-versionens beteende och JSON-format ska behållas.

## DD-032 – Sökvy utan ändring av garagets tillstånd

**Datum:** 2026-10-16

**Beslut:** `FindVehicleView` söker genom befintliga domänmetoder och visar fordonets identifierare, typ, parkeringsplats, ankomsttid och förfluten parkeringstid. Sökningen ändrar inte `ParkingGarage` eller parkeringsfilen.

**Motivering:** Informationssökning ska vara en läsoperation. Samma skiftlägesokänsliga identifiering används som i andra fordonsflöden.

**Verifiering:** Bil och MC har hittats via UI med korrekt plats och tider. Sökning efter `def123` hittade `DEF123` på plats 003. Ett okänt registreringsnummer gav ett kontrollerat felmeddelande och möjlighet till ett nytt försök.

## DD-033 – Prisomläsning via Administration under pågående körning

**Datum:** 2026-10-16

**Beslut:** G-versionens `AdministrationView` läser endast om prisinställningarna från runtime-`config.json` via `ConfigurationService.LoadPricing()` och tillämpar dem på den **delade** `ParkingFeeCalculator`-instansen med `UpdatePricing()`. Övrig konfiguration och befintliga parkerade fordon ska inte byggas om vid denna åtgärd.

**Motivering:** Lärarens G-krav avser prisomläsning utan omstart, inte omläsning av hela garaget. Genom att återanvända samma kalkylator får efterföljande utcheckningar aktuella priser utan att ändra domänobjekten.

**Verifiering:** Ändring av bilens timpris från 20 till 30 CZK gav korrekt nytt belopp vid efterföljande beräkning. Ogiltigt negativt pris avvisades och föregående pris behölls. Standardpriset återställdes till 20 CZK. Hela-konfigurationen-reload är fortfarande ett VG-krav, inte en genomförd G-funktion.

---

## DD-034 – Separata körbara versioner i samma solution

**Datum:** 2026-10-20  
**Status:** Genomfört på projektstrukturnivå; VG-funktionerna är inte färdigutvecklade.

**Beslut:** Vidareutvecklingen görs i `PragueParkingV2_1` och `PragueParkingV2_1.Data` i samma solution som `PragueParkingV2` och `PragueParkingV2.Data`. G-versionen används som stabil referens och ändras inte för att lösa VG-krav.

**Motivering:** Båda versionerna ska kunna byggas, startas och jämföras separat. Det minskar risken att nya fordonsregler förstör fungerande G-logik och möjliggör regressionstester mot dokumenterade G-flöden.

**Genomfört:** De nya projekten använder .NET 10. Namnrymder har anpassats för V2.1, projektberoendet har lagts till och alla fyra projekt har byggts utan kompileringsfel. V2.1:s huvudmeny har startats, och titel/versionsinformation har anpassats. I G-versionen har standardantalet parkeringsplatser återställts till 100 efter tidigare tester med större garage.

**Avgränsning:** En lyckad build är inte ett funktionstest av V2.1. De båda versionernas parkeringsdata ligger normalt i separata körkataloger eftersom lagringen utgår från `AppContext.BaseDirectory`.

## DD-035 – IVehicle som första interface i V2.1

**Datum:** 2026-10-20  
**Status:** Delvis implementerat; kravet på minst två använda interfaces är inte uppfyllt ännu.

**Beslut:** `IVehicle` definierar de gemensamma egenskaperna `Identifier` och `ArrivalTime`. Den abstrakta basklassen `Vehicle` implementerar interfacet, vilket också omfattar `Car`, `MC`, `Bicycle` och `Bus` via arv.

**Motivering:** Kod som endast behöver ett fordons identitet och ankomsttid ska kunna arbeta mot ett gemensamt kontrakt. Den behöver då inte känna till vilken konkret fordonsklass som används.

**Genomfört:** Nya kapacitetsmetoder använder `IVehicle` som parametertyp och i `Func<IVehicle, int>`. Den interna samlingen i `ParkingSpot` är fortfarande en `List<Vehicle>` som exponeras som `IReadOnlyList<Vehicle>`. All relevant kod arbetar alltså **inte** mot interfaces ännu.

**Återstår:** Ett andra interface, exempelvis `IParkingSpot`, behöver utformas och användas men ska införas först när dess ansvar är tydligt. Det räcker inte att skapa två oanvända interfacefiler.

## DD-036 – Storleksbaserad kapacitet och Func för storleksuppslagning

**Datum:** 2026-10-20  
**Status:** Pågående migrering i endast V2.1.

**Beslut:** V2.1 ska använda storleksenheter i stället för G-versionens antal fordon per plats. Målreglerna är `Bicycle = 1`, `MC = 2`, `Car = 4` och `Bus = 16`; en vanlig parkeringsplats rymmer `4` enheter. Inställningarna ska hämtas från konfigurationen, inte skrivas som fasta kapacitetsvärden i `ParkingSpot`.

**Motivering:** Den tidigare modellen `VehiclesPerSpot` kan hantera en bil eller två motorcyklar, men uttrycker inte en generell storlek och gör det svårare att hantera kombinationer av olika fordon. Genom att summera storleksenheter kan systemet avgöra hur mycket av en parkeringsplats som återstår.

**Genomfört på kodnivå:**

- `VehicleTypeConfiguration` har fått `Size` och `GarageConfiguration` har fått `SpotCapacity`.
- `ParkingSpot.GetUsedCapacity(Func<IVehicle, int> getSize)` summerar storlekarna för redan parkerade fordon och avvisar ogiltiga storlekar.
- Överlagrade `ParkingSpot.CanPark()` och `ParkingSpot.ParkVehicle()` tar emot platskapacitet och en funktion som kan slå upp varje fordons storlek.
- `ParkingGarage` har motsvarande överlagringar för `FindAvailableSpot()`, `ParkVehicle()` och `ParkVehicleAtSpot()`.
- `ParkVehicleView` använder en lambda för att hämta storlekarna från `configuration.VehicleTypes` i stället för att be `ParkingSpot` läsa JSON.

Exempel på den funktionstyp som används:

```csharp
Func<IVehicle, int> getSize = vehicle =>
    configuration.VehicleTypes![vehicle.GetType().Name].Size;
```

**Förklaring:** `Func<IVehicle, int>` beskriver en funktion som tar emot ett `IVehicle` och returnerar ett `int`. `ParkingSpot` anropar funktionen för att få storleken men känner inte till varifrån värdet kommer. Delegaten löser inte all konfigurationsvalidering: saknade fordonstyper och ogiltiga värden behöver fortfarande hanteras kontrollerat.

**Metodöverlagring:** De nya metoderna har samma namn som de gamla men andra parameterlistor. I C# kan en metod **inte** överlagras enbart genom att returtypen ändras; om parameterlistorna skiljer sig får returtyperna däremot vara olika.

**Återstår:** Migrera bland annat `ParkingDataMapper.ToGarage()`, flytt, utcheckning och garageöversikt från `VehiclesPerSpot` till storleksreglerna. När alla anrop använder den nya modellen kan tillfälliga överlagringar och äldre VG-beroenden tas bort. Befintlig G-kod ska inte ändras. Inget genomgående test av blandade storlekar har redovisats ännu.

## DD-037 – Bussens fyra sammanhängande platser: algoritm under utvärdering

**Datum:** 2026-10-20  
**Status:** Föreslagen algoritm; varken reservationsmodell eller full implementation är beslutad.

**Arbetshypotes för nyparkering:** En buss ska vara **ett fordon** med storleken 16 och behöver fyra **helt lediga, sammanhängande** parkeringsplatser. Sökningen sker bland platserna 1–50. Övriga fordon får också parkera inom detta område; deras beläggning kan därför göra en fyrplatsgrupp oanvändbar för buss.

**Föreslaget sökflöde:**

1. Kontrollera att bussens `Identifier` inte redan finns i garaget.
2. Börja med startplats 1 och kontrollera platserna 1–4.
3. Om alla fyra platser är helt lediga kan de reserveras som en sammanhängande grupp för **en** buss.
4. Annars flyttas startplatsen ett steg framåt och nästa grupp om fyra kontrolleras.
5. Den sista giltiga startplatsen inom 1–50 är 47, vilket motsvarar gruppen 47–50.
6. Om ingen giltig grupp hittas ska parkeringen avvisas med ett begripligt meddelande.

**Motivering:** En sekventiell sökning med en glidande startpunkt är lätt att följa och kan hitta fyra intilliggande platser även om tidigare platser är upptagna. Kontrollerna ska ske **innan** garaget ändras.

**Öppna designfrågor:** Hur markeras tre ytterligare platser som upptagna när bussobjektet bara registreras en gång? Hur kopplas de fyra platserna ihop vid sökning, flytt, utcheckning och JSON-återställning? En misslyckad reservation eller flytt får inte lämna kvar någon delvis parkerad buss. Den exakta representationen i minnet och i JSON är **inte fastställd**.

**Områdesregel:** Nyparkering begränsas till 1–50. Om bussflytt får ske utanför detta område behöver avgöras utifrån uppgiftskravet innan flyttlogiken utformas. Detta dokument fastställer inget sådant undantag.

## DD-038 – VG-konfiguration, validering och omläsning under körning

**Datum:** 2026-10-20  
**Status:** Struktur och viss validering påbörjad; säker omläsning av hela konfigurationen saknas.

**Beslut:** V2.1 ska definiera fordonsstorlekar per typ via `VehicleTypes` och en gemensam platskapacitet via `Garage.SpotCapacity`. Planerade VG-priser är 5 CZK per påbörjad timme för `Bicycle` och 80 CZK för `Bus`, utöver befintliga priser. G:s inställningar och `Reload Pricing` hålls åtskilda från V2.1:s framtida omläsning av hela konfigurationen.

**Genomfört:** Storleksegenskapen `Size` används i den nya kapacitetslogiken. `ConfigurationService` har en `foreach`-kontroll av namn och positiv `Size` för konfigurerade fordonstyper. Övrig validering och skapande av fullständiga standardvärden är inte slutförda eller verifierade.

**Princip för kommande omläsning:** Läs nya värden, kontrollera deras giltighet och kontrollera att redan parkerade fordon fortfarande får plats **innan** de nya inställningarna aktiveras. En ogiltig omläsning ska lämna både aktiv konfiguration och garagets tillstånd oförändrade. UI, avgifter och kapacitet ska därefter använda samma giltiga konfiguration.

**Begränsning:** En post i `VehicleTypes` skapar inte automatiskt en ny C#-klass eller en fullständig meny-/persistensimplementation. Stöd för att lägga till en helt ny fordonstyp utan kodändring är därför ännu inte etablerat och ska utvärderas separat mot uppgiftskravet.

## DD-039 – V2.1:s mappning och automatisk testdata vid saknad datafil

**Datum:** 2026-10-20  
**Status:** Fordonstypmappning utökad; kompletta VG-flöden och testdatagenerering återstår.

**Beslut:** `ParkingDataMapper` ska även i V2.1 hålla omvandlingen mellan DTO:er och domänmodellen i applikationslagret. De separata G- och VG-projekten ska kunna använda sina egna giltiga parkeringsdata utan att G-versionens tidigare filformat ändras i förtid.

**Genomfört:** `ToVehicle()` och `ToVehicleData()` har fått stöd för `Bicycle` och `Bus` som klassnamn/fordonstyper. Detta innebär inte att `ParkingDataService` redan godkänner dessa typer vid inläsning och sparning eller att en buss kan återställas över fyra platser.

**Återstår:** `ToGarage()` behöver använda storleksbaserad kapacitet. DTO-validering, JSON-representation för bussens fyra reservationer, korrekt återställning och funktionstester behöver färdigställas. Om **V2.1:s datafil saknas** ska systemet skapa filen och fylla den med giltiga testfordon. En **befintlig felaktig** fil får inte automatiskt skrivas över. G-versionen behåller sitt nuvarande beteende vid saknad datafil.

## DD-040 – Prioritera kurskraven före frivillig distribution

**Datum:** 2026-10-20  
**Status:** Aktuell projektavgränsning.

**Beslut:** Frivillig extern driftsättning och betatestning ingår inte längre i den aktuella projektplanen för Prague Parking V2.1.

**Motivering:** Utvecklingstiden prioriteras till kurskraven, testning av den nya parkeringslogiken, dokumentation och Git-hantering. **Kurskravet att verifiera körning på en annan dator kvarstår**, eftersom det inte är samma sak som frivillig driftsättning eller extern betatestning.

---

## DD-041 – Skilja på V2.1-konfigurationsmall och aktiv konfiguration

**Datum:** 2026-10-21. **Status:** Implementerad; starttest utfört.

**Beslut:** Versionshantera `PragueParkingV2_1/Config/config.template.json` och kopiera mallen till output (`PreserveNewest`). `ConfigurationService` skapar aktiv runtime-`Config/config.json` genom `File.Copy()` **endast om den saknas**. En befintlig fil läses/valideras men skrivs inte över. Den gamla runtime-filen med `VehiclesPerSpot` säkerhetskopierades separat före test.

**Motivering:** Användarens konfiguration ska överleva nya builds, och giltiga standardvärden ska inte hårdkodas parallellt i `CreateDefaultConfiguration()`.

## DD-042 – IParkingSpot som andra interface

**Datum:** 2026-10-21. **Status:** Kompilerar; faktisk interfacebaserad användning behöver fortsätta följas upp.

**Beslut:** `ParkingSpot : IParkingSpot` exponerar avtal för `SpotNumber`, `ParkedVehicles`, `GetUsedCapacity`, `CanPark`, `ParkVehicle`, `FindVehicle` och `RemoveVehicle`. Gamla `VehiclesPerSpot`-överlagringar ingår inte i det nya interfacet. Bussens reservationsmetoder hanteras för tillfället av konkreta `ParkingSpot` och koordineras av `ParkingGarage`.

## DD-043 – En Bus på huvudplatsen, fyra objektreferenser

**Datum:** 2026-10-21. **Status:** Implementerat på kodnivå; genomgående test återstår.

**Beslut:** Bussen läggs **en gång** i `ParkedVehicles` på gruppens första plats. Varje plats i en fyra platser lång, sammanhängande grupp får `ReservedByBus` pekande på **samma Bus-instans**. Platserna 001–050 är tillåtna för buss, men inte exklusiva för buss.

**Genomförda metoder:** `ReserveForBus`, `ReleaseBusReservation`, `ParkBusAtPrimarySpot`, `FindAvailableBusSpots`, `FindAvailableBusSpotsAt`, `ParkBus`, `ParkBusAtSpot`. `ParkingGarage.RemoveVehicle` kontrollerar fyra reservationer före frigöring. Den vanliga `CanPark` avvisar Bus och bussreserverade platser.

**JSON-princip:** Endast huvudplatsens `VehicleData` sparas. `ParkingDataMapper.ToGarage()` skiljer på Bus och andra fordon och återställer samma grupp genom `ParkBusAtSpot()`.

**Återstår:** Körtester, säker bussflytt, prisberäkning, fullständig utcheckning/rollback, gränsfall, garageöversikt och återställning under fel. Nuvarande kompilerad metodkod får inte beskrivas som verifierad full funktionalitet.

## DD-044 – Samlad ombyggnad av Garage Overview efter bussintegrationen

**Datum:** 2026-10-21. **Status:** Beslut, ej genomfört.

**Motivering:** Vyn omfattar cirka 500 kodrader och använder fortfarande G:s `VehiclesPerSpot`. `GetVehicleType` saknar Bicycle och vyn gav ett oväntat fel efter cykelregistrering. Därför skjuts ombyggnaden upp tills bussens fyra reservationer är stabila, så att färgstatus, detaljpanel och summering av platser/fordon kan uppdateras tillsammans.
