using Spectre.Console;

namespace PragueParkingV2.UI
{
    internal static class MessageService
    {
        public static void ShowError(string message)
        {
            ShowPanel("FEL", message, Color.Red);
        }

        public static void ShowSuccess(string message)
        {
            ShowPanel("OK", message, Color.Green);
        }

        public static void ShowWarning(string message)
        {
            ShowPanel("VARNING", message, Color.Yellow);
        }

        public static void ShowInfo(string message)
        {
            ShowPanel("INFO", message, Color.Blue);
        }

        private static void ShowPanel(string title, string message, Color color)
        {
            var content = new Markup(
                $"[bold]{Markup.Escape(message)}[/]");

            var panel = new Panel(content)
            {
                Header = new PanelHeader($" {title} "),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(foreground: color),
                Padding = new Padding(1, 0, 1, 0)
            };

            AnsiConsole.Write(panel);
        }
    }
}