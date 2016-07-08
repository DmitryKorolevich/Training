$(document).ready(function ()
{
    var submitted = false;

    $('.category-skus table input').change(function ()
    {
        if(submitted)
        {
            isValidSku(this);
        }
    });

    $("body").on("click", "#lnkClose", closeCartLite);

    $("body").on("click", ".proposals-item-link", addCrossToCart);

    function isValidSku(elem)
    {
        var value = elem.value;
        if (value.trim() != "" && !value.match(/^[0-9]+$/))
        {
            $(elem).addClass("input-validation-error");
            $(elem).parent().find(".field-validation-error").removeClass("hide-imp");
            return false;
        }
        else
        {
            $(elem).removeClass("input-validation-error");
            $(elem).parent().find(".field-validation-error").addClass("hide-imp");
            return true;
        }
    }

    $(".category-skus #lnkAddToCart").click(function (e)
    {
        var valid = true;
        $.each($('.category-skus table input'), function (index, item)
        {
            var newValid = valid && isValidSku(item);
            if (valid && !newValid)
            {
                $.scrollTo(item);
            }
            valid = newValid;
        });

        if (valid)
        {
            var skus = [];
            $.each($('.category-skus table input'), function (index, item)
            {
                var value = item.value;
                if (value.trim() != "" && value.match(/^[0-9]+$/) && parseInt(value)!=0)
                {
                    skus.push({
                        Code: $(item).data("sku-code"),
                        Quantity: parseInt(value),
                    });
                }
            });

            if(skus.length>0)
            {
                addToCartMultiple(e.currentTarget, skus);
                $('.category-skus table input').val("");
            }
        }
        else
        {
            submitted = true;
        }
        e.preventDefault();
    });
});