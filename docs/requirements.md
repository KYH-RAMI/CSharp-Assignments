# Prague Parking V2 – Kravanalys

**Uppdaterad:** 2026-10-21

> Kraven nedan är en projektspecifik nedbrytning av uppgiftens krav till mindre,
> spårbara delkrav. ID-numren (G-xx och VG-xx) kommer alltså inte från den
> ursprungliga uppgiftstexten utan används för att göra implementation och
> uppföljning tydligare.

**Statusprincip:** Krav följs upp mot implementation, byggresultat och genomförda manuella tester. Statusen *verifierad* avser kontrollerade scenarier och innebär inte fullständig kodgranskning eller lärarens godkännande. G-status avser V2.0; VG-status avser V2.1 per 2026-10-21.

## Statusförklaring

- **Ej påbörjad** – ingen implementation finns ännu.
- **Pågår** – arbetet är påbörjat men kravet är inte färdigt.
- **Implementerad** – funktionaliteten finns i koden men är inte fullt verifierad.
- **Verifierad** – funktionaliteten är implementerad och har testats.

---

# G – Prague Parking 2.0

## Funktionella och tekniska krav

**Lägesbild 2026-10-16:** De sex huvudfunktionerna i användargränssnittet är implementerade: **Park Vehicle, Check Out Vehicle, Move Vehicle, Find Vehicle, Garage Overview och Administration**. Parkering, utcheckning och flytt sparar till `parking-data.json`; positiva flöden har verifierats med omstart. `Find Vehicle` har testats med giltig och okänd identifierare samt skiftlägesokänslig sökning. `Administration` har verifierats med prisändring **20 → 30 CZK** utan omstart och avvisning av **−10 CZK** utan att ersätta aktiva giltiga priser. Två MC (`DEF123`, `EDF123`) har återställts på samma plats (003) med bevarade ankomsttider. Negativa JSON-test omfattar korrupt syntax, dubbletter, platsnummer utanför garagekapaciteten, överskriden MC-kapacitet och otillåten blandning av Car/MC. Kontrollerade skrivfel har provats för parkering, utcheckning och flytt, inklusive återställning av garagets tillstånd i de testade fallen. Garagekartan har testats med **100, 200 och 1 000** konfigurerade platser, inklusive viewport och navigering till sista platsen. Efter testerna återställdes standardkapaciteten **100**; aktuell testbeläggning var **98 lediga platser, 2 upptagna och 3 fordon** (MC på 003, Car på 051). **VG-utvecklingen har därefter påbörjats i separata V2.1-projekt; uppgifterna om G-tester ovan avser fortfarande V2.0.** Test på annan dator, slutlig kravavstämning och vissa terminal-/JSON-gränsfall återstår.

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-00 | Grundfunktionaliteten från Prague Parking V1 ska finnas kvar | Parkering, utcheckning, flytt och sökning fungerar via UI. Prisomläsning i Administration och dynamisk garageöversikt fungerar också. En uttrycklig, fullständig V1-jämförelse återstår före slutligt godkännande. | Domain / UI | Pågår |
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
| G-17 | Fordonstyper ska kunna konfigureras | `VehicleTypes` i `config.json` definierar inställningar för `Car` och `MC` i G. Nya klasser/typer som `Bicycle`/`Bus` stöds först i VG. | Configuration / Domain | Implementerad |
| G-18 | Antal fordon per parkeringsplats ska kunna konfigureras för respektive fordonstyp | Kapacitetsregler hämtas från konfigurationen i stället för att hårdkodas i parkeringslogiken | Configuration / Domain | Verifierad |
| G-19 | Bil kostar 20 CZK per påbörjad timme | `ParkingFeeCalculator` använder bilpriset från konfigurationen och debiterar per påbörjad timme. Gränsfall har verifierats genom test, exempelvis 61 minuters parkering som debiteras som två timmar | Domain / Configuration | Verifierad |
| G-20 | MC kostar 10 CZK per påbörjad timme | `ParkingFeeCalculator` använder MC-priset från konfigurationen och debiterar per påbörjad timme. Funktionen har verifierats genom test | Domain / Configuration | Verifierad |
| G-21 | Parkering kortare än 10 minuter är gratis | `ParkingFeeCalculator` använder konfigurerad gratisgräns. Testat: under 10 minuter = gratis; exakt 10 minuter = avgiftsbelagt. Lärartextens formuleringar om gränsen bör jämföras före slutlig inlämning. | Domain / Configuration | Verifierad |
| G-22 | Fordonets ankomsttid ska registreras | Ankomsttid registreras i UTC och behålls vid flytt samt JSON-återställning. Verifierat för bil och för två MC på plats 003 efter omstart. | Domain / Persistence | Verifierad |
| G-23 | Systemet ska kunna ange hur länge fordonet varit parkerat | Parkeringstid beräknas från UTC-ankomsttid och avgångstid samt visas i utcheckningsflödet; `ParkingFeeResult` och `CheckoutResult` används. Domänberäkning och normal utcheckning är manuellt testade. | Domain | Verifierad |
| G-24 | Applikationen ska ha ett visuellt användargränssnitt i konsolen | Spectre.Console används i samtliga huvudsakliga UI-flöden, inklusive `FindVehicleView` och `AdministrationView`; paneler och statusmeddelanden har testats. Tester i andra terminaler återstår. | UI | Verifierad |
| G-25 | Konsolapplikationen ska använda Spectre.Console | Spectre.Console används för menyn, formulären, statuspaneler, garagekartan och liveuppdatering. Den gemensamma väntetexten i `MessageService` har justerats och kontrollerats visuellt. | UI | Verifierad |
| G-26 | Alla fordon ska hanteras som objekt | Fordonsdata representeras med objekt i stället för sammansatta strängar som i V1 | Domain | Implementerad |
| G-27 | Systemet ska innehålla minst fem klasser | Kravet är uppfyllt genom bland annat `Vehicle`, `Car`, `MC`, `ParkingSpot`, `ParkingGarage`, `ParkingFeeCalculator`, `ParkingFeeResult`, `CheckoutService` och `CheckoutResult` | Architecture / Domain | Implementerad |
| G-28 | Klassen `Vehicle` ska finnas | Gemensamma egenskaper och regler för fordon samlas i en abstrakt basklass | Domain | Implementerad |
| G-29 | `Car` ska ärva från `Vehicle` | Bil modelleras som en specialisering av `Vehicle` | Architecture / Domain | Implementerad |
| G-30 | `MC` ska ärva från `Vehicle` | MC modelleras som en specialisering av `Vehicle` | Architecture / Domain | Implementerad |
| G-31 | `ParkingSpot` ska finnas som klass | Varje parkeringsplats representeras av ett objekt med platsnummer och parkerade fordon | Domain | Implementerad |
| G-32 | `ParkingGarage` ska finnas som klass | Parkeringshuset ansvarar för helheten och samlingen av parkeringsplatser | Domain | Implementerad |
| G-33 | Standardvärdet ska vara 100 parkeringsplatser | Standardkonfigurationen skapar 100 platser, men värdet kan ändras via konfigurationen | Domain / Configuration | Verifierad |
| G-34 | Lösningen ska innehålla ett separat klassbibliotek för dataåtkomst | `PragueParkingV2.Data` är ett separat klassbibliotek med `ConfigurationService`, `ParkingDataService` och DTO:er; lösningen har byggts och komponenterna har använts i manuella integrationstester. | Architecture / Persistence | Verifierad |
| G-35 | Dataåtkomst ska separeras från övrig logik | Data-projektet hanterar JSON/filer; `ParkingDataMapper` i Application översätter DTO ↔ domän. Positiva och negativa inläsnings-/sparflöden samt riktade rollback-scenarier har provats. | Architecture / Persistence | Verifierad |
| G-36 | Inställningar ska läsas från konfigurationsfilen vid programstart | `config.json` läses och valideras innan parkeringsmodellen byggs upp | Configuration / Domain | Verifierad |
| G-37 | Uppgiften ska huvudsakligen lösas med kursens hittills behandlade tekniker | Klasser, arv, samlingar, `foreach` och enkel LINQ används där det förbättrar läsbarheten. `ToDocument()` använder LINQ och `ToGarage()` nästlade `foreach`. Fullständig avstämning mot kursens behandlade moment återstår. | Architecture | Pågår |

## Leverans- och processkrav

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-38 | Programmet ska kunna köras på en annan dator | Portabla runtime-sökvägar används (`AppContext.BaseDirectory`), men körning på en annan dator har ännu inte redovisats som verifierad. | Architecture / Deployment | Pågår |
| G-39 | Särskilda startinstruktioner ska dokumenteras i README | README innehåller startinstruktioner för Visual Studio/.NET CLI och förklarar runtime-filer. Slutlig kontroll med faktisk repositorystruktur och extern startmiljö återstår. | Documentation / Deployment | Pågår |
| G-40 | Projektet ska lämnas in via GitHub | Projektet versionshanteras i Git och GitHub. Den slutliga inlämningen ska innehålla aktuell kod och dokumentation | Git / Delivery | Pågår |
| G-41 | GitHub-repositoryt ska vara publikt | Repositoryt hålls privat under utvecklingen och ska göras publikt inför inlämningen så att läraren kan komma åt det utan särskild behörighet | Git / Delivery | Pågår |
| G-42 | Inlämningen ska länka till hela repositoryt | Länken i lärplattformen ska gå till repositoryts startsida | Git / Delivery | Ej påbörjad |
| G-43 | Loggbok ska föras under projektets gång | `LOGGBOK.md` innehåller utvecklingshistorik, felsökning, utförda tester och egna reflektioner från arbetet med både G och VG. Dokumentationen uppdateras löpande. | Documentation / Process | Pågår |
| G-44 | Inlämningen är individuell | Den slutliga inlämningen görs individuellt även om diskussion och samarbete är tillåtet | Process / Delivery | Pågår |

---

# VG – Prague Parking 2.1

**Kravstatus 2026-10-21 efter dagens tester:** Koden har kompilerat och centrala funktioner har testats i Windows-miljön. Status avser endast redovisade tester, inte automatiserade enhetstester, granskning av hela aktuella repositoryt eller lärarens godkännande. G-versionen är orörd. Bussarnas tillåtna område vid nyparkering **och** flytt är 001–050 enligt lärarens förtydligande; standardfilen använder `BusMaxParkingSpot = 50`.

| ID | Krav | Aktuellt utfall | Status |
|---|---|---|---|
| VG-01 | Vidareutveckla fungerande V2.0 | Två separata V2.1-projekt i samma solution; G:s fungerande kod behålls. | Verifierad |
| VG-02 | Minst två interfaces | `IVehicle`/`IParkingSpot` implementerade av `Vehicle`/`ParkingSpot`. Fördjupad kontroll av interface-typning i alla berörda anrop återstår. | Implementerad |
| VG-03 | Bicycle | Registrering, sökning, flytt, blandad kapacitet, JSON-lagring och återställning visade i tester. Faktisk utcheckning med pris 5 CZK och fyra cyklar på en plats ej separat testade. | Implementerad, delvis verifierad |
| VG-04 | Bus | Nyparkering, överlappande flytt, omstart, sökning och normal utcheckning testade. Sparfel/rollback och områdesgränser ej färdigtestade. | Implementerad, delvis verifierad |
| VG-05 | Storleksbaserad kapacitet | Storlekar Bicycle 1 / MC 2 / Car 4 / Bus 16; plats 4; MC + Bicycle på samma plats 3/4 verifierad. Gränsfall kvar. | Implementerad, delvis verifierad |
| VG-06 | Buss på fyra sammanhängande platser | Buss lagras en gång, reserverar fyra platser. `BUS001` 004–007 → 006–009, frigjord vid checkout; JSON-återställning visad. Buss får enligt läraren bara använda 001–050. | Verifierad i normalflöde; gränstester kvar |
| VG-07 | Bicycle 5 CZK/påbörjad timme | Pris finns i `ParkingFeeCalculator` och konfigurationen; separat cykelutcheckning behövs för verifiering. | Implementerad |
| VG-08 | Bus 80 CZK/påbörjad timme | `BUS001` 1 h 38 min → 160 CZK. | Verifierad |
| VG-09 | Konfigurerbara fordonstyper/storlekar | `VehicleTypes.Size` och `SpotCapacity` används i parkering, flytt, återställning och overview; stöd för helt ny C#-fordonsklass via endast JSON är inte visat. | Implementerad för fyra typer; tolkning/gränstest kvar |
| VG-10 | Läs om hela konfigurationen under körning | `Reload Configuration` i Administration; 100→200 och 500 platser har provats utan omstart. | Verifierad för garageantal |
| VG-11 | Validera ny konfiguration före aktivering | Nytt garage byggs från aktuell parkeringsdata. Kapacitet 4→2 avvisades med parkerad bil; gamla garaget fanns kvar. Andra negativa kombinationer kvar. | Implementerad, delvis verifierad |
| VG-12 | UI följer omladdad konfiguration | Garage Overview uppdaterades till 200 och 500 platser och behöll fordon/viewport. Ändrade pris-/tidszonvärden via full reload är inte separat provade. | Verifierad för garageantal |
| VG-13 | Personlig reflektion, cirka 2–3 sidor | Slutlig personlig reflektion på cirka 2–3 sidor återstår. I loggboken finns reflektioner kring busslogiken och arbetet med `Action` i C#. | Pågår |
| VG-14 | Skapa och fyll datafil om den saknas | `TestDataSeeder` kopplad i `Program.cs`; fyra giltiga fordon skapade och sparade. Vid 200 platser: 194 lediga / 6 upptagna / 4 fordon, oförändrat efter omstart. Befintlig fil genererar inte ny testdata. Mindre/ovanlig konfiguration ej provad. | Verifierad för testad konfiguration |

**Skillnaden mellan byggt och verifierat:** Funktioner som har byggts men ännu inte testats markeras inte som fullständigt verifierade. Den föreslagna automatiska Live Demonstration i Garage Overview är **inte implementerad**, och ingår inte i tabellens uppfyllda krav.


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
- **VG – Kapacitetsmodell:** Storlekar och kapacitet ska komma från `config.json`. Den nya logiken beräknar använd kapacitet via `Func<IVehicle, int>`; migrering av gamla `VehiclesPerSpot`-anrop pågår. Standardvärden (100 platser, 4 enheter per plats) ska skiljas från hårdkodade verksamhetsregler.
- **VG – Bus:** Platserna 001–050 är **tillåtna för bussparkering**, inte exklusiva för bussar. Varje buss kräver fyra sammanhängande helt lediga platser och ska vara ett enda fordon vid sökning, avgifter, flytt, utcheckning och JSON-lagring. Arbetsalgoritmen söker successivt platserna 1–4, 2–5 och så vidare tills en giltig grupp hittas. Reservationsmodellen är nu genomförd med ett bussobjekt på primärplatsen och `ReservedByBus` på fyra platser. Enligt lärarens förtydligande gäller 001–050 även vid flytt.
- **VG – Testdata:** Om datafilen saknas ska V2.1 skapa den **och** fylla den med giltiga testfordon. G-versionens beteende med tomt garage vid saknad fil förblir oförändrat.
- **VG – Reflektion:** Den individuella slutreflektionen sammanställs utifrån erfarenheter, problem och lärdomar som dokumenterats i `LOGGBOK.md`.
- **Avgränsning:** Tidigare diskuterad frivillig driftsättning och betatestning via Windows-paketering, Synology NAS/Docker, Cloudflare och Discord ingår inte längre i projektplanen. Ett separat **kurskrav** på körning på annan dator kvarstår tills det verifierats.

## G: verifierat 2026-10-16 och kvarstående kontroller

**Genomförda manuella verifieringar:**

- Parkera, checka ut och flytta med JSON-sparning; lyckade förändringar kvarstod efter programomstart.
- Två MC (`DEF123`, `EDF123`) på plats 003 efter omstart, med separata identifierare och bibehållna ankomsttider.
- Korrupt JSON; dubbletter av identifierare (`ABC123`/`abc123`); plats 201 i ett garage med 200 platser; tre MC på samma plats; otillåten Car/MC-blandning i sparad data. Programmet avvisade testfilerna kontrollerat i stället för att skriva över dem.
- Framprovocerat sparfel genom blockerande katalog `Data/parking-data.json.tmp`: ny parkering `ROLL123` avbröts och saknades efteråt i minnet; utcheckning av `ABC123` återställdes; misslyckad flytt 050 → 051 återställde plats 050. Efter att blockeringen togs bort sparades flytten 050 → 051, och den kvarstod efter omstart.
- `Find Vehicle` med giltiga och ogiltiga identifierare, skiftlägesokänslig sökning och ny sökfråga. `Administration → Reload Pricing`: 20 → 30 CZK utan omstart; ogiltigt pris −10 CZK avvisades utan att ändra aktiva giltiga priser; standardvärden återställdes.
- Dynamisk garageöversikt vid 100, 200 och 1 000 platser, inklusive intern viewport och navigering till 1000 och tillbaka. Efter återgång till 100 platser: `FREE 98 / OCCUPIED 2 / VEHICLES 3`, med MC på 003 och Car på 051.
- Centrering av `Press any key to continue...` i `MessageService` korrigerades och verifierades visuellt.

**Kvar att kontrollera; markeras inte som verifierat:**

- Slutlig jämförelse med samtliga V1- och G-krav, inklusive motsägelsefull formulering om gratisparkering vid **exakt 10 minuter** (nuvarande beteende: avgift vid exakt 10).
- Avbrytande och ytterligare gränsfall i UI (exempelvis Cancel/No och fullt garage) där inget explicit test har redovisats.
- JSON med *utelämnade* obligatoriska fält, identifierarens gränslängder och städning av `.tmp` vid fler filsystems-/kraschscenarier. Riktad rollback-testning är inte liktydigt med fullständig transaktionssäkerhet.
- Mindre eller storleksändrade terminaler, återstående navigeringsgränsfall och separat körning på **annan dator**.
- Slutligt kontrollera repositorystruktur, Git-status, dokumentationslänkar, README-instruktioner och inlämningsformalia. Inga Git-commits eller publik repository-status antas vara klara.

## Uppföljning av öppna VG-tester och leverans – 2026-10-21

Följande får **inte** förväxlas med verifierade krav: felinjektion i bussens `Park`/`Move`/`Checkout`-flöden; fyra Bicycle på samma plats; giltig och ogiltig ändring av `BusMaxParkingSpot` med en parkerad buss; `Reload Configuration` med ändrade priser, tidszon och storlekar; drift med konfiguration där testseeder inte får plats; ofullständiga JSON-fält; portabilitet på annan dator; slutlig dokument- och Git-granskning. Grundläggande Reload Pricing och G-versionens sparfelstester har testats tidigare men är **inte** samma sak som alla dessa V2.1-fall.

Enligt lärarens förtydligande får bussar inte använda parkeringsplatser utanför 001–050, vare sig vid incheckning eller flytt. Möjligheten att ändra `BusMaxParkingSpot` i JSON är en teknisk konfigurationsmöjlighet, inte ett påstående att andra gränser är godkända för kursinlämningen. Om kurskravet senare ändras behöver en ny avstämning göras.
