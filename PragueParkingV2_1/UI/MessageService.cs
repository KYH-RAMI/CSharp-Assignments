using Spectre.Console;

namespace PragueParkingV2_1.UI
{
    internal static class MessageService
    {
        public static void ShowError(
            string message,
            bool waitForKey = false)
        {
            ShowPanel("ERROR", message, Color.Red, waitForKey);
        }

      

        public static void ShowSuccess(
            string message,
            bool waitForKey = false)
        {
            ShowPanel("SUCCESS", message, Color.Green, waitForKey);
        }

        public static void ShowWarning(
            string message,
            bool waitForKey = false)
        {
            ShowPanel("WARNING", message, Color.Yellow, waitForKey);
        }

        public static void ShowInfo(
            string message,
            bool waitForKey = false)
        {
            ShowPanel(
                "INFO",
                message,
                AppTheme.BorderColor,
                waitForKey);
        }

        private static void ShowPanel(
            string title,
            string message,
            Color color,
            bool waitForKey)
        {
            var content = new Markup(
                Markup.Escape(message));

            var panel = AppTheme.CreatePanel(
                content,
                title: title,
                width: 62,
                borderColor: color);

            AppTheme.WriteCenteredPanel(panel);

            if (waitForKey)
            {
                const int formWidth = 62;

                int leftMargin = Math.Max(
                    0,
                    (Console.WindowWidth - formWidth) / 2);

                string padding = new string(' ', leftMargin);

                AnsiConsole.WriteLine();

                AnsiConsole.MarkupLine(
                    $"{padding}[{AppTheme.Muted}]Press any key to continue...[/]");

                Console.ReadKey(intercept: true);
            }
        }
    }
}