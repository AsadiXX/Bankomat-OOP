using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Models
{
    /// <summary>
    /// Klasa bazowa dla wszystkich niestandardowych wyjątków w aplikacji Bankomat.
    /// Ułatwia łapanie wszystkich błędów związanych z logiką bankomatu w jednym bloku catch.
    /// </summary>
    public abstract class BankomatException : Exception
    {
        public BankomatException(string message) : base(message) { }
    }

    // --- Wyjątki związane z BAZĄ DANYCH / KONTEM (Usługi) ---

    /// <summary>
    /// Rzucany, gdy próba pobrania konta z bazy danych kończy się niepowodzeniem.
    /// </summary>
    public class KontoNotFoundException : BankomatException
    {
        public KontoNotFoundException(int idKonta)
            : base($"Błąd DB: Nie znaleziono konta o ID {idKonta}.") { }
    }

    /// <summary>
    /// Rzucany, gdy saldo konta jest niewystarczające do wykonania transakcji.
    /// (Wymaganie 7: Obsługa wyjątków)
    /// </summary>
    public class InsufficientFundsException : BankomatException
    {
        public InsufficientFundsException(decimal saldo, decimal kwota)
            : base($"Błąd transakcji: Saldo ({saldo:C}) jest niewystarczające do wypłaty kwoty {kwota:C}.") { }
    }

    // --- Wyjątki związane z WALIDACJĄ DANYCH ---

    /// <summary>
    /// Rzucany, gdy format PIN-u wprowadzany przez użytkownika (lub w konstruktorze karty) jest niepoprawny.
    /// </summary>
    public class InvalidPinFormatException : BankomatException
    {
        public InvalidPinFormatException(string message) : base(message) { }
    }

    /// <summary>
    /// Rzucany, gdy kwota wypłaty narusza zasady bankomatu (np. nie jest wielokrotnością 10 PLN).
    /// </summary>
    public class InvalidWithdrawalAmountException : BankomatException
    {
        public InvalidWithdrawalAmountException(string message) : base(message) { }
    }
}