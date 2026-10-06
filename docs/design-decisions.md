# Design Decisions

Version: 0.1  
Senast uppdaterad: 2026-10-05

**Beslut:**  
UI, domänlogik och persistence separeras.

**Motivering:**  
Varje del ska ha ett tydligt ansvar. UI ska hantera presentation och användarinteraktion, domänlagret parkeringslogik och persistence läsning och skrivning av data.

---

## DD-002 – Gemensam konfigurationsfil

**Beslut:**  
Prislista och övrig konfiguration kombineras i `config.json`.

**Motivering:**  
Det samlar administratörens inställningar på ett ställe och gör dem enklare att hitta och ändra utan kodändringar.

---

## DD-003 – Fordonet lagrar inte parkeringsplatsnummer

**Beslut:**  
`ParkingSpotNumber` lagras inte i `Vehicle`.

**Motivering:**  
Placeringen kan redan bestämmas av vilken `ParkingSpot` som innehåller fordonet. Att lagra samma relation även i `Vehicle` skulle skapa duplicerad information och risk för inkonsekvent data.

---

## DD-004 – ParkingSpot använder platsnummer som identitet

**Beslut:**  
`ParkingSpot` använder sitt platsnummer som naturlig identitet.

**Motivering:**  
Varje parkeringsplats har ett unikt platsnummer i det aktuella parkeringshuset, så ett separat tekniskt ID behövs inte i nuläget.


## DD-005 – Kapacitetskontroll ligger i ParkingSpot

**Beslut:**  
`ParkingSpot` ansvarar för att avgöra om dess egen lediga kapacitet räcker för ett fordon.

**Motivering:**  
En parkeringsplats känner till sin totala kapacitet och vilka fordon som redan använder platsen. `ParkingGarage` 
behöver därför inte hantera detaljerna för varje enskild plats utan kan fråga platserna och fokusera på att hitta en lämplig plats.

## DD-006 – Antal parkeringsplatser härleds från samlingen

**Beslut:**  
`ParkingGarage` lagrar inte ett separat värde för aktuellt antal parkeringsplatser utöver sin samling av `ParkingSpot`.

**Motivering:**  
Antalet kan härledas från samlingen. Ett separat värde skulle duplicera information och kunna bli inkonsekvent med det faktiska antalet parkeringsplatser.

## DD-007 – ParkingSpot äger fordonsrelationen

**Beslut:**  
`ParkingSpot` innehåller referenser till de fordon som använder platsen.

**Motivering:**  
Placeringen hålls på ett enda ställe i modellen. `Vehicle` behöver därför inte lagra parkeringsplatsnummer, vilket minskar duplicerad information.

## DD-008 – Konfiguration och parkeringsdata separeras

**Beslut:**  
Programinställningar lagras i `config.json`, medan det aktuella parkeringsläget lagras separat i `parking-data.json`.

**Motivering:**  
Konfiguration beskriver hur systemet ska fungera medan parkeringsdata beskriver systemets aktuella tillstånd. Separationen gör filerna enklare att förstå och minskar risken att användarinställningar blandas ihop med runtime-data.

## DD-009 – G-versionen använder antal fordon per plats

**Beslut:**  
Prague Parking 2.0 använder konfigurerbara värden för hur många fordon av respektive typ som ryms på en parkeringsplats.

**Motivering:**  
Det följer G-kravet direkt. Den generella storleksmodellen med kapacitetsenheter införs först i Prague Parking 2.1 där cykel, MC, bil och buss uttryckligen får storlekarna 1, 2, 4 och 16.

## DD-010 – Endast upptagna parkeringsplatser sparas

**Beslut:**  
`parking-data.json` lagrar endast parkeringsplatser som innehåller parkerade fordon.

**Motivering:**  
Det totala antalet parkeringsplatser definieras redan i `config.json`. Tomma parkeringsplatser kan därför återskapas vid programstart och behöver inte dupliceras i datafilen.

## DD-011 – System.Text.Json används för JSON-hantering

**Beslut:**  
Projektet använder `System.Text.Json` för att läsa och skriva JSON-data.

**Motivering:**  
`System.Text.Json` ingår direkt i modern .NET och täcker projektets behov utan att ytterligare externa paket som Newtonsoft.Json behöver installeras.

