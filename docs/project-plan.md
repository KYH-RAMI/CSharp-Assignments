# Prague Parking V2 – Projektplan

**Uppdaterad:** 2026-10-10

**Projekt:** Prague Parking V2  
**Utvecklare:** Rami Issa  
**Utvecklingsmiljö:** Visual Studio Community 2026  
**Teknik:** C# / .NET 10 / Spectre.Console

---

## Projektmål

Utveckla ett objektorienterat parkeringssystem som uppfyller G-kraven för Prague Parking 2.0.

Systemet ska innehålla:

- Fordonshantering.
- Parkeringsplatser.
- Kapacitetskontroll.
- Prisberäkning.
- Incheckning och utcheckning.
- Flytt och sökning.
- JSON-konfiguration.
- Permanent datalagring.
- Interaktivt användargränssnitt.

VG-versionen utvecklas separat när G-versionen är stabil.

---

## Projektfaser

| Fas | Arbetsområde | Status |
|---|---|---|
| 1 | Kravanalys | Genomförd |
| 2 | Solution och projektstruktur | Genomförd |
| 3 | Domänmodell | Genomförd |
| 4 | JSON-konfiguration | Genomförd |
| 5 | Parkering och sökning i domänlagret | Genomförd |
| 6 | Prisberäkning | Genomförd |
| 7 | CheckoutService | Genomförd |
| 8 | Spectre.Console och garageöversikt | Pågår |
| 9 | Main Menu och UI-flöden | Pågår |
| 10 | Parkeringspersistens | Ej påbörjad |
| 11 | Administrationsfunktioner | Ej påbörjad |
| 12 | Integrationstester och dokumentation | Pågår |
| 13 | Prague Parking 2.1 / VG | Ej påbörjad |

---

## Milstolpe 1 – Arkitektur och domänmodell

**Status: Genomförd**

Domänmodellen har implementerats med:

- Vehicle.
- Car.
- MC.
- ParkingSpot.
- ParkingGarage.
- ParkingFeeCalculator.
- ParkingFeeResult.
- CheckoutService.
- CheckoutResult.

Grundläggande funktionalitet har testats manuellt.

---

## Milstolpe 2 – Konfiguration

**Status: Genomförd**

ConfigurationService har implementerats.

Funktionalitet:

- JSON-inläsning.
- Standardkonfiguration.
- Konfigurationsvalidering.
- Garagekapacitet.
- Prisinställningar.
- Tidszon.
- Fordonskapacitet.

---

## Milstolpe 3 – Prisberäkning och Checkout

**Status: Genomförd på domännivå**

ParkingFeeCalculator och CheckoutService har implementerats.

Funktionerna är testade separat från det slutliga användargränssnittet.

Fullständig UI-integration återstår.

---

## Milstolpe 4 – User Interface

**Status: Pågår**

### Main Menu

En interaktiv huvudmeny har implementerats.

Funktioner:

- Centrering i terminalfönstret.
- Tangentbordsnavigering.
- Markering av aktivt menyval.
- Återgång från Garage Overview.
- Exit-funktion.
- Gemensamt färgtema.
- Anpassad rendering för minskat flimmer.

### AppTheme

En gemensam UI-klass har skapats för:

- Färgpalett.
- Bakgrund.
- Textfärger.
- Panelramar.
- Guldfärgade accenter.
- Konsolrendering.

Temat är inspirerat av Sveriges flagga.

### Garage Overview

Implementerat eller påbörjat:

- Parkeringskarta med Grid och Panel.
- 25 parkeringsplatser per sida.
- Fem kolumner och fem rader.
- Dynamisk pagination.
- Navigering med fyra piltangenter.
- Page Up och Page Down.
- Visuell markering.
- Färgkodning av parkeringsstatus.
- Informationspanel för markerad plats.
- Anpassad footer.

### Spot Details

Utveckling pågår av en informationspanel som ska visa:

- Parkeringsplatsnummer.
- Status.
- Beläggning.
- Samtliga parkerade fordon.
- Fordonstyp.
- Identifierare.
- Ankomsttid.
- Duration.

Senaste implementationen behöver testas med parkerade fordon.

---

## Milstolpe 5 – UI-funktionalitet

**Status: Pågår**

Huvudmenyn finns.

Nästa steg är att koppla befintliga domänfunktioner till riktiga användarflöden.

Prioriterad arbetsordning:

1. Park Vehicle.
2. Check Out Vehicle.
3. Find Vehicle.
4. Move Vehicle.
5. Administration.

---

## Milstolpe 6 – Persistens

**Status: Ej påbörjad**

Parkeringsinformation ska sparas i `parking-data.json`.

Data ska kunna återställas efter omstart.

Filen ska uppdateras efter:

- Incheckning.
- Utcheckning.
- Flytt av fordon.

Persistens ska hanteras genom Data-projektet.

---

## Milstolpe 7 – Administration

**Status: Ej påbörjad**

Planerad funktionalitet:

- Omläsning av prislista.
- Validering av uppdaterade priser.
- Tillämpning av nya prisregler under körning.

---

## Milstolpe 8 – Testning

**Status: Pågår**

Genomförda manuella tester finns dokumenterade i LOGGBOK.md.

Återstående tester omfattar:

- Fullständiga incheckningsflöden.
- Fullständiga utcheckningsflöden.
- Flyttfunktion.
- JSON-persistens.
- Omläsning av priser.
- Spot Details med parkerade fordon.
- Terminalstorlekar och rendering.
- Navigationens gränsfall.

---

## Milstolpe 9 – Prague Parking 2.1

**Status: Ej påbörjad**

Planerade VG-funktioner:

- Separat projekt.
- Minst två interfaces.
- Bicycle.
- Bus.
- Storleksbaserad parkeringskapacitet.
- Fyra sammanhängande parkeringsplatser för Bus.
- Nya parkeringspriser.
- Dynamisk konfigurationsomläsning.
- Validering mot befintliga parkerade fordon.
- Personlig reflektion.

---

## Närmaste utvecklingsmål

### Prioritet 1

Verifiera den senaste Spot Details-implementationen.

### Prioritet 2

Implementera Park Vehicle i användargränssnittet.

### Prioritet 3

Integrera CheckoutService med användargränssnittet.

### Prioritet 4

Implementera Find Vehicle och Move Vehicle.

### Prioritet 5

Implementera permanent lagring av parkeringsinformation.

### Prioritet 6

Genomföra integrationstester och färdigställa G-versionen.

---

## Risker

### Console Rendering

Olika terminaler kan hantera ANSI-sekvenser och färger på olika sätt.

Applikationen behöver verifieras i fler terminalmiljöer.

### Data Persistence

Parkeringsdata riskerar att gå förlorade om permanent lagring inte implementeras korrekt.

### Configuration

Ogiltig konfiguration ska inte påverka en fungerande installation.

### Scope Management

G-versionen ska färdigställas innan VG-funktionalitet introduceras.

---

## Dokumentation

Utvecklingsarbetet dokumenteras löpande i:

- LOGGBOK.md
- requirements.md
- design-decisions.md
- class-diagram.md
- configuration.md
- parking-data.md

Dokumentationen ska spegla faktisk implementation och verifierade tester.