using CsvHelper.Configuration;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Helpers.Export;

namespace VitalChoice.Interfaces.Services
{
    public interface IExportService<T, D> where T : IExportable
                                          where D : CsvClassMap<T>
    {
        byte[] ExportToCSV(ICollection<T> items);
    }
}
