using Program.Models;
using Program.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Logic
{
    public class Bankomat
    {
        private readonly IDataService _dataService;
        private readonly List<Type> _akceptowaneTypyKart;

        // Obecna karta włożona do bankomatu
        private KartaBankomatowa _wlozonaKarta = null;

        // Konstruktor symulujący "instalację" bankomatu
        // Tutaj definiujemy, jakie typy kart są akceptowane (Wymaganie projektowe)
        public Bankomat(IDataService dataService)
        {
            _dataService = dataService;

            // Określamy listę akceptowanych TYPÓW kart
            _akceptowaneTypyKart = new List<Type>
        {
            typeof(Visa),
            typeof(Mastercard),
            typeof(AmericanExpress),
            typeof(VisaElectron)
            // Można by tu NIE dodać np. AmericanExpress, jeśli bankomat jej nie obsługuje
        };

            Console.WriteLine("--- BANKOMAT ZAINSTALOWANY I GOTOWY ---");
            Console.WriteLine($"Akceptowane typy kart: {string.Join(", ", _akceptowaneTypyKart.Select(t => t.Name))}");
            Console.WriteLine("--------------------------------------");
        }

        /// <summary>
        /// Symuluje włożenie karty i wstępną weryfikację.
        /// </summary>
        public bool WlozKarte(KartaBankomatowa karta)
        {
            if (!_akceptowaneTypyKart.Contains(karta.GetType()))
            {
                Console.WriteLine($"[BANKOMAT] Błąd: Karta typu {karta.GetType().Name} nie jest akceptowana.");
                return false;
            }

            // Karta jest akceptowana! Sprawdzamy system
            if (!karta.WeryfikujDostepnoscSystemu())
            {
                Console.WriteLine("[BANKOMAT] Błąd: System autoryzacyjny niedostępny.");
                return false;
            }

            _wlozonaKarta = karta;
            Console.WriteLine($"[BANKOMAT] Karta typu {karta.GetType().Name} zaakceptowana. Proszę podać PIN.");
            return true;
        }

        /// <summary>
        /// Weryfikuje PIN i autoryzuje transakcję.
        /// </summary>
        public bool Autoryzuj(string pin)
        {
            if (_wlozonaKarta == null)
            {
                Console.WriteLine("[BANKOMAT] Błąd: Brak włożonej karty.");
                return false;
            }

            // Walidacja formatu PIN na poziomie KartaBankomatowa (Wymaganie 8)
            try
            {
                // PIN jest weryfikowany w klasie SqlDataService
                if (_dataService.WalidujPin(_wlozonaKarta.IDKonta, pin))
                {
                    Console.WriteLine("[BANKOMAT] PIN poprawny. Sesja autoryzowana.");
                    return true;
                }
                else
                {
                    Console.WriteLine("[BANKOMAT] Błąd autoryzacji: Niepoprawny PIN!");
                }
            }
            catch (InvalidPinFormatException ex)
            {
                Console.WriteLine($"[BANKOMAT] Błąd: {ex.Message}");
            }

            // Zawsze czyścimy sesję po nieudanej autoryzacji
            _wlozonaKarta = null;
            return false;
        }

        /// <summary>
        /// Wykonuje operację wypłaty gotówki (CRUD: Update salda).
        /// </summary>

        public bool WyplacGotowke(decimal kwota)
        {
            if (_wlozonaKarta == null)
            {
                Console.WriteLine("[BANKOMAT] Błąd: Brak autoryzowanej karty w sesji.");
                return false;
            }

            try
            {
                // 1. Walidacja Kwoty (Wymaganie 8)
                if (kwota <= 0)
                {
                    throw new InvalidWithdrawalAmountException("Kwota wypłaty musi być większa od zera.");
                }
                if (kwota % 10.00m != 0)
                {
                    throw new InvalidWithdrawalAmountException("Kwota wypłaty musi być wielokrotnością 10 PLN.");
                }

                // 2. Walidacja Salda (Wymaganie 7/8 - rzuca wyjątek z warstwy Services)
                _dataService.CzyWystarczajaceSaldo(_wlozonaKarta.IDKonta, kwota); // Jeśli brakuje środków, rzuca InsufficientFundsException

                // 3. Wykonanie Transakcji (CRUD: UPDATE)
                KontoBankowe konto = _dataService.PobierzKontoPoID(_wlozonaKarta.IDKonta);
                decimal noweSaldo = konto.Saldo - kwota;

                if (_dataService.AktualizujSaldo(konto.IDKonta, noweSaldo))
                {
                    Console.WriteLine($"\n[BANKOMAT] Pomyślnie wypłacono: {kwota:C} PLN.");
                    Console.WriteLine($"[BANKOMAT] Nowe saldo: {noweSaldo:C} PLN.");
                    return true;
                }
            }
            // OBSŁUGA WYJĄTKÓW (Wymaganie 7)
            catch (InvalidWithdrawalAmountException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[BANKOMAT BŁĄD WALIDACJI] {ex.Message}");
                Console.ResetColor();
            }
            catch (InsufficientFundsException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[BANKOMAT BŁĄD TRANSAKCJI] {ex.Message}");
                Console.ResetColor();
            }
            catch (BankomatException ex)
            {
                // Łapanie innych błędów z systemu Bankomatu
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[BANKOMAT KRYTYCZNY BŁĄD] Wystąpił błąd: {ex.Message}");
                Console.ResetColor();
            }

            return false;
        }

        public void ZwrocKarte()
        {
            if (_wlozonaKarta != null)
            {
                Console.WriteLine($"[BANKOMAT] Proszę odebrać kartę {_wlozonaKarta.GetType().Name}.");
            }
            _wlozonaKarta = null;
        }

    }
}
