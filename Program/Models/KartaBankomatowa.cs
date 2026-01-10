using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Models
{
    public abstract class KartaBankomatowa
    {
        // Pola (Properties)
        public string NumerKarty { get; private set; }
        public string Wlasciciel { get; private set; }
        public string PIN { get; private set; }
        public int IDKonta { get; private set; }

        // Konstruktor
        public KartaBankomatowa(string numerKarty, string wlasciciel, string pin, int idKonta)
        {
            // Walidacja PIN-u na poziomie modelu
            if (!WalidujFormatPin(pin))
            {
                // Rzucamy wyjątek (Obsługa Wyjątków - Wymaganie 7)
                throw new InvalidPinFormatException("PIN musi składać się z 4 cyfr.");
            }

            this.NumerKarty = numerKarty;
            this.Wlasciciel = wlasciciel;
            this.PIN = pin;
            this.IDKonta = idKonta;
        }

        // Prywatna metoda walidacyjna
        private bool WalidujFormatPin(string pin)
        {
            // Sprawdzanie, czy PIN ma dokładnie 4 znaki i są to cyfry
            return pin != null && pin.Length == 4 && pin.All(char.IsDigit);
        }

        // Abstrakcyjna metoda - musi być zaimplementowana w klasach pochodnych.
        // Symuluje specyficzny typ weryfikacji lub opłat.
        public abstract bool WeryfikujDostepnoscSystemu();

        // Metoda wspólna dla wszystkich kart
        public bool WeryfikujPin(string wprowadzonyPin)
        {
            return this.PIN == wprowadzonyPin;
        }
    }
}
