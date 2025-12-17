using IstripuenKudeaketaYago.Data;
using IstripuenKudeaketaYago.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System;

namespace IstripuenKudeaketaYago
{
    /// <summary>
    /// Istripuak kudeatzeko orria.
    /// </summary>
    public partial class IstripuakOrria : ContentPage
    {
        private readonly DatuBasea _datuBasea;
        private List<Istripua> _istripuZerrenda;
        private List<Ibilgailua> _ibilgailuZerrendaCombo;
        private int _oraingoIndizea = -1;
        private bool _erregistroBerriaDa = false;

        public ICommand JoanHasieraraKomandoa { get; }
        public ICommand AldatuLekuaKomandoa { get; }
        public ICommand EguneratuKomandoa { get; }
        public ICommand AldatuGaiaKomandoa { get; }

        public IstripuakOrria()
        {
            InitializeComponent();
            _datuBasea = App.DatuBasea;

            // --- MENU KOMANDOAK ---
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

        protected override async void OnAppearing() { base.OnAppearing(); await DatuakKargatuAsync(); }

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
            _ibilgailuZerrendaCombo = await _datuBasea.LortuIbilgailuakAsync();
            IbilgailuAukeratzailea.ItemsSource = _ibilgailuZerrendaCombo;

            _istripuZerrenda = await _datuBasea.LortuIstripuakAsync();

            if (_istripuZerrenda.Count > 0) { _oraingoIndizea = 0; ErakutsiErregistroa(); }
            else { _oraingoIndizea = -1; GarbituFormularioa(); KontagailuaEtiketa.Text = "0 / 0"; }
        }

        private void ErakutsiErregistroa()
        {
            if (_oraingoIndizea >= 0 && _oraingoIndizea < _istripuZerrenda.Count)
            {
                var istripua = _istripuZerrenda[_oraingoIndizea];
                IdSarrera.Text = istripua.Id.ToString();
                DataAukeratzailea.Date = istripua.Data;
                HildakoakSarrera.Text = istripua.Hildakoak.ToString();
                OspitaleratuakSarrera.Text = istripua.Ospitaleratuak.ToString();
                ArinakSarrera.Text = istripua.Arinak.ToString();

                IbilgailuAukeratzailea.SelectedItem = _ibilgailuZerrendaCombo.FirstOrDefault(v => v.Id == istripua.IbilgailuId);

                KontagailuaEtiketa.Text = $"{_oraingoIndizea + 1} / {_istripuZerrenda.Count}";
                _erregistroBerriaDa = false;
                IdSarrera.IsReadOnly = true;
            }
        }

        private void GarbituFormularioa()
        {
            IdSarrera.Text = "";
            DataAukeratzailea.Date = DateTime.Today;
            HildakoakSarrera.Text = "0";
            OspitaleratuakSarrera.Text = "0";
            ArinakSarrera.Text = "0";
            IbilgailuAukeratzailea.SelectedItem = null;
            IdSarrera.IsReadOnly = false;
        }

        // --- BOTOIEN GERTAERAK ---
        private void LehenengoaKlikatzean(object igorlea, EventArgs e) { if (_istripuZerrenda?.Count > 0) { _oraingoIndizea = 0; ErakutsiErregistroa(); } }
        private void AurrekoaKlikatzean(object igorlea, EventArgs e) { if (_oraingoIndizea > 0) { _oraingoIndizea--; ErakutsiErregistroa(); } }
        private void HurrengoaKlikatzean(object igorlea, EventArgs e) { if (_oraingoIndizea < _istripuZerrenda.Count - 1) { _oraingoIndizea++; ErakutsiErregistroa(); } }
        private void AzkenaKlikatzean(object igorlea, EventArgs e) { if (_istripuZerrenda?.Count > 0) { _oraingoIndizea = _istripuZerrenda.Count - 1; ErakutsiErregistroa(); } }

        private void BerriaKlikatzean(object igorlea, EventArgs e)
        {
            _erregistroBerriaDa = true;
            GarbituFormularioa();
            IdSarrera.Focus();
            KontagailuaEtiketa.Text = "Berria";
        }

        private async void GordeKlikatzean(object igorlea, EventArgs e)
        {
            if (!int.TryParse(IdSarrera.Text, out int id)) { await DisplayAlert("Errorea", "ID baliogabea", "Ados"); return; }
            if (IbilgailuAukeratzailea.SelectedItem == null) { await DisplayAlert("Errorea", "Ibilgailua aukeratu behar duzu", "Ados"); return; }

            int.TryParse(HildakoakSarrera.Text, out int h);
            int.TryParse(OspitaleratuakSarrera.Text, out int o);
            int.TryParse(ArinakSarrera.Text, out int a);
            var ibilgailua = (Ibilgailua)IbilgailuAukeratzailea.SelectedItem;

            var istripua = new Istripua
            {
                Id = id,
                IbilgailuId = ibilgailua.Id,
                Data = DataAukeratzailea.Date,
                Hildakoak = h,
                Ospitaleratuak = o,
                Arinak = a
            };

            try
            {
                await _datuBasea.GordeIstripuaAsync(istripua);
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
                if (await DisplayAlert("Ezabatu", "Ziur zaude?", "Bai", "Ez"))
                {
                    await _datuBasea.EzabatuIstripuaAsync(_istripuZerrenda[_oraingoIndizea]);
                    await DatuakKargatuAsync();
                }
            }
        }
    }
}