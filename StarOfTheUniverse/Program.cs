using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static ConsoleGraphics gr = new ConsoleGraphics(true);

    static void Main(string[] args)
    {
        // Startet die Musik im Hintergrund (wird unendlich oft wiederholt)
        SoundManager.PlayGameSound();

        // Login Fenster anzeigen
        Login login = new Login(gr);
        login.Show();

        // Spiel starten da das Passwort richtig eingegeben wurde
        GameMenu game = new GameMenu(gr);
        game.Show();
    }
}
