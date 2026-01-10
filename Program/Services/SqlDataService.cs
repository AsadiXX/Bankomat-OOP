using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Program.Models;

namespace Program.Services
{
    public class SqlDataService : IDataService
    {
        // Connection string do Twojego serwera SQL Express
        private readonly string _connectionString = @"Server=.\SQLEXPRESS04;Database=SystemBankowy;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";
        public KontoBankowe PobierzKontoPoID(int idKonta)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT Konta.IDKonta, Klienci.Imie, Klienci.Nazwisko, Konta.Saldo, Karty.PINHash
                    FROM Konta
                    JOIN Klienci ON Konta.IDKlienta = Klienci.IDKlienta
                    JOIN Karty ON Konta.IDKonta = Karty.IDKonta
                    WHERE Konta.IDKonta = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idKonta);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new KontoBankowe(
                                reader.GetInt32(0), // IDKonta
                                reader.GetString(1), // Imie
                                reader.GetString(2), // Nazwisko
                                reader.GetDecimal(3), // Saldo
                                reader.GetString(4)  // PINHash
                            );
                        }
                    }
                }
            }
            throw new Exception($"Nie znaleziono konta o ID: {idKonta}");
        }

        // 2. AKTUALIZACJA SALDA (UPDATE)
        public bool AktualizujSaldo(int idKonta, decimal nowaKwota)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE Konta SET Saldo = @saldo WHERE IDKonta = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@saldo", nowaKwota);
                    command.Parameters.AddWithValue("@id", idKonta);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // 3. POBIERANIE WSZYSTKICH KONT (DLA EKSPORTU CSV)
        public List<KontoBankowe> PobierzWszystkieKonta()
        {
            List<KontoBankowe> lista = new List<KontoBankowe>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT Konta.IDKonta, Klienci.Imie, Klienci.Nazwisko, Konta.Saldo, Karty.PINHash 
                    FROM Konta
                    JOIN Klienci ON Konta.IDKlienta = Klienci.IDKlienta
                    JOIN Karty ON Konta.IDKonta = Karty.IDKonta";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new KontoBankowe(
                                reader.GetInt32(0), // IDKonta
                                reader.GetString(1), // Imie
                                reader.GetString(2), // Nazwisko
                                reader.GetDecimal(3), // Saldo
                                reader.GetString(4)  // PINHash
                            ));
                        }
                    }
                }
            }
            return lista;
        }

        // Metody walidacyjne (używają PobierzKontoPoID)
        public bool WalidujPin(int idKonta, string pin)
        {
            var konto = PobierzKontoPoID(idKonta);
            return konto.PINHash == pin;
        }

        public bool CzyWystarczajaceSaldo(int idKonta, decimal kwota)
        {
            var konto = PobierzKontoPoID(idKonta);
            return konto.Saldo >= kwota;
        }
    }
}