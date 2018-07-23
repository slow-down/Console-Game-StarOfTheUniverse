using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameMenu
{
    private int selectedIndex = 0;
    private string[] menuAuswahl = { "N o r m a l", "E n d l o s", "H a r d c o r e", "", "H i g h s c o r e  N o r m a l", "H i g h s c o r e   E n d l o s", "H i g h s c o r e   H a r d c o r e", "", "S t o r e", "A l l e s  z u r ü c k s e t z e n", "B e e n d e n" };

    private ConsoleGraphics gr;

    private string[] SUPERMAN_LOGO =
    {
                @"            ,,########################################,,           ",
                @"         .*##############################################*         ",
                @"       ,*####*:::*########***::::::::**######:::*###########,      ",
                @"     .*####:öööö*#####*.ööööööööööööööööö:*###,.#######*,####*.    ",
                @"    *####:öööö*#####*öööööööööööööööööööööö.###########*öö,####*   ",
                @" .*####:öööö,#######,öööööööööööööööööööööööö##########*öööö:####* ",
                @" *####.öööö:#########*,ööööööööööööööööööööööö,,,,,,,,.öööööö,####:",
                @"   ####*öö,##############****************:,,ööööööööööööööö.####*  ",
                @"    :####*#####################################**,öööööööö*####.   ",
                @"      *############################################*,ööö:####:     ",
                @"       .#############################################*,####*       ",
                @"         :#####:*****#####################################.        ",
                @"           *####:öööööööööööööööööö.,,,:*****###########,          ",
                @"            .*####,öööööööööööööööööööööööööööö*######*            ",
                @"              .####*ö:*#######*ööööööööööööööö,#####*              ",
                @"                *###############*,,,,,,,,::**######,               ",
                @"                  *##############################:                 ",
                @"                    *####*****##########**#####*                   ",
                @"                     .####*.öööööööööööö:####*                     ",
                @"                       :####*ööööööööö.#####,                      ",
                @"                         *####:öööööö*####:                        ",
                @"                          .*####,öö*####*                          ",
                @"                            :####*####*                            ",
                @"                              *######,                             ",
                @"                                *##,                               ",
    };

    public GameMenu(ConsoleGraphics gr)
    {
        this.gr = gr;
    }

    /// <summary>
    /// Zeichnet das Hauptmenü auf die Konsole
    /// </summary>
    public void Show()
    {
        // Main loop
        while (true)
        {

            ConsoleKeyInfo kInfo;
            do
            {
                gr.ClearBuffer();

                // Positioniert das Logo mittig oben in der Konsole
                for (int i = 0; i < SUPERMAN_LOGO.Length; i++)
                {
                    int length = SUPERMAN_LOGO[i].Length;

                    for (int j = 0; j < length; j++)
                    {
                        // Offset 2 damit es nicht ganz oben am Rand ist sondern etwas drunter
                        gr.SetCursorPosition(((gr.CONSOLE_WIDTH - length) / 2 + j), i + 2);

                        if (SUPERMAN_LOGO[i][j] == 'ö')
                        {
                            // Hintergrund vom Superman Logo soll gelb sein
                            gr.WriteToBuffer('#', ConsoleAttribute.FG_YELLOW);
                        }
                        else
                        {
                            // Der Rest rot
                            gr.WriteToBuffer(SUPERMAN_LOGO[i][j], ConsoleAttribute.FG_RED);
                        }
                    }
                }

                // Menüpunkt anzeigen
                for (int i = 0; i < menuAuswahl.Length; i++)
                {
                    gr.SetCursorPosition((gr.CONSOLE_WIDTH - menuAuswahl[i].Length) / 2, (gr.CONSOLE_HEIGHT / 2) + i);
                    if (selectedIndex == i)
                    {
                        // Die Frabe des ausgewählten Menüpunkts ändern
                        gr.WriteToBuffer(menuAuswahl[i], ConsoleAttribute.FG_GREEN);
                    }
                    else
                    {
                        // Standard Farbe
                        gr.WriteToBuffer(menuAuswahl[i], ConsoleAttribute.FG_WHITE);
                    }
                }

                gr.DrawBufferToConsole();

                // Menüabfrage ob hoch oder runter bewegt wird
                kInfo = Console.ReadKey(true);
                if (kInfo.Key == ConsoleKey.UpArrow && selectedIndex > 0)
                {
                    selectedIndex--;
                    if (selectedIndex == 3) selectedIndex = 2;
                    else if (selectedIndex == 7) selectedIndex = 6;
                }
                else if (kInfo.Key == ConsoleKey.DownArrow && selectedIndex < (menuAuswahl.Length - 1))
                {
                    selectedIndex++;
                    if (selectedIndex == 3) selectedIndex = 4;
                    else if (selectedIndex == 7) selectedIndex = 8;
                }

            } while (kInfo.Key != ConsoleKey.Enter);


            switch (selectedIndex)
            {
                case 0: // Normal
                    Game game = new Game(gr);
                    game.Start(Spielmodus.Normal);
                    break;

                case 1: // Endlos
                    Game game2 = new Game(gr);
                    game2.Start(Spielmodus.Endlos);

                    break;

                case 2: // Hardcore
                    Game game3 = new Game(gr);
                    game3.Start(Spielmodus.Hardcore);

                    break;

                case 3: break;

                case 4: // Highscore Normal
                    Highscore highscore = new Highscore(gr, Spielmodus.Normal);
                    highscore.Draw();

                    break;

                case 5: // Highscore Endlos
                    Highscore highscore2 = new Highscore(gr, Spielmodus.Endlos);
                    highscore2.Draw();
                    break;

                case 6: // Highscore Hardcore
                    Highscore highscore3 = new Highscore(gr, Spielmodus.Hardcore);
                    highscore3.Draw();

                    break;

                case 7: break;

                case 8: // Store
                    Store store = new Store(gr);
                    store.Draw();
                    break;

                case 9: // Werkeinstellungen
                    ResetSettings();
                    break;
                case 10:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    private void ResetSettings()
    {
        Settings.Default.Superman1 = false;
        Settings.Default.Superman2 = false;
        Settings.Default.Superman3 = false;
        Settings.Default.Superman4 = false;
        Settings.Default.Superman5 = true;
        Settings.Default.SupermanAuswahl = 4;
        Settings.Default.Stars = 0;

        Highscore.ResetHighscore();

        Settings.Default.Save();
    }
}
