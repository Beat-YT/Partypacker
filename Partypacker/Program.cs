using Partypacker.Net;
using Pastel;
using System.Diagnostics;
using System.Drawing;
using static Partypacker.Win32;

namespace Partypacker
{
    public static class Program
    {
        private static SetConsoleCtrlEventHandler CtrlHandler;
        private static Proxy Proxx;

        private static void Main(string[] args)
        {
            ushort? port = 6969;

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
                new SelectableOption("Launch Fortnite", () =>
                    Run(port, () => Process.Start(new ProcessStartInfo { FileName = "com.epicgames.launcher://apps/fn%3A4fe75bbc5a674f4f9b356b5c90567da5%3AFortnite?action=launch&silent=true", UseShellExecute = true }))),
                new SelectableOption("Open Dashboard", () => Process.Start(new ProcessStartInfo { FileName = "https://partypack.mcthe.dev", UseShellExecute = true })),
                new SelectableOption("Settings", () => { Console.WriteLine("awesome!"); })
            };

            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Debug.Fail(e.StackTrace);
            }
            finally
            {
                OnApplicationExit();
            }
        }

        private static void Run(ushort? port, Action Finish)
        {
            CtrlHandler = CleanUp;
            SetConsoleCtrlHandler(CtrlHandler, true);
            Proxx = port switch
            {
                null => new Proxy(),
                _ => new Proxy((ushort)port)
            };
            Proxx.StartProxy();

            Finish.Invoke();
        }

        public static void OnApplicationExit()
        {
            CleanUp(CtrlType.CTRL_C_EVENT);
        }

        private static bool CleanUp(CtrlType ctrlType)
        {
            Proxx?.StopProxy();
            return true;
        }
    }
}

