using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace MeetingManager;

public static class GUI
{
    
    public static void Draw(string text, bool isClear, bool IsN)
    {
        if(isClear)
            Console.Clear();
        
        Console.Write(text);
        var originalLeft = Console.CursorLeft;
        var originalTop = Console.CursorTop;
        Console.Write(new string(' ', Console.BufferWidth - Console.CursorLeft-1));
        Console.SetCursorPosition(originalLeft, originalTop);
        if(IsN)
            Console.WriteLine();
        
    }

    public static void Notify(string text)
    {
        var originalLeft = Console.CursorLeft;
        var originalTop = Console.CursorTop;
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.ForegroundColor = ConsoleColor.Black;
        var write = text.Split('\n');
        var i = 0;
        foreach (var str in write)
        {
            
            Console.SetCursorPosition(30, i);
            Console.Write(str);
            i++;
        }
        Console.ResetColor();
        Console.SetCursorPosition(originalLeft, originalTop);
    }

    public static void ClearMissingString(int count, int left = 0)
    {
        var cursor = Console.CursorTop - count; 
        for (int i = 0; i < count; i++)
        {
            Console.SetCursorPosition(0, cursor+i);
            Console.Write(new String(' ', Console.BufferWidth));
        }
        Console.SetCursorPosition(0, cursor);
    }

    public static void Wait()
    {
        Console.ReadLine();
    }
    
}