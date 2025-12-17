using Microsoft.Maui.Controls;
using System.IO;
using IstripuenKudeaketaYago.Data;
using Microsoft.Maui.Storage; // FileSystem-erako beharrezkoa

namespace IstripuenKudeaketaYago
{
    public partial class App : Application
    {
        // Propietatea estatikoa da aplikazio osoan eskuragarri egoteko
        public static DatuBasea DatuBasea { get; private set; }

        public App()
        {
            InitializeComponent();

            // Gaia argia behartu (Letra beltza ondo ikusteko)
            UserAppTheme = AppTheme.Light;

            // Datu-basea hasieratu
            string dbBidea = Path.Combine(FileSystem.AppDataDirectory, "istripuak_eus.db");
            DatuBasea = new DatuBasea(dbBidea);

            MainPage = new AppShell();
        }
    }
}