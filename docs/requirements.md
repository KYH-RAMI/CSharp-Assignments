# Prague Parking V2 – Kravanalys

**Uppdaterad:** 2026-10-14

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

**Lägesbild 2026-10-14:** `ParkingDataMapper` i applikationslagret återställer nu sparade fordonsobjekt på deras ursprungliga platser vid programstart. `ParkVehicleView` sparar automatiskt till JSON innan lyckad parkering bekräftas. Ett end-to-end-test har verifierat att `UKB48K` låg kvar på plats 002 efter omstart tillsammans med tidigare sparade `ABC123` på plats 001, med bevarad ankomsttid och korrekt garagebeläggning. `GarageOverView` visade 198 lediga och två fulla bilplatser av totalt 200. Därmed är normalflödet för **incheckning → JSON-sparning → omstart → återställning** verifierat för bil. **Checkout, Move, Find och Administration saknar färdiga UI-flöden; skrivfel, MC-återställning och negativ validering återstår.** Statusen beskriver dokumenterade tester, inte en slutlig genomgång av hela repositoryt.

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-00 | Grundfunktionaliteten från Prague Parking V1 ska finnas kvar | Incheckning fungerar via UI med JSON-sparning; översikten visar återställda fordon. Domänlogik för sökning och utcheckning finns, men Check Out, Move, Find och Administration återstår som kompletta UI-flöden. | Domain / UI | Pågår |
| G-01 | Parkerade fordonsdata ska sparas permanent | Normalflödet för permanent lagring vid incheckning och återställning efter omstart har verifierats 2026-10-14. Permanens efter utcheckning/flytt och felscenarier återstår. | Persistence | Pågår |
| G-02 | Parkeringsdata ska lagras i JSON-format | Det beslutade JSON-formatet och `System.Text.Json` används. En ny parkerad bil har sparats och återlästs från fil efter omstart; full filfelsverifiering återstår. | Persistence | Verifierad |
| G-03 | Parkeringsdata ska läsas in vid programstart | `Program.cs` läser JSON och `ParkingDataMapper.ToGarage()` återställer bilar på samma plats med ursprunglig UTC-tid. Biltest verifierat med `ABC123`/`UKB48K`; negativa fall återstår. | Persistence / Domain | Verifierad |
| G-04 | Datafilen ska uppdateras vid incheckning | `ParkVehicleView` anropar `ToDocument()` och `Save()` efter parkering, före framgångsmeddelandet. Normal bilparkering → omstart är verifierad; skrivfel-/rollback-test återstår. | Persistence | Verifierad |
| G-05 | Datafilen ska uppdateras vid utcheckning | Efter lyckad utcheckning ska aktuell parkeringsdata sparas och det utcheckade fordonet inte längre finnas i filen; sparfel ska hanteras kontrollerat | Persistence | Ej påbörjad |
| G-06 | Datafilen ska uppdateras när ett fordon flyttas | Efter lyckad flytt ska både tidigare och ny plats återspeglas i filen, utan att fordonets `ArrivalTime` ändras; sparfel får inte lämna ett motsägelsefullt tillstånd | Persistence | Ej påbörjad |
| G-07 | Systemet ska kunna hantera olika parkeringskapacitet | I G används konfigurerbart antal fordon per plats. Kapacitetsregler för Car och MC har implementerats och verifierats. Den generella storleksmodellen för Bicycle, MC, Car och Bus implementeras först i VG-versionen | Domain / Configuration | Verifierad |
| G-08 | Parkeringshuset ska visas som en karta/översikt | Kompakt dynamisk `Grid`-karta med Spot Details och Garage Status; 200 platser och två upptagna bilplatser har visats utan oönskad terminalscrollning i testfönstret. Viewport och gränsfall återstår. | UI | Implementerad |
| G-09 | Det ska vara enkelt att se hur många platser som är lediga | Garage Status har verifierats efter omstart med 200 platser: `FREE 198`, `OCCUPIED 2`, `VEHICLES 2`. Två fulla bilplatser visades röda; orange MC-status återstår. | UI | Verifierad |
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
| G-22 | Fordonets ankomsttid ska registreras | Nyincheckning sätter UTC-tid. JSON-återställningen behöll bilens ursprungliga ankomsttid efter omstart; ytterligare fordonstyper återstår att testa. | Domain / Persistence | Verifierad |
| G-23 | Systemet ska kunna ange hur länge fordonet varit parkerat | Parkeringstiden beräknas från `ArrivalTime` och `departureTime` och returneras via `ParkingFeeResult` och `CheckoutResult`. Funktionen har verifierats i checkout-test | Domain | Verifierad |
| G-24 | Applikationen ska ha ett visuellt användargränssnitt i konsolen | Spectre.Console används i aktiva vyer, inklusive verifierad incheckning med sparning och garageöversikt med parkerade bilar. Checkout, flytt, sökning och administration är ännu inte färdiga UI-flöden. | UI | Pågår |
| G-25 | Konsolapplikationen ska använda Spectre.Console | Spectre.Console används i huvudmeny, statusmeddelanden och garageöversikt | UI | Verifierad |
| G-26 | Alla fordon ska hanteras som objekt | Fordonsdata representeras med objekt i stället för sammansatta strängar som i V1 | Domain | Implementerad |
| G-27 | Systemet ska innehålla minst fem klasser | Kravet är uppfyllt genom bland annat `Vehicle`, `Car`, `MC`, `ParkingSpot`, `ParkingGarage`, `ParkingFeeCalculator`, `ParkingFeeResult`, `CheckoutService` och `CheckoutResult` | Architecture / Domain | Implementerad |
| G-28 | Klassen `Vehicle` ska finnas | Gemensamma egenskaper och regler för fordon samlas i en abstrakt basklass | Domain | Implementerad |
| G-29 | `Car` ska ärva från `Vehicle` | Bil modelleras som en specialisering av `Vehicle` | Architecture / Domain | Implementerad |
| G-30 | `MC` ska ärva från `Vehicle` | MC modelleras som en specialisering av `Vehicle` | Architecture / Domain | Implementerad |
| G-31 | `ParkingSpot` ska finnas som klass | Varje parkeringsplats representeras av ett objekt med platsnummer och parkerade fordon | Domain | Implementerad |
| G-32 | `ParkingGarage` ska finnas som klass | Parkeringshuset ansvarar för helheten och samlingen av parkeringsplatser | Domain | Implementerad |
| G-33 | Standardvärdet ska vara 100 parkeringsplatser | Standardkonfigurationen skapar 100 platser, men värdet kan ändras via konfigurationen | Domain / Configuration | Verifierad |
| G-34 | Lösningen ska innehålla ett separat klassbibliotek för dataåtkomst | `PragueParkingV2.Data` finns och hanterar både konfiguration och grundläggande JSON-persistens med `ParkingDataService` och tre DTO-klasser | Architecture / Persistence | Implementerad |
| G-35 | Dataåtkomst ska separeras från övrig logik | Data-projektet ansvarar för JSON/konfiguration; `ParkingDataMapper` i Application översätter mot interna domänklasser. UI-sparning för incheckning är inkopplad; robust felhantering och övriga åtgärder återstår. | Architecture / Persistence | Pågår |
| G-36 | Inställningar ska läsas från konfigurationsfilen vid programstart | `config.json` läses och valideras innan parkeringsmodellen byggs upp | Configuration / Domain | Verifierad |
| G-37 | Uppgiften ska huvudsakligen lösas med kursens hittills behandlade tekniker | Klasser, arv, collections, nästlade `foreach` och LINQ används där de ger tydlighet. `ToDocument()` använder LINQ och `ToGarage()` behåller nästlade loopar av läsbarhetsskäl. | Architecture | Pågår |

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
- Efter varje lyckad incheckning, utcheckning eller flytt ska det persistenta tillståndet uppdateras. Säker filersättning och konsekvent felhantering ska utformas vid implementationen.
- Saknad runtime-konfiguration får återskapas med standardvärden.
- En befintlig men ogiltig konfiguration ska inte skrivas över automatiskt; programmet ska i stället avsluta kontrollerat.
- `AppTheme.ShowMenu()` används för återanvändbar menypresentation. `MessageService` samlar återkommande statusmeddelanden; enhetlig integration i alla UI-vyer återstår att verifiera.
- `ParkVehicleView` har tagits fram men är inte ännu verifierad i ett genomgående flöde med persistent lagring.
- `GarageOverView` använder ett kompakt Spectre.Console-`Grid` i stället för tabellrader med avgränsare eller separata paneler per parkeringsplats. Antalet visade kolumner och en eventuell viewport beror på terminalutrymmet; `GARAGE STATUS` visar lediga och upptagna platser samt antal fordon. Den visuella kontrollen med 200 tomma platser är genomförd, medan ytterligare navigerings- och beläggningstester återstår.
- **VG – Bus:** Vid nyparkering gäller fyra sammanhängande platser inom nummer 1–50, men vid flytt kan fyra sammanhängande platser även ligga efter nummer 50. Bussens exakta flerplatsrepresentation i JSON är ännu inte beslutad.
- Körbar Windows-version och eventuell Docker-baserad betatestning på Synology NAS planeras efter att kärnfunktionerna är färdiga; detta är inte nya uppgiftskrav.

# Future improvements

- Undersök möjlighet att visuellt gruppera flera sammanhängande lediga parkeringsplatser i garageöversikten utan att försämra den kompakta kartan eller orsaka terminalscrollning.
- Utforska möjlighet till separat webbaserad betatestning med Docker och webterminal utan att exponera NAS-administration. En framtida gemensam parkeringsmiljö kräver samordning av samtidiga ändringar; en delad JSON-fil mellan fristående processer räcker inte på egen hand.
