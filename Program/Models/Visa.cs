using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Models
{
    public class Visa : KartaBankomatowa
    {
        public Visa(string numerKarty, string wlasciciel, string pin, int idKonta)
            : base(numerKarty, wlasciciel, pin, idKonta)
        {
            // Specyficzna inicjalizacja dla Visy (opcjonalnie)
        }

        public override bool WeryfikujDostepnoscSystemu()
        {
            // Logika specyficzna dla systemu Visa (np. sprawdzenie dostępności ich serwerów)
            Console.WriteLine(" -> Weryfikacja karty w systemie Visa: OK.");
            return true;
        }

        public void NaliczOplate()
        {
            // Metoda unikalna dla Visy (np. naliczanie prowizji)
        }
    }
}