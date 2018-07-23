using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

public class Stars
{
    private Stopwatch timer = new Stopwatch();
    private long lastMove = 0;
    private ConsoleGraphics gr;

    public List<Point> listPoints = new List<Point>();
    private Size gameSize;

    private int speed = 1;

    public const char STAR_ICON = 'x';
    public const ConsoleAttribute Color = ConsoleAttribute.FG_YELLOW;

    public Stars(ConsoleGraphics gr, Size gameSize)
    {
        this.gr = gr;
        this.gameSize = gameSize;
        timer.Start();
    }

    /// <summary>
    /// Erstellt mehrere Sterne an zufälligen Position und speichert sie in einer Liste ab
    /// </summary>
    /// <param name="speed">Die Geschwindigkeit die jetzt gespielt wird</param>
    /// <param name="spielmodus">Der Spielmodus der zurzeit gespielt wird</param>
    public void SpawnStars(int speed, Spielmodus spielmodus)
    {
        this.speed = speed;

        this.listPoints.Clear();
        int amount = 0;

        if (spielmodus == Spielmodus.Normal)
        {
            // Fester Wert damit es im Normalmodus wegen der Zeit immer für jeden fair ist und nicht der eine nur 10 einsammeln muss under andere mehr
            amount = 20;
        }
        else if (spielmodus == Spielmodus.Endlos)
        {
            // Im Endlosmodus soll random zwischen 10 und 20 Sternen sein
            amount = ConsoleGraphics.rnd.Next(15, 31);
        }
        else if (spielmodus == Spielmodus.Hardcore)
        {
            amount = 50;
        }

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
    /// Bewegt jeden Stern aus der Liste in eine zufällige Richtung
    /// </summary>
    public void MoveStars(List<Point> listKryptonite)
    {
        // Vorerst nur jede Sekunde bewegen lassen
        if (timer.ElapsedMilliseconds - lastMove < (1000 / speed)) return;

        lastMove = timer.ElapsedMilliseconds;

        for (int i = 0; i < listPoints.Count; i++)
        {
            // Vorerst nur random links/rechts/oben/unten
            int richtung = ConsoleGraphics.rnd.Next(0, 4);

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
                        if (listKryptonite.Contains(listPoints[i]))
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
                        if (listKryptonite.Contains(listPoints[i]))
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
                        if (listKryptonite.Contains(listPoints[i]))
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
                        if (listKryptonite.Contains(listPoints[i]))
                        {
                            listPoints[i].Y++;
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Zeichnet jeden Stern aus der Liste in die Konsole
    /// </summary>
    public void Draw()
    {
        for (int i = 0; i < listPoints.Count; i++)
        {
            gr.WriteToBuffer(listPoints[i].X, listPoints[i].Y, STAR_ICON, Color);
        }
    }

}
