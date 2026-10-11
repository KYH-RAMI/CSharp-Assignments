# Prague Parking V2 – Designbeslut

**Uppdaterad:** 2026-10-11  
**Omfattning:** Arkitektur- och implementationsbeslut för Prague Parking 2.0 (G), med dokumenterade avgränsningar inför Prague Parking 2.1 (VG).

Besluten nedan beskriver både genomförda arkitekturval och planerade lösningar. Där implementation eller tester återstår anges det uttryckligen. Beslut om ett framtida arbetssätt innebär inte att funktionen redan finns i programmet.

## DD-001 – Separation mellan UI, domän och dataåtkomst

**Beslut:** Presentation, affärsregler och dataåtkomst hålls separerade.

**Motivering:** Gränssnittet kan ändras utan att fordon och parkeringsregler påverkas.

JSON-hantering placeras i det separata Data-projektet. Dataåtkomst ska inte blandas med UI-kod eller domänregler.

## DD-002 – Gemensam abstrakt basklass för fordon

**Beslut:** `Vehicle` är abstrakt och används som basklass för `Car` och `MC`.

**Motivering:** Gemensam identifierare och ankomsttid ska inte dupliceras mellan fordonstyper.

## DD-003 – Generell fordonsidentifierare

**Beslut:** Fordon identifieras med `Identifier` i stället för en egenskap som förutsätter registreringsnummer.

**Motivering:** Framtida fordonstyper, exempelvis `Bicycle`, kan sakna registreringsnummer.

För `Car` och `MC` motsvarar identifieraren normalt registreringsnummer.

## DD-004 – Parkeringsplatsen ansvarar för sina fordon

**Beslut:** `ParkingSpot` har en privat lista över parkerade fordon och exponerar den som `IReadOnlyList<Vehicle>`.

**Motivering:** UI och garageklass ska kunna läsa beläggningen utan att direkt manipulera parkeringsplatsens interna lista.

`IReadOnlyList<T>` begränsar vad en anropare kan göra genom egenskapen. Den interna samlingen hanteras fortsatt av `ParkingSpot`.

## DD-005 – Fordon lagrar inte parkeringsplatsnummer

**Beslut:** `Vehicle` lagrar inte `SpotNumber`.

**Motivering:** Fordonets placering avgörs av vilken `ParkingSpot` som innehåller objektet.

Detta undviker dubbellagrad information i G-versionen. VG-versionens bussar kräver en särskild modell för beläggning av flera platser; det beslutet behandlas i DD-026.

## DD-006 – Garageklassen samordnar placering och sökning

**Beslut:** `ParkingGarage` skapar och söker bland platserna, hittar första lämpliga platsen, parkerar och tar bort fordon.

**Motivering:** En parkeringsplats hanterar sina lokala regler medan garaget hanterar hela samlingen.

`ParkingGarage` kontrollerar även om en fordonsidentifierare redan finns innan en ny parkering genomförs.

## DD-007 – Kapacitetsmodell i G-versionen

**Beslut:** Antalet fordon per parkeringsplats konfigureras separat för `Car` och `MC` via `VehiclesPerSpot`.

**Motivering:** G-versionen kan hantera en bil eller två MC per plats utan att införa VG-versionens generella storleksenheter i förtid.

Fordonskapaciteten hämtas från konfigurationen och skickas till parkeringslogiken. Olika fordonstyper blandas inte på samma plats i den nuvarande G-modellen.

## DD-008 – JSON som filformat

**Beslut:** Konfiguration och planerade permanenta parkeringsdata lagras i JSON.

**Motivering:** Formatet är läsbart, kan hanteras med `System.Text.Json` och uppfyller projektets filkrav.

Grundläggande inläsning och skrivning av parkeringsdata är implementerad; fullständig återställning och automatisk sparning återstår.

## DD-009 – Separata filer för konfiguration och parkeringsdata

**Beslut:** `config.json` innehåller inställningar och prislista, medan `parking-data.json` ska innehålla parkerade fordon.

**Motivering:** Administrativa inställningar och föränderliga parkeringsdata har olika livscykler.

## DD-010 – Spara endast upptagna platser

**Beslut:** I den planerade persistensen ska endast platser med parkerade fordon sparas.

**Motivering:** Tomma parkeringsplatser kan återskapas utifrån konfigurerat garageantal.

**Status:** Validering av sparade platser utan fordon är implementerad; konvertering från ett verkligt `ParkingGarage` till en fil med enbart upptagna platser återstår. Se `parking-data.md` och DD-025.

## DD-011 – UTC för tidpunkter

**Beslut:** `ArrivalTime` registreras och används internt i UTC.

**Motivering:** Beräkningarna ska vara konsekventa oberoende av lokala tidsomställningar.

Tidszon för presentation anges i konfigurationen, med `Europe/Prague` som standard. JSON-inläsningen har kontrollerats med `ArrivalTime.Kind = Utc`; återställning av fordon i domänmodellen med oförändrad tid återstår.

## DD-012 – Samlad JSON-konfiguration med priser först

**Beslut:** Prislista, garagekapacitet, tidszon och fordonsregler finns i `config.json`, med prisuppgifterna tidigt i filen.

**Motivering:** Administratören kan ändra priser och inställningar utan att kompilera om programmet.

Nya filvärden börjar inte automatiskt användas av en redan körande instans. Omläsning hanteras separat.

## DD-013 – Skapa saknad runtime-konfiguration

**Beslut:** Om programmets runtime-`config.json` saknas får en standardkonfiguration skapas.

**Motivering:** Första starten kan ske med dokumenterade standardvärden.

Detta gäller runtime-filen, inte en automatisk omskrivning av projektets ursprungliga konfigurationsfil.

## DD-014 – Avvisa felaktig befintlig konfiguration

**Beslut:** Ogiltig befintlig JSON-konfiguration ska inte tyst skrivas över.

**Motivering:** Användaren ska få ett kontrollerat fel i stället för att förlora sina ändringar.

## DD-015 – Separat prisberäkning

**Beslut:** `ParkingFeeCalculator` hanterar prisregler och avgiftsberäkning, inte `ParkingGarage`.

**Motivering:** Ansvarsfördelningen gör prislogiken oberoende av fordonsplaceringen.

## DD-016 – Prisvärden hålls i kalkylatorinstansen

**Beslut:** Aktiva priser och gratisminuter laddas vid start och kan ändras genom `UpdatePricing()` efter validering.

**Motivering:** Nya priser ska kunna tas i bruk utan att bygga om hela domänmodellen.

Menybaserad omläsning av prislistan återstår för G-versionen. Omläsning av **hela** konfigurationen tillhör VG-kraven.

## DD-017 – Read-only resultat för prisberäkning

**Beslut:** `ParkingFeeCalculator.CalculateParkingFee()` returnerar `ParkingFeeResult`.

**Motivering:** UI får ett strukturerat resultat med parkeringstid, timpris, gratisminuter och avgift utan att innehålla prislogik.

## DD-018 – Checkout samordnas av en separat service

**Beslut:** `CheckoutService` ansvarar för sökning, prisberäkning och borttagning, i den ordningen.

**Motivering:** Fordonet ska inte tas bort innan en korrekt avgift har kunnat beräknas.

**Status:** Domänlogiken har tidigare testats separat. Ett färdigt UI-flöde och koppling till permanent lagring återstår.

## DD-019 – Strukturerat utcheckningsresultat

**Beslut:** `CheckoutService` returnerar `CheckoutResult`.

`CheckoutResult` och `ParkingFeeResult` sparas inte som parkeringsdata.

**Motivering:** Resultaten tillhör det aktuella användarflödet och ska kunna presenteras i UI utan att lagras som permanent garagetillstånd.

---

## DD-020 – Interaktiv garageöversikt med Grid och Panel

**Beslut:** `GarageOverView` använder Spectre.Console `Grid` med individuella `Panel`-objekt i stället för en lång vertikal `Table`.

**Motivering:** En 25-raders tabell tog för mycket höjd och försvårade en överblick utan terminalscrollning.

Ett rutnät med fem kolumner och maximalt fem rader visar 25 parkeringsplatser per sida.

Individuella paneler ger dessutom möjlighet att framhäva hela den markerade parkeringsrutan.

### Genomförande

- `ParkingGarage.ParkingSpots` ordnas efter `SpotNumber`.
- Parkeringsplatserna delas upp i sidor med `Skip()` och `Take()`.
- Antalet sidor beräknas dynamiskt utifrån garagekapaciteten.
- Varje parkeringsplats presenteras i en individuell `Panel`.
- Fem kolumner används för att skapa parkeringskartan.
- Piltangenterna flyttar markeringen inom rutnätet.
- Page Up och Page Down används för sidbyte.
- ESC lämnar garageöversikten.
- Vid sidbyte återställs markeringen till första platsen.
- Markeringens och panelernas färger följer numera `AppTheme` i stället för tidigare fristående färgval som `LightGoldenrod1` och vita ramar.
- Grön, orange och röd status används för ledig, delvis upptagen respektive full parkeringsplats.
- Kapacitetsinformationen hämtas från `AppConfiguration`.
- `Columns` används för att placera parkeringskartan bredvid informationspanelen `SPOT DETAILS`.
- Informationspanelen visar detaljer om vald plats; tidpunkter presenteras i konfigurerad lokal tidszon.

### Arkitektur

Detta är en lösning i presentationslagret.

Garageöversikten ansvarar för presentation och användarinteraktion.

`Vehicle`, `ParkingSpot`, `ParkingGarage` och prislogiken behöver inte ändras för att stödja presentationsvalet.

Parkeringsregler och kapacitetskontroll hanteras fortsatt av domänlagret.

### Kvarstående arbete

- Integrationstesta översikten med faktiskt parkerade fordon, inklusive två MC på samma plats.
- Verifiera statusfärgerna för ledig, delvis upptagen och full plats.
- Kontrollera navigeringens gränsfall och sista sidan vid andra platsantal än multiplar av 25.
- Kontrollera layout vid olika terminalstorlekar.

---

## DD-021 – Gemensamt UI-tema med AppTheme

**Datum:** 2026-10-10

### Beslut

Prague Parking V2 använder en gemensam klass, `AppTheme`, för applikationens visuella tema och återanvändbara presentationsfunktioner.

Temat inspireras av svenska flaggans blå och gula färger.

### Motivering

Tidigare använde olika UI-komponenter separata färgdefinitioner.

Detta riskerade att skapa ett inkonsekvent användargränssnitt och duplicerad kod.

Genom att centralisera färgpaletten och gemensamma panel- och menyfunktioner kan flera vyer använda samma presentationsregler.

### Genomförande

`AppTheme` innehåller bland annat:

- Bakgrundsfärg och återställning av konsolpresentation med `PrepareScreen()` och `RestoreScreen()`.
- Primär och sekundär textfärg.
- Guldaccenter, panelramarnas färger och statusfärger.
- `CreatePanel()` för gemensamt utformade paneler.
- `ShowMenu()` för menyvisning och tangentbordsnavigering.
- Hjälpmetoder för gemensam text- och färgpresentation.

Temat används i huvudmenyn och garageöversikten. `MessageService` hanterar status- och felmeddelanden med samma visuella uttryck.

### Fördelar

- Enhetlig grafisk identitet.
- Enklare underhåll.
- Minskad duplicering av färg- och layoutdefinitioner.
- Gemensamt beteende när fler UI-vyer skapas.

### Begränsningar

Alla terminaler stöder inte samma färgdjup eller ANSI-funktioner.

Terminalkompatibilitet och återställning av bakgrunden måste därför verifieras på olika körmiljöer.

---

## DD-022 – Positionsbaserad och återanvändbar menynavigering

**Datum:** 2026-10-10

### Beslut

Menyvisning och tangentbordsnavigering samlas i `AppTheme.ShowMenu()` i stället för att varje UI-vy implementerar ett eget menysystem.

Huvudmenyn använder den gemensamma funktionen och skickar in menyval, aktuell markering och eventuella texter för presentation.

### Bakgrund

Den första implementationen använde Spectre.Console `SelectionPrompt`.

Därefter utvecklades en anpassad meny med individuella färger och paneler.

När hela konsolen ritades om vid varje tangenttryckning uppstod synligt flimmer.

Spectre.Console Live testades också men skapade komplikationer tillsammans med den anpassade bakgrundsrenderingen.

### Genomförande

Den dokumenterade menylösningen använder:

- `Console.ReadKey()` för inmatning.
- Positionsbaserad konsolrendering med `Console.SetCursorPosition()` där det är tillämpligt.
- En markerad menyposition som kan behållas mellan navigeringstillfällen.
- Uppdatering av tidigare och nytt markerat menyval för att minska onödig omritning.
- `AppTheme.ShowMenu()` som gemensam ingång för huvudmenyn och andra vyer.

Den gamla beskrivningen av en separat menyimplementation i `MainMenu` är därmed inte längre arkitekturens avsikt. Den tidigare dokumenterade reservlösningen med `SelectionPrompt` är inte bekräftad i den aktuella koden och anges därför inte som implementerad.

### Fördelar

- Mindre duplicerad kod mellan menyer.
- Mer enhetlig navigering och presentation.
- Mindre onödig konsolrendering.
- Enklare att lägga till nya funktioner i UI.

### Arkitektur

`MainMenu` ansvarar för att välja och öppna funktioner. `AppTheme` ansvarar för återanvändbar menypresentation.

Parkeringsreglerna ligger fortsatt i domänlagret.

### Fortsatt arbete

- Verifiera tangentbordsnavigering och ESC-beteende i samtliga vyer.
- Kontrollera konsolbakgrundens återställning.
- Testa vid ändrad terminalstorlek och på andra terminaler.
- Säkerställa att färdiga UI-funktioner återgår korrekt till huvudmenyn.

---

## DD-023 – Centraliserade statusmeddelanden

**Datum:** 2026-10-10

**Beslut:** `MessageService` används för återkommande meddelandetyper såsom fel, varning, information och lyckade åtgärder. Visuell utformning delas med `AppTheme`.

**Motivering:** UI-vyer ska inte duplicera panelkonstruktion, färgval och väntan på användarinmatning.

**Genomförande:** Gemensamma meddelandemetoder används eller återanvänds när nya vyer införs. Integration och enhetlig användning i alla vyer behöver fortfarande verifieras.

**Viktig regel inför persistensen:** En parkering, flytt eller utcheckning ska inte presenteras som varaktigt sparad om skrivningen till `parking-data.json` misslyckas.

---

## DD-024 – Unika identifierare i hela parkeringshuset

**Datum:** 2026-10-10

**Beslut:** `ParkingGarage` kontrollerar om ett fordon redan finns innan nyparkering. Fordonsidentifierare jämförs utan skillnad mellan stora och små bokstäver.

**Motivering:** Exempelvis `ABC123` och `abc123` ska inte kunna registreras som två separata fordon. Samma regel ska även tillämpas när sparad data återställs.

**Status:** Kontroll av dubbletter och skiftlägesokänslig jämförelse har tidigare testats i domänmodellen. `ValidateParkingData()` har nu `HashSet<string>(StringComparer.OrdinalIgnoreCase)` för inlästa fordonsidentifierare; specifika negativa JSON-test med dubbletter återstår.

---

## DD-025 – Fastställd struktur och säker återställning för parking-data.json

**Datum:** 2026-10-10

**Beslut:** Permanent parkeringsdata ska följa den redan beslutade strukturen i `parking-data.md`. Den får inte ersättas av ett platt JSON-format eller få nya obligatoriska fält utan separat beslut.

**Motivering:** Platsnumret, samlingen fordon och fordonens ursprungliga ankomsttider behöver bevaras för att garaget ska kunna återskapas korrekt efter omstart.

### Beslutad struktur för G-versionen

```json
{
  "ParkingSpots": [
    {
      "SpotNumber": 17,
      "Vehicles": [
        {
          "Identifier": "ABC123",
          "VehicleType": "Car",
          "ArrivalTime": "2026-10-05T14:30:00Z"
        }
      ]
    }
  ]
}
```

### Regler för den planerade persistensen

- Endast upptagna parkeringsplatser sparas.
- Tomma parkeringsplatser återskapas enligt `config.json`.
- Varje fordon återställs till samma plats med samma identifierare, fordonstyp och UTC-ankomsttid.
- Återställning ska kontrollera platsnummer, fordonstyp, kapacitet och dubbletter innan ett inläst tillstånd accepteras.
- En befintlig felaktig eller ofullständig JSON-fil får inte automatiskt ersättas med en tom fil.
- `Save()` skriver till en `.tmp`-fil i samma katalog och använder `File.Move()` vid första sparningen eller `File.Replace()` när originalfilen finns. Fullständig verifiering av filersättning och felhantering återstår.
- Dataåtkomst och serialisering ska ske utanför UI och hållas separerade från domänens parkeringsregler.
- Fordonsincheckning, utcheckning och förflyttning ska omfattas av en konsekvent sparrutin; användaren ska få ett korrekt fel om lagring misslyckas.

**Status 2026-10-11:** `ParkingDataService`, de tre DTO-klasserna, validering av grundstrukturen, unika platsnummer/fordonsidentifierare, G-typerna och UTC-tid finns. Inläsning av giltig data och avvisning av typen `Truck` har testats manuellt. Serialiserad JSON med `WriteIndented = true` har granskats. `Save()` har följts till `finally`, men slutlig filersättning, återställning i `ParkingGarage` och automatisk sparning är inte verifierade. Konstruktorsöverlagringar för att återskapa `Car` och `MC` med befintlig UTC-ankomsttid ska bekräftas i källkoden vid integrationen.

**Ansvarsgräns:** `ParkingDataService` validerar dokumentets struktur och interna värden. Att sparade fordon ryms på platsnummer och enligt kapacitetsreglerna i aktuell `AppConfiguration` valideras vid återställning till domänmodellen, inte genom att Data-projektet får ett beroende på domänklasser. Konsekvent behandling av skrivfel och skydd mot överskrivning av befintlig korrupt data ska verifieras innan funktionaliteten används i färdiga UI-flöden.

---

## DD-026 – VG-utökning med Bicycle och Bus utan att ändra G-formatet i förtid

**Datum:** 2026-10-10

**Beslut:** Prague Parking 2.1 ska utgå från den befintliga domän- och lagringsmodellen men införa storleksbaserad kapacitet och stöd för fordon som kan uppta flera sammanhängande platser.

**Motivering:** Nya fordonstyper ska kunna införas utan att en fungerande G-version behöver byggas om i onödan eller dess befintliga parkeringsdata förloras.

### Regler för VG-versionen

- `Bicycle` = 1, `MC` = 2, `Car` = 4 och `Bus` = 16 storleksenheter.
- En normal parkeringsplats motsvarar 4 storleksenheter.
- En buss behöver fyra sammanhängande parkeringsplatser.
- **Nyparkering av buss:** De fyra platserna ska ligga inom parkeringsplats 1–50.
- **Förflyttning av buss:** En redan parkerad buss får flyttas till fyra sammanhängande lediga platser även efter plats 50.
- Bussens identifierare och ursprungliga UTC-ankomsttid ska bevaras vid flytt.
- Minst två interfaces och fullständig omläsning av konfigurationen ingår också i VG-arbetet.

### Följder för domän och lagring

Nyparkering och flytt av buss kan inte använda exakt samma placeringsbegränsning. Därför ska dessa regler vara åtskilda i den framtida domänlogiken.

En buss får inte räknas som fyra oberoende fordon. En framtida utökning av JSON-modellen måste kunna representera kopplingen mellan en buss och dess fyra platser, men den exakta representationen är **inte beslutad**. G-versionens fastställda format gäller tills vidare.

**Status:** Planerat för VG. Ingen buss-, cykel- eller flerplatslogik anges som implementerad.
