var shippingAddresses = null;

$(function () {
	shippingAddresses = $.parseJSON(shippingAddressesJson);
	if (shippingAddresses && shippingAddresses.length > 0) {
		$.each(shippingAddresses, function (shippingAddressIndex, shippingAddress) {
			$("#ddShippingAddressesSelection").append($('<option></option>').val(shippingAddress.Id).html(shippingAddress.FirstName + " " + shippingAddress.LastName + " " + shippingAddress.Address1 + " " + (shippingAddress.Default ? "(Default)" : "")));
		});
	}

	$("body").on("change", "#ddShippingAddressesSelection", function () {
		changeSelection($("#ddShippingAddressesSelection").val());
	});

	$("body").on("click", "#addNew", function() {
		var newItem = {
			Id: 0,
			IdCountry: null,
			IdState: null,
			FirstName: "",
			LastName: "",
			Company: "",
			Address1: "",
			Address2: "",
			City: "",
			County: "",
			PostalCode: "",
			Phone: "",
			Fax: "",
			Email: ""
		}

		setChangedData(newItem);
	});

	$("body").on("click", "#delSelected", function() {
		var idToRemove = $("#ddShippingAddressesSelection").val();

		deleteShippingAddress(idToRemove, function (result) {
			$("ddShippingAddressesSelection option[value=" + idToRemove + "]").remove();
		}, function (errorResult) {
			//todo: handle result
		});
	});
});

function changeSelection(id) {
	var selectedCreditCard = $.grep(creditCards, function(item) {
		return item.Id == id;
	})[0];

	setChangedData(selectedCreditCard);
}

function setChangedData(selectedCreditCard) {
	$("#hdCountry").val(selectedCreditCard.IdCountry);
	$("#hdState").val(selectedCreditCard.IdState);

	$("input[name=FirstName]").val(selectedCreditCard.FirstName);
	$("input[name=LastName]").val(selectedCreditCard.LastName);
	$("input[name=Company]").val(selectedCreditCard.Company);
	$("#ddCountry").val(selectedCreditCard.IdCountry);
	$("input[name=Address1]").val(selectedCreditCard.Address1);
	$("input[name=Address2]").val(selectedCreditCard.Address2);
	$("input[name=City]").val(selectedCreditCard.City);
	$("#ddState").val(selectedCreditCard.IdState);
	$("input[name=County]").val(selectedCreditCard.County);
	$("input[name=PostalCode]").val(selectedCreditCard.PostalCode);
	$("input[name=Phone]").val(selectedCreditCard.Phone);
	$("input[name=Fax]").val(selectedCreditCard.Fax);
	$("input[name=Email]").val(selectedCreditCard.Email);

	$("form").removeData("validator");
	$("form").removeData("unobtrusiveValidation");
	$.validator.unobtrusive.parse("form");
}