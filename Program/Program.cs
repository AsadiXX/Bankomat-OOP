using System;
using Program.Services;
using Program.Logic;
using Program.Models;
using System.Linq;

public class Program1
{
    // PRZYKŁADOWE KARTY TESTOWE DO WYBORU 
    private static List<KartaBankomatowa> TestoweKarty = new List<KartaBankomatowa>
    {
        // Karta American Express: (Konto ID 100, PIN 0000, Saldo 10000.00)
        new AmericanExpress("370000000000000", "Jan Wójcik", "0000", 100),
        // Karta Visa: (Konto ID 101, PIN 1234, Saldo 1500.50)
        new Visa("4567123456789012", "Anna Kowalska", "1234", 101),
        // Karta Mastercard: (Konto ID 102, PIN 4321, Saldo 450.00)
        new Mastercard("5555432187654321", "Piotr Nowak", "4321", 102)
    };

    // START APLIKACJI
    public static void Main(string[] args)
    {
        // INICJALIZACJA USŁUG
        IDataService dataService = new SqlDataService();
        Bankomat atm = new Bankomat(dataService);

        Console.WriteLine("\n=== Witamy w Bankomacie ===");

        // KROK 1: SYMULACJA WŁOŻENIA KARTY (Wybór z listy)
        KartaBankomatowa karta = WybierzKarte(atm);
        if (karta == null) return;

        // KROK 2: WERYFIKACJA PIN (Przed wejściem do menu)
        if (!SesjaAutoryzacji(atm))
        {
            atm.ZwrocKarte();
            Console.WriteLine("Błędny PIN. Karta została zwrócona.");
            return;
        }

        // KROK 3: GŁÓWNA PĘTLA SESJI
        bool czyKontynuowac = true;
        while (czyKontynuowac)
        {
            // MenuTransakcji zwraca 'false' tylko gdy użytkownik wybierze "Zakończ"
            czyKontynuowac = MenuTransakcji(atm);
        }

        atm.ZwrocKarte();
        Console.WriteLine("\n=== Sesja zakończona. Pobierz kartę. ===");

        // KROK 4: EKSPORT (Opcjonalny po zakończeniu sesji)
        Console.WriteLine("\nCzy chcesz wygenerować raport kont do pliku CSV? (t/n)");
        string wyborRaportu = Console.ReadLine();

        if (wyborRaportu?.ToLower() == "t")
        {
            var konta = dataService.PobierzWszystkieKonta();
            var csvService = new CsvFileService();
            csvService.ExportujDoCsv(konta, "RaportKont.csv");
            Console.WriteLine("Plik 'RaportKont.csv' został wygenerowany w folderze bin.");
        }

        Console.WriteLine("\n=== Dziękujemy za skorzystanie z Bankomatu! ===");
    }
    // --- POMOCNICZE METODY INTERAKCJI ---

    private static KartaBankomatowa WybierzKarte(Bankomat atm)
    {
        while (true) // Pętla będzie się powtarzać, aż napotka instrukcję 'return'
        {
            Console.WriteLine("\nProszę włożyć kartę (Wybierz numer karty do symulacji):");

            for (int i = 0; i < TestoweKarty.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {TestoweKarty[i].GetType().Name} - {TestoweKarty[i].NumerKarty.Substring(0, 4)}XXXX... (Właściciel: {TestoweKarty[i].Wlasciciel})");
            }

            Console.Write("Wybór (1-3): ");
            string wejscie = Console.ReadLine();

            // Sprawdzamy, czy użytkownik wpisał liczbę i czy mieści się ona w zakresie listy kart
            if (int.TryParse(wejscie, out int wybor) && wybor > 0 && wybor <= TestoweKarty.Count)
            {
                var karta = TestoweKarty[wybor - 1];

                // Próba włożenia karty do obiektu bankomatu
                if (atm.WlozKarte(karta))
                {
                    Console.WriteLine($"\n[SYSTEM]: Karta zaakceptowana. Witaj {karta.Wlasciciel}.");
                    return karta; // Zwracamy kartę i kończymy metodę (pętla zostaje przerwana)
                }
                else
                {
                    Console.WriteLine("\n[BŁĄD]: Bankomat odrzucił tę kartę. Spróbuj innej.");
                }
            }
            else
            {
                // Ten blok wykona się, gdy użytkownik wpisze np. 4, 0, "abc" lub zostawi puste pole
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[BŁĄD]: Niepoprawny wybór. Wybierz numer od 1 do " + TestoweKarty.Count);
                Console.ResetColor();

                Console.WriteLine("Naciśnij dowolny klawisz, aby spróbować ponownie...");
                Console.ReadKey();
                Console.Clear(); // Czyścimy konsolę, aby menu wyświetliło się ponownie na czystym ekranie
            }
        }
    }

    private static bool SesjaAutoryzacji(Bankomat atm)
    {
        int proby = 3;

        while (proby > 0)
        {
            Console.Write($"\nProszę wprowadzić PIN (Pozostało prób: {proby}): ");
            string pin = Console.ReadLine();

            // Wywołujemy autoryzację
            if (atm.Autoryzuj(pin))
            {
                return true;
            }
            else
            {
                proby--;
                if (proby > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[BŁĄD]: Niepoprawny PIN.");
                    Console.ResetColor();
                }
            }
        }

        // Dopiero tutaj, gdy wszystkie próby zawiodą:
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n[BŁĄD]: 3-krotnie podano błędny PIN. Sesja zakończona.");
        Console.ResetColor();

        return false;
    }
    private static bool MenuTransakcji(Bankomat atm)
    {
        Console.WriteLine("\n--- MENU BANKOMATU ---");
        Console.WriteLine("1. Wypłata Gotówki");
        Console.WriteLine("2. Zakończ sesję i wyjmij kartę");
        Console.Write("Wybór (1-2): ");

        string wybor = Console.ReadLine();

        switch (wybor)
        {
            case "1":
                return WypłataInteraktywna(atm);
            case "2":
                Console.WriteLine("Zamykanie sesji...");
                return false; // Zwraca false, co przerywa pętlę while w Main
            default:
                Console.WriteLine("[BŁĄD]: Niepoprawna opcja. Wybierz 1 lub 2.");
                return true; // Zwraca true, więc pętla w Main kręci się dalej
        }
    }

    private static bool WypłataInteraktywna(Bankomat atm)
    {
        Console.Write("Wprowadź kwotę do wypłaty (wielokrotność 10 PLN): ");
        string kwotaStr = Console.ReadLine();

        if (decimal.TryParse(kwotaStr, out decimal kwota))
        {
            atm.WyplacGotowke(kwota);
        }
        else
        {
            Console.WriteLine("Niepoprawny format kwoty.");
        }

        Console.WriteLine("\nNaciśnij ENTER, aby wrócić do menu...");
        Console.ReadLine();
        return true;
    }

}