$(function ()
{
    var root = $('.ad-banners');
    var codeArea = root.find('.code-area');
    var st1 = root.find('.h-step1');
    var st2 = root.find('.h-step2');
    var st3 = root.find('.h-step3');

    root.find('.banner-size').click(function ()
    {
        var bannersid=$(this).data('banner-size');
        root.find('.banners').hide();
        root.find('.banners-'+bannersid).show();

        codeArea.hide();
        $.scrollTo(st2);
    });

    root.find('.banners img').click(function ()
    {
        var width = $(this).data('width');
        if (!width)
        {
            width = this.width;
        }
        var height = $(this).data('height');
        if (!height)
        {
            height = this.height;
        }
        var text = generateBannerCode(this.src, width, height);
        codeArea.text(text)

        codeArea.show();
        $.scrollTo(st3);
        codeArea.focus();
    });

    root.find('img.main-banner').click(function ()
    {
        var text = generateBannerCode(this.src, this.width, this.height);
        codeArea.text(text)

        codeArea.show();
        root.find('.banners').hide();
        $.scrollTo(st3);
        codeArea.focus();
    });

    codeArea.focus(function()
    {
        this.select();
    });

    var generateBannerCode = function (imgUrl, width, height)
    {
        var toReturn = '<a href="{0}/?idaffiliate={4}"><img src="{1}" width="{2}" height="{3}" border="0"/></a>'.format(window.location.origin, imgUrl, width, height, idAffiliate);
        return toReturn;
    };
});