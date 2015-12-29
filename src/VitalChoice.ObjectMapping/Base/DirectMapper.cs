using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.ObjectMapping.Base
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