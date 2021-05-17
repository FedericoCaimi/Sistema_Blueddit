using System.Threading;

namespace SistemaBlueddit.Domain.Interface
{
    public interface ISemaphoreSlim
    {
        SemaphoreSlim SemaphoreSlim { get; }
    }
}
