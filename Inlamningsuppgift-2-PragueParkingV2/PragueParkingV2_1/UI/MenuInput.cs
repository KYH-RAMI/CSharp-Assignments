using System.Text;
using Spectre.Console;

namespace PragueParkingV2_1.UI
{
    internal static class MenuInput
    {
        public static string? ReadText(
            string label,
            int leftMargin,
            int maxLength = 30)
        {
            Console.SetCursorPosition(
                leftMargin,
                Console.CursorTop);

            AnsiConsole.Markup(
                $"[{AppTheme.Gold}]{Markup.Escape(label)}:[/] ");

            var input = new StringBuilder();

            while (true)
            {
                ConsoleKeyInfo key =
                    Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        Console.WriteLine();
                        return null;

                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        return input.ToString().Trim();

                    case ConsoleKey.Backspace:
                        if (input.Length > 0)
                        {
                            input.Length--;
                            Console.Write("\b \b");
                        }

                        continue;
                }

                if (char.IsControl(key.KeyChar) ||
                    input.Length >= maxLength)
                {
                    continue;
                }

                input.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }

        public static bool Confirm(
            string label,
            int leftMargin)
        {
            Console.SetCursorPosition(
                leftMargin,
                Console.CursorTop);

            AnsiConsole.Markup(
                $"[{AppTheme.Gold}]{Markup.Escape(label)}[/] " +
                $"[{AppTheme.Muted}](y/N):[/] ");

            while (true)
            {
                ConsoleKeyInfo key =
                    Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.Y:
                        Console.WriteLine("Y");
                        return true;

                    case ConsoleKey.N:
                    case ConsoleKey.Enter:
                    case ConsoleKey.Escape:
                        Console.WriteLine();
                        return false;
                }
            }
        }
    }
}