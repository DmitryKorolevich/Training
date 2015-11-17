$(function ()
{
    function getGCInfo(code, successCallback, errorCallback)
    {
        $.ajax({
            url: "/GC/GetGCInfo/"+code,
            dataType: "json"
        }).success(function (result)
        {
            if (successCallback)
            {
                successCallback(result);
            }
        }).error(function (result)
        {
            if (errorCallback)
            {
                errorCallback(result);
            }
        });
    }

    $(document).ready(function ()
    {
        var root = $('.gc-checker');
        if (root.length > 0)
        {
            root.find('input[type=button]').click(function() {
                var code = root.find('input[type=text]').val();
                root.find('.info').hide();
                getGCInfo(code, function (result)
                {
                    if(result.Data)
                    {
                        if(result.Data.IsActive)
                        {
                            root.find('.active').show();
                            root.find('.inactive').hide();
                            root.find('.status').text('ACTIVE');
                        }
                        else
                        {
                            root.find('.active').hide();
                            root.find('.inactive').show();
                            root.find('.status').text('INACTIVE');
                        }
                        root.find('.code').show();
                        root.find('.amount').show();
                        root.find('.code strong').text(result.Data.Code);
                        root.find('.amount strong').text(result.Data.AmountLine);
                    }
                    else
                    {
                        root.find('.active').hide();
                        root.find('.inactive').show();
                        root.find('.status').text('Gift Certificate Not Found');
                        root.find('.code').hide();
                        root.find('.amount').hide();
                        root.find('.code strong').text('');
                        root.find('.amount strong').text('');
                    }
                    root.find('.info').show();
                },
                function (result)
                {
                    var text = null;
                    if (result.Messages)
                    {
                        text = result.Messages.map(function () { return this.Message; });
                    }
                    notifyError(text);
                });
            });
        }
    });
});