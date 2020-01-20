using System;
using Ced.BusinessEntities;

namespace Ced.BusinessServices.Interfaces
{
    public interface IServiceBase
    {
        LogEntity CreateInternalLog(Exception exc, string extraInfo);
    }
}
