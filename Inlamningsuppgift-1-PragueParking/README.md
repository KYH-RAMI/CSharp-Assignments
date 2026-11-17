# Prague Parking

Första inlämningsuppgiften i C#.

Prague Parking är ett textbaserat system för att hantera en
valet parking med plats för bilar och motorcyklar.

Solutionen innehåller två projekt:

- `PragueParking1.0` - grundversionen
- `PragueParking1.1` - vidareutveckling för VG-delen

## Prague Parking 1.0

Systemet kan:

- [x] Visa en textbaserad meny
- [x] Hantera 100 parkeringsplatser
- [x] Läsa in fordonstyp
- [x] Validera CAR eller MC
- [x] Läsa in registreringsnummer
- [x] Kontrollera registreringsnummer
- [x] Parkera fordon på en lämplig plats
- [x] Visa parkeringsplatserna
- [x] Hantera två MC på samma parkeringsplats
- [x] Kontrollera dubbletter av registreringsnummer
- [x] Söka efter fordon
- [x] Flytta fordon
- [x] Hämta ut fordon

## Prague Parking 1.1

Version 1.1 bygger vidare på version 1.0.

Koden har strukturerats om för att göra programmet lättare
att läsa och för att separera olika delar av programmet.

Bland annat har huvudmenyn flyttats till en egen metod och
delar av platslogiken har delats upp i mindre metoder.

Funktioner som redan finns från version 1.0:

- [x] Visualisering av parkeringsplatser
- [x] Validering av användarinput

Funktioner i version 1.1:

- [x] Optimering av MC-parkering med arbetsordrar
- [ ] ~~Automatisk tidsstämpling och beräkning av parkeringstid~~ – valdes bort

### MC-optimering

MC-optimeringen söker efter ensamma motorcyklar och parar ihop
dem så att så många MC som möjligt delar parkeringsplats.

Programmet prioriterar lägre parkeringsnummer och flyttar MC
från högre platser för att frigöra fler parkeringsplatser.

För varje flytt skrivs en arbetsorder ut och efter optimeringen
visas hur många arbetsordrar som skapades.

### Tidsstämpling

Tidsstämpling valdes bort i denna version eftersom version 1.1
redan innehåller tre av förbättringarna i uppgiften.

En implementering av tidsstämpling hade krävt att lagringsformatet
för fordon ändrades, vilket även hade påverkat befintlig logik för
sökning, flyttning och uthämtning.



## Programstruktur

I version 1.1 är `Program.cs` uppdelad med `#region` för att göra koden
lättare att navigera i.

Koden är uppdelad i bland annat:

- programstart och kärnprogram
- användargränssnitt
- inmatning och validering
- sökning och logik
- fordons- och platslogik
- parkeringsfunktioner
- sökning och visning

Platslogiken har även delats upp så att kontroll, placering
och borttagning av fordon kan hanteras i mindre metoder.


## Parkeringsplatser

Parkeringen representeras med en string-array.

```csharp
string[] parkingGarage = new string[101];
parkingGarage[0] = "PERSONALPARKERING";
```

Index 0 används som reserverad plats.

Det gör att parkeringsplats 1 motsvarar `parkingGarage[1]`
och parkeringsplats 100 motsvarar `parkingGarage[100]`.


## Lagringsformat

En bil lagras t.ex. som:

`CAR#ABC123`

En MC lagras som:

`MC#ABC123`

Två MC på samma parkeringsplats lagras som:

`MC#ABC123|MC#DEF456`


## Beroenden

Båda projekten använder NuGet-paketet `Spectre.Console`
för huvudmenyn och konsolpresentationen.

Paketet återställs normalt automatiskt när projekten
öppnas och byggs i Visual Studio.

## Avgränsningar

Programmet hanterar inte fordon som är kvar efter parkeringens
stängning vid 00.00. Den hanteringen ligger utanför det aktuella
systemet enligt uppgiften.