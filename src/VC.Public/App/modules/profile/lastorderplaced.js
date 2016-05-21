$(function ()
{
    $(document).ready(function ()
    {
        $("body").on("click", ".add-to-cart", function ()
        {
            addToCart(null, $(this).attr("data-sku-code"));
            return false;
        });
    });
});