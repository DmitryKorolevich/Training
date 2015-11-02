namespace VitalChoice.ContentProcessing.Base
{
    public class ContentFilterModel
    {
        public string Url { get; set; }
    }


    public class ContentFilterModel<T>: ContentFilterModel
    {
        public T Model { get; set; }
    }
}