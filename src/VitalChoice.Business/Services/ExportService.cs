using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.VitalGreen;
using VitalChoice.Domain.Helpers.Export;
using VitalChoice.Domain.Transfer.VitalGreen;
using VitalChoice.Interfaces.Services;
using System.Reflection;
using System.Reflection.Emit;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Delegates;
using System.Text;
using CsvHelper;
using System.IO;
using CsvHelper.Configuration;

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
