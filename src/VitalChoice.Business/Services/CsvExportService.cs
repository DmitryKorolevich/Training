using System.Collections.Generic;
using VitalChoice.Interfaces.Services;
using CsvHelper;
using System.IO;
using CsvHelper.Configuration;

namespace VitalChoice.Business.Services
{
    public class CsvExportService<T,TMap> : ICsvExportService<T, TMap>
        where TMap : CsvClassMap<T>
    {
        public byte[] ExportToCsv(ICollection<T> items)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        using (var csv = new CsvWriter(streamWriter))
                        {
                            csv.Configuration.RegisterClassMap<TMap>();
                            csv.WriteRecords(items);
                            streamWriter.Flush();

                            memoryStream.Position = 0;
                            return System.Text.Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }
    }
}
