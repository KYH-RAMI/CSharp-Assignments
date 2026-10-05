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