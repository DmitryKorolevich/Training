using System.Collections.Generic;
using System.Text.RegularExpressions;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Business.Queries.Products
{
    public class SkuQuery : QueryObject<Sku>
    {
        private static readonly Regex DescriptionParse = new Regex("(?<subtitle>.+)(\\((?<qty>[0-9]+)\\))?", RegexOptions.Compiled);

        public SkuQuery NotDeleted()
        {
            Add(s => s.StatusCode != (int)RecordStatusCode.Deleted);
            return this;
        }

        public SkuQuery Excluding(ICollection<int> ids)
        {
            if (ids != null && ids.Count > 0)
                Add(s => !ids.Contains(s.Id));
            return this;
        }

        public SkuQuery Including(ICollection<string> codes)
        {
            Add(s => codes.Contains(s.Code));
            return this;
        }

        public SkuQuery ByIds(ICollection<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                Add(s => ids.Contains(s.Id));
            }
            return this;
        }

        public SkuQuery ByProductIds(ICollection<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                Add(s => ids.Contains(s.IdProduct));
            }
            return this;
        }

        public SkuQuery WithProductId(int id)
        {
            Add(s => s.IdProduct == id);
            return this;
        }

        public SkuQuery WithProductIds(ICollection<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                Add(s => ids.Contains(s.IdProduct));
            }
            return this;
        }

        public SkuQuery WithCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Add(s => s.Code.StartsWith(code));
            }
            return this;
        }

        public SkuQuery WithText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Add(x => x.Code.Contains(text) || x.Product.Name.Contains(text));
            }
            return this;
        }

        public SkuQuery WithSubTitleAndQty(string subTitle, int? qty)
        {
            if (!string.IsNullOrEmpty(subTitle))
            {
                Add(x => x.Product.WhenValues(new {SubTitle = subTitle}, ValuesFilterType.And, CompareBehaviour.Equals));
                if (qty.HasValue)
                {
                    Add(x => x.WhenValues(new {QTY = qty.Value}, ValuesFilterType.And, CompareBehaviour.Equals));
                }
            }
            return this;
        }

        public SkuQuery WithExactDescriptionName(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                string subTitle = null;
                int? qty = null;
                var match = DescriptionParse.Match(description);
                if (match.Success)
                {
                    var subTitleGroup = match.Groups["subtitle"];
                    if (subTitleGroup.Success)
                    {
                        subTitle = subTitleGroup.Value;
                    }
                    var qtyGroup = match.Groups["qty"];
                    if (qtyGroup.Success)
                    {
                        int parsedQty;
                        if (int.TryParse(qtyGroup.Value, out parsedQty))
                        {
                            qty = parsedQty;
                        }
                    }
                }
                return WithSubTitleAndQty(subTitle, qty);
            }
            return this;
        }

        public SkuQuery WithExactCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Add(x => x.Code == code);
            }
            return this;
        }

        public SkuQuery WithExactCodes(ICollection<string> codes)
        {
            if (codes != null)
            {
                Add(x => codes.Contains(x.Code));
            }
            return this;
        }

        public SkuQuery WithDescriptionName(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                string subTitle = null;
                int? qty = null;
                var match = DescriptionParse.Match(description);
                if (match.Success)
                {
                    var subTitleGroup = match.Groups["subtitle"];
                    if (subTitleGroup.Success)
                    {
                        subTitle = subTitleGroup.Value;
                    }
                    var qtyGroup = match.Groups["qty"];
                    if (qtyGroup.Success)
                    {
                        int parsedQty;
                        if (int.TryParse(qtyGroup.Value, out parsedQty))
                        {
                            qty = parsedQty;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(subTitle))
                {
                    Add(x => x.Product.WhenValues(new {SubTitle = subTitle}, ValuesFilterType.And, CompareBehaviour.StartsWith));
                    if (qty.HasValue)
                    {
                        Add(x => x.WhenValues(new {QTY = qty.Value}, ValuesFilterType.And, CompareBehaviour.StartsWith));
                    }
                }
            }
            return this;
        }

        public SkuQuery WithActiveSku()
        {
            Add(x => x.StatusCode == (int)RecordStatusCode.Active);
            return this;
        }

        public SkuQuery WithActiveProduct()
        {
            Add(x => x.Product.StatusCode == (int)RecordStatusCode.Active);
            return this;
        }

        public SkuQuery WithType(ProductType? type)
        {
            if (type.HasValue)
            {
                Add(x => x.Product.IdObjectType == (int) type.Value);
            }

            return this;
        }

        public SkuQuery WithIds(ICollection<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                Add(x => ids.Contains(x.Id));
            }

            return this;
        }

        public SkuQuery WithIdProducts(ICollection<int> idProducts)
        {
            if (idProducts != null && idProducts.Count > 0)
            {
                Add(x => idProducts.Contains(x.IdProduct));
            }

            return this;
        }

        public SkuQuery WithIdProductTypes(ICollection<int> idProductTypes)
        {
            if (idProductTypes != null && idProductTypes.Count > 0)
            {
                Add(x => idProductTypes.Contains(x.Product.IdObjectType));
            }

            return this;
        }

        public SkuQuery NotHiddenOnly(bool notHidden)
        {
            if (notHidden)
            {
                Add(x => !x.Hidden && x.Product.IdVisibility != null);
            }

            return this;
        }

        public SkuQuery ActiveOnly(bool active)
        {
            if (active)
            {
                Add(x => x.Product.StatusCode == (int) RecordStatusCode.Active && x.StatusCode == (int) RecordStatusCode.Active);
            }

            return this;
        }
    }
}