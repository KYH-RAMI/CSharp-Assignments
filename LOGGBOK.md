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
