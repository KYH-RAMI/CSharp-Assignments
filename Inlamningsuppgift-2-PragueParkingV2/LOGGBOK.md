## 2026-10-05

### Vad jag gjorde
- Skapade solutionen PragueParkingV2 i .NET 10.
- Skapade separat klassbibliotek `PragueParkingV2.Data`.
- Lade till projektreferens från huvudprojektet till Data-projektet.
- Skapade dokumentationsstruktur med README, loggbok och docs.
- Gick igenom G-kraven och skapade en kravmatris.
- Började skissa klassdiagrammet för Vehicle, Car, MC, ParkingSpot och ParkingGarage.

### Beslut
- UI, domänlogik och dataåtkomst ska hållas separerade.
- Parkeringsdata ska sparas som JSON.
- Prislista och konfiguration ska kombineras i `config.json`.
- Priser ska ligga tidigt i konfigurationsfilen för enklare redigering.
- ParkingSpot identifieras med platsnummer.
- Motorfordon identifieras med registreringsnummer.
- Vehicle ska inte lagra sitt ParkingSpot-nummer eftersom placeringen redan kan bestämmas av vilken ParkingSpot som innehåller fordonet.

### Nästa steg
- Fortsätta utveckla klassdiagrammet.
- Bestämma klassernas properties och kapacitetsmodell.
- Därefter planera beteenden/metoder innan implementation.


## 2026-10-06

### Genomfört

- Installerat och konfigurerat Spectre.Console.
- Skapat domänklasserna `Vehicle`, `Car`, `MC`, `ParkingSpot` och `ParkingGarage`.
- `Vehicle` har fått:
  - identifierare
  - automatisk ankomsttid i UTC
  - validering av identifierare
  - stöd för återställning av en tidigare sparad ankomsttid
- `Car` och `MC` ärver från `Vehicle`.
- `ParkingSpot` har fått:
  - platsnummer
  - privat lista över parkerade fordon
  - `CanPark()` för kontroll av kapacitet och fordonstyp
  - `ParkVehicle()` för kontrollerad parkering
  - `FindVehicle()` för sökning på den egna parkeringsplatsen
  - `RemoveVehicle()` för borttagning av ett fordon från den egna platsen
- `ParkingGarage`:
  - skapar antal parkeringsplatser utifrån konfigurationen
  - har `FindAvailableSpot()` för att hitta första lämpliga parkeringsplats
  - har `FindVehicle()` för sökning genom hela garaget
  - har `ParkVehicle()` för att hitta en plats och parkera fordonet
  - kontrollerar att samma identifierare inte checkas in flera gånger
- Skapat `config.json` med:
  - priser
  - antal parkeringsplatser
  - tidszon
  - kapacitetsregler för Car och MC
- Skapat typad konfigurationsmodell i `AppConfiguration`.
- Implementerat `ConfigurationService` med `System.Text.Json`.
- Om runtime-versionen av `config.json` saknas skapas den med standardvärden.
- Befintlig konfiguration valideras innan den används.
- Ogiltiga priser, parkeringsantal, tidszoner och fordonsregler stoppas.
- Skapat central `MessageService` med Spectre.Console-paneler för:
  - SUCCESS
  - INFO
  - WARNING
  - ERROR
- Startup-fel hanteras kontrollerat i stället för att ge obehandlade exceptions.
- Verifierat att lösningen bygger med både huvudprojektet och `PragueParkingV2.Data`.
- Uppdaterat projektets dokumentation:
  - kravanalys
  - klassdiagram
  - konfigurationsdokumentation
  - planerad struktur för `parking-data.json`
  - designbeslut
  - projektplan
  - README

### Tester

- Testat att borttagen runtime-`config.json` återskapas automatiskt.
- Testat att standardvärden skrivs korrekt till JSON.
- Testat ogiltigt antal parkeringsplatser (`-100`).
- Verifierat att ogiltig konfiguration stoppas och visas med ett tydligt felmeddelande.
- Verifierat att 100 parkeringsplatser skapas från konfigurationen.
- Testat `FindAvailableSpot()` och `ParkVehicle()` med en bil och tre motorcyklar.
- Verifierat att en bil får en egen parkeringsplats.
- Verifierat att två MC kan dela samma parkeringsplats när `VehiclesPerSpot` är 2.
- Verifierat att en tredje MC automatiskt placeras på nästa lediga parkeringsplats.
- Verifierat att bil och MC inte blandas på samma parkeringsplats.
- Testat att `ABC123` och `abc123` behandlas som samma identifierare.
- Verifierat att dubbel incheckning stoppas innan en ny parkeringsplats tilldelas.
- Verifierat att programmet avslutas kontrollerat vid kritiska startup-fel.

### Problem och lösningar

- Under test av automatisk återskapning av konfiguration råkade projektets ursprungliga `Config/config.json` tas bort i stället för endast runtime-kopian i `bin`.
- Detta gjorde att Visual Studio inte kunde kopiera konfigurationsfilen vid build.
- Ursprungsfilen återställdes och skillnaden mellan projektets konfigurationsfil och runtime-kopian blev tydligare.
- Upptäckte även att flera dokument hade blivit inaktuella i takt med implementationen. Dessa uppdaterades så att dokumentationen åter speglar projektets aktuella tillstånd.

### Nästa steg

- Implementera `ParkingGarage.RemoveVehicle()`.
- Bygga det riktiga utcheckningsflödet.
- Implementera prisberäkning utifrån `ArrivalTime` och konfigurationen.
- Implementera flytt av fordon mellan parkeringsplatser.
- Implementera persistens i `parking-data.json`.

## 2026-10-07

### Genomfört 

- Verifierat med läraren att `Identifier` är rätt generell identifierare för fordon eftersom alla fordonstyper inte har registreringsnummer.
- Verifierat att en Bus i VG-versionen ska kräva fyra sammanhängande parkeringsplatser, motsvarande totalt 16 kapacitetsenheter.

### Reflektion

- `Vehicle` är en generell basklass för alla fordonstyper och använder därför den generella egenskapen `Identifier`.
- `Car` och `MC` ärver från `Vehicle`, och deras `Identifier` motsvarar i praktiken registreringsnumret.
- Den generella benämningen gör modellen mer flexibel eftersom framtida fordonstyper, exempelvis Bicycle, kan använda en annan typ av identifierare.


## 2026-10-08

### Genomfört

- Implementerat `ParkingGarage.RemoveVehicle()`.
- Metoden söker igenom garagets parkeringsplatser och försöker ta bort fordonet från respektive `ParkingSpot`.
- Om fordonet hittas returneras det borttagna `Vehicle`-objektet.
- Om inget fordon med angiven identifierare finns returneras `null`.
- Återanvänt befintlig `ParkingSpot.RemoveVehicle()` i stället för att ge `ParkingGarage` direkt åtkomst till listan över parkerade fordon.
- Behållit inkapslingen genom att `_parkedVehicles` fortfarande endast hanteras av `ParkingSpot`.

### Fortsatt arbete

- Skapat domänklassen `ParkingFeeCalculator`.
- Implementerat prisberäkning utifrån:
  - fordonets `ArrivalTime`
  - angiven `departureTime`
  - pris per påbörjad timme
  - gratisgräns i minuter
- Implementerat stöd för olika timpriser för `Car` och `MC`.
- Refaktorerat `ParkingFeeCalculator` så att prisregler laddas in vid skapandet av objektet i stället för att skickas in vid varje metodanrop.
- `ParkingFeeCalculator` lagrar nu aktiva värden för:
  - Car-pris
  - MC-pris
  - antal gratisminuter
- Implementerat `UpdatePricing()` för framtida omläsning av prislistan.
- `UpdatePricing()` validerar samtliga nya värden innan de aktiva värdena skrivs över.
- Implementerat typkontroll med `switch` för att välja rätt timpris utifrån om fordonet är `Car` eller `MC`.
- Skapat grunden till `CheckoutService` för att senare samordna sökning, prisberäkning och borttagning vid utcheckning.
- Flyttat skapandet av `ParkingFeeCalculator` till efter att konfigurationen har lästs in i `Program.cs`.
- Förenklat läsbarheten i `Program.cs` genom att använda en lokal `pricing`-variabel för prisinställningarna.

### Tester

- Verifierat att parkering kortare än 10 minuter ger `0 CZK`.
- Verifierat att exakt 10 minuter debiteras som en påbörjad timme.
- Verifierat att exakt 60 minuter debiteras som en timme.
- Verifierat att 60 minuter och 1 sekund debiteras som två påbörjade timmar.
- Verifierat MC-pris med 61 minuters parkering.
- Verifierat att `departureTime` före `ArrivalTime` stoppas med `ArgumentOutOfRangeException`.
- Verifierat att prisberäkningen fungerar korrekt med UTC-baserade tider.

### Designbeslut

- Prislogiken ska hållas separat från `ParkingGarage` och `ParkingSpot`.
- `ParkingFeeCalculator` ansvarar för prisregler och avgiftsberäkning.
- `CheckoutService` ska samordna själva utcheckningsprocessen.
- Aktiva prisvärden hålls i minnet efter att `config.json` har lästs in.
- Vid framtida reload ska nya prisvärden valideras innan de ersätter de gamla aktiva värdena.
- Ogiltiga reload-värden ska inte påverka redan fungerande konfiguration.

### Nästa steg

- Slutföra `CheckoutService`.
- Koppla ihop:
  - `FindVehicle()`
  - `ParkingFeeCalculator.CalculateParkingFee()`
  - `RemoveVehicle()`
- Testa ett komplett checkout-flöde.
- Därefter fortsätta med flytt av fordon och persistens i `parking-data.json`.

## 2026-10-09

### Genomfört

#### Prisberäkning och utcheckning

- Skapat `ParkingFeeResult` som read-only resultatobjekt för prisberäkning.
- Ändrat `ParkingFeeCalculator.CalculateParkingFee()` så att metoden returnerar ett `ParkingFeeResult`.
- Skapat `CheckoutResult` som innehåller:
  - fordonsidentifierare
  - ankomsttid
  - avgångstid
  - total parkeringstid
  - timpris
  - gratisminuter
  - total avgift
- Implementerat `CheckoutService`.
- `CheckoutService` samordnar:
  - sökning efter fordon
  - prisberäkning
  - borttagning av fordon
  - skapande av `CheckoutResult`
- Säkerställt att prisberäkning sker innan fordonet tas bort från garaget.
- Verifierat att `UpdatePricing()` uppdaterar de aktiva prisreglerna i samma `ParkingFeeCalculator`-instans.

#### GarageOverView – Interaktiv parkeringsöversikt

- Påbörjat implementationen av `GarageOverView` med Spectre.Console.
- Implementerat en interaktiv parkeringsöversikt som använder befintliga objekt från `ParkingGarage` och `ParkingSpot`.
- Implementerat dynamisk pagination:
  - 25 parkeringsplatser per sida.
  - Antalet sidor beräknas utifrån parkeringshusets faktiska storlek.
  - Parkeringsplatser sorteras efter `SpotNumber`.
  - Sista sidan hanterar även färre än 25 parkeringsplatser.
- Utvecklat den ursprungliga tabellbaserade översikten till en parkeringskarta med `Grid` och individuella `Panel`-komponenter.
- Parkeringskartan använder fem kolumner och fem rader per sida.
- Varje parkeringsplats har en egen ram och visar:
  - parkeringsplatsnummer
  - parkeringsstatus
  - beläggning när platsen är delvis upptagen
- Implementerat färgkodning för parkeringsstatus:
  - GREEN – FREE
  - ORANGE – PARTIALLY OCCUPIED
  - RED – FULL
- Kapacitetsgränser hämtas från `AppConfiguration` och är inte hårdkodade i presentationslogiken.
- Implementerat visuell markering av vald parkeringsplats:
  - `LightGoldenrod1` används för markerad parkeringsplats.
  - Vita ramar används för övriga parkeringsplatser.
- Implementerat tangentbordsnavigering:
  - Pil upp – föregående rad.
  - Pil ned – nästa rad.
  - Pil vänster – föregående kolumn.
  - Pil höger – nästa kolumn.
  - Page Up – föregående sida.
  - Page Down – nästa sida.
  - ESC – lämna garageöversikten.
- Navigeringen använder `selectedSpotIndex` för att hålla reda på markerad parkeringsplats.
- Markeringen återställs till första parkeringsplatsen vid sidbyte.
- Påbörjat en dynamisk informationspanel med rubriken `SPOT DETAILS`.
- Informationspanelen visar för närvarande:
  - markerat parkeringsplatsnummer
  - antal parkerade fordon
- Använt `Columns` för att placera parkeringskartan och informationspanelen bredvid varandra.
- Anpassat gränssnittet för att minska behovet av terminalscrollning.
- Implementerat en footer med:
  - aktuellt sidnummer
  - totalt antal sidor
  - parkeringsplatsintervall
  - totalt antal parkeringsplatser
  - tangentbordsinstruktioner

### Tester

#### Prisberäkning och utcheckning

- Testat `UpdatePricing()` genom att ändra Car-priset från 20 till 25 CZK.
- Verifierat att 61 minuters parkering ändrades från 40 CZK till 50 CZK efter uppdateringen.
- Genomfört ett komplett checkout-test med fordonet `UKB48K`.
- Verifierat att 61 minuters parkering med 20 CZK per timme ger 40 CZK.
- Verifierat att `CheckoutResult` innehåller korrekt:
  - identifierare
  - parkeringstid
  - timpris
  - gratisgräns
  - total avgift
- Verifierat att fordonet inte längre finns i `ParkingGarage` efter lyckad checkout.
- Testkörningen avslutades utan fel med exit code `0`.

#### GarageOverView

- Testat parkeringsöversikten i Windows Terminal.
- Verifierat visuellt att 100 parkeringsplatser delas upp på fyra sidor.
- Verifierat att första sidan visar parkeringsplatserna 001–025.
- Kontrollerat att parkeringskartan visas som ett 5 × 5-rutnät.
- Kontrollerat att individuella parkeringsplatser presenteras med separata ramar.
- Kontrollerat att markerad parkeringsplats framhävs med en annan ramfärg.
- Testat sidnavigeringen under utvecklingen.
- Kontrollerat att informationspanelen visar den markerade parkeringsplatsens nummer.
- Kontrollerat att tomma parkeringsplatser visas med grön `FREE`-status.
- Tidigare problem med en för hög vertikal tabell har hanterats genom att ersätta tabellpresentationen med `Grid` och `Panel`.

Följande återstår att verifiera:

- ORANGE och RED med faktiskt parkerade fordon.
- Samtliga navigeringsgränsfall.
- Sista sidan när antalet parkeringsplatser inte är jämnt delbart med 25.
- Layouten vid olika terminalstorlekar.
- Informationspanelen när en eller två motorcyklar finns på samma parkeringsplats.

### Problem och lösningar

#### Problem 1 – Garageöversikten orsakade terminalscrollning

Den första implementationen använde en vertikal `Table` med
25 parkeringsplatser per sida.

Tabellen tog för mycket plats på höjden och orsakade problem med
scrollning och presentation vid tangentbordsnavigering.

**Lösning:**

Parkeringsöversikten byggdes om till ett 5 × 5-rutnät med Spectre.Console
`Grid` och individuella `Panel`-komponenter.

Detta minskade behovet av vertikalt utrymme och gav bättre
överblick över parkeringshuset.

#### Problem 2 – Markerad parkeringsplats var svår att urskilja

Den ursprungliga implementationen använde en gul pil framför
parkeringsplatsnumret.

Markeringen var inte tillräckligt tydlig i den nya parkeringskartan.

**Lösning:**

Varje parkeringsplats fick en individuell ram.

Den markerade parkeringsplatsen framhävs med `LightGoldenrod1`,
medan övriga parkeringsplatser använder vita ramar.

#### Problem 3 – Layouten utnyttjade inte terminalens bredd

Den tidigare presentationen placerade informationspanelen långt
från parkeringskartan.

**Lösning:**

Layouten anpassades med Spectre.Console `Columns` och
`Expand = false`, vilket placerar komponenterna närmare varandra.

#### Problem 4 – Navigeringen behövde anpassas till rutnätet

Den första tangentbordsnavigeringen utgick från en vertikal lista.

Den fungerade inte naturligt när parkeringsplatserna senare
presenterades i fem kolumner.

**Lösning:**

Navigeringen anpassades till rutnätets struktur.

Uppåt- och nedåtpilarna flyttar markeringen fem positioner,
medan vänster- och högerpilarna flyttar en position inom samma rad.

Page Up och Page Down används separat för sidbyte.

### Reflektion

- `CheckoutService` fungerar som samordnande domänklass och gör att `Program.cs` inte behöver innehålla hela utcheckningslogiken.
- `ParkingFeeCalculator` ansvarar endast för prisberäkning medan `ParkingGarage` fortsatt ansvarar för fordonens placering.
- Separata resultatobjekt gör att UI senare kan presentera checkout-information utan att innehålla affärslogik.
- Arbetet med Spectre.Console har visat skillnaden mellan `Table`, `Grid`, `Panel` och `Columns`.
- En parkeringskarta med individuella paneler ger större kontroll över markeringar och presentation än en traditionell tabell.
- Det är viktigt att anpassa tangentbordsnavigeringen efter hur informationen faktiskt presenteras.
- Separationen mellan UI och domänlogik har gjort det möjligt att förändra garageöversiktens utseende utan att ändra `ParkingGarage`, `ParkingSpot` eller `Vehicle`.
- Projektet har blivit mer komplext i presentationslagret, men den objektorienterade domänmodellen har kunnat behållas.

### Nästa steg

- Färdigställa `SPOT DETAILS` med:
  - parkeringsstatus
  - fordonsidentifierare
  - fordonstyp
  - ankomsttid
  - information om beläggning
- Verifiera GREEN, ORANGE och RED med parkerade fordon.
- Genomföra ytterligare tester av pagination och tangentbordsnavigering.
- Implementera det fullständiga användarflödet för incheckning.
- Implementera det fullständiga UI-flödet för utcheckning.
- Implementera flytt av fordon mellan parkeringsplatser.
- Implementera persistens av parkeringsdata i `parking-data.json`.
- Implementera omläsning av prislistan via meny.
- Uppdatera kravanalys och övrig dokumentation efter genomförda ändringar.
- Fortsätta mot en stabil Prague Parking 2.0 innan arbetet med VG-versionen påbörjas.

---

## 2026-10-10

### Genomfört

#### MainMenu – Huvudmeny

- Skapat `MainMenu.cs` i UI-lagret.
- Implementerat en interaktiv huvudmeny.
- Menyn innehåller:
  - Park Vehicle
  - Check Out Vehicle
  - Move Vehicle
  - Find Vehicle
  - Garage Overview
  - Administration
  - Exit
- Implementerat tangentbordsnavigering med piltangenterna.
- Enter används för att välja en funktion.
- ESC används för att avsluta huvudmenyn.
- Kopplat Garage Overview till huvudmenyn.
- Implementerat återgång från Garage Overview.
- Behållit övriga menyfunktioner som ännu inte implementerade.
- Separat menyklass används för att undvika att lägga navigeringslogik i `Program.cs`.

#### AppTheme – Gemensam färgpalett

- Skapat `AppTheme.cs` i UI-lagret.
- Tagit fram ett färgtema inspirerat av svenska flaggan.
- Implementerat gemensamma färgdefinitioner för:
  - bakgrund
  - text
  - sekundär information
  - panelramar
  - accentfärger
- Implementerat en guldgradient i projekttiteln.
- Använt motsvarande gradient för `Author: Rami Issa`.
- Utvecklat en enhetlig mörkblå bakgrund.
- Påbörjat anpassning av garageöversikten till samma färgtema.

#### Console Rendering

- Undersökt olika metoder för att rendera ett konsolgränssnitt.
- Testat Spectre.Console SelectionPrompt.
- Testat egen tangentbordsnavigering.
- Testat Spectre.Console Live.
- Undersökt hur Console.Clear() påverkar gränssnittet.
- Undersökt positionsbaserad rendering med Console.SetCursorPosition().
- Implementerat en huvudmeny där endast förändrade menyval behöver ritas om.
- Förbättrat navigeringen för att minska synligt flimmer.
- Implementerat centrering utifrån terminalens bredd och höjd.
- Infört en alternativ meny för terminalfönster som är för små för den ordinarie presentationen.

#### GarageOverView

- Fortsatt anpassningen av garageöversikten till det nya färgtemat.
- Tagit fram en guldgradient för rubriken GARAGE OVERVIEW.
- Arbetat med en mer sammanhängande footer.
- Planerat utökning av informationspanelen SPOT DETAILS.

#### Spot Details

- Påbörjat vidareutvecklingen av informationspanelen för markerad parkeringsplats.
- Utformat presentation för:
  - parkeringsplatsnummer
  - parkeringsstatus
  - beläggning
  - samtliga parkerade fordon
  - fordonstyp
  - identifierare
  - ankomsttid
  - parkeringstid
- Planerat stöd för att visa två motorcyklar på samma parkeringsplats.
- Använt befintliga egenskaper från Vehicle och ParkingSpot.
- Tagit fram logik för att beräkna Duration från ArrivalTime.
- Duration ska presenteras i dagar, timmar och minuter.
- UTC används för tidsberäkningen.
- Ankomsttiden ska presenteras enligt den konfigurerade tidszonen.
- Tidszonens namn behöver inte visas i informationspanelen.
- Slutlig verifiering av den senaste implementationen återstår.

### Tester

#### MainMenu

- Kontrollerat visuellt att huvudmenyn visas centrerad.
- Testat navigering mellan menyval.
- Testat olika metoder för att uppdatera markeringen.
- Observerat att omritning av hela konsolen orsakar synligt flimmer.
- Konstaterat att selektiv uppdatering av menyval ger en förbättrad användarupplevelse.
- Kontrollerat huvudmenyns presentation i Windows Terminal.
- Kontrollerat att Garage Overview kan öppnas från huvudmenyn.

#### AppTheme

- Testat flera blå bakgrundsfärger.
- Testat gradientbaserad bakgrund.
- Observerat problem när Spectre.Console uppdaterade delar av skärmen.
- Undersökt skillnader mellan terminalens standardbakgrund och applikationens bakgrund.
- Valt att prioritera en enhetlig mörkblå bakgrund framför en fullständig bakgrundsgradient.
- Kontroll av bakgrundsfärger och kompatibilitet behöver fortsätta.

#### Spot Details

Följande tester återstår:

- Ledig parkeringsplats.
- Parkeringsplats med en bil.
- Parkeringsplats med en motorcykel.
- Parkeringsplats med två motorcyklar.
- Korrekt fordonsidentifierare.
- Korrekt ankomsttid.
- Korrekt beräkning av Duration.
- Korrekt status för delvis upptagen och full parkeringsplats.

### Problem och lösningar

#### Problem 1 – Huvudmenyn var oproportionerlig

Den ursprungliga presentationen använde en bred header och en separat vänsterjusterad meny.

Det gav ett osammanhängande gränssnitt.

**Lösning:**

Huvudmenyn byggdes om till en centrerad layout med gemensam panel och anpassad navigering.

#### Problem 2 – Synligt flimmer vid navigering

När konsolen rensades och hela huvudmenyn ritades om vid varje tangenttryckning uppstod flimmer.

**Lösning:**

Undersökte Spectre.Console Live och gick därefter vidare med positionsbaserad rendering.

Endast tidigare och nytt markerat menyval behöver uppdateras.

#### Problem 3 – Bakgrunden visades i olika blå nyanser

Applikationens anpassade bakgrund och terminalens standardbakgrund hade olika färger.

Problemet blev särskilt synligt när Spectre.Console uppdaterade delar av skärmen.

**Lösning:**

Arbetat med att samordna applikationens bakgrundsinställningar och konsolrendering.

En enhetlig mörkblå bakgrund prioriteras framför en mer komplicerad bakgrundsgradient.

#### Problem 4 – Informationspanelen innehöll för lite information

Den ursprungliga SPOT DETAILS-panelen visade endast parkeringsplatsnummer och antal fordon.

Det var inte tillräckligt för att ge användaren en komplett översikt.

**Lösning:**

Planerat en utökad informationspanel med samtliga parkerade fordon, identifierare, fordonstyp, ankomsttid och Duration.

### Reflektion

- Jag har fått en bättre förståelse för skillnaden mellan applikationens färgtema och terminalens egna färginställningar.
- Arbetet med huvudmenyn har visat hur Console.SetCursorPosition() kan användas för att kontrollera placeringen av innehåll.
- Jag har också lärt mig varför det kan vara bättre att uppdatera en mindre del av gränssnittet i stället för att rita om hela konsolen.
- Spectre.Console erbjuder färdiga komponenter, men vissa mer anpassade gränssnitt kräver kompletterande konsolhantering.
- Genom att samla färgdefinitioner i AppTheme blir gränssnittet enklare att underhålla.
- Jag har fortsatt att hålla presentationen separat från parkeringssystemets domänlogik.
- Jag har även fått en bättre förståelse för hur TimeSpan kan användas för att presentera parkeringstid utan att lagra information som kan beräknas vid behov.

### Fortsatt arbete den 10 oktober

#### Refaktorering av gemensamma UI-komponenter

- Vidareutvecklat `AppTheme` som central plats för färger, paneler, bakgrund och gemensam menypresentation.
- Samlat återanvändbar menynavigering i `AppTheme.ShowMenu()` så att varje vy inte behöver implementera samma tangentbordshantering.
- Använt `AppTheme.CreatePanel()` som gemensam utgångspunkt för panelernas utformning.
- Förtydligat ansvarsfördelningen: `MainMenu` väljer och öppnar funktioner, `AppTheme` hanterar gemensam presentation och `MessageService` hanterar återkommande statusmeddelanden.
- Gått igenom hur `PrepareScreen()` och `RestoreScreen()` används för konsolens bakgrund och återställning. Kompatibiliteten med andra terminaler är ännu inte verifierad.

#### GarageOverView och SPOT DETAILS

- Arbetat vidare med att dela upp garageöversiktens presentation i mindre, tydligare metoder för rutnät, platsdetaljer och footer.
- Tagit fram presentation för fordonstyp, identifierare, ankomsttid och parkeringstid utifrån befintliga domänobjekt.
- Behållit 5 × 5-kartan, pagineringen och de konfigurationsstyrda beläggningsreglerna.
- Anpassat färger och paneler till det gemensamma temat i `AppTheme` i stället för separata färgval i varje vy.
- Gjort en visuell genomgång av den omarbetade layouten. Fullständiga tester med parkerade fordon, statusfärger och navigeringens gränsfall återstår.

#### ParkVehicleView – påbörjat incheckningsflöde

- Tagit fram kod för en separat `ParkVehicleView` där användaren kan välja `Car` eller `MC` och ange fordonsidentifierare.
- Återanvänt `AppTheme.ShowMenu()` och `MessageService` i det planerade UI-flödet.
- Använt befintliga domänklasser för validering, kontroll av dubbletter och tilldelning av första lämpliga parkeringsplats.
- Förberett presentation av ankomsttid i den tidszon som anges i `config.json`.
- **Status:** Vyn är framtagen men ett fullständigt genomgående incheckningstest och kopplingen till permanent lagring återstår. Den får inte visa att en parkering har sparats permanent förrän JSON-sparningen har lyckats.

#### Parkeringsdata och framtida VG-stöd

- Gått igenom den tidigare beslutade strukturen för `parking-data.json` och behållit `ParkingSpots`, `SpotNumber`, `Vehicles`, `Identifier`, `VehicleType` och `ArrivalTime`.
- Fastställt att endast upptagna parkeringsplatser ska sparas och att parkeringsplats samt ursprunglig ankomsttid i UTC ska kunna återställas oförändrade.
- Planerat kontroller av dubbletter, felaktiga platsnummer, okända fordonstyper och kapacitetsöverträdelser innan sparad data accepteras.
- Beslutat att en befintlig ogiltig datafil inte ska skrivas över automatiskt. Säker skrivning och felhantering ska utformas vid implementationen.
- Förtydligat VG-regeln för `Bus`: nyparkering kräver fyra sammanhängande platser inom nummer 1–50, men en redan parkerad buss ska kunna flyttas till fyra sammanhängande platser även längre ner i garaget.
- Behållit G-versionens JSON-format utan att införa nya VG-fält. Bussens exakta representation över flera platser är ännu inte beslutad.
- **Status:** `ParkingDataService`, inläsning vid start och automatisk sparning är inte implementerade. Återställningskonstruktorerna för `Car` och `MC` behöver bekräftas i aktuell kod.

#### Dokumentation och framtida distribution

- Gått igenom och uppdaterat dokumentationen för klassdiagram, konfiguration, designbeslut, parkeringsdata, projektplan och kravanalys.
- Bevarat dokumentens egna mallar och skilt på **implementerat**, **verifierat** och **planerat** arbete.
- Korrigerat projektplanens prioritering så att JSON-persistensen kommer före slutlig integration av in- och utcheckningsflödena.
- Planerat att senare kunna distribuera en körbar Windows-version för betatestning utan att klasskamraterna behöver källkoden.
- Diskuterat en separat framtida DevSecOps-testmiljö med Docker på Synology NAS, webbaserad terminal och begränsad åtkomst via Cloudflare. Ingen sådan driftsättning har genomförts och NAS-administrationen ska förbli privat.
- Ingen Git-commit för den samlade dokumentationsuppdateringen har ännu utförts.

### Testläge efter dagens fortsatta arbete

- Tidigare manuella domäntester av kapacitet, dubblettkontroll, prisberäkning och `CheckoutService` kvarstår som genomförda.
- Garageöversikt och huvudmeny har kontrollerats visuellt, men den senaste refaktoreringen är inte fullständigt integrationstestad.
- Inga verifierade tester av JSON-sparning eller återställning finns ännu, eftersom persistensen inte är implementerad.
- `ParkVehicleView` behöver testas med både Car och MC, felaktig identifierare, dubbletter, fullt garage och återgång till huvudmenyn.
- Garageöversikten behöver testas med ledig, delvis upptagen och full plats samt med två motorcyklar på samma plats.
- Sparfel, felaktig befintlig JSON, omstart och korrekt bevarad UTC-ankomsttid blir särskilt viktiga tester när persistensen är färdig.

### Nästa steg

1. Kontrollera aktuell kod i `ParkingSpot.cs`, `Program.cs` och `ConfigurationService.cs` samt konstruktorsöverlagringarna i `Car` och `MC` innan nya beroenden införs.
2. Implementera `ParkingDataService` enligt `parking-data.md`, inklusive validerad återställning, säker sparning och korrekta fel vid misslyckad filskrivning.
3. Testa att ett parkerat fordon finns kvar på samma plats med samma ankomsttid efter omstart.
4. Integrera och testa `ParkVehicleView` med permanent sparning innan ett lyckat resultat visas.
5. Integrera `CheckoutService` i UI och koppla utcheckning till sparning.
6. Implementera och testa `Move Vehicle` och `Find Vehicle`, inklusive korrekt lagring efter flytt.
7. Implementera administrationsmenyn med validerad omläsning av prislistan.
8. Genomföra fullständiga G-tester, slutföra dokumentationen och först därefter påbörja VG-versionen.
9. Förbereda körbar Windows-version och eventuell Docker-baserad betatestning först när kärnfunktionerna är stabila.


---

## 2026-10-10 – Senare arbete med JSON-persistens

### Implementerat

- Skapat datamodellerna `ParkingDataDocument`, `ParkingSpotData` och `VehicleData` i `PragueParkingV2.Data/Persistence`.
- Implementerat `ParkingDataService` med `ParkingDataExists()`, `Load()` och `Save()`.
- Använt `AppContext.BaseDirectory` för `Data/parking-data.json`, `System.Text.Json` för serialisering/deserialisering och `Directory.CreateDirectory()` för saknad katalog.
- Lagt in skrivning till `.tmp`, `File.Move()` för ny fil, `File.Replace()` för befintlig fil och `try/finally` för att försöka rensa temporärfilen.
- Börjat validera parkeringsplatser och fordon innan data används eller sparas.
- Behållit JSON-formatet med endast upptagna parkeringsplatser; ingen ny VG-struktur infördes.

**Avgränsning:** Detta var en implementation av dataåtkomst, inte återställning av domänobjekt eller automatisk lagring efter parkeringsåtgärder. Ovanstående anteckningar från tidigare under den 10 oktober beskriver läget *innan* detta senare arbete.

## 2026-10-11 – Validering, felsökning och JSON-test

### Genomfört

- Utökat `ValidateParkingData()` med `ArgumentNullException.ThrowIfNull(parkingData)` och kontroller av dokumentets och parkeringsplatsernas listor.
- Använt `HashSet<int>` för att upptäcka dubbla `SpotNumber`.
- Använt `HashSet<string>(StringComparer.OrdinalIgnoreCase)` för att upptäcka dubbla fordonsidentifierare oberoende av stora och små bokstäver.
- Validerat att sparade platser innehåller minst ett fordon och att fordonsobjekt och identifierare inte saknas eller är tomma.
- Begränsat tillåtna sparade fordonstyper i G-versionen till `Car` och `MC`.
- Kontrollerat att `ArrivalTime` inte är standardvärdet och att `ArrivalTime.Kind` är `DateTimeKind.Utc`.
- Anropat `ParkingDataService.Load()` från `Program.cs` efter konfigurationsinläsningen, före skapandet av `ParkingGarage`.
- Förtydligat felmeddelandet i `catch (InvalidDataException ex)` så att både konfiguration och parkeringsdata omfattas.
- Aktiverat `JsonSerializerOptions { WriteIndented = true }` i `Save()` för människoläsbar JSON.
- Använt Visual Studios Breakpoints, Step Into/Step Over, Locals, Watch och Text Visualizer för att förstå filflödet.

### Verifierade manuella tester

| Test | Observation | Resultat |
|---|---|---|
| Positiv `Load()` | `ParkingSpots.Count = 1`; plats 1 hade ett fordon med identifieraren `ABC123` och typen `Car` | Godkänt |
| UTC vid deserialisering | `parkingData.ParkingSpots[0].Vehicles[0].ArrivalTime.Kind` visade `Utc` i Watch | Godkänt |
| Negativ `Load()` | `VehicleType` ändrades till `Truck`; `InvalidDataException` med `Unsupported vehicle type: Truck` fångades i `Program.cs` | Godkänt |
| Indenterad JSON | `JsonSerializer.Serialize(parkingData, jsonOptions)` visade radbruten, indenterad JSON i Text Visualizer | Godkänt för serialiserad text |
| `Save()` vid felsökning | Exekveringen gick igenom skrivningsdelen till `finally`; något separat bekräftat slutresultat för filersättning och städning redovisades inte | Delvis testat |

### Lärdomar

- `HashSet<T>.Add()` returnerar `true` för ett nytt värde och `false` om värdet redan finns; användbart för unikhetskontroller.
- `DateTime` beskriver ett datum/klockslag, `DateTimeKind` dess tidsreferens och `TimeSpan` en tidslängd.
- En breakpoint på metodanropet betyder att raden ännu inte körts; med F11 går det att följa metoden steg för steg.
- `File.WriteAllText()` skapar en fysisk temporärfil; efter lyckad flytt eller ersättning finns den normalt inte kvar.
- `finally` körs också vid fel. Att exekveringen når `finally` räcker därför inte ensamt som bevis för lyckad `Save()`.
- Filåtkomst/strukturell validering hålls åtskild från garage- och kapacitetsregler som kräver aktuell konfiguration.

### Kvarstående arbete och nästa tillfälle

1. Verifiera att `Save()` avslutas utan undantag, att originalfilen verkligen ersätts och att `.tmp` inte ligger kvar; prova även ny-fil-fallet med `File.Move()`.
2. Ta bort den tillfälliga `Save(parkingData)`-testraden i `Program.cs` efter test; behåll `Load()` för fortsatt integration.
3. Komplettera kontroll av utelämnade obligatoriska JSON-fält, identifierarlängd och fel vid filskrivning; skydda befintlig korrupt data innan automatisk sparning införs.
4. Skapa ett tydligt återställningsflöde som validerar mot `AppConfiguration`, återskapar `Car`/`MC` på rätt `SpotNumber` med ursprunglig UTC-tid och aktiverar garaget först när hela återställningen lyckats.
5. Integrera konsekvent sparning med incheckning, utcheckning och förflyttning; testa även misslyckad sparning utan felaktigt framgångsmeddelande.
6. Fortsätta UI-integrationen och därefter G-verifiering, VG-arbete och eventuell distribution enligt projektplanen.

**Teststatus:** `Load()` och dess typvalidering har verifierats med konkreta testdata. `Save()` är delvis manuellt granskat, men fullständigt slutresultat samt omstart och återställning av `ParkingGarage` är ännu inte verifierade.

## 2026-10-12 - Ombyggnad av Garage Overview

### Målsättning
Förbättra parkeringsöversikten genom att skapa en kompakt 10x10 karta där samtliga parkeringsplatser visas samtidigt.

### Genomförande
- Tidigare Grid/Panel-lösning ersattes med Spectre.Console Table.
- Parkeringsplatser visas nu i ett tabellformat liknande en parkeringskarta.
- Layouten separerades i:
  - Garage Overview
  - Spot Details
  - Garage Status

### Designbeslut
- Parkeringsnumret visas endast i kartan.
- Statusinformation visas inte i varje cell för att minska visuellt brus.
- Färg används för status:
  - Grön: ledig
  - Orange: delvis upptagen
  - Röd: full

### Tekniska lärdomar
- Grid och Panel gav för stora komponenter för en 100-platsers översikt.
- Table passar bättre för fasta rader och kolumner.
- UI-komponenter måste väljas efter informationsmängd och skärmyta.

### Nästa steg
- Implementera tydlig markering av vald parkeringsplats.
- Kontrollera om individuell borderfärg kan lösas utan att påverka tabellens kompakthet.
- Uppdatera README och övrig dokumentation.

---

## 2026-10-13 - Fortsatt utveckling av Garage Overview

### Genomfört

- Fortsatt utvecklingen av Garage Overview för att få en mer kompakt parkeringskarta.
- Testat flera olika Spectre.Console-komponenter:
  - Grid
  - Panel
  - Table
  - Columns
  - Align
- Utvärderat skillnaden mellan individuella Panel-komponenter och en tabellbaserad presentation.

### Designbeslut

- Behållit en 10x10-layout för att kunna visa 100 parkeringsplatser samtidigt.
- Lösningen är fortfarande dynamisk och bygger på antalet parkeringsplatser från konfigurationen.
- Parkeringsplatsens nummer visas endast i cellen för att undvika visuellt brus.
- Status presenteras genom cellens bakgrundsfärg:
  - Grön: ledig parkeringsplats
  - Orange: delvis upptagen
  - Röd: full parkeringsplats
- Markerad parkeringsplats visas genom förändrad textfärg istället för extra komponenter runt cellen.

### Tekniska lärdomar

- Spectre.Console Table har begränsningar när individuell borderfärg behöver ändras per cell.
- Individuella Panel-komponenter gav bättre kontroll över styling men ökade storleken på presentationen och skapade mer behov av scrollning.
- Table gav bättre kompakt översikt för många parkeringsplatser.
- Val av UI-komponent måste balansera visuell kontroll mot mängden information som ska visas.

### Problem och lösningar

#### Problem - Markering av vald parkeringsplats

- Individuell borderfärg per cell var inte direkt tillgänglig genom Spectre.Console Table.
- Flera alternativa lösningar testades:
  - extra symboler framför parkeringsnummer
  - separata paneler
  - ändrad bakgrundsfärg
  - ändrad textfärg

### Lösning

- Behöll en enkel markering genom förändrad textfärg på aktiv parkeringsplats.
- Prioriterade kompakt layout och tydlig navigering framför mer avancerad grafisk presentation.

### Nästa steg

- Slutföra verifiering av:
  - ledig plats
  - delvis upptagen plats
  - full plats
- Testa navigering med olika antal parkeringsplatser.
- Fortsätta utveckla SPOT DETAILS.

---

## 2026-10-13 – Senare arbete: fungerande, kompakt Garage Overview

### Genomfört

- Byggt om parkeringskartan från den tillfälliga `Table`-lösningen till ett kompakt `Grid` inuti en gemensam `Panel`. Varje plats behöver inte längre en egen panel eller tabellram.
- Låtit terminalens faktiska bredd och höjd styra antalet kolumner och hur många rader som kan visas. Parkeringshusets storlek hämtas fortfarande från konfigurationen.
- Anpassat huvudlayouten så att `PARKING MAP` får huvuddelen av utrymmet och `SPOT DETAILS` ligger i en smalare panel längst till höger. `GARAGE STATUS` visas nedanför och sträcker sig över båda panelerna.
- Behållit `AnsiConsole.Live()` för uppdatering under tangentbordsnavigering och kopplat navigeringens stegstorlek till samma dynamiska kolumnantal som används för rutnätet.
- Använt statusfärgad bakgrund på parkeringsplatsnumren och en avvikande textfärg för vald plats. Statusfärgerna hämtas från den befintliga tematiken och garagekonfigurationen.
- Lagt in en intern, automatiskt flyttad visningsyta när fler parkeringsrader finns än terminalen kan visa; syftet är att undvika att terminalen själv behöver scrolla.

### Tester och observationer

- Körde garageöversikten med **200 konfigurerade parkeringsplatser** i Windows Terminal.
- Kontrollerade visuellt att **plats 001–200** visas utan avklippta platsnummer i det aktuella terminalfönstret.
- Kontrollerade att `SPOT DETAILS` syns till höger samtidigt som hela kartan, och att `GARAGE STATUS` visas under båda panelerna.
- Kontrollerade visuellt att lediga platser har grön bakgrund och att markerad plats har en avvikande textfärg.
- **Återstår att testa:** navigering till samtliga gränslägen, förändrad terminalstorlek, 100/500/1 000 platser, viewport när alla rader inte ryms samt orange och röd status med parkerade fordon. Detaljpanelen behöver även verifieras med bil, en MC och två MC på samma plats.

### Problem och lösning

De tidigare försöken med en `Table` gjorde att parkeringsnummer radbröts när cellerna blev för smala. När cellbredden ökades blev tabellen i stället för hög, vilket gjorde att delar av parkeringskartan hamnade utanför terminalfönstret. Vid vissa layouter syntes inte heller `SPOT DETAILS`.

**Lösning:** En kompakt `Grid` utan individuella parkeringsplatsramar gör att varje rad tar betydligt mindre plats. Panelernas bredd och antalet kolumner beräknas utifrån terminalstorleken, och detaljpanelen får en tydligt avgränsad del av utrymmet. Den här lösningen ersätter dagens tidigare försök med fast 10 × 10-layout och `Table`; anteckningarna ovan beskriver de mellanliggande experimenten.

### Mina reflektioner

Jag trodde faktiskt att Spectre.Console skulle vara ganska enkelt att arbeta med eftersom jag använde det redan i den första versionen av Prague Parking. Men i V2 ville jag göra en mycket mer avancerad parkeringsöversikt, och då märkte jag att det inte alls var lika enkelt. Jag har lagt ungefär två dagar på att försöka få gränssnittet att se bra ut, visa alla parkeringsplatser som får plats och samtidigt slippa onödig scrollning. Flera lösningar fungerade på ett sätt men skapade nya problem: siffror delades upp på flera rader, delar av kartan försvann eller så fick informationspanelen inte plats.

Det har varit både frustrerande och tidskrävande, särskilt när en ändring som skulle förbättra utseendet i stället gjorde något annat sämre. Jag ville inte nöja mig med en lösning som bara tekniskt fungerade men såg ful och rörig ut. Därför fortsatte jag att testa olika komponenter tills jag fick en översikt som både är mer lättläst och använder terminalens utrymme bättre.

Det viktigaste jag har lärt mig är att man måste förstå hur `Grid`, `Table`, `Panel` och terminalens bredd och höjd samverkar. En konsolapplikation kan också kräva ganska mycket UI-arbete när man vill göra något mer avancerat än en vanlig meny. Jag är nöjd med hur parkeringsöversikten ser ut nu, men jag vill fortfarande testa den ordentligt innan jag räknar den som färdig.

### Nästa steg

- Testa piltangenterna och markeringen även på sista parkeringsraden.
- Kontrollera kompakt layout vid andra terminalstorlekar och större garage.
- Testa statusfärgerna och `SPOT DETAILS` med verkligt parkerade fordon.
- Uppdatera `README.md`, designbeslut, projektplan och kravstatus efter de verifierade ändringarna.
- Kontrollera ändrade filer och Git-historiken innan nästa commit och push till GitHub.

---

## 2026-10-14 – JSON-återställning, automatisk sparning och felsökning

### Genomfört

- Lade `ParkingDataMapper.cs` i huvudprojektets `Application`-mapp. Klassen översätter mellan domänmodellen (`ParkingGarage`, `ParkingSpot`, `Vehicle`, `Car`, `MC`) och DTO-modellerna i `PragueParkingV2.Data.Persistence`.
- Implementerade `ToVehicle()` och `ToVehicleData()` samt `ToDocument()` och `ToGarage()`. `ToDocument()` tar med endast upptagna parkeringsplatser. `ToGarage()` skapar det konfigurerade antalet platser och placerar sparade fordon på deras ursprungliga plats med bevarad UTC-ankomsttid.
- Lade till `ParkVehicleAtSpot()` i `ParkingGarage` för återställning av en bestämd plats; återanvänder befintlig kapacitets- och dubblettkontroll.
- Kopplade `ParkingDataService.Load()` och `ParkingDataMapper.ToGarage()` till programstarten i `Program.cs`.
- Skickade samma `ParkingDataService` genom `Program.cs` → `MainMenu.Show()` → `ParkVehicleView.Show()` → `RegisterVehicle()` så att incheckningsvyn kan spara.
- Kopplade in `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()` efter lyckad `ParkingGarage.ParkVehicle()`. Bekräftelse visas först efter att `Save()` har returnerat utan rapporterat fel.
- Lade in felhantering för `IOException` och `UnauthorizedAccessException`; vid dessa fel försöker programmet ångra parkeringen i minnet med `RemoveVehicle()`. Detta är **grundläggande återställning**, inte ett färdigtestat transaktionsskydd.
- Lät den fungerande `GarageOverView` vara oförändrad.

### Tester och resultat

- **Kompilering:** Visual Studio rapporterade `Build: 1 succeeded, 0 failed, 1 up-to-date`.
- **Inläsning:** Den befintliga testbilen `ABC123` återställdes från JSON och visades på plats **001** med äldre ankomsttid.
- **Ny incheckning:** Registrerade bilen `UKB48K` via `Park Vehicle`. Programmet tilldelade plats **002**, visade korrekt registreringsnummer och lokal ankomsttid **2026-10-14 20:53** samt meddelandet *Vehicle parked successfully!*.
- **Sparning och omstart:** Avslutade programmet helt och startade igen. Bilen `UKB48K` återkom på **plats 002** med samma ankomsttid. `ABC123` fanns kvar på plats 001.
- **Garageöversikt:** `Garage Status` visade **FREE 198**, **OCCUPIED 2** och **VEHICLES 2** av 200 konfigurerade platser. Båda upptagna bilplatserna visades röda (FULL).
- Testet verifierar en **lyckad normal incheckning, fysisk JSON-sparning och återställning efter omstart**. Det verifierar **inte** simulerade skrivfel, korrupt fil, två MC på samma plats efter omstart eller sparning efter utcheckning/flytt.

### Problem och lösningar

**1. `Application` hittades inte.** Först låg `ParkingDataMapper.cs` på solution-nivå/i en *Solution Folder* i stället för som kompilerad fil i huvudprojektet. Jag skapade en riktig `Application`-mapp under C#-projektet `PragueParkingV2` och lade in filen där.

**2. Ändrad metodsignatur gav kompileringsfel.** Jag lade till `ParkingDataService` som parameter i `MainMenu.Show()` men missade först att skicka den från `Program.cs`. Visual Studios felmeddelande visade att det tredje argumentet saknades. Efter ändringen byggdes projektet utan fel.

**3. Felsökaren stannade vid `File.WriteAllText()` i `ParkingDataService.Save()`.** Jag misstänkte först ett fel i lagringen. Efter fortsatt körning kom dock en lyckad parkeringsbekräftelse och det efterföljande omstartstestet visade att datan bevarats. Den exakta orsaken till att felsökaren stannade är inte slutligt fastställd; jag ska skilja på brytpunkter och undantag nästa gång.

### Mina reflektioner

Idag var det ganska körigt att hålla reda på alla filer, metoder och hur de hänger ihop. Jag märkte att när man ändrar något i en fil kan det påverka flera andra filer, vilket inte alltid är lätt att komma ihåg.

Jag lärde mig också att använda Shift + F12 i Visual Studio för att hitta var en metod används. Det är en genväg jag ska försöka komma ihåg eftersom den kan underlätta arbetet.

Jag försökte även tillämpa lite enklare LINQ i koden. Det fungerade bra på vissa ställen, men när jag försökte ersätta två nästlade foreach-loopar blev det alldeles för bökigt. Därför valde jag att behålla den mer traditionella lösningen med foreach, eftersom den var enklare att förstå.

Mot slutet var jag ganska trött och gjorde några onödiga misstag. Trots det lyckades vi få JSON-sparningen att fungera, och jag kunde testa att parkera ett fordon, avsluta programmet och starta om det. Fordonet fanns fortfarande kvar med rätt parkeringsplats och ankomsttid.
### Nästa steg

1. Implementera **Check Out Vehicle** i UI med prisinformation och säker JSON-uppdatering när fordonet lämnar garaget.
2. Implementera **Move Vehicle** med bibehållen ankomsttid och sparning; därefter **Find Vehicle**.
3. Implementera administrationsmenyns prisomläsning för G-versionen.
4. Testa skrivfel, ogiltig/korrupt befintlig JSON, saknade fält, dubbletter, återställning av två MC på samma plats samt garagekapacitet som inte matchar sparade platsnummer.
5. Fortsätta testa terminalstorlekar, navigation och konfigurerade garageantal (standard **100**, aktuellt test **200**).
6. Kontrollera `git status` och säkerställ att `bin/`, `obj/` och runtime-`parking-data.json` inte av misstag inkluderas i commit.

---

## 2026-10-15 – Check Out, Move Vehicle och förbättrad felhantering

### Genomfört

#### Check Out Vehicle

- Kopplat `CheckOutVehicleView` till huvudmenyn och den befintliga domänlogiken i `CheckoutService` och `ParkingFeeCalculator`.
- Visar registreringsnummer, parkeringstid, beräknad avgift och en bekräftelse innan utcheckningen genomförs.
- Använder `ParkingDataMapper.ToDocument()` och `ParkingDataService.Save()` för att spara att fordonet har lämnat garaget.
- Lagt in ett försök att återställa fordonet på dess ursprungliga plats om JSON-sparningen misslyckas.
- Anpassat vyn till centrerade Spectre.Console-paneler och en grön kvittopanel efter lyckad utcheckning.

#### Move Vehicle – domänlogik och användargränssnitt

- Implementerat `ParkingGarage.MoveVehicle()` genom att återanvända `FindVehicle()`, `ParkingSpot.CanPark()`, `RemoveVehicle()` och `ParkVehicle()`.
- Kontrollerar att fordonet och destinationsplatsen finns, att platserna inte är samma och att fordonstyp och kapacitet tillåter flytten.
- Återanvänder samma `Vehicle`-objekt vid flytten för att behålla den ursprungliga `ArrivalTime`.
- Lagt till ett återställningsförsök om parkeringen på den nya platsen misslyckas.
- Skapat `MoveVehicleView.cs` och anslutit vyn till `MainMenu`.
- Byggt centrerade paneler för fordonsinformation, val av ny plats, bekräftelse och grönt kvitto med `AppTheme`.
- Använt `int.TryParse()` för att kontrollera platsnumret och hämtar kapacitet för `Car` respektive `MC` ur `config.json` via `AppConfiguration`.
- Infört en `while`-loop som låter användaren välja en ny plats direkt efter ogiltig inmatning. Tomt inmatningsfält och Enter avbryter; direkt avbrytning med ESC i textfältet är ännu inte implementerad.
- Kontrollerar destinationsplatsens kapacitet redan före bekräftelsen, men behåller samma kontroll i domänlogiken.
- Kopplat flytten till JSON-sparning med försök till återflytt till ursprungsplatsen om sparningen rapporterar ett filfel.

#### Gemensam presentation och felmeddelanden

- Uppdaterat `MessageService` så att fel- och informationspaneler centreras genom `AppTheme.WriteCenteredPanel()`.
- Förbättrat `MoveVehicleView` så att felaktiga platsförsök inte skapar en växande stapel av felpaneler. Med `Console.CursorTop`, `Console.SetCursorPosition()` och ANSI-rensning (`\x1b[0J`) ritas endast inmatningsområdet om.
- Behållit `GarageOverView` som separat, terminalanpassad helbreddsvy; övriga formulär använder konsekventa centrerade paneler.

### Tester och resultat

- **Kompilering:** Flera byggningar av lösningen genomfördes utan kompileringsfel under arbetet.
- **Utcheckning och omstart:** Testfordonet `UKB48K` checkades ut via UI med kvitto och avgift. Efter omstart var dess tidigare plats ledig medan övriga parkerade fordon fanns kvar. Därmed verifierades normalflödet för sparad utcheckning.
- **Flytt av bil:** `ABC123` flyttades från plats **001** till plats **050**. `Garage Overview` visade därefter plats 001 som ledig och plats 050 som upptagen.
- **Flytt och omstart:** Efter att programmet avslutats och startats igen fanns bilen kvar på den nya platsen. Normalflödet för JSON-sparning och återställning efter flytt är därmed verifierat.
- **Felaktig destinationsplats:** En bil nekades flytt till en plats som inte kunde ta emot den. Kontroll sker nu redan när destinationsplatsen anges, utan att användaren måste bekräfta en flytt som ändå inte kan genomföras.
- **Inmatning:** Ogiltiga värden, platsnummer utanför garaget och val av samma plats ger ett felmeddelande och möjlighet att ange ett nytt platsnummer utan att registreringsnumret behöver anges på nytt.
- **Motorcyklar:** Verifierat att två MC kan dela plats **003** när kapaciteten är två och att en MC inte får flyttas till en plats som innehåller en bil. `Garage Overview` visade två MC på samma plats, en bil på en annan plats samt status **FREE 198 / OCCUPIED 2 / VEHICLES 3** vid 200 platser.

### Problem och lösningar

**1. Svårt att hitta rätt metod och klass.** Under arbetet med `MoveVehicle()` blandade jag till en början ihop `List<T>.Find()`, LINQ:s `FirstOrDefault()` och den befintliga `ParkingSpot.CanPark()`. Jag fick reda ut vilken klass som ansvarar för respektive metod och hur ett `ParkingSpot`-objekt används från `ParkingGarage`.

**2. Risk att fordon tas bort för tidigt.** Den första versionen tog bort fordonet innan destinationsplatsen hade validerats. Jag ändrade ordningen så att samtliga kontroller sker före borttagningen och lade till försök att återställa fordonet vid fel.

**3. Felaktigt platsnummer skickade tillbaka användaren till huvudmenyn.** Jag ersatte direkt återgång med en `while`-loop och `continue`, så att användaren kan försöka igen. Jag arbetade också med skillnaden mellan `continue`, `break` och `return`.

**4. Felpanelerna hamnade fel och staplades på varandra.** Jag återanvände `AppTheme.WriteCenteredPanel()` i `MessageService` och begränsade omritningen till området med platsinmatning och senaste felmeddelandet. Layouten blev tydligare utan att övriga paneler behövde ritas om.

### Mina reflektioner

Jag känner att jag gör stora framsteg nu när jag börjar implementera och använda metoder och funktioner som jag har byggt tidigare. Programmet börjar verkligen ta form, och jag märker hur de olika delarna fungerar tillsammans.

Samtidigt märker jag att jag ibland lägger för mycket tid på det visuella gränssnittet och lätt tappar tråden när jag växlar mellan olika klasser och funktioner. Det är något jag behöver bli bättre på att hantera.

Trots det är jag väldigt nöjd med det jag har åstadkommit hittills. Det känns motiverande att se hur projektet utvecklas och att funktionerna jag byggt tidigare nu faktiskt används i programmet.

### Nästa steg

1. Kontrollera efter omstart att **båda motorcyklarna** på en delad plats finns kvar med rätt identifierare och ursprunglig ankomsttid.
2. Testa och verifiera återställningsförsöken vid **simulerade JSON-sparfel**, både för utcheckning och flytt. Den normala sparningen fungerar, men felscenarierna är ännu inte verifierade och innebär ingen fullständig transaktionsgaranti.
3. Implementera och testa **Find Vehicle** i UI.
4. Implementera **Administration** med omläsning och validering av prislistan för G-versionen.
5. Fortsätta tester av felaktig JSON, konfigurationsgränser och terminalbeteende samt uppdatera övrig dokumentation.
6. Granska Git-ändringarna innan commit; säkerställ att `bin/`, `obj/` och runtime-`parking-data.json` inte inkluderas av misstag.

---

## 2026-10-16 – Sluttester av G-versionen och förbättrad felhantering

### Genomfört

#### Find Vehicle och Administration

- Färdigställt och manuellt testat `FindVehicleView` via huvudmenyn. Sökningen är skiftlägesokänslig och visar fordonstyp, identifierare, parkeringsplats, lokal ankomsttid och parkeringstid utan att förändra garagets innehåll.
- Sökt efter befintlig bil samt `def123`, som hittades som `DEF123` på plats 003. En sökning efter en identifierare som inte finns visade ett felmeddelande och möjlighet att försöka igen.
- Färdigställt `AdministrationView` med **Reload Pricing**. `ConfigurationService.LoadPricing()` läser priserna från runtime-`Config/config.json`, varefter samma `ParkingFeeCalculator` uppdateras genom `UpdatePricing()` utan programomstart.
- För G-versionen läses endast **prislistan** om. Omläsning av hela konfigurationen hör till VG.

#### Robustare återställning av parkeringsdata

- Kompletterat `ParkingDataMapper.ToGarage()` med kontroll att sparade `SpotNumber` ligger inom den konfigurerade garagekapaciteten.
- Förtydligat felmeddelanden när ett fordon inte kan återställas på sin sparade plats på grund av kapacitet eller otillåten fordonsblandning. Felmeddelandet identifierar fordonet och parkeringsplatsen.
- Förtydligat felmeddelandet vid ogiltig JSON under programstart. Befintliga felaktiga parkeringsfiler ska inte automatiskt skrivas över.

#### MessageService – samma marginal för panel och fortsättningstext

- Upptäckte att den centrerade `ERROR`-panelen följdes av `Press any key to continue...` längst till vänster i terminalen.
- Ändrade `MessageService.ShowPanel()` så att vänstermarginalen för texten beräknas med terminalbredden och en panelbredd på 62 tecken.
- Kontrollerade resultatet visuellt. Felpanelen och fortsättningstexten ligger nu i linje med varandra. Ändringen påverkar gemensamma `MessageService`-meddelanden där `waitForKey: true` används.

### Tester och resultat

| Test | Observation | Resultat |
|---|---|---|
| Find Vehicle – Car och MC | Befintliga fordon hittas med rätt plats och tidsinformation, även med små bokstäver (`def123` → `DEF123`) | Godkänt |
| Find Vehicle – saknat fordon | Felmeddelande och möjlighet till nytt försök visas | Godkänt |
| Reload Pricing – giltig ändring | Car-priset ändrades från 20 till 30 CZK utan omstart | Godkänt |
| Checkout med nytt pris | 20 h 20 min gav 21 påbörjade timmar × 30 CZK = 630 CZK | Godkänt |
| Reload Pricing – ogiltigt värde | Car-pris −10 CZK avvisades; det tidigare giltiga priset 30 CZK behölls | Godkänt |
| Återställda priser | Car-priset återställdes till 20 CZK; MC-pris 10 CZK och gratisgräns 10 minuter | Godkänt |
| Två MC efter omstart | `DEF123` och `EDF123` fanns kvar tillsammans på plats 003 med sina ursprungliga ankomsttider | Godkänt |
| Korrupt JSON | Syntaktiskt trasig `parking-data.json` stoppade uppstarten kontrollerat; originalfilen behölls | Godkänt |
| Dubblett i JSON | `ABC123` och `abc123` i samma garagetillstånd avvisades | Godkänt |
| Plats utanför konfigurationen | Plats 201 i ett garage konfigurerat för 200 platser avvisades med tydligt fel | Godkänt |
| För många MC på en plats | Tre MC på plats 003 avvisades när kapaciteten var två | Godkänt |
| Bil och MC på samma plats | Ogiltig kombination av Car och MC avvisades vid återställning | Godkänt |
| Sparfel – Park Vehicle | Försök att parkera `ROLL123` gav sparfel; fordonet fanns därefter inte kvar i minnet | Godkänt |
| Sparfel – Check Out | Försök att checka ut `ABC123` gav sparfel; bilen stod kvar på plats 050 | Godkänt |
| Sparfel – Move Vehicle | Flytt av `ABC123` från 050 till 051 gav sparfel; bilen återställdes till 050 | Godkänt |
| Normal sparning efter feltester | Testhindret togs bort; flytt av `ABC123` från 050 till 051 sparades och fanns kvar efter omstart | Godkänt |
| Garage Overview – 1 000 platser | Intern viewport fungerade och status visade 998 lediga platser, två upptagna platser och tre fordon | Godkänt |
| Garage Overview – navigering | Markerade plats 1000, kontrollerade att navigationen inte gick bortom sista platsen och navigerade tillbaka till början | Godkänt för genomförda steg |
| Återgång till 100 platser | Runtime-konfigurationen ändrades till standardkapaciteten 100; fordonen kvar på 003 och 051 | Godkänt: 98 lediga, 2 upptagna, 3 fordon |
| Dubblett via Park Vehicle | Ny registrering av `abc123` avvisades när `ABC123` redan var parkerad | Godkänt |
| Centrerad fortsättningstext | `Press any key to continue...` ligger i linje med `ERROR`-panelen | Godkänt |

**Så framkallades skrivfel:** Under testerna skapades tillfälligt en **katalog** med namnet `parking-data.json.tmp` i körmiljöns `Data`-katalog. När `ParkingDataService.Save()` försökte skriva temporärfilen misslyckades operationen. Efter testerna togs katalogen bort och en lyckad flytt/sparning genomfördes igen. Testet visar återställning vid detta specifika skrivfel; det bevisar inte atomisk transaktionssäkerhet vid strömavbrott eller samtliga filsystemfel.

**Senaste verifierade garagetillstånd:** `DEF123` och `EDF123` på plats **003**, `ABC123` på plats **051**, garagekapacitet **100**. Efter återgång från testkapaciteten 1 000 visade `GARAGE STATUS` **FREE 98 / OCCUPIED 2 / VEHICLES 3**.

### Problem och lösningar

**1. Otillräckligt detaljerade fel vid felaktig JSON-återställning.** Tester med platsnummer utanför garagekapaciteten och otillåten beläggning gav inledningsvis allmänna felmeddelanden. Jag kompletterade `ParkingDataMapper` med tydligare validering och felkontext och körde om de negativa testerna.

**2. Osäkerhet kring återställning vid sparfel.** Den normala JSON-sparningen hade fungerat, men det räckte inte för att veta vad som händer när filskrivningen misslyckas. Genom att avsiktligt blockera temporärfilen kunde jag testa att Park, Check Out och Move visar fel och kompenserar sina ändringar i minnet. Därefter verifierade jag att en normal sparning åter fungerar.

**3. Felplacerad uppmaning i gränssnittet.** `ERROR`-panelen var centrerad, men fortsättningstexten började vid vänsterkanten. En beräknad marginal i `MessageService` löste problemet utan att behöva ändra varje enskild UI-vy.

### Krav och dokumentation

- Påbörjat slutlig avstämning mellan lärarens uppgiftsbeskrivning och projektets dokumentation. Flera avsnitt från 15 oktober beskrev fortfarande `Find Vehicle`, `Administration`, felinjektion och viewport-test som återstående arbete trots dagens verifieringar.
- Gått igenom `requirements.md`, `project-plan.md`, `parking-data.md`, `design-decisions.md`, `configuration.md` och `class-diagram.md`; dokumenten uppdateras och levereras separat, en fil i taget.
- Läraren har enligt mitt förtydligande godkänt att använda **de första 50 parkeringsplatserna som bussområde** i VG i stället för att modellera takhöjd. Bussar kräver fortfarande **fyra sammanhängande platser**. Den närmare regeln för flytt utanför området måste vara konsekvent i krav och implementation.
- Uppmärksammat VG-kravet att skapa och fylla en saknad parkeringsdatafil med **testfordon**.
- Bestämt att `reflektion.md` sammanställs **sist, efter att både G och VG är färdiga**, med loggbokens egna reflektioner som underlag.

### Nästa steg

1. Slutföra kravavstämning och synkronisering av `README.md`, `requirements.md` och `project-plan.md`.
2. Vid behov testa återstående avbrytnings-/terminalgränsfall och programmets start i en annan körmiljö. Sådana tester ska inte anges som genomförda innan de faktiskt körts.
3. Därefter börja med **Prague Parking 2.1 (VG)** som ett separat körbart projekt i **samma solution och Git-repository**: interfaces, Bicycle, Bus, storleksbaserad kapacitet, konfigurationsomläsning och testdatagenerering.
4. Vänta med Git-commit tills kod och tillhörande dokumentation är genomgångna. Kontrollera `git status` så att `bin/`, `obj/` och runtime-data inte följer med.

---

## 2026-10-20 – Prague Parking V2.1: uppstart, interfaces och storleksbaserad kapacitet

### Genomfört

#### Separat VG-version i samma solution

- Skapat `PragueParkingV2_1` (Console App) och `PragueParkingV2_1.Data` (Class Library) i samma solution som den färdiga G-versionen. Båda projekten använder .NET 10.
- Kopierat relevant källkod från V2.0 till de nya projekten utan att kopiera gamla projektfiler eller byggmappar.
- Installerat `Spectre.Console` 0.57.2 och verifierat projektreferensen från `PragueParkingV2_1` till det nya databiblioteket.
- Bytt namnrymder från `PragueParkingV2` till `PragueParkingV2_1` i VG-projekten. Visual Studio rapporterade 48 ersättningar i huvudprojektet och 6 i databiblioteket.
- Byggt hela solutionen utan kompileringsfel (4 projekt lyckades). Startat VG-kopian och kontrollerat att huvudmenyn visas. Denna kontroll gjordes innan dagens fortsatta ändringar av parkeringslogiken.
- Uppdaterat versionsnamnet till **V2.1** i `Program.cs`, `MainMenu` och `Administration`, så att det går att skilja programversionerna åt.
- Återställt standardantalet parkeringsplatser till **100** i konfigurationen. Även G-versionens projektkonfiguration ändrades tillbaka från testvärdet 200 till 100.

#### Fordon, interfaces och konfiguration

- Skapat `IVehicle` med `Identifier` och `ArrivalTime`; den abstrakta basklassen `Vehicle` implementerar nu interfacet.
- Skapat `Bicycle` och `Bus`, med två konstruktorer vardera: en för ny registrering och en för återställning med tidigare UTC-ankomsttid.
- Påbörjat övergången från `VehiclesPerSpot` till storleksbaserad kapacitet. `VehicleTypeConfiguration` har fått `Size` och `GarageConfiguration` har fått `SpotCapacity`.
- Uppdaterat `config.json` för fyra fordonstyper: Bicycle (1), MC (2), Car (4) och Bus (16) storleksenheter, med 4 enheter per parkeringsruta. Konfigurationen omfattar också planerade VG-priser och bussområdet 1–50.
- Uppdaterat `ConfigurationService` med en `foreach` som validerar att konfigurerade fordonstyper har namn och en positiv storlek. Arbetet med standardkonfiguration, övrig validering och omladdning under drift är ännu inte slutfört.

#### Parkeringslogik och JSON-mappning

- Lagt till `GetUsedCapacity()` i `ParkingSpot` för att summera storlekarna hos parkerade fordon.
- Lagt till överlagrade versioner av `CanPark()` och `ParkVehicle()` som använder `spotCapacity` och en `Func<IVehicle, int>` för att hämta storleken från konfigurationen.
- Lagt till överlagrade `FindAvailableSpot()`, `ParkVehicle()` och `ParkVehicleAtSpot()` i `ParkingGarage` med den nya kapacitetsmodellen.
- Ändrat `ParkVehicleView` så att rutkapaciteten hämtas från `Garage.SpotCapacity` och fordonsstorleken från `VehicleTypes[...].Size` genom en lambda/delegat.
- Utökat `ParkingDataMapper.ToVehicle()` och `ToVehicleData()` så att `Bicycle` och `Bus` kan översättas mellan fordonsobjekt och JSON-datamodell.
- Låtit de äldre metodvarianterna finnas kvar tillfälligt medan anropen migreras. Den gamla `VehiclesPerSpot`-logiken används fortfarande på andra ställen och ska inte tas bort genom en blind sök-och-ersätt-operation.

### Kontroller och aktuell status

- Visual Studio har rapporterat lyckade byggen efter de genomförda delstegen. De nya metoderna har alltså kompilerat, men det är **inte** samma sak som att hela VG-flödet är funktionstestat.
- Det finns ännu inget verifierat end-to-end-test av blandade fordonsstorlekar, bussparkering, flytt/utcheckning av buss eller återställning av VG-data efter omstart.
- `ParkingDataMapper.ToGarage()`, kvarvarande `VehiclesPerSpot`-anrop, JSON-validering, prisberäkning/UI och reservationer för buss behöver fortfarande anpassas och testas.
- Ingen ny Git-commit eller push har gjorts för dagens VG-arbete. `PragueParkingV2.slnx` och G-versionens `Config/config.json` är ändrade; de två VG-projektmapparna är ännu inte incheckade i Git enligt dagens `git status --short`.

### Pågående problemlösning – bussparkering

Den svåraste delen hittills är att hitta en hållbar lösning för bussen. Jag försökte tänka igenom problemet steg för steg innan jag börjar skriva kod. Min nuvarande strategi för nyparkering är:

1. En buss anländer.
2. Kontrollera att bussen inte redan är registrerad.
3. Börja undersöka parkeringsplats 1.
4. Kontrollera om platserna 1, 2, 3 och 4 är helt lediga.
5. Om alla fyra är lediga: reservera platserna, registrera bussen som **ett** fordon och avsluta sökningen.
6. Annars flyttas startpunkten till plats 2 och platserna 2, 3, 4 och 5 kontrolleras.
7. Fortsätt tills fyra sammanhängande lediga platser hittas, men sök inte utanför bussområdet **1–50**.
8. Om ingen giltig grupp hittas: meddela att bussen inte kan parkeras.

Det återstår att bestämma hur de fyra platserna ska markeras som upptagna utan att samma buss sparas som fyra olika fordon. Sökning, JSON-lagring, flytt, utcheckning och frigöring av samtliga platser måste fungera tillsammans. Flyttreglerna behöver också kontrolleras mot uppgiftsbeskrivningen innan designen låses. **Detta är en planerad algoritm, inte en färdig eller testad implementation.**

### Problem under dagen – Visual Studio 2026

- Visual Studio kraschade efter att jag hade laddat ner flera teman. Jag vet inte säkert om temana orsakade kraschen, men efter återställningen saknades många av mina tidigare inställningar och tillägg. Jag fick felsöka och konfigurera om mycket manuellt.
- Kodformateringen har fortsatt att krångla. Jag vill ha kompakta properties som `string Identifier { get; }`, men tydligt uppdelade metodblock. Inställningarna för `Wrapping` gav inte den automatiska formatering jag förväntade mig.
- Jag undersökte också tangentbordsinställningarna och upptäckte att `Edit.FormatDocument` var kopplat till `Ctrl+E, D` i profilen **Visual C# 2005**, inte till `Ctrl+K, Ctrl+D` som jag hade försökt använda.
- Jag väljer tills vidare att inte installera fler teman. Jag vill prioritera att utvecklingsmiljön är stabil och lägga tiden på programmeringen.

### Mina personliga reflektioner

Idag har jag lärt mig mer om `Func`. Vi gick igenom `delegate` lite under lektion 14–15, men här fick jag se hur man faktiskt kan använda en delegat i ett projekt. Med `Func<IVehicle, int>` kan jag skicka in en funktion som tar emot ett fordon och returnerar dess storlek, utan att `ParkingSpot` behöver veta något om själva JSON-filen. Jag vill förstå detta bättre när vi fortsätter.

Jag upptäckte också att **inte bara konstruktorer, utan även vanliga metoder kan överlagras**. Det var nytt för mig att se samma metodnamn användas med olika parameterlistor. Jag funderade även på returtyper som `bool` och `int`. Där behöver jag hålla isär begreppen: det är **antalet, typerna eller ordningen på parametrarna** som skiljer överlagringar åt. Det går inte att överlagra en metod *enbart* genom att byta returtyp, men returtyperna behöver inte vara lika om parameterlistorna skiljer sig.

Bussproblemet är det jag tycker är svårast just nu. Jag kan förstå hur vi letar efter fyra sammanhängande platser, men det är inte lika självklart hur vi ska koppla ihop reservationerna med ett enda bussobjekt och samtidigt få flytt, utcheckning och sparning att fungera. Jag får sova lite på saken och fortsätta resonera innan jag bestämmer hur lösningen ska se ut i kod.

Det har också varit frustrerande att behöva lägga så mycket tid på Visual Studio i stället för själva uppgiften. Efter kraschen och återställningen fick jag konfigurera om flera saker, och formateringen fungerar fortfarande inte riktigt som jag vill. Jag vill inte fastna i det igen, utan hellre fokusera på att förstå och färdigställa VG-funktionerna.

### Nästa steg

1. Fortsätta med `ParkingDataMapper.ToGarage()` och anpassa återställning till `SpotCapacity` och konfigurerade fordonsstorlekar.
2. Migrera kvarvarande `VehiclesPerSpot`-beroenden i **endast V2.1**, och därefter ta bort överflödiga gamla metodvarianter.
3. Slutföra standardkonfiguration, priser och validering för de fyra fordonstyperna samt anpassa återstående UI- och checkout-flöden.
4. Designa och implementera bussreservationer med fyra sammanhängande platser, följt av flytt, utcheckning och JSON-persistens.
5. Verifiera varje färdigt VG-flöde med funktionstester och omstarter. Dokumentera endast faktiskt genomförda tester som godkända.
6. Uppdatera den centrala dokumentationen en fil i taget, granska Git-ändringarna och skapa en korrekt commit innan arbetet fortsätter för långt.

**Avgränsning:** Tidigare idéer om frivillig Windows-betadistribution, Docker/Synology NAS, Cloudflare och Discord-feedback prioriteras inte längre. Projektet fokuserar nu på kurskrav och nödvändiga tester. Tidigare loggboksanteckningar om idéerna behålls som historik.

---

## 2026-10-21 – V2.1: konfigurationsmall, fordonsflytt, Bicycle och bussreservationer

### Vad jag gjorde

- Jag fortsatte utvecklingen på `feature/prague-parking-v2.1` utan att skriva om den färdigtestade G-versionen. GitHub visade att den tidigare lokala commiten `273d2c73` redan fanns på utvecklingsbranchen, trots att Visual Studio först hade rapporterat ett fel vid push. De **nya ändringarna från i dag är ännu inte redovisade som committade**.
- Jag flyttade V2.1:s JSON-återställning (`ParkingDataMapper.ToGarage()`), utcheckningens rollback och fordonsflytten (`MoveVehicleView` samt `ParkingGarage.MoveVehicle`) till kapacitetsmodellen med `SpotCapacity`, `Size` och `Func<IVehicle, int>`. Jag byggde lösningen efter ändringarna utan kompileringsfel.
- Jag upptäckte ett startfel: `Vehicle type 'Car' must have a size greater than zero`. Orsaken var att runtime-`Config/config.json` fortfarande innehöll G-versionens `VehiclesPerSpot`, medan projektets V2.1-konfiguration hade `Size`. Jag verifierade skillnaden i PowerShell.
- Jag ändrade `ConfigurationService` så att en saknad aktiv `Config/config.json` skapas genom kopiering från `Config/config.template.json`. Befintlig aktiv fil lämnas orörd och valideras. Jag tog bort den tidigare hårdkodade `CreateDefaultConfiguration()` för att undvika dubbla källor för standardvärden.
- Jag kompletterade konfigurationsmodellen och valideringen med `SpotCapacity`, `BusMaxParkingSpot`, `BicycleHourlyPrice`, `BusHourlyPrice`, krav på fyra fordonstyper, positiva fordonsstorlekar och tillåtna garagegränser. Standardmallen anger 100 platser, kapacitet 4, bussgräns 50, storlekar Bicycle/MC/Car/Bus = 1/2/4/16 samt priser 5/10/20/80 CZK.
- Jag döpte om projektets V2.1-konfiguration till `config.template.json` och konfigurerade kopiering av **enbart mallen** i `PragueParkingV2_1.csproj` med `CopyToOutputDirectory=PreserveNewest`. PowerShell bekräftade `Test-Path .../Config/config.template.json` med `True`. Jag döpte om den gamla runtime-konfigurationen till `config.backup-before-v2.1.json`, utan att röra `parking-data.json`. Programmet startade därefter utan konfigurationsfel.
- Jag lade till `Bicycle` i `ParkVehicleView` och `ParkingDataService.ValidateParkingData()`; `ParkingDataMapper` hade redan stöd för serialisering och återställning av Bicycle. Registrering av den första cykeln lyckades, men test med fyra cyklar tillsammans och kontroll efter omstart återstår.
- Jag skapade `IParkingSpot` och lät `ParkingSpot` implementera interfacet. Jag lade till `ReservedByBus`, `ReserveForBus()`, `ReleaseBusReservation()` och `ParkBusAtPrimarySpot()` samt kontroller som blockerar vanlig parkering på bussreserverade platser och förhindrar att Bus använder vanlig enplatsparkering.
- Jag implementerade i `ParkingGarage` sökning efter första gruppen sammanhängande lediga bussplatser (`FindAvailableBusSpots`) och kontroll av en **bestämd huvudplats** (`FindAvailableBusSpotsAt`), samt `ParkBus()` och `ParkBusAtSpot()` med reservationer och kompensation om registreringen misslyckas.
- Jag ändrade `ParkingGarage.RemoveVehicle()` så att borttagning av en buss kontrollerar och frigör dess fyra reservationer. `ParkingDataMapper.ToGarage()` anropar nu `ParkBusAtSpot()` för Bus, och `ParkingDataService` accepterar Bus som fordonstyp. I `CheckOutVehicleView` har jag lagt till särskild rollback som återställer bussens ursprungliga grupp om sparning misslyckas. Koden bygger, men **bussens hela flöde är ännu inte funktionstestat**.

### Manuella tester – faktiskt utförda i V2.1

| Test | Utfall | Status |
|---|---|---|
| Byggning efter successiva kodändringar | Visual Studio visade grön build | Godkänt som kompilering, inte komplett funktionstest |
| V2.1-start efter ny konfigurationsmall | Programmet startade efter att gammal runtime-konfiguration säkerhetskopierats | Godkänt |
| Flytta `UKB48K` från 001 till redan upptagen 002 | Flytten nekades med begripligt felmeddelande | Godkänt |
| Flytta `UKB48K` från 001 till ledig 003 | Flytten lyckades | Godkänt |
| Omstart och `Find Vehicle` för `UKB48K` | Bilen hittades på plats **003** efter omstart | Godkänt: placering i JSON och återställning |
| Registrera första Bicycle | Programmet visade lyckad registrering | Godkänt för nyregistreringen; inte fler-cykel-/omstartstest |
| Öppna Garage Overview med en Bicycle parkerad | Programmet gav ett oväntat fel och avslutades | **Fel reproducerat; ej åtgärdat** |
| Buss: fyra reserverade platser, borttagning och rollback | Metoderna kompilerar, men inga körtester redovisade | Ej verifierat |

### Problem, felsökning och lärdomar

- Startfelet berodde inte på själva `Size`-värdet i projektfilen utan på skillnaden mellan **projektmall** och **runtime-konfiguration**. Jag lärde mig att kontrollera den faktiska fil som programmet läser under `AppContext.BaseDirectory`.
- Jag råkade först placera en ny `RemoveVehicle()` i `ParkingSpot.cs` i stället för `ParkingGarage.cs`, vilket gav fel på `_parkingSpots`. Efter att jag skiljde på klassernas ansvar rättade jag placeringen och lösningen byggde igen.
- Jag resonerade själv kring att lagra bussen en gång på huvudplatsen och låta de övriga tre platserna referera till **samma bussobjekt**. Det undviker fyra kopior av samma registreringsnummer och ger en tydlig modell för borttagning och återställning.
- Garage Overview använder fortfarande G-versionens `VehiclesPerSpot` och en `GetVehicleType()` som inte hanterar `Bicycle`. Jag valde att vänta med större ombyggnad av den cirka 500 rader långa vyn tills bussens domänmodell är färdig, så att UI-anpassningen kan göras samlat. Tills dess är Garage Overview **inte säker att öppna** med nya fordonstyper.
- Jag skiljer nu tydligare på **grön byggning**, **manuellt verifierat flöde** och **ännu inte testad funktionalitet**. Jag ska inte beskriva bussparkeringen som färdig bara för att metoderna kompilerar.

### Nästa steg

1. Granska och utöka `ParkingFeeCalculator` samt relaterade resultat-/utcheckningsflöden för Bicycle (5 CZK/h) och Bus (80 CZK/h).
2. Granska `MoveVehicle` så att buss inte kan flyttas med den vanliga enplatsmetoden; besluta hur särskild bussflytt ska fungera inom intervallet 001–050 och implementera säker rollback.
3. Funktionstesta bussparkering, dubblettskydd, fyra på varandra följande platser, borttagning, JSON-omstart och sparfel/rollback. Kontrollera att bussen lagras **en gång** och att reservationerna återskapas.
4. Färdigställa `GarageOverView` för storleksbaserad beläggning, Bicycle, blandad beläggning och bussens fyra reserverade platser; åtgärda den observerade kraschen.
5. Testa fyra cyklar på samma plats, cykel efter omstart, kombinationerna MC+Bicycle och full plats; implementera fullständig config reload och automatisk testdata vid saknad `parking-data.json`.
6. Granska kvarvarande äldre `VehiclesPerSpot`-anrop i V2.1, uppdatera dokumentation och kör G-regressionstest i relevant omfattning. Senare: Git commit/push på feature-branchen och test på annan dator.


## 2026-10-21 – V2.1: avslutande verifieringar, konfiguration och testdata

### Genomfört och testat

- Bussens fyra sammanhängande reservationer färdigställdes och testades genom registrering, överlappande flytt och normal utcheckning. `BUS001` flyttades från 004–007 till 006–009 utan att ankomsttiden ändrades. Utcheckningen beräknade 160 CZK för 1 timme och 38 minuter och frigjorde bussens fyra platser.
- Garage Overview uppdaterades för `Bus`/`Bicycle`, storleksbaserad beläggning, fyra bussreservationer och statusräknare. Det tidigare rapporterade felet vid visning av Bicycle åtgärdades. Ett nytt varmt **Sunset-tema** ersatte färgsättningen i V2.1; G-temat lämnades som tidigare.
- `FindVehicleView` kompletterades så att `Bus` och `Bicycle` visas med rätt typ och bussens fyra platsnummer. ESC-inmatning kodades för Find Vehicle, och vyn byggdes framgångsrikt; enhetlig ESC-hantering i övriga formulär kvarstår att se över.
- `Administration` fick `Reload Configuration` med en `Action` från MainMenu. `ConfigurationService.LoadConfiguration(createIfMissing: false)` används vid manuell reload. Ny konfiguration testas först genom att befintliga parkeringar återskapas i ett tillfälligt garage via `ParkingDataMapper`; först vid lyckad kontroll aktiveras nytt garage, nya inställningar och priser.
- Funktionstest: utökning från **100 till 200** platser utan omstart med bevarade fordon. Även **500 platser** visades och navigerades med intern viewport. Negativt test: `SpotCapacity` ändrades från 4 till 2 med `OEJ989` (Car) på plats 002; Reload Configuration avvisade ändringen och det tidigare aktiva garaget förblev korrekt.
- `TestDataSeeder` infördes i `Application` och kopplades i `Program.cs`, endast när `parking-data.json` saknas. Testkörning i separat kopia gav `TESTBUS1` på 001–004, `TESTCAR1` på 005, `TESTMC1` och `TESTBIKE1` tillsammans på 006 (3/4 enheter). Med 200 platser visades **FREE 194 / OCCUPIED 6 / VEHICLES 4**. En JSON-fil skapades; samma fordon, platser och UTC-tider lästes tillbaka efter omstart. En buss sparas bara på sin primärplats.
- Efter lärarens förtydligande har jag begränsat både bussparkering och bussflytt till parkeringsplatserna **001–050**. Standardkonfigurationen använder `BusMaxParkingSpot = 50`.
- Två interfaces finns: `IVehicle` och `IParkingSpot`. G-projekten har inte ändrats som del av VG-implementeringen.

### Mina personliga reflektioner

Busslogiken var den svåraste delen av arbetet med V2.1. En buss måste använda fyra sammanhängande parkeringsplatser, och dessa platser behöver hanteras korrekt både när bussen parkeras, flyttas och checkas ut. Efter att ha arbetat vidare med logiken och testat funktionerna fick jag det att fungera.

Jag lärde mig även hur `Action` kan användas i C#. I arbetet med `Reload Configuration` använde jag `Action` för att koppla samman `AdministrationView` och `MainMenu`. På så sätt kan administrationsvyn anropa funktionen utan att själv innehålla logiken som läser in och aktiverar den nya konfigurationen.

### Kvar inför slutlig leverans

- Granska V2.1 efter oanvända gamla `VehiclesPerSpot`-överlagringar och annan överflödig kod, med regressionstest före borttagning; ändra inte V2.0.
- Verifiera bussens felinjektion/rollback, ytterligare gränsfall för Bicycle och kapacitet, mer omfattande full Reload Configuration och portabilitet.
- Den planerade automatiska Live Demonstration i en isolerad Garage Overview har diskuterats men **är inte implementerad**.
- Kontrollera Git Changes och versionshantera den uppdaterade koden och dokumentationen på feature-branchen. Genererade filer i `bin/` och `obj/` samt lokala runtime-data ska inte ingå.

---

## 2026-10-22 – Slutförande av Prague Parking V2.0 och V2.1

### Genomfört

Idag färdigställde jag de sista delarna av Prague Parking V2.1 och genomförde den avslutande kodstädningen.

#### Live Demonstration

Jag färdigställde den interaktiva Live Demonstration som jag hade påbörjat tidigare.

- Implementerade en automatisk demonstration med Car, MC, Bicycle och Bus.
- Demonstrationen använder ett separat parkeringsgarage med 100 platser i minnet.
- Fordon kan parkeras, flyttas och checkas ut under demonstrationen.
- Parkeringskartan uppdateras löpande och visar aktuell beläggning med gröna, orange och röda färger.
- Anpassade `SPOT DETAILS` och `LAST EVENT` så att informationen presenteras tydligt utan att panelernas storlek förändras i onödan.
- Förbättrade den dynamiska renderingen och presentationen av parkeringskartan.
- Implementerade möjligheten att avsluta demonstrationen med ESC.
- Säkerställde att demonstrationen inte påverkar det ordinarie parkeringsgaraget eller dess sparade JSON-data.

Arbetet med demonstrationen tog betydligt mer tid än jag först hade räknat med. Framför allt behövde jag arbeta med hur konsolen uppdateras medan fordonen ändrar parkeringsplatser.

#### Kodstädning och slutkontroll

Jag gick igenom V2.1 för att rensa bort äldre kod som inte längre behövdes efter övergången till den storleksbaserade kapacitetsmodellen.

- Tog bort gamla metodöverlagringar som använde `maxVehiclesPerSpot`.
- Tog bort den gamla konfigurationsegenskapen `VehiclesPerSpot` från V2.1.
- Behöll den nya modellen med `SpotCapacity` och fordonsstorlekar via `Size`.
- Kontrollerade att `config.template.json` innehåller rätt standardvärden.
- Granskade `ConfigurationService` och hur den aktiva konfigurationsfilen skapas från mallen.
- Kontrollerade att ändringarna inte gav kompileringsfel.
- Lät den färdiga G-versionens kod och kapacitetsmodell vara oförändrade.

### Slutlig projektstatus

Både Prague Parking V2.0 (G) och Prague Parking V2.1 (VG) är nu färdigutvecklade och testade.

Projektet påbörjades den **5 oktober 2026** och färdigställdes den **22 oktober 2026**, vilket motsvarar 18 kalenderdagar inklusive start- och slutdatum.

Under utvecklingen har jag arbetat med objektorientering, arv, interfaces, delegater, JSON-persistens, konfigurationshantering, felhantering, felsökning och användargränssnitt med Spectre.Console.

Den återstående inlämningsförberedelsen består av att kontrollera dokumentationen mot lärarens kravspecifikation och säkerställa att rätt filer versionshanteras i GitHub.

### Mina personliga reflektioner – 22 oktober 2026

Det känns riktigt skönt att äntligen ha blivit klar med Prague Parking V2.0 och V2.1. Jag började den 5 oktober och blev färdig den 22 oktober 2026. Det har varit 18 intensiva kalenderdagar där jag har lagt ner väldigt mycket tid på projektet. Jag vågar knappt tänka på hur många timmar det egentligen har blivit.

En stor del av tiden har gått åt till research, felsökning och att försöka förstå hur olika delar av C# fungerar och hänger ihop. Det har inte alltid varit enkelt. Ibland har jag fastnat länge på saker som jag först trodde skulle vara ganska enkla. Samtidigt har det varit lärorikt eftersom jag verkligen har behövt sätta mig in i problemen och förstå varför vissa lösningar fungerar och andra inte.

Jag har också fått lägga mycket tid på att lära mig Visual Studio 2026. Det är en kraftfull utvecklingsmiljö med många användbara funktioner, men det tar tid att lära sig hur allt fungerar. Jag har bland annat blivit bättre på att använda felsökaren, hitta metodreferenser, navigera mellan klasser och förstå felmeddelanden. Det är kunskaper som jag kommer att ha nytta av även i kommande projekt.

#### Mina tankar om undervisningen

Något jag hade önskat under kursens gång är att undervisningen hade varit mer inriktad på objektorienterad programmering (OOP) än på de absoluta grunderna i programmering. Jag tycker personligen att man borde ha läst exempelvis Programmering A/1, eller ha motsvarande grundläggande kunskaper, innan man söker sig till en utbildning inom systemutveckling.

Jag hade själv aldrig programmerat i C# innan den här kursen. Min tidigare erfarenhet kommer främst från webbutveckling och Java samt Programmering B/2, där jag arbetade med C++. C++ var ett språk jag verkligen inte tyckte om, men C# har jag däremot fått ett helt annat intresse för. Jag tycker faktiskt väldigt mycket om språket och vill fortsätta utvecklas inom det.

Det jag framför allt saknade var mer praktisk träning i OOP. Att förstå hur klasser, objekt, arv, interfaces och metoder fungerar tillsammans i ett större program är enligt mig betydligt svårare än att lära sig grundläggande syntax. Det är också något som kräver mycket övning.

Jag försökte även bidra genom att ta initiativ till guider som kunde hjälpa mina klasskamrater att förstå grunderna bättre. Min förhoppning var att det skulle frigöra mer undervisningstid till de avancerade momenten. Tyvärr upplevde jag att initiativet inte fick det gensvar jag hade hoppats på och att mina synpunkter om undervisningens nivå inte riktigt fick gehör. Det tycker jag är synd, eftersom min avsikt var att både hjälpa andra och skapa bättre möjligheter för oss som ville fördjupa oss ytterligare.

Jag tar ändå med mig väldigt mycket från kursen. Framför allt har arbetet med Prague Parking gett mig den praktiska OOP-träning som jag efterfrågade. Det har också fått mig att inse hur mycket jag fortfarande har kvar att lära mig, vilket snarare motiverar mig att fortsätta utvecklas på egen hand.

#### Mina avslutande tankar

Under projektets gång har jag samtidigt märkt hur mycket jag faktiskt har utvecklats. I början var det svårt att hålla reda på alla klasser, metoder och hur de skulle samarbeta. Mot slutet började jag få en bättre helhetsförståelse. Busslogiken i V2.1 var förmodligen den största utmaningen, men också ett av momenten där jag lärde mig mest.

Jag har även lagt väldigt mycket tid på det visuella gränssnittet och på den avslutande Live Demonstration. Det tog betydligt längre tid än jag hade räknat med, men jag ville inte lämna ifrån mig något som jag själv inte var nöjd med.

Även om det känns skönt att ha blivit färdig betyder det inte att mitt lärande slutar här. Tvärtom har projektet gjort mig ännu mer intresserad av att fortsätta utvecklas inom C#. Jag känner fortfarande att det finns mycket kvar att lära, framför allt inom objektorientering och hur man bygger större och mer välstrukturerade applikationer.

Jag tänker därför fortsätta på egen hand, utmana mig själv med nya projekt och fördjupa mina kunskaper. Jag vill inte bara kunna skriva kod som fungerar, utan också förstå varför den fungerar och hur jag kan förbättra den.

Jag är nöjd med att jag tog mig hela vägen i mål, trots all tid och energi som krävdes. Samtidigt ser jag det här projektet som ett steg i min utveckling snarare än ett slutmål.

**Prague Parking är färdigt, men min utveckling som programmerare fortsätter.**
