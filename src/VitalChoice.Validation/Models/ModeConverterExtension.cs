namespace VitalChoice.Validation.Models
{
    public static class ModeConverterExtension
    {
        public static T ToMode<TMode, T>(this TMode mode)
            where T : AbstractModeContainer<TMode>, new()
        {
            var modeContainer = new T {Mode = mode};
            return modeContainer;
        }
    }
}