using System;
using SistemaBlueddit.Domain;
using System.Collections.Generic;

namespace SistemaBlueddit.LogServer.Logic.Interfaces
{
    public interface IPostLogLogic
    {
        IEnumerable<Log> GetAll();
    }
}
