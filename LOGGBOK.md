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

### Nästa steg

- Verifiera Spot Details med parkerade fordon.
- Kontrollera att garageöversikten fungerar med det gemensamma färgtemat.
- Testa konsolrenderingen i olika terminalstorlekar.
- Implementera Park Vehicle i huvudmenyn.
- Koppla befintlig parkeringslogik till användargränssnittet.
- Implementera Check Out Vehicle i UI.
- Fortsätta med Find Vehicle och Move Vehicle.
- Implementera permanent lagring i parking-data.json.
- Implementera administrationsfunktioner.
- Fortsätta dokumentera utveckling och tester.

