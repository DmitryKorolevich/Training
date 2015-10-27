﻿var creditCards = null;

$(function () {
	getCreditCardTypes(function (result) {
		$.each(result.Data, function (creditTypeIndex, creditType) {
			$("#ddCardType").append($('<option></option>').val(creditType.Key).html(creditType.Text));
		});

		var idCreditType = $("#hdCardType").val();
		if (idCreditType) {
			$("#ddCardType").val(idCreditType);
		}
	}, function(errorResult) {
		//todo: handle result
	});

	creditCards = [];
	if (creditCardsJson != "") {
		creditCards = $.parseJSON(creditCardsJson);
	} else {
		$("#delSelected").hide();
	}
	populateCreditCardsSelection(creditCards, false);

	$("body").on("change", "#ddCreditCardsSelection", function() {
		changeSelection($("#ddCreditCardsSelection").val());

		$("#delSelected").show();
	});

	$("body").on("click", "#addNew", function() {
		var newItem = {
			Id: 0,
			CardType: 1,
			IdCountry: null,
			IdState: null,
			CardNumber: "",
			NameOnCard: "",
			ExpirationDateMonth: null,
			ExpirationDateYear: null,
			FirstName: "",
			LastName: "",
			Company: "",
			Address1: "",
			Address2: "",
			City: "",
			County: "",
			PostalCode: "",
			Phone: "",
			Fax: ""
		}

		$("#ddCreditCardsSelection").val("");

		$("#delSelected").hide();

		setChangedData(newItem);
	});

	$("body").on("click", "#delSelected", function() {
		var idToRemove = $("#ddCreditCardsSelection").val();

		confirmAction(function() {
			deleteCreditCard(idToRemove, function (result) {
				if (result.Success) {
					creditCards = $.grep(creditCards, function (item) {
						return item.Id != idToRemove;
					});

					populateCreditCardsSelection(creditCards, true);

					if (!creditCards || creditCards.length < 1) {
						$("#addNew").trigger("click");
					}

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

function populateCreditCardsSelection(creditCards, setDefault) {
	$("#ddCreditCardsSelection").html("");

	if (creditCards && creditCards.length > 0) {
		$.each(creditCards, function (creditCardIndex, creditCard) {
			var option = $('<option></option>').val(creditCard.Id).html(creditCard.CardNumber);
			if ((setDefault && creditCardIndex == 0) || (!setDefault && creditCard.Id == $("#hdCreditCard").val())) {
				$(option).attr("selected", "selected")
			}

			$("#ddCreditCardsSelection").append(option);
		});
	}

	if ($("#hdCreditCard").val() == 0) {
		$("#ddCreditCardsSelection").val("");
	}
}

function changeSelection(id) {
	var selectedCreditCard = $.grep(creditCards, function(item) {
		return item.Id == id;
	})[0];

	setChangedData(selectedCreditCard);

	$("#ddCountry").trigger("change");
}

function setChangedData(selectedCreditCard) {
	$("#hdCreditCard").val(selectedCreditCard.Id);
	$("#hdCardType").val(selectedCreditCard.CardType);
	$("#hdCountry").val(selectedCreditCard.IdCountry);
	$("#hdState").val(selectedCreditCard.IdState);

	$("#ddCardType").val(selectedCreditCard.CardType);
	$("input[name=CardNumber]").val(selectedCreditCard.CardNumber);
	$("input[name=NameOnCard]").val(selectedCreditCard.NameOnCard);
	$("input[name=ExpirationDateMonth]").val(selectedCreditCard.ExpirationDateMonth);
	$("input[name=ExpirationDateYear]").val(selectedCreditCard.ExpirationDateYear);

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

	$("form").removeData("validator");
	$("form").removeData("unobtrusiveValidation");
	$.validator.unobtrusive.parse("form");
}

function deleteCreditCard(idCreditCard, successCallback, errorCallback) {
	$.ajax({
		type: "POST",
		url: "/Profile/DeleteBillingInfo/" + idCreditCard,
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