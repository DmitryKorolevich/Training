using VitalChoice.Domain.Entities.eCommerce.Addresses;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class AddressDynamic : DynamicObject<Address, AddressOptionValue, AddressOptionType>
    {
        public AddressDynamic()
        {

        }

        public AddressDynamic(Address entity, bool withDefaults = false) : base(entity, withDefaults)
        {
        }

		public int IdCustomer { get; set; }

	    public int IdCountry { get; set; }

		public string County { get; set; }

		public int? IdState { get; set; }

		public AddressType AddressType { get; set; }

		protected override void FillNewEntity(Address entity)
        {
            entity.IdCustomer = IdCustomer;
            entity.IdCountry = IdCountry;
            entity.County = County;
            entity.IdState = IdState;
            entity.IdAddressType = AddressType;
        }

		protected override void UpdateEntityInternal(Address entity)
        {
            entity.IdCustomer = IdCustomer;
            entity.IdCountry = IdCountry;
            entity.County = County;
            entity.IdState = IdState;
            entity.IdAddressType = AddressType;
        }

        protected override void FromEntity(Address entity, bool withDefaults = false)
        {
			IdCustomer = entity.IdCustomer;
			IdCountry = entity.IdCountry;
			County = entity.County;
			IdState = entity.IdState;
			AddressType = entity.IdAddressType;
		}
    }
}
