using System;
using Microsoft.Maui.Controls;

namespace IstripuenKudeaketaYago
{
    /// <summary>
    /// Txostenak eta taulak erakusteko leiho modala.
    /// </summary>
    public partial class EmaitzakOrria : ContentPage
    {
        /// <summary>
        /// Orriaren eraikitzailea.
        /// </summary>
        /// <param name="izenburua">Leihoaren goiburua.</param>
        /// <param name="datuak">Erakutsi beharreko testua edo taula.</param>
        public EmaitzakOrria(string izenburua, string datuak)
        {
            InitializeComponent(); // Osagaiak hasieratu
            IzenburuaEtiketa.Text = izenburua;
            DatuakEtiketa.Text = datuak;
        }

        /// <summary>
        /// Leihoa ixteko botoiaren gertaera.
        /// </summary>
        private async void ItxiBotoiaSakatzean(object igorlea, EventArgs e)
        {
            // Leiho modala pilatik atera (itxi)
            await Navigation.PopModalAsync();
        }
    }
}