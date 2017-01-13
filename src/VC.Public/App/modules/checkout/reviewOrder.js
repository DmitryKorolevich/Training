$(function() {
	$("body").on("click", "#btnPlaceOrder", function() {
		$("#viewCartForm").submit();
	})

	var items = $('td.cart-line-info');
	$.each(items, function (index, item)
	{
	    var buttons = $('.assign-buttons.template').clone();
	    buttons.removeClass('hide');
	    buttons.removeClass('template');
	    $(item).append(buttons);
	});

	var popupContent = '<div title="Select order"><form class="form-regular medium-small"><div class="form-group"><label class="control-label">Orders</label><div class="input-group"><select class="form-control" id="ddOrder">' +
        '<option value="1">Order 1</option><option value="2">Order 2</option><option value="4">Order 4</option><option value="5">Order 5</option></select></div></div></form></div>';

	$("body").on("click", ".assign-buttons .button-green", function ()
	{
	    var wrapper = $(this).closest('table').eq(0);
	    var row = $(this).closest('tr');
	    openAssignPopup(function (id)
	    {
	        row.remove();
	        var targetItem = $('.item[data-id=' + id + ']');
	        targetItem.find('tbody').eq(1).append(row);
	    }, wrapper);
	});

	$("body").on("click", ".assign-buttons .button-red", function ()
	{
	    var wrapper = $(this).closest('table').eq(0);
	    var row = $(this).closest('tr');
	    openAssignPopup(function (id)
	    {
	        row = row.clone();
	        var targetItem = $('.item[data-id=' + id + ']');
	        targetItem.find('tbody').eq(1).append(row);
	    }, wrapper);
	});

	var openAssignPopup = function (successCallback, wrapper)
	{
	    $(popupContent).dialog({
	        resizable: false,
	        modal: true,
	        minWidth: defaultModalSize,
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