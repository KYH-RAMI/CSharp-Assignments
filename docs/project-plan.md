# Prague Parking V2 – Projektplan

**Uppdaterad:** 2026-10-09  
**Utvecklingsmiljö:** Visual Studio Community 2026, C# och .NET 10  
**Arbetsordning:** Prague Parking 2.0 (G) färdigställs och verifieras före Prague Parking 2.1 (VG).

## Mål och avgränsning

Syftet är att vidareutveckla Prague Parking från en enklare konsollösning till en objektorienterad applikation.

Lösningen delas upp i UI, domänlogik och dataåtkomst. Konfiguration och parkeringsdata hanteras med JSON.

Först prioriteras fungerande G-funktionalitet, därefter VG-utbyggnad i ett eget projekt i samma solution.

## Arbetsfaser

Status beskriver läget den 9 oktober, inte att arbetet är slutgiltigt godkänt eller fullständigt testat.

| Fas | Område | Status |
| --- | --- | --- |
| 1 | Genomgång av uppgift och G-/VG-krav | Genomförd |
| 2 | Projektstruktur, solution och separat Data-klassbibliotek | Genomförd |
| 3 | Domänmodell: `Vehicle`, `Car`, `MC`, `ParkingSpot`, `ParkingGarage` | Genomförd, fortsatt verifiering möjlig |
| 4 | JSON-konfiguration, standardvärden och validering | Genomförd, manuellt testad |
| 5 | Parkera, söka och ta bort fordon i domänlagret | Genomförd, manuellt testad |
| 6 | Timtaxa, gratisgräns, prisresultat och `UpdatePricing()` | Genomförd, manuellt testad |
| 7 | Utcheckningslogik med `CheckoutService` och `CheckoutResult` | Genomförd, manuellt testad |
| 8 | Spectre.Console och garageöversikt | Pågår |
| 9 | Inchecknings-, sök-, flytt- och utcheckningsflöden i UI | Återstår |
| 10 | Parkeringspersistens med `parking-data.json` | Återstår |
| 11 | Prisomläsning via huvudmeny | Återstår |
| 12 | Sluttest, dokumentation och GitHub-leverans | Återstår |
| 13 | VG: nytt projekt, interfaces, `Bicycle`, `Bus`, storleksmodell och reload | Ej påbörjad |

**Planeringsuppskattning för fas 8:** 4–6 timmar i den ursprungliga arbetsplanen. Faktisk tid och eventuella ändringar noteras i loggboken.

## Milstolpe 1 – Grundläggande arkitektur och domän

**Status:** Uppnådd på implementationsnivå.

- Projektstruktur med två projekt i solutionen.
- Fordonsklasser, parkeringsplatser och garage.
- Konfiguration med validering.
- Grundläggande parkering, sökning och borttagning.
- Separat prisberäkning och checkout-service.

Återstående användarflöden och persistens påverkar inte att grundarkitekturen har etablerats.

## Milstolpe 2 – Prague Parking 2.0 / G

**Status:** Pågår.

### Genomfört eller påbörjat

- Interaktiv parkeringskarta med Spectre.Console `Grid` och individuella `Panel`.
- 25 parkeringsplatser per sida i ett rutnät med fem kolumner.
- Dynamisk pagination efter antal parkeringsplatser.
- Tangentbordsnavigering med pilar och Page Up/Page Down.
- Visuell markering av vald plats och statusfärger för beläggning.
- Grundläggande detaljpanel samt sidfot.
- Domänlogik för prisberäkning och utcheckning är implementerad.

### Kvar innan G kan bedömas som klar

- Färdigställa och testa detaljpanelen, färgkodning och navigering.
- Koppla in domänlogiken i användbara UI-menyer för incheckning och utcheckning.
- Implementera sök- och flyttflöden.
- Spara och läsa tillbaka parkerade fordon via JSON.
- Möjliggöra prisomläsning från fil via meny.
- Verifiera hela flödet och uppdatera dokumentationen.

## Milstolpe 3 – Prague Parking 2.1 / VG

**Status:** Ej påbörjad.

VG utvecklas först när G fungerar stabilt.

### Planerade tillägg

- Separat projekt i samma solution.
- Minst två interfaces.
- `Bicycle` och `Bus`.
- Kapacitetsenheter:
  - Bicycle = 1.
  - MC = 2.
  - Car = 4.
  - Bus = 16.
  - Vanlig parkeringsplats = 4.
- Buss på fyra sammanhängande platser bland de första 50.
- Pris för Bicycle 5 CZK/timme och Bus 80 CZK/timme.
- Konfigurerbara storlekar och säker omläsning av hela konfigurationen under körning.
- Individuell reflektion på cirka 2–3 sidor.

## Närmaste prioritering

1. Spara nuvarande fungerande utvecklingsläge och dokumentation i Git.
2. Bygga ett fullständigt incheckningsflöde som använder befintligt domänlager.
3. Fortsätta med utcheckning, sökning och flytt i UI.
4. Implementera parkeringspersistens och därefter sluttesta helheten.

## Risker och uppföljning

### Felaktig kravstatus

Vi behöver skilja mellan implementerat och verifierat i kravmatrisen. Att en funktion finns i koden innebär inte att den har testats fullständigt.

### Förlorad parkeringsdata

JSON-lagring måste uppdateras vid varje relevant förändring innan G-versionen är färdig.

### Terminalstorlek

Parkeringskartan och detaljpanelen behöver kontrolleras vid olika fönsterstorlekar.

### Konfiguration

En ogiltig befintlig JSON-fil ska inte automatiskt ersättas av standardvärden.

### Tidsplan

Färdigställ G-funktionalitet innan visuellt finlir eller VG-funktioner prioriteras.

## Dokumentation och uppföljning

Detaljerade arbetsanteckningar, genomförda tester, designbeslut, problem och lösningar dokumenteras i `LOGGBOK.md`.

Kravstatus uppdateras löpande i `requirements.md`.

Arkitekturella beslut dokumenteras i `design-decisions.md`.