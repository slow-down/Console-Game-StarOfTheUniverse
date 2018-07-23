using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

public class ConsoleGraphics
{
    private IntPtr hConsoleOutput;
    private Structs.CharInfo[] buffer;
    private Structs.SmallRect rect;
    public static Random rnd = new Random();

    public readonly short CONSOLE_WIDTH;
    public readonly short CONSOLE_HEIGHT;

    private int currentIndex = 0;

    // https://en.wikipedia.org/wiki/Box-drawing_character
    public const char HORIZONTAL_BORDER = (char)0x2550;
    public const char VERTICAL_BORDER = (char)0x2551;
    public const char EDGE_TOP_LEFT = (char)0x2554;
    public const char EDGE_TOP_RIGHT = (char)0x2557;
    public const char EDGE_BOTTOM_LEFT = (char)0x255A;
    public const char EDGE_BOTTOM_RIGHT = (char)0x255D;

    public const char CROSS_TO_RIGHT = (char)0x2560;
    public const char CROSS_TO_LEFT = (char)0x2563;
    public const char CROSS_TO_TOP = (char)0x2569;
    public const char CROSS_TO_BOTTOM = (char)0x2566;

    public const char BLOCK_SOLID = (char)0x2588;
    public const char BLOCK_SOLID_SLIM = (char)0x258C;


    public ConsoleGraphics(bool fullscreen)
    {
        if (fullscreen)
        {
            Native.ShowWindow(Process.GetCurrentProcess().MainWindowHandle, Native.SW_MAXIMIZE);
        }

        this.CONSOLE_WIDTH = (short)Console.WindowWidth;
        this.CONSOLE_HEIGHT = (short)(Console.WindowHeight - 1);

        Init();
    }

    public ConsoleGraphics(short width, short height)
    {
        this.CONSOLE_WIDTH = width;
        this.CONSOLE_HEIGHT = height;

        Console.SetWindowSize(width, height + 1);

        Init();
    }

    private void Init()
    {
        hConsoleOutput = Native.GetStdHandle(Native.STD_OUTPUT_HANDLE);
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        buffer = new Structs.CharInfo[this.CONSOLE_WIDTH * this.CONSOLE_HEIGHT];
        rect = new Structs.SmallRect { Left = 0, Top = 0, Right = this.CONSOLE_WIDTH, Bottom = this.CONSOLE_HEIGHT };
    }

    public bool DrawBufferToConsole()
    {
        currentIndex = 0;
        return Native.WriteConsoleOutput(hConsoleOutput, buffer, new Structs.Coord { X = this.CONSOLE_WIDTH, Y = this.CONSOLE_HEIGHT }, new Structs.Coord { X = 0, Y = 0 }, ref rect);
    }

    internal void WriteToBuffer(int x, int y, char powerUpChar, object powerupColor)
    {
        throw new NotImplementedException();
    }

    public bool WriteToBuffer(int x, int y, char c, ConsoleAttribute attribute = ConsoleAttribute.FG_WHITE)
    {
        // if out of bounds
        if (y * this.CONSOLE_WIDTH + x + 1 >= buffer.Length) return false;

        buffer[(y * this.CONSOLE_WIDTH) + x].Char.UnicodeChar = c;
        buffer[(y * this.CONSOLE_WIDTH) + x].Attributes = (short)attribute;
        currentIndex = (y * this.CONSOLE_WIDTH) + x + 1;

        return true;
    }

    public bool WriteToBuffer(int x, int y, int num, ConsoleAttribute attribute = ConsoleAttribute.FG_WHITE)
    {
        return WriteToBuffer(x, y, num.ToString(), attribute);
    }

    public bool WriteToBuffer(int x, int y, char[] arr, ConsoleAttribute attribute = ConsoleAttribute.FG_WHITE)
    {
        // if out of bounds
        if (y * this.CONSOLE_WIDTH + x + arr.Length >= buffer.Length) return false;

        for (int i = 0; i < arr.Length; i++)
        {
            buffer[y * this.CONSOLE_WIDTH + x + i].Char.UnicodeChar = arr[i];
            buffer[y * this.CONSOLE_WIDTH + x + i].Attributes = (short)attribute;
        }

        currentIndex = (y * this.CONSOLE_WIDTH) + x + arr.Length;

        return true;
    }

    public bool WriteToBuffer(int x, int y, string str, ConsoleAttribute attribute = ConsoleAttribute.FG_WHITE)
    {
        // if out of bounds
        if (y * this.CONSOLE_WIDTH + x + str.Length >= buffer.Length) return false;

        for (int i = 0; i < str.Length; i++)
        {
            buffer[y * this.CONSOLE_WIDTH + x + i].Char.UnicodeChar = str[i];
            buffer[y * this.CONSOLE_WIDTH + x + i].Attributes = (short)attribute;
        }

        currentIndex = (y * this.CONSOLE_WIDTH) + x + str.Length;

        return true;
    }

    public bool WriteToBuffer(char c, ConsoleAttribute attribute = ConsoleAttribute.FG_WHITE)
    {
        // if out of bounds
        if (currentIndex + 1 >= buffer.Length) return false;

        buffer[currentIndex].Char.UnicodeChar = c;
        buffer[currentIndex].Attributes = (short)attribute;

        currentIndex++;

        return true;
    }

    public bool WriteToBuffer(char[] arr, ConsoleAttribute attribute = ConsoleAttribute.FG_WHITE)
    {
        // if out of bounds
        if (currentIndex + arr.Length >= buffer.Length) return false;

        for (int i = 0; i < arr.Length; i++)
        {
            buffer[currentIndex + i].Char.UnicodeChar = arr[i];
            buffer[currentIndex + i].Attributes = (short)attribute;
        }

        currentIndex += arr.Length;

        return true;
    }

    public bool WriteToBuffer(string str, ConsoleAttribute attribute = ConsoleAttribute.FG_WHITE)
    {
        // if out of bounds
        if (currentIndex + str.Length >= buffer.Length) return false;

        for (int i = 0; i < str.Length; i++)
        {
            buffer[currentIndex + i].Char.UnicodeChar = str[i];
            buffer[currentIndex + i].Attributes = (short)attribute;
        }

        currentIndex += str.Length;

        return true;
    }

    public bool SetCursorPosition(int x, int y)
    {
        // if out of bounds
        if (y * this.CONSOLE_WIDTH + x >= buffer.Length) return false;

        this.currentIndex = y * this.CONSOLE_WIDTH + x;

        return true;
    }

    public Structs.CharInfo ReadFromBuffer(int x, int y)
    {
        return buffer[y * CONSOLE_WIDTH + x];
    }

    public void ClearBuffer()
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i].Attributes = 0;
            //buffer[i].Char.AsciiChar = (byte)'\0';
            buffer[i].Char.UnicodeChar = '\0';
        }

        currentIndex = 0;
    }
}


// Console Colors are OR compatible to combine colors (r,g,b), ex. FG_RED | FG_GREEN | FG_BLUE = FG_WHITE
public enum ConsoleAttribute : short
{
    FG_BLACK = 0x0000,
    FG_BLUE_DARK = 0x0001,
    FG_GREEN_DARK = 0x0002,
    FG_BLUE_BRIGHT = 0x0003,
    FG_RED_DARK = 0x0004,
    FG_PURPLE = 0x0005,
    FG_YELLOW_DARK = 0x0006,
    FG_GRAY_LIGHT = 0x0007,
    FG_GRAY_DARK = 0x0008,
    FG_BLUE = 0x0009,
    FG_GREEN = 0x000A,
    FG_CYAN = 0x000B,
    FG_RED = 0x000C,
    FG_PINK = 0x000D,
    FG_YELLOW = 0x000E,
    FG_WHITE = 0x000F,

    BG_BLUE_DARK = 0x0010,
    BG_GREEN_DARK = 0x0020,
    BG_BLUE_BRIGHT = 0x0030,
    BG_RED_DARK = 0x0040,
    BG_PURPLE = 0x0050,
    BG_YELLOW_DARK = 0x0060,
    BG_GRAY_LIGHT = 0x0070,
    BG_GRAY_DARK = 0x0080,
    BG_BLUE = 0x0090,
    BG_GREEN = 0x00A0,
    BG_CYAN = 0x00B0,
    BG_RED = 0x00C0,
    BG_PINK = 0x00D0,
    BG_YELLOW = 0x00E0,
    BG_WHITE = 0x00F0
}


internal static class Native
{
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool WriteConsoleOutput(IntPtr hConsoleOutput, Structs.CharInfo[] lpBuffer, Structs.Coord dwBufferSize, Structs.Coord dwBufferCoord, ref Structs.SmallRect lpWriteRegion);

    // Aus der WinAPI32 wird die externe Methode ShowWindow verwendet weil C# von Haus aus sowas nicht kann (z.B. zum Maximieren der Konsole)
    // https://msdn.microsoft.com/de-de/library/windows/desktop/ms633548(v=vs.85).aspx
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool ShowWindow(IntPtr hWindow, int nCmdShow);
    internal const short SW_HIDE = 0;
    internal const short SW_MAXIMIZE = 3;
    internal const short SW_MINIMIZE = 6;
    internal const short SW_RESTORE = 9;

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr GetStdHandle(int nStdHandle);
    internal const int STD_INPUT_HANDLE = -10;
    internal const int STD_OUTPUT_HANDLE = -11;
    internal const int STD_ERROR_HANDLE = -12;

    // Wir verwenden GetAsyncKeyState aus der WinAPI32, weil es den Thread nicht blockiert und man mehrere Tasten gleichzeitig drücken kann oder ein workaround programmierne zu müssen
    // https://msdn.microsoft.com/de-de/library/windows/desktop/ms646293(v=vs.85).aspx
    [DllImport("user32.dll")]
    internal static extern int GetAsyncKeyState(int vKey);
    // Virtual Key Codes : https://docs.microsoft.com/en-us/windows/desktop/inputdev/virtual-key-codes
    internal static short VK_LEFT = 0x25;
    internal static short VK_UP = 0x26;
    internal static short VK_RIGHT = 0x27;
    internal static short VK_DOWN = 0x28;
    internal static short VK_ESCAPE = 0x1B;
}

public static class Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
        public short X;
        public short Y;

        public Coord(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }
    };

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CharUnion
    {
        [FieldOffset(0)] public char UnicodeChar;
        [FieldOffset(0)] public byte AsciiChar;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CharInfo
    {
        [FieldOffset(0)] public CharUnion Char;
        [FieldOffset(2)] public short Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }
}
