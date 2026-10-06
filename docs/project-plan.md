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
| 7 | Prisberäkning | 1–2 h | Ej påbörjad |
| 8 | Spectre.Console och garageöversikt | 4–6 h | Pågår |
| 9 | Testning och felsökning av G-version | 3–5 h | Pågår |
| 10 | VG – interfaces och refaktorering | 2–3 h | Ej påbörjad |
| 11 | VG – Bicycle, Bus och storleksenheter | 3–5 h | Ej påbörjad |
| 12 | VG – omläsning och validering av konfiguration | 2–4 h | Ej påbörjad |
| 13 | Sluttest, README, loggbok och reflektion | 3–5 h | Pågår |

## Milstolpar

### Milstolpe 1 – Design klar

Status: **Pågår**

- Kravanalys dokumenterad
- Klassdiagram dokumenterat och uppdateras löpande
- `config.json` planerad och implementerad
- `parking-data.json` planerad
- Viktiga designbeslut dokumenterade

### Milstolpe 2 – Prague Parking 2.0 / G

Status: **Pågår**

- Grundläggande domänmodell implementerad
- Parkeringslogik påbörjad
- Sökning efter fordon implementerad
- Kontroll mot dubbel incheckning implementerad
- Borttagning av fordon implementerad på `ParkingSpot`-nivå
- Konfiguration läses från JSON
- Konfiguration valideras vid programstart
- Saknad runtime-konfiguration kan återskapas med standardvärden
- Parkeringsdata ska sparas och återställas
- Prisberäkning ska implementeras
- Spectre.Console används för meddelandepresentation
- Garageöversikt ska implementeras
- G-versionen ska sluttestas och stabiliseras

### Milstolpe 3 – Prague Parking 2.1 / VG

Status: **Ej påbörjad**

- Minst två interfaces används
- Bicycle och Bus stöds
- Storleksenheter används för kapacitet
- Buss kan hantera sitt större platsbehov
- Konfiguration kan läsas om under körning
- Ny konfiguration valideras innan den aktiveras

### Milstolpe 4 – Inlämningsklar

Status: **Pågår**

- Samtliga krav verifierade
- README uppdaterad
- LOGGBOK uppdateras löpande
- Reflektion färdig
- Projektet testat från ren start
- GitHub-repositoryt är uppdaterat