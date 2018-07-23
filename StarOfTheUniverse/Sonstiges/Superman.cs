using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Superman
{
    // Position origin oben links
    private Point location;
    private Size supermanSize;
    private Size gameSize;

    private ConsoleGraphics gr;

    public int HP { get; set; } = 100;
    public int Score { get; set; }


    private int _defense = 0;
    public int Defense
    {
        get
        {
            return _defense;
        }
        set
        {
            if (value > 5)
            {
                _defense = 5;
            }
            else
            {
                _defense = value;
            }
        }
    }


    private static char[][] supermanRight = new char[][]
    {
       new char[]{ ' ' , 'O' , '/' },
       new char[]{ '/'  , '|' , ' ' },
       new char[]{ '/'  , ' ' , '/' }
    };

    private static char[][] supermanLeft = new char[][]
    {
       new char[]{ '\\' , 'O' , ' ' },
       new char[]{ ' '  , '|' , '\\' },
       new char[]{ '\\'  , ' ' , '\\' }
    };

    public static char[][] superman = supermanRight;


    public Superman(ConsoleGraphics gr, Size gameSize)
    {
        this.gr = gr;
        location = new Point(gr.CONSOLE_WIDTH / 2, gr.CONSOLE_HEIGHT / 2);
        this.gameSize = gameSize;
        this.supermanSize = new Size(superman[0].Length, superman.GetLength(0));
    }

    /// <summary>
    /// Bewegt Superman nach rechts
    /// </summary>
    public void MoveRight()
    {
        int rightBound = (gr.CONSOLE_WIDTH + gameSize.Width) / 2;
        if (location.X + superman[0].Length + 1 < rightBound)
        {
            location.X++;
            superman = supermanRight;
        }
    }

    /// <summary>
    /// Bewegt Superman nach unten
    /// </summary>
    public void MoveDown()
    {
        int bottomBound = (gr.CONSOLE_HEIGHT + gameSize.Height) / 2;
        if (location.Y + superman.GetLength(0) < bottomBound)
        {
            location.Y++;
        }
    }

    /// <summary>
    /// Bewegt Superman nach links
    /// </summary>
    public void MoveLeft()
    {
        int leftBound = (gr.CONSOLE_WIDTH - gameSize.Width) / 2;
        if (location.X - 1 > leftBound)
        {
            location.X--;
            superman = supermanLeft;
        }
    }

    /// <summary>
    /// Bewegt Superman nach oben
    /// </summary>
    public void MoveUp()
    {
        int topBound = (gr.CONSOLE_HEIGHT - gameSize.Height) / 2;
        if (location.Y - 1 > topBound)
        {
            location.Y--;
        }
    }

    /// <summary>
    /// Zeichnet Superman in die Konsole
    /// </summary>
    public void Draw()
    {
        for (int y = 0; y < 3; y++)
        {
            gr.WriteToBuffer(location.X, location.Y + y, superman[y], Store.colors[Settings.Default.SupermanAuswahl]);
        }
    }

    /// <summary>
    /// Prüft ob der angegebene Punkt sich innerhalb von Superman befindet
    /// </summary>
    /// <param name="p">Der Punkt der geprüft werden soll</param>
    /// <returns>true, wenn der Punkt innerhalb liegt</returns>
    public bool Intersects(Point p)
    {
        for (int y = 0; y < superman.Length; y++)
        {
            for (int x = 0; x < superman[0].Length; x++)
            {
                if (p.X == location.X + x && p.Y == location.Y + y)
                {
                    return true;
                }
            }
        }

        return false;
    }

}
