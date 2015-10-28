var shippingAddresses = null;

$(function () {
	shippingAddresses = [];
	if (shippingAddressesJson != "") {
		shippingAddresses = $.parseJSON(shippingAddressesJson);
	}
	populateAddressesSelection(shippingAddresses, false);

	syncDefaultBtnState();

	$("body").on("change", "#ddShippingAddressesSelection", function () {
		changeSelection($("#ddShippingAddressesSelection").val());

		syncDefaultBtnState();

		$("#delSelected").show();
	});

	$("body").on("click", "#setDefaultSelected", function () {
		$("#hdDefault").val(true);

		$.each(shippingAddresses, function(index, item) {
			item.Default = false;
		});

		var selectedShippingAddress = $.grep(shippingAddresses, function (item) {
			return item.Id == $("#hdShipping").val();
		})[0];

		if (selectedShippingAddress) {
			setDefaultShippingAddress($("#hdShipping").val(), function (result, shippingAddresses) {
				if (result.Success) {
					selectedShippingAddress.Default = true;

					updateOptionsText();

					notifySuccess("Successfully updated");
				} else {
					notifyError(result.Messages[0].Message);
				}

			}, function (errorResult) {
				//todo: handle result
			});
		}

		$('#setDefaultSelected').hide();
	});

	$("body").on("click", "#addNew", function() {
		var newItem = {
			Id: 0,
			IdCountry: appSettings.DefaultCountryId,
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
			Email: "",
			Default: false
		}

		$("#ddShippingAddressesSelection").val("");

		$("#delSelected").hide();

		setChangedData(newItem);
	});

	$("body").on("click", "#delSelected", function() {
		var idToRemove = $("#ddShippingAddressesSelection").val();

		confirmAction(function () {
			deleteShippingAddress(idToRemove, function (result) {
				if (result.Success) {
					shippingAddresses = $.grep(shippingAddresses, function (item) {
						return item.Id != idToRemove;
					});

					populateAddressesSelection(shippingAddresses, true);

					notifySuccess("Successfully deleted");
				} else {
					notifyError(result.Messages[0].Message);
				}

			}, function(errorResult) {
				//todo: handle result
			});
		});
	});
});

function populateAddressesSelection(shippingAddresses, setDefault) {
	$("#ddShippingAddressesSelection").html("");
	if (shippingAddresses && shippingAddresses.length > 0) {
		$.each(shippingAddresses, function (shippingAddressIndex, shippingAddress) {
			var option = $('<option></option>').val(shippingAddress.Id).html(shippingAddress.FirstName + " " + shippingAddress.LastName + " " + shippingAddress.Address1 + " " + (shippingAddress.Default ? "(Default)" : ""));
			if ((setDefault && shippingAddress.Default) || (!setDefault && shippingAddress.Id == $("#hdShipping").val())) {
				$(option).attr("selected", "selected")
			}

			$("#ddShippingAddressesSelection").append(option);
		});
	}

	if ($("#hdShipping").val() == 0) {
		$("#ddShippingAddressesSelection").val("");
	}
}

function updateOptionsText() {
	$.each($("#ddShippingAddressesSelection option"), function (index, option) {
		var shippingAddress = $.grep(shippingAddresses, function (item) {
			return item.Id == $(option).val();
		})[0];

		$(option).html(shippingAddress.FirstName + " " + shippingAddress.LastName + " " + shippingAddress.Address1 + " " + (shippingAddress.Default ? "(Default)" : ""));
	});
}

function syncDefaultBtnState() {
	var selectedShippingAddress = $.grep(shippingAddresses, function (item) {
		return item.Id == $("#ddShippingAddressesSelection").val();
	})[0];

	if (!selectedShippingAddress) {
		$('#setDefaultSelected').show();
	} else {
		if (selectedShippingAddress.Default) {
			$('#setDefaultSelected').hide();
		} else {
			$('#setDefaultSelected').show();
		}
	}
}

function changeSelection(id) {
	var selectedshippingAddress = $.grep(shippingAddresses, function (item) {
		return item.Id == id;
	})[0];

	setChangedData(selectedshippingAddress);

	$("#ddCountry").trigger("change");
}

function setChangedData(selectedShippingAddress) {
	$("#hdShipping").val(selectedShippingAddress.Id);
	$("#hdCountry").val(selectedShippingAddress.IdCountry);
	$("#hdState").val(selectedShippingAddress.IdState);
	$("#hdDefault").val(selectedShippingAddress.Default);

	$("input[name=FirstName]").val(selectedShippingAddress.FirstName);
	$("input[name=LastName]").val(selectedShippingAddress.LastName);
	$("input[name=Company]").val(selectedShippingAddress.Company);
	$("#ddCountry").val(selectedShippingAddress.IdCountry);
	$("input[name=Address1]").val(selectedShippingAddress.Address1);
	$("input[name=Address2]").val(selectedShippingAddress.Address2);
	$("input[name=City]").val(selectedShippingAddress.City);
	$("#ddState").val(selectedShippingAddress.IdState);
	$("input[name=County]").val(selectedShippingAddress.County);
	$("input[name=PostalCode]").val(selectedShippingAddress.PostalCode);
	$("input[name=Phone]").val(selectedShippingAddress.Phone);
	$("input[name=Fax]").val(selectedShippingAddress.Fax);
	$("input[name=Email]").val(selectedShippingAddress.Email);

	$("form").removeData("validator");
	$("form").removeData("unobtrusiveValidation");
	$.validator.unobtrusive.parse("form");

	syncDefaultBtnState();
}

function deleteShippingAddress(idAddress, successCallback, errorCallback) {
	$.ajax({
		type: "POST",
		url: "/Profile/DeleteShippingInfo/" + idAddress,
		dataType: "json"
	}).success(function (result) {
		if (successCallback) {
			successCallback(result, shippingAddresses);
		}
	}).error(function (result) {
		if (errorCallback) {
			errorCallback(result, shippingAddresses);
		}
	});
}

function setDefaultShippingAddress(idAddress, successCallback, errorCallback) {
	$.ajax({
		type: "POST",
		url: "/Profile/SetDefaultShippingInfo/" + idAddress,
		dataType: "json"
	}).success(function (result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function (result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}