using Spectre.Console;

namespace PragueParkingV2.UI
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
                borderColor: color);

            AnsiConsole.Write(panel);

            if (waitForKey)
            {
                AnsiConsole.MarkupLine(
                    $"\n[{AppTheme.Muted}]Press any key to continue...[/]");

                Console.ReadKey(intercept: true);
            }
        }
    }
}