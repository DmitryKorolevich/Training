$(function ()
{
    var ignoreCanadaNotice = false;

	controlGuestCheckout();

	$("body").on("click", "#GuestCheckout", function() {
		controlGuestCheckout();
	});

	$("body").on("change", "#CardNumber", function () {
	    controlCardChange();
	});

	$("body").on("change", "#ddCreditCardsSelection", function () {
	    var cardId = $("#ddCreditCardsSelection").val();
	    changeSelection(cardId);
	    checkCreditCard(cardId);
	});

	$("body").on("click", ".canada-shipping-notice .arrow-right-blue", function ()
	{
	    $("#canada-shipping-notice").dialog('close');
	    var form = $(".columns-container form").get(0);
	    $(".columns-container .overlay").show();
	    ignoreCanadaNotice = true;
	    setTimeout(function ()
	    {
	        form.submit();
	    }, 100);
	});

	$(".columns-container form").data("validator").settings.submitHandler = function (form)
	{
	    var idCustomerType = $(".columns-container form #hdIdCustomerType").val();
	    var countryName = $(".columns-container form #ddCountry option:selected").text();
	    if (idCustomerType == 1 && countryName != "United States" && !ignoreCanadaNotice)
	    {
	        $(".columns-container .overlay").show();
	        $.ajax({
	            url: "/Checkout/CanadaShippingNoticeView",
	            dataType: "html",
	            type: "GET"
	        }).success(function (result, textStatus, xhr)
	        {
	            $(result).dialog({
	                resizable: false,
	                modal: true,
	                minWidth: 450,
	                dialogClass: "canada-shipping-notice",
	                close: function ()
	                {
	                    $(this).dialog('destroy').remove();
	                }
	            });
	        }).error(function (result)
	        {
	            notifyError();
	        }).complete(function ()
	        {
	            $(".columns-container .overlay").hide();
	        });
	    }
	    else
	    {
	        $(".columns-container .overlay").show();
	        setTimeout(function ()
	        {
	            form.submit();
	        }, 100);
	    }
	};

	populateCardTypes();

	var cardId = $("#ddCreditCardsSelection").val() === undefined ? $("#hdCreditCard").val() : $("#ddCreditCardsSelection").val();
	checkCreditCard(cardId);
	getBrontoIsUnsubscribed();

	$("input[type=submit]").prop("disabled", false);
});

function changeSelection(selId) {
	$.ajax({
		url: "/Checkout/GetBillingAddress/" + selId,
		dataType: "html"
	}).success(function (result) {
		$("#dynamicArea").html(result);

		refreshCountries();
		populateCardTypes();

		$('.tooltip-v').each(function () {
			var title = $(this).data("tooltip-title");
			var body = $(this).data("tooltip-body");
			settingsVertical.content = getBaseHtml(title, body);
			$(this).tooltipster(settingsVertical);
		});

		$(".phone-mask").mask("(999) 999-9999? x99999");

		reparseElementValidators("form");
	}).error(function (result) {
		notifyError();
	});
}

function checkCreditCard(cardId) {
    if ($("#CardNumber").val().indexOf("X") != -1 || $("#CardNumber").val().indexOf("x") != -1) {
        $.ajax({
            url: "/Checkout/CheckCreditCard/" + cardId,
            dataType: "json"
        }).success(function (result) {
            if (result && result.Data) {
                $(".validation-summary-errors>ul li:contains(Please enter all credit card)").remove();
                if ($(".validation-summary-errors>ul li").length == 0) {
                    $(".validation-summary-errors").remove();
                }
            }
            else {
                if ($(".validation-summary-errors").length > 0) {
                    $(".validation-summary-errors>ul li:contains(Please enter all credit card)").remove();
                    $(".validation-summary-errors>ul").prepend('<li>For security reasons. Please enter all credit card details for this card or please select a new one to continue.</li>');
                }
                else {
                    $("form").prepend('<div class="validation-summary-errors"><ul><li>For security reasons. Please enter all credit card details for this card or please select a new one to continue.</li></ul></div>');
                }
            }
        }).error(function (result) {
            notifyError();
        });
    }
}

function getBrontoIsUnsubscribed() {
    $.ajax({
        url: "/Checkout/GetIsUnsubscribed",
        dataType: "json"
    }).success(function (result) {
        if (result && result.Data) {
            $("#SendNews").prop("checked", false);
        }
        else {
            $("#SendNews").prop("checked", true);
        }
    }).error(function (result) {
        notifyError();
    });
}

function controlCardChange() {
    if ($("#CardNumber").val().indexOf("X") == -1 && $("#CardNumber").val().indexOf("x") == -1) {
        $(".validation-summary-errors>ul li:contains(Please enter all credit card)").remove();
        if ($(".validation-summary-errors>ul li").length == 0) {
            $(".validation-summary-errors").remove();
        }
    }
    else if ($(".validation-summary-errors>ul li:contains(Please enter all credit card)").length === 0)
    {
        var cardId = $("#ddCreditCardsSelection").val() === undefined ? $("#hdCreditCard").val() : $("#ddCreditCardsSelection").val();
        checkCreditCard(cardId);
    }
}

function controlGuestCheckout() {
	if ($("#GuestCheckout").is(":checked")) {
		$("#Password").val("password");
		$("#Password").closest(".form-group").hide();
		$("#ConfirmPassword").val("password");
		$("#ConfirmPassword").closest(".form-group").hide();
		$("#spGuestNotify").show();
		$("#spPasswordHint").hide();
	} else {
		$("#Password").val("");
		$("#Password").closest(".form-group").show();
		$("#ConfirmPassword").val("");
		$("#ConfirmPassword").closest(".form-group").show();
		$("#spGuestNotify").hide();
		$("#spPasswordHint").show();
	}
}