$(function() {
	getCreditCardTypes(function (result) {
		var creditCardTypes = result.Data;

		$.each(result.Data, function (creditTypeIndex, creditType) {
			$("#ddCardType").append($('<option></option>').val(creditType.Key).html(creditType.Text));
		});
	}, function (errorResult) {
		//todo: handle result
	});
});