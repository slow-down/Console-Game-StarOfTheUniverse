using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Kryptonite
{
    private Stopwatch timer = new Stopwatch();
    private long lastMove = 0;
    private ConsoleGraphics gr;

    public List<Point> listPoints = new List<Point>();
    private Size gameSize;

    private int speed = 1;

    public const char KRYPTONITE_ICON = '*';
    public const ConsoleAttribute Color = ConsoleAttribute.FG_GREEN;

    public Kryptonite(ConsoleGraphics gr, Size gameSize)
    {
        this.gr = gr;
        this.gameSize = gameSize;
        timer.Start();
    }

    /// <summary>
    /// Erstellt mehrere Krptonite an zufälligen Position und speichert sie in einer Liste ab
    /// </summary>
    /// <param name="speed">Die Geschwindigkeit die jetzt gespielt wird</param>
    /// <param name="level">Das Level das jetzt gespielt wird</param>
    public void SpawnKryptonite(int speed, int level, Spielmodus spielmodus)
    {
        this.speed = speed;
        this.listPoints.Clear();

        int amountParam = 0;

        switch (spielmodus)
        {
            case Spielmodus.Normal: amountParam = 200; break;
            case Spielmodus.Endlos: amountParam = 200; break;
            case Spielmodus.Hardcore: amountParam = 50; break;
        }

        // Je nach Spielgröße soll mehr oder weniger Kryptonit spawnen (200 -> kleiner = mehr Kryptonit in einem Bereich, höher = weniger Kryptonit)
        int amount = ((gameSize.Width * gameSize.Height) / amountParam) + level; // jedes level 1 kryptonit mehr

        int xOffset = ((gr.CONSOLE_WIDTH - this.gameSize.Width) / 2) + 1;
        int yOffset = ((gr.CONSOLE_HEIGHT - this.gameSize.Height) / 2) + 1;

        for (int i = 0; i < amount; i++)
        {
            int tries = 0;
            Point p;

            do
            {
                // Versuche einen neuen Punkt zu erstellen, wenn dieser jedoch besetzt ist soll maximal 20 mal versucht werden.
                // Wichtig für den Endlosmodus, bei dem es bei höheren Leveln sehr viel mehr Kryptonit auf einer Fläche gibt
                if (tries > 20) return;

                int x = ConsoleGraphics.rnd.Next(2, gameSize.Width - 2) + xOffset;
                int y = ConsoleGraphics.rnd.Next(2, gameSize.Height - 2) + yOffset;
                p = new Point(x, y);

            } while (listPoints.Contains(p));

            listPoints.Add(p);
        }
    }

    /// <summary>
    /// Bewegt jedes Kryptonit aus der Liste in eine zufällige Richtung
    /// </summary>
    public void MoveKryptonite(List<Point> listStars)
    {
        // Vorerst nur jede Sekunde bewegen lassen
        if (timer.ElapsedMilliseconds - lastMove < (1000 / speed)) return;

        lastMove = timer.ElapsedMilliseconds;

        for (int i = 0; i < listPoints.Count; i++)
        {
            // Vorerst nur random links/rechts/oben/unten
            int richtung = ConsoleGraphics.rnd.Next(0, 5);

            int rightBound = (gr.CONSOLE_WIDTH + gameSize.Width) / 2;
            int bottomBound = (gr.CONSOLE_HEIGHT + gameSize.Height) / 2;
            int leftBound = (gr.CONSOLE_WIDTH - gameSize.Width) / 2;
            int topBound = (gr.CONSOLE_HEIGHT - gameSize.Height) / 2;

            switch (richtung)
            {
                // rechts
                case 0:
                    if (listPoints[i].X + 3 < rightBound)
                    {
                        listPoints[i].X++;
                        if (listStars.Contains(listPoints[i]))
                        {
                            listPoints[i].X--;
                        }
                    }
                    break;

                // unten
                case 1:
                    if (listPoints[i].Y + 2 < bottomBound)
                    {
                        listPoints[i].Y++;
                        if (listStars.Contains(listPoints[i]))
                        {
                            listPoints[i].Y--;
                        }
                    }
                    break;

                // links
                case 2:
                    if (listPoints[i].X - 1 > leftBound)
                    {
                        listPoints[i].X--;
                        if (listStars.Contains(listPoints[i]))
                        {
                            listPoints[i].X++;
                        }
                    }
                    break;

                // oben
                case 3:
                    if (listPoints[i].Y - 1 > topBound)
                    {
                        listPoints[i].Y--;
                        if (listStars.Contains(listPoints[i]))
                        {
                            listPoints[i].Y++;
                        }
                    }
                    break;

                // Nicht bewegen
                default:

                    break;
            }
        }
    }

    /// <summary>
    /// Zeichnet jedes Kryptonit aus der Liste in die Konsole
    /// </summary>
    public void Draw()
    {
        for (int i = 0; i < listPoints.Count; i++)
        {

            gr.WriteToBuffer(listPoints[i].X, listPoints[i].Y, KRYPTONITE_ICON, Color);
        }
    }

}
