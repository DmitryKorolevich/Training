using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductIngredientsTabModel: TtlProductPageTabModel
	{
        [Map("Ingredients")]
	    public override string Content { get; set; }

        [Map("IngredientsHide")]
	    public override bool Hidden { get; set; }

        [Map("IngredientsTitleOverride")]
        public override string TitleOverride { get; set; }

	    [Map]
		public string NutritionalTitle { get; set; }

        [Map]
        public string IngredientsTitle { get; set; }

        [Map]
        public string ServingSize { get; set; }

        [Map]
        public string Servings { get; set; }

        [Map]
        public string Calories { get; set; }

        [Map]
        public string CaloriesFromFat { get; set; }

        [Map]
        public string TotalFat { get; set; }

        [Map]
        public string TotalFatPercent { get; set; }

        [Map]
        public string SaturatedFat { get; set; }

        [Map]
        public string SaturatedFatPercent { get; set; }

        [Map]
        public string TransFat { get; set; }

        [Map]
        public string TransFatPercent { get; set; }

        [Map]
        public string Cholesterol { get; set; }

        [Map]
        public string CholesterolPercent { get; set; }

        [Map]
        public string Sodium { get; set; }

        [Map]
        public string SodiumPercent { get; set; }

        [Map]
        public string TotalCarbohydrate { get; set; }

        [Map]
        public string TotalCarbohydratePercent { get; set; }

        [Map]
        public string DietaryFiber { get; set; }

        [Map]
        public string DietaryFiberPercent { get; set; }

        [Map]
        public string Sugars { get; set; }

        [Map]
        public string SugarsPercent { get; set; }

        [Map]
        public string Protein { get; set; }

        [Map]
        public string ProteinPercent { get; set; }

        [Map]
        public string AdditionalNotes { get; set; }
	}
}
