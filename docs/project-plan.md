# Projektplan – Prague Parking V2

## Syfte

Projektplanen används för att strukturera utvecklingen av Prague Parking 2.0 (G)
och Prague Parking 2.1 (VG).

Tidsuppskattningarna är preliminära och kan förändras beroende på testning,
felsökning och nya tekniska problem.

## Övergripande tidsplan

| Fas | Innehåll | Uppskattad tid | Status |
|---|---|---:|---|
| 1 | Kravanalys och projektstruktur | 3–5 h | Pågår |
| 2 | Domänmodell och klassdesign | 2–3 h | Pågår |
| 3 | Grundklasser: Vehicle, Car, MC, ParkingSpot, ParkingGarage | 4–6 h | Implementerad |
| 4 | Konfiguration och JSON-inläsning | 3–4 h | Verifierad |
| 5 | JSON-persistens av parkeringsdata | 2–3 h | Ej påbörjad |
| 6 | Incheckning, utcheckning och flytt av fordon | 3–4 h | Pågår |
| 7 | Prisberäkning | 1–2 h | Verifierad |
| 8 | Spectre.Console och garageöversikt | 4–6 h | Pågår |
| 9 | Testning och felsökning av G-version | 3–5 h | Pågår |
| 10 | VG – interfaces och refaktorering | 2–3 h | Ej påbörjad |
| 11 | VG – Bicycle, Bus och storleksenheter | 3–5 h | Ej påbörjad |
| 12 | VG – omläsning och validering av konfiguration | 2–4 h | Ej påbörjad |
| 13 | Sluttest, README, loggbok och reflektion | 3–5 h | Pågår |

## Milstolpar

### Milstolpe 1 – Design klar

Status: **Pågår**

- Kravanalys dokumenterad och uppdateras löpande
- Klassdiagram dokumenterat och uppdateras efter implementation
- `config.json` planerad och implementerad
- `parking-data.json` planerad
- Viktiga designbeslut dokumenterade
- Ansvar mellan `ParkingSpot`, `ParkingGarage`, `ParkingFeeCalculator` och `CheckoutService` definierat
- Resultatobjekten `ParkingFeeResult` och `CheckoutResult` dokumenterade

### Milstolpe 2 – Prague Parking 2.0 / G

Status: **Pågår**

- Grundläggande domänmodell implementerad
- Parkeringslogik påbörjad
- Sökning efter fordon implementerad
- Kontroll mot dubbel incheckning implementerad
- Borttagning av fordon implementerad på både `ParkingSpot`- och `ParkingGarage`-nivå
- Konfiguration läses från JSON
- Konfiguration valideras vid programstart
- Saknad runtime-konfiguration kan återskapas med standardvärden
- Prisberäkning implementerad och verifierad i `ParkingFeeCalculator`
- Pris per påbörjad timme verifierad för `Car` och `MC`
- Gratisgräns för kort parkering implementerad och verifierad
- Felaktig avgångstid före ankomsttid hanteras
- Prisregler hämtas från konfigurationen och hålls som aktiva värden i minnet
- `UpdatePricing()` implementerad och verifierad
- `ParkingFeeResult` används för strukturerat resultat från prisberäkning
- `CheckoutService` implementerad
- `CheckoutResult` används för strukturerat resultat från utcheckning
- Checkout-flödet i domänlagret implementerat och verifierat
- Prisberäkning sker före borttagning av fordon vid checkout
- Verifierat att fordon tas bort från `ParkingGarage` efter lyckad checkout
- Spectre.Console används för meddelandepresentation
- Parkeringsdata ska sparas och återställas
- Fullständigt användarflöde för incheckning ska färdigställas
- Fullständigt UI-flöde för utcheckning ska implementeras
- Flytt av fordon ska implementeras
- Prislista ska kunna läsas om via meny
- Garageöversikt ska implementeras
- G-versionen ska sluttestas och stabiliseras

### Milstolpe 3 – Prague Parking 2.1 / VG

Status: **Ej påbörjad**

- Minst två interfaces används
- Bicycle och Bus stöds
- Storleksenheter används för kapacitet
- Bicycle använder 1 kapacitetsenhet
- MC använder 2 kapacitetsenheter
- Car använder 4 kapacitetsenheter
- Bus använder totalt 16 kapacitetsenheter
- Fyra sammanhängande parkeringsplatser krävs för Bus
- Bussparkering begränsas till de första 50 parkeringsplatserna
- Bicycle och Bus får egna prisregler
- Fordonstyper och storlekar hanteras via konfiguration
- Hela konfigurationen kan läsas om under körning
- Ny konfiguration valideras innan den aktiveras
- Tidigare giltig konfiguration behålls om omläsningen misslyckas
- UI uppdateras efter lyckad omläsning av konfiguration

### Milstolpe 4 – Inlämningsklar

Status: **Pågår**

- Samtliga G-krav verifierade
- Samtliga valda VG-krav verifierade
- README uppdaterad
- LOGGBOK uppdaterad
- Klassdiagram och övrig dokumentation synkroniserad med slutlig implementation
- Reflektion färdig
- Projektet testat från ren start
- Programmet testat på annan dator eller motsvarande ren miljö
- GitHub-repositoryt uppdaterat
- Repositoryt gjort publikt inför inlämning
- Slutlig repository-länk kontrollerad