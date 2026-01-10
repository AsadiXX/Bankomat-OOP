using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Models
{
    public class Mastercard : KartaBankomatowa
    {
        public Mastercard(string numerKarty, string wlasciciel, string pin, int idKonta)
            : base(numerKarty, wlasciciel, pin, idKonta)
        {
        }

        public override bool WeryfikujDostepnoscSystemu()
        {
            // Logika specyficzna dla systemu Mastercard
            Console.WriteLine(" -> Weryfikacja karty w systemie Mastercard: OK.");
            return true;
        }
    }
}
