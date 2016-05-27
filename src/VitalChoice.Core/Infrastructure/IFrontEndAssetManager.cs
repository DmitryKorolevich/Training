using VitalChoice.Core.Infrastructure.Models;

namespace VitalChoice.Core.Infrastructure
{
    public interface IFrontEndAssetManager
    {
        AssetInfo GetOrderInvoiceStyles();
        AssetInfo GetScripts();
        AssetInfo GetStyles();
    }
}