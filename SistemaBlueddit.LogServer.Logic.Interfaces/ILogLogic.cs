using SistemaBlueddit.Domain;
using System.Collections.Generic;

namespace SistemaBlueddit.LogServer.Logic.Interfaces
{
    public interface ILogLogic
    {
        void AddLog(Log log);

        IEnumerable<Log> GetAll(string type, string dateString);
    }
}
