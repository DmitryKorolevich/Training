$(function() {
	$("body").on("click", "#btnPlaceOrder", function() {
		$("#viewCartForm").submit();
	})

	var updateUIVisibility = function ()
	{
	    if ($('.item').length >1)
	    {
	        $('.top-billing-wrapper').show();
	        $('.item .reivew-info-box.billing').hide();
	        $('.item .cart-promo-container').hide();
	        $('.item .cart-gc-container').hide();
	    }
	    else
	    {
	        $('.top-billing-wrapper').hide();
	        $('.item .reivew-info-box.billing').show();
	        $('.item .cart-promo-container').show();
	        $('.item .cart-gc-container').show();
	    }

	    $('.item').removeClass('alternate-color');
	    $.each($('.item'), function (i, item)
	    {
	        if (i % 2 == 0)
	        {
	            $(item).addClass('alternate-color');
	        }
	    });
	};
	updateUIVisibility();

	$("body").on("click", ".item .delete-order", function ()
	{
	    $(this).closest('.item').remove();
	    updateUIVisibility();
	});

	var items = $('td.cart-line-info');
	$.each(items, function (index, item)
	{
	    var buttons = $('.assign-buttons.template').clone();
	    buttons.removeClass('hide');
	    buttons.removeClass('template');
	    $(item).append(buttons);
	});

	var popupCopyContent = '<div title="Select order"><form class="form-regular small"><div class="form-group"><label class="control-label">Orders</label><div class="input-group"><select class="form-control big" id="ddOrder">' +
        '<option value="1">Order #1 - Gary1 Gould 806 Front ST</option>' +
        '<option value="2">Order #2 - Gary2 Gould 806 Front ST</option>' +
        '<option value="4">Order #4 - Gary4 Gould 806 Front ST</option>' +
        '<option value="5">Order #5 - Gary5 Gould 806 Front ST</option></select></div></div></form></div>';

	var popupMoveContent = '<div title="Select order"><form class="form-regular small"><div class="form-group"><label class="control-label">Orders</label><div class="input-group"><select class="form-control big" id="ddOrder">' +
        '<option value="1">Order #1 - Gary1 Gould 806 Front ST</option>' +
        '<option value="2">Order #2 - Gary2 Gould 806 Front ST</option>' +
        '<option value="4">Order #4 - Gary4 Gould 806 Front ST</option>' +
        '<option value="5">Order #5 - Gary5 Gould 806 Front ST</option></select></div></div>' +
        '<div class="form-group"><label class="control-label">QTY</label><div class="input-group"><div class="input-group"><input class="form-control small-form-control" type="text" id="qty" value="1"></div></div>' +
        '</form></div>';

	$("body").on("click", ".assign-buttons .button-blue", function ()
	{
	    var row = $(this).closest('tr');
	    openAssignPopup(function (id)
	    {
	        row.remove();
	        var targetItem = $('.item[data-id=' + id + ']');
	        targetItem.find('tbody').eq(1).append(row);
	    }, popupMoveContent);
	});

	$("body").on("click", ".assign-buttons .button-green", function ()
	{
	    var row = $(this).closest('tr');
	    openAssignPopup(function (id)
	    {
	        row = row.clone();
	        var targetItem = $('.item[data-id=' + id + ']');
	        targetItem.find('tbody').eq(1).append(row);
	    }, popupCopyContent);
	});

	var openAssignPopup = function (successCallback, content)
	{
	    $(content).dialog({
	        resizable: false,
	        modal: true,
	        minWidth: 515,
	        open: function ()
	        {
	        },
	        close: function ()
	        {
	            $(this).dialog('destroy').remove();
	        },
	        buttons: [
                {
                    text: "Select",
                    'class': "main-dialog-button",
                    click: function ()
                    {
                        successCallback($('#ddOrder').val());
                        $(this).dialog("close");
                    }
                },
                {
                    text: "Cancel",
                    click: function ()
                    {
                        $(this).dialog("close");
                    }
                }
	        ]
	    });
	};
});