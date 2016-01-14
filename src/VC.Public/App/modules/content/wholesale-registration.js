$(function ()
{
    var root = $('.wholesale.registration');

    root.on('click', '#step1go', function ()
    {
        var email = root.find('#Email').val();
        if (!email)
        {
            alert('E-mail addess is a required field.');
            return;
        }
    });
});