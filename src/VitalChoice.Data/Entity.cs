
using VitalChoice.Data.Infrastructure;

namespace VitalChoice.Data
{
    public abstract class Entity : IObjectState
    {
		public int Id { get; set; }

        public ObjectState ObjectState { get; set; } 
    }
}