# Prague Parking V2 – Projektplan och avslut

**Uppdaterad:** 2026-10-22  
**Projekt:** Prague Parking V2.0 (G) och V2.1 (VG)  
**Utvecklare:** Rami Issa  
**Utbildning:** Systemutvecklare DevSecOps, KYH  
**Utvecklingsmiljö:** Visual Studio Community 2026  
**Teknik:** C# / .NET 10 / Spectre.Console / System.Text.Json / Git och GitHub  
**Projektstart:** 2026-10-05  
**Färdigutvecklat:** 2026-10-22  
**Utvecklingsperiod:** **18 kalenderdagar inklusive start- och slutdatum** (17 dygn mellan datumen).

> **Slutstatus 22 oktober 2026:** Både **Prague Parking V2.0 (G)** och **Prague Parking V2.1 (VG)** är färdigutvecklade och testade. Projektets G- och VG-krav har gåtts igenom inför inlämningen. Arbetet med programkod och funktioner är avslutat. Den sista dokumentationsgenomgången och själva inlämningen hanteras separat från utvecklingsarbetet.
>
> Projektets GitHub-repository **förblir privat enligt ett individuellt undantag godkänt av läraren**. Läraren får åtkomst till repositoryt inför bedömningen.

---

## 1. Projektmål och omfattning

Målet var att vidareutveckla Prague Parking V1 till ett objektorienterat parkeringssystem och leverera två **separata, körbara versioner i samma solution**:

- **Prague Parking V2.0 (G):** Objektorienterad modell för Car och MC, konfigurerbara parkeringsplatser och priser, Spectre.Console-baserat gränssnitt, incheckning, utcheckning, sökning, flytt och JSON-persistens.
- **Prague Parking V2.1 (VG):** Vidareutveckling i egna projekt med två interfaces, Bicycle och Bus, storleksbaserad kapacitet, bussens fyra sammanhängande parkeringsplatser, fullständig konfigurationsomläsning och testdata när parkeringsfilen saknas.
- **Egen extrafunktion i V2.1:** Live Demonstration, som visar parkeringshändelser automatiskt i ett **separat garage i minnet** utan att ändra den ordinarie parkeringsfilen.

G-versionens kod, konfiguration och dokumenterade tester bevarades separat från VG-versionens vidareutveckling. Den nya versionen har ett eget konsolprojekt och ett eget dataklassbibliotek.

**Avgränsning:** Frivillig Windows-paketering, Synology NAS/Docker, Cloudflare och Discord-betatestning ingår inte i den färdiga kursinlämningen. De diskuterades under arbetets gång, men valdes bort för att prioritera kursens krav.

---

## 2. Projektfaser – slutligt utfall

Den ursprungliga arbetsplanen omfattade 13 faser. Tabellen visar **slutresultatet per 22 oktober**, inte läget vid en enskild utvecklingsdag.

| Fas | Arbetsområde | Slutligt utfall |
|---|---|---|
| 1 | Kravanalys och spårbarhet | **Avslutad** – G- och VG-krav dokumenterade och genomgångna i `requirements.md` |
| 2 | Solution och projektstruktur | **Avslutad** – fyra projekt i samma solution: två konsolappar och två dataklassbibliotek |
| 3 | Domänmodell och objektorientering | **Avslutad** – fordonshierarki, garage, parkeringsplatser och två VG-interfaces |
| 4 | JSON-konfiguration | **Avslutad** – separata konfigurationer, mallfil i VG, validering och omläsning |
| 5 | Parkering, sökning och flytt | **Avslutad och testad** – samtliga fyra fordonstyper i VG, bussens fyra platser |
| 6 | Prisberäkning | **Avslutad och testad** – timpriser och gratisgräns; även Bus och Bicycle i VG |
| 7 | CheckoutService och utcheckning | **Avslutad och testad** – avgift, kvitto, borttagning och sparning |
| 8 | Spectre.Console och garageöversikt | **Avslutad och testad** – dynamisk layout, platsinformation, färgstatus och viewport |
| 9 | Huvudmeny och UI-flöden | **Avslutad och testad** – alla grundfunktioner, administration och Live Demonstration i VG |
| 10 | Parkeringspersistens | **Avslutad och testad** – JSON-sparning och återställning, inklusive bussreservationer |
| 11 | Administration | **Avslutad och testad** – Reload Pricing (G), Reload Configuration (VG) |
| 12 | Integrationstester och dokumentation | **Funktionstester genomförda** – resultat i loggboken; avslutande dokumentgranskning 22 oktober |
| 13 | Prague Parking V2.1 / VG | **Avslutad 2026-10-22** – VG-funktionerna och den extra demonstrationen färdigställda |

**Statusprincip:** *Färdigutvecklad* betyder att planerad funktionalitet är implementerad; *testad* avser de manuella scenarier som dokumenterats. Det innebär inte att alla teoretiskt möjliga fel, operativsystem eller terminalmiljöer provats. Inga automatiserade enhetstester påstås ha körts om det saknas sådana protokoll.

---

## 3. Milstolpe 1 – Arkitektur, klasser och interfaces

**Slutstatus: Genomförd och testad.**

G-versionens domänmodell bygger bland annat på `Vehicle`, `Car`, `MC`, `ParkingSpot`, `ParkingGarage`, `ParkingFeeCalculator`, `ParkingFeeResult`, `CheckoutService` och `CheckoutResult`.

- **Arv:** `Car` och `MC` ärver från den abstrakta basklassen `Vehicle`. I V2.1 ärver även `Bicycle` och `Bus` från `Vehicle`.
- **Inkapsling:** `ParkingSpot` ansvarar för fordonen på den egna platsen. `ParkingGarage` ansvarar för samlingen av platser och operationer som involverar hela garaget.
- **Ansvarsfördelning:** `ParkingFeeCalculator` beräknar avgiften. `CheckoutService` samordnar utcheckningen. `ParkingDataMapper` översätter mellan domänobjekt och JSON-datamodeller.
- **Tidsmodell:** `ArrivalTime` lagras i UTC och kan bevaras när fordon flyttas eller återställs efter omstart.

### Vad ett interface är i projektet

Ett **interface** är ett kontrakt: det talar om vilka medlemmar en klass måste erbjuda när klassen anger att den implementerar interfacet. Interfacet beskriver i första hand **vad som måste finnas**, medan klassen ansvarar för **hur det fungerar**. Kompilatorn kontrollerar att kontraktet uppfylls.

V2.1 innehåller två interfaces:

| Interface | Implementeras av | Funktion och kontraktsidé |
|---|---|---|
| **`IVehicle`** | Den abstrakta klassen `Vehicle`; ärvs därigenom av `Car`, `MC`, `Bicycle` och `Bus` | Kräver en **`Identifier`** och en **`ArrivalTime`**. Kod kan därmed behandla olika fordon som `IVehicle` utan att först känna till exakt fordonsklass. |
| **`IParkingSpot`** | Klassen `ParkingSpot` | Beskriver vad en parkeringsplats ska kunna exponera och göra, bland annat sin identitet, parkerade fordon samt operationer för kapacitet och fordonshantering. Den konkreta klassen hanterar själva lagringen, valideringen och eventuella bussreservationer. |

**`IVehicle` – kontraktsanvisningar:**

- `Identifier` ger en gemensam identifierare. För Car/MC motsvarar den normalt registreringsnummer; Bicycle kan ha en annan identifierare.
- `ArrivalTime` gör fordonets ankomsttid tillgänglig oavsett fordonstyp. Projektet använder UTC för tidsberäkning och JSON-återställning.
- Ett interface kräver **inte** att klasserna har samma konstruktor och bestämmer inte hur identifieraren eller tiden lagras internt.

**`IParkingSpot` – kontraktsanvisningar:**

- Parkeringsplatsen har ett platsnummer (`SpotNumber`) och kan exponera sin samling parkerade fordon utan att ge anropande kod fri åtkomst till den privata listan.
- Kapacitetskontroll och operationer som `GetUsedCapacity()`, `CanPark()`, `ParkVehicle()`, `FindVehicle()` och `RemoveVehicle()` beskriver sådant som `ParkingSpot` hanterar. De används enligt den storleksbaserade modellen i VG.
- Kontraktet är **inte** samma sak som en hårdkodad kapacitetsregel. Hur många storleksenheter ett fordon upptar hämtas från konfigurationen.
- Bussens **fyra sammanhängande platser** kräver samordning mellan flera platser; det ansvaret ligger på `ParkingGarage`, inte enbart på `IParkingSpot`.

**Skillnaden mellan interface, arv och delegat:** `Vehicle` samlar gemensam implementation genom arv; `IVehicle` och `IParkingSpot` anger kontrakt. `Func<IVehicle, int>` är däremot en **delegattyp**, inte ett interface. Den används för att skicka in en funktion som hämtar ett fordons storlek. På så sätt behöver `ParkingSpot` inte själv läsa `config.json`.

> Beskrivningen ovan förklarar kontraktens ansvar. Exakta deklarationer, tillgänglighetsnivåer och metodsignaturer finns i `IVehicle.cs` och `IParkingSpot.cs`; projektplanen återger dem inte som fullständig API-dokumentation. Se även `docs/class-diagram.md`.

**Historiska milstolpar:** G-modellen etablerades **5–9 oktober**. De separata VG-projekten och `IVehicle` skapades **20 oktober**; `IParkingSpot` och bussens reservationsmodell följde **21 oktober**. Överflödiga äldre kapacitetsöverlagringar i **V2.1** rensades **22 oktober**. G:s kapacitetsmodell ändrades inte.

---

## 4. Milstolpe 2 – Konfiguration

**Slutstatus: Genomförd och testad.**

I båda versionerna hanteras inställningar utanför programkoden med JSON och `ConfigurationService`. Inställningarna omfattar priser, gratisminuter, antal platser och tidszon (`Europe/Prague`).

**V2.0 (G):** `config.json` innehåller bland annat kapacitetsreglerna `VehiclesPerSpot` för Car/MC. **Administration → Reload Pricing** uppdaterar priser utan omstart; bilpris **20 → 30 CZK** fungerade i test och ett ogiltigt värde på **−10 CZK** avvisades den **16 oktober**.

**V2.1 (VG):** Standardinställningarna ligger i `Config/config.template.json`. Om aktiv `Config/config.json` saknas vid första start kopierar `ConfigurationService` mallen till programmets körkatalog. En **befintlig** konfigurationsfil valideras och skrivs inte över automatiskt.

Standardvärden för V2.1:

| Inställning | Värde |
|---|---|
| `Garage.NumberOfParkingSpots` | 100 |
| `Garage.SpotCapacity` | 4 enheter per vanlig plats |
| `Garage.BusMaxParkingSpot` | 50 |
| `VehicleTypes.Bicycle.Size` | 1 |
| `VehicleTypes.MC.Size` | 2 |
| `VehicleTypes.Car.Size` | 4 |
| `VehicleTypes.Bus.Size` | 16 |
| `Pricing.FreeMinutes` | 10 |
| Timpris Car / MC / Bicycle / Bus | 20 / 10 / 5 / 80 CZK |
| `System.TimeZone` | `Europe/Prague` |

**Administration → Reload Configuration** läser om **hela konfigurationen** under körning. Den nya konfigurationen kontrolleras mot befintliga parkeringar i ett tillfälligt garage och aktiveras först när kontrollen lyckas. Den **21 oktober** testades utökning **100 → 200** samt visning med **500** platser. Ett försök att minska `SpotCapacity` från **4 till 2** med en parkerad Car avvisades, utan att den giltiga aktiva konfigurationen ersattes.

---

## 5. Milstolpe 3 – Prisberäkning och utcheckning

**Slutstatus: Genomförd och testad.**

`ParkingFeeCalculator` hanterar avgifter och `CheckoutService` samordnar sökning, beräkning och borttagning. `ParkingFeeResult` och `CheckoutResult` bär beräknings- och kvittoinformation i minnet och ingår inte i `parking-data.json`.

- Avgiften beräknas per **påbörjad timme** enligt konfigurationen.
- Projektets testade gratisregel innebär att parkering **kortare än 10 minuter** är gratis; vid exakt 10 minuter debiterar den implementerade beräkningen.
- G-versionens priser är **20 CZK/h för Car** och **10 CZK/h för MC**.
- VG-versionen har även **5 CZK/h för Bicycle** och **80 CZK/h för Bus**.
- En buss debiteras som **ett fordon**, trots att den reserverar fyra platser. Den **21 oktober** gav `BUS001` med parkeringstiden **1 h 38 min** avgiften **160 CZK**.
- En utcheckning uppdaterar den permanenta JSON-filen; programmet ska inte visa ett lyckat kvitto om sparningen rapporterar fel.

**Dokumenterade tester:** `UKB48K` checkades ut och förblev borttagen efter omstart (**15 oktober**). Den **16 oktober** prövades återställning vid framprovocerat sparfel i G-versionen. Den **21 oktober** testades bussens normala utcheckning med frigöring av alla fyra reserverade platser i VG.

---

## 6. Milstolpe 4 – Spectre.Console och garageöversikt

**Slutstatus: Genomförd och testad.**

Gränssnittet använder `Spectre.Console` för menyer, paneler, kvitton och parkeringskarta. Gemensamma presentationselement finns i `AppTheme` och statusmeddelanden i `MessageService`.

**Garage Overview** använder ett kompakt `Grid` inuti en gemensam `Panel` utan separata ramar runt varje parkeringsplats. Antalet synliga kolumner och rader anpassas till terminalens storlek. `PARKING MAP`, `SPOT DETAILS` och `GARAGE STATUS` visar platsnummer, beläggning, fordon och aktuell parkeringstid. När kartan inte ryms används en intern viewport. Piltangenterna används för navigering och **ESC** för återgång.

Färgerna visar **grönt = ledig**, **orange = delvis upptagen** och **rött = full eller bussreserverad**. G-versionen behåller sitt blågula tema; VG-versionen använder ett separat mörkt **Sunset-tema**.

**Utvecklingshistorik:** Efter flera försök med `Table`, individuella `Panel`-komponenter och fasta rutnät blev den dynamiska, kompaktare layouten fungerande **13 oktober**. G-versionen testades med **100, 200 och 1 000** platser, inklusive navigering till plats **1000** den **16 oktober**. VG:s blandade kapacitet, Bus/Bicycle och 500-platsersvisning verifierades **21 oktober**. Renderingen i Live Demonstration färdigställdes **22 oktober**.

---

## 7. Milstolpe 5 – Användarflöden och fordonslogik

**Slutstatus: Genomförd och testad.**

Båda versionerna har Park Vehicle, Check Out Vehicle, Move Vehicle, Find Vehicle, Garage Overview och Administration. I **V2.1** finns dessutom Live Demonstration.

- **Park Vehicle:** Fordonet får den första lämpliga platsen enligt aktuell kapacitet och konfiguration.
- **Move Vehicle:** Destinationen kontrolleras innan flytt; samma fordonsobjekt och `ArrivalTime` bevaras.
- **Find Vehicle:** Sökning sker med fordonsidentifierare och visar plats samt fordonsinformation. Bus visas med sitt platsintervall.
- **Check Out Vehicle:** Avgift beräknas, kvitto visas och platsen frigörs efter den lyckade operationen.
- **Administration:** Reload Pricing i G; Reload Pricing och Reload Configuration i VG.

### VG – storleksbaserad parkering och buss

En normal plats rymmer **4 enheter**. Bicycle använder 1, MC 2 och Car 4. Fordon kan dela plats när summan ryms: **MC + Bicycle = 2 + 1 = 3/4**, vilket testades **21 oktober**.

Bus har storlek 16 och reserverar **fyra helt lediga, sammanhängande platser**. En buss representeras som **ett logiskt fordon** på primärplatsen, med reservationer för de övriga platserna. Det gör att sökning, flytt, utcheckning, avgift och JSON-lagring kan behandla den som ett fordon. Enligt lärarens förtydligande får bussar använda **platserna 001–050**, även vid flytt; `BusMaxParkingSpot` är satt till 50 i kurskonfigurationen.

Den **21 oktober** testades bussflytten **004–007 → 006–009** med bibehållen ankomsttid, följt av utcheckning som frigjorde reservationerna. Det tidigare visningsfelet för Bicycle i Garage Overview åtgärdades samma dag.

---

## 8. Milstolpe 6 – JSON-persistens och testdata

**Slutstatus: Genomförd och testad.**

Det separata dataklassbiblioteket hanterar filåtkomst, konfiguration och JSON-datamodeller. `ParkingDataMapper` i applikationslagret översätter mellan JSON-modeller och domänens fordons- och garageobjekt.

Grundstrukturen är:

```text
ParkingSpots
└── SpotNumber
    └── Vehicles
        ├── Identifier
        ├── VehicleType
        └── ArrivalTime (UTC)
```

Endast upptagna platser och deras fordon sparas. Vid uppstart skapas ett garage med aktuellt konfigurerat platsantal, och fordonen återställs på ursprunglig plats med samma identifierare och UTC-ankomsttid. En befintlig felaktig JSON-fil ska inte tyst ersättas med ny data.

**V2.1:** En buss sparas som **ett fordonsobjekt på primärplatsen**; de fyra reservationerna återskapas när JSON läses in. När `parking-data.json` **saknas**, skapar `TestDataSeeder` testfordon och sparar en ny fil. Detta görs inte när en datafil redan finns.

**Dokumenterade verifieringar:**

- **11 oktober:** Tidiga `Load()`- och JSON-valideringstester.
- **14–15 oktober:** G – parkering, utcheckning och flytt med omstart och bibehållna data.
- **16 oktober:** G – korrupt JSON, dubbletter, ogiltiga platsnummer/kapacitet och framprovocerade skrivfel i Park/Checkout/Move med återställning i de testade fallen.
- **21 oktober:** VG – testdata med `TESTBUS1` (001–004), `TESTCAR1` (005), `TESTMC1` och `TESTBIKE1` (båda på 006). Med 200 platser visade kartan **FREE 194 / OCCUPIED 6 / VEHICLES 4**. Fordon, placeringar och UTC-tider fanns kvar efter omstart.

De specifika skrivfelstesterna visar fungerande återställning i just de fallen; de bevisar inte generell kraschsäkerhet vid till exempel strömavbrott.

---

## 9. Milstolpe 7 – Administration och omläsning

**Slutstatus: Genomförd och testad.**

G-versionens **Reload Pricing** ändrar de aktiva priserna utan att programmet behöver startas om. V2.1:s **Reload Configuration** omfattar hela konfigurationen och validerar ändringarna innan det aktiva garaget byts ut. Det förhindrar bland annat att en konfigurationsändring gör redan parkerade fordon ogiltiga.

I VG används en `Action`-delegat för att låta administrationsvyn anropa omläsningsfunktionen som samordnas av huvudmenyn, utan att lägga ansvaret för uppdatering av hela garaget i själva UI-vyn. Detta blev också ett praktiskt exempel på användning av delegater i projektet.

De redovisade testerna **16 oktober** (Reload Pricing) och **21 oktober** (Reload Configuration, godkända och avvisade värden) dokumenteras närmare i `LOGGBOK.md`.

---

## 10. Milstolpe 8 – Testning och kvalitetssäkring

**Slutstatus: Manuella funktionstester genomförda i både G och VG.**

Arbetet har omfattat både lyckade normalflöden och utvalda negativa tester. För detaljerade datum, inmatningar, resultat och felsökning hänvisas till `LOGGBOK.md` och kravuppföljningen i `requirements.md`.

| Datum | Dokumenterad test- och utvecklingsmilstolpe |
|---|---|
| **2026-10-05** | Projektstart, solution, arkitektur och inledande kravanalys |
| **2026-10-06–09** | Domänmodell, kapacitet, avgifter, checkout och tidig UI-utveckling |
| **2026-10-10–11** | UI-struktur och grund för JSON-persistens; validering och felsökning |
| **2026-10-12–13** | Iterationer av garagekartan; fungerande kompakt layout |
| **2026-10-14** | G – incheckning, JSON-sparning och återställning efter omstart |
| **2026-10-15** | G – utcheckning, fordonsflytt, kvitton och delad MC-plats |
| **2026-10-16** | G – avslutande manuella tester med negativa JSON-fall, rollback och upp till 1 000 platser |
| **2026-10-20** | VG – två nya projekt, fordonsklasser, `IVehicle` och ny kapacitetsmodell påbörjas |
| **2026-10-21** | VG – buss, `IParkingSpot`, Bicycle, Sunset, Reload Configuration, TestDataSeeder och JSON-omstart testas |
| **2026-10-22** | VG – Live Demonstration färdigställs och testas; kodstädning genomförs; båda versionerna färdigutvecklade och testade; kravmatris genomgången |

**Testunderlagets avgränsning:** Ingen automatiserad testsamling, generell garanti om atomiska filtransaktioner eller separat protokoll från en annan fysisk dator har redovisats. De avgränsningarna innebär inte att ytterligare programutveckling planerats inför denna inlämning. Resultat för enskilda specialfall ska inte tillskrivas projektet utan testbelägg.

---

## 11. Milstolpe 9 – Live Demonstration och projektavslut

**Slutstatus: Färdigställd och testad 2026-10-22.**

Live Demonstration är en frivillig extra funktion i V2.1, skild från de obligatoriska G- och VG-kraven. Demonstrationen visar automatisk parkering, flytt och utcheckning för Car, MC, Bicycle och Bus. Parkeringskarta, `SPOT DETAILS`, `GARAGE STATUS` och `LAST EVENT` uppdateras under simuleringen. Demonstrationen avslutas med **ESC** och arbetar i ett separat garage i minnet, vilket gör att den inte ändrar den ordinarie `Data/parking-data.json`.

Den **22 oktober** gick jag även igenom V2.1:s äldre kapacitetsmodell och tog bort överflödiga `VehiclesPerSpot`-referenser och metodöverlagringar för `maxVehiclesPerSpot`. Den storleksbaserade modellen med `SpotCapacity`, `VehicleTypes.Size` och `Func<IVehicle, int>` behölls. G-versionens tidigare fungerande modell lämnades oförändrad. Lösningen byggde därefter utan rapporterade kompileringsfel.

**Projektets utvecklingsarbete avslutades den 22 oktober 2026.** Avslutande dokumentationsarbete och den faktiska inlämningen är administrativa leveranssteg, inte nya funktioner eller utvecklingsfaser.

---

## 12. Dokumentation och leverans

Projektets dokumentation har delats upp så att **README** ger en snabb översikt, medan övriga dokument ger teknisk fördjupning och historik.

| Dokument | Syfte |
|---|---|
| [`README.md`](../README.md) | Överblick, projektstatus och startinstruktioner |
| [`LOGGBOK.md`](../LOGGBOK.md) | Kronologiskt arbete, tester, fel, lösningar och personliga reflektioner |
| [`requirements.md`](requirements.md) | Kravmatris för G och VG med dokumenterad uppföljning |
| [`project-plan.md`](project-plan.md) | Projektfaser, milstolpar, utvecklingstid och slutligt utfall |
| [`design-decisions.md`](design-decisions.md) | Arkitekturbeslut och tekniska avvägningar |
| [`class-diagram.md`](class-diagram.md) | Klasser, relationer och gränssnitt |
| [`configuration.md`](configuration.md) | Konfigurationsmodell och omläsning |
| [`parking-data.md`](parking-data.md) | JSON-struktur, validering och återställning |

*Länkarna ovan förutsätter att `project-plan.md` ligger i repositoryts `docs/` och att `README.md` och `LOGGBOK.md` ligger i repositoryroten.*

**Versionshantering:** Git och GitHub. Repositoryt ska **inte** göras publikt eftersom läraren har godkänt ett individuellt undantag. Läraren får åtkomst till det privata repositoryt. Den faktiska inlämningen registreras när den genomförs.

**Slutsats:** Det planerade utvecklingsarbetet för **Prague Parking V2.0 (G) och V2.1 (VG)** är genomfört och testat. Kravuppföljningen har gåtts igenom. Projektet är färdigt för inlämning efter den sista dokumentationskontrollen.
