using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VitalChoice.Interfaces.Services.VeraCore
{
    public interface IVeraCoreNotificationService
    {
        Task ProcessFiles();

        Task ProcessQueue();
    }
}
