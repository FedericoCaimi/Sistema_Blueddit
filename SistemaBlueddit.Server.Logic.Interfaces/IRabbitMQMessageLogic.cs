using System.Threading.Tasks;

namespace SistemaBlueddit.Server.Logic.Interfaces
{
    public interface IRabbitMQMessageLogic
    {
        void DisposeConnections();

        Task<bool> SendMessageAsync(string message, string type);
    }
}