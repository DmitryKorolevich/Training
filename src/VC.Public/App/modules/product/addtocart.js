function addToCart(event, sku)
{
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
    });

    return false;
}