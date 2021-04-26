using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic.Interfaces;

namespace SistemaBlueddit.Server.Logic
{
    public class UserLogic: Logic<User>, IUserLogic
    {
        public void CloseAll()
        {
            foreach (var user in _elements)
            {
                user.TcpClient.GetStream().Close();
                user.TcpClient.Close();
            }
        }
    }
}
