﻿var shippingAddresses = null;
var preferredOptions = [];

$(function () {
	getShippingPreferredOptions(function(res) {
		if (res.Success) {
			preferredOptions = res.Data;
		} else {
			notifyError(res.Messages[0].Message);
		}
	}, function() {
		notifyError();
	});

	changeSaveButtonLabel($("#hdShipping").val());

	shippingAddresses = [];
	if (shippingAddressesJson) {
		shippingAddresses = shippingAddressesJson;
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
				notifyError();
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
			Default: false,
			AddressType: 1,
			DeliveryInstructions: "",
			PreferredShipMethod: 1
		}

		$("#ddShippingAddressesSelection").val("");

		$("#delSelected").hide();

		setChangedData(newItem);

		$("#ddCountry").trigger("change");
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
					if ($("#ddShippingAddressesSelection").val() != "") {
						changeSelection($("#ddShippingAddressesSelection").val());
					}

					notifySuccess("Successfully deleted");
				} else {
					notifyError(result.Messages[0].Message);
				}

			}, function(errorResult) {
				notifyError();
			});
		});
	});

	$("body .right-pane form").data("validator").settings.submitHandler = function (form)
	{
	    $("body .right-pane .overlay").show();
	    setTimeout(function ()
	    {
	        form.submit();
	    }, 100);
	};
});

function populateAddressesSelection(shippingAddresses, setDefault) {
	$("#ddShippingAddressesSelection").html("");
	if (shippingAddresses && shippingAddresses.length > 0) {
	    $.each(shippingAddresses, function (shippingAddressIndex, shippingAddress)
	    {
	        var data = (shippingAddress.FirstName ? shippingAddress.FirstName : "") + " " +
                (shippingAddress.LastName ? shippingAddress.LastName : "") + " " +
                (shippingAddress.Address1 ? shippingAddress.Address1 : "") + " " +
                (shippingAddress.Default ? "(Default)" : "");
			var option = $('<option></option>').val(shippingAddress.Id).html(data);
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

function changeSaveButtonLabel(id) {
	if (id == 0) {
		$("input[type=submit]").val("Save");
	} else {
		$("input[type=submit]").val("Update");
	}
}

function setChangedData(selectedShippingAddress) {
	changeSaveButtonLabel(selectedShippingAddress.Id);

	$("#hdShipping").val(selectedShippingAddress.Id);
	$("#hdCountry").val(selectedShippingAddress.IdCountry);
	$("#hdState").val(selectedShippingAddress.IdState);
	$("#hdDefault").val(selectedShippingAddress.Default);
	$("#hdPreferredShipMethod").val(selectedShippingAddress.PreferredShipMethod);

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

	if (selectedShippingAddress.AddressType == 1) {
		$("input[name=AddressType][value=Residential]").prop("checked", true).trigger('change');;
	} else if (selectedShippingAddress.AddressType == 2) {
		$("input[name=AddressType][value=Commercial]").prop("checked", true).trigger('change');;
	}

	$("#PreferredShipMethod").text($.grep(preferredOptions, function (elem) { return elem.Key == selectedShippingAddress.PreferredShipMethod; })[0].Text);
	$("textarea[name=DeliveryInstructions]").val(selectedShippingAddress.DeliveryInstructions);

	$("form").removeData("validator");
	$("form").removeData("unobtrusiveValidation");
	$.validator.unobtrusive.parse("form");

	syncDefaultBtnState();

	processCharcount({ target: $("textarea[name=DeliveryInstructions]") });
}

function deleteShippingAddress(idAddress, successCallback, errorCallback)
{
    $("body .right-pane .overlay").show();
	$.ajax({
		type: "POST",
		url: "/Profile/DeleteShippingInfo/" + idAddress,
		dataType: "json"
	}).success(function (result)
	{
	    $("body .right-pane .overlay").hide();
		if (successCallback) {
			successCallback(result, shippingAddresses);
		}
	}).error(function (result)
	{
	    $("body .right-pane .overlay").hide();
		if (errorCallback) {
			errorCallback(result, shippingAddresses);
		}
	});
}

function setDefaultShippingAddress(idAddress, successCallback, errorCallback)
{
    $("body .right-pane .overlay").show();
	$.ajax({
		type: "POST",
		url: "/Profile/SetDefaultShippingInfo/" + idAddress,
		dataType: "json"
	}).success(function (result)
	{
	    $("body .right-pane .overlay").hide();
		if (successCallback) {
			successCallback(result);
		}
	}).error(function (result)
	{
	    $("body .right-pane .overlay").hide();
		if (errorCallback) {
			errorCallback(result);
		}
	});
}