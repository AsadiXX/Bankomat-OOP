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
        // 1. INICJALIZACJA USŁUG
        IDataService dataService = new SqlDataService();
        Bankomat atm = new Bankomat(dataService);

        Console.WriteLine("\n=== Witamy w Bankomacie ===");

        // KROK 1: SYMULACJA WŁOŻENIA KARTY (Wybór z listy)
        KartaBankomatowa karta = WybierzKarte(atm);
        if (karta == null) return;

        // KROK 2: JEDNORAZOWA WERYFIKACJA PIN (Przed wejściem do menu)
        if (!SesjaAutoryzacji(atm))
        {
            atm.ZwrocKarte();
            Console.WriteLine("Błędny PIN. Karta została zwrócona.");
            return;
        }

        // KROK 3: GŁÓWNA PĘTLA SESJI (Teraz tylko menu, bez ponownego PINu)
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
        Console.WriteLine("\nProszę włożyć kartę (Wybierz numer karty do symulacji):");
        for (int i = 0; i < TestoweKarty.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {TestoweKarty[i].GetType().Name} - {TestoweKarty[i].NumerKarty.Substring(0, 4)}XXXX... (Właściciel: {TestoweKarty[i].Wlasciciel})");
        }
        Console.Write("Wybór: ");

        if (int.TryParse(Console.ReadLine(), out int wybor) && wybor > 0 && wybor <= TestoweKarty.Count)
        {
            var karta = TestoweKarty[wybor - 1];
            if (atm.WlozKarte(karta))
            {
                return karta;
            }
        }
        Console.WriteLine("Niepoprawny wybór lub karta odrzucona.");
        return null;
    }

    private static bool SesjaAutoryzacji(Bankomat atm)
    {
        Console.Write("\nProszę wprowadzić PIN: ");
        string pin = Console.ReadLine();
        return atm.Autoryzuj(pin);
    }

    private static bool MenuTransakcji(Bankomat atm)
    {
        Console.WriteLine("\n--- MENU ---");
        Console.WriteLine("1. Wypłata Gotówki");
        Console.WriteLine("2. Zakończ sesję i wyjmij kartę");
        Console.Write("Wybór: ");

        string wybor = Console.ReadLine();

        switch (wybor)
        {
            case "1":
                return WypłataInteraktywna(atm);
            case "2":
                return false; // Zakończenie sesji
            default:
                Console.WriteLine("Niepoprawna opcja.");
                return true; // Kontynuuj pętlę
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
        return true; // Wróć do menu
    }

}