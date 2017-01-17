$(function ()
{
    $(".shipping-items").on("click", ".delete-shipping", function ()
    {
        $(this).closest('.item').remove();
        updateItemsUI();
    });

    $(".new-shipping").click(function ()
    {
        $(".main-shipping-item-wrapper .minus").click();
        $(".shipping-items .minus").click();

        var item = $(".item.template").clone();
        item.removeClass('hide');
        item.removeClass('template');
        var content = $(".main-shipping-item").contents().clone();
        item.find(".item-content").append(content);
        $(".shipping-items").append("<div class='clear'></div>");
        $(".shipping-items").append(item);
        item.find('.use-billing-section').remove();

        updateItemsUI();
    });

    var updateItemsUI = function ()
    {
        if ($(".item").length > 1)
        {
            $(".new-shipping").text('Add Another Recipient');
            $(".main-shipping-item-wrapper .order-header").removeClass('hide');
        }
        else
        {
            $(".new-shipping").text('Send Order To Multiple Receipents');
            $(".main-shipping-item-wrapper .order-header").addClass('hide');
            $('.main-shipping-item-wrapper .main-shipping-item').removeClass('hide-imp');
        }

        $('.main-shipping-item-wrapper').removeClass('alternate-color');
        $('.item').removeClass('alternate-color');
        var number = 2;
        $.each($(".item"), function (i, item)
        {
            if (!$(item).hasClass('template'))
            {
                $(item).find('.checkout-step-heading span span').text('Order #'+number);
                number++;
                if (i % 2 != 0)
                {
                    $(item).addClass('alternate-color');
                }
            }
        });
        if($(".item").length>1)
        {
            $('.main-shipping-item-wrapper').addClass('alternate-color');
        }
    };

    $(".shipping-items").on("click", ".item .plus", function ()
    {
        $(".main-shipping-item-wrapper .minus").click();
        $(".shipping-items .minus").click();
        $(this).closest('.item').find('.item-content').removeClass('hide-imp');
        $(this).addClass('hide-imp');
        $(this).closest('.item').find('.minus').removeClass('hide-imp');
    });

    $(".shipping-items").on("click", ".item .minus", function ()
    {
        $(this).closest('.item').find('.item-content').addClass('hide-imp');
        $(this).addClass('hide-imp');
        $(this).closest('.item').find('.plus').removeClass('hide-imp');
    });

    $(".main-shipping-item-wrapper").on("click", ".plus", function ()
    {
        $(".shipping-items .minus").click();
        $(this).closest('.main-shipping-item-wrapper').find('.main-shipping-item').removeClass('hide-imp');
        $(this).addClass('hide-imp');
        $(this).closest('.main-shipping-item-wrapper').find('.minus').removeClass('hide-imp');
    });

    $(".main-shipping-item-wrapper").on("click", ".minus", function ()
    {
        $(this).closest('.main-shipping-item-wrapper').find('.main-shipping-item').addClass('hide-imp');
        $(this).addClass('hide-imp');
        $(this).closest('.main-shipping-item-wrapper').find('.plus').removeClass('hide-imp');
    });

    controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther");
    controlSectionState("#GiftMessageBox", "#IsGiftOrder");
    controlUseBillingState(".form-two-column", "#UseBillingAddress");

    $("body")
        .on("click",
            "#chkSelectOther",
            function () { controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther"); });
    $("body")
        .on("click",
            "#IsGiftOrder",
            function () {
                controlSectionState("#GiftMessageBox", "#IsGiftOrder");
                controlSectionContent("#GiftMessage", "#IsGiftOrder");
            });
    $("body")
        .on("click",
            "#UseBillingAddress",
            function () { controlUseBillingState(".form-two-column", "#UseBillingAddress"); });

    $("body")
        .on("change",
            "#ddShippingAddressesSelection",
            function () {
                changeSelection($("#ddShippingAddressesSelection").val());
            });

    $(".columns-container form").data("validator").settings.submitHandler = function (form)
    {
        $(".columns-container .overlay").show();
        form.submit();
    };

    controlUpdateSavedState();

    var param = getQueryParameterByName("canadaissue");
    if (param)
    {
        $(".columns-container .overlay").show();
        $.ajax({
            url: "/Checkout/CanadaShippingIssueView",
            dataType: "html",
            type: "GET"
        }).success(function (result, textStatus, xhr)
        {
            $(result).dialog({
                resizable: false,
                modal: true,
                minWidth: 450,
                dialogClass: "canada-shipping-notice",
                open: function(event,ui) {
                    $(this).parent().focus();
                },
                close: function ()
                {
                    $(this).dialog('destroy').remove();
                },
                buttons: [
                    {
                        text: "Ok",
                        click: function ()
                        {
                            $(this).dialog("close");
                        }
                    }
                ]
            });
        }).error(function (result)
        {
            notifyError();
        }).complete(function ()
        {
            $(".columns-container .overlay").hide();
        });
    }

    $(".columns-container .overlay").hide();
});

function controlUpdateSavedState() {
    if ($("#ShipAddressIdToOverride").val() == null || $("#ShipAddressIdToOverride").val() == "") {
        $("#SaveToProfile").prop("checked", false);
        $("#SaveToProfile").trigger("change");
        $("#updateSaved").hide();
    } else {
        $("#updateSaved").show();
    }
}

function changeSelection(selId) {
    $.ajax({
        url: "/Checkout/GetShippingAddress/" + selId,
        dataType: "html"
    }).success(function (result) {
        $("#dynamicArea").html(result);

        refreshCountries();

        controlUpdateSavedState();

        $(".phone-mask").mask("(999) 999-9999? x99999");

        reparseElementValidators("form");
    }).error(function (result) {
        notifyError();
    });
}

function controlSectionContent(selector, controlId) {
    var jSel = $(selector);

    if (!$(controlId).is(":checked")) {
        jSel.val("");
        jSel.trigger("change");
        jSel.trigger("keyup");
    }
}

function controlSectionState(selector, controlId) {
    var jSel = $(selector).closest(".form-group");

    if ($(controlId).is(":checked")) {
        jSel.show();
    } else {
        jSel.hide();
    }
}

var lastPrefShipMethodLabel = null;

function controlUseBillingState(selector, controlId) {
    var jSel = $(selector);
    var jDrop = $("#ddShippingAddressesSelection").closest(".form-group");

    var jChk = $("#SaveToProfile");
    var jChkContainer = $("#updateSaved");

    var jSpan = $("#spEnterAddress");
    var jPrefShipMethod = $("#PreferredShipMethodLabel");

    if ($(controlId).is(":checked")) {
        jSel.hide();
        jDrop.hide();
        jChk.prop("checked", false);
        jChk.trigger("change");
        jChkContainer.hide();
        jSpan.hide();
        lastPrefShipMethodLabel = jPrefShipMethod.text();
        jPrefShipMethod.text("Best");
    } else {
        jSel.show();
        jDrop.show();
        jChkContainer.show();
        jSpan.show();
        if (lastPrefShipMethodLabel !== null) {
            jPrefShipMethod.text(lastPrefShipMethodLabel);
        }
        //controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther");
    }
}
