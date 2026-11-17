# Prague Parking V2 – Designbeslut

**Uppdaterad:** 2026-10-22  
**Projektperiod:** 2026-10-05–2026-10-22 (18 kalenderdagar)  
**Omfattning:** Arkitektur- och implementationsbeslut för Prague Parking V2.0 (G) och V2.1 (VG).  
**Slutstatus:** Båda versionerna är färdigutvecklade, manuellt funktionstestade och genomgångna mot projektets krav inför inlämning. Dokumentationsarbetet avslutas före inlämningen.

## Så läses dokumentet

Dokumentet beskriver **vilka designval jag gjorde, varför jag gjorde dem och hur de fungerar i den färdiga lösningen**. Samtliga ursprungliga besluts-ID, **DD-001–DD-050**, är bevarade och står i nummerordning. Beslutens ursprungliga datum har behållits där de var angivna. Ett äldre datum visar *när beslutet togs*, medan den angivna slutstatusen avser **22 oktober 2026**.

Två självständigt körbara versioner finns i samma Visual Studio-solution:

| Version | Projekt | Kapacitetsmodell | Fordon |
|---|---|---|---|
| **V2.0 (G)** | `PragueParkingV2` och `PragueParkingV2.Data` | `VehiclesPerSpot` | `Car`, `MC` |
| **V2.1 (VG)** | `PragueParkingV2_1` och `PragueParkingV2_1.Data` | `SpotCapacity` + `VehicleTypes.Size` | `Car`, `MC`, `Bicycle`, `Bus` |

V2.0 behåller sin kod och sitt etablerade JSON-format. V2.1 använder egna projekt, runtime-filer och konfigurationsregler. *Manuellt testad* innebär här att dokumenterade scenarier har prövats; det är inte liktydigt med en automatisk testsvit, fullständig formell verifiering eller en garanti mot alla tänkbara fel. Utförliga testhändelser finns i `LOGGBOK.md` och kravuppföljningen i `requirements.md`.

---

## DD-001 – Separation mellan UI, domän och dataåtkomst

**Beslut:** Jag separerade presentation, parkeringsregler och JSON-/filhantering. UI ligger i huvudprojektet, domänobjekten hanterar affärsreglerna och respektive `.Data`-projekt sköter filåtkomst.

**Motivering:** Det ska gå att ändra en konsolvy eller lagringsmetod utan att behöva skriva om reglerna för parkering och kapacitet. `ParkingDataMapper` i applikationslagret översätter mellan datamodeller och domänobjekt; se DD-027.

## DD-002 – Gemensam abstrakt basklass för fordon

**Beslut:** `Vehicle` är en abstrakt basklass för `Car` och `MC` i G. I VG ärver också `Bicycle` och `Bus` från `Vehicle`.

**Motivering:** Identifierare och ankomsttid är gemensam information och ska inte dupliceras i varje fordonsklass. Fordonsklasserna kan konstrueras både vid ny parkering och vid återställning av en tidigare sparad UTC-ankomsttid.

**Historik:** `Bicycle` och `Bus` tillkom den 20 oktober och integrerades senare i färdiga VG-flöden.

## DD-003 – Generell fordonsidentifierare

**Beslut:** Den gemensamma egenskapen heter `Identifier`, inte `RegistrationNumber`.

**Motivering:** Bilar och motorcyklar kan använda registreringsnummer, medan exempelvis cyklar kan identifieras på annat sätt. Identifikationen fungerar därför oberoende av fordonstyp.

## DD-004 – Parkeringsplatsen ansvarar för sina fordon

**Beslut:** `ParkingSpot` äger sin interna samling av parkerade fordon och exponerar den som `IReadOnlyList<Vehicle>`.

**Motivering:** Övriga delar av programmet får läsa vilka fordon som står där utan att direkt ändra den interna listan. Tillåtna förändringar ska gå genom parkeringsplatsens och garagets metoder, där reglerna kan kontrolleras.

## DD-005 – Fordon lagrar inte parkeringsplatsnummer

**Beslut:** `Vehicle` innehåller inte något eget `SpotNumber`.

**Motivering:** Placeringen framgår av det `ParkingSpot`-objekt som innehåller fordonet. Dubbel lagring av platsnummer skulle kunna ge inkonsekvent tillstånd vid flytt. Bussens särskilda flerplatsreservationer förklaras i DD-043.

## DD-006 – Garageklassen samordnar placering och sökning

**Beslut:** `ParkingGarage` hanterar samlingen av platser, söker lämplig plats, kontrollerar dubbletter och samordnar parkering, flytt och borttagning.

**Motivering:** `ParkingSpot` känner sina lokala kapacitetsregler; `ParkingGarage` behöver känna till hur platserna samverkar. Den uppdelningen är särskilt viktig för en buss som berör fyra platser.

## DD-007 – Kapacitetsmodell i G-versionen

**Beslut:** V2.0 använder `VehicleTypes.<typ>.VehiclesPerSpot` från `config.json`: normalt en bil eller två motorcyklar per plats. Car och MC får inte blandas på samma plats i G.

**Motivering:** Den modellen motsvarar G-versionens regler och behålls oförändrad. Storleksmodellen `Size` + `SpotCapacity` infördes bara i V2.1; se DD-036.

## DD-008 – JSON som filformat

**Beslut:** `System.Text.Json` används för konfiguration och parkeringsdata.

**Motivering:** JSON är läsbart, har stöd i .NET och fungerar för både inställningar och återställning av parkerade fordon. De vanliga spar- och inläsningsflödena har testats manuellt, inklusive omstart.

## DD-009 – Separata filer för konfiguration och parkeringsdata

**Beslut:** `config.json` innehåller inställningar; `parking-data.json` innehåller garagets sparade tillstånd.

**Motivering:** Pris- och kapacitetsregler ändras av administratören, medan fordonsdata ändras vid parkering, flytt och utcheckning. Dessa två typer av data ska inte blandas i en fil.

## DD-010 – Spara endast upptagna platser

**Beslut:** `ParkingDataMapper.ToDocument()` sparar bara platser som innehåller fordon. Tomma platser byggs upp igen från den konfigurerade garagekapaciteten.

**Motivering:** JSON-filen blir mindre och representerar endast det tillstånd som faktiskt behöver återställas. Bussens primärplats utgör ett särskilt fall där de tre ytterligare reservationerna återskapas i minnet; se DD-043.

## DD-011 – UTC för tidpunkter

**Beslut:** `ArrivalTime` registreras och sparas i UTC. Vid visning används tidszonen från konfigurationen, normalt `Europe/Prague`.

**Motivering:** Parkeringstid och avgifter ska inte påverkas av lokal sommar-/vintertid. Samma ankomsttid följer med vid flytt och återställning från JSON. Detta har verifierats i dokumenterade omstartstester.

## DD-012 – Samlad JSON-konfiguration med priser först

**Beslut:** Prislista, gratisgräns, garageinställningar, tidszon och fordonsregler ligger samlade i `config.json` med priserna lättåtkomliga.

**Motivering:** Inställningarna ska vara begripliga och kunna redigeras utan ny kompilering. Förändringar under pågående körning börjar gälla först efter avsedd omläsning.

## DD-013 – Skapa saknad runtime-konfiguration

**Beslut:** Programmet kan återskapa sin aktiva `Config/config.json` när den saknas. I V2.1 används `config.template.json` som förlaga.

**Motivering:** Första uppstarten får användbara standardvärden utan att en befintlig användarkonfiguration skrivs över. Se DD-041 för skillnaden mellan mall och aktiv fil.

## DD-014 – Avvisa felaktig befintlig konfiguration

**Beslut:** En befintlig ogiltig `config.json` ska inte ersättas automatiskt med standardinställningar.

**Motivering:** Programmet ska hellre visa ett begripligt fel än förlora användarens inställningar. Vid full omläsning i V2.1 behålls den senast aktiva giltiga konfigurationen om den nya inte går att använda.

## DD-015 – Separat prisberäkning

**Beslut:** `ParkingFeeCalculator` ansvarar för avgiftsregler; `ParkingGarage` ansvarar för fordonsplacering.

**Motivering:** Prislogiken går att förstå och justera utan att påverka kapacitetskontroll och parkeringsplatser.

## DD-016 – Prisvärden hålls i kalkylatorinstansen

**Beslut:** Aktiva priser och gratisminuter laddas från konfigurationen och kan uppdateras genom en validerad `UpdatePricing()`.

**Motivering:** `Administration → Reload Pricing` kan ändra priser utan omstart. I G testades 20 → 30 CZK för Car; ett ogiltigt värde −10 CZK avvisades utan att ersätta giltiga priser. V2.1 har dessutom full omläsning av konfigurationen, se DD-047.

## DD-017 – Strukturerat resultat för prisberäkning

**Beslut:** `ParkingFeeCalculator.CalculateParkingFee()` returnerar `ParkingFeeResult` med uppgifter för presentation av tid, timpris, gratisgräns och avgift.

**Motivering:** UI visar ett färdigt beräkningsresultat och behöver inte innehålla själva beräkningsreglerna. Resultatet är inte permanent parkeringsdata.

## DD-018 – Checkout samordnas av en separat service

**Beslut:** `CheckoutService` samordnar sökning, avgiftsberäkning och borttagning av ett fordon.

**Motivering:** Parkeringsavgiften ska kunna beräknas innan fordonet tas bort. Själva sparningen och återkopplingen sker genom UI-flödet; se DD-029.

## DD-019 – Strukturerat utcheckningsresultat

**Beslut:** `CheckoutService` lämnar `CheckoutResult` till presentationslagret. `ParkingFeeResult` och `CheckoutResult` sparas inte i `parking-data.json`.

**Motivering:** De beskriver en aktuell åtgärd, inte fordon som fortfarande står parkerade.

## DD-020 – Dynamisk garageöversikt med kompakt Grid och intern viewport

**Senast reviderat i G:** 2026-10-13. **Tester:** 2026-10-13–16.

**Beslut:** `GarageOverView` använder ett kompakt Spectre.Console-`Grid` i en gemensam `Panel`, med `PARKING MAP`, `SPOT DETAILS` och `GARAGE STATUS`. Rutnätet anpassar antal kolumner efter terminalens bredd och parkeringsplatsernas numrering.

**Bakgrund:** Jag gick från separata paneler i ett 5 × 5-rutnät (9–10 oktober), via `Table` (12 oktober), till `Grid` (13 oktober). Anledningen var att utnyttja terminalytan bättre och undvika onödig scrollning.

**Genomförande:** Grön, orange och röd bakgrund visar ledig, delvis upptagen respektive full plats. Markerad plats får egen textmarkering. Piltangenter navigerar; ESC återgår. Om hela kartan inte ryms används en intern viewport som följer vald plats. `AnsiConsole.Live()` uppdaterar vyn utan att rita om hela terminalen vid varje tangenttryckning.

**Verifiering:** G-översikten testades med 100, 200 och 1 000 platser, inklusive navigering till plats 1000 och återgång. Vid 100 platser med en bil och två MC på samma MC-plats redovisades `FREE 98 / OCCUPIED 2 / VEHICLES 3`. En viss minsta terminalstorlek krävs; alla externa terminalmiljöer eller kraschscenarier är inte dokumenterat provade. V2.1:s motsvarande översikt med storleksenheter och bussreservationer behandlas i DD-045.

## DD-021 – Gemensamt UI-tema med AppTheme

**Datum:** 2026-10-10.

**Beslut:** Gemensamma färger, paneler, menyer och skärmpresentation hanteras genom `AppTheme`. G använder ett blått/gult tema; V2.1 har ett separat varmt **Sunset-tema**.

**Motivering:** Temat ska vara enhetligt inom varje version och enkelt att justera utan upprepade färgdefinitioner i varje vy. `MessageService` använder samma presentationshjälpmedel.

## DD-022 – Positionsbaserad och återanvändbar menynavigering

**Datum:** 2026-10-10.

**Beslut:** `AppTheme.ShowMenu()` återanvänds för menypresentation och tangentbordsnavigering. `MainMenu` ansvarar för vilket alternativ som öppnar vilken funktion.

**Motivering:** En tidigare helt omritad meny gav flimmer. Positionsbaserad uppdatering med konsolens markörfunktioner minskar onödig omritning och samlar menybeteendet på ett ställe. Garageöversiktens `AnsiConsole.Live()` är en separat lösning för just den vyn.

## DD-023 – Centraliserade statusmeddelanden

**Datum:** 2026-10-10. **Visuella justeringar testade:** 2026-10-16.

**Beslut:** `MessageService` visar återanvändbara paneler för fel, varning, information och lyckade åtgärder.

**Motivering:** Alla vyer ska ge enhetlig återkoppling. Särskilt viktigt är att Park, Check Out och Move inte visar framgång innan sparningen avslutats utan rapporterat fel. Ett test med blockerad `.tmp`-sökväg visade felmeddelande och återställningsförsök i de tre G-flödena.

## DD-024 – Unika identifierare i hela parkeringshuset

**Datum:** 2026-10-10. **Testad:** 2026-10-16.

**Beslut:** Varje fordon har en identifierare som är unik i hela garaget, utan skillnad mellan stora och små bokstäver.

**Motivering:** `ABC123` och `abc123` ska inte kunna räknas som två fordon. Regeln används vid registrering och JSON-inläsning; G-tester omfattade båda fallen. JSON-valideringen använder `StringComparer.OrdinalIgnoreCase`.

## DD-025 – Fastställd struktur och säker återställning för parking-data.json

**Datum:** 2026-10-10. **Testhistorik:** 2026-10-11, 14–16 och 21 oktober.

**Beslut:** JSON-formatet består av `ParkingSpots` med `SpotNumber` och `Vehicles`. Varje fordon har `Identifier`, `VehicleType` och `ArrivalTime` (UTC). G-formatet ändras inte i V2.1.

```json
{
  "ParkingSpots": [
    {
      "SpotNumber": 17,
      "Vehicles": [
        {
          "Identifier": "ABC123",
          "VehicleType": "Car",
          "ArrivalTime": "2026-10-05T14:30:00Z"
        }
      ]
    }
  ]
}
```

**Motivering:** Vid omstart ska rätt fordonsobjekt kunna återställas till exakt samma plats och med ursprunglig tid. `ParkingDataService` validerar dokumentet; `ParkingDataMapper` och domänlogiken kontrollerar att fordonen verkligen ryms i garaget.

**Filhantering:** `Save()` skriver normalt till en `.tmp`-fil och använder `File.Move()` eller `File.Replace()` beroende på om originalfilen finns. Manuella G-tester den 16 oktober omfattade korrupt JSON, dubbletter, ogiltiga platser och kapacitet samt ett simulerat skrivfel med återställningsförsök. Det är inte ett bevis på fullständig atomisk transaktionshantering eller säkerhet vid alla typer av plötsligt processavbrott. Se `parking-data.md` för detaljer och V2.1:s bussrepresentation.

## DD-026 – VG-utökning med Bicycle och Bus utan att ändra G-formatet

**Datum:** 2026-10-10. **Slutresultat:** Implementerat och funktionstestat i V2.1.

**Beslut:** V2.1 lägger till Bicycle och Bus samt storleksbaserad kapacitet utan att förändra G-versionens kod eller filformat. Standardstorlekarna är Bicycle **1**, MC **2**, Car **4**, Bus **16**, och en vanlig plats har kapacitet **4**.

**Bussregel:** En buss kräver fyra sammanhängande, helt lediga platser inom **001–050**. Området är inte reserverat enbart för bussar. Lärarens förtydligande gäller **både parkering och flytt**. Standardvärdet `BusMaxParkingSpot = 50` följer denna regel. I den färdiga lösningen hanteras bussen som ett objekt; platserna reserveras tillsammans och återställs vid inläsning.

**Motivering:** Storleksbaserad kapacitet möjliggör även blandningar som MC + Bicycle, medan bussens fyra platser förutsätter en egen reservationsmodell. Se DD-036, DD-043 och DD-046.

## DD-027 – Mapper i applikationslagret isolerar DTO och domän

**Datum:** 2026-10-14.

**Beslut:** `ParkingDataMapper` ligger i huvudprojektets applikationslager, inte i Data-biblioteket. `ToDocument()` omvandlar garaget till DTO:er för sparning och `ToGarage()` återskapar domänobjekten från data.

**Motivering:** Data-biblioteket behöver då inte känna till `ParkingGarage` och fordonsklasserna. Mappningen återställer exakta platsnummer och UTC-tider; V2.1 använder dessutom domänens bussparkering för att återupprätta reservationerna.

**Historik:** G:s sparning och återställning verifierades 14–16 oktober. Buss och de andra VG-typerna verifierades därefter i JSON-omstartstester den 21 oktober.

## DD-028 – Incheckning sparas innan bekräftelse

**Datum:** 2026-10-14. **Riktat feltest:** 2026-10-16.

**Beslut:** `ParkVehicleView` parkerar via domänlogik, skapar JSON-dokumentet och anropar `Save()` innan ett lyckat resultat visas.

**Motivering:** Om sparningen misslyckas får användaren inte se ett felaktigt framgångsbesked. Vid vissa filskrivningsundantag försöker vyn ångra parkeringen. I G-testet med blockerad `.tmp`-sökväg fanns `ROLL123` inte kvar i minnet efter avbruten parkering.

## DD-029 – Utcheckning med bekräftelse och sparad borttagning

**Datum:** 2026-10-15. **Riktat feltest:** 2026-10-16.

**Beslut:** `CheckOutVehicleView` visar tid och beräknad avgift, begär bekräftelse, genomför borttagning genom tjänsten och sparar garaget innan kvitto visas.

**Motivering:** Användaren kan avbryta, och en osparad borttagning ska inte presenteras som genomförd. Vid framprovocerat G-skrivfel återfanns `ABC123` på sin ursprungsplats. För V2.1 frigör en normal bussutcheckning samtliga fyra reservationer.

## DD-030 – Flytt med förkontroller och JSON-sparning

**Datum:** 2026-10-15. **Riktat feltest:** 2026-10-16.

**Beslut:** Flytt kontrollerar ursprung, destination och platsens kapacitet innan den ändrar garaget. Samma fordonsobjekt och `ArrivalTime` bevaras. Resultatet sparas till JSON före framgångsmeddelandet.

**Motivering:** Felaktiga platsval ska avvisas utan att parkeringen förloras. I G-testet återställdes en misslyckad sparning av flytten 050 → 051 till plats 050; efter att hindret tagits bort lyckades flytten och kvarstod efter omstart. V2.1:s bussflytt använder särskilda kontroller för sammanhängande platser och delvis överlappande intervall; se DD-046.

**Versionsgräns:** Den ursprungliga G-metoden använde `maxVehiclesPerSpot`. V2.1 använder den nya storleksmodellen; äldre `maxVehiclesPerSpot`-överlagringar avlägsnades från **VG** vid slutstädningen den 22 oktober, inte från G.

## DD-031 – Tydlig återkoppling vid platsval

**Datum:** 2026-10-15.

**Beslut:** `MoveVehicleView` använder ett upprepat inmatningsflöde där ogiltiga målplatser ger ett tydligt fel och möjlighet att ange ett nytt nummer utan att starta om hela åtgärden.

**Motivering:** Användaren ska kunna rätta inmatningsfel. Felpaneler placeras och ritas om så att de inte staplas. I den dokumenterade G-vyn används tom inmatning/Enter för att backa ur vissa textfält; tangenthantering kan skilja sig mellan vyerna.

## DD-032 – Sökvy utan ändring av garagets tillstånd

**Datum:** 2026-10-16.

**Beslut:** `FindVehicleView` söker och visar information om ett fordon utan att ändra garagets innehåll eller JSON-filen.

**Motivering:** Sökning är en läsoperation. Skiftlägesokänslig sökning testades för G; V2.1:s sökvy visar även Bus som ett platsintervall och Bicycle på en enskild plats. Se DD-049.

## DD-033 – Prisomläsning via Administration under körning

**Datum:** 2026-10-16.

**Beslut:** G:s `Administration → Reload Pricing` uppdaterar den gemensamma prisberäknaren utan att återskapa garage eller ändra fordonsplaceringar.

**Motivering:** G-kravet gäller att läsa om priser utan programomstart. Ändrat bilpris 20 → 30 CZK fungerade och negativt pris avvisades. VG:s funktion för omläsning av *hela* konfigurationen är ett separat designbeslut, DD-047.

## DD-034 – Separata körbara versioner i samma solution

**Datum:** 2026-10-20. **Slutstatus:** Båda versionerna färdigutvecklade och testade 2026-10-22.

**Beslut:** `PragueParkingV2` / `PragueParkingV2.Data` och `PragueParkingV2_1` / `PragueParkingV2_1.Data` ligger i samma solution, men har separata körvägar och runtime-data. Båda använder .NET 10.

**Motivering:** VG-utvecklingen får inte bryta G-versionens fungerande beteende. Versionerna kan startas var för sig och jämföras mot sina respektive krav och testresultat.

## DD-035 – IVehicle som fordonskontrakt i V2.1

**Datum:** 2026-10-20. **Slutstatus:** Implementerat och använt i V2.1.

**Beslut:** `IVehicle` anger att en fordonstyp ska tillhandahålla **`Identifier`** och **`ArrivalTime`**. Den abstrakta basklassen `Vehicle` implementerar interfacet; `Car`, `MC`, `Bicycle` och `Bus` omfattas genom arv.

**Kontraktsförklaring:** Ett **interface** beskriver *vilka medlemmar en typ lovar att erbjuda*, men inte hur fordonsklasserna måste lagra dem eller vilka konstruktorer de ska ha. En metod som tar emot `IVehicle` får därför tillgång till de deklarerade medlemmarna utan att veta om objektet är en bil eller cykel. En sådan metod får däremot inte automatiskt åtkomst till alla specialmedlemmar i en konkret klass.

**Motivering:** Gemensam typning minskar beroendet av konkreta klasser. `IVehicle` förekommer bland annat i kapacitetslogikens funktionsparameter `Func<IVehicle, int>`. Interfacet ersätter inte basklassens återanvändbara implementation – det är två olika OOP-verktyg.

## DD-036 – Storleksbaserad kapacitet och Func för storleksuppslagning

**Datum:** 2026-10-20. **Slutstatus:** Implementerat och testat i V2.1; äldre VG-överlagringar städades bort 2026-10-22.

**Beslut:** `Garage.SpotCapacity` och `VehicleTypes[...].Size` används för kapacitetskontroll. Standardvärden: Bicycle 1, MC 2, Car 4, Bus 16 och platskapacitet 4. Bussens 16 enheter hanteras genom fyra reserverade platser och inte genom att lägga ett vanligt 16-enhetersfordon på en 4-enhetersplats.

**Genomförande:** `ParkingSpot.GetUsedCapacity()` summerar storleken för fordon på platsen. Parkering och flytt får använda en funktion som slår upp fordonsstorleken i den aktiva konfigurationen. Exempel på den princip som används:

```csharp
Func<IVehicle, int> getSize = vehicle =>
    configuration.VehicleTypes![vehicle.GetType().Name].Size;
```

**Förklaring:** `Func<IVehicle, int>` är en **delegattyp**: funktionen tar ett objekt som uppfyller `IVehicle`-kontraktet och returnerar ett `int`. `ParkingSpot` behöver bara anropa funktionen för att få storleken; platsklassen behöver inte läsa eller känna till JSON-filen. Lambdauttrycket ovan visar *hur* ett storleksuppslag kan skickas in. Det kräver fortfarande att konfigurationen innehåller giltiga storlekar.

**Resultat:** Blandad beläggning med MC (2) + Bicycle (1) gav 3/4 enheter i ett dokumenterat test. Modellen används vid parkering, flytt, återställning och garageöversikt. G:s `VehiclesPerSpot` behölls oförändrat. Den 22 oktober avlägsnades de tidigare `VehiclesPerSpot`-beroendena och gamla `maxVehiclesPerSpot`-metodöverlagringarna från V2.1.

## DD-037 – Bussens fyra sammanhängande platser: algoritmskiss

**Datum:** 2026-10-20. **Historik:** Detta var algoritmskissen före den färdiga bussimplementationen den 21 oktober.

**Beslut:** Sök efter fyra sammanhängande, helt lediga platser. Börja med 001–004 och flytta sökfönstret ett steg i taget tills en giltig grupp hittas. Sista tillåtna gruppen med `BusMaxParkingSpot = 50` är 047–050.

**Motivering:** En enkel glidande sökning är lätt att förstå och kan hitta en tillgänglig grupp även när andra fordon använder delar av bussområdet. Det ska inte gå att reservera en ofullständig grupp.

**Slutlig lösning:** Gruppen reserveras genom samma bussobjekt på primärplatsen och fyra `ReservedByBus`-referenser; se DD-043. Läraren förtydligade att gränsen 001–050 även gäller vid flytt; se DD-046. Den tidigare öppna frågan är därmed avgjord.

## DD-038 – VG-konfiguration, validering och omläsning under körning

**Datum:** 2026-10-20. **Slutstatus:** Implementerat och manuellt testat i V2.1.

**Beslut:** V2.1 använder storlekar per fordonstyp, gemensam platskapacitet och priser för samtliga fyra typer. Priser per påbörjad timme i standardkonfigurationen: Car 20, MC 10, Bicycle 5 och Bus 80 CZK. Parkering **kortare än 10 minuter** är gratis; exakt 10 minuter debiteras enligt implementerad och dokumenterat testad gränsregel.

**Motivering:** Inställningar ska ligga utanför hårdkodade domänregler. Vid full omläsning kontrolleras även att befintliga fordon kan återställas under den föreslagna konfigurationen. En post i JSON skapar däremot inte automatiskt en helt ny C#-klass – de fyra implementerade typerna har uttryckligt kodstöd.

**Resultat:** Den fullständiga reloadfunktionen beskrivs i DD-047. `Reload Pricing` i G har ett avgränsat ansvar och ersätts inte retroaktivt av VG-lösningen.

## DD-039 – V2.1:s mappning och automatisk testdata

**Datum:** 2026-10-20. **Slutstatus:** Implementerat och testat med JSON-omstart 2026-10-21.

**Beslut:** V2.1:s `ParkingDataMapper` hanterar Car, MC, Bicycle och Bus. Om `parking-data.json` saknas vid första uppstart skapar `TestDataSeeder` ett giltigt garage med testfordon och sparar det genom den vanliga datavägen.

**Motivering:** VG-kravet på testdata uppfylls utan att blanda testobjekten med G-versionen och utan att skriva över en befintlig fil. Även en befintlig, giltig fil med tom lista respekteras. En befintlig korrupt fil ska inte automatiskt ersättas med nyskapad testdata.

**Resultat:** Ett sparat testgarage med Bus, Car, MC och Bicycle återställdes efter omstart med ursprungliga tider och korrekt beläggning. Se DD-048 och `parking-data.md`.

## DD-040 – Kurskraven framför frivillig driftsättning

**Datum:** 2026-10-20. **Slutstatus:** Beslutad avgränsning för den färdiga inlämningen.

**Beslut:** Frivillig distribution och betatestning via separat Windows-paketering, NAS/Docker, Cloudflare eller Discord ingår inte i projektets leverans.

**Motivering:** Fokus låg på G-/VG-kraven, funktionalitet, manuell testning och dokumentation. Projektet versionshanteras på GitHub, men **repositoryt får vara privat enligt lärarens individuellt godkända undantag**. Läraren ges åtkomst till det privata projektet. Detta ska inte förväxlas med en påstådd verifiering av drift på varje annan dator eller terminaltyp.

## DD-041 – Skilja på V2.1-konfigurationsmall och aktiv konfiguration

**Datum:** 2026-10-21. **Slutstatus:** Implementerat och starttestat.

**Beslut:** `PragueParkingV2_1/Config/config.template.json` är förlagan. Om den aktiva runtime-filen `Config/config.json` saknas skapar `ConfigurationService` den genom kopiering av mallen; en befintlig fil läses och valideras men skrivs inte över automatiskt.

**Motivering:** Egna inställningar ska bevaras mellan builds. Standardvärdena ska inte dubbleras och riskera att skilja sig åt i olika delar av implementationen.

## DD-042 – IParkingSpot som parkeringsplatskontrakt

**Datum:** 2026-10-21. **Slutstatus:** Implementerat som andra interface i V2.1.

**Beslut:** `ParkingSpot` implementerar `IParkingSpot`. Kontraktet omfattar enligt den dokumenterade interface-definitionen **`SpotNumber`**, **`ParkedVehicles`**, **`GetUsedCapacity`**, **`CanPark`**, **`ParkVehicle`**, **`FindVehicle`** och **`RemoveVehicle`**.

**Kontraktsförklaring:** Interfacet anger *vilka egenskaper och operationer en parkeringsplats ska tillhandahålla*. Det avgör inte hur fordonen lagras internt eller exakt hur kapacitetsberäkningen görs; den implementationen finns i `ParkingSpot`. `CanPark` uttrycker en tillåtenhetskontroll, medan `ParkVehicle` står för en förändring av tillståndet. Att två klasser kan ha ett gemensamt kontrakt innebär inte att de automatiskt måste ha samma interna data eller kod.

**Avgränsning:** Bussreservationernas särskilda metoder ligger utanför detta generella interface och samordnas av `ParkingGarage`. `IVehicle` och `IParkingSpot` fyller alltså olika roller: det ena beskriver ett fordon, det andra en parkeringsplats.

## DD-043 – En Bus på huvudplatsen och fyra reservationsreferenser

**Datum:** 2026-10-21. **Slutstatus:** Implementerat och funktionstestat.

**Beslut:** En `Bus` lagras bara **en gång** i primärplatsens `ParkedVehicles`. Samtliga fyra platser i gruppen får `ReservedByBus` som hänvisar till **samma Bus-instans**. Reservationerna gör platserna otillgängliga för vanlig parkering.

**Motivering:** Bussen ska räknas som ett fordon vid sökning, avgift, utcheckning och JSON-lagring, men som fyra upptagna platser i garagekartan. Alternativet att lägga fyra separata bussobjekt i datamodellen hade gett dubbletter och inkonsekvenser.

**Genomförande:** Domänlogiken reserverar och frigör hela gruppen. `ParkingDataMapper.ToDocument()` sparar bara primärplatsens bussobjekt. Vid JSON-inläsning återskapar `ToGarage()` de fyra reservationerna genom bussparkeringen på den sparade primärplatsen.

## DD-044 – Samlad ombyggnad av Garage Overview efter bussintegrationen

**Datum:** 2026-10-21. **Historiskt beslut – genomfört samma dag.**

**Beslut:** Anpassningen av V2.1:s garageöversikt gjordes samlat efter att bussens reservationsmodell etablerats, i stället för att ändra varje delvy separat.

**Motivering:** Tidigare visning utgick från G:s `VehiclesPerSpot`, och en Bicycle hade orsakat ett körfel i översikten. En samlad uppdatering gjorde det möjligt att få statusfärger, detaljpanel och summering att använda samma nya regler.

**Resultat:** Ombyggnaden genomfördes och Bicycle-felet åtgärdades; den färdiga vyn dokumenteras i DD-045.

## DD-045 – V2.1:s Garage Overview och Sunset-tema

**Datum:** 2026-10-21. **Slutstatus:** Implementerat och manuellt testat.

**Beslut:** V2.1:s `GarageOverView` räknar platskapacitet med `SpotCapacity` och fordonsstorlekar. Alla fyra fordonstyper känns igen, bussens fyra platser visas som upptagna, men bussen räknas som **ett** fordon i summeringen.

**Motivering:** UI ska spegla samma domänregler som parkerings- och JSON-logiken. Det separata Sunset-temat skiljer visuellt VG från G-versionen. Intern viewport prövades med 500 platser; den tidigare kraschen vid Bicycle rättades.

**Tillägg 22 oktober:** Den fristående Live Demonstration använder samma presentationsprinciper, men har ett eget garage i minnet. Se DD-050.

## DD-046 – Bussens områdesgräns och verifierade flöden

**Datum:** 2026-10-21. **Slutstatus:** Implementerat och testat i dokumenterade normalflöden.

**Beslut:** Bus får stå endast inom **001–050**, både vid nyparkering och flytt enligt lärarens förtydligande. Platserna får också användas av andra fordon. `BusMaxParkingSpot` är tekniskt konfigurerbar, men kursens standardvärde är **50**.

**Verifiering:** `BUS001` flyttades från **004–007 till 006–009** trots delvis överlappning. JSON-omstart återskapade placeringen. Vid normal utcheckning frigjordes de fyra platserna, och **1 timme 38 minuter** gav **160 CZK** i avgift. Dessa resultat visar fungerande normalflöden, inte att varje tänkbart IO-fel eller gränskombination har testats.

## DD-047 – Reload Configuration med tillfälligt garage

**Datum:** 2026-10-21. **Slutstatus:** Implementerat och manuellt prövat med giltig och ogiltig omläsning.

**Beslut:** `AdministrationView` kan begära full `Reload Configuration` via en `Action` som kopplas från `MainMenu`. Ny konfiguration läses in och garagets sparbara tillstånd prövas mot den genom att bygga ett **tillfälligt garage** innan ny konfiguration, garage och prisinställningar aktiveras.

**Motivering:** Redan parkerade fordon och bussreservationer får inte förloras för att någon har valt en oförenlig konfiguration. Vid fel används de gamla aktiva objekten vidare; den nya filen på disken återställs inte automatiskt av denna åtgärd.

**Verifiering:** Omläsning från 100 till 200 och 500 platser fungerade utan omstart. Ändring av `SpotCapacity` från 4 till 2 med befintlig Car avvisades och det gamla garaget behölls. Detta styrker de redovisade fallen; varje kombination av ändrad tidszon, pris och storlek har inte dokumenterats som separat testad.

## DD-048 – TestDataSeeder bara när datafilen saknas

**Datum:** 2026-10-21. **Slutstatus:** Implementerat och testat genom omstart.

**Beslut:** Vid V2.1-uppstart kontrolleras om `parking-data.json` finns. Endast vid saknad fil skapar `TestDataSeeder.CreateGarage()` testfordon med samma parkeringsregler som normalt används, varefter filen skrivs. Om filen redan finns laddas den i stället.

**Verifiering:** `TESTBUS1` reserverade 001–004, `TESTCAR1` stod på 005 och `TESTMC1` + `TESTBIKE1` delade 006. Med 200 platser visades **FREE 194 / OCCUPIED 6 / VEHICLES 4**. Vid omstart återkom samma fordon och UTC-tider utan att en ny uppsättning testfordon skapades.

**Motivering:** Detta uppfyller VG-kravet utan att skriva över riktiga parkeringsdata. En befintlig giltig tom fil ska förbli tom; en befintlig felaktig fil ska inte tyst ersättas.

## DD-049 – Hitta nya fordonsklasser och hantera avbrytning

**Datum:** 2026-10-21. **Slutstatus:** Genomfört och manuellt prövat för beskrivna sökflöden.

**Beslut:** `FindVehicleView` hanterar även Bicycle och Bus. Vid Bus visas hela det reserverade platsintervallet; vid Bicycle visas dess enskilda plats. Identifieraren söks utan att ändra garaget.

**Motivering:** Sökresultat ska visa riktiga fordonstyper i stället för `Unknown`. Den nya inmatningshanteringen i V2.1:s sökvy med `Console.ReadKey(intercept: true)` ger stöd för bland annat ESC och Backspace. Detta är ett vylokalt beslut och innebär inte att alla andra textfält använder exakt samma tangentbeteende.

## DD-050 – Live Demonstration i isolerat garage

**Beslutades:** 2026-10-21. **Färdigställdes och testades:** **2026-10-22**. **Status:** Genomförd frivillig extrafunktion, inte ett separat VG-krav.

**Beslut:** Live Demonstration kör en automatisk sekvens i ett **separat parkeringsgarage med 100 platser i minnet**. Den visar Car, MC, Bicycle och Bus som parkeras, flyttas och checkas ut. Garageöversikten uppdateras fortlöpande med kartans beläggning, statusfärger, `SPOT DETAILS` och `LAST EVENT`. Demonstrationen kan avbrytas med **ESC**.

**Motivering:** Det blir möjligt att visa de färdiga parkeringsfunktionerna visuellt utan att först manuellt registrera en mängd fordon eller riskera att påverka test- och användardata.

**Datagräns:** Demonstrationsgaraget hålls åtskilt från den vanliga programkörningens parkeringsgarage och dess permanenta `Data/parking-data.json`. Live Demonstration är inte samma funktion som `TestDataSeeder`, vilken bara används vid saknad ordinarie VG-datafil (DD-048).

**Resultat:** Den automatiska sekvensen, dynamisk uppdatering och ESC-avslutning färdigställdes den 22 oktober. Slutstädningen i V2.1 omfattade samtidigt borttagning av äldre `VehiclesPerSpot`-beroenden och `maxVehiclesPerSpot`-överlagringar. G-versionen ändrades inte.

---

## Projektavslut och dokumentationshänvisningar

**Utveckling:** 5–22 oktober 2026. Båda versionerna är färdigutvecklade och manuellt funktionstestade. Kravmatrisen har gåtts igenom. Därefter färdigställs dokumentationen och projektet lämnas in. GitHub-repositoryt förblir privat enligt lärarens godkända undantag; läraren ges tillgång till projektet.

För en samlad bild av hur besluten omsattes i kod och tester, se:

- [`requirements.md`](requirements.md) – uppföljning av G- och VG-krav.
- [`class-diagram.md`](class-diagram.md) – domänklasser och relationer.
- [`configuration.md`](configuration.md) – inställningar, mall och omläsning.
- [`parking-data.md`](parking-data.md) – JSON-struktur, återställning och datavalidering.
- [`project-plan.md`](project-plan.md) – projektfaser och milstolpar.
- [`LOGGBOK.md`](../LOGGBOK.md) – utvecklingshistorik, felsökning och manuella tester.
