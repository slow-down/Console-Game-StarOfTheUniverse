using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class Highscore
{
    private const string fileHighscoreNormal = "HighscoreNormal.txt";
    private const string fileHighscoreEndlos = "HighscoreEndlos.txt";
    private const string fileHighscoreHardcore = "HighscoreHardcore.txt";
    private static string chosenHighscore = "";

    public static string[] HIGHSCORE_LOGO_SMALL =
    {
       @" _    _ _____ _____ _    _  _____  _____ ____  _____  ______ ",
       @"| |  | |_   _/ ____| |  | |/ ____|/ ____/ __ \|  __ \|  ____|",
       @"| |__| | | || |  __| |__| | (___ | |   | |  | | |__) | |__   ",
       @"|  __  | | || | |_ |  __  |\___ \| |   | |  | |  _  /|  __|  ",
       @"| |  | |_| || |__| | |  | |____) | |___| |__| | | \ \| |____ ",
       @"|_|  |_|_____\_____|_|  |_|_____/ \_____\____/|_|  \_\______|"
    };

    public static string[] HIGHSCORE_LOGO =
    {
        @"$$\   $$\ $$$$$$\  $$$$$$\  $$\   $$\  $$$$$$\   $$$$$$\   $$$$$$\  $$$$$$$\  $$$$$$$$\ ",
        @"$$ |  $$ |\_$$  _|$$  __$$\ $$ |  $$ |$$  __$$\ $$  __$$\ $$  __$$\ $$  __$$\ $$  _____|",
        @"$$ |  $$ |  $$ |  $$ /  \__|$$ |  $$ |$$ /  \__|$$ /  \__|$$ /  $$ |$$ |  $$ |$$ |      ",
        @"$$$$$$$$ |  $$ |  $$ |$$$$\ $$$$$$$$ |\$$$$$$\  $$ |      $$ |  $$ |$$$$$$$  |$$$$$\    ",
        @"$$  __$$ |  $$ |  $$ |\_$$ |$$  __$$ | \____$$\ $$ |      $$ |  $$ |$$  __$$< $$  __|   ",
        @"$$ |  $$ |  $$ |  $$ |  $$ |$$ |  $$ |$$\   $$ |$$ |  $$\ $$ |  $$ |$$ |  $$ |$$ |      ",
        @"$$ |  $$ |$$$$$$\ \$$$$$$  |$$ |  $$ |\$$$$$$  |\$$$$$$  | $$$$$$  |$$ |  $$ |$$$$$$$$\ ",
        @"\__|  \__|\______| \______/ \__|  \__| \______/  \______/  \______/ \__|  \__|\________|"
    };

    private Spielmodus spielmodus;
    private ConsoleGraphics gr;

    public Highscore(ConsoleGraphics gr, Spielmodus spielmodus)
    {
        this.gr = gr;
        this.spielmodus = spielmodus;

        switch (this.spielmodus)
        {
            case Spielmodus.Normal:
                chosenHighscore = fileHighscoreNormal;
                break;

            case Spielmodus.Endlos:
                chosenHighscore = fileHighscoreEndlos;
                break;

            case Spielmodus.Hardcore:
                chosenHighscore = fileHighscoreHardcore;
                break;
        }

        if (!File.Exists(chosenHighscore))
        {
            File.Create(chosenHighscore);
        }
    }

    /// <summary>
    /// Zeichnet die Highscore Liste in die Konsole
    /// </summary>
    /// <param name="spielmodus">Der Spielmodus aus dem der Highscore angezeigt werden soll</param>
    public void Draw()
    {
        gr.ClearBuffer();

        // Positioniert das Logo mittig in der Konsole
        for (int i = 0; i < HIGHSCORE_LOGO.Length; i++)
        {
            // Offset 5 damit es nicht ganz oben am Rand ist sondern etwas drunter
            gr.WriteToBuffer((gr.CONSOLE_WIDTH - HIGHSCORE_LOGO[i].Length) / 2, i + 5, HIGHSCORE_LOGO[i], ConsoleAttribute.FG_RED);
        }

        ScoreRow[] scores = LoadScores();

        // Sortiert die Highscores
        scores = scores.OrderByDescending(row => row.Score).ToArray();

        for (int i = 0; i < scores.Length; i++)
        {
            if (i >= 10) break; // nur die top 10 anzeigen

            // Ziel Format:  1.     Name    Punkte
            string formatted = string.Format("{0,2}. {1,-20} {2,7}", i + 1, scores[i].Name, scores[i].Score);

            // Mittig in der Konsole platzieren und dabei beachten, dass es auch je nach Anzahl der Scores weiter oben oder unten ist
            gr.SetCursorPosition((gr.CONSOLE_WIDTH - formatted.Length) / 2, (gr.CONSOLE_HEIGHT / 2) + i - ((scores.Length > 10 ? 10 : scores.Length) / 2));

            if (i % 2 == 0)
            {
                gr.WriteToBuffer(formatted, ConsoleAttribute.FG_CYAN);
            }
            else
            {
                gr.WriteToBuffer(formatted, ConsoleAttribute.FG_WHITE);
            }
        }

        // Draw Border
        if (scores.Length > 0)
        {
            ConsoleAttribute borderColor = ConsoleAttribute.FG_BLUE_BRIGHT;

            int borderOffset = 3;
            int borderWidth = 35;

            int amount = scores.Length > 10 ? 10 : scores.Length;

            int x = ((gr.CONSOLE_WIDTH - borderWidth) / 2) - borderOffset;
            int y = ((gr.CONSOLE_HEIGHT - amount) / 2) - borderOffset;

            // left/right border
            for (int rowY = 0; rowY < amount + (borderOffset * 2); rowY++)
            {
                if (rowY == 0)
                {
                    // top left
                    gr.WriteToBuffer(x, y + rowY, ConsoleGraphics.EDGE_TOP_LEFT, borderColor);

                    // top right
                    gr.WriteToBuffer(x + borderWidth + (borderOffset + 1), y + rowY, ConsoleGraphics.EDGE_TOP_RIGHT, borderColor);
                }
                else if (rowY == (amount + (borderOffset * 2) - 1))
                {
                    // bottom left
                    gr.WriteToBuffer(x, y + rowY, ConsoleGraphics.EDGE_BOTTOM_LEFT, borderColor);

                    // bottom right
                    gr.WriteToBuffer(x + borderWidth + (borderOffset + 1), y + rowY, ConsoleGraphics.EDGE_BOTTOM_RIGHT, borderColor);
                }
                else
                {
                    // vertical left
                    gr.WriteToBuffer(x, y + rowY, ConsoleGraphics.VERTICAL_BORDER, borderColor);

                    // vertical right
                    gr.WriteToBuffer(x + borderWidth + (borderOffset + 1), y + rowY, ConsoleGraphics.VERTICAL_BORDER, borderColor);
                }
            }

            // top border
            gr.WriteToBuffer(x + 1, y, new string(ConsoleGraphics.HORIZONTAL_BORDER, borderWidth + (borderOffset * 2) - 3), borderColor);

            // bottom border
            gr.WriteToBuffer(x + 1, y + amount + (borderOffset * 2) - 1, new string(ConsoleGraphics.HORIZONTAL_BORDER, borderWidth + (borderOffset * 2) - 3), borderColor);
        }

        gr.DrawBufferToConsole();

        ConsoleKeyInfo kInfo;
        while (((kInfo = Console.ReadKey(true)).Key != ConsoleKey.Escape) && kInfo.Key != ConsoleKey.Backspace && kInfo.Key != ConsoleKey.Enter) ;
    }

    /// <summary>
    /// Lädt und gibt alle Scores aus der Highscoreliste zurück
    /// </summary>
    /// <returns>Eine unsortierte Liste aller Punktestände</returns>
    private ScoreRow[] LoadScores()
    {
        List<ScoreRow> scores = new List<ScoreRow>();

        string[] lines;
        try
        {
            // Alle Zeilen aus der Datei auslesen
            lines = File.ReadAllLines(chosenHighscore);
        }
        catch
        {
            return scores.ToArray();
        }

        // Durch jede Zeile iterieren
        for (int i = 0; i < lines.Length; i++)
        {
            string[] splitted = lines[i].Split(':'); // Highscore Datei sieht z.B. so aus: "Alexej:330" (links Name, rechts Punkte)

            if (splitted.Length > 1)
            {
                scores.Add(new ScoreRow(splitted[0], Convert.ToInt32(splitted[1])));
            }
        }

        return scores.ToArray();
    }

    /// <summary>
    /// Speichert den Namen und den Score in die jeweilige Highscore Liste des Spielmodus
    /// </summary>
    /// <returns>false, wenn es nicht gespeichert werden konnte</returns>
    public static bool SaveScore(string name, int score, Spielmodus spielmodus)
    {
        try
        {
            switch (spielmodus)
            {
                case Spielmodus.Normal:
                    File.AppendAllText(fileHighscoreNormal, string.Format("{0}:{1}\n", name, score));
                    break;
                case Spielmodus.Endlos:
                    File.AppendAllText(fileHighscoreEndlos, string.Format("{0}:{1}\n", name, score));
                    break;
                case Spielmodus.Hardcore:
                    File.AppendAllText(fileHighscoreHardcore, string.Format("{0}:{1}\n", name, score));
                    break;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Löscht alle Higscores
    /// </summary>
    public static void ResetHighscore()
    {
        try
        {
            File.Delete(fileHighscoreNormal);
        }
        catch { }

        try
        {
            File.Delete(fileHighscoreEndlos);
        }
        catch { }

        try
        {
            File.Delete(fileHighscoreHardcore);
        }
        catch { }
    }

    private class ScoreRow
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public ScoreRow(string name, int score)
        {
            this.Name = name;
            this.Score = score;
        }
    }

}