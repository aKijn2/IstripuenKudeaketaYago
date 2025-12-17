using SQLite;
using System;

namespace IstripuenKudeaketaYago.Models
{
    /// <summary>
    /// Istripu baten datuak gordetzeko klasea.
    /// </summary>
    public class Istripua
    {
        /// <summary>
        /// Istripuaren identifikatzaile bakarra.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Istripuan inplikatutako ibilgailuaren IDa (Erlazioa).
        /// </summary>
        public int IbilgailuId { get; set; } // Lehen "IdIbilgailua"

        /// <summary>
        /// Istripua gertatu zen data.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Hildako pertsonen kopurua.
        /// </summary>
        public int Hildakoak { get; set; } // Lehen "Hilketa"

        /// <summary>
        /// Ospitaleratutako pertsonen kopurua.
        /// </summary>
        public int Ospitaleratuak { get; set; } // Lehen "Ospitaleratzeak"

        /// <summary>
        /// Zauri arinak izan dituztenen kopurua.
        /// </summary>
        public int Arinak { get; set; }
    }
}