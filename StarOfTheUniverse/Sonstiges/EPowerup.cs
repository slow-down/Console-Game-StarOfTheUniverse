using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Powerup
{
    Health = 1,
    Points,
    Defense
}

public static class PowerupChars
{
    public const char Health = 'H';
    public const char Points = 'P';
    public const char Defense = 'D';
}

public static class PowerupColors
{
    public const ConsoleAttribute Health = ConsoleAttribute.FG_RED;
    public const ConsoleAttribute Points = ConsoleAttribute.FG_PINK;
    public const ConsoleAttribute Defense = ConsoleAttribute.FG_CYAN;
}