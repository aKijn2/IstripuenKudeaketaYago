using IstripuenKudeaketaYago.Data;
using IstripuenKudeaketaYago.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System;

namespace IstripuenKudeaketaYago
{
    /// <summary>
    /// Aplikazioaren sarrera orria. 
    /// Nabigazioa goiko menuaren bidez egiten da.
    /// </summary>
    public partial class OrriNagusia : ContentPage
    {
        private readonly DatuBasea _datuBasea;

        // --- KOMANDOAK (Bindings) ---
        public ICommand JoanIbilgailuetaraKomandoa { get; }
        public ICommand JoanIstripuetaraKomandoa { get; }
        public ICommand AldatuLekuaKomandoa { get; }
        public ICommand EguneratuKomandoa { get; }
        public ICommand AldatuGaiaKomandoa { get; }

        public OrriNagusia()
        {
            InitializeComponent();
            _datuBasea = App.DatuBasea;

            // --- NABIGAZIOA (Mantentze-lanak menua) ---
            JoanIbilgailuetaraKomandoa = new Command(async () => await Shell.Current.GoToAsync(nameof(IbilgailuakOrria)));
            JoanIstripuetaraKomandoa = new Command(async () => await Shell.Current.GoToAsync(nameof(IstripuakOrria)));

            // --- 1. ESTATISTIKAK ---
            AldatuLekuaKomandoa = new Command<string>(async (parametroa) =>
            {
                await _datuBasea.HasieratuAsync();

                if (parametroa == "Hilketa")
                {
                    var istripuak = await _datuBasea.LortuIstripuakAsync();
                    if (!istripuak.Any()) { await DisplayAlert("Informazioa", "Ez dago daturik.", "Ados"); return; }

                    var taldea = istripuak.GroupBy(i => i.Data.Year)
                                          .OrderByDescending(t => t.Sum(x => x.Hildakoak))
                                          .FirstOrDefault();

                    await DisplayAlert("📉 Estatistika", $"Hildako GEHIEN izan dituen urtea:\n\n📅 Urtea: {taldea.Key}\n💀 Hildakoak: {taldea.Sum(x => x.Hildakoak)}", "Ados");
                }
                else if (parametroa == "Istripuak")
                {
                    var istripuak = await _datuBasea.LortuIstripuakAsync();
                    if (!istripuak.Any()) { await DisplayAlert("Informazioa", "Ez dago daturik.", "Ados"); return; }

                    var taldea = istripuak.GroupBy(i => i.Data.Year)
                                          .OrderBy(t => t.Count())
                                          .FirstOrDefault();

                    await DisplayAlert("📈 Estatistika", $"Istripu GUTXIEN izan dituen urtea:\n\n📅 Urtea: {taldea.Key}\n💥 Kopurua: {taldea.Count()}", "Ados");
                }
            });

            // --- 2. INPRIMATU: URTEKA ---
            EguneratuKomandoa = new Command(async () =>
            {
                var txostena = await SortuUrtekoTaulaAsync();
                await Navigation.PushModalAsync(new EmaitzakOrria("ISTRIPUAK URTEKA", txostena));
            });

            // --- 3. INPRIMATU: ISTRIPU LARRIENA ---
            AldatuGaiaKomandoa = new Command(async () =>
            {
                var xehetasunak = await LortuIstripuLarrienaAsync();
                await Navigation.PushModalAsync(new EmaitzakOrria("ISTRIPU LARRIENA", xehetasunak));
            });

            BindingContext = this;
        }

        // --- METODO LAGUNTZAILEAK ---

        /// <summary>
        /// Urteko istripu taula sortzen du.
        /// </summary>
        private async Task<string> SortuUrtekoTaulaAsync()
        {
            var zerrenda = await _datuBasea.LortuIstripuakAsync();
            if (zerrenda.Count == 0) return "Ez dago daturik.";

            var taldeak = zerrenda.GroupBy(x => x.Data.Year).OrderBy(g => g.Key);
            var kateEraikitzailea = new StringBuilder();

            kateEraikitzailea.AppendLine(" URTEA | KOP. | HIL. | OSP. | ARIN. ");
            kateEraikitzailea.AppendLine("-------+------+------+------+-------");

            foreach (var taldea in taldeak)
            {
                int istripuKopurua = taldea.Count();
                int hildakoak = taldea.Sum(x => x.Hildakoak);
                int ospitaleratuak = taldea.Sum(x => x.Ospitaleratuak);
                int arinak = taldea.Sum(x => x.Arinak);

                kateEraikitzailea.AppendLine(string.Format(" {0,-5} | {1,4} | {2,4} | {3,4} | {4,5} ",
                    taldea.Key, istripuKopurua, hildakoak, ospitaleratuak, arinak));
            }
            kateEraikitzailea.AppendLine("-------+------+------+------+-------");
            kateEraikitzailea.AppendLine($" GUZTIRA: {zerrenda.Count} erregistro.");

            return kateEraikitzailea.ToString();
        }

        /// <summary>
        /// Istripu larrienaren datuak lortzen ditu.
        /// </summary>
        private async Task<string> LortuIstripuLarrienaAsync()
        {
            var zerrenda = await _datuBasea.LortuIstripuakAsync();
            if (zerrenda.Count == 0) return "Ez dago daturik.";

            var larriena = zerrenda.OrderByDescending(x => x.Hildakoak + x.Arinak + x.Ospitaleratuak).FirstOrDefault();
            var ibilgailua = await _datuBasea.LortuIbilgailuaIdBidezAsync(larriena.IbilgailuId);
            string ibilgailuIzena = ibilgailua?.Izena ?? "Ezezaguna";

            var kateEraikitzailea = new StringBuilder();
            kateEraikitzailea.AppendLine("BIKTIMA GEHIEN IZAN DITUEN ISTRIPUA");
            kateEraikitzailea.AppendLine("===================================");
            kateEraikitzailea.AppendLine($"🆔 ID:      {larriena.Id}");
            kateEraikitzailea.AppendLine($"📅 DATA:    {larriena.Data:yyyy/MM/dd}");
            kateEraikitzailea.AppendLine($"🚗 IBILG.:  {ibilgailuIzena}");
            kateEraikitzailea.AppendLine("-----------------------------------");
            kateEraikitzailea.AppendLine($"💀 HILDAKOAK:     {larriena.Hildakoak}");
            kateEraikitzailea.AppendLine($"TOTALA:           {larriena.Hildakoak + larriena.Ospitaleratuak + larriena.Arinak}");

            return kateEraikitzailea.ToString();
        }
    }
}