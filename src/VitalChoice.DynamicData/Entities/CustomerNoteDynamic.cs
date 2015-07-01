using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class CustomerNoteDynamic : DynamicObject<CustomerNote, CustomerNoteOptionValue, CustomerNoteOptionType>
    {
        public CustomerNoteDynamic()
        {

        }

        public CustomerNoteDynamic(CustomerNote entity, bool withDefaults = false) : base(entity, withDefaults)
        {
        }

		public int IdCustomer { get; set; }

		public string Note { get; set; }

		protected override void FillNewEntity(CustomerNote entity)
        {
            entity.IdCustomer = IdCustomer;
            entity.Note = Note;
        }

        protected override void UpdateEntityInternal(CustomerNote entity)
        {
			entity.IdCustomer = IdCustomer;
			entity.Note = Note;

			foreach (var value in entity.OptionValues)
            {
                value.Id = Id;
            }

        }

        protected override void FromEntity(CustomerNote entity, bool withDefaults = false)
        {
			entity.IdCustomer = IdCustomer;
			entity.Note = Note;
		}
    }
}
