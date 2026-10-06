# Prague Parking V2

Prague Parking V2 är en objektorienterad vidareutveckling av Prague Parking V1.

Projektet utvecklas i C# med .NET 10 och följer en tydlig separation mellan
presentation, domänlogik och dataåtkomst.

## Projektstruktur

- `PragueParkingV2` – huvudapplikation, domänmodell och konsolgränssnitt
- `PragueParkingV2.Data` – separat klassbibliotek för konfiguration och dataåtkomst
- `docs` – kravanalys, klassdiagram, projektplan och designbeslut
- `LOGGBOK.md` – löpande dokumentation av utvecklingsarbetet

## Status

Projektet är under aktiv utveckling.

Följande delar är implementerade eller påbörjade:

- objektorienterad domänmodell med `Vehicle`, `Car`, `MC`, `ParkingSpot` och `ParkingGarage`
- konfiguration via `config.json`
- validering av konfiguration vid programstart
- automatisk återskapning av saknad runtime-konfiguration
- stöd för konfigurerbart antal parkeringsplatser
- konfigurerbara kapacitetsregler för Car och MC
- registrering av ankomsttid i UTC
- sökning efter parkerade fordon
- kontroll mot dubbel incheckning
- grundläggande parkeringslogik
- borttagning av fordon på `ParkingSpot`-nivå
- Spectre.Console-baserad meddelandepresentation

Följande återstår bland annat:

- fullständigt incheckningsflöde
- utcheckning och prisberäkning
- flytt av fordon
- garageöversikt
- persistens av parkeringsdata i `parking-data.json`
- slutlig menystruktur
- VG-funktionalitet i Prague Parking 2.1

## Teknik

- C# / .NET 10
- Spectre.Console
- System.Text.Json
- Git och GitHub

## Dokumentation

Projektets dokumentation finns i `docs`:

- `requirements.md` – kravanalys
- `class-diagram.md` – aktuell domänmodell
- `configuration.md` – konfigurationsstruktur
- `parking-data.md` – planerad struktur för parkeringsdata
- `design-decisions.md` – viktiga arkitektur- och designbeslut
- `project-plan.md` – projektplan och milstolpar

## Versioner

- Prague Parking 2.0 – G-version
- Prague Parking 2.1 – VG-version, planeras efter att 2.0 är stabil