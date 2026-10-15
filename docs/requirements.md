# Prague Parking V2 – Kravanalys

**Uppdaterad:** 2026-10-15

> Kraven nedan är en projektspecifik nedbrytning av uppgiftens krav till mindre,
> spårbara delkrav. ID-numren (G-xx och VG-xx) kommer alltså inte från den
> ursprungliga uppgiftstexten utan används för att göra implementation och
> uppföljning tydligare.

**Avstämning:** Status nedan utgår från de filer och manuella tester som hittills har redovisats. En fullständig verifiering mot det aktuella Git-repositoryt har inte genomförts. Dokumenterade designbeslut är inte detsamma som färdig implementation.

## Statusförklaring

- **Ej påbörjad** – ingen implementation finns ännu.
- **Pågår** – arbetet är påbörjat men kravet är inte färdigt.
- **Implementerad** – funktionaliteten finns i koden men är inte fullt verifierad.
- **Verifierad** – funktionaliteten är implementerad och har testats.

---

# G – Prague Parking 2.0

## Funktionella och tekniska krav

**Lägesbild 2026-10-15:** `Park Vehicle`, `Check Out Vehicle` och `Move Vehicle` är inkopplade i huvudmenyn och sparar ändringar i `parking-data.json`. **Normalflödet för incheckning, utcheckning och flytt har testats**, inklusive återstart för att kontrollera att en utcheckad bil förblir borttagen och att en flyttad bil behåller sin nya placering. Vid en verifierad flytt flyttades `ABC123` från plats 001 till 050. Flytt till en plats som inte kan ta emot fordonet stoppas före bekräftelse. Två MC har testats tillsammans på samma plats, och en MC har nekats flytt till en bilplats. `GarageOverView` visade vid MC-testet **198 lediga platser, 2 upptagna platser och 3 fordon** av 200 platser. **Find Vehicle** och **Administration** återstår som UI-flöden. Felinjektion vid JSON-sparning, återställning av två MC efter omstart, ogiltiga filer och fler gränsfall är ännu inte verifierade. Statusen grundas på redovisad kod och manuella tester, inte på en fullständig granskning av repositoryt.

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-00 | Grundfunktionaliteten från Prague Parking V1 ska finnas kvar | Incheckning, utcheckning och flytt fungerar via UI med sparning till JSON och manuellt verifierade normalflöden. Sökning via separat Find Vehicle-vy och Administration återstår; fullständig V1-/G-avstämning är inte genomförd. | Domain / UI | Pågår |
| G-01 | Parkerade fordonsdata ska sparas permanent | Incheckning, utcheckning och flytt sparar aktuellt garagetillstånd. Lyckade normalflöden har verifierats med omstart för bil; fel vid skrivning och vissa återställningsfall återstår. | Persistence | Verifierad |
| G-02 | Parkeringsdata ska lagras i JSON-format | `System.Text.Json` och det fastställda JSON-formatet används. Sparad incheckning, utcheckning och flytt har testats i normalflödet; korrupt fil och filfelsfall återstår. | Persistence | Verifierad |
| G-03 | Parkeringsdata ska läsas in vid programstart | `Program.cs` läser JSON och `ParkingDataMapper.ToGarage()` återställer fordon till deras sparade platser med ursprunglig UTC-tid. Återstart är verifierad för bil, inklusive efter flytt; två MC efter omstart återstår. | Persistence / Domain | Verifierad |
| G-04 | Datafilen ska uppdateras vid incheckning | `ParkVehicleView` använder `ToDocument()` och `Save()` före framgångsbekräftelsen. Incheckning och återstart är verifierade; felinjektion/rollback återstår. | Persistence | Verifierad |
| G-05 | Datafilen ska uppdateras vid utcheckning | `CheckOutVehicleView` beräknar avgift, tar bort fordonet, anropar `ToDocument()`/`Save()` och visar kvitto efter sparning. Manuellt test: `UKB48K` saknades efter utcheckning och omstart, medan andra fordon låg kvar. Återställning vid skrivfel är implementerad men inte feltestad. | Persistence | Verifierad |
| G-06 | Datafilen ska uppdateras när ett fordon flyttas | `MoveVehicleView` anropar `ParkingGarage.MoveVehicle()` och sparar med `ToDocument()`/`Save()`. Bil `ABC123` flyttades 001 → 050; efter omstart kvarstod den nya platsen. Samma fordonsobjekt behåller `ArrivalTime`. Återflytt vid sparfel finns i kod men är ännu inte feltestad. | Persistence | Verifierad |
| G-07 | Systemet ska kunna hantera olika parkeringskapacitet | Konfigurerbar kapacitet för Car/MC används även vid flytt. Två MC på en plats (2/2) har verifierats och flytt av MC till bilplats nekas. Storleksbaserad kapacitetsmodell kommer först i VG. | Domain / Configuration | Verifierad |
| G-08 | Parkeringshuset ska visas som en karta/översikt | Dynamisk `Grid`-karta med Spot Details och Garage Status har visats med 200 platser och olika beläggning, inklusive en plats med två MC. Full verifiering av viewport, terminalstorlekar och navigeringsgränser återstår. | UI | Implementerad |
| G-09 | Det ska vara enkelt att se hur många platser som är lediga | Garage Status har visat korrekta totalsiffror både för två bilar (`FREE 198`, `OCCUPIED 2`, `VEHICLES 2`) och två MC på samma plats samt en bil (`FREE 198`, `OCCUPIED 2`, `VEHICLES 3`). | UI | Verifierad |
| G-10 | Prislista ska kunna lagras utanför programkoden | Priser lagras i `config.json` och används av prisberäkningen utan att C#-koden behöver ändras eller kompileras om | Configuration | Verifierad |
| G-11 | Prislistan ska läsas in vid programstart | Programmet använder aktuella priser från konfigurationen när det startar | Configuration | Verifierad |
| G-12 | Prislistan ska kunna läsas om under körning via menyval | Ändrade priser ska kunna aktiveras utan omstart. `ParkingFeeCalculator.UpdatePricing()` finns och är manuellt testad, men det synliga menyvalet för omläsning av prislistan från `config.json` saknas | Configuration / UI | Pågår |
| G-13 | Prisinformationen ska vara enkel att redigera | Priserna ligger i en människoläsbar JSON-konfiguration | Configuration | Implementerad |
| G-14 | Systemet ska ha en konfigurationsfil | Inställningar som administratören ska kunna ändra hålls utanför programkoden | Configuration | Verifierad |
| G-15 | Konfigurationsfilens format ska vara JSON | Projektet använder `config.json` och `System.Text.Json` för konfigurationen | Configuration | Verifierad |
| G-16 | Antal parkeringsplatser ska kunna konfigureras | Parkeringshusets storlek hämtas från `config.json` och är inte hårdkodad | Configuration / Domain | Verifierad |
| G-17 | Fordonstyper ska kunna konfigureras | Konfigurerbara uppgifter för `Car` och `MC` lagras i `VehicleTypes` i `config.json`; Bicycle/Bus införs först i VG | Configuration / Domain | Implementerad |
| G-18 | Antal fordon per parkeringsplats ska kunna konfigureras för respektive fordonstyp | Kapacitetsregler hämtas från konfigurationen i stället för att hårdkodas i parkeringslogiken | Configuration / Domain | Verifierad |
| G-19 | Bil kostar 20 CZK per påbörjad timme | `ParkingFeeCalculator` använder bilpriset från konfigurationen och debiterar per påbörjad timme. Gränsfall har verifierats genom test, exempelvis 61 minuters parkering som debiteras som två timmar | Domain / Configuration | Verifierad |
| G-20 | MC kostar 10 CZK per påbörjad timme | `ParkingFeeCalculator` använder MC-priset från konfigurationen och debiterar per påbörjad timme. Funktionen har verifierats genom test | Domain / Configuration | Verifierad |
| G-21 | Parkering kortare än 10 minuter är gratis | Gratisgränsen hämtas från konfigurationen och används av `ParkingFeeCalculator`. Tester har verifierat parkering under gränsen samt exakt vid gränsen | Domain / Configuration | Verifierad |
| G-22 | Fordonets ankomsttid ska registreras | Ankomsttid registreras i UTC. Vid flytt används samma fordonsobjekt, och uppstarten återställer sparad ankomsttid. Bil har verifierats efter omstart; två MC tillsammans efter omstart återstår. | Domain / Persistence | Verifierad |
| G-23 | Systemet ska kunna ange hur länge fordonet varit parkerat | Parkeringstid beräknas från UTC-ankomsttid och avgångstid samt visas i utcheckningsflödet; `ParkingFeeResult` och `CheckoutResult` används. Domänberäkning och normal utcheckning är manuellt testade. | Domain | Verifierad |
| G-24 | Applikationen ska ha ett visuellt användargränssnitt i konsolen | Spectre.Console används nu i Park Vehicle, Check Out Vehicle, Move Vehicle, Garage Overview och huvudmenyn. Bekräftelsepaneler, felhantering och inmatningsloop i Move Vehicle är manuellt testade; Find och Administration återstår. | UI | Pågår |
| G-25 | Konsolapplikationen ska använda Spectre.Console | Spectre.Console används i huvudmeny, parkering, utcheckning, flytt, statusmeddelanden och garageöversikt. Centrerade paneler och gemensamt tema används i aktiva formulär. | UI | Verifierad |
| G-26 | Alla fordon ska hanteras som objekt | Fordonsdata representeras med objekt i stället för sammansatta strängar som i V1 | Domain | Implementerad |
| G-27 | Systemet ska innehålla minst fem klasser | Kravet är uppfyllt genom bland annat `Vehicle`, `Car`, `MC`, `ParkingSpot`, `ParkingGarage`, `ParkingFeeCalculator`, `ParkingFeeResult`, `CheckoutService` och `CheckoutResult` | Architecture / Domain | Implementerad |
| G-28 | Klassen `Vehicle` ska finnas | Gemensamma egenskaper och regler för fordon samlas i en abstrakt basklass | Domain | Implementerad |
| G-29 | `Car` ska ärva från `Vehicle` | Bil modelleras som en specialisering av `Vehicle` | Architecture / Domain | Implementerad |
| G-30 | `MC` ska ärva från `Vehicle` | MC modelleras som en specialisering av `Vehicle` | Architecture / Domain | Implementerad |
| G-31 | `ParkingSpot` ska finnas som klass | Varje parkeringsplats representeras av ett objekt med platsnummer och parkerade fordon | Domain | Implementerad |
| G-32 | `ParkingGarage` ska finnas som klass | Parkeringshuset ansvarar för helheten och samlingen av parkeringsplatser | Domain | Implementerad |
| G-33 | Standardvärdet ska vara 100 parkeringsplatser | Standardkonfigurationen skapar 100 platser, men värdet kan ändras via konfigurationen | Domain / Configuration | Verifierad |
| G-34 | Lösningen ska innehålla ett separat klassbibliotek för dataåtkomst | `PragueParkingV2.Data` finns och hanterar både konfiguration och grundläggande JSON-persistens med `ParkingDataService` och tre DTO-klasser | Architecture / Persistence | Implementerad |
| G-35 | Dataåtkomst ska separeras från övrig logik | Data-projektet ansvarar för konfiguration och JSON; `ParkingDataMapper` i Application översätter mellan DTO och domän. Incheckning, utcheckning och flytt använder samma persistensflöde; särskilda fel- och rollback-tester återstår. | Architecture / Persistence | Pågår |
| G-36 | Inställningar ska läsas från konfigurationsfilen vid programstart | `config.json` läses och valideras innan parkeringsmodellen byggs upp | Configuration / Domain | Verifierad |
| G-37 | Uppgiften ska huvudsakligen lösas med kursens hittills behandlade tekniker | Klasser, arv, collections, `foreach`, lambda-uttryck och enkel LINQ används där de ökar tydligheten. `ToDocument()` använder LINQ och `ToGarage()` behåller nästlade loopar; flytten återanvänder domänmetoder. | Architecture | Pågår |

## Leverans- och processkrav

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-38 | Programmet ska kunna köras på en annan dator | Lösningen ska använda portabla sökvägar och kunna startas på en annan dator. Självständig publicering och test på annan dator är planerade till slutet, inte verifierade | Architecture / Deployment | Pågår |
| G-39 | Särskilda startinstruktioner ska dokumenteras i README | Start, .NET-krav, konfigurationsfiler och eventuella särskilda steg ska dokumenteras i README och stämmas av mot den slutliga leveransen | Documentation / Deployment | Pågår |
| G-40 | Projektet ska lämnas in via GitHub | Projektet versionshanteras i Git och GitHub. Den slutliga inlämningen ska innehålla aktuell kod och dokumentation | Git / Delivery | Pågår |
| G-41 | GitHub-repositoryt ska vara publikt | Repositoryt hålls privat under utvecklingen och ska göras publikt inför inlämningen så att läraren kan komma åt det utan särskild behörighet | Git / Delivery | Pågår |
| G-42 | Inlämningen ska länka till hela repositoryt | Länken i lärplattformen ska gå till repositoryts startsida | Git / Delivery | Ej påbörjad |
| G-43 | Loggbok ska föras under projektets gång | Arbetsprocess, beslut, problem och lösningar dokumenteras löpande i `LOGGBOK.md` | Documentation / Process | Pågår |
| G-44 | Inlämningen är individuell | Den slutliga inlämningen görs individuellt även om diskussion och samarbete är tillåtet | Process / Delivery | Pågår |

---

# VG – Prague Parking 2.1

> VG-funktionaliteten implementeras först när Prague Parking 2.0 fungerar stabilt.

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| VG-01 | Prague Parking 2.1 ska utvecklas vidare från en fungerande 2.0-version | G-versionen färdigställs och stabiliseras innan VG-funktionalitet införs | Architecture | Ej påbörjad |
| VG-02 | Minst två interfaces ska användas | Exempel är `IVehicle` och `IParkingSpot`; implementationen ska i huvudsak arbeta mot interfaces | Architecture / Domain | Ej påbörjad |
| VG-03 | Bicycle ska stödjas | Bicycle införs som en ny fordonstyp. Om cykeln saknar registreringsnummer behövs en unik och stabil systemidentifierare som kan sparas och återställas | Domain | Ej påbörjad |
| VG-04 | Bus ska stödjas | Bus införs som en ny fordonstyp med större kapacitetsbehov | Domain | Ej påbörjad |
| VG-05 | Parkeringskapacitet ska baseras på storleksenheter | Bicycle = 1, MC = 2, Car = 4, Bus = 16 och en normal parkeringsplats = 4 enheter | Domain / Configuration | Ej påbörjad |
| VG-06 | Bus kräver fyra sammanhängande parkeringsplatser | Systemet ska hitta fyra sammanhängande lediga parkeringsplatser för en buss, motsvarande totalt 16 kapacitetsenheter. Vid **nyparkering** måste fyra sammanhängande lediga platser finnas inom nummer 1–50. En **redan parkerad** buss ska kunna flyttas till fyra sammanhängande lediga platser även efter nummer 50. Bussens identitet och ursprungliga ankomsttid bevaras | Domain | Ej påbörjad |
| VG-07 | Bicycle kostar 5 CZK per påbörjad timme | Priset ska ligga i konfigurationen | Domain / Configuration | Ej påbörjad |
| VG-08 | Bus kostar 80 CZK per påbörjad timme | Priset ska ligga i konfigurationen | Domain / Configuration | Ej påbörjad |
| VG-09 | Fordonstyper och storlekar ska kunna konfigureras | Storleksregler ska hämtas från konfigurationen och inte hårdkodas i parkeringslogiken | Configuration / Domain | Ej påbörjad |
| VG-10 | Hela konfigurationen ska kunna läsas om under körning | Ett synligt menyval ska kunna ladda om konfigurationen utan omstart | Configuration / UI | Ej påbörjad |
| VG-11 | Ändrad konfiguration ska valideras innan den aktiveras | En ändring får inte skapa ett ogiltigt garagetillstånd, exempelvis ta bort upptagna platser. Tidigare giltig konfiguration ska behållas om den nya är ogiltig | Configuration / Domain | Ej påbörjad |
| VG-12 | UI ska återspegla den nya konfigurationen | Efter lyckad reload ska översikt och regler använda de nya värdena | UI / Domain | Ej påbörjad |
| VG-13 | En reflektion på cirka 2–3 sidor ska lämnas in | Reflektionen ska behandla lösning, designval, problem och lärdomar | Documentation | Ej påbörjad |

---

# Designbeslut och projekttolkningar

- Prislista och övriga programinställningar lagras i samma `config.json`.
- Prisinställningarna placeras tydligt i konfigurationen så att de är enkla att hitta och ändra.
- UI, domänlogik och dataåtkomst hålls separerade.
- `ParkingSpot.SpotNumber` används som parkeringsplatsens naturliga identitet.
- Fordon identifieras i domänen genom `Identifier`; jämförelsen för fordonsdubbletter ska vara skiftlägesokänslig. Detta har redan verifierats på domännivå.
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
- Efter incheckning, utcheckning och flytt uppdateras det persistenta tillståndet med `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()`. Grundläggande kompensation i minnet finns vid rapporterade sparfel, men felinjektion, garanterad återställning och filersättningens gränsfall är inte verifierade.
- Saknad runtime-konfiguration får återskapas med standardvärden.
- En befintlig men ogiltig konfiguration ska inte skrivas över automatiskt; programmet ska i stället avsluta kontrollerat.
- `AppTheme.ShowMenu()` används för återanvändbar menypresentation, och `AppTheme.WriteCenteredPanel()` för centrerade paneler. `MessageService` visar centrerade statusmeddelanden. I `MoveVehicleView` hanteras felaktiga platsnummer med `while`/`continue` och ett omritat inmatningsområde så att felpaneler inte staplas.
- `ParkVehicleView`, `CheckOutVehicleView` och `MoveVehicleView` är kopplade till huvudmenyn och har manuellt testade normalflöden med JSON-persistens. `Find Vehicle` och `Administration` är fortfarande platshållare.
- `GarageOverView` använder ett kompakt Spectre.Console-`Grid` i stället för tabellrader med avgränsare eller separata paneler per parkeringsplats. Antalet visade kolumner och en eventuell viewport beror på terminalutrymmet; `GARAGE STATUS` visar lediga och upptagna platser samt antal fordon. 200 platser har kontrollerats med både bilar och två MC på samma plats; fler terminalstorlekar, navigeringsgränser och återställning av två MC efter omstart återstår.
- **VG – Bus:** Vid nyparkering gäller fyra sammanhängande platser inom nummer 1–50, men vid flytt kan fyra sammanhängande platser även ligga efter nummer 50. Bussens exakta flerplatsrepresentation i JSON är ännu inte beslutad.
- Körbar Windows-version och eventuell Docker-baserad betatestning på Synology NAS planeras efter att kärnfunktionerna är färdiga; detta är inte nya uppgiftskrav.

## Kvarstående verifiering före färdig G-version

- Felinjektion vid `ParkingDataService.Save()` för incheckning, utcheckning och flytt; kontrollera att minnet återställs och att felmeddelanden inte felaktigt anger framgång.
- Omstartstest för två MC på samma parkeringsplats med oförändrade identifierare och UTC-ankomsttider; testa även full MC-plats och bil/MC-blandning vid fler scenarier.
- Negativa JSON-fall: korrupt fil, saknade obligatoriska fält, dubbletter, ogiltiga platsnummer och avvikande kapacitet samt temporärfilshantering.
- Slutföra `Find Vehicle` och `Administration` (G: omläsning av prislistan), därefter testa hela menyn och programmets beteende på annan dator.
- Granska resterande navigerings- och terminalstorleksfall för garageöversikten samt slutlig kravavstämning mot kursuppgiften.

# Future improvements

- Undersök möjlighet att visuellt gruppera flera sammanhängande lediga parkeringsplatser i garageöversikten utan att försämra den kompakta kartan eller orsaka terminalscrollning.
- Utforska möjlighet till separat webbaserad betatestning med Docker och webterminal utan att exponera NAS-administration. En framtida gemensam parkeringsmiljö kräver samordning av samtidiga ändringar; en delad JSON-fil mellan fristående processer räcker inte på egen hand.
