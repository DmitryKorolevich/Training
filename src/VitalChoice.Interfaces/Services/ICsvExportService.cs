using CsvHelper.Configuration;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Interfaces.Services
{
    public interface ICsvExportService<T, TMap>
        where TMap : CsvClassMap<T>
    {
        byte[] ExportToCsv(ICollection<T> items, bool withHeaders = true);
    }
}
