﻿using Newtonsoft.Json;
using Partypacker.Core;
using Partypacker.Net;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
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

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Exit += OnApplicationExit;

            var DiscordURL = PartypackServer.GET("/api/discord/url");
            if (!DiscordURL.Key)
            {
                MessageBox.Show("Failed to contact the Partypack API. (Is it running?)", "Partypack API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
                return;
            }

            DiscordAuthURL = DiscordURL.Value;
        }

        void OnApplicationExit(object sender, ExitEventArgs e) => Proxx?.StopProxy();
        void OnChangePort(object sender, TextChangedEventArgs e) {
            if (!int.TryParse(((TextBox)sender).Text, out int P))
                return;

            Port = P;
        }

        void OnDashboard(object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = PartypackServer.DashboardURL });

        private static async Task DefaultRoute(HttpContext ctx)
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

            await ctx.Response.Send("All done! You can close this tab now.");
            sv.Stop();
        }

        void OnLoginUsingDiscord(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = $"{DiscordAuthURL}&state={HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                Client = "PartypackerDesktop"
            }))))}" });

            sv = new Server("127.0.0.1", 14968, false, DefaultRoute);
            sv.Start();
        }

        void OnLaunch(object sender, RoutedEventArgs e)
        {
            Proxx = new Proxy(Port);
            // please make this dynamic later :D
            Proxx.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJRCI6IjQ1NDk2ODU0MjcyMzU3MTcxNSIsImlhdCI6MTcwNTkyODQ0M30.ogINqFZ_3DBkECbHo87HjW9c6p2imT1CnCvfIR3iGJ4";
            Proxx.StartProxy();

            //Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = "com.epicgames.launcher://apps/fn%3A4fe75bbc5a674f4f9b356b5c90567da5%3AFortnite?action=launch&silent=true" });
        }
    }
}