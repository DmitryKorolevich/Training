$(function() {
	getCountries(function(result) {
		$.each(result.Data, function(countryIndex, country) {
			$("#ddCountry").append($('<option></option>').val(country.Id).html(country.CountryName));
		});

		var idCountry = $("#hdCountry").val();
		if (idCountry) {
			$("#ddCountry").val(idCountry);
		} else {
			$("#ddCountry").val(appSettings.DefaultCountryId);
		}

		populateStates(result.Data);

		$("#ddCountry").on("change", function () {
			populateStates(result.Data);
		});

	}, function(errorResult) {
		//todo: handle result
	});
});

function populateStates(result) {
	var selectedCountry = $.grep(result, function (country, countryIndex) {
		if ($('#ddCountry option:selected').val() == country.Id) {
			return country;
		}
	})[0];

	if (selectedCountry && selectedCountry.States && selectedCountry.States.length > 0) {
		$("#txtState").closest(".form-group").hide();
		$("#ddState").closest(".form-group").show();

		$("#txtState").val("");
		$("#ddState").html("");

		$.each(selectedCountry.States, function(stateIndex, state) {
			$("#ddState").append($('<option></option>').val(state.Id).html(state.StateName));
		});

		var idState = $("#hdState").val();
		if (idState) {
			$("#ddState").val(idState);
		}
	} else {
		$("#txtState").closest(".form-group").show();
		$("#ddState").closest(".form-group").hide();

		$("#ddState").html("");
	}
};