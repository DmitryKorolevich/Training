function getCountries(idVisibility, successCallback, errorCallback, complete)
{
    var url = "/Lookup/GetCountries";
    if (idVisibility)
    {
        url += "/" + idVisibility;
    }
    $.ajax({
        url: url,
        dataType: "json"
    }).success(function (result) {
        if (successCallback) {
            successCallback(result);
        }
    }).error(function (result) {
        if (errorCallback) {
            errorCallback(result);
        }
    }).complete(function (result) {
        if (complete) {
            complete(result);
        }
    });
}

function getCreditCardTypes(successCallback, errorCallback, complete) {
    $.ajax({
        url: "/Lookup/GetCreditCardTypes",
        dataType: "json"
    }).success(function (result) {
        if (successCallback) {
            successCallback(result);
        }
    }).error(function (result) {
        if (errorCallback) {
            errorCallback(result);
        }
    }).complete(function (result) {
        if (complete) {
            complete(result);
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
