var loaded = false;

$(document).ready(function ()
{
    var setSelectedSku = function () {
        var jChecked = $("input[name=sku]:checked");

        var inStock = jChecked.data("in-stock");
        if (inStock) {
            $(".product-action-right .in-stock").show();
            $(".product-action-right .out-of-stock").hide();
            $("#spSelectedPrice").text("Selected Price " + jChecked.attr("data-price"));
            if (jChecked.attr("data-autoship") && jChecked.attr("data-autoship").toLowerCase() == "true") {
                $(".product-autoship-container").show();
            } else {
                $(".product-autoship-container").hide();
            }
        }
        else {
            $(".product-action-right .in-stock").hide();
            $(".product-action-right .out-of-stock").show();
        }
        $("#hSelectedCode").text("Product #" + jChecked.val());
    };

    $("input[name=sku]:first").attr("checked", true);
    setSelectedSku();

    $("body").on("click", "#lnkReviewsTab", function () {
        $(".tabs-control").tabs({ "active": 1 });
    });

    $("body").on("click", "#lnkDescriptionTab", function () {
        $(".tabs-control").tabs({ "active": 0 });
    });

    $("body").on("change", "input[name=sku]", setSelectedSku);

    $("body").on("click", "#lnkAddToCart", addToCartSelectedSku);
    $("body").on("click", "#lnkClose", closeCartLite);

    $("body").on("click", ".proposals-item-link", addCrossToCart);

    $("body").on("click", ".product-autoship-link", function () {
        $("<div title='Set Up My New Auto-Ship' class='autoship-confirm-dialog'>" +
			"Enjoy Free Shipping and Discount* on every Auto-Shipment!<ul><li>Upon clicking Proceed, any items in your Cart will be replaced<br>with this Auto-Ship item, and no other items can be added later.</li><li>Just complete this Auto-Ship order*, then shop to fill a new Cart.</li><li>If you don't want to Proceed with this Auto-Ship order, click Cancel.</li></ul>*Cannot be combined with other offers. Not available to Canadian or other international addresses.</div>").dialog({
			    resizable: false,
			    modal: true,
			    minWidth: defaultModalSize,
			    close: function () {
			        $(this).dialog('destroy').remove();
			    },
			    buttons: [
                    {
                        text: "Proceed",
                        'class': "main-dialog-button",
                        click: function () {
                            var jChecked = $("input[name=sku]:checked");
                            sku = jChecked.val();

                            window.location.href = "/Cart/AutoShip/" + sku;
                        }
                    },
                    {
                        text: "Cancel",
                        click: function () {
                            $(this).dialog("close");
                        }
                    }
			    ]
			});

        return false;
    });

    $(".product-action-right .out-of-stock a").click(function (e) {
        $.ajax({
            url: "/Product/AddOutOfStockProductRequest/" + productPublicId,
            dataType: "html"
        }).success(function (result) {
            $(result).dialog({
                resizable: false,
                modal: true,
                minWidth: defaultModalSize,
                dialogClass: "add-out-of-stock-request-dialog",
                open: function () {
                    grecaptcha.render('googleCaptcha', {
                        'sitekey': captchaSiteKey
                    });
                },
                close: function () {
                    $(this).dialog('destroy').remove();
                },
                buttons: [
					{
					    text: "Submit Out of Stock Request",
					    'class': "main-dialog-button",
					    click: function () {
					        var selector = "#addOutOfStockProductRequestDialog form";
					        var jForm = $(selector);
					        reparseElementValidators(selector);
					        jForm.validate()
					        if (jForm.valid()) {
					            jForm.submit();
					        }
					    }
					},
					{
					    text: "Cancel",
					    click: function () {
					        $(this).dialog("close");
					    }
					}
                ]
            });
        }).error(function (result) {
            notifyError();
        });

        e.preventDefault();
        return false;
    });

    var review = getQueryParameterByName("review");
    if (review=='true')
    {
        productPageAddReview();
    }
});

function addOutOfStockProductRequestFormSubmitSuccess(data)
{
    ajaxFormSubmitSuccess();
    if (successMessage)
    {
        $("#addOutOfStockProductRequestDialog").dialog("close");
    }
}

function addToCartSelectedSku()
{
    var jChecked = $("input[name=sku]:checked");
    sku = jChecked.val();

    addToCart(null, sku);
    return false;
}

function addCrossToCart() {
	closeCartLite();
	addToCart(null, $(this).attr("data-sku-code"));
	return false;
}

function closeCartLite() {
	$("#" + $("#lnkClose").closest("div[aria-describedby]").attr('aria-describedby')).dialog('close');
	return false;
}