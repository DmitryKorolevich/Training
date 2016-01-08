$(function ()
{
    getCountries(function (result)
    {
        var selectedCountry = $.grep(result.Data, function (country, countryIndex)
        {
            if (appSettings.DefaultCountryId == country.Id)
            {
                return country;
            }
        })[0];

        if (selectedCountry && selectedCountry.States && selectedCountry.States.length > 0)
        {
            $("#ddState").html("");

            $.each(selectedCountry.States, function (stateIndex, state)
            {
                $("#ddState").append($('<option></option>').val(state.StateCode).html(state.StateName));
            });

            var idState = $("#hdState").val();
            if (idState)
            {
                $("#ddState").val(idState);
            }
        }

    }, function (errorResult)
    {
        
    });

    var root = $('.vitalgreen');
    root.on("click", ".zone-description a", function (e)
    {
        var zoneId = $(this).data('zone-id');
        e.preventDefault();
        root.find('.overlay').show();

        $.ajax({
            url: "/VitalGreen/Step2?zoneid=" + zoneId,
            dataType: "html"
        }).success(function (result)
        {
            root.find('.step2').html(result);
            root.find('.step2').show();
        }).error(function (result)
        {
            notifyError();
        }).complete(function (result)
        {
            $('.vitalgreen .content-ajax-form-wrapper').hide();
            root.find('.overlay').hide();
        });
    });
});

function vitalGreenStep1SubmitSuccess(data)
{
    $('.vitalgreen .step1').hide();
}