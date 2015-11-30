using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductIngredientsTabModel: TtlProductPageTabModel
	{
		public string NutritionalTitle { get; set; }

		public string ServingSize { get; set; }

		public string Servings { get; set; }

		public string Calories { get; set; }

		public string CaloriesFromFat { get; set; }

		public string TotalFat { get; set; }

		public string TotalFatPercent { get; set; }

		public string SaturatedFat { get; set; }

		public string SaturatedFatPercent { get; set; }

		public string TransFat { get; set; }

		public string TransFatPercent { get; set; }

		public string Cholesterol { get; set; }

		public string CholesterolPercent { get; set; }

		public string Sodium { get; set; }

		public string SodiumPercent { get; set; }

		public string TotalCarbohydrate { get; set; }

		public string TotalCarbohydratePercent { get; set; }

		public string DietaryFiber { get; set; }

		public string DietaryFiberPercent { get; set; }

		public string Sugars { get; set; }

		public string SugarsPercent { get; set; }

		public string Protein { get; set; }

		public string ProteinPercent { get; set; }

		public string AdditionalNotes { get; set; }
	}
}
