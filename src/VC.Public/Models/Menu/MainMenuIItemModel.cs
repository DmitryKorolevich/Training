using System.Collections.Generic;

namespace VC.Public.Models.Menu
{
    public class MainMenuIItemModel
    {
	    public string Label { get; set; }

	    public string Link { get; set; }

	    public IList<MainMenuIItemModel> SubItems { get; set; }

	    public MainMenuIItemModel()
	    {
			SubItems = new List<MainMenuIItemModel>();
	    }
    }
}