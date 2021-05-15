using SistemaBlueddit.Domain;
using SistemaBlueddit.Domain.Interface;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SistemaBlueddit.Server.Logic.Interfaces
{
    public interface ILogic<T> where T : ISerializable<T>
    {
        Task RecieveAsync(Header header, NetworkStream stream, T objectToRecieve);

        void Add(T objectToAdd);

        void Clear();

        void AddMultiple(ICollection<T> objectsToAdd);

        List<T> GetAll();

        string Show(T objectToShow);

        string ShowMultiple(ICollection<T> objectsToShow);

        string ShowAll();

        T GetByName(string name);

        void Delete(T objectToDelete);

        bool Validate(T objectToValidate);
    }
}
