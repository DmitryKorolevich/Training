using System.Collections.Generic;
using VitalChoice.Interfaces.Services;
using CsvHelper;
using System.IO;
using CsvHelper.Configuration;
using VitalChoice.Ecommerce.Domain.Helpers.Export;

namespace VitalChoice.Business.Services
{
    public class ExportService<T,D> : IExportService<T, D> where T : IExportable
                                                           where D : CsvClassMap<T>
    {
        public ExportService()
        {
        }

        public byte[] ExportToCSV(ICollection<T> items)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        using (var csv = new CsvWriter(streamWriter))
                        {
                            csv.Configuration.RegisterClassMap<D>();
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
