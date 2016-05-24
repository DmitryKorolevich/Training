$(function () {
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

    $(".columns-container form").submit(function ()
    {
        $(".columns-container .overlay").show();
    });

    controlUpdateSavedState();
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
