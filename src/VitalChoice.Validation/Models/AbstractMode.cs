using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Models
{
    public abstract class AbstractMode<TMode>: IMode
        //where TMode: struct
    {
        public TMode Mode
        {
            get { return (TMode)(this as IMode).Mode; }
            set { (this as IMode).Mode = value; }
        }

        object IMode.Mode { get; set; }
    }
}