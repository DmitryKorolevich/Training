
using System.ComponentModel.DataAnnotations.Schema;

namespace VitalChoice.Data.Infrastructure
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}