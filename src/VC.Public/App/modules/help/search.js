﻿$(function ()
{
    google.load('search', '1', { language: 'en' });
    google.setOnLoadCallback(function ()
    {
        var customSearchControl = new google.search.CustomSearchControl(googleSearchcx);
        customSearchControl.setResultSetSize(google.search.Search.FILTERED_CSE_RESULTSET);
        customSearchControl.setLinkTarget(google.search.Search.LINK_TARGET_SELF);
        var options = new google.search.DrawOptions();
        options.setAutoComplete(true);
        options.enableSearchResultsOnly();
        customSearchControl.draw('search-result-box', options);
        var queryFromUrl = getQueryParameterByName('q');
        if (queryFromUrl)
        {
            customSearchControl.execute(queryFromUrl);
            $('.working-area-holder .overlay').hide();
        }
    }, true);
});