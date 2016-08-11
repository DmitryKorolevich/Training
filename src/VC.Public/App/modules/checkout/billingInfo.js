$(function () {
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

	$(".columns-container form").data("validator").settings.submitHandler = function (form)
	{
	    $(".columns-container .overlay").show();
	    setTimeout(function ()
	    {
	        form.submit();
        }, 100);
	};

	populateCardTypes();

	var cardId = $("#ddCreditCardsSelection").val() === undefined ? $("#hdCreditCard").val() : $("#ddCreditCardsSelection").val();
	checkCreditCard(cardId);
	getBrontoIsUnsubscribed();
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