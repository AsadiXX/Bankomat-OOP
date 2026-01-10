using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program.Models;

namespace Program.Services
{
    public interface IDataService
    {
        // CRUD: Read
        KontoBankowe PobierzKontoPoID(int idKonta);
        List<KontoBankowe> PobierzWszystkieKonta();
        // CRUD: Update (Wypłata)
        bool AktualizujSaldo(int idKonta, decimal nowaKwota);

        // Walidacja
        bool WalidujPin(int idKonta, string pin);
        bool CzyWystarczajaceSaldo(int idKonta, decimal kwota);
    }
}