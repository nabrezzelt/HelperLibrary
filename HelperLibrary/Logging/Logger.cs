using System;

namespace HelperLibrary.Logging
{
    public class Log
    {
        private const ConsoleColor ColorFatal = ConsoleColor.Red;
        private const ConsoleColor ColorError = ConsoleColor.DarkRed;
        private const ConsoleColor ColorWarn = ConsoleColor.DarkYellow;
        private const ConsoleColor ColorDebug = ConsoleColor.DarkCyan;

        public static void Debug(string message)
        {
            ClearCurrentConsoleLine();
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ColorDebug;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void Info(string message, bool desktopClient = false)
        {
            ClearCurrentConsoleLine();
            Console.WriteLine(message);
        }

        public static void Warn(string message)
        {
            ClearCurrentConsoleLine();
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ColorWarn;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void Error(string message)
        {
            ClearCurrentConsoleLine();
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ColorError;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void Fatal(string message)
        {
            ClearCurrentConsoleLine();
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ColorFatal;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }

        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }

    public interface ILog
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
    }
}
