namespace VitalChoice.ContentProcessing.Base
{
    public class ProcessorModel
    {
        public string Url { get; set; }
    }


    public class ProcessorModel<T>: ProcessorModel
    {
        public T Model { get; set; }
    }
}