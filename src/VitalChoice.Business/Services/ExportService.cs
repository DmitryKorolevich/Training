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

namespace VitalChoice.Business.Services
{
    public class ExportService<T> : IExportService<T> where T : IExportable
    {
        public ExportService()
        {
        }

        public byte[] ExportToCSV(ICollection<T> items)
        {
            byte[] toReturn = null;

            var type = typeof(T).GetTypeInfo();
            var properties = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(true).Any(a => a is ExportHeaderAttribute)).ToList();
            StringBuilder builder = new StringBuilder();
            string header = String.Empty;
            List<GenericProperty> genericProperties = new List<GenericProperty>();
            for (int i = 0; i < properties.Count; i++)
            {                
                genericProperties.Add(new GenericProperty()
                {
                    Get = properties[i].GetMethod?.CompileAccessor<object, object>(),
                    PropertyType = properties[i].PropertyType
                });
                var exportHeader = (ExportHeaderAttribute)properties[i].GetCustomAttributes(true).FirstOrDefault(p => p is ExportHeaderAttribute);
                builder.Append(exportHeader.HeaderText + (i!=properties.Count-1 ? "," : ""));
            }
            builder.AppendLine();

            foreach (var item in items)
            {
                for (int i = 0; i < genericProperties.Count; i++)
                {
                    string value = genericProperties[i].Get?.Invoke(item).ToString();
                    builder.Append(value + (i != properties.Count - 1 ? "," : ""));
                }
                builder.AppendLine();
            }

            return System.Text.Encoding.UTF8.GetBytes(builder.ToString());
        }
    }
}
