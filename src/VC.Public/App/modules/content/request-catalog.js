$(function ()
{
    $('#requestCatalogFormWrapperInner ').on( "change", "#SignUpOnNewsletters", function ()
    {
        if (this.checked)
        {
            $('#requestCatalogFormWrapperInner #Email').closest(".form-group").show();
        }
        else
        {
            $('#requestCatalogFormWrapperInner #Email').closest(".form-group").hide();
        }
    });
});