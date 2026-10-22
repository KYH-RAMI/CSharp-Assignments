# Prague Parking V2 – Kravanalys

**Uppdaterad:** 2026-10-22

> Kraven nedan är en projektspecifik nedbrytning av uppgiftens krav till mindre,
> spårbara delkrav. ID-numren (G-xx och VG-xx) kommer alltså inte från den
> ursprungliga uppgiftstexten utan används för att göra implementation och
> uppföljning tydligare.

**Slutstatus 2026-10-22:** **Prague Parking V2.0 (G) och V2.1 (VG) är färdigutvecklade och testade.** Projektutvecklaren har gjort en avslutande genomgång av G- och VG-kraven i denna kravmatris. Projektet utvecklades **5–22 oktober 2026** (18 kalenderdagar inklusive start- och slutdagen). Tidigare daterade testavsnitt nedan behålls som dokumenterad historik, inte som aktuell att-göra-lista.

**Statusprincip:** *Verifierad* betyder att ett krav har implementerats och kontrollerats med relevanta bygg- eller manuella funktionstester; det innebär inte att varje möjligt gränsfall eller kraschscenario har testats eller att läraren redan har bedömt inlämningen. Där separat verifiering eller formell inlämning saknar dokumenterat resultat anges detta uttryckligen. **Läraren har beviljat individuellt undantag från kravet på publikt GitHub-repository.** Läraren får i stället tillgång till det privata repositoryt.

## Statusförklaring

- **Ej påbörjad** – ingen implementation finns ännu.
- **Pågår** – arbetet är påbörjat men kravet är inte färdigt.
- **Implementerad** – funktionaliteten finns i koden men är inte fullt verifierad.
- **Verifierad** – funktionaliteten är implementerad och har testats i dokumenterade scenarier.
- **Klar för inlämning** – underlaget är färdigt; själva leveransåtgärden redovisas separat.
- **Ej tillämpligt (undantag)** – läraren har beviljat ett individuellt undantag från kravet.

---

# G – Prague Parking 2.0

## Funktionella och tekniska krav

**Lägesbild 2026-10-16:** De sex huvudfunktionerna i användargränssnittet är implementerade: **Park Vehicle, Check Out Vehicle, Move Vehicle, Find Vehicle, Garage Overview och Administration**. Parkering, utcheckning och flytt sparar till `parking-data.json`; positiva flöden har verifierats med omstart. `Find Vehicle` har testats med giltig och okänd identifierare samt skiftlägesokänslig sökning. `Administration` har verifierats med prisändring **20 → 30 CZK** utan omstart och avvisning av **−10 CZK** utan att ersätta aktiva giltiga priser. Två MC (`DEF123`, `EDF123`) har återställts på samma plats (003) med bevarade ankomsttider. Negativa JSON-test omfattar korrupt syntax, dubbletter, platsnummer utanför garagekapaciteten, överskriden MC-kapacitet och otillåten blandning av Car/MC. Kontrollerade skrivfel har provats för parkering, utcheckning och flytt, inklusive återställning av garagets tillstånd i de testade fallen. Garagekartan har testats med **100, 200 och 1 000** konfigurerade platser, inklusive viewport och navigering till sista platsen. Efter testerna återställdes standardkapaciteten **100**; aktuell testbeläggning var **98 lediga platser, 2 upptagna och 3 fordon** (MC på 003, Car på 051). **VG-versionen har därefter färdigställts i separata V2.1-projekt; testresultaten ovan avser G-versionens kontroller den 16 oktober.** Slutstatus den 22 oktober redovisas överst. De äldre noteringarna om ytterligare miljö- och gränstester är inte påståenden om att dessa tester har genomförts.

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-00 | Grundfunktionaliteten från Prague Parking V1 ska finnas kvar | Parkering, utcheckning, flytt och sökning fungerar via UI. Funktionerna har behållits och kontrollerats i det färdiga V2.0-systemet; fullständig rad-för-rad-jämförelse med ursprunglig V1-kod är inte separat dokumenterad. | Domain / UI | Verifierad för dokumenterade kärnflöden |
| G-01 | Parkerade fordonsdata ska sparas permanent | Ändringar sparas i `parking-data.json`. Positiva flöden med omstart och riktade negativa tester har verifierats; ingen garanti för alla tänkbara kraschscenarier. | Persistence | Verifierad |
| G-02 | Parkeringsdata ska lagras i JSON-format | `System.Text.Json` och det fastställda G-formatet används. Giltig data sparas/läses; korrupt JSON har avvisats kontrollerat utan att originalet skrivs över. | Persistence | Verifierad |
| G-03 | Parkeringsdata ska läsas in vid programstart | `ParkingDataMapper.ToGarage()` återställer exakt plats, typ, identifierare och UTC-ankomsttid. Bil och två MC på samma plats har verifierats efter omstart. | Persistence / Domain | Verifierad |
| G-04 | Datafilen ska uppdateras vid incheckning | `ParkVehicleView` sparar via `ToDocument()`/`Save()` före framgångsmeddelande. Simulerat skrivfel avvisade `ROLL123` och fordonet fanns inte kvar i minnet. | Persistence | Verifierad |
| G-05 | Datafilen ska uppdateras vid utcheckning | `CheckOutVehicleView` sparar borttagningen före kvitto. Sparad utcheckning verifierad efter omstart; vid simulerat skrivfel fanns `ABC123` kvar på ursprungsplatsen. | Persistence | Verifierad |
| G-06 | Datafilen ska uppdateras när ett fordon flyttas | `MoveVehicleView` sparar flytten och behåller samma `Vehicle`-objekt. Normalflöde 001 → 050 och senare 050 → 051 kontrollerades efter omstart; misslyckad sparning återställde plats 050. | Persistence | Verifierad |
| G-07 | Systemet ska kunna hantera olika parkeringskapacitet | Konfigurerbar kapacitet för Car/MC används även vid flytt. Två MC på en plats (2/2) har verifierats och flytt av MC till bilplats nekas. Storleksbaserad kapacitetsmodell kommer först i VG. | Domain / Configuration | Verifierad |
| G-08 | Parkeringshuset ska visas som en karta/översikt | Dynamisk Spectre.Console-`Grid` med `SPOT DETAILS`, `GARAGE STATUS` och intern viewport. Testad med 100, 200 och 1 000 platser, navigering till 1000 och tillbaka; andra terminalstorlekar återstår. | UI | Verifierad |
| G-09 | Det ska vara enkelt att se hur många platser som är lediga | `GARAGE STATUS` skiljer på upptagna platser och fordon: vid 100 platser och tre fordon på två platser visades `FREE 98 / OCCUPIED 2 / VEHICLES 3`; motsvarande tal kontrollerades för 200 och 1 000 platser. | UI | Verifierad |
| G-10 | Prislista ska kunna lagras utanför programkoden | Priser lagras i `config.json` och används av prisberäkningen utan att C#-koden behöver ändras eller kompileras om | Configuration | Verifierad |
| G-11 | Prislistan ska läsas in vid programstart | Programmet använder aktuella priser från konfigurationen när det startar | Configuration | Verifierad |
| G-12 | Prislistan ska kunna läsas om under körning via menyval | `AdministrationView` anropar `ConfigurationService.LoadPricing()` och uppdaterar den gemensamma `ParkingFeeCalculator`. Bilpris 20 → 30 CZK användes utan omstart; −10 CZK avvisades med oförändrat giltigt pris. | Configuration / UI | Verifierad |
| G-13 | Prisinformationen ska vara enkel att redigera | Prisfält ligger samlade i en läsbar `config.json` och kan redigeras utan ny kompilering. Omläsning av priser sker via Administration. | Configuration | Verifierad |
| G-14 | Systemet ska ha en konfigurationsfil | Inställningar som administratören ska kunna ändra hålls utanför programkoden | Configuration | Verifierad |
| G-15 | Konfigurationsfilens format ska vara JSON | Projektet använder `config.json` och `System.Text.Json` för konfigurationen | Configuration | Verifierad |
| G-16 | Antal parkeringsplatser ska kunna konfigureras | `Garage.NumberOfParkingSpots` hämtas från `config.json`. Omstarter och översikt har testats med 100, 200 och 1 000 platser; ändring av hela garagekapaciteten utan omstart är ett VG-krav. | Configuration / Domain | Verifierad |
| G-17 | Fordonstyper ska kunna konfigureras | `VehicleTypes` i `config.json` styr Car/MC-regler i G och har använts vid kapacitets- och konfigurationstester; Bicycle/Bus ingår i V2.1. | Configuration / Domain | Verifierad |
| G-18 | Antal fordon per parkeringsplats ska kunna konfigureras för respektive fordonstyp | Kapacitetsregler hämtas från konfigurationen i stället för att hårdkodas i parkeringslogiken | Configuration / Domain | Verifierad |
| G-19 | Bil kostar 20 CZK per påbörjad timme | `ParkingFeeCalculator` använder bilpriset från konfigurationen och debiterar per påbörjad timme. Gränsfall har verifierats genom test, exempelvis 61 minuters parkering som debiteras som två timmar | Domain / Configuration | Verifierad |
| G-20 | MC kostar 10 CZK per påbörjad timme | `ParkingFeeCalculator` använder MC-priset från konfigurationen och debiterar per påbörjad timme. Funktionen har verifierats genom test | Domain / Configuration | Verifierad |
| G-21 | Parkering kortare än 10 minuter är gratis | `ParkingFeeCalculator` använder konfigurerad gratisgräns. Testat: under 10 minuter = gratis; exakt 10 minuter = avgiftsbelagt. Lärartextens formuleringar om gränsen bör jämföras före slutlig inlämning. | Domain / Configuration | Verifierad |
| G-22 | Fordonets ankomsttid ska registreras | Ankomsttid registreras i UTC och behålls vid flytt samt JSON-återställning. Verifierat för bil och för två MC på plats 003 efter omstart. | Domain / Persistence | Verifierad |
| G-23 | Systemet ska kunna ange hur länge fordonet varit parkerat | Parkeringstid beräknas från UTC-ankomsttid och avgångstid samt visas i utcheckningsflödet; `ParkingFeeResult` och `CheckoutResult` används. Domänberäkning och normal utcheckning är manuellt testade. | Domain | Verifierad |
| G-24 | Applikationen ska ha ett visuellt användargränssnitt i konsolen | Spectre.Console används i samtliga huvudsakliga UI-flöden, inklusive `FindVehicleView` och `AdministrationView`; paneler och statusmeddelanden har testats. Tester i andra terminaler återstår. | UI | Verifierad |
| G-25 | Konsolapplikationen ska använda Spectre.Console | Spectre.Console används för menyn, formulären, statuspaneler, garagekartan och liveuppdatering. Den gemensamma väntetexten i `MessageService` har justerats och kontrollerats visuellt. | UI | Verifierad |
| G-26 | Alla fordon ska hanteras som objekt | Car och MC hanteras som domänobjekt. Dessa används i testade parkering-, flytt- och utcheckningsflöden. | Domain | Verifierad |
| G-27 | Systemet ska innehålla minst fem klasser | Fler än fem klasser finns och används, bland annat `Vehicle`, `Car`, `MC`, `ParkingSpot`, `ParkingGarage`, `ParkingFeeCalculator` och `CheckoutService`. | Architecture / Domain | Verifierad |
| G-28 | Klassen `Vehicle` ska finnas | `Vehicle` är en abstrakt basklass med gemensamma fordonsuppgifter; används i testade flöden. | Domain | Verifierad |
| G-29 | `Car` ska ärva från `Vehicle` | `Car` ärver från `Vehicle` och används i manuellt verifierade flöden. | Architecture / Domain | Verifierad |
| G-30 | `MC` ska ärva från `Vehicle` | `MC` ärver från `Vehicle` och används i manuellt verifierade flöden, inklusive två MC på samma plats. | Architecture / Domain | Verifierad |
| G-31 | `ParkingSpot` ska finnas som klass | `ParkingSpot` representerar parkeringsplatser och ansvarar för sina parkerade fordon; har använts i testade kapacitets- och återställningsflöden. | Domain | Verifierad |
| G-32 | `ParkingGarage` ska finnas som klass | `ParkingGarage` hanterar samlingen av parkeringsplatser och domänoperationer; används i verifierade programflöden. | Domain | Verifierad |
| G-33 | Standardvärdet ska vara 100 parkeringsplatser | Standardkonfigurationen skapar 100 platser, men värdet kan ändras via konfigurationen | Domain / Configuration | Verifierad |
| G-34 | Lösningen ska innehålla ett separat klassbibliotek för dataåtkomst | `PragueParkingV2.Data` är ett separat klassbibliotek med `ConfigurationService`, `ParkingDataService` och DTO:er; lösningen har byggts och komponenterna har använts i manuella integrationstester. | Architecture / Persistence | Verifierad |
| G-35 | Dataåtkomst ska separeras från övrig logik | Data-projektet hanterar JSON/filer; `ParkingDataMapper` i Application översätter DTO ↔ domän. Positiva och negativa inläsnings-/sparflöden samt riktade rollback-scenarier har provats. | Architecture / Persistence | Verifierad |
| G-36 | Inställningar ska läsas från konfigurationsfilen vid programstart | `config.json` läses och valideras innan parkeringsmodellen byggs upp | Configuration / Domain | Verifierad |
| G-37 | Uppgiften ska huvudsakligen lösas med kursens hittills behandlade tekniker | Projektet använder klasser, arv, samlingar, `foreach` och avgränsad LINQ. `ToDocument()` använder LINQ och `ToGarage()` nästlade `foreach`. Projektets kravmatris har slutgranskats av utvecklaren; en fristående kartläggning av varje kurslektion ingår inte i testprotokollet. | Architecture | Genomgången |

## Leverans- och processkrav

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-38 | Programmet ska kunna köras på en annan dator | Applikationen använder portabla sökvägar via `AppContext.BaseDirectory`, .NET 10 och dokumenterade startkommandon. **Körning på en annan fysisk dator saknar separat redovisat testresultat** och ska inte anges som utfört. | Architecture / Deployment | Implementerad; externt körtest ej dokumenterat |
| G-39 | Särskilda startinstruktioner ska dokumenteras i README | Uppdaterad `README.md` innehåller startinstruktioner för Visual Studio och .NET CLI samt förklarar konfigurationsmall och runtime-filer. | Documentation / Deployment | Verifierad i dokumentation |
| G-40 | Projektet ska lämnas in via GitHub | Källkoden versionshanteras med Git/GitHub. Dokumentationen är färdigställd för inlämning; själva slutliga åtkomstdelningen/inlämningen är en separat åtgärd. | Git / Delivery | Klar för inlämning |
| G-41 | GitHub-repositoryt ska vara publikt | **Individuellt undantag godkänt av läraren:** repositoryt får vara privat och behöver inte publiceras. Läraren ges åtkomst till det privata projektet. | Git / Delivery | Ej tillämpligt (undantag) |
| G-42 | Inlämningen ska länka till hela repositoryt | Läraren får åtkomst till hela det privata repositoryt och får repo-länken enligt överenskommelsen. Delad åtkomst och inlämning redovisas när de faktiskt är genomförda. | Git / Delivery | Klar för inlämning |
| G-43 | Loggbok ska föras under projektets gång | `LOGGBOK.md` täcker **5–22 oktober 2026** med utvecklingshistorik, testresultat, problemlösning och godkänd personlig slutreflektion. | Documentation / Process | Verifierad |
| G-44 | Inlämningen är individuell | Detta är ett individuellt projekt och ska lämnas in av utvecklaren. Diskussioner eller hjälpmedel ändrar inte inlämningens individuella ansvar. | Process / Delivery | Verifierad |

---

# VG – Prague Parking 2.1

**Slutstatus 2026-10-22:** V2.1 är färdigutvecklad och testad. Kraven i projektets G/VG-matris har gåtts igenom vid slutavstämningen. De faktiska testerna från den **21 oktober** är dokumenterade i `LOGGBOK.md`, och Live Demonstration samt kodstädning avslutades den **22 oktober**. Detta är utvecklarens uppföljning, inte lärarens betygsbeslut. Bussar får använda platser **001–050** även vid flytt, enligt lärarens förtydligande. `BusMaxParkingSpot = 50` är standard.

| ID | Krav | Aktuellt utfall | Status |
|---|---|---|---|
| VG-01 | Vidareutveckla fungerande V2.0 | Två separata V2.1-projekt i samma solution; G:s fungerande kod behålls. | Verifierad |
| VG-02 | Minst två interfaces | Båda interfacen `IVehicle` och `IParkingSpot` är implementerade av `Vehicle` respektive `ParkingSpot` och används i den färdiga V2.1-modellen. | Verifierad |
| VG-03 | Bicycle | Bicycle stöds för registrering, sökning, flytt, kapacitet, JSON-lagring och återställning. Dessa flöden har testats. Separat cykelutcheckning med uppmätt avgift och fyra cyklar på samma plats redovisas inte som specifika genomförda testfall. | Verifierad i dokumenterade flöden |
| VG-04 | Bus | Bus hanteras som ett fordon med fyra platser; parkering, överlappande flytt, sökning, utcheckning och återställning har testats. Separata felinjektionstester för varje bussoperation redovisas inte. | Verifierad i dokumenterade flöden |
| VG-05 | Storleksbaserad kapacitet | Storleksmodellen är färdig: Bicycle 1 / MC 2 / Car 4 / Bus 16; platskapacitet 4. MC + Bicycle med 3/4 kapacitet har testats. | Verifierad |
| VG-06 | Buss på fyra sammanhängande platser | Bussen sparas en gång på primärplatsen och reserverar fyra sammanhängande platser. `BUS001` flyttades 004–007 → 006–009 och checkades ut med frigjorda reservationer; JSON-återställning testad. Bussområde 001–050 gäller även flytt enligt lärarens förtydligande. | Verifierad i normalflöden |
| VG-07 | Bicycle 5 CZK/påbörjad timme | Cykelpriset **5 CZK/påbörjad timme** finns i konfiguration och `ParkingFeeCalculator`. Ett separat dokumenterat utcheckningstest med en cykel finns inte i den redovisade testhistoriken. | Implementerad; separat prisflöde ej dokumenterat |
| VG-08 | Bus 80 CZK/påbörjad timme | `BUS001` 1 h 38 min → 160 CZK. | Verifierad |
| VG-09 | Konfigurerbara fordonstyper/storlekar | `VehicleTypes.Size` och `SpotCapacity` styr kapacitet för de fyra implementerade fordonstyperna vid parkering, flytt och återställning. Att skapa helt nya C#-klasser enbart via JSON ingår inte i den dokumenterade implementationen. | Verifierad för fyra typer |
| VG-10 | Läs om hela konfigurationen under körning | `Reload Configuration` är implementerad och har testats med förändrat garageantal, 100 → 200 och 500, utan omstart. | Verifierad |
| VG-11 | Validera ny konfiguration före aktivering | Ny konfiguration valideras i ett tillfälligt garage innan aktivering. Ändring av `SpotCapacity` 4 → 2 med parkerad bil nekades och tidigare konfiguration behölls. | Verifierad i testade scenarier |
| VG-12 | UI följer omladdad konfiguration | Garage Overview uppdaterades vid `Reload Configuration` till 200 och 500 platser och behöll fordon och viewport. Separat test av tidszons- och prisändring via full reload finns inte dokumenterat. | Verifierad för testat UI-flöde |
| VG-13 | Personlig reflektion, cirka 2–3 sidor | Personliga erfarenheter, svårigheter, lärdomar och slutreflektion är sammanställda i `LOGGBOK.md` den 22 oktober. Omfattningen i **sidor** är inte separat verifierad mot lärarens 2–3-sidorsangivelse. | Färdigskriven; sidomfång ej separat kontrollerat |
| VG-14 | Skapa och fyll datafil om den saknas | `TestDataSeeder` skapar fyra giltiga testfordon och en JSON-fil när `parking-data.json` saknas. Vid 200 platser verifierades 194 lediga, 6 upptagna och 4 fordon; efter omstart bevarades data. | Verifierad |

**Extra funktion – Live Demonstration (slutförd 2026-10-22):** Den automatiska demonstrationen är färdigbyggd och testad. Den använder ett separat garage i minnet med Car, MC, Bicycle och Bus, uppdaterar parkeringskarta och händelseinformation samt avslutas med ESC. Den påverkar inte ordinarie `parking-data.json`. Detta är en **egen extrafunktion**, inte ett påhittat VG-krav.

**Verifieringens omfattning:** Projektet är färdigutvecklat och testat. Matrisens specificerade observationer är evidens för de angivna scenarierna. Avsaknad av separat testprotokoll för ett enskilt specialfall innebär inte att dess implementering är ofärdig, men resultatet ska inte uppfinnas.


# Designbeslut och projekttolkningar

- Prislista och övriga programinställningar lagras i samma `config.json`.
- Prisinställningarna placeras tydligt i konfigurationen så att de är enkla att hitta och ändra.
- UI, domänlogik och dataåtkomst hålls separerade.
- `ParkingSpot.SpotNumber` används som parkeringsplatsens naturliga identitet.
- Fordon identifieras i domänen genom `Identifier`; skiftlägesokänslig dubblettkontroll är verifierad i domänen, vid JSON-inläsning och via UI (`ABC123`/`abc123`).
- Motorfordons `Identifier` motsvarar registreringsnummer.
- Fordon utan registreringsnummer, exempelvis Bicycle, kan få en systemgenererad identifierare.
- Ankomsttid lagras internt i UTC och konverteras till konfigurerad tidszon vid presentation.
- Prisberäkning hanteras separat av `ParkingFeeCalculator`.
- Aktiva prisregler hålls i minnet efter att konfigurationen har lästs in. G-kravet avser omläsning av prislistan; VG omfattar omläsning av hela konfigurationen.
- Nya prisvärden ska valideras innan de ersätter de aktiva prisvärdena.
- `ParkingFeeResult` används för att returnera parkeringstid, timpris, gratisgräns och beräknad avgift från prislogiken.
- Utcheckningsflödet samordnas av `CheckoutService`.
- Prisberäkningen genomförs innan fordonet tas bort från parkeringshuset.
- `CheckoutResult` används för att lämna ett strukturerat resultat från utcheckningen till presentationslagret.
- `ParkingFeeResult` och `CheckoutResult` är runtime-resultat och ska inte lagras i `parking-data.json`.
- Den fastställda strukturen för `parking-data.json` är `ParkingSpots` med `SpotNumber` och `Vehicles`; varje fordon har `Identifier`, `VehicleType` och `ArrivalTime`. Endast upptagna platser sparas. Inga nya obligatoriska VG-fält läggs till i G-formatet.
- Fordon ska återställas på exakt samma plats och med ursprunglig UTC-ankomsttid; ett eventuellt fel i en befintlig fil får inte automatiskt leda till att den skrivs över.
- Efter incheckning, utcheckning och flytt uppdateras det persistenta tillståndet med `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()`. Riktade tester av skrivfel och återställning i minnet har genomförts för alla tre åtgärderna. Detta utgör inte en garanti för generell transaktions- eller kraschsäkerhet.
- Saknad runtime-konfiguration får återskapas med standardvärden.
- En befintlig men ogiltig konfiguration ska inte skrivas över automatiskt; programmet ska i stället avsluta kontrollerat.
- `AppTheme.ShowMenu()` används för återanvändbar menypresentation, och `AppTheme.WriteCenteredPanel()` för centrerade paneler. `MessageService` visar centrerade statusmeddelanden. I `MoveVehicleView` hanteras felaktiga platsnummer med `while`/`continue` och ett omritat inmatningsområde så att felpaneler inte staplas.
- `ParkVehicleView`, `CheckOutVehicleView`, `MoveVehicleView`, `FindVehicleView` och `AdministrationView` är kopplade till huvudmenyn och har manuellt testade UI-flöden. De tre förstnämnda ändrar och sparar garagetillstånd; Find söker utan mutation och Administration läser om priser.
- `GarageOverView` använder ett kompakt Spectre.Console-`Grid` med dynamisk viewport. 100, 200 och 1 000 platser har kontrollerats, inklusive navigering till plats 1000; två MC på samma plats har verifierats även efter omstart. Gränsfall för liten/ändrad terminalstorlek är inte fullständigt testade.
- **VG – Kapacitetsmodell:** Storlekar och kapacitet hämtas från konfigurationen; den färdiga logiken använder `Func<IVehicle, int>`, `SpotCapacity` och `VehicleTypes.Size`. Äldre `VehiclesPerSpot`-kod i **V2.1** har rensats bort den **22 oktober 2026**. G-versionens separat fungerande kapacitetsmodell är oförändrad. Standardvärdena är 100 platser och 4 enheter per plats.
- **VG – Bus:** Platserna 001–050 är **tillåtna för bussparkering**, inte exklusiva för bussar. Varje buss kräver fyra sammanhängande helt lediga platser och ska vara ett enda fordon vid sökning, avgifter, flytt, utcheckning och JSON-lagring. Arbetsalgoritmen söker successivt platserna 1–4, 2–5 och så vidare tills en giltig grupp hittas. Reservationsmodellen är nu genomförd med ett bussobjekt på primärplatsen och `ReservedByBus` på fyra platser. Enligt lärarens förtydligande gäller 001–050 även vid flytt.
- **VG – Testdata:** Om datafilen saknas ska V2.1 skapa den **och** fylla den med giltiga testfordon. G-versionens beteende med tomt garage vid saknad fil förblir oförändrat.
- **VG – Reflektion:** Den individuella slutreflektionen finns färdig i `LOGGBOK.md` under **22 oktober 2026**, med egna erfarenheter, svårigheter och lärdomar. Eventuellt separat dokument/sidomfång hanteras enligt lärarens leveransanvisning.
- **Avgränsning:** Frivillig betadistribution via Windows-paketering, NAS/Docker, Cloudflare och Discord ingår inte i projektets inlämningsomfattning. Projektet är färdigutvecklat, medan en separat dokumenterad körning på en **annan dator** inte finns i underlaget. För GitHub-publicering gäller individuellt lärarundantag: repositoryt förblir privat.

## G: dokumenterade tester den 16 oktober 2026

**Genomförda manuella verifieringar:**

- Parkera, checka ut och flytta med JSON-sparning; lyckade förändringar kvarstod efter programomstart.
- Två MC (`DEF123`, `EDF123`) på plats 003 efter omstart, med separata identifierare och bibehållna ankomsttider.
- Korrupt JSON; dubbletter av identifierare (`ABC123`/`abc123`); plats 201 i ett garage med 200 platser; tre MC på samma plats; otillåten Car/MC-blandning i sparad data. Programmet avvisade testfilerna kontrollerat i stället för att skriva över dem.
- Framprovocerat sparfel genom blockerande katalog `Data/parking-data.json.tmp`: ny parkering `ROLL123` avbröts och saknades efteråt i minnet; utcheckning av `ABC123` återställdes; misslyckad flytt 050 → 051 återställde plats 050. Efter att blockeringen togs bort sparades flytten 050 → 051, och den kvarstod efter omstart.
- `Find Vehicle` med giltiga och ogiltiga identifierare, skiftlägesokänslig sökning och ny sökfråga. `Administration → Reload Pricing`: 20 → 30 CZK utan omstart; ogiltigt pris −10 CZK avvisades utan att ändra aktiva giltiga priser; standardvärden återställdes.
- Dynamisk garageöversikt vid 100, 200 och 1 000 platser, inklusive intern viewport och navigering till 1000 och tillbaka. Efter återgång till 100 platser: `FREE 98 / OCCUPIED 2 / VEHICLES 3`, med MC på 003 och Car på 051.
- Centrering av `Press any key to continue...` i `MessageService` korrigerades och verifierades visuellt.

**Begränsningar i de dokumenterade testprotokollen (inte kvarvarande utvecklingsuppgifter):**

- Utvecklarens kravavstämning är slutförd den **22 oktober**. Testat beteende vid **exakt 10 minuter** är debitering; denna uppföljning påstår inte att läraren uttryckligen har bedömt den tolkningen.
- Avbrytande och ytterligare gränsfall i UI (exempelvis Cancel/No och fullt garage) där inget explicit test har redovisats.
- JSON med *utelämnade* obligatoriska fält, identifierarens gränslängder och städning av `.tmp` vid fler filsystems-/kraschscenarier. Riktad rollback-testning är inte liktydigt med fullständig transaktionssäkerhet.
- De dokumenterade testerna täcker inte alla terminalmiljöer, terminalstorlekar eller körning på **annan dator**.
- Kodprojektet färdigställdes den 22 oktober. Slutlig GitHub-åtkomst och faktisk inlämning är administrativa leveranssteg och ska inte blandas ihop med funktionstester. **Publikt repository krävs inte**, enligt lärarens individuella undantag.

## VG: särskilda testnoteringar från 21 oktober 2026

Den detaljerade uppföljningen per **21 oktober** redovisade inte separata provprotokoll för varje tänkbart gräns- och felscenario: bland annat felinjektion för bussens alla operationer, fyra Bicycle på en plats, omkonfigurering av bussgräns med aktiv buss, pris- och tidszonsbyte via full reload, ovanliga startkonfigurationer samt vissa JSON-felfall. Dessa historiska testbegränsningar ändrar inte slutstatusen **färdigutvecklad och testad den 22 oktober** och ska inte beskrivas som oimplementerade funktioner. De specifika testresultaten ska inte påstås utförda utan belägg.

Enligt lärarens förtydligande får bussar inte använda parkeringsplatser utanför 001–050, vare sig vid incheckning eller flytt. Möjligheten att ändra `BusMaxParkingSpot` i JSON är en teknisk konfigurationsmöjlighet, inte ett påstående att andra gränser är godkända för kursinlämningen. Den konfigurerbara gränsen ändrar inte lärarens godkända bussområde för inlämningsversionen.
