# Design Decisions

Version: 0.1  
Senast uppdaterad: 2026-10-08

## DD-001 – Separation mellan UI, domän och persistence

**Beslut:**  
UI, domänlogik och persistence separeras.

**Motivering:**  
Varje del ska ha ett tydligt ansvar. UI ska hantera presentation och
användarinteraktion, domänlagret parkeringslogik och persistence läsning och
skrivning av data.

---

## DD-002 – Gemensam konfigurationsfil

**Beslut:**  
Prislista och övrig konfiguration kombineras i `config.json`.

**Motivering:**  
Det samlar administratörens inställningar på ett ställe och gör dem enklare
att hitta och ändra utan kodändringar.

---

## DD-003 – Fordonet lagrar inte parkeringsplatsnummer

**Beslut:**  
`ParkingSpotNumber` lagras inte i `Vehicle`.

**Motivering:**  
Placeringen kan redan bestämmas av vilken `ParkingSpot` som innehåller
fordonet. Att lagra samma relation även i `Vehicle` skulle skapa duplicerad
information och risk för inkonsekvent data.

---

## DD-004 – ParkingSpot använder platsnummer som identitet

**Beslut:**  
`ParkingSpot` använder sitt platsnummer som naturlig identitet.

**Motivering:**  
Varje parkeringsplats har ett unikt platsnummer i det aktuella
parkeringshuset, så ett separat tekniskt ID behövs inte i nuläget.

---

## DD-005 – Kapacitetskontroll ligger i ParkingSpot

**Beslut:**  
`ParkingSpot` ansvarar för att avgöra om ett fordon kan parkeras på den
specifika platsen utifrån dess aktuella beläggning och den kapacitetsgräns
som skickas in.

**Motivering:**  
`ParkingSpot` känner till vilka fordon som redan använder platsen och kan
därför avgöra om ytterligare ett fordon får plats. Kapacitetsgränsen hämtas
från konfigurationen och skickas in till `CanPark()`, vilket gör att
kapacitetsregler inte behöver hårdkodas i klassen.

---

## DD-006 – Antal parkeringsplatser härleds från samlingen

**Beslut:**  
`ParkingGarage` lagrar inte ett separat värde för aktuellt antal
parkeringsplatser utöver sin samling av `ParkingSpot`.

**Motivering:**  
Antalet kan härledas från samlingen. Ett separat värde skulle duplicera
information och kunna bli inkonsekvent med det faktiska antalet
parkeringsplatser.

---

## DD-007 – ParkingSpot äger fordonsrelationen

**Beslut:**  
`ParkingSpot` innehåller referenser till de fordon som använder platsen.

**Motivering:**  
Placeringen hålls på ett enda ställe i modellen. `Vehicle` behöver därför
inte lagra parkeringsplatsnummer, vilket minskar duplicerad information.

---

## DD-008 – Konfiguration och parkeringsdata separeras

**Beslut:**  
Programinställningar lagras i `config.json`, medan det aktuella
parkeringsläget lagras separat i `parking-data.json`.

**Motivering:**  
Konfiguration beskriver hur systemet ska fungera medan parkeringsdata
beskriver systemets aktuella tillstånd. Separationen gör filerna enklare att
förstå och minskar risken att användarinställningar blandas ihop med
runtime-data.

---

## DD-009 – G-versionen använder antal fordon per plats

**Beslut:**  
Prague Parking 2.0 använder konfigurerbara värden för hur många fordon av
respektive typ som ryms på en parkeringsplats.

**Motivering:**  
Det följer G-kravet direkt. Den generella storleksmodellen med
kapacitetsenheter införs först i Prague Parking 2.1 där Bicycle, MC, Car och
Bus får storlekarna 1, 2, 4 och 16.

---

## DD-010 – Endast upptagna parkeringsplatser sparas

**Beslut:**  
`parking-data.json` lagrar endast parkeringsplatser som innehåller parkerade
fordon.

**Motivering:**  
Det totala antalet parkeringsplatser definieras redan i `config.json`.
Tomma parkeringsplatser kan därför återskapas vid programstart och behöver
inte dupliceras i datafilen.

---

## DD-011 – System.Text.Json används för JSON-hantering

**Beslut:**  
Projektet använder `System.Text.Json` för att läsa och skriva JSON-data.

**Motivering:**  
`System.Text.Json` ingår direkt i modern .NET och täcker projektets behov
utan att ytterligare externa paket som Newtonsoft.Json behöver installeras.

---

## DD-012 – Ankomsttid lagras i UTC

**Beslut:**  
`Vehicle.ArrivalTime` lagras internt som UTC.

**Motivering:**  
UTC ger en entydig representation av tid och minskar risken för problem med
lokal tidszon och sommartid. Vid presentation kan tiden konverteras till den
tidszon som anges i `config.json`, exempelvis `Europe/Prague`.

---

## DD-013 – Centraliserad meddelandepresentation

**Beslut:**  
Användarmeddelanden presenteras centralt genom `MessageService` i
presentationslagret.

**Motivering:**  
Domän- och dataåtkomstklasser ska inte ansvara för konsolutskrift.
`MessageService` ger ett konsekvent gränssnitt för `SUCCESS`, `INFO`,
`WARNING` och `ERROR`, medan själva felhanteringen sker på lämplig nivå i
applikationen.

---

## DD-014 – Saknad runtime-konfiguration återskapas med standardvärden

**Beslut:**  
Om den `config.json` som programmet använder vid körning saknas skapas en ny
fil med systemets standardvärden.

**Motivering:**  
Programmet ska kunna starta även om runtime-konfigurationen saknas. En
befintlig men ogiltig konfigurationsfil skrivs däremot inte över
automatiskt, eftersom användarens inställningar då riskerar att förloras.

---

## DD-015 – Prisberäkning separeras från ParkingGarage

**Beslut:**  
Beräkning av parkeringsavgift hanteras av `ParkingFeeCalculator` och inte av
`ParkingGarage` eller `ParkingSpot`.

**Motivering:**  
`ParkingGarage` och `ParkingSpot` ansvarar för placering och hantering av
fordon. Prisberäkning är ett separat domänansvar och hålls därför i en egen
klass.

Det gör koden lättare att läsa, testa och förändra utan att
parkeringslogiken behöver känna till prisregler.

---

## DD-016 – Aktiva prisregler hålls i minnet

**Beslut:**  
`ParkingFeeCalculator` får prisregler från den inlästa konfigurationen och
lagrar de aktiva värdena i minnet.

De aktiva värdena omfattar:

- pris per timme för `Car`
- pris per timme för `MC`
- antal gratisminuter

**Motivering:**  
`config.json` behöver då inte läsas och deserialiseras vid varje
prisberäkning.

`CalculateParkingFee()` behöver endast information om fordonet och
avgångstiden. Kalkylatorn känner redan till de aktiva prisreglerna.

---

## DD-017 – Prisregler uppdateras först efter fullständig validering

**Beslut:**  
`ParkingFeeCalculator.UpdatePricing()` ska validera samtliga nya prisvärden
innan något av de aktiva värdena ersätts.

**Motivering:**  
En omläsning av konfigurationen får inte lämna systemet i ett delvis
uppdaterat tillstånd.

Principen är:

```text
nya värden
    ↓
validera samtliga
    ↓
alla giltiga?
    ├── Ja  → ersätt aktiva värden
    └── Nej → behåll tidigare värden
```

Om ett nytt värde är ogiltigt ska de tidigare fungerande prisreglerna ligga
kvar i minnet.

Den felaktiga konfigurationsfilen ska inte heller skrivas över automatiskt.

---

## DD-018 – Checkout samordnas separat från garage- och prislogik

**Beslut:**  
Det fullständiga utcheckningsflödet ska samordnas av `CheckoutService`.

**Motivering:**  
En utcheckning består av flera separata domänoperationer:

```text
Identifier
    ↓
hitta fordon
    ↓
beräkna parkeringstid och avgift
    ↓
ta bort fordon
    ↓
returnera resultat
```

`ParkingGarage` ska fortsatt ansvara för att hitta och ta bort fordon,
medan `ParkingFeeCalculator` ansvarar för avgiftsberäkningen.

`CheckoutService` ska samordna dessa operationer utan att
`Program.cs` behöver innehålla hela affärsflödet.

Själva checkout-flödet är ännu inte färdigimplementerat.