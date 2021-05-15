using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SistemaBlueddit.Domain.Interface
{
    public interface ISemaphoreSlim
    {
        SemaphoreSlim SemaphoreSlim { get; }
    }
}
