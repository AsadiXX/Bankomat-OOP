using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Models
{
    public class VisaElectron : Visa
    {
        // VisaElectron często ma większe ograniczenia, np. tylko debet
        public bool CzyTylkoDebet { get; private set; } = true;

        public VisaElectron(string numerKarty, string wlasciciel, string pin, int idKonta)
            : base(numerKarty, wlasciciel, pin, idKonta)
        {
        }

        public override bool WeryfikujDostepnoscSystemu()
        {
            Console.WriteLine(" -> Weryfikacja karty w systemie Visa Electron (z ograniczonym debetem): OK.");
            return true;
        }
    }
}
