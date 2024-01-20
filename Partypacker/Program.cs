using Partypacker;
using Pastel;
using System.Drawing;

Console.SetWindowSize(75, 20);

Console.CursorVisible = false;

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

Console.WriteLine("Welcome to Partypacker - Select an option below:");

ConsoleKeyInfo ReceivedKeyInput;
int SelectedOptionIndex = 0;
bool Running = true;
SelectableOption[] Options = new SelectableOption[]
{
    new SelectableOption("Launch Fortnite", () => { Console.WriteLine("cool!"); }),
    new SelectableOption("Open Dashboard", () => { Console.WriteLine("epic!"); }),
    new SelectableOption("Settings", () => { Console.WriteLine("awesome!"); })
};

while (Running)
{
    (int left, int top) = Console.GetCursorPosition();
    for (int i = 0; i < Options.Length; i++)
    {
        bool Selected = SelectedOptionIndex == i;
        SelectableOption Option = Options[i];
        Console.WriteLine($"{(Selected ? ">".Pastel(Color.LimeGreen) : " ")}   {Option.Name.Pastel(Selected ? Color.DarkGreen : Color.Gray)}");
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