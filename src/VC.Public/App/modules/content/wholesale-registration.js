$(function ()
{
    var root = $('.wholesale.registration');
    reInitFormValidation(".wholesale.registration form");

    $(document).ready(function ()
    {
        root.find('#Email').focus();
    });

    root.on('submit', '.body-inner-step1 form', function ()
    {
        var form = $(this);
        if (form.valid())
        {
            executeStep1(root.find('#Email').val());
        }
        return false;
    });

    var executeStep1 = function (email)
    {
        root.find('.overlay').show();
        $.ajax({
            type: "POST",
            url: "/Account/CheckEmail/?email=" + email,
            dataType: "json"
        }).success(function (result)
        {
            if (!result.Data)
            {
                root.find('form #Email').val(email)
                root.removeClass('step1');
                root.addClass('step2');
            }
            else
            {
                window.location.href = result.Data;
            }
        }).error(function (result)
        {

        }).complete(function (result)
        {
            root.find('.overlay').hide();
        });
    };
});

var registerWholesaleFormSubmitBegin = function (data)
{
    $('.wholesale.registration .overlay').show();
}

var registerWholesaleFormSubmitComplete = function (data)
{
    $('.wholesale.registration .overlay').hide();
}

var registerWholesaleFormSubmitSuccess = function (data)
{
    if (successMessage)
    {
        window.location.href = '/content/wholesale-review';
    } else
    {
        reInitFormValidation(".wholesale.registration form");
        refreshAjaxForm(data);
        $.scrollTo(".wholesale.registration form");
    }
};