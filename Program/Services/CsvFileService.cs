using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Program.Models;

namespace Program.Services
{
    public class CsvFileService
    {
        public void ExportujDoCsv(List<KontoBankowe> konta, string sciezka)
        {
            // Budujemy treść pliku
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("IDKonta;Imie;Nazwisko;Saldo"); // Nagłówek

            foreach (var k in konta)
            {
                sb.AppendLine($"{k.IDKonta};{k.Imie};{k.Nazwisko};{k.Saldo}");
            }

            // Zapis na dysk
            File.WriteAllText(sciezka, sb.ToString(), Encoding.UTF8);
            Console.WriteLine($"\n[SYSTEM] Eksport zakończony sukcesem: {sciezka}");
        }
    }
}