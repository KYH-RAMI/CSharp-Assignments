using PragueParkingV2.Data.Configuration;
using PragueParkingV2.Domain;
using Spectre.Console;

namespace PragueParkingV2.UI
{
    internal static class MainMenu
    {
        private const int PanelWidth = 62;
        private const int PanelHeight = 22;
        private const int MenuWidth = 26;
        private const int FirstOptionRow = 11;

        private static readonly string[] MenuOptions =
        {
            "Park Vehicle",
            "Check Out Vehicle",
            "Move Vehicle",
            "Find Vehicle",
            "Garage Overview",
            "Administration",
            "Exit"
        };

        public static void Show(
            ParkingGarage parkingGarage,
            AppConfiguration configuration)
        {
            int selectedIndex = 0;

            try
            {
                while (true)
                {
                    string selectedOption =
                        ReadMenu(ref selectedIndex);

                    switch (selectedOption)
                    {
                        case "Garage Overview":

                            GarageOverView.DisplayGarageOverview(
                                parkingGarage,
                                configuration);

                            break;

                        case "Exit":
                            return;

                        default:

                            AppTheme.PrepareScreen();

                            MessageService.ShowInfo(
                                "This feature is under development.");

                            AnsiConsole.MarkupLine(
                                "\n[white]Press any key to continue...[/]");

                            Console.ReadKey(intercept: true);

                            break;
                    }
                }
            }
            finally
            {
                AppTheme.RestoreScreen();
            }
        }

        private static string ReadMenu(ref int selectedIndex)
        {
            if (Console.WindowWidth < PanelWidth + 2 ||
                Console.WindowHeight < PanelHeight + 2)
            {
                AnsiConsole.Clear();

                return AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]MAIN MENU[/]")
                        .PageSize(7)
                        .AddChoices(MenuOptions));
            }

            AppTheme.PrepareScreen();

            int left =
                (Console.WindowWidth - PanelWidth) / 2;

            int top =
                (Console.WindowHeight - PanelHeight) / 2;

            DrawFrame(left, top);
            DrawHeader(left, top);

            for (int i = 0; i < MenuOptions.Length; i++)
            {
                DrawOption(
                    left,
                    top,
                    i,
                    i == selectedIndex);
            }

            while (true)
            {
                var key =
                    Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.Enter)
                    return MenuOptions[selectedIndex];

                if (key == ConsoleKey.Escape)
                    return "Exit";

                int nextIndex = key switch
                {
                    ConsoleKey.UpArrow =>
                        (selectedIndex - 1 + MenuOptions.Length)
                        % MenuOptions.Length,

                    ConsoleKey.DownArrow =>
                        (selectedIndex + 1)
                        % MenuOptions.Length,

                    _ => selectedIndex
                };

                if (nextIndex == selectedIndex)
                    continue;

                DrawOption(
                    left,
                    top,
                    selectedIndex,
                    false);

                selectedIndex = nextIndex;

                DrawOption(
                    left,
                    top,
                    selectedIndex,
                    true);
            }
        }

        private static void DrawFrame(int left, int top)
        {
            AppTheme.WriteText(
                left,
                top,
                "╭" + new string('─', PanelWidth - 2) + "╮",
                AppTheme.Border);

            for (int row = 1; row < PanelHeight - 1; row++)
            {
                AppTheme.WriteText(
                    left,
                    top + row,
                    "│",
                    AppTheme.Border);

                AppTheme.WriteText(
                    left + PanelWidth - 1,
                    top + row,
                    "│",
                    AppTheme.Border);
            }

            AppTheme.WriteText(
                left,
                top + PanelHeight - 1,
                "╰" + new string('─', PanelWidth - 2) + "╯",
                AppTheme.Border);
        }

        private static void DrawHeader(int left, int top)
        {
            int titleLeft =
                left + (PanelWidth - 17) / 2;

            AppTheme.WriteText(
                titleLeft,
                top + 2,
                "PRAGUE",
                AppTheme.GoldLight,
                bold: true);

            AppTheme.WriteText(
                titleLeft + 7,
                top + 2,
                "PARKING",
                AppTheme.Gold,
                bold: true);

            AppTheme.WriteText(
                titleLeft + 15,
                top + 2,
                "V2",
                AppTheme.GoldDark,
                bold: true);

            WriteCentered(
                left,
                top + 3,
                "PARKING MANAGEMENT SYSTEM",
                AppTheme.Text);

            int dividerLeft =
                left + (PanelWidth - 18) / 2;

            AppTheme.WriteText(
                dividerLeft,
                top + 5,
                "━━━━━━",
                AppTheme.GoldLight);

            AppTheme.WriteText(
                dividerLeft + 6,
                top + 5,
                "━━━━━━",
                AppTheme.Gold);

            AppTheme.WriteText(
                dividerLeft + 12,
                top + 5,
                "━━━━━━",
                AppTheme.GoldDark);

            int authorLeft =
                left + (PanelWidth - 17) / 2;

            AppTheme.WriteText(
                authorLeft,
                top + 7,
                "Author:",
                AppTheme.GoldLight);

            AppTheme.WriteText(
                authorLeft + 8,
                top + 7,
                "Rami",
                AppTheme.Gold);

            AppTheme.WriteText(
                authorLeft + 13,
                top + 7,
                "Issa",
                AppTheme.GoldDark);

            WriteCentered(
                left,
                top + 9,
                "MAIN MENU",
                AppTheme.Gold,
                true);

            WriteCentered(
                left,
                top + 19,
                "↑ ↓ Navigate   |   Enter Select   |   Esc Exit",
                AppTheme.Muted);
        }

        private static void DrawOption(
            int left,
            int top,
            int index,
            bool isSelected)
        {
            int menuLeft =
                left + (PanelWidth - MenuWidth) / 2;

            int menuTop =
                top + FirstOptionRow + index;

            string option = isSelected
                ? $" ❯ {MenuOptions[index]}"
                : $"   {MenuOptions[index]}";

            option = option.PadRight(MenuWidth);

            AppTheme.WriteText(
                menuLeft,
                menuTop,
                option,
                isSelected ? "#071C35" : AppTheme.Text,
                isSelected ? AppTheme.Accent : AppTheme.Background,
                isSelected);
        }

        private static void WriteCentered(
            int left,
            int top,
            string text,
            string color,
            bool bold = false)
        {
            int textLeft =
                left + (PanelWidth - text.Length) / 2;

            AppTheme.WriteText(
                textLeft,
                top,
                text,
                color,
                bold: bold);
        }
    }
}