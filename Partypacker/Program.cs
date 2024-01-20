// See https://aka.ms/new-console-template for more information
using Partypacker;
using Pastel;
using System.Drawing;

Console.WriteLine(@"
  _____           _                          _             
 |  __ \         | |                        | |            
 | |__) |_ _ _ __| |_ _   _ _ __   __ _  ___| | _____ _ __ 
 |  ___/ _` | '__| __| | | | '_ \ / _` |/ __| |/ / _ \ '__|
 | |  | (_| | |  | |_| |_| | |_) | (_| | (__|   <  __/ |   
 |_|   \__,_|_|   \__|\__, | .__/ \__,_|\___|_|\_\___|_|   
                       __/ | |                             
                      |___/|_|                             ".Pastel(Color.IndianRed));
Console.WriteLine("-----------------------------------------------------------".Pastel(Color.CadetBlue));

Console.WriteLine("Welcome to Partypacker - what do you want to do?");

ConsoleKeyInfo ReceivedKeyInput;
int SelectedOptionIndex = 0;
bool Running = true;
SelectableOption[] Options = new SelectableOption[]
{
    new SelectableOption("Test 1", () => { Console.WriteLine("cool!"); }),
    new SelectableOption("Test 2", () => { Console.WriteLine("epic!"); }),
    new SelectableOption("Test 3", () => { Console.WriteLine("awesome!"); })
};

while (Running)
{
    (int left, int top) = Console.GetCursorPosition();
    for (int i = 0; i < Options.Length; i++)
    {
        SelectableOption Option = Options[i];
        Console.WriteLine($"{(SelectedOptionIndex == i ? ">".Pastel(Color.LimeGreen) : " ")}   {Option.Name.Pastel(Color.DarkGreen)}");
    }

    ReceivedKeyInput = Console.ReadKey();
    switch (ReceivedKeyInput.Key)
    {
        case ConsoleKey.UpArrow:
            SelectedOptionIndex--;
            break;
        case ConsoleKey.DownArrow:
            SelectedOptionIndex++;
            break;
        case ConsoleKey.Enter:
            Options[SelectedOptionIndex].OnPress.Invoke();
            break;
    }

    Console.SetCursorPosition(left, top);
    SelectedOptionIndex = Math.Clamp(SelectedOptionIndex, 0, Options.Length);
}