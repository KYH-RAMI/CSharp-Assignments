# Prague Parking V2 – Designbeslut

**Uppdaterad:** 2026-10-10

## DD-001 – Separation mellan UI, domän och dataåtkomst

**Beslut:** Presentation, affärsregler och dataåtkomst hålls separerade.

**Motivering:** Gränssnittet kan ändras utan att fordon och parkeringsregler påverkas.

JSON-hantering placeras i det separata Data-projektet.

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

## DD-005 – Fordon lagrar inte parkeringsplatsnummer

**Beslut:** `Vehicle` lagrar inte `SpotNumber`.

**Motivering:** Fordonets placering avgörs av vilken `ParkingSpot` som innehåller objektet.

Detta undviker dubbellagrad information.

## DD-006 – Garageklassen samordnar placering och sökning

**Beslut:** `ParkingGarage` skapar och söker bland platserna, hittar första lämpliga platsen, parkerar och tar bort fordon.

**Motivering:** En parkeringsplats hanterar sina lokala regler medan garaget hanterar hela samlingen.

## DD-007 – Kapacitetsmodell i G-versionen

**Beslut:** Antalet fordon per parkeringsplats konfigureras separat för `Car` och `MC` via `VehiclesPerSpot`.

**Motivering:** G-versionen kan hantera en bil eller två MC per plats utan att införa VG-versionens generella storleksenheter i förtid.

## DD-008 – JSON som filformat

**Beslut:** Konfiguration och framtida parkeringsdata lagras i JSON.

**Motivering:** Formatet är läsbart, kan hanteras med `System.Text.Json` och uppfyller projektets filkrav.

## DD-009 – Separata filer för konfiguration och parkeringsdata

**Beslut:** `config.json` innehåller inställningar och prislista, medan `parking-data.json` ska innehålla parkerade fordon.

**Motivering:** Administrativa inställningar och föränderliga parkeringsdata har olika livscykler.

## DD-010 – Spara endast upptagna platser

**Beslut:** I den planerade persistensen ska endast platser med parkerade fordon sparas.

**Motivering:** Tomma parkeringsplatser kan återskapas utifrån konfigurerat garageantal.

**Status:** Ännu inte implementerat.

## DD-011 – UTC för tidpunkter

**Beslut:** `ArrivalTime` registreras och används internt i UTC.

**Motivering:** Beräkningarna ska vara konsekventa oberoende av lokala tidsomställningar.

Tidszon för presentation anges i konfigurationen.

## DD-012 – Samlad JSON-konfiguration med priser först

**Beslut:** Prislista, garagekapacitet, tidszon och fordonsregler finns i `config.json`, med prisuppgifterna tidigt i filen.

**Motivering:** Administratören kan ändra priser och inställningar utan att kompilera om programmet.

## DD-013 – Skapa saknad runtime-konfiguration

**Beslut:** Om programmets runtime-`config.json` saknas får en standardkonfiguration skapas.

**Motivering:** Första starten kan ske med dokumenterade standardvärden.

## DD-014 – Avvisa felaktig befintlig konfiguration

**Beslut:** Ogiltig befintlig JSON-konfiguration ska inte tyst skrivas över.

**Motivering:** Användaren ska få ett kontrollerat fel i stället för att förlora sina ändringar.

## DD-015 – Separat prisberäkning

**Beslut:** `ParkingFeeCalculator` hanterar prisregler och avgiftsberäkning, inte `ParkingGarage`.

**Motivering:** Ansvarsfördelningen gör prislogiken oberoende av fordonsplaceringen.

## DD-016 – Prisvärden hålls i kalkylatorinstansen

**Beslut:** Aktiva priser och gratisminuter laddas vid start och kan ändras genom `UpdatePricing()` efter validering.

**Motivering:** Nya priser ska kunna tas i bruk utan att bygga om hela domänmodellen.

Menybaserad filomläsning återstår.

## DD-017 – Read-only resultat för prisberäkning

**Beslut:** `ParkingFeeCalculator.CalculateParkingFee()` returnerar `ParkingFeeResult`.

**Motivering:** UI får ett strukturerat resultat med parkeringstid, timpris, gratisminuter och avgift utan att innehålla prislogik.

## DD-018 – Checkout samordnas av en separat service

**Beslut:** `CheckoutService` ansvarar för sökning, prisberäkning och borttagning, i den ordningen.

**Motivering:** Fordonet ska inte tas bort innan en korrekt avgift kunnat beräknas.

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

- `ParkingGarage.ParkingSpots` sorteras efter `SpotNumber`.
- Parkeringsplatserna delas upp i sidor med `Skip()` och `Take()`.
- Antalet sidor beräknas dynamiskt utifrån garagekapaciteten.
- Varje parkeringsplats presenteras i en individuell `Panel`.
- Fem kolumner används för att skapa parkeringskartan.
- Piltangenterna flyttar markeringen inom rutnätet.
- Page Up och Page Down används för sidbyte.
- ESC lämnar garageöversikten.
- Vid sidbyte återställs markeringen till första platsen.
- `LightGoldenrod1` används för markeringen.
- Omarkerade parkeringsplatser använder vita ramar.
- Grön, orange och röd status används för ledig, delvis upptagen och full parkeringsplats.
- Kapacitetsinformationen hämtas från `AppConfiguration`.
- `Columns` används för att placera parkeringskartan bredvid informationspanelen `SPOT DETAILS`.

### Arkitektur

Detta är en lösning i presentationslagret.

Garageöversikten ansvarar för presentation och användarinteraktion.

`Vehicle`, `ParkingSpot`, `ParkingGarage` och prislogiken behöver inte ändras för att stödja presentationsvalet.

Parkeringsregler och kapacitetskontroll hanteras fortsatt av domänlagret.

### Kvarstående arbete

- Utökade platsdetaljer.
- Verifiering med parkerade fordon.
- Kontroll av navigeringsgränser.
- Kontroll av layout vid olika terminalstorlekar.

---

## DD-021 – Gemensamt UI-tema med AppTheme

**Datum:** 2026-10-10

### Beslut

Prague Parking V2 använder en gemensam klass, `AppTheme`, för att definiera applikationens visuella färgtema.

Temat inspireras av svenska flaggans blå och gula färger.

### Motivering

Tidigare använde olika UI-komponenter separata färgdefinitioner.

Detta riskerade att skapa ett inkonsekvent användargränssnitt.

Genom att centralisera färgpaletten kan flera komponenter använda samma färger.

### Genomförande

AppTheme innehåller:

- Bakgrundsfärg.
- Primär textfärg.
- Sekundär textfärg.
- Guldaccenter.
- Panelramarnas färg.
- Funktioner för konsolpresentation.

Färgtemat används i huvudmenyn.

Garageöversikten anpassas successivt till samma tema.

### Fördelar

- Enhetlig grafisk identitet.
- Enklare underhåll.
- Minskad duplicering av färgdefinitioner.
- Möjlighet att ändra tema centralt.

### Begränsningar

Alla terminaler stöder inte samma färgdjup eller ANSI-funktioner.

Terminalkompatibilitet måste därför verifieras.

---

## DD-022 – Positionsbaserad huvudmeny med selektiv rendering

**Datum:** 2026-10-10

### Beslut

Huvudmenyn använder positionsbaserad konsolrendering för att styra placering och navigering.

I stället för att rita om hela huvudmenyn vid varje tangenttryckning uppdateras endast de menyval vars markeringsstatus förändras.

### Bakgrund

Den första implementationen använde Spectre.Console SelectionPrompt.

Därefter utvecklades en anpassad meny med individuella färger och paneler.

När hela konsolen ritades om vid varje tangenttryckning uppstod synligt flimmer.

Spectre.Console Live testades för att minska detta.

Live-renderingen minskade flimret men skapade komplikationer tillsammans med den anpassade bakgrundsrenderingen.

### Genomförande

Huvudmenyn använder:

- Console.ReadKey().
- Console.SetCursorPosition().
- En markerad menyposition.
- Separata metoder för menykomponenternas presentation.
- Uppdatering av tidigare och nytt markerat menyval.

### Fördelar

- Mindre onödig konsolrendering.
- Tydligare kontroll över menyplacering.
- Mjukare navigering.
- Möjlighet att centrera menyn.
- Sammanhängande användargränssnitt.

### Reservlösning

Om terminalfönstret är för litet används en enklare meny baserad på Spectre.Console SelectionPrompt.

### Arkitektur

MainMenu tillhör UI-lagret.

Klassen ansvarar för:

- Presentation.
- Tangentbordsnavigering.
- Val av funktion.
- Öppning av andra UI-vyer.

Parkeringsreglerna ligger fortsatt i domänlagret.

### Fortsatt arbete

- Verifiera terminalkompatibilitet.
- Kontrollera konsolbakgrundens återställning.
- Testa navigering vid ändrad terminalstorlek.
- Säkerställa att UI-funktioner återgår korrekt till huvudmenyn.