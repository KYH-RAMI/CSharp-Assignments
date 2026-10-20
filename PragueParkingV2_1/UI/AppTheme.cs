using Spectre.Console;
using Spectre.Console.Rendering;

namespace PragueParkingV2_1.UI
{
    internal static class AppTheme
    {
        public const string Background = "#0B2D50";

        public const string GoldLight = "#FFDF70";
        public const string Gold = "#FECC00";
        public const string GoldDark = "#D9A600";

        public const string Accent = Gold;
        public const string Text = "#F5F8FF";
        public const string Muted = "#B8CBE0";
        public const string Border = "#4488BE";
        public const string SelectionText = "#071C35";
        public const int FormWidth = 62;
        private const int PanelWidth = FormWidth;

        private const int PanelHeight = 22;
        private const int OptionWidth = 26;
        public const string StatusFree = "green";
        public const string StatusPartial = "orange1";
        public const string StatusFull = "red";

        public const int GarageMapWidth = 70;
        public const string StatusText = "#071C35";
        public const string SelectedSpotText = GoldLight;
        public const string SelectedSpotBackground = Gold;

        public const int GarageSpotWidth = 12;
        public const int GarageDetailsWidth = 40;
        public const int GarageFooterWidth = 80;

        public static readonly Color SelectedSpotBorder = Color.LightGoldenrod1;
        public static readonly Color NormalSpotBorder = Color.White;

        public static readonly Color BorderColor =
            new(68, 136, 190);
        public static readonly Color SuccessBorderColor = Color.Green;
        public static void PrepareScreen()
        {
            Console.CursorVisible = false;

            Console.Write("\u001b]11;rgb:0B/2D/50\u001b\\");
            Console.Write("\u001b[0m");

            AnsiConsole.Clear();
        }

        public static void RestoreScreen()
        {
            Console.Write("\u001b]111\u001b\\");
            Console.Write("\u001b[0m");

            Console.CursorVisible = true;
        }
        public static string GradientMarkup(
            string light,
            string middle,
            string dark)
        {
            return
                $"[bold {GoldLight}]{Markup.Escape(light)}[/]" +
                $"[bold {Gold}]{Markup.Escape(middle)}[/]" +
                $"[bold {GoldDark}]{Markup.Escape(dark)}[/]";
        }
        public static void WriteText(
            int left,
            int top,
            string value,
            string foreground,
            string? background = null,
            bool bold = false)
        {
            Console.SetCursorPosition(left, top);

            string style = bold ? "bold " : "";

            AnsiConsole.Markup(
                $"[{style}{foreground} on {background ?? Background}]" +
                $"{Markup.Escape(value)}[/]");
        }

        public static void WriteCentered(
            int left,
            int top,
            int width,
            string text,
            string color,
            bool bold = false)
        {
            int textLeft =
                left + ((width - text.Length) / 2);

            WriteText(textLeft, top, text, color, bold: bold);
        }

        public static void WriteGradientCentered(
            int left,
            int top,
            int width,
            string text)
        {
            string[] words = text.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            int position =
                left + ((width - text.Length) / 2);

            for (int i = 0; i < words.Length; i++)
            {
                string color = i switch
                {
                    0 when words.Length == 1 => Gold,

                    0 => GoldLight,

                    _ when i == words.Length - 1 => GoldDark,

                    _ => Gold
                };

                WriteText(
                    position,
                    top,
                    words[i],
                    color,
                    bold: true);

                position += words[i].Length + 1;
            }
        }

        public static void DrawFrame(
            int left,
            int top,
            int width,
            int height)
        {
            WriteText(
                left,
                top,
                "╭" + new string('─', width - 2) + "╮",
                Border);

            for (int row = 1; row < height - 1; row++)
            {
                WriteText(left, top + row, "│", Border);

                WriteText(
                    left + width - 1,
                    top + row,
                    "│",
                    Border);
            }

            WriteText(
                left,
                top + height - 1,
                "╰" + new string('─', width - 2) + "╯",
                Border);
        }

        public static void DrawDivider(
            int left,
            int top,
            int width)
        {
            const string segment = "━━━━━━";

            int position =
                left + ((width - (segment.Length * 3)) / 2);

            WriteText(position, top, segment, GoldLight);

            WriteText(
                position + segment.Length,
                top,
                segment,
                Gold);

            WriteText(
                position + (segment.Length * 2),
                top,
                segment,
                GoldDark);
        }

        private static void DrawMenuOption(
            int left,
            int top,
            string option,
            bool isSelected)
        {
            string text = isSelected
                ? $" ❯ {option}"
                : $"   {option}";

            text = text.PadRight(OptionWidth);

            WriteText(
                left,
                top,
                text,
                isSelected ? SelectionText : Text,
                isSelected ? Accent : Background,
                isSelected);
        }

        public static string ShowMenu(
            string title,
            string subtitle,
            IReadOnlyList<string> options,
            ref int selectedIndex,
            string escapeOption,
            string? description = null,
            string? author = null,
            string? hint = null)
        {
            if (options.Count == 0)
            {
                throw new ArgumentException(
                    "The menu must contain at least one option.",
                    nameof(options));
            }

            selectedIndex = Math.Clamp(
                selectedIndex,
                0,
                options.Count - 1);

            PrepareScreen();

            if (Console.WindowWidth < PanelWidth + 2 ||
                Console.WindowHeight < PanelHeight + 2 ||
                options.Count > 7)
            {
                return AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[bold {Gold}]{Markup.Escape(title)}[/]")
                        .PageSize(Math.Min(options.Count, 7))
                        .AddChoices(options));
            }

            int left =
                (Console.WindowWidth - PanelWidth) / 2;

            int top =
                (Console.WindowHeight - PanelHeight) / 2;

            bool hasAuthor =
                !string.IsNullOrWhiteSpace(author);

            int firstOptionRow = hasAuthor ? 11 : 10;
            int optionSpacing = !hasAuthor && options.Count <= 3 ? 2 : 1;

            DrawFrame(left, top, PanelWidth, PanelHeight);

            WriteGradientCentered(
                left,
                top + 2,
                PanelWidth,
                title);

            WriteCentered(
                left,
                top + (hasAuthor ? 3 : 4),
                PanelWidth,
                description ?? "",
                Text);

            DrawDivider(
                left,
                top + (hasAuthor ? 5 : 6),
                PanelWidth);

            if (hasAuthor)
            {
                WriteGradientCentered(
                    left,
                    top + 7,
                    PanelWidth,
                    author!);
            }

            WriteCentered(
                left,
                top + (hasAuthor ? 9 : 8),
                PanelWidth,
                subtitle,
                Gold,
                true);

            if (!hasAuthor && !string.IsNullOrWhiteSpace(hint))
            {
                WriteCentered(
                    left,
                    top + 17,
                    PanelWidth,
                    hint,
                    Muted);
            }

            WriteCentered(
                left,
                top + 19,
                PanelWidth,
                $"↑ ↓ Navigate   |   Enter Select   |   Esc {escapeOption}",
                Muted);

            int menuLeft =
                left + ((PanelWidth - OptionWidth) / 2);

            for (int i = 0; i < options.Count; i++)
            {
                DrawMenuOption(
                    menuLeft,
                    top + firstOptionRow + (i * optionSpacing),
                    options[i],
                    i == selectedIndex);
            }

            while (true)
            {
                ConsoleKey key =
                    Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.Enter)
                {
                    return options[selectedIndex];
                }

                if (key == ConsoleKey.Escape)
                {
                    return escapeOption;
                }

                int nextIndex = key switch
                {
                    ConsoleKey.UpArrow =>
                        (selectedIndex - 1 + options.Count)
                        % options.Count,

                    ConsoleKey.DownArrow =>
                        (selectedIndex + 1)
                        % options.Count,

                    _ => selectedIndex
                };

                if (nextIndex == selectedIndex)
                {
                    continue;
                }

                DrawMenuOption(
                    menuLeft,
                    top + firstOptionRow + (selectedIndex * optionSpacing),
                    options[selectedIndex],
                    false);

                selectedIndex = nextIndex;

                DrawMenuOption(
                    menuLeft,
                    top + firstOptionRow + (selectedIndex * optionSpacing),
                    options[selectedIndex],
                    true);
            }
        }

        public static void ShowNotice(
            string title,
            string message)
        {
            PrepareScreen();

            if (Console.WindowWidth < PanelWidth + 2 ||
                Console.WindowHeight < PanelHeight + 2)
            {
                AnsiConsole.MarkupLine(
                    $"[bold {Gold}]{Markup.Escape(title)}[/]");

                AnsiConsole.MarkupLine(
                    $"\n[{Text}]{Markup.Escape(message)}[/]");

                AnsiConsole.MarkupLine(
                    $"\n[{Muted}]Press any key to continue...[/]");

                _ = Console.ReadKey(intercept: true);
                return;
            }

            int left =
                (Console.WindowWidth - PanelWidth) / 2;

            int top =
                (Console.WindowHeight - PanelHeight) / 2;

            DrawFrame(left, top, PanelWidth, PanelHeight);

            WriteGradientCentered(
                left,
                top + 2,
                PanelWidth,
                title);

            DrawDivider(left, top + 5, PanelWidth);

            WriteCentered(
                left,
                top + 9,
                PanelWidth,
                "VEHICLE TYPE SELECTED",
                Gold,
                true);

            WriteCentered(
                left,
                top + 11,
                PanelWidth,
                message,
                Text);

            WriteCentered(
                left,
                top + 19,
                PanelWidth,
                "Press any key to return to Main Menu",
                Muted);

            _ = Console.ReadKey(intercept: true);
        }
        public static Panel CreatePanel(
            IRenderable content,
            string? title = null,
            int? width = null,
            Color? borderColor = null,
            Padding? padding = null)
        {
            var panel = new Panel(content)
            {
                Border = BoxBorder.Rounded,

                BorderStyle = new Style(
                    foreground: borderColor ?? BorderColor),

                Padding = padding ?? new Padding(1, 0, 1, 0)
            };

            if (!string.IsNullOrWhiteSpace(title))
            {
                panel.Header = new PanelHeader(
                    $"[bold {Gold}] {Markup.Escape(title)} [/]");
            }

            if (width.HasValue)
            {
                panel.Width = width.Value;
            }

            return panel;
        }
        public static void WriteCenteredPanel(Panel panel)
        {
            ArgumentNullException.ThrowIfNull(panel);

            AnsiConsole.Write(
                Align.Center(panel));
        }
    }
}