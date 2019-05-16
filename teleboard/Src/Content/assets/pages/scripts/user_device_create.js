$(function () {
    teleboard.page.initialize();
});

teleboard.page = {
    strings: {
    },
    templates: {

    },
    initialize: function () {
        var zoneOffset = moment().tz(moment.tz.guess()).format('ZZ');
        var timeZone = $('option[data-tz-offset = "' + zoneOffset + '"]').val();
        var option = $('#Device_TimeZoneId').val(timeZone);
    },

    onTenantChange: function (e) {
        var value = $(e.target).val();
        var option = $('option[value=' + value + ']');
        var adsEnabled = option.data('ads-enabled').toLowerCase() === 'true';
        var adsFormGroup = $('.ads-form-group, .ads-form-group *');
        if (adsEnabled) {
            adsFormGroup.removeAttr('disabled').removeClass('disabled');
        } else {
            $('#playAds-no').prop('checked', true).change();
            adsFormGroup.attr('disabled', '').addClass('disabled');
        }
    }
}
