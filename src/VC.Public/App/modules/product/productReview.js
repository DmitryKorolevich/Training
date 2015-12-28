window.addEventListener("load", function() {
    $("body").on("click", ".write-review-link", productPageAddReview);
}, false);

function productPageAddReview() {
    $.ajax({
        url: "/Product/AddReview/" + productPublicId,
        dataType: "html"
    }).success(function (result) {
        $(result).dialog({
            resizable: false,
            modal: true,
            minWidth: defaultModalSize,
            dialogClass: "product-reviews-dialog",
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
                    text: "Submit Your Review",
                    'class': "main-dialog-button",
                    click: function () {
                        var selector = "#reviewDialog form";
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

    return false;
}

function reviewSubmitSuccess() {
	if (successMessage) {
		notifySuccess(successMessage);
		$("#reviewDialog").dialog("close");
	} else {
		grecaptcha.render('googleCaptcha', {
			'sitekey': captchaSiteKey
		});
	}
}

function reviewSubmitError() {
	notifyError("Server error occured");
}