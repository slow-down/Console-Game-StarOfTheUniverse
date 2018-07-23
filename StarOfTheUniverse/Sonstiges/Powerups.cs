using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Powerups
{
    public List<Tuple<Powerup, Point>> listPowerup = new List<Tuple<Powerup, Point>>();

    private Stopwatch timer = new Stopwatch();
    private int nextSpawn = 0;

    private ConsoleGraphics gr;
    private Size gameSize;

    public Powerups(ConsoleGraphics gr, Size gameSize)
    {
        this.gr = gr;
        this.gameSize = gameSize;
        timer.Start();
    }

    public void SpawnPowerup()
    {
        if (timer.Elapsed.TotalSeconds < nextSpawn) return;

        // Nächster Spawn in 10-60 Sekunden
        nextSpawn = ConsoleGraphics.rnd.Next((int)timer.Elapsed.TotalSeconds + 10, (int)timer.Elapsed.TotalSeconds + 30);

        // Zufällig ein Powerup auswählen
        Powerup randomPowerup = (Powerup)ConsoleGraphics.rnd.Next(1, Enum.GetNames(typeof(Powerup)).Length + 1);

        // Einen zufälligen Punkt auf dem Spielfeld suchen und in der Liste speichern
        listPowerup.Add(new Tuple<Powerup, Point>(randomPowerup, GetRandomLocation()));
    }

    private Point GetRandomLocation()
    {
        bool free = false;

        int xOffset = ((gr.CONSOLE_WIDTH - gameSize.Width) / 2) + 2;
        int yOffset = ((gr.CONSOLE_HEIGHT - gameSize.Height) / 2) + 2;

        Point p = new Point(0, 0);

        do
        {
            int x = ConsoleGraphics.rnd.Next(xOffset, xOffset + gameSize.Width - 2);
            int y = ConsoleGraphics.rnd.Next(yOffset, yOffset + gameSize.Height - 2);

            char ch = gr.ReadFromBuffer(x, y).Char.UnicodeChar;

            // Wenn Platz frei
            if (ch == '\0' || ch == ' ')
            {
                free = true;
                p.X = x;
                p.Y = y;
            }

        } while (!free);

        return p;
    }

    public void Draw()
    {
        foreach (var powerup in listPowerup)
        {
            char powerUpChar = ' ';
            ConsoleAttribute powerupColor = ConsoleAttribute.FG_WHITE;

            switch (powerup.Item1)
            {
                case Powerup.Health: powerUpChar = PowerupChars.Health; powerupColor = PowerupColors.Health; break;
                case Powerup.Points: powerUpChar = PowerupChars.Points; powerupColor = PowerupColors.Points; break;
                case Powerup.Defense: powerUpChar = PowerupChars.Defense; powerupColor = PowerupColors.Defense; break;
            }

            gr.WriteToBuffer(powerup.Item2.X, powerup.Item2.Y, powerUpChar, powerupColor);
        }
    }

}