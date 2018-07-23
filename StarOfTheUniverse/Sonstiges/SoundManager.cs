using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media; // Für das abspielen der Sounds

public static class SoundManager
{
    private static SoundPlayer player = new SoundPlayer();

    /// <summary>
    /// Spielt den Superman Theme Song im Hintergrund in einer Schleife ab
    /// </summary>
    public static void PlayGameSound()
    {
        player.Stop(); // Falls es schon gestartet sein sollte
        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + @"\Sounds\game.wav";
        player.PlayLooping();
    }
}
