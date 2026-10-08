# Prague Parking V2 – Kravanalys

> Kraven nedan är en projektspecifik nedbrytning av uppgiftens krav till mindre,
> spårbara delkrav. ID-numren (G-xx och VG-xx) kommer alltså inte från den
> ursprungliga uppgiftstexten utan används för att göra implementation och
> uppföljning tydligare.

## Statusförklaring

- **Ej påbörjad** – ingen implementation finns ännu.
- **Pågår** – arbetet är påbörjat men kravet är inte färdigt.
- **Implementerad** – funktionaliteten finns i koden men är inte fullt verifierad.
- **Verifierad** – funktionaliteten är implementerad och har testats.

---

# G – Prague Parking 2.0

## Funktionella och tekniska krav

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-00 | Grundfunktionaliteten från Prague Parking V1 ska finnas kvar | Incheckning, flytt, utcheckning, sökning och översikt ska finnas även efter övergången till OOP-versionen | Domain / UI | Pågår |
| G-01 | Parkerade fordonsdata ska sparas permanent | Parkeringsinformationen ska finnas kvar även efter att programmet har stängts | Persistence | Ej påbörjad |
| G-02 | Parkeringsdata ska lagras i JSON-format | Parkerade fordon och deras tillstånd ska serialiseras till en JSON-fil | Persistence | Ej påbörjad |
| G-03 | Parkeringsdata ska läsas in vid programstart | Sparad JSON-data ska användas för att återställa parkeringshusets tillstånd när programmet startar | Persistence / Domain | Ej påbörjad |
| G-04 | Datafilen ska uppdateras vid incheckning | När ett fordon parkeras ska den aktuella parkeringsdatan sparas | Persistence | Ej påbörjad |
| G-05 | Datafilen ska uppdateras vid utcheckning | När ett fordon lämnar parkeringen ska den aktuella parkeringsdatan sparas | Persistence | Ej påbörjad |
| G-06 | Datafilen ska uppdateras när ett fordon flyttas | När ett parkerat fordon flyttas ska dess nya placering sparas | Persistence | Ej påbörjad |
| G-07 | Systemet ska kunna hantera olika parkeringskapacitet | I G används konfigurerbart antal fordon per plats. Den generella storleksmodellen för Bicycle, MC, Car och Bus implementeras först i VG-versionen | Domain / Configuration | Pågår |
| G-08 | Parkeringshuset ska visas som en karta/översikt | Användaren ska kunna se parkeringshusets beläggning i en tydlig konsolvy | UI | Ej påbörjad |
| G-09 | Det ska vara enkelt att se hur många platser som är lediga | Ledigt och upptaget utrymme ska vara visuellt tydligt i översikten | UI | Ej påbörjad |
| G-10 | Prislista ska kunna lagras utanför programkoden | Priser ska kunna ändras utan att C#-koden behöver ändras eller kompileras om | Configuration | Implementerad |
| G-11 | Prislistan ska läsas in vid programstart | Programmet använder aktuella priser från konfigurationen när det startar | Configuration | Verifierad |
| G-12 | Prislistan ska kunna läsas om under körning via menyval | Ändrade priser ska kunna aktiveras utan att programmet startas om | Configuration / UI | Ej påbörjad |
| G-13 | Prisinformationen ska vara enkel att redigera | Priserna ligger i en människoläsbar JSON-konfiguration | Configuration | Implementerad |
| G-14 | Systemet ska ha en konfigurationsfil | Inställningar som administratören ska kunna ändra hålls utanför programkoden | Configuration | Verifierad |
| G-15 | Konfigurationsfilens format ska vara JSON | Projektet använder `config.json` och `System.Text.Json` | Configuration | Verifierad |
| G-16 | Antal parkeringsplatser ska kunna konfigureras | Parkeringshusets storlek hämtas från `config.json` och är inte hårdkodad | Configuration / Domain | Verifierad |
| G-17 | Fordonstyper ska kunna konfigureras | Konfigurerbara uppgifter för fordonstyper lagras i `VehicleTypes` i `config.json` | Configuration / Domain | Implementerad |
| G-18 | Antal fordon per parkeringsplats ska kunna konfigureras för respektive fordonstyp | Kapacitetsregler hämtas från konfigurationen i stället för att hårdkodas i parkeringslogiken | Configuration / Domain | Verifierad |
| G-19 | Bil kostar 20 CZK per påbörjad timme | `ParkingFeeCalculator` använder bilpriset från konfigurationen och debiterar per påbörjad timme | Domain / Configuration | Implementerad |
| G-20 | MC kostar 10 CZK per påbörjad timme | `ParkingFeeCalculator` använder MC-priset från konfigurationen och debiterar per påbörjad timme | Domain / Configuration | Implementerad |
| G-21 | Parkering kortare än 10 minuter är gratis | Gratisgränsen hämtas från konfigurationen och används av `ParkingFeeCalculator` | Domain / Configuration | Implementerad |
| G-22 | Fordonets ankomsttid ska registreras | `Vehicle` får automatiskt en UTC-tid vid ny incheckning. Persistens av tiden återstår | Domain / Persistence | Pågår |
| G-23 | Systemet ska kunna ange hur länge fordonet varit parkerat | Parkeringstiden beräknas från `ArrivalTime` och `departureTime` i prislogiken. Presentation vid utcheckning återstår | Domain | Pågår |
| G-24 | Applikationen ska ha ett visuellt användargränssnitt i konsolen | Menyer, parkeringsöversikt och meddelanden ska presenteras tydligt i konsolen | UI | Pågår |
| G-25 | Konsolapplikationen ska använda Spectre.Console | Spectre.Console används för presentation och meddelanden | UI | Verifierad |
| G-26 | Alla fordon ska hanteras som objekt | Fordonsdata representeras med objekt i stället för sammansatta strängar som i V1 | Domain | Implementerad |
| G-27 | Systemet ska innehålla minst fem klasser | `Vehicle`, `Car`, `MC`, `ParkingSpot` och `ParkingGarage` finns | Architecture / Domain | Implementerad |
| G-28 | Klassen `Vehicle` ska finnas | Gemensamma egenskaper och regler för fordon samlas i en abstrakt basklass | Domain | Implementerad |
| G-29 | `Car` ska ärva från `Vehicle` | Bil modelleras som en specialisering av `Vehicle` | Architecture / Domain | Implementerad |
| G-30 | `MC` ska ärva från `Vehicle` | MC modelleras som en specialisering av `Vehicle` | Architecture / Domain | Implementerad |
| G-31 | `ParkingSpot` ska finnas som klass | Varje parkeringsplats representeras av ett objekt med platsnummer och parkerade fordon | Domain | Implementerad |
| G-32 | `ParkingGarage` ska finnas som klass | Parkeringshuset ansvarar för helheten och samlingen av parkeringsplatser | Domain | Implementerad |
| G-33 | Standardvärdet ska vara 100 parkeringsplatser | Standardkonfigurationen skapar 100 platser, men värdet kan ändras via konfigurationen | Domain / Configuration | Verifierad |
| G-34 | Lösningen ska innehålla ett separat klassbibliotek för dataåtkomst | Datarelaterad kod ligger i `PragueParkingV2.Data` | Architecture / Persistence | Implementerad |
| G-35 | Dataåtkomst ska separeras från övrig logik | Härlett från kravet på separat Data-projekt. Filåtkomst ska inte blandas med UI eller domänlogik | Architecture / Persistence | Pågår |
| G-36 | Inställningar ska läsas från konfigurationsfilen vid programstart | `config.json` läses och valideras innan parkeringsmodellen byggs upp | Configuration / Domain | Verifierad |
| G-37 | Uppgiften ska huvudsakligen lösas med kursens hittills behandlade tekniker | Lösningen använder klasser, arv, collections, loopar, listor och grundläggande LINQ där det förbättrar läsbarheten | Architecture | Pågår |

## Leverans- och processkrav

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-38 | Programmet ska kunna köras på en annan dator | Lösningen ska undvika absoluta lokala sökvägar och andra datorberoenden | Architecture / Deployment | Pågår |
| G-39 | Särskilda startinstruktioner ska dokumenteras i README | Om något utöver normal start krävs ska det beskrivas tydligt i README | Documentation / Deployment | Pågår |
| G-40 | Projektet ska lämnas in via GitHub | Hela projektet versionshanteras i Git och GitHub | Git / Delivery | Pågår |
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
| VG-03 | Bicycle ska stödjas | Bicycle införs som en ny fordonstyp. Om cykeln saknar registreringsnummer behövs en stabil systemidentifierare | Domain | Ej påbörjad |
| VG-04 | Bus ska stödjas | Bus införs som en ny fordonstyp med större kapacitetsbehov | Domain | Ej påbörjad |
| VG-05 | Parkeringskapacitet ska baseras på storleksenheter | Bicycle = 1, MC = 2, Car = 4, Bus = 16 och en normal parkeringsplats = 4 enheter | Domain / Configuration | Ej påbörjad |
| VG-06 | Bus kräver fyra sammanhängande parkeringsplatser | Systemet ska hitta fyra sammanhängande lediga parkeringsplatser för en buss, motsvarande totalt 16 kapacitetsenheter. Bussparkering får endast ske inom de första 50 parkeringsplatserna | Domain | Ej påbörjad |
| VG-07 | Bicycle kostar 5 CZK per påbörjad timme | Priset ska ligga i konfigurationen | Domain / Configuration | Ej påbörjad |
| VG-08 | Bus kostar 80 CZK per påbörjad timme | Priset ska ligga i konfigurationen | Domain / Configuration | Ej påbörjad |
| VG-09 | Fordonstyper och storlekar ska kunna konfigureras | Storleksregler ska hämtas från konfigurationen och inte hårdkodas i parkeringslogiken | Configuration / Domain | Ej påbörjad |
| VG-10 | Hela konfigurationen ska kunna läsas om under körning | Ett synligt menyval ska kunna ladda om konfigurationen utan omstart | Configuration / UI | Ej påbörjad |
| VG-11 | Ändrad konfiguration ska valideras innan den aktiveras | En ändring får inte skapa ett ogiltigt garagetillstånd, exempelvis ta bort upptagna platser | Configuration / Domain | Ej påbörjad |
| VG-12 | UI ska återspegla den nya konfigurationen | Efter lyckad reload ska översikt och regler använda de nya värdena | UI / Domain | Ej påbörjad |
| VG-13 | En reflektion på cirka 2–3 sidor ska lämnas in | Reflektionen ska behandla lösning, designval, problem och lärdomar | Documentation | Ej påbörjad |

---

# Designbeslut och projekttolkningar

- Prislista och övriga programinställningar lagras i samma `config.json`.
- Prisinställningarna placeras tydligt i konfigurationen så att de är enkla att hitta och ändra.
- UI, domänlogik och dataåtkomst hålls separerade.
- `ParkingSpot.SpotNumber` används som parkeringsplatsens naturliga identitet.
- Fordon identifieras i domänen genom `Identifier`.
- Motorfordons `Identifier` motsvarar registreringsnummer.
- Fordon utan registreringsnummer, exempelvis Bicycle, kan få en systemgenererad identifierare.
- Ankomsttid lagras internt i UTC och konverteras till konfigurerad tidszon vid presentation.
- Prisberäkning hanteras separat av `ParkingFeeCalculator`.
- Aktiva prisregler hålls i minnet efter att konfigurationen har lästs in.
- Nya prisvärden ska valideras innan de ersätter de aktiva prisvärdena.
- Endast upptagna parkeringsplatser planeras att sparas i `parking-data.json`; tomma platser återskapas från konfigurationen vid programstart.
- Saknad runtime-konfiguration får återskapas med standardvärden.
- En befintlig men ogiltig konfiguration ska inte skrivas över automatiskt; programmet ska i stället avsluta kontrollerat.

# Future improvements

- Undersök möjlighet att visuellt gruppera flera sammanhängande lediga parkeringsplatser i garageöversikten.