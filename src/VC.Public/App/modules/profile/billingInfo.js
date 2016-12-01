var creditCards = null;

$(function () {
	changeSaveButtonLabel($("#hdCreditCard").val());

	creditCards = [];
	if (creditCardsJson) {
		creditCards = creditCardsJson;
	} else {
		$("#delSelected").hide();
	}

	populateCardTypes(function() {
		populateCreditCardsSelection(creditCards, false);

		syncDefaultBtnState();
	});

	$("body").on("change", "#ddCreditCardsSelection", function() {
		changeSelection($("#ddCreditCardsSelection").val());

		syncDefaultBtnState();

		$("#delSelected").show();

		$(".validation-summary-errors").hide();
	});

	$("body").on("click", "#setDefaultSelected", function () {
		$("#hdDefault").val(true);

		$.each(creditCards, function (index, item) {
			item.Default = false;
		});

		var selectedCreditCard = $.grep(creditCards, function (item) {
			return item.Id == $("#hdCreditCard").val();
		})[0];

		if (selectedCreditCard) {
			setDefaultCreditCard($("#hdCreditCard").val(), function (result, creditCards) {
				if (result.Success) {
					selectedCreditCard.Default = true;

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
			CardType: 1,
			IdCountry: appSettings.DefaultCountryId,
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
			Fax: "",
			Default:false
		}

		$("#ddCreditCardsSelection").val("");

		$("#delSelected").hide();

		setChangedData(newItem);

		$("#ddCountry").trigger("change");
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
					} else if ($("#ddCreditCardsSelection").val() != "") {
						changeSelection($("#ddCreditCardsSelection").val());
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

function populateCreditCardsSelection(creditCards, setDefault) {
	$("#ddCreditCardsSelection").html("");

	if (creditCards && creditCards.length > 0) {
	    $.each(creditCards, function (creditCardIndex, creditCard)
	    {
	        var data = "";
	        var cards = $.grep(creditCardTypes, function (cardType) { return cardType.Key == creditCard.CardType });
	        if (cards.length > 0)
	        {
	            data += cards[0].Text;
	        }
	        data += ', ending in ' + getLast4(creditCard.CardNumber) + ' ' + (creditCard.Default ? '(Default)' : '');
	        var option = $('<option></option>').val(creditCard.Id).html(data);
			if ((setDefault && creditCard.Default) || (!setDefault && creditCard.Id == $("#hdCreditCard").val())) {
				$(option).attr("selected", "selected")
			}

			$("#ddCreditCardsSelection").append(option);
		});
	}

	if ($("#hdCreditCard").val() == 0) {
		$("#ddCreditCardsSelection").val("");
	}
}

function syncDefaultBtnState() {
	var selectedCreditCard = $.grep(creditCards, function (item) {
		return item.Id == $("#ddCreditCardsSelection").val();
	})[0];

	if (!selectedCreditCard) {
		$('#setDefaultSelected').show();
	} else {
		if (selectedCreditCard.Default) {
			$('#setDefaultSelected').hide();
		} else {
			$('#setDefaultSelected').show();
		}
	}
}

function changeSelection(id) {
	var selectedCreditCard = $.grep(creditCards, function(item) {
		return item.Id == id;
	})[0];

	setChangedData(selectedCreditCard);

	$("#ddCountry").trigger("change");
}

function changeSaveButtonLabel(id) {
	if (id == 0) {
		$("input[type=submit]").val("Save");
	} else {
		$("input[type=submit]").val("Update");
	}
}

function deleteCreditCard(idCreditCard, successCallback, errorCallback)
{
    $("body .right-pane .overlay").show();
	$.ajax({
		type: "POST",
		url: "/Profile/DeleteBillingInfo/" + idCreditCard,
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

function setChangedData(selectedCreditCard) {
	changeSaveButtonLabel(selectedCreditCard.Id);

	$("#hdCreditCard").val(selectedCreditCard.Id);
	$("#hdCardType").val(selectedCreditCard.CardType);
	$("#hdCountry").val(selectedCreditCard.IdCountry);
	$("#hdState").val(selectedCreditCard.IdState);
	$("#hdDefault").val(selectedCreditCard.Default);

	$("#ddCardType").val(selectedCreditCard.CardType);
	$("input[name=CardNumber]").val(selectedCreditCard.CardNumber);
	$("input[name=SecurityCode]").val('');
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

function setDefaultCreditCard(idAddress, successCallback, errorCallback)
{
    $("body .right-pane .overlay").show();
	$.ajax({
		type: "POST",
		url: "/Profile/SetDefaultCreditCard/" + idAddress,
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

function updateOptionsText() {
	$.each($("#ddCreditCardsSelection option"), function (index, option) {
		var creditCard = $.grep(creditCards, function (item) {
			return item.Id == $(option).val();
		})[0];

		var text = $.grep(creditCardTypes, function(cardType) {
			 return cardType.Key == creditCard.CardType
		})[0].Text + ', ending in ' + getLast4(creditCard.CardNumber);

		$(option).html(text + " " + (creditCard.Default ? "(Default)" : ""));
	});
}