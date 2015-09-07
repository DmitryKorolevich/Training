using System;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Models
{
    public abstract class AbstractModeContainer<TMode>: IMode
    {
        public TMode Mode
        {
            get { return (TMode)(this as IMode).Mode; }
            set { (this as IMode).Mode = value; }
        }

        object IMode.Mode { get; set; }

        public static implicit operator TMode(AbstractModeContainer<TMode> modeContainerContainer)
        {
            if (modeContainerContainer == null)
                throw new ArgumentNullException(nameof(modeContainerContainer));
            return modeContainerContainer.Mode;
        }
    }
}