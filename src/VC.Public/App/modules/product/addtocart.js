function addToCart(elem, sku)
{
    addToCartMultiple(elem, [{Code: sku, Quantity: 1}])

    return false;
}

function addToCartMultiple(elem, skus)
{
    var l = null;
    if (elem != null)
    {
        l = Ladda.create(elem)
        l.start();
    }

    $(".overlay").show();

    $.ajax({
        url: "/Cart/AddToCartView",
        dataType: "html",
        data: JSON.stringify(skus),
        contentType: "application/json; charset=utf-8",
        type: "POST"
    }).success(function (result)
    {

        $.ajax({
            url: "/Cart/GetCartLiteComponent",
            dataType: "html",
            type: "GET"
        }).success(function (result)
        {
            $("#cart-lite-component").html(result);
        }).error(function (result)
        {
            notifyError();
        });

        $(result).dialog({
            resizable: false,
            modal: true,
            minWidth: 520,
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
        if (l != null)
        {
            l.stop();
        }

        $(".overlay").hide();
    });

    return false;
}

function closeCartLite()
{
    $("#" + $("#lnkClose").closest("div[aria-describedby]").attr('aria-describedby')).dialog('close');
    return false;
}

function addCrossToCart()
{
    closeCartLite();
    addToCart(null, $(this).attr("data-sku-code"));
    return false;
}