using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using IstripuenKudeaketaYago.Models;
using System;

namespace IstripuenKudeaketaYago.Data
{
    /// <summary>
    /// SQLite datu-basearekin konexioa eta eragiketak kudeatzen dituen klasea.
    /// </summary>
    public class DatuBasea
    {
        private readonly SQLiteAsyncConnection _konexioa;
        private bool _hasieratuta = false;

        public DatuBasea(string dbBidea)
        {
            _konexioa = new SQLiteAsyncConnection(dbBidea);
        }

        public async Task HasieratuAsync()
        {
            if (_hasieratuta) return;

            // --- ALDAKETA GARRANTZITSUA ---
            // Lehenengo, taula zaharrak ezabatuko ditugu datu berriak kargatzeko.
            // Hau probak egiteko bakarrik da. Datuak mantendu nahi dituzunean, kendu bi lerro hauek.
            await _konexioa.DropTableAsync<Ibilgailua>();
            await _konexioa.DropTableAsync<Istripua>();
            // -----------------------------

            // Taulak sortu berriro (hutsik egongo dira orain)
            await _konexioa.CreateTableAsync<Ibilgailua>();
            await _konexioa.CreateTableAsync<Istripua>();

            // 1. IBILGAILUAK
            if (await _konexioa.Table<Ibilgailua>().CountAsync() == 0)
            {
                await _konexioa.InsertAllAsync(new List<Ibilgailua>
                {
                    new Ibilgailua{ Izena="Bizikletak" },
                    new Ibilgailua{ Izena="Ziklomotorrak" },
                    new Ibilgailua{ Izena="Motozikletak" },
                    new Ibilgailua{ Izena="Turismoak" }
                });
            }

            // 2. ISTRIPUAK (Zerrenda luzea)
            if (await _konexioa.Table<Istripua>().CountAsync() == 0)
            {
                var istripuZerrenda = new List<Istripua>
                {
                    // 2019
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2019,1,15), Hildakoak=0, Ospitaleratuak=1, Arinak=2 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2019,2,10), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2019,3,05), Hildakoak=1, Ospitaleratuak=2, Arinak=0 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2019,4,12), Hildakoak=0, Ospitaleratuak=1, Arinak=1 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2019,5,20), Hildakoak=0, Ospitaleratuak=0, Arinak=3 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2019,6,15), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2019,7,01), Hildakoak=0, Ospitaleratuak=1, Arinak=4 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2019,8,14), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2019,9,23), Hildakoak=1, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2019,10,11), Hildakoak=0, Ospitaleratuak=0, Arinak=2 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2019,11,05), Hildakoak=0, Ospitaleratuak=1, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2019,12,24), Hildakoak=0, Ospitaleratuak=0, Arinak=5 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2019,5,15), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2019,8,20), Hildakoak=0, Ospitaleratuak=0, Arinak=2 },

                    // 2020
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2020,1,10), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2020,2,15), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2020,3,12), Hildakoak=0, Ospitaleratuak=1, Arinak=0 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2020,7,20), Hildakoak=1, Ospitaleratuak=2, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2020,9,05), Hildakoak=0, Ospitaleratuak=0, Arinak=2 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2020,11,30), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2020,4,10), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2020,12,01), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },

                    // 2021
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2021,1,05), Hildakoak=1, Ospitaleratuak=1, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2021,2,14), Hildakoak=2, Ospitaleratuak=3, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2021,3,20), Hildakoak=0, Ospitaleratuak=1, Arinak=2 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2021,4,10), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2021,5,05), Hildakoak=1, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2021,6,12), Hildakoak=0, Ospitaleratuak=2, Arinak=3 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2021,7,08), Hildakoak=0, Ospitaleratuak=1, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2021,8,15), Hildakoak=3, Ospitaleratuak=4, Arinak=2 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2021,9,22), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2021,10,30), Hildakoak=1, Ospitaleratuak=1, Arinak=1 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2021,11,11), Hildakoak=0, Ospitaleratuak=1, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2021,12,05), Hildakoak=0, Ospitaleratuak=0, Arinak=4 },

                    // 2022
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2022,1,02), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2022,1,15), Hildakoak=0, Ospitaleratuak=0, Arinak=2 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2022,2,20), Hildakoak=0, Ospitaleratuak=1, Arinak=3 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2022,3,05), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2022,3,25), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2022,4,10), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2022,5,12), Hildakoak=0, Ospitaleratuak=0, Arinak=2 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2022,6,18), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2022,7,22), Hildakoak=0, Ospitaleratuak=1, Arinak=5 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2022,8,09), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2022,9,14), Hildakoak=0, Ospitaleratuak=0, Arinak=3 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2022,10,02), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2022,11,11), Hildakoak=0, Ospitaleratuak=0, Arinak=2 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2022,12,20), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2022,12,30), Hildakoak=0, Ospitaleratuak=0, Arinak=4 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2022,5,05), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2022,9,10), Hildakoak=1, Ospitaleratuak=0, Arinak=0 },

                    // 2023
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2023,1,10), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2023,2,14), Hildakoak=1, Ospitaleratuak=1, Arinak=0 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2023,3,20), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2023,4,05), Hildakoak=0, Ospitaleratuak=0, Arinak=2 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2023,5,15), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2023,6,25), Hildakoak=0, Ospitaleratuak=1, Arinak=3 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2023,7,30), Hildakoak=0, Ospitaleratuak=2, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2023,8,12), Hildakoak=1, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2023,9,18), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2023,10,22), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2023,11,05), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2023,11,20), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2023,12,01), Hildakoak=0, Ospitaleratuak=1, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2023,12,15), Hildakoak=0, Ospitaleratuak=0, Arinak=2 },
                    new Istripua{ IbilgailuId=1, Data=new DateTime(2023,12,31), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2023,2,22), Hildakoak=0, Ospitaleratuak=0, Arinak=1 },
                    new Istripua{ IbilgailuId=2, Data=new DateTime(2023,4,14), Hildakoak=0, Ospitaleratuak=0, Arinak=0 },
                    new Istripua{ IbilgailuId=3, Data=new DateTime(2023,6,06), Hildakoak=0, Ospitaleratuak=1, Arinak=0 },
                    new Istripua{ IbilgailuId=4, Data=new DateTime(2023,8,28), Hildakoak=0, Ospitaleratuak=0, Arinak=2 }
                };

                await _konexioa.InsertAllAsync(istripuZerrenda);
            }

            _hasieratuta = true;
        }

        // --- IRAKURKETA METODOAK ---
        public async Task<List<Ibilgailua>> LortuIbilgailuakAsync()
        {
            await HasieratuAsync();
            return await _konexioa.Table<Ibilgailua>().ToListAsync();
        }

        public async Task<List<Istripua>> LortuIstripuakAsync()
        {
            await HasieratuAsync();
            return await _konexioa.Table<Istripua>().ToListAsync();
        }

        public async Task<Ibilgailua> LortuIbilgailuaIdBidezAsync(int id)
        {
            await HasieratuAsync();
            return await _konexioa.Table<Ibilgailua>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        // --- IDAZKETA METODOAK ---
        public async Task<int> GordeIbilgailuaAsync(Ibilgailua ibilgailua)
        {
            await HasieratuAsync();
            if (ibilgailua.Id != 0)
                return await _konexioa.UpdateAsync(ibilgailua);
            else
                return await _konexioa.InsertAsync(ibilgailua);
        }

        public async Task<int> GordeIstripuaAsync(Istripua istripua)
        {
            await HasieratuAsync();
            if (istripua.Id != 0)
                return await _konexioa.UpdateAsync(istripua);
            else
                return await _konexioa.InsertAsync(istripua);
        }

        public async Task<int> EzabatuIbilgailuaAsync(Ibilgailua ibilgailua)
        {
            await HasieratuAsync();
            return await _konexioa.DeleteAsync(ibilgailua);
        }

        public async Task<int> EzabatuIstripuaAsync(Istripua istripua)
        {
            await HasieratuAsync();
            return await _konexioa.DeleteAsync(istripua);
        }
    }
}