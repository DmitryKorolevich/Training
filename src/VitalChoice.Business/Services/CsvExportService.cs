using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using VitalChoice.Interfaces.Services;
using CsvHelper;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Data.Extensions;

namespace VitalChoice.Business.Services
{
    public class CsvExportService
    {
        public static byte[] ExportToCsv(ICollection<DynamicExportColumn> dynamicColumns, ICollection<ExpandoObject> items, bool withHeaders = true)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        using (var csv = new CsvWriter(streamWriter))
                        {
                            dynamicColumns.ForEach(item => csv.WriteField(item.DisplayName));

                            csv.NextRecord();

                            foreach (var item in items)
                            {
                                var data = (IDictionary<string, object>) item;
                                foreach (var dynamicExportColumn in dynamicColumns)
                                {
                                    var value = data[dynamicExportColumn.Name];
                                    csv.WriteField(value ?? string.Empty);
                                }
                                csv.NextRecord();
                            }

                            streamWriter.Flush();

                            memoryStream.Position = 0;
                            return System.Text.Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }
    }

    public class CsvExportService<T,TMap> : ICsvExportService<T, TMap>
        where TMap : CsvClassMap<T>
    {
        public byte[] ExportToCsv(ICollection<T> items, bool withHeaders=true)
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
                            csv.Configuration.HasHeaderRecord = withHeaders;
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
