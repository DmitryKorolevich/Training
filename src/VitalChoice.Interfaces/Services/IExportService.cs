using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Helpers.Export;
using VitalChoice.Domain.Transfer.VitalGreen;

namespace VitalChoice.Interfaces.Services
{
    public interface IExportService<T, D> where T : IExportable
                                          where D : CsvClassMap<T>
    {
        byte[] ExportToCSV(ICollection<T> items);
    }
}
