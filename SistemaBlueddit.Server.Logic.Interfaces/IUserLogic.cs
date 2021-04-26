using SistemaBlueddit.Domain;

namespace SistemaBlueddit.Server.Logic.Interfaces
{
    public interface IUserLogic: ILogic<User>
    {
        void CloseAll();
    }
}
