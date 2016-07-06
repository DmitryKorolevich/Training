function addToCart(elem, sku)
{
    var l = Ladda.create(elem)
    l.start();

    $.ajax({
        url: "/Cart/AddToCartView?skuCode=" + sku,
        dataType: "html",
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
    }).complete(function() {
        l.stop();
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