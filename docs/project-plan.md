# Prague Parking V2 – Projektplan

**Uppdaterad:** 2026-10-10

**Projekt:** Prague Parking V2  
**Utvecklare:** Rami Issa  
**Utvecklingsmiljö:** Visual Studio Community 2026  
**Teknik:** C# / .NET 10 / Spectre.Console

**Arbetsordning:** Prague Parking 2.0 (G) färdigställs och verifieras innan Prague Parking 2.1 (VG) implementeras.

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

VG-versionen utvecklas separat när G-versionen är stabil. Dokumenterade VG-behov beaktas i arkitekturen, men implementeras inte i förtid.

**Statusprincip:** *Implementerad* betyder att kod har tagits fram. *Testad* eller *verifierad* används endast när ett motsvarande test faktiskt har genomförts. Planerade funktioner räknas inte som färdiga. Statusen nedan bygger på det senast redovisade utvecklingsläget; slutlig avstämning mot hela källkodsträdet återstår.

---

## Projektfaser

| Fas | Arbetsområde | Status |
|---|---|---|
| 1 | Kravanalys | Genomförd; kravmatrisen hålls uppdaterad |
| 2 | Solution och projektstruktur | Genomförd |
| 3 | Domänmodell | Implementerad; centrala delar manuellt testade |
| 4 | JSON-konfiguration | Implementerad; grundfall och validering manuellt testade |
| 5 | Parkering och sökning i domänlagret | Implementerad; grundfall manuellt testade |
| 6 | Prisberäkning | Implementerad; manuellt testad |
| 7 | CheckoutService | Implementerad; manuellt testad på domännivå |
| 8 | Spectre.Console och garageöversikt | Implementerad i huvudsak; utökad verifiering återstår |
| 9 | Main Menu och UI-flöden | Pågår; `ParkVehicleView` framtagen, integrationstest återstår |
| 10 | Parkeringspersistens | Pågår; datamodeller samt grundläggande JSON-inläsning och sparning implementerade |
| 11 | Administrationsfunktioner | Ej påbörjad i UI |
| 12 | Integrationstester och dokumentation | Pågår; fullständig G-verifiering återstår |
| 13 | Prague Parking 2.1 / VG | Ej påbörjad |

---

## Milstolpe 1 – Arkitektur och domänmodell

**Status: Genomförd på implementationsnivå**

Domänmodellen har implementerats med:

- `Vehicle`.
- `Car`.
- `MC`.
- `ParkingSpot`.
- `ParkingGarage`.
- `ParkingFeeCalculator`.
- `ParkingFeeResult`.
- `CheckoutService`.
- `CheckoutResult`.

Grundläggande parkering, sökning, borttagning, dubblettkontroll, prisberäkning och utcheckning har tidigare testats manuellt från programkoden. Det är inte samma sak som att motsvarande UI-flöden eller permanent datalagring är färdigtestade.

`Vehicle` innehåller stöd för en befintlig UTC-ankomsttid. Konstruktorsöverlagringar i `Car` och `MC` för återställning från JSON behöver kontrolleras eller färdigställas i samband med persistensen.

---

## Milstolpe 2 – Konfiguration

**Status: Genomförd för grundläggande inläsning och validering**

`ConfigurationService` har implementerats.

Funktionalitet:

- JSON-inläsning.
- Standardkonfiguration när runtime-filen saknas.
- Validering av befintlig konfiguration.
- Garagekapacitet.
- Prisinställningar.
- Tidszon.
- Fordonskapacitet via `VehiclesPerSpot`.

En befintlig ogiltig konfigurationsfil får inte skrivas över automatiskt. Omläsning av prislistan via administrationsmenyn återstår.

---

## Milstolpe 3 – Prisberäkning och Checkout

**Status: Genomförd på domännivå**

`ParkingFeeCalculator`, `ParkingFeeResult`, `CheckoutService` och `CheckoutResult` har implementerats.

- Timpriser för `Car` och `MC` samt gratisminuter hämtas från konfigurationen.
- Avgift beräknas enligt reglerna för påbörjade timmar.
- `UpdatePricing()` kan validera och ersätta aktiva prisvärden.
- Utcheckning samordnar sökning, prisberäkning och borttagning.

Funktionerna har testats separat från det slutliga användargränssnittet. **UI-integration och sparning efter utcheckning återstår.**

---

## Milstolpe 4 – User Interface

**Status: Pågår**

### Main Menu

En interaktiv huvudmeny har implementerats med:

- Centrering i terminalfönstret.
- Tangentbordsnavigering och markering av aktivt menyval.
- Återgång från `GarageOverView`.
- Exit-funktion.
- Gemensamt färgtema.
- Positionsbaserad uppdatering av menyval för att minska flimmer.

Återanvändbar menynavigering samlas i `AppTheme.ShowMenu()` i stället för att varje vy ska skapa en egen lösning.

### AppTheme

`AppTheme` samlar:

- Blå och gula temafärger.
- Bakgrund och textfärger.
- Panelramar och gemensamma panelinställningar.
- Återanvändbar menypresentation.
- Hjälpmetoder för konsolens visning och återställning.

Temat används av huvudmenyn och garageöversikten. Kompatibilitet med andra terminaler behöver fortfarande testas.

### MessageService

`MessageService` används för återanvändbara fel-, varnings-, informations- och bekräftelsemeddelanden. Enhetlig användning i samtliga kommande UI-flöden ska kontrolleras.

**Viktig regel:** Ett lyckat resultat får inte presenteras som permanent sparat om den kommande JSON-skrivningen misslyckas.

### Garage Overview

Funktionalitet som har implementerats i vy-koden:

- Parkeringskarta med Spectre.Console `Grid` och `Panel`.
- 25 parkeringsplatser per sida, fördelade på fem kolumner och fem rader.
- Dynamisk sidindelning utifrån garagets storlek.
- Navigering med piltangenter, Page Up, Page Down och ESC.
- Visuell markering av vald parkeringsplats.
- Färgkodning för ledig, delvis upptagen och full plats.
- Informationspanel och sidfot.

Utökade tester med parkerade fordon, två MC på samma plats, sidgränser och olika terminalstorlekar återstår.

### Spot Details

Informationspanelen är framtagen för att visa:

- Parkeringsplatsnummer.
- Status och beläggning.
- Parkerade fordon.
- Fordonstyp och identifierare.
- Lokal presentation av ankomsttid.
- Parkeringstid.

Den senaste implementationen behöver integrationstestas med faktiskt parkerade fordon. Dokumentet markerar därför inte den delen som slutverifierad.

---

## Milstolpe 5 – UI-funktionalitet

**Status: Pågår**

Huvudmenyn är implementerad. `ParkVehicleView` har tagits fram, men dess integration med den aktuella källkoden och den slutliga användarupplevelsen är ännu inte bekräftad som testad.

Följande användarflöden återstår att färdigställa och verifiera:

1. **Park Vehicle:** Koppla samman fordonsval, identifierare, kapacitetskontroll, parkering och säker JSON-sparning.
2. **Check Out Vehicle:** Använd `CheckoutService`, visa avgift och spara borttagningen.
3. **Move Vehicle:** Flytta ett fordon utan att ändra dess ursprungliga ankomsttid och spara den nya placeringen.
4. **Find Vehicle:** Visa rätt fordon och parkeringsplats.
5. **Administration:** Läs om prislistan, validera den och aktivera nya priser utan omstart.

Den praktiska utvecklingsordningen anpassas så att persistensen kommer på plats **innan incheckning och utcheckning betraktas som färdiga UI-flöden**.

---

## Milstolpe 6 – Persistens

**Status: Pågår – grundläggande implementation färdig, validering och integration återstår**

Parkeringsinformation ska sparas i `parking-data.json` via det separata Data-projektet. Den överenskomna strukturen i `parking-data.md` gäller för Prague Parking 2.0:

```text
ParkingSpots
    └── SpotNumber
        └── Vehicles
            ├── Identifier
            ├── VehicleType
            └── ArrivalTime (UTC)
```

Regler:

- Endast upptagna parkeringsplatser sparas.
- Tomma platser skapas utifrån `config.json` vid uppstart.
- Varje fordon återställs till **samma parkeringsplats** med **samma identifierare, fordonstyp och UTC-ankomsttid**.
- Filen uppdateras efter lyckad incheckning, utcheckning och flytt.
- Inläst data valideras, bland annat avseende dubbletter, platsnummer, kapacitet och giltiga fordonstyper.
- Befintlig felaktig JSON ska inte tyst skrivas över.
- Skrivfel får inte leda till falskt framgångsmeddelande eller förlorat befintligt datainnehåll.
- Filsökvägar får inte vara hårdkodade till en utvecklingsdator.

Den planerade lagringen ska kunna vidareutvecklas för `Bicycle` och `Bus`, men **inga nya obligatoriska VG-fält läggs till i G-formatet nu**. Lösningen för en buss som upptar fyra platser fastställs under VG-implementationen.

**Nästa tekniska steg:** Gå igenom aktuella `ParkingSpot.cs`, `Program.cs` och `ConfigurationService.cs` samt projektreferenserna, och implementera laddning, validering, återställning och säker sparning utan att skapa cirkulära beroenden.

### Genomfört 2026-10-10

- Skapat separat mapp för persistens i Data-projektet.
- Implementerat `ParkingDataDocument`, `ParkingSpotData` och `VehicleData`.
- Implementerat `ParkingDataService`.
- Implementerat kontroll av datafilens existens.
- Implementerat grundläggande JSON-inläsning och deserialisering.
- Implementerat JSON-serialisering och sparning.
- Implementerat temporär filskrivning med `File.Move()` och `File.Replace()`.
- Implementerat grundläggande innehållsvalidering.
- Implementerat `try/finally` för städning av temporära filer.

### Återstående arbete

- Fullständig validering av parkeringsdata.
- Skydd mot överskrivning av redan korrupt data.
- Verifiering av filskrivning och felhantering.
- Återställning av fordon till rätt parkeringsplats.
- Integration med `Program.cs`.
- Automatisk sparning efter incheckning, utcheckning och förflyttning.
- Integrationstester av hela persistensflödet.
---

## Milstolpe 7 – Administration

**Status: Ej påbörjad i UI**

Planerad funktionalitet för G:

- Omläsning av prislistan från `config.json`.
- Validering av samtliga nya prisvärden.
- Tillämpning via befintliga `ParkingFeeCalculator.UpdatePricing()`.
- Bibehållna tidigare giltiga värden om omläsningen misslyckas.

**VG-utökning:** Omläsning av *hela* konfigurationen under körning, inklusive validering mot redan parkerade fordon. Detta ska inte förväxlas med G-versionens prisomläsning.

---

## Milstolpe 8 – Testning

**Status: Pågår**

Tidigare manuella domäntester finns noterade i `LOGGBOK.md`. De utgör inte en fullständig end-to-end-verifiering.

Återstående tester omfattar:

- Incheckning via användargränssnittet.
- Utcheckning med korrekt pris och sparad borttagning.
- Flytt och sökning av parkerade fordon.
- Dubbelregistrering med olika versaler/gemener.
- Kapacitetsgränser för `Car` och `MC`.
- Återstart med oförändrad parkeringsplats och UTC-ankomsttid.
- Felaktig JSON, ogiltiga platsnummer och simulerade skrivfel.
- Omläsning av prislistan och avvisning av ogiltiga priser.
- Spot Details med parkerade fordon, inklusive två MC på samma plats.
- Navigeringens gränsfall, färger och varierande terminalstorlekar.
- Körning utan hårdkodade sökvägar på en annan dator.

Slutlig dokumentation och status i `requirements.md` uppdateras utifrån faktiskt genomförda tester.

---

## Milstolpe 9 – Prague Parking 2.1

**Status: Ej påbörjad**

Planerade VG-funktioner:

- Separat projekt för vidareutvecklingen enligt projektplanen.
- Minst två interfaces.
- `Bicycle` och `Bus`.
- Storleksbaserad parkeringskapacitet: Bicycle = 1, MC = 2, Car = 4, Bus = 16; en normal plats = 4 enheter.
- **Nyparkering av Bus:** Fyra sammanhängande lediga parkeringsplatser inom plats 1–50.
- **Flytt av Bus:** Fyra sammanhängande lediga platser även längre ner i garaget, utanför plats 50.
- Bibehållen bussidentifierare och ankomsttid vid flytt.
- Timpriser: Bicycle 5 CZK och Bus 80 CZK.
- Fordonstyper och storlekar konfigureras enligt VG-kraven.
- Omläsning av hela konfigurationen med säker validering mot aktuellt garagetillstånd.
- Personlig reflektion enligt uppgiftskraven.

Bussens framtida representation i `parking-data.json` är **inte beslutad**. Den ska hantera fyra upptagna platser utan att behandla en buss som fyra olika fordon, samtidigt som befintlig G-data kan läsas.

---

## Närmaste utvecklingsmål

### Prioritet 1

Färdigställa och testa `ParkingDataService`, inklusive
validering av parkeringsplatser och fordon, skydd mot korrupt
data samt korrekt hantering av filfel.

Därefter integreras inläsning och återställning vid programstart.
### Prioritet 2

Integrera och testa `ParkVehicleView` med automatisk sparning. Kontrollera samtidigt `GarageOverView` och `Spot Details` med verkligt parkerade fordon.

### Prioritet 3

Implementera och integrationstesta utcheckningsflödet med `CheckoutService` och permanent uppdatering av parkeringsdata.

### Prioritet 4

Implementera och testa `Move Vehicle` och `Find Vehicle`, inklusive bibehållen ankomsttid vid flytt.

### Prioritet 5

Implementera administrationsmenyns omläsning av prislistan och hantering av ogiltiga ändringar.

### Prioritet 6

Genomföra fullständig G-verifiering, stämma av `requirements.md` och dokumentera verifierade resultat i `LOGGBOK.md`.

### Prioritet 7

Bygga vidare till VG och därefter förbereda distribution och frivillig betatestning när applikationen är stabil.

---

## Risker

### Console Rendering

Olika terminaler kan hantera ANSI-sekvenser, färger, tangentbordsinmatning och fönsterstorlekar olika. Applikationen ska verifieras i fler terminalmiljöer.

### Data Persistence

Parkeringsdata kan gå förlorade vid felaktig filskrivning eller återställning. Särskild försiktighet krävs vid uppstart, skrivfel och samtidiga uppdateringar i en eventuell framtida fleranvändarmiljö.

### Configuration

Ogiltig befintlig konfiguration får inte ersättas automatiskt. Ändringar under körning ska inte lämna prisreglerna i ett delvis uppdaterat tillstånd.

### Scope Management

G-versionen ska färdigställas före VG. Publicering, Docker, Cloudflare och fleranvändarstöd är inte en del av den aktuella G-implementationen.

### Korrekt projektstatus

Att en klass eller vy har kodats innebär inte att den är integrationstestad. Dokumentation och kravmatris får inte markera funktioner som verifierade utan genomförda tester.

---

## Dokumentation

Utvecklingsarbetet dokumenteras löpande i:

- `LOGGBOK.md` – datum, genomfört arbete, tester, problem och nästa steg.
- `requirements.md` – krav, tolkning, berörd komponent och faktisk status.
- `design-decisions.md` – motiverade och numrerade arkitekturbeslut.
- `class-diagram.md` – implementerad domänmodell och avgränsningar.
- `configuration.md` – konfigurationsformat och regler för prisomläsning.
- `parking-data.md` – fastställt G-format och regler för planerad persistens.
- `README.md` – projektöversikt och instruktioner för att köra applikationen.

Dokumenten ska beskriva **samma utvecklingsläge**. Historiska aktiviteter redovisas i loggboken; framtida arbete ska inte framställas som genomfört. Efter avslutad dokumentationsgenomgång jämförs filerna och sammanställs i en gemensam Git-commit. **Ingen sådan commit är genomförd inom denna dokumentationsgenomgång ännu.**

---

## Framtida distribution och betatestning

**Status: Idé och planering; inte del av G-kravens aktuella implementation**

När G och VG är stabila kan en körbar Windows-version publiceras för test utan att klasskamraterna behöver bygga källkoden. Det kräver bland annat kontroll av filvägar, konfigurationsfiler och körning på en annan dator.

En möjlig separat DevSecOps-utökning är att köra programmet i en isolerad Docker-miljö på Synology NAS med webbaserad terminal och tillfällig Cloudflare-adress. Klasskamraterna ska i så fall bara nå applikationen, inte NAS-administrationen eller Tailscale-nätet. Alla testare ska se samma parkeringsdata. Detta kräver central samordning av ändringar; flera oberoende processer får inte skriva okontrollerat till samma JSON-fil.

Feedback samlas via Discord. Ingen driftsättning eller fleranvändarlösning är implementerad eller testad i nuläget.
