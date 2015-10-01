using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Helpers.Export;
using VitalChoice.Domain.Transfer.VitalGreen;

namespace VitalChoice.Interfaces.Services
{
    public interface IExportService<T> where T : IExportable
    {
        byte[] ExportToCSV(ICollection<T> items);
    }
}
