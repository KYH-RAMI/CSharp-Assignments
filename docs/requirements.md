# Prague Parking V2 – Kravanalys

**Uppdaterad:** 2026-10-21

> Kraven nedan är en projektspecifik nedbrytning av uppgiftens krav till mindre,
> spårbara delkrav. ID-numren (G-xx och VG-xx) kommer alltså inte från den
> ursprungliga uppgiftstexten utan används för att göra implementation och
> uppföljning tydligare.

**Avstämning 2026-10-20:** Kravstatusen grundas på redovisad implementation, tidigare manuella tester, byggloggar och uppgiftens dokumenterade krav. Detta är **inte** en fullständig ny källkodsgranskning, ett automatiserat testsuite eller ett godkännande från läraren. **G-status** nedan avser V2.0:s dokumenterade tester. **VG-status** avser endast utvecklingsläget i V2.1; lyckad kompilering är inte samma sak som verifierad funktionalitet.

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
| G-43 | Loggbok ska föras under projektets gång | `LOGGBOK.md` dokumenterar G-utvecklingen och har kompletterats med V2.1-arbetet och användarens personliga reflektioner den 20 oktober. Dokumentationen fortsätter under projektet. | Documentation / Process | Pågår |
| G-44 | Inlämningen är individuell | Den slutliga inlämningen görs individuellt även om diskussion och samarbete är tillåtet | Process / Delivery | Pågår |

---

# VG – Prague Parking 2.1

**Aktuell avstämning 2026-10-21:** Flytt och JSON-återställning för en bil har manuellt testats (UKB48K: 001→003, kvar på 003 efter omstart). En Bicycle har registrerats. Ett andra interface (`IParkingSpot`) används som kontrakt av `ParkingSpot`, och metoder för fyra reserverade bussplatser är implementerade men inte körtestade. Garage Overview kraschar när en Bicycle finns parkerad och är inte VG-klar. Nedanstående anteckningar med datum 2026-10-20 är historiska statusnoteringar; matrisraderna och bilagan nedan beskriver dagens läge.


**Lägesbild 2026-10-20:** Två separata V2.1-projekt (`PragueParkingV2_1` och `PragueParkingV2_1.Data`) har lagts till i samma solution, kompilerats och huvudmenyn har startats. Namnrymder och programtitlar har uppdaterats. `IVehicle` har införts; `Vehicle` implementerar gränssnittet. Klasserna `Bicycle` och `Bus` finns med två konstruktorer vardera. Konfigurationsmodellen har börjat övergå från `VehiclesPerSpot` till `Size` och `SpotCapacity`. Nya, **överlagrade** metoder har lagts till i `ParkingSpot` och `ParkingGarage` för storleksberäkning, bland annat med `Func<IVehicle, int>`. `ParkVehicleView` har kopplats till den nya storleksbaserade metoden för Car/MC, och `ParkingDataMapper` har utökats med omvandling för Bicycle/Bus. **Äldre anrop och lagrings-/valideringslogik återstår att migrera.** Nya VG-funktioner har inte verifierats genom fullständiga kör- och omstartstester.

**Kvarvarande integrationsrisk:** Den gamla egenskapen `VehiclesPerSpot` används fortfarande på flera ställen, bland annat vid `ParkingDataMapper.ToGarage()`, flytt, utcheckning och garageöversikt. Standardkonfigurationen i `ConfigurationService` har inte verifierats som fullständigt uppdaterad till den nya JSON-modellen. En grön build räcker därför inte som bevis på fungerande V2.1.

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| VG-01 | Prague Parking 2.1 ska utvecklas vidare från en fungerande 2.0-version | Separata körbara projekt `PragueParkingV2_1` och `PragueParkingV2_1.Data` har skapats i samma solution. Båda har byggts och V2.1:s huvudmeny har startats. VG-funktionerna är ännu inte färdiga. | Architecture | Verifierad |
| VG-02 | Minst två interfaces ska användas | `IVehicle` och `IParkingSpot` finns; `Vehicle` implementerar `IVehicle` och `ParkingSpot` implementerar `IParkingSpot`. Båda kompilerar. Granska om programmet använder `IParkingSpot` via interface-typer i tillräcklig omfattning. | Architecture / Domain | Pågår |
| VG-03 | Bicycle ska stödjas | Bicycle-klass, menyval, kapacitetsberäkning, mapper och JSON-validering finns. Första cykeln registrerades framgångsrikt 2026-10-21. Fyra cyklar på samma plats, omstart och översikten är inte verifierade; Garage Overview kraschar med Bicycle. | Domain / UI / Persistence | Pågår |
| VG-04 | Bus ska stödjas | Bus-klass, `ParkBus`, `ParkBusAtSpot`, sök-/reservationsmetoder, frigöring och mapper/JSON-typstöd är implementerade och kompilerade. Helt körflöde för Bus, avgifter, flytt och rollback har inte testats. | Domain / Persistence | Pågår |
| VG-05 | Parkeringskapacitet ska baseras på storleksenheter | Kapacitet 4 per plats med storlekar Bicycle/MC/Car/Bus = 1/2/4/16. Nya `CanPark`, `ParkVehicle`, `MoveVehicle`, `ParkingDataMapper.ToGarage` och Checkout-rollback använder storlek. Garage Overview använder ännu `VehiclesPerSpot`, och äldre överlagringar finns kvar. | Domain / Configuration | Pågår |
| VG-06 | Bus kräver fyra sammanhängande parkeringsplatser | Buss är ett fordon lagrat på huvudplatsen; fyra sammanhängande lediga platser inom 001–050 får samma `ReservedByBus`-referens. Sökning, parkering, borttagning och återställning på angiven plats har kodats, men inte funktionstestats. Särskild bussflytt återstår. | Domain / Persistence | Implementerad |
| VG-07 | Bicycle kostar 5 CZK per påbörjad timme | 5 CZK finns i V2.1-mallen, modellen och icke-negativ prisvalidering. `ParkingFeeCalculator` och full utcheckning för Bicycle är ännu inte verifierade/kompletterade. | Domain / Configuration | Pågår |
| VG-08 | Bus kostar 80 CZK per påbörjad timme | 80 CZK finns i V2.1-mallen, modellen och icke-negativ prisvalidering. `ParkingFeeCalculator` och full utcheckning för Bus är ännu inte verifierade/kompletterade. | Domain / Configuration | Pågår |
| VG-09 | Fordonstyper och storlekar ska kunna konfigureras | V2.1 använder JSON-mall med `Size`, `SpotCapacity` och `BusMaxParkingSpot` och validerar alla fyra förväntade typer. Runtime-mallens kopiering och start är verifierade. Godtyckligt nya fordonsklasser via enbart JSON stöds ännu inte. | Configuration / Domain | Pågår |
| VG-10 | Hela konfigurationen ska kunna läsas om under körning | Ett synligt menyval ska ladda om **hela** konfigurationen utan omstart. G-versionens `Reload Pricing` uppfyller inte detta. | Configuration / UI | Ej påbörjad |
| VG-11 | Ändrad konfiguration ska valideras innan den aktiveras | Grundvalidering av positiva storlekar/kapacitet, bussområde, priser, tidszon och fyra obligatoriska typer finns. Säker fullständig konfigurationsomläsning mot befintlig beläggning och reservationsgränser återstår. | Configuration / Domain | Pågår |
| VG-12 | UI ska återspegla den nya konfigurationen | V2.1-menyn stödjer Bicycle och använder konfigurerade storlekar, men Garage Overview använder gamla `VehiclesPerSpot` och kraschar med en parkerad Bicycle. Bussmeny, ny avgiftspresentation och live reload återstår. | UI / Domain | Pågår |
| VG-13 | En reflektion på cirka 2–3 sidor ska lämnas in | Personlig reflektion planeras sist. Dagbok 2026-10-21 dokumenterar JSON-mall, VS/Git, lambda/kapacitet, bussobjektreferenser samt fungerande och fallerande tester. | Documentation | Pågår |
| VG-14 | Automatiskt skapande och ifyllning av parkeringsdata för test | Om V2.1:s parkeringsdatafil saknas ska programmet skapa och fylla den med giltiga testfordon. En befintlig korrupt fil ska inte skrivas över. Funktionen är ännu inte implementerad. | Persistence / Domain | Ej påbörjad |

---

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
- **VG – Bus:** Platserna 001–050 är **tillåtna för bussparkering**, inte exklusiva för bussar. Varje buss kräver fyra sammanhängande helt lediga platser och ska vara ett enda fordon vid sökning, avgifter, flytt, utcheckning och JSON-lagring. Arbetsalgoritmen söker successivt platserna 1–4, 2–5 och så vidare tills en giltig grupp hittas. Den bestämda datamodellen för reservationerna återstår; regeln för flytt utanför området behöver klarläggas.
- **VG – Testdata:** Om datafilen saknas ska V2.1 skapa den **och** fylla den med giltiga testfordon. G-versionens beteende med tomt garage vid saknad fil förblir oförändrat.
- **VG – Reflektion:** Den egna reflektionen sammanställs sist, utifrån användarens dokumenterade erfarenheter i `LOGGBOK.md`.
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

# Nästa verifieringspunkter för V2.1 (uppdaterad 2026-10-21)

- Komplettera prisberäkning för Bicycle och Bus samt utcheckning/rollback och flytt för bussens fyra platsreservationer.
- Körtesta Bus: ledig följd inom 001–050, blockerade platser, dubblettregistrering, borttagning, sparning och återställning på samma huvudplats efter omstart; framkalla även kontrollerade sparfel.
- Testa fyra cyklar på en plats, MC+Bicycle-kombinationer, fulla platser och återställning efter omstart.
- Bygg om `GarageOverView` för storleksbaserad kapacitet samt Bus/Bicycle; åtgärda den observerade kraschen.
- Inför/validera fullständig config reload, därefter automatisk skapning av testdata vid saknad `parking-data.json`.
- Avlägsna återstående gamla `VehiclesPerSpot`-anrop och överlagringar först när motsvarande V2.1-flöden är verifierade. Testa även på annan dator inför inlämning.

## Utfall 2026-10-21 – distinktion mellan implementerat och verifierat

| Område | Utfall |
|---|---|
| V2.1, Car Move/JSON | **Manuellt verifierat**: full 002 nekades; 001 → 003 lyckades; 003 kvarstod efter omstart |
| V2.1, Bicycle-registrering | **Manuellt verifierat** för första cykeln; flera cyklar och omstart ej testat |
| V2.1, Garage Overview med Bicycle | **Känt körfel**, ej åtgärdat |
| V2.1, config.template.json | **Bygg- och starttest utfört**: mall kopierades, aktiv fil skapades vid saknad |
| V2.1, Bus metoder och JSON-väg | **Kompilerar**, men bussens end-to-end-funktioner är ännu ej verifierade |
| V2.1, full reload, testdata och ny prissättning | Återstår |

