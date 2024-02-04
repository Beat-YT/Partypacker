using DiscordRPC;
using DiscordRPC.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Partypacker.Core;
using Partypacker.Net;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Transactions;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WatsonWebserver;

namespace Partypacker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Proxy Proxx;
        static string DiscordAuthURL;
        static int Port = 6969;
        static string Token;
        static UserDetailObject UserDetails;
        static Server sv;
        public static INIFile settings = new("settings.ini", true);

        public MainWindow()
        {
            InitializeComponent();
            InitializeRPC();
            Application.Current.Exit += OnApplicationExit;
            Process.GetCurrentProcess().Exited += MainWindow_Exited;

            var DiscordURL = PartypackServer.GET("/api/discord/url");
            if (!DiscordURL.Key)
            {
                MessageBox.Show("Failed to contact the Partypack API. (Is it running?)", "Partypack API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
                return;
            }

            DiscordAuthURL = DiscordURL.Value;

            if (!string.IsNullOrWhiteSpace(settings.GetValue("Launcher", "token")))
                AutoLogin();
        }

        private void MainWindow_Exited(object? sender, EventArgs e)
        {
            Proxx?.StopProxy();
        }

        void OnApplicationExit(object sender, ExitEventArgs e) => Proxx?.StopProxy();
        void OnChangePort(object sender, TextChangedEventArgs e) {
            if (!int.TryParse(((TextBox)sender).Text, out int P))
                return;

            Port = P;
        }

        void OnDashboard(object sender, MouseButtonEventArgs e) => Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = PartypackServer.DashboardURL + "/profile"});

        private void UpdateUserUI()
        {
            this.Dispatcher.Invoke(() =>
            {
                UsernameTextBlock.Text = string.IsNullOrWhiteSpace(UserDetails.GlobalName) ? UserDetails.Username : @$"{UserDetails.GlobalName} (@{UserDetails.Username})";
                ProfilePictureImage.ImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(UserDetails.Avatar);
                UsernameTextBlock.Visibility = Visibility.Visible;
                PFPContainer.Visibility = Visibility.Visible;
            });
        }

        void AutoLogin()
        {
            var B64Token = settings.GetValue("Launcher", "token");
            var NonB64Bytes = Convert.FromHexString(B64Token);
            var NonB64Str = Encoding.UTF8.GetString(NonB64Bytes);
            Token = NonB64Str;
            UserDetails = JsonConvert.DeserializeObject<UserDetailObject>(Encoding.UTF8.GetString(Convert.FromHexString(HttpUtility.UrlDecode(settings.GetValue("Launcher", "user")))));
            UpdateUserUI();
            Dispatcher.Invoke(() => LaunchButton.IsEnabled = true);
            ConvertLoginToLogout();
        }

        private void ConvertLoginToLogout()
        {
            Dispatcher.Invoke(() =>
            {
                LoginButton.Content = "Log Out";
                LoginButton.Click -= OnLoginUsingDiscord;
                LoginButton.Click += OnLogout;
            });
        }

        private void OnLogout(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                UsernameTextBlock.Visibility = Visibility.Hidden;
                PFPContainer.Visibility = Visibility.Hidden;
                UserDetails = null;
                Token = "";
                settings.SetValue("Launcher", "user", "");
                settings.SetValue("Launcher", "token", "");

                LoginButton.Content = "Log in using Discord";
                LoginButton.Click += OnLoginUsingDiscord;
                LoginButton.Click -= OnLogout;
            });
        }

        private async Task DefaultRoute(HttpContext ctx)
        {
            string _Token = ctx.Request.Query.Elements["token"];
            string _UserDetails = ctx.Request.Query.Elements["user"];

            if (_Token == null || _UserDetails == null)
            {
                await ctx.Response.Send($"Invalid request. ({_Token}, {_UserDetails})");
                return;
            }

            Token = _Token;
            UserDetails = JsonConvert.DeserializeObject<UserDetailObject>(Encoding.UTF8.GetString(Convert.FromHexString(HttpUtility.UrlDecode(_UserDetails))));
            settings.SetValue("Launcher", "user", HttpUtility.UrlDecode(_UserDetails));
            settings.SetValue("Launcher", "token", Convert.ToHexString(Encoding.UTF8.GetBytes(Token)));
            UpdateUserUI();
            Dispatcher.Invoke(() =>
            {
                LaunchButton.IsEnabled = true;
            });
            ConvertLoginToLogout();

            await ctx.Response.Send("All done! You can close this tab now.");
            sv.Stop();
            sv = null;
        }

        void OnLoginUsingDiscord(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = $"{DiscordAuthURL}&state={HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                Client = "PartypackerDesktop"
            }))))}"});

            if (sv == null)
            {
                sv = new Server("127.0.0.1", 14968, false, DefaultRoute);
                sv.Start();
            }
        }

        bool WaitingForGameToOpen = true;
        System.Timers.Timer GameCheckTimer;
        private void CheckForProcessAndClose(object sender, ElapsedEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("FortniteLauncher");

            if (WaitingForGameToOpen)
            {
                if (processes.Length > 0)
                    WaitingForGameToOpen = false;

                return;
            }

            if (processes.Length > 0)
                return;

            Dispatcher.Invoke(() =>
            {
                LaunchButton.IsEnabled = true;
            });
            WaitingForGameToOpen = true;
            GameCheckTimer.Stop();
            Proxx?.StopProxy();
        }

        async void OnLaunch(object sender, RoutedEventArgs e)
        {
            Proxx = new Proxy(Port);
            Proxx.Token = Token;
            Proxx.StartProxy();

            using (Process p = Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = "com.epicgames.launcher://apps/fn%3A4fe75bbc5a674f4f9b356b5c90567da5%3AFortnite?action=launch&silent=true" }))
            {
                LaunchButton.IsEnabled = false;
                GameCheckTimer = new System.Timers.Timer();
                GameCheckTimer.Interval = 5000;
                GameCheckTimer.Elapsed += CheckForProcessAndClose;
                GameCheckTimer.Start();
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public DiscordRpcClient client;
        void InitializeRPC()
        {
            client = new DiscordRpcClient("1198605718169858088");

            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Details = "Modding Fortnite Festival",
                State = "Loading Custom Tracks",
                Timestamps = new DiscordRPC.Timestamps()
                {
                    Start = DateTime.UtcNow,
                },
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Partypacker - Alpha",
                    //SmallImageKey = "image_small"
                },
                Buttons = new DiscordRPC.Button[]
                {
                    new DiscordRPC.Button() {Label = "Check out Partypack!", Url = "https://partypack.mcthe.dev/"}
                }
            });
        }
    }
}