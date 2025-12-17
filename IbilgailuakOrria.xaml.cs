using IstripuenKudeaketaYago.Data;
using IstripuenKudeaketaYago.Models;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;
using System.Linq; // Beharrezkoa estatistiketarako

namespace IstripuenKudeaketaYago
{
    /// <summary>
    /// Ibilgailuak kudeatzeko orria (CRUD eragiketak).
    /// </summary>
    public partial class IbilgailuakOrria : ContentPage
    {
        private readonly DatuBasea _datuBasea;
        private List<Ibilgailua> _ibilgailuZerrenda;
        private int _oraingoIndizea = -1;
        private bool _erregistroBerriaDa = false;

        // Komandoak
        public ICommand JoanHasieraraKomandoa { get; }
        public ICommand AldatuLekuaKomandoa { get; }
        public ICommand EguneratuKomandoa { get; }
        public ICommand AldatuGaiaKomandoa { get; }

        public IbilgailuakOrria()
        {
            InitializeComponent();
            _datuBasea = App.DatuBasea;

            // --- MENUAREN KOMANDOAK ---
            JoanHasieraraKomandoa = new Command(async () =>
            {
                try { await Shell.Current.GoToAsync("///OrriNagusia"); }
                catch (Exception e) { await DisplayAlert("Errorea", e.Message, "Ados"); }
            });

            // --- 1. ESTATISTIKAK (Logika osoa) ---
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

            // --- 2. INPRIMATU: URTEKA (Logika osoa) ---
            EguneratuKomandoa = new Command(async () =>
            {
                var txostena = await SortuUrtekoTaulaAsync();
                await Navigation.PushModalAsync(new EmaitzakOrria("ISTRIPUAK URTEKA", txostena));
            });

            // --- 3. INPRIMATU: ISTRIPU LARRIENA (Logika osoa) ---
            AldatuGaiaKomandoa = new Command(async () =>
            {
                var xehetasunak = await LortuIstripuLarrienaAsync();
                await Navigation.PushModalAsync(new EmaitzakOrria("ISTRIPU LARRIENA", xehetasunak));
            });

            BindingContext = this;
        }

        /// <summary>
        /// Orria agertzen denean datuak kargatzen ditu.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await DatuakKargatuAsync();
        }

        // --- TXOSTENETARAKO METODOAK (OrriNagusitik ekarriak) ---

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
            kateEraikitzailea.AppendLine($"🏥 OSPITALERATU.: {larriena.Ospitaleratuak}");
            kateEraikitzailea.AppendLine($"🤕 ARINAK:        {larriena.Arinak}");
            kateEraikitzailea.AppendLine("-----------------------------------");
            kateEraikitzailea.AppendLine($"GUZTIRA:          {larriena.Hildakoak + larriena.Ospitaleratuak + larriena.Arinak}");

            return kateEraikitzailea.ToString();
        }

        // --- DATUAK KUDEATZEKO LOGIKA (CRUD) ---
        private async Task DatuakKargatuAsync()
        {
            _ibilgailuZerrenda = await _datuBasea.LortuIbilgailuakAsync();
            if (_ibilgailuZerrenda.Count > 0)
            {
                _oraingoIndizea = 0;
                ErakutsiErregistroa();
            }
            else
            {
                _oraingoIndizea = -1;
                GarbituFormularioa();
                KontagailuaEtiketa.Text = "0 / 0";
            }
        }

        private void ErakutsiErregistroa()
        {
            if (_oraingoIndizea >= 0 && _oraingoIndizea < _ibilgailuZerrenda.Count)
            {
                var ibilgailua = _ibilgailuZerrenda[_oraingoIndizea];
                IdSarrera.Text = ibilgailua.Id.ToString();
                IzenaSarrera.Text = ibilgailua.Izena;
                KontagailuaEtiketa.Text = $"{_oraingoIndizea + 1} / {_ibilgailuZerrenda.Count}";
                _erregistroBerriaDa = false;
                IdSarrera.IsReadOnly = true;
            }
        }

        private void GarbituFormularioa()
        {
            IdSarrera.Text = "";
            IzenaSarrera.Text = "";
            IdSarrera.IsReadOnly = false;
        }

        // --- BOTOIEN GERTAERAK ---
        private void LehenengoaKlikatzean(object igorlea, EventArgs e) { if (_ibilgailuZerrenda?.Count > 0) { _oraingoIndizea = 0; ErakutsiErregistroa(); } }
        private void AurrekoaKlikatzean(object igorlea, EventArgs e) { if (_oraingoIndizea > 0) { _oraingoIndizea--; ErakutsiErregistroa(); } }
        private void HurrengoaKlikatzean(object igorlea, EventArgs e) { if (_oraingoIndizea < _ibilgailuZerrenda.Count - 1) { _oraingoIndizea++; ErakutsiErregistroa(); } }
        private void AzkenaKlikatzean(object igorlea, EventArgs e) { if (_ibilgailuZerrenda?.Count > 0) { _oraingoIndizea = _ibilgailuZerrenda.Count - 1; ErakutsiErregistroa(); } }

        private void BerriaKlikatzean(object igorlea, EventArgs e)
        {
            _erregistroBerriaDa = true;
            GarbituFormularioa();
            IdSarrera.Focus();
            KontagailuaEtiketa.Text = "Berria";
        }

        private async void GordeKlikatzean(object igorlea, EventArgs e)
        {
            if (!int.TryParse(IdSarrera.Text, out int idBerria)) { await DisplayAlert("Errorea", "IDa zenbakia izan behar da", "Ados"); return; }
            var ibilgailua = new Ibilgailua { Id = idBerria, Izena = IzenaSarrera.Text };

            try
            {
                await _datuBasea.GordeIbilgailuaAsync(ibilgailua);
                await DatuakKargatuAsync();
                await DisplayAlert("Ondo", "Gordeta", "Ados");
            }
            catch
            {
                await DisplayAlert("Errorea", "Errorea gordetzerakoan", "Ados");
            }
        }

        private async void EzabatuKlikatzean(object igorlea, EventArgs e)
        {
            if (_oraingoIndizea >= 0 && !_erregistroBerriaDa)
            {
                if (await DisplayAlert("Ezabatu", "Ziur zaude ezabatu nahi duzula?", "Bai", "Ez"))
                {
                    await _datuBasea.EzabatuIbilgailuaAsync(_ibilgailuZerrenda[_oraingoIndizea]);
                    await DatuakKargatuAsync();
                }
            }
        }
    }
}