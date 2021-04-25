using SistemaBlueddit.Domain;

namespace SistemaBlueddit.Server.Logic
{
    public class UserLogic: Logic<User>
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
