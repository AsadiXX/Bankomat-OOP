using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Models
{
    public class KontoBankowe
    {
        public int IDKonta { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public decimal Saldo { get; set; }
        public string PINHash { get; set; } // Lepiej przechowywać PIN jako Hash, a nie czysty ciąg

        // Konstruktor na potrzeby odczytu z bazy
        public KontoBankowe(int idKonta, string imie, string nazwisko, decimal saldo, string pinHash)
        {
            this.IDKonta = idKonta;
            this.Imie = imie;
            this.Nazwisko = nazwisko;
            this.Saldo = saldo;
            this.PINHash = pinHash;
        }
    }
}
