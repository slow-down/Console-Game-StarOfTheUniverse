using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Store
{
    private string[] STORE_LOGO =
    {
        @"  ______     __                                   ",
        @" /      \   |  \                                  ",
        @"|  $$$$$$\ _| $$_     ______    ______    ______  ",
        @"| $$___\$$|   $$ \   /      \  /      \  /      \ ",
        @" \$$    \  \$$$$$$  |  $$$$$$\|  $$$$$$\|  $$$$$$\",
        @" _\$$$$$$\  | $$ __ | $$  | $$| $$   \$$| $$    $$",
        @"|  \__| $$  | $$|  \| $$__/ $$| $$      | $$$$$$$$",
        @" \$$    $$   \$$  $$ \$$    $$| $$       \$$     \",
        @"  \$$$$$$     \$$$$   \$$$$$$  \$$        \$$$$$$$"
    };

    private int[] preise =
    {
        1337,
        250,
        100,
        50,
        0 // Default(Weiß) = Kostenlos
    };

    // public static, damit Superman darauf zugreifen kann.
    public static ConsoleAttribute[] colors =
    {
        ConsoleAttribute.FG_RED,
        ConsoleAttribute.FG_BLUE,
        ConsoleAttribute.FG_CYAN,
        ConsoleAttribute.FG_BLUE_BRIGHT,
        ConsoleAttribute.FG_WHITE
    };

    private Stopwatch timer;
    private string[][] liftingAnimation =
    {
        new string[]
        {
            @"           ",
            @"           ",
            @"    _._    ",
            @"   / O \   ",
            @"   \| |/   ",
            @"O--+=-=+--O"
        },

        new string[]
        {
            @"           ",
            @"           ",
            @"           ",
            @"   ,-O-,   ",
            @"O--=---=--O",
            "    2\"2    "
        },

        new string[]
        {
            @"           ",
            @"           ",
            @"   ,_O_,   ",
            @"O--(---)--O",
            @"    >'>    ",
            @"    - -    "
        },

        new string[]
        {
            @"           ",
            @"   ._O_.   ",
            @"O--<-+->--O",
            @"     X     ",
            @"    / \    ",
            @"   -   -   "
        },

        new string[]
        {
            @"           ",
            @"O--=-O-=--O",
            @"    '-'    ",
            @"     v     ",
            @"    / )    ",
            @"   ~  z     "
        },

        new string[]
        {
            @"O--,---,--O",
            @"   \ O /   ",
            @"    - -    ",
            @"     -     ",
            @"    / \    ",
            @"   =   =   "
        },
    };
    private int animIndex = 0;
    private long lastAnimUpdate = 0;
    private bool animGoingBack = false;

    private int currentSelectionIndex = 4;
    private ConsoleGraphics gr;


    public Store(ConsoleGraphics gr)
    {
        this.gr = gr;
        timer = Stopwatch.StartNew();
    }

    /// <summary>
    /// Zeichnet den Store auf die Konsole
    /// </summary>
    public void Draw()
    {
        this.currentSelectionIndex = Settings.Default.SupermanAuswahl;

        ConsoleKeyInfo kInfo = new ConsoleKeyInfo();
        do
        {
            gr.ClearBuffer();

            // Zeichnet das "Store" Logo mittig oben
            for (int i = 0; i < STORE_LOGO.Length; i++)
            {
                // Offset 5 damit es nicht ganz oben am Rand ist sondern etwas drunter
                gr.WriteToBuffer((gr.CONSOLE_WIDTH - STORE_LOGO[i].Length) / 2, i + 5, STORE_LOGO[i], ConsoleAttribute.FG_YELLOW);
            }

            int xCenter = gr.CONSOLE_WIDTH / 2;
            int yCenter = gr.CONSOLE_HEIGHT / 2;

            gr.WriteToBuffer(xCenter - (preise.Length * 7 / 2) - "Preis: ".Length - 5, yCenter + 5, "Preis: ");


            // Geht durch jede Auswahl durch und Zeichne
            for (int i = 0; i < preise.Length; i++)
            {
                // 3x3 -> mit rand = 5x5
                // abstand von 2 px -> 7x5
                int xOffsetToLeft = (preise.Length / 2) - i;
                int x = (xCenter - 2) - (xOffsetToLeft * 7);

                DrawSupermanAt(x, yCenter, colors[i]);

                // Jeweiligen Preis anzeigen
                if (!GetAuswahlByIndex(i))
                {
                    gr.WriteToBuffer(x, yCenter + 5, preise[i], ConsoleAttribute.FG_YELLOW);
                }
            }

            // Zeigt an welche Farbe zurzeit aktiv ist
            int xAuswahl = (gr.CONSOLE_WIDTH / 2) - (((preise.Length / 2) - Settings.Default.SupermanAuswahl) * 7) - 1;
            gr.WriteToBuffer(xAuswahl - ("Aktiv".Length / 2), yCenter - 3, "Aktiv", ConsoleAttribute.FG_RED);

            // Zeichnet das gerade ausgewählte Item mit einem Rechteck
            DrawRectangle();

            // Zeichnet die aktuelle Anzahl an Sternen
            string stars = Settings.Default.Stars.ToString();

            gr.WriteToBuffer((gr.CONSOLE_WIDTH - "Sterne: ".Length - stars.Length) / 2, yCenter + 15, "Sterne: ");
            gr.WriteToBuffer(stars, ConsoleAttribute.FG_YELLOW);

            // Zeichnet die Animation
            DrawAnimation();

            gr.DrawBufferToConsole();

            if (!Console.KeyAvailable) continue;

            kInfo = Console.ReadKey(true);
            if (kInfo.Key == ConsoleKey.Enter)
            {
                bool auswahlFreigeschaltet = false;

                switch (currentSelectionIndex)
                {
                    case 0:
                        auswahlFreigeschaltet = Settings.Default.Superman1;
                        break;
                    case 1:
                        auswahlFreigeschaltet = Settings.Default.Superman2;
                        break;
                    case 2:
                        auswahlFreigeschaltet = Settings.Default.Superman3;
                        break;
                    case 3:
                        auswahlFreigeschaltet = Settings.Default.Superman4;
                        break;
                    default:
                        auswahlFreigeschaltet = Settings.Default.Superman5;
                        break;
                }

                // Wenn schon freigeschaltet, dann nur auswählen, ansonsten Kaufen
                if (auswahlFreigeschaltet)
                {
                    Settings.Default.SupermanAuswahl = currentSelectionIndex;
                    Settings.Default.Save();
                }
                else
                {
                    // Prüfen ob genug "Sterne" vorhanden sind
                    if (Settings.Default.Stars >= preise[currentSelectionIndex])
                    {
                        // Kaufen
                        Settings.Default.Stars -= preise[currentSelectionIndex];
                        Settings.Default.SupermanAuswahl = currentSelectionIndex;
                        switch (currentSelectionIndex)
                        {
                            case 0: Settings.Default.Superman1 = true; break;
                            case 1: Settings.Default.Superman2 = true; break;
                            case 2: Settings.Default.Superman3 = true; break;
                            case 3: Settings.Default.Superman4 = true; break;
                            default: Settings.Default.Superman5 = true; break;
                        }
                        Settings.Default.Save();
                    }
                }
            }
            else if (kInfo.Key == ConsoleKey.RightArrow)
            {
                // Auswahlrechteck nach rechts bewegen
                if (currentSelectionIndex < preise.Length - 1)
                {
                    currentSelectionIndex++;
                }
            }
            else if (kInfo.Key == ConsoleKey.LeftArrow)
            {
                // Auswahlrechteck nach links bewegen
                if (currentSelectionIndex > 0)
                {
                    currentSelectionIndex--;
                }
            }


        } while (kInfo.Key != ConsoleKey.Escape && kInfo.Key != ConsoleKey.Backspace);
    }

    /// <summary>
    /// Zeichnet den Superman an der Postion die übergeben wird
    /// </summary>
    private void DrawSupermanAt(int x, int y, ConsoleAttribute color)
    {
        for (int i = 0; i < 3; i++)
        {
            gr.WriteToBuffer(x, y + i, Superman.superman[i], color);
        }
    }

    /// <summary>
    /// Zeichnet ein Rechteck um die aktuelle Auswahl
    /// </summary>
    private void DrawRectangle()
    {
        // Das Auswahl Rechteck
        int xOffsetToLeft = (preise.Length / 2) - currentSelectionIndex;
        int x = (gr.CONSOLE_WIDTH / 2) - 2 - (xOffsetToLeft * 7);
        int yCenter = (gr.CONSOLE_HEIGHT / 2);

        // top
        gr.WriteToBuffer(x, yCenter - 1, new string(ConsoleGraphics.HORIZONTAL_BORDER, 3), ConsoleAttribute.FG_GREEN);

        // bottom
        gr.WriteToBuffer(x, yCenter - 1 + 4, new string(ConsoleGraphics.HORIZONTAL_BORDER, 3), ConsoleAttribute.FG_GREEN);

        // left / right
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                // top left
                gr.WriteToBuffer(x - 1, yCenter - 1 + i, ConsoleGraphics.EDGE_TOP_LEFT, ConsoleAttribute.FG_GREEN);

                // top right
                gr.WriteToBuffer(x - 1 + 4, yCenter - 1 + i, ConsoleGraphics.EDGE_TOP_RIGHT, ConsoleAttribute.FG_GREEN);
            }
            else if (i == 4)
            {
                // bottom left
                gr.WriteToBuffer(x - 1, yCenter - 1 + i, ConsoleGraphics.EDGE_BOTTOM_LEFT, ConsoleAttribute.FG_GREEN);

                // bottom right
                gr.WriteToBuffer(x - 1 + 4, yCenter - 1 + i, ConsoleGraphics.EDGE_BOTTOM_RIGHT, ConsoleAttribute.FG_GREEN);
            }
            else
            {
                // left
                gr.WriteToBuffer(x - 1, yCenter - 1 + i, ConsoleGraphics.VERTICAL_BORDER, ConsoleAttribute.FG_GREEN);

                // right
                gr.WriteToBuffer(x - 1 + 4, yCenter - 1 + i, ConsoleGraphics.VERTICAL_BORDER, ConsoleAttribute.FG_GREEN);
            }
        }

    }

    /// <summary>
    /// Zeichnet Superman der Gewichte hebt als Animation
    /// </summary>
    private void DrawAnimation()
    {
        int x = (gr.CONSOLE_WIDTH - liftingAnimation[0][0].Length) / 2;
        int y = ((gr.CONSOLE_HEIGHT - liftingAnimation[0].Length) / 2) - 10;

        if (animGoingBack)
        {
            // Animation rückwärts
            for (int i = liftingAnimation[0].Length - 1; i >= 0; i--)
            {
                gr.WriteToBuffer(x, y + i, liftingAnimation[animIndex][i], ConsoleAttribute.FG_RED);
            }
        }
        else
        {
            // Animation vorwärts
            for (int i = 0; i < liftingAnimation[0].Length; i++)
            {
                gr.WriteToBuffer(x, y + i, liftingAnimation[animIndex][i], ConsoleAttribute.FG_RED);
            }
        }

        // Es soll nur jedes sekunde geupdatet werden
        if (timer.ElapsedMilliseconds - lastAnimUpdate < 120) return;

        lastAnimUpdate = timer.ElapsedMilliseconds;

        // update index
        if (animGoingBack)
        {
            if (animIndex > 0)
            {
                animIndex--;
            }
            else
            {
                animGoingBack = !animGoingBack;
            }
        }
        else
        {
            if (animIndex < liftingAnimation.Length - 1)
            {
                animIndex++;
            }
            else
            {
                animGoingBack = !animGoingBack;
            }
        }
    }


    /// <summary>
    /// Gibt zurück ob die Auswahl freigeschaltet wurde oder nicht
    /// </summary>
    private bool GetAuswahlByIndex(int index)
    {
        switch (index)
        {
            case 0: return Settings.Default.Superman1;
            case 1: return Settings.Default.Superman2;
            case 2: return Settings.Default.Superman3;
            case 3: return Settings.Default.Superman4;
            default: return Settings.Default.Superman5;
        }
    }

}
