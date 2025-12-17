using SQLite;

namespace IstripuenKudeaketaYago.Models
{
    /// <summary>
    /// Ibilgailuaren datuak gordetzeko klasea.
    /// Datu-baseko taula bat ordezkatzen du.
    /// </summary>
    public class Ibilgailua
    {
        /// <summary>
        /// Ibilgailuaren identifikatzaile bakarra (Gako nagusia).
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Ibilgailu motaren izena (Adibidez: "Turismoa", "Bizikleta").
        /// </summary>
        public string Izena { get; set; } = string.Empty;
    }
}