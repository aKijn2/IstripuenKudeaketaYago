using IstripuenKudeaketaYago.Data;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using System.IO;

namespace IstripuenKudeaketaYago
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            //string dbPath = Path.Combine(FileSystem.AppDataDirectory, "istripuak.db3");
            //builder.Services.AddSingleton(new Database(dbPath));

            return builder.Build();
        }
    }
}
