# Prague Parking

Första inlämningsuppgiften i C#.

Prague Parking är ett textbaserat system för att hantera en
valet parking med plats för bilar och motorcyklar.

Projektet utvecklas först som Prague Parking 1.0.
När version 1.0 är färdig kommer ett separat projekt för
Prague Parking 1.1 att skapas för VG-delen.

## Prague Parking 1.0

Systemet ska kunna:

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

Funktioner som ska läggas till i version 1.1:

- [ ] Optimering av MC-parkering med arbetsordrar
- [ ] Automatisk tidsstämpling och beräkning av parkeringstid


## Programstruktur

`Program.cs` är uppdelad med `#region` för att göra koden
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

Version 1.0 hanterar inte fordon som är kvar efter parkeringens
stängning vid 00.00. Den hanteringen ligger utanför det aktuella
systemet enligt uppgiften.