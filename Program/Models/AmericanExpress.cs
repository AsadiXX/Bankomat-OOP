using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Models
{
    public class AmericanExpress : KartaBankomatowa
    {
        public AmericanExpress(string numerKarty, string wlasciciel, string pin, int idKonta)
            : base(numerKarty, wlasciciel, pin, idKonta)
        {
        }

        public override bool WeryfikujDostepnoscSystemu()
        {
            // American Express może miec inne wymagania autoryzacyjne
            Console.WriteLine(" -> Weryfikacja karty w systemie American Express: OK.");
            return true;
        }
    }
}
