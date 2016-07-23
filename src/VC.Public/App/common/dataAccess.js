function getCountries(idVisibility, successCallback, errorCallback)
{
    var url = "/Lookup/GetCountries";
    if (idVisibility)
    {
        url += "/" + idVisibility;
    }
	$.ajax({
	    url: url,
		dataType: "json"
	}).success(function(result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function(result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}

function getCreditCardTypes(successCallback, errorCallback) {
	$.ajax({
		url: "/Lookup/GetCreditCardTypes",
		dataType: "json"
	}).success(function(result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function(result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}

function getCartShippingOptions(successCallback, errorCallback) {
	$.ajax({
		url: "/Lookup/GetCartShippingOptions",
		dataType: "json"
	}).success(function (result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function (result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}

function getShippingPreferredOptions(successCallback, errorCallback) {
	$.ajax({
		url: "/Lookup/GetShippingPreferredOptions",
		dataType: "json"
	}).success(function (result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function (result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}
