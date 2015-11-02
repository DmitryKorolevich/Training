using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public class DirectMapper<TObject> : ObjectMapper<TObject> 
        where TObject : class, new()
    {
        protected override bool UseMapAttribute => false;

        public DirectMapper(ITypeConverter typeConverter, IModelConverterService converterService)
            : base(typeConverter, converterService)
        {
        }
    }
}