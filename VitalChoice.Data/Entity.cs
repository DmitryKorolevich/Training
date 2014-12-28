using System.ComponentModel.DataAnnotations.Schema;
using VitalChoice.Data.Infrastructure;

namespace VitalChoice.Data
{
    public abstract class Entity : IObjectState
    {
		public int Id { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; } 
    }
}