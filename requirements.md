# Prague Parking V2 – Kravanalys

## G – Prague Parking 2.0

| ID | Krav | Tolkning | Berör | Status |
|---|---|---|---|---|
| G-00 | Grundfunktionaliteten från Prague Parking V1 ska finnas kvar | Funktionerna från V1 ska fortfarande fungera efter övergången till OOP-versionen | Domain / UI | Ej påbörjad |
| G-01 | Parkerade fordonsdata ska sparas permanent | Parkeringsinformationen ska finnas kvar även efter att programmet har stängts | Persistence | Ej påbörjad |
| G-02 | Parkeringsdata ska lagras i JSON-format | Parkerade fordon och deras tillstånd ska serialiseras till en JSON-fil | Persistence | Ej påbörjad |
| G-03 | Parkeringsdata ska läsas in vid programstart | Sparad JSON-data ska användas för att återställa parkeringshusets tillstånd när programmet startar | Persistence / Domain | Ej påbörjad |
| G-04 | Datafilen ska uppdateras vid incheckning | När ett fordon parkeras ska den aktuella parkeringsdatan omedelbart sparas | Persistence | Ej påbörjad |
| G-05 | Datafilen ska uppdateras vid utcheckning | När ett fordon lämnar parkeringen ska den aktuella parkeringsdatan omedelbart sparas | Persistence | Ej påbörjad |
| G-06 | Datafilen ska uppdateras när ett fordon flyttas | När ett parkerat fordon flyttas ska dess nya placering omedelbart sparas | Persistence | Ej påbörjad |
| G-07 | Parkeringsplatser av olika storlek ska kunna hanteras | Parkeringsmodellen ska kunna representera olika kapacitetsbehov och får inte byggas runt endast en hårdkodad fordonsstorlek | Domain / Configuration | Ej påbörjad |
| G-08 | Parkeringshuset ska visas som en karta/översikt | Användaren ska kunna se parkeringshusets beläggning i en tydlig konsolvy | UI | Ej påbörjad |
| G-09 | Det ska vara enkelt att se hur många platser som är lediga | Ledigt och upptaget utrymme ska vara visuellt tydligt även när man betraktar översikten som helhet | UI | Ej påbörjad |
| G-10 | Prislista ska kunna lagras utanför programkoden | Priser ska kunna ändras utan att C#-koden behöver ändras eller kompileras om | Configuration | Ej påbörjad |
| G-11 | Prislistan ska läsas in vid programstart | Programmet ska använda aktuella priser från konfigurationen när det startar | Configuration | Ej påbörjad |
| G-12 | Prislistan ska kunna läsas om under körning via menyval | Ändrade priser ska kunna aktiveras utan att programmet behöver startas om | Configuration / UI | Ej påbörjad |
| G-13 | Prisinformationen ska vara enkel att redigera | Priserna ska ligga i en människoläsbar konfiguration som kan ändras med en vanlig texteditor | Configuration | Ej påbörjad |
| G-14 | Systemet ska ha en konfigurationsfil | Inställningar som administratören ska kunna ändra ska hållas utanför programkoden | Configuration | Ej påbörjad |
| G-15 | Konfigurationsfilens format bör vara JSON | Vi använder standard-JSON eftersom formatet är strukturerat, människoläsbart och stöds direkt av .NET | Configuration | Ej påbörjad |
| G-16 | Antal parkeringsplatser ska kunna konfigureras | Parkeringshusets storlek ska hämtas från `config.json` och inte vara hårdkodad | Configuration / Domain | Ej påbörjad |
| G-17 | Fordonstyper ska kunna konfigureras | Fordonstypernas konfigurerbara uppgifter ska lagras utanför programkoden | Configuration / Domain | Ej påbörjad |
| G-18 | Antal fordon per parkeringsplats ska kunna konfigureras för respektive fordonstyp | Kapacitetsreglerna ska kunna ändras via konfigurationen i stället för att spridas som hårdkodade värden i programmet | Configuration / Domain | Ej påbörjad |
| G-19 | Bil kostar 20 CZK per påbörjad timme | Prisberäkningen för bil ska utgå från 20 CZK per påbörjad timme enligt aktuell konfiguration | Domain / Configuration | Ej påbörjad |
| G-20 | MC kostar 10 CZK per påbörjad timme | Prisberäkningen för MC ska utgå från 10 CZK per påbörjad timme enligt aktuell konfiguration | Domain / Configuration | Ej påbörjad |
| G-21 | Parkering kortare än 10 minuter är gratis | Om parkeringstiden är mindre än 10 minuter ska kostnaden vara 0 CZK | Domain / Configuration | Ej påbörjad |
| G-22 | Fordonets ankomsttid ska registreras | Systemet ska spara tidpunkten när ett fordon checkas in | Domain / Persistence | Ej påbörjad |
| G-23 | Systemet ska kunna ange hur länge fordonet varit parkerat | Vid utcheckning ska parkeringstiden beräknas utifrån den sparade ankomsttiden | Domain | Ej påbörjad |
| G-24 | Applikationen ska ha ett grafiskt användargränssnitt | Parkeringsinformationen och menyvalen ska presenteras tydligt i konsolen | UI | Ej påbörjad |
| G-25 | Konsolapplikationen ska använda Spectre.Console | Spectre.Console ska användas för programmets användargränssnitt | UI | Ej påbörjad |
| G-26 | Alla fordon ska hanteras som objekt | Fordonsdata ska representeras med objekt i stället för sammansatta strängar som i V1 | Domain | Ej påbörjad |
| G-27 | Systemet ska innehålla minst fem klasser | De fem obligatoriska klasserna ska finnas och fler får skapas om de har ett tydligt ansvar | Architecture / Domain | Ej påbörjad |
| G-28 | Klassen `Vehicle` ska finnas | Gemensamma egenskaper och beteenden för fordon ska samlas i en gemensam basklass | Domain | Ej påbörjad |
| G-29 | `Car` ska ärva från `Vehicle` | Bil ska modelleras som en specialisering av `Vehicle` | Architecture / Domain | Ej påbörjad |
| G-30 | `MC` ska ärva från `Vehicle` | MC ska modelleras som en specialisering av `Vehicle` | Architecture / Domain | Ej påbörjad |
| G-31 | `ParkingSpot` ska finnas som klass | Varje parkeringsplats ska representeras av ett objekt med egen identitet och eget tillstånd | Domain | Ej påbörjad |
| G-32 | `ParkingGarage` ska finnas som klass | Parkeringshuset ska representeras av ett objekt som ansvarar för helheten och samlingen av parkeringsplatser | Domain | Ej påbörjad |
| G-33 | Standardvärdet ska vara 100 parkeringsplatser | Om inget annat anges ska garaget ha 100 platser, men värdet ska kunna ändras via konfigurationen | Domain / Configuration | Ej påbörjad |
| G-34 | Lösningen ska innehålla ett separat klassbibliotek för dataåtkomst | Filinläsning och filsparning ska ligga i `PragueParkingV2.Data` och inte blandas med UI eller parkeringslogik | Architecture / Persistence | Ej påbörjad |
| G-35 | Dataåtkomst ska separeras från övrig logik | Klasser och metoder som läser eller skriver filer ska ha ett separat ansvar | Architecture / Persistence | Ej påbörjad |
| G-36 | Datafilen ska kunna användas som testdata | Programmet ska kunna startas med redan sparade parkerade fordon så att funktionaliteten kan testas | Persistence / Testing | Ej påbörjad |
| G-37 | Inställningar ska läsas från konfigurationsfilen vid programstart | `config.json` ska läsas innan systemets parkeringsmodell byggs upp | Configuration / Domain | Ej påbörjad |
| G-38 | Uppgiften ska huvudsakligen lösas med kursens hittills behandlade tekniker | Lösningen ska bygga på bland annat klasser, arv, collections, loopar och listor | Architecture | Ej påbörjad |
| G-39 | Programmet ska kunna köras på en annan dator | Lösningen får inte vara beroende av absoluta lokala sökvägar eller andra inställningar som endast fungerar på utvecklingsdatorn | Architecture / Deployment | Ej påbörjad |
| G-40 | Särskilda startinstruktioner ska dokumenteras i README | Om något utöver normal start krävs ska det beskrivas tydligt i projektets README | Documentation / Deployment | Ej påbörjad |
| G-41 | Projektet ska lämnas in via GitHub | Hela projektet ska versionshanteras och lämnas in genom ett GitHub-repository | Git / Delivery | Ej påbörjad |
| G-42 | GitHub-repositoryt ska vara publikt | Läraren ska kunna komma åt och granska projektet utan särskild behörighet | Git / Delivery | Ej påbörjad |
| G-43 | Inlämningen ska länka till hela repositoryt | Länken i lärplattformen ska gå till repositoryt och inte till en enskild fil | Git / Delivery | Ej påbörjad |
| G-44 | Loggbok ska föras under projektets gång | Arbetsprocess, beslut, problem och lösningar ska dokumenteras löpande i `LOGGBOK.md` | Documentation / Process | Ej påbörjad |
| G-45 | Inlämningen är individuell | Den slutliga inlämningen ska vara individuell även om diskussion och samarbete under arbetets gång är tillåtet | Process / Delivery | Ej påbörjad |

## VG – Prague Parking 2.1

## Egna tolkningar
- Prislista och övriga programinställningar kombineras i en gemensam `config.json`.
- Prisinställningarna placeras överst i konfigurationsfilen för att göra dem enkla att hitta och ändra.
- UI, domänlogik och dataåtkomst ska hållas separerade.
- Parkeringsplatsens platsnummer används som dess identitet.
- Motorfordon identifieras med registreringsnummer. Fordon utan registreringsnummer, exempelvis cyklar i VG-versionen, kommer att behöva en systemgenererad identifierare.


## Öppna frågor

- Grundkraven nämner stöd för olika stora parkeringsplatser så att bussar och cyklar kan hanteras, medan den konkreta buss- och cykelfunktionaliteten ligger under VG/2.1. G-versionen bör därför förbereda kapacitetsmodellen utan att implementera hela VG-funktionaliteten.

## Future improvements

- Undersök möjlighet att visuellt gruppera flera sammanhängande lediga parkeringsplatser i garageöversikten.