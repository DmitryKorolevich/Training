﻿$(function() {
    $(document).ready(function ()
    {
        $('.cb-promote').change(function ()
        {
            var section = $(this).closest(".form-group").find('.section');
            if (this.checked)
            {
                section.show();
            }
            else
            {
                section.hide();
            }
        });
    });
});