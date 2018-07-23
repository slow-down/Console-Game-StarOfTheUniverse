using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Game
{
    private Stopwatch timer = new Stopwatch();

    private int level = 1;
    private int speed = 1;
    private int gesammelteSterne = 0;
    private Spielmodus spielmodus;
    private ConsoleGraphics gr;

    private const float FPS = 20;
    private readonly int GAME_WIDTH = 160;
    private readonly int GAME_HEIGHT = 60;
    private const int menuWidth = 30;
    private const int menuHeight = 7;

    private bool gameOver = false;

    private Superman superman;
    private Stars stars;
    private Kryptonite kryptonite;
    private Powerups powerups;
    private bool abbruch;


    ConsoleAttribute borderColor = ConsoleAttribute.FG_BLUE_BRIGHT;

    public Game(ConsoleGraphics gr)
    {
        this.gr = gr;
        this.GAME_WIDTH = gr.CONSOLE_WIDTH;
        this.GAME_HEIGHT = gr.CONSOLE_HEIGHT - 20;
    }

    /// <summary>
    /// Initialisiert alles und startet das Spiel
    /// </summary>
    /// <param name="spielmodus">Beschreibt welcher Spielmodus gespielt werden soll</param>
    public void Start(Spielmodus spielmodus)
    {
        this.spielmodus = spielmodus;
        timer.Start();
        superman = new Superman(gr, new Size(GAME_WIDTH, GAME_HEIGHT));
        stars = new Stars(gr, new Size(GAME_WIDTH, GAME_HEIGHT));
        kryptonite = new Kryptonite(gr, new Size(GAME_WIDTH, GAME_HEIGHT));
        powerups = new Powerups(gr, new Size(GAME_WIDTH, GAME_HEIGHT));

        if (this.spielmodus == Spielmodus.Normal)
        {
            // Normalmodus etwas schneller machen damit es nicht zu langweilig wird
            this.speed = 3;
        }
        else if (this.spielmodus == Spielmodus.Endlos)
        {
            this.speed = 3;
        }
        else if (this.spielmodus == Spielmodus.Hardcore)
        {
            this.speed = 6;
        }

        stars.SpawnStars(this.speed, spielmodus);
        kryptonite.SpawnKryptonite(this.speed, this.level, this.spielmodus);
        powerups.SpawnPowerup();

        Draw();
    }

    /// <summary>
    ///  Main Game Loop
    ///  Zeichnet alles was auf der Konsole angezeigt werden soll,
    ///  solange man nicht abgebrochen oder verloren hat
    /// </summary>
    private void Draw()
    {
        // Game Loop
        while (!gameOver && !abbruch)
        {
            gr.ClearBuffer();

            GetKeyInput();
            powerups.SpawnPowerup();

            stars.MoveStars(kryptonite.listPoints);
            kryptonite.MoveKryptonite(stars.listPoints);

            DrawBorder();
            DrawUI();
            DrawLegend();

            stars.Draw();
            kryptonite.Draw();
            powerups.Draw();
            superman.Draw();

            CheckIntersection();

            gr.DrawBufferToConsole();
            Thread.Sleep((int)(1000 / FPS));
        }

        // Nur Game Over Screen zeigen, wenn nicht abgebrochen wurde
        if (gameOver)
        {
            // Spiel vorbei (Egal ob verloren oder gewonnen, selber Screen)
            GameOverScreen();
        }
    }

    /// <summary>
    /// Prüft ob Superman in irgendeinen Punkt gestoßen ist
    /// </summary>
    private void CheckIntersection()
    {
        // Sterne prüfen
        foreach (Point star in stars.listPoints)
        {
            if (superman.Intersects(star))
            {
                // Stern eingesammelt
                gesammelteSterne++;
                if (this.spielmodus == Spielmodus.Normal)
                {
                    // Normaler Modus immer 10 Punkte
                    superman.Score += 10;
                }
                else if (this.spielmodus == Spielmodus.Endlos)
                {
                    // Im Endlosmodus zufällig viele Punkte (zwischen 10 und 30) mal das jetztige Level. Desto höher das Level, desto mehr Punkte bekommt man
                    superman.Score += ConsoleGraphics.rnd.Next(10, 31) * this.level;
                }
                else if (this.spielmodus == Spielmodus.Hardcore)
                {
                    // Hardcore Modus immer 10 Punkte
                    superman.Score += 10;
                }

                stars.listPoints.Remove(star); // Stern aus Liste löschen
                break; // aus methode raus, da wenn wir einen Stern einfangen, und es nicht noch unnötig weiter suchen soll da wir es ja schon gefunden haben
            }
        }

        // Alle Sterne gefangen
        if (stars.listPoints.Count <= 0)
        {
            switch (this.spielmodus)
            {
                case Spielmodus.Normal:
                    this.gameOver = true;
                    return; ;
                case Spielmodus.Endlos:
                    NextLevel();
                    break;
                case Spielmodus.Hardcore:
                    
                    this.gameOver = true;
                    return;
            }
        }

        // Powerups prüfen
        foreach (var powerup in powerups.listPowerup)
        {
            if (superman.Intersects(powerup.Item2))
            {
                switch (powerup.Item1)
                {
                    case Powerup.Health:
                        superman.HP += ConsoleGraphics.rnd.Next(10, 30);
                        if (superman.HP > 100) superman.HP = 100;
                        break;
                    case Powerup.Points:
                        superman.Score += ConsoleGraphics.rnd.Next(10, 20);
                        break;
                    case Powerup.Defense:
                        superman.Defense++;
                        break;
                }

                powerups.listPowerup.Remove(powerup); // Powerup wurde eingesammelt, lösche Element aus Liste
                break;
            }
        }


        // Kryptonit prüfen
        foreach (Point kryptonite in kryptonite.listPoints)
        {
            if (superman.Intersects(kryptonite))
            {
                if (superman.Defense > 0)
                {
                    superman.Defense--;
                }
                else
                {
                    if (this.spielmodus == Spielmodus.Normal)
                    {
                        // Normaler Modus immer minus 34 Leben
                        superman.HP -= 34;
                    }
                    else if (this.spielmodus == Spielmodus.Endlos)
                    {
                        // Im Endlosmodus verliert man zufällig zwischen 20 und 40 Leben
                        superman.HP -= ConsoleGraphics.rnd.Next(20, 41);
                    }
                    else if (this.spielmodus == Spielmodus.Hardcore)
                    {
                        superman.HP -= 100;
                    }
                }


                if (superman.HP <= 0)
                {
                    superman.HP = 0;
                    gameOver = true;
                }

                this.kryptonite.listPoints.Remove(kryptonite); // Kryptonit aus Liste löschen
                break;
            }
        }

        // Sonstiges ??
    }

    /// <summary>
    /// Zeichnet den Rand des Spiels
    /// </summary>
    private void DrawBorder()
    {
        int x = (gr.CONSOLE_WIDTH - GAME_WIDTH) / 2;
        int y = (gr.CONSOLE_HEIGHT - GAME_HEIGHT) / 2;

        // Top
        gr.WriteToBuffer(x + 1, y, new string(ConsoleGraphics.HORIZONTAL_BORDER, GAME_WIDTH - 2), borderColor);

        // Bottom Border
        gr.WriteToBuffer(x + 1, y + GAME_HEIGHT, new string(ConsoleGraphics.HORIZONTAL_BORDER, GAME_WIDTH - 2), borderColor);

        // Left/Right Border
        for (int i = 0; i < GAME_HEIGHT + 1; i++)
        {
            if (i == 0)
            {
                // top left
                gr.WriteToBuffer(x, y + i, ConsoleGraphics.EDGE_TOP_LEFT, borderColor);

                // top right
                gr.WriteToBuffer(x + GAME_WIDTH - 1, y + i, ConsoleGraphics.EDGE_TOP_RIGHT, borderColor);
            }
            else if (i == GAME_HEIGHT)
            {
                // bottom left
                gr.WriteToBuffer(x, y + i, ConsoleGraphics.CROSS_TO_RIGHT, borderColor);

                // bottom right
                gr.WriteToBuffer(x + GAME_WIDTH - 1, y + i, ConsoleGraphics.CROSS_TO_LEFT, borderColor);
            }
            else
            {
                // left
                gr.WriteToBuffer(x, y + i, ConsoleGraphics.VERTICAL_BORDER, borderColor);

                // right
                gr.WriteToBuffer(x + GAME_WIDTH - 1, y + i, ConsoleGraphics.VERTICAL_BORDER, borderColor);
            }
        }
    }

    /// <summary>
    /// Zeichnet das "User Interface" des Spiels
    /// </summary>
    private void DrawUI()
    {
        // Unter dem Spielfeld soll gezeichnet werden
        int x = (gr.CONSOLE_WIDTH - GAME_WIDTH) / 2;
        int y = (gr.CONSOLE_HEIGHT + GAME_HEIGHT) / 2 + 1;

        // left/right border
        for (int i = 0; i < menuHeight; i++)
        {
            if (i == menuHeight - 1)
            {
                // bottom left
                gr.WriteToBuffer(x, y + i, ConsoleGraphics.EDGE_BOTTOM_LEFT, borderColor);

                // bottom right
                gr.WriteToBuffer(x + GAME_WIDTH - 1, y + i, ConsoleGraphics.EDGE_BOTTOM_RIGHT, borderColor);
            }
            else
            {
                // left border
                gr.WriteToBuffer(x, y + i, ConsoleGraphics.VERTICAL_BORDER, borderColor);

                // right border
                gr.WriteToBuffer(x + GAME_WIDTH - 1, y + i, ConsoleGraphics.VERTICAL_BORDER, borderColor);
            }
        }

        // bottom border
        gr.WriteToBuffer(x + 1, y + menuHeight - 1, new string(ConsoleGraphics.HORIZONTAL_BORDER, GAME_WIDTH - 2), borderColor);

        // Stats anzeigen
        gr.WriteToBuffer(x + 2, y, string.Format("Score: {0}", CalculateScore()), PowerupColors.Points);
        gr.WriteToBuffer(x + 2, y + 1, string.Format("HP: {0}", superman.HP), PowerupColors.Health);
        gr.WriteToBuffer(x + 2, y + 2, string.Format("Verteidigung: {0}", new string(ConsoleGraphics.BLOCK_SOLID_SLIM, superman.Defense)), PowerupColors.Defense);
        gr.WriteToBuffer(x + 2, y + 3, string.Format("Level: {0}", this.level));
        gr.WriteToBuffer(x + 2, y + 4, string.Format("Zeit: {0}", (int)this.timer.Elapsed.TotalSeconds));
        gr.WriteToBuffer(x + 2, y + 5, string.Format("Sterne: {0}", this.gesammelteSterne), Stars.Color);
    }

    /// <summary>
    /// Zeichnet die Legende innerhalb des User Interface
    /// </summary>
    private void DrawLegend()
    {
        // Unter dem Spielfeld soll gezeichnet werden
        int x = ((gr.CONSOLE_WIDTH + GAME_WIDTH) / 2) - 20;
        int y = ((gr.CONSOLE_HEIGHT + GAME_HEIGHT) / 2) + 1;


        gr.WriteToBuffer(x, y, "Legende: ");

        gr.WriteToBuffer(x, y + 1, "Sterne = ");
        gr.WriteToBuffer(Stars.STAR_ICON, Stars.Color);

        gr.WriteToBuffer(x, y + 2, "Kryptonit = ");
        gr.WriteToBuffer(Kryptonite.KRYPTONITE_ICON, Kryptonite.Color);

        gr.WriteToBuffer(x, y + 3, "Health = ");
        gr.WriteToBuffer(PowerupChars.Health, PowerupColors.Health);

        gr.WriteToBuffer(x, y + 4, "Points = ");
        gr.WriteToBuffer(PowerupChars.Points, PowerupColors.Points);

        gr.WriteToBuffer(x, y + 5, "Defense = ");
        gr.WriteToBuffer(PowerupChars.Defense, PowerupColors.Defense);
    }

    /// <summary>
    /// Setzt alle nötigen Werte so, dass man ins nächste Level kommt
    /// </summary>
    private void NextLevel()
    {
        level++;
        speed++;
        kryptonite.SpawnKryptonite(this.speed, this.level, this.spielmodus);
        stars.SpawnStars(this.speed, this.spielmodus);
    }

    /// <summary>
    /// Zeichnet ein extra Screen wenn man verloren hat und fragt den Namen ab
    /// </summary>
    private void GameOverScreen()
    {
        timer.Stop();

        // Egal ob gewonnen wird oder nicht, es soll immer der Highscore gespeichert werden
        int maxLength = 20;
        string inputName = "";


        // Punkte anzeigen
        int punkte = CalculateScore();
        if(this.spielmodus == Spielmodus.Hardcore)
        {
            punkte *= 2;
        }

        // Namen abfragen (keine Sonderzeichen erlauben)
        ConsoleKeyInfo kInfo;
        do
        {
            gr.ClearBuffer();

            // Highscore logo anzeigen
            for (int i = 0; i < Highscore.HIGHSCORE_LOGO.Length; i++)
            {
                // Offset 5 damit es nicht ganz oben am Rand ist sondern etwas drunter
                gr.WriteToBuffer((gr.CONSOLE_WIDTH - Highscore.HIGHSCORE_LOGO[i].Length) / 2, i + 5, Highscore.HIGHSCORE_LOGO[i], ConsoleAttribute.FG_RED);
            }

            gr.WriteToBuffer((gr.CONSOLE_WIDTH - ("Erreichte Punkte: " + punkte).Length) / 2, (gr.CONSOLE_HEIGHT / 2) - 3, string.Format("Erreichte Punkte: {0}", punkte));

            // Positioniert die Namensabfrage mittig in der Konsole
            gr.WriteToBuffer((gr.CONSOLE_WIDTH - (("Name: ").Length + maxLength + maxLength)) / 2, (gr.CONSOLE_HEIGHT / 2) - 1, "Name: ");

            for (int i = 0; i < maxLength; i++)
            {
                if (inputName.Length > i)
                {
                    gr.WriteToBuffer(inputName[i] + " ");
                }
                else
                {
                    gr.WriteToBuffer("_ ");
                }
            }

            gr.DrawBufferToConsole();

            kInfo = Console.ReadKey(true);
            if (kInfo.Key == ConsoleKey.Backspace)
            {
                if (inputName.Length > 0)
                {
                    // Das letzte Zeichen löschen
                    inputName = inputName.Remove(inputName.Length - 1, 1);
                }
            }
            else if (((kInfo.KeyChar >= 'a' && kInfo.KeyChar <= 'z') || (kInfo.KeyChar >= 'A' && kInfo.KeyChar <= 'Z')) && inputName.Length < maxLength)
            {
                // Nur Zeichen von a-z & A-Z zulassen, keine Sonderzeichen
                inputName += kInfo.KeyChar;
            }

        } while (kInfo.Key != ConsoleKey.Enter || inputName == "");

        Highscore.SaveScore(inputName, punkte, this.spielmodus);
        Settings.Default.Stars += this.gesammelteSterne;
        Settings.Default.Save();
    }

    /// <summary>
    /// Prüft welche Tasten gedrückt sind.
    /// </summary>
    private void GetKeyInput()
    {
        // High Bit = UND Verknpüft mit 0x8000 bzw Dec: 32.768 oder Bin: 1000 000 000 000
        // Low Bit = UND Verknüpft mit 0x0001 bzw Dec: 1 oder Bin: 0001
        // High Bit sagt ob die Taste gehalten wird
        // Low bit sagt ob die Taste gerade angefangen wurde zu drücken

        // Superman nach oben bewegen
        if ((Native.GetAsyncKeyState(Native.VK_UP) & 0x8000) > 0)
        {
            superman.MoveUp();
        }

        // Superman nach links bewegen
        if ((Native.GetAsyncKeyState(Native.VK_LEFT) & 0x8000) > 0)
        {
            superman.MoveLeft();
        }

        // Superman nach unten bewegen
        if ((Native.GetAsyncKeyState(Native.VK_DOWN) & 0x8000) > 0)
        {
            superman.MoveDown();
        }

        // Superman nach rechts bewegen
        if ((Native.GetAsyncKeyState(Native.VK_RIGHT) & 0x8000) > 0)
        {
            superman.MoveRight();
        }

        // Beenden
        if ((Native.GetAsyncKeyState(Native.VK_ESCAPE) & 0x8000) > 0)
        {
            abbruch = true;
        }

    }

    /// <summary>
    /// Berechnet den Punktestand je nach Spielmodus
    /// </summary>
    /// <returns></returns>
    private int CalculateScore()
    {
        if (this.spielmodus == Spielmodus.Normal)
        {
            // Desto länger man braucht, desto weniger Punkte
            // Zusätzlich abhängig von der Gesundheit
            return ((this.superman.Score * 10) / ((int)this.timer.Elapsed.TotalSeconds + 1)) * (superman.HP + 1);
        }
        else if (this.spielmodus == Spielmodus.Endlos)
        {
            // Endlosmodus soll nicht abhängig von der Zeit sein, man kann sich ruhig Zeit nehmen, da es mit der Zeit ja sehr schwerer wird
            return (this.superman.Score * 10);
        }
        else if (this.spielmodus == Spielmodus.Hardcore)
        {
            return (this.superman.Score * ((int)this.timer.Elapsed.TotalSeconds));
        }

        return 0;
    }
}