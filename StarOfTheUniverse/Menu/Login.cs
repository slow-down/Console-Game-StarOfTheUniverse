using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Login
{
    private const string PASSWORD = "DufterMan";
    private string input = "";

    private string[] STAR_OF_THE_UNIVERSE_SMALL =
    {
        @"  _____ _                      __   _   _            _    _       _                         ",
        @" / ____| |                    / _| | | | |          | |  | |     (_)                        ",
        @"| (___ | |_ __ _ _ __    ___ | |_  | |_| |__   ___  | |  | |_ __  ___   _____ _ __ ___  ___ ",
        @" \___ \| __/ _` | '__|  / _ \|  _| | __| '_ \ / _ \ | |  | | '_ \| \ \ / / _ \ '__/ __|/ _ \",
        @" ____) | || (_| | |    | (_) | |   | |_| | | |  __/ | |__| | | | | |\ V /  __/ |  \__ \  __/",
        @"|_____/ \__\__,_|_|     \___/|_|    \__|_| |_|\___|  \____/|_| |_|_| \_/ \___|_|  |___/\___|"
    };

    private bool loggedIn = false;

    private ConsoleGraphics gr;

    public Login(ConsoleGraphics gr)
    {
        this.gr = gr;
    }

    /// <summary>
    /// Zeigt den Loginscreen auf der Konsole an
    /// </summary>
    public void Show()
    {
        while (!loggedIn)
        {
            gr.ClearBuffer();

            // Positioniert die Überschrift mittig in der Konsole
            for (int i = 0; i < STAR_OF_THE_UNIVERSE_SMALL.Length; i++)
            {
                gr.WriteToBuffer((gr.CONSOLE_WIDTH - STAR_OF_THE_UNIVERSE_SMALL[i].Length) / 2, i + 5, STAR_OF_THE_UNIVERSE_SMALL[i], ConsoleAttribute.FG_GREEN);
            }

            // Positioniert das wort "Passwort: " zentriert in Abhängigkeit von der Passwortlänge * 2
            gr.WriteToBuffer((gr.CONSOLE_WIDTH - ("Passwort: " + PASSWORD + PASSWORD).Length) / 2, (gr.CONSOLE_HEIGHT / 2) - 1, "Passwort: ");

            for (int i = 0; i < PASSWORD.Length; i++)
            {
                if (input.Length > i)
                {
                    // Für jedes eingegebene Zeichen soll ein "*" angezeigt werden
                    gr.WriteToBuffer("* ");
                }
                else
                {
                    // Für jedes leere Zeichen soll ein "_" angezeigt werden
                    gr.WriteToBuffer("_ ");
                }
            }

            // Schreibt den gesamten Buffer in die Konsole
            gr.DrawBufferToConsole();

            ConsoleKeyInfo kInfo = Console.ReadKey(true);
            if (kInfo.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    // Das letzte Zeichen löschen
                    input = input.Remove(input.Length - 1, 1);
                }
            }
            else if (input.Length < PASSWORD.Length && kInfo.KeyChar >= 33 && kInfo.KeyChar <= 126)
            {
                // neues Zeichen hinzufügen
                input += kInfo.KeyChar;
            }
            else if (kInfo.Key == ConsoleKey.Enter)
            {
                if (input == PASSWORD)
                {
                    loggedIn = true; // Passwort richtig, gehe raus aus der Schleife
                }
                else
                {
                    input = "";
                }
            }
            else if (kInfo.Key == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }
        }

    }



}
