$(function ()
{
    $("body").on("change", ".receipt-egift-email-wrapper .all-egifts", function ()
    {
        $("body .receipt-egift-email-wrapper .egift").prop('checked', this.checked);
    });

    $("body").on("change", ".receipt-egift-email-wrapper .egift", function ()
    {
        var allChecked = true;
        $.each($("body .receipt-egift-email-wrapper .egift"), function (i, item)
        {
            if (!item.checked)
            {
                allChecked = false;
                return false;
            }
        });
        $("body .receipt-egift-email-wrapper .all-egifts").prop('checked', allChecked);
    });
});