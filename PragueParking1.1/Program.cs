// Initiering av programmet 
#region START
using Spectre.Console;

Console.Title = "Prague Parking v1.1 - Rami I";
bool programmetKörs = true;
string[] parkingGarage = new string[101];
parkingGarage[0] = "PERSONALPARKERING";

#endregion

// Kärn programmet 
#region Kärnprogram


while (programmetKörs)
{
    string val = VisaHuvudmeny();


    switch (val)
    {
        case "Parkera fordon":
            ParkeraFordon(parkingGarage);
            break;

        case "Flytta fordon":
            FlyttaFordon(parkingGarage);
            break;

        case "Hämta ut fordon":
            HämtaUtFordon(parkingGarage);
            break;

        case "Sök fordon":
            SökFordon(parkingGarage);
            break;

        case "Visa parkering":
            VisaParkering(parkingGarage);
            break;

        case "Avsluta":
            programmetKörs = false;
            break;
    }
}
#endregion


#region Användargränssnitt

static string VisaHuvudmeny()
{
    AnsiConsole.Clear();


    AnsiConsole.Write(
        new FigletText("Prague Parking")
            .Centered()
            .Color(Color.White));

    AnsiConsole.MarkupLine("[grey]Version 1.1[/]\n");

    string val = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[red]Vad vill du göra?[/]")
            .PageSize(12)
            .HighlightStyle(new Style(Color.LightGoldenrod1))
            .AddChoices(
                "Parkera fordon",
                "Flytta fordon",
                "Hämta ut fordon",
                "Sök fordon",
                "Visa parkering",
                "Avsluta"
            )
    );

    return val;
}

#endregion

#region Inmatningar och valideringar metoder(funktioner)

// LäsFordonstyp()
static string LäsFordonstyp()
{
    while (true)
    {
        Console.Write("Ange fordonstyp (CAR/MC): ");
        string fordonstyp = (Console.ReadLine() ?? "").ToUpper();

        if (fordonstyp == "CAR" || fordonstyp == "MC")
        {
            return fordonstyp;
        }

        AnsiConsole.MarkupLine($"[red]\nOgiltig fordonstyp. Ange CAR eller MC.[/]\n");
    }
}

// LäsRegistreringsnummer()

static string LäsRegistreringsnummer()
{
    while (true)
    {
        Console.Write("Ange registreringsnummer: ");
        string registreringsnummer = (Console.ReadLine() ?? "").ToUpper();
        if (string.IsNullOrWhiteSpace(registreringsnummer))
        {
            AnsiConsole.MarkupLine($"[red]\nRegistreringsnummer får inte vara tomt.[/]\n");
            continue;
        }
        if (registreringsnummer.Length > 10)
        {
            AnsiConsole.MarkupLine($"[red]\nRegistreringsnummer får inte vara längre än 10 tecken.[/]\n");
            continue;
        }
        bool giltigaTecken = true;
        foreach (char tecken in registreringsnummer)
        {
            if (!char.IsLetterOrDigit(tecken))
            {
                giltigaTecken = false;
                break;
            }
        }
        if (!giltigaTecken)
        {
            AnsiConsole.MarkupLine($"[red]" +
                $"Registreringsnumret får bara innehålla bokstäver och siffror.[/]\n");
            continue;
        }
        return registreringsnummer;
    }
}

// LäsUniktRegistreringsnummer()

static string LäsUniktRegistreringsnummer(string[] parkingGarage)
{
    while (true)
    {
        string registreringsnummer = LäsRegistreringsnummer();
        int plats = HittaFordon(parkingGarage, registreringsnummer);
        if (plats != -1)
        {
            AnsiConsole.MarkupLine($"[red]\nFordonet med registreringsnummer {registreringsnummer} finns redan på plats {plats}.[/]\n\n");
            continue;
        }
        return registreringsnummer;
    }
}

// LäsParkeringsplats()

static int LäsParkeringsplats(string[] parkingGarage)
{
    while (true)
    {
        Console.Write("Ange parkeringplats: ");
        if (!int.TryParse(Console.ReadLine(), out int plats))
        {
            AnsiConsole.MarkupLine("[red]Du måste ange ett nummer.[/]");
            continue;
        }
        if (plats < 1 || plats >= parkingGarage.Length)
        {
            AnsiConsole.MarkupLine("[red]Ogiltig plats.[/]");
            continue;
        }
        return plats;
    }
}
#endregion

#region Sökning och logik

// HittaFordon()
static int HittaFordon(string[] parkingGarage, string registreringsnummer)
{
    for (int plats = 1; plats < parkingGarage.Length; plats++)
    {
        if (string.IsNullOrEmpty(parkingGarage[plats]))
        {
            continue;
        }

        string[] fordonPåPlatsen = parkingGarage[plats].Split('|');

        foreach (string fordon in fordonPåPlatsen)
        {
            string[] delar = fordon.Split('#');

            if (delar.Length >= 2 && delar[1] == registreringsnummer)
            {
                return plats;
            }
        }
    }

    return -1;
}

// HittaTomPlats()
static int HittaTomPlats(string[] parkingGarage)
{
    for (int plats = 1; plats < parkingGarage.Length; plats++)
    {
        if (string.IsNullOrEmpty(parkingGarage[plats]))
        {
            return plats;
        }
    }

    return -1;
}

// HittaPlatsMedEnsamMC()

static int HittaPlatsMedEnsamMC(string[] parkingGarage)
{
    for (int plats = 1; plats < parkingGarage.Length; plats++)
    {
        string? innehåll = parkingGarage[plats];

        if (!string.IsNullOrEmpty(innehåll) &&
            innehåll.StartsWith("MC#") &&
            !innehåll.Contains("|"))
        {
            return plats;
        }
    }

    return -1;
}

// HittaPlatsFörFordon()
static int HittaPlatsFörFordon(string[] parkingGarage, string fordonstyp)
{
    if (fordonstyp == "MC")
    {
        int mcPlats = HittaPlatsMedEnsamMC(parkingGarage);

        if (mcPlats != -1)
        {
            return mcPlats;
        }
    }

    return HittaTomPlats(parkingGarage);
}

#endregion

//Kan fordon parkeras?, lägg till fordon och tabort fordon
#region Fordon och platslogik

static bool KanFordonStåPåPlats(
    string[] parkingGarage,
    int plats,
    string fordon)
{
    if (string.IsNullOrEmpty(parkingGarage[plats]))
    {
        return true;
    }

    if (fordon.StartsWith("MC#") &&
        parkingGarage[plats].StartsWith("MC#") &&
        !parkingGarage[plats].Contains("|"))
    {
        return true;
    }

    return false;
}

static void PlaceraFordonPåPlats(
    string[] parkingGarage,
    int plats,
    string fordon)
{
    if (string.IsNullOrEmpty(parkingGarage[plats]))
    {
        parkingGarage[plats] = fordon;
    }
    else
    {
        parkingGarage[plats] += $"|{fordon}";
    }
}

static void TaBortFordonFrånPlats(
    string[] parkingGarage,
    int plats,
    string fordon)
{
    string[] fordonPåPlatsen = parkingGarage[plats].Split('|');

    if (fordonPåPlatsen.Length == 1)
    {
        parkingGarage[plats] = string.Empty;
    }
    else if (fordonPåPlatsen[0] == fordon)
    {
        parkingGarage[plats] = fordonPåPlatsen[1];
    }
    else
    {
        parkingGarage[plats] = fordonPåPlatsen[0];
    }
}

#endregion

#region Parkeringsfunktioner

// ParkeraFordon()
static void ParkeraFordon(string[] parkingGarage)
{
    AnsiConsole.Clear();

    Console.WriteLine("=== Parkera fordon ===");

    // STEG 1: Fordonstyp
    string fordonstyp = LäsFordonstyp();
    // STEG 2: Registreringsnummer
    string registreringsnummer = LäsUniktRegistreringsnummer(parkingGarage);

    // STEG 3: Hitta en lämplig plats för fordonet
    int plats = HittaPlatsFörFordon(parkingGarage, fordonstyp);
    if (plats == -1)
    {
        AnsiConsole.MarkupLine($"[red]\nIngen tom plats kvar. Parkeringen är full.[/]");
        AnsiConsole.MarkupLine($"[white]\n\nTryck på en tangent för att gå tillbaka...[/]");
        Console.ReadKey();
        return;
    }
    // STEG 4: Parkera fordonet
    if (fordonstyp == "MC" && !string.IsNullOrEmpty(parkingGarage[plats]))
    {
        parkingGarage[plats] += $"|MC#{registreringsnummer}";
    }
    else
    {
        parkingGarage[plats] = $"{fordonstyp}#{registreringsnummer}";
    }
    AnsiConsole.MarkupLine($"[green]\nFordonet har parkerats på plats {plats}.[/]");
    Console.WriteLine("\nTryck på en tangent för att gå tillbaka...");
    Console.ReadKey();
}

// FlyttaFordon()
static void FlyttaFordon(string[] parkingGarage)
{
    AnsiConsole.Clear();
    Console.WriteLine("=== Flytta fordon ===");

    while (true)
    {
        string registreringsnummer = LäsRegistreringsnummer();
        int plats = HittaFordon(parkingGarage, registreringsnummer);

        if (plats == -1)
        {
            AnsiConsole.MarkupLine(
                "[red]\nFordonet hittades inte.[/]\n");

            string val = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Vad vill du göra?")
                    .HighlightStyle(new Style(Color.LightGoldenrod1))
                    .AddChoices(
                        "Försök igen",
                        "Tillbaka till huvudmenyn"
                    )
            );

            if (val == "Tillbaka till huvudmenyn")
            {
                return;
            }

            continue;
        }

        AnsiConsole.MarkupLine(
            $"[green]\nFordonet finns på plats {plats}.[/]");

        string[] fordonPåPlatsen = parkingGarage[plats].Split('|');
        string fordonAttFlytta = "";

        foreach (string fordon in fordonPåPlatsen)
        {
            string[] delar = fordon.Split('#');

            if (delar.Length >= 2 &&
                delar[1] == registreringsnummer)
            {
                fordonAttFlytta = fordon;
                break;
            }
        }

        // Egen loop för att välja ny parkeringsplats
        while (true)
        {
            AnsiConsole.MarkupLine(
                "\n[white]Vilken plats vill du flytta fordonet till? [/]");

            int nyPlats = LäsParkeringsplats(parkingGarage);

            if (nyPlats == plats)
            {
                AnsiConsole.MarkupLine(
                    "[red]\nFordonet är redan på den platsen.[/]");
                continue;
            }

            // Kontrollera om fordonet kan flyttas till den nya platsen
            if (!KanFordonStåPåPlats(parkingGarage, nyPlats, fordonAttFlytta))
            {
                AnsiConsole.MarkupLine(
                    "[red]\nDen nya platsen är upptagen. Välj en annan plats.[/]");
                continue;
            }

            PlaceraFordonPåPlats(parkingGarage, nyPlats, fordonAttFlytta);
            TaBortFordonFrånPlats(parkingGarage, plats, fordonAttFlytta);

            AnsiConsole.MarkupLine(
                $"[green]\nFordonet har flyttats från plats {plats} till plats {nyPlats}.[/]");

            Console.WriteLine(
                "\nTryck på en tangent för att gå tillbaka...");
            Console.ReadKey();

            return;
        }
    }
}

// HämtaUtFordon()

static void HämtaUtFordon(string[] parkingGarage)
{
    AnsiConsole.Clear();
    Console.WriteLine("=== Hämta ut fordon ===");
    while (true)
    {
        string registreringsnummer = LäsRegistreringsnummer();
        int plats = HittaFordon(parkingGarage, registreringsnummer);

        if (plats == -1)
        {
            AnsiConsole.MarkupLine($"[red]\n\nFordonet hittades inte.[/]");
            string val = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Vad vill du göra? ")
                .HighlightStyle(new Style(Color.LightGoldenrod1))
                .AddChoices(
                    "Försök igen!",
                    "Tillbaka till huvudmenyn"
                    ));
            if (val == "Tillbaka till huvudmenyn")
            {
                return;
            }
            Console.WriteLine();
            continue;
        }
        else if (parkingGarage[plats].Contains("|"))
        {
            string[] mcFordon = parkingGarage[plats].Split('|');
            string[] kvarVarandeFordon = new string[mcFordon.Length - 1];
            int index = 0;
            foreach (string fordon in mcFordon)
            {
                if (!fordon.EndsWith($"#{registreringsnummer}"))
                {
                    kvarVarandeFordon[index] = fordon;
                    index++;
                }
            }
            parkingGarage[plats] = string.Join("|", kvarVarandeFordon);

            AnsiConsole.MarkupLine($"[green]\nFordonet har hämtats ut från plats {plats}.[/]");
        }
        else
        {
            parkingGarage[plats] = string.Empty;
            AnsiConsole.MarkupLine($"[green]\nFordonet har hämtats ut från plats {plats}.[/]");
        }

        Console.WriteLine("\nTryck på en tangent för att gå tillbaka...");
        Console.ReadKey();
        return;
    }
}

#endregion

#region Sök och Visning

// SökFordon()

static void SökFordon(string[] parkingGarage)
{
    AnsiConsole.Clear();

    Console.WriteLine("=== Sök fordon ===");

    string registreringsnummer = LäsRegistreringsnummer();

    int plats = HittaFordon(parkingGarage, registreringsnummer);

    if (plats == -1)
    {
        AnsiConsole.MarkupLine($"[red]\n\nFordonet hittades inte.[/]");
    }
    else
    {
        AnsiConsole.MarkupLine($"[green]\nFordonet finns på plats {plats}.[/]");
    }

    Console.WriteLine("\nTryck på en tangent för att gå tillbaka...");
    Console.ReadKey();

}

// VisaParkering()

static void VisaParkering(string[] parkingGarage)
{

    AnsiConsole.Clear();

    AnsiConsole.MarkupLine("[bold green]=== Visa parkering ===[/]\n");

    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Blue)
        .Title("[bold red]=== Parkeringen ===[/]")
        .ShowRowSeparators();

    //Kolumnrubriker 1-10
    for (int i = 1; i <= 10; i++)
    {
        table.AddColumn($"[grey]{i}[/]");

    }

    //10 rader med 10 P-platser
    for (int rad = 1; rad <= 10; rad++)
    {
        string[] row = new string[10];
        for (int kolumn = 0; kolumn < 10; kolumn++)
        {

            int plats = (rad - 1) * 10 + kolumn + 1;
            string innehåll;

            if (string.IsNullOrEmpty(parkingGarage[plats]))
            {
                innehåll = $"[bold white]P-{plats}[/]\n[grey]LEDIG[/]";
            }
            else if (parkingGarage[plats].StartsWith("CAR#"))
            {
                innehåll = $"[bold white]P-{plats}[/]\n[green]{parkingGarage[plats]}[/]";
            }
            else if (parkingGarage[plats].StartsWith("MC#"))
            {
                string mcVisning = parkingGarage[plats].Replace("|", "\n");
                innehåll = $"[bold white]P-{plats}[/]\n[DarkOrange]{mcVisning}[/]";
            }
            else
            {
                innehåll = $"[bold white]P-{plats}[/]\n{parkingGarage[plats]}";
            }
            row[kolumn] = innehåll;

        }


        table.AddRow(row);
    }
    AnsiConsole.Write(table);
    Console.WriteLine();
    Console.WriteLine("Tryck på en tangent för att gå tillbaka...");
    Console.ReadKey();
}

#endregion

