using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VC.Public.Models
{
    public class ContentPageViewModel
    {
        public string HTML { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public ContentPageViewModel(ContentViewModel data)
        {
            HTML = data.Body;
            Title = data.Title;
            MetaKeywords = data.MetaKeywords;
            MetaDescription = data.MetaDescription;
        }
    }
}