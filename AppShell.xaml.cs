using Microsoft.Maui.Controls;

namespace IstripuenKudeaketaYago
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Ibilbideak erregistratu orrien izen berriekin
            Routing.RegisterRoute(nameof(IbilgailuakOrria), typeof(IbilgailuakOrria));
            Routing.RegisterRoute(nameof(IstripuakOrria), typeof(IstripuakOrria));
        }
    }
}