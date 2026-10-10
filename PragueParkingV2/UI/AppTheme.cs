using Spectre.Console;

namespace PragueParkingV2.UI
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

        public static readonly Color BorderColor =
            new Color(68, 136, 190);

        public static void PrepareScreen()
        {
            Console.CursorVisible = false;

            Console.Write("\u001b]11;rgb:0B/2D/50\u001b\\");

            Console.Write("\u001b[0m");

            AnsiConsole.Clear();
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

        public static void RestoreScreen()
        {
            Console.Write("\u001b[0m");
            Console.CursorVisible = true;
        }
    }
}