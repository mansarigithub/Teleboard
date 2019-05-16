$(document).ajaxComplete(function (event, xhr, settings) {
    //if (xhr.status == 200) return;
    //var r = xhr.responseJSON;
    //if (typeof (r) != 'undefined' && r.Type == teleboard.enums.processResultType.error) {
    //    teleboard.ui.showMessage(r.Message, '', 'error');
    //}
    teleboard.loadingData = false;
    teleboard.ui.showLoadingIcon(false);
});

$(document).ajaxSend(function (event, xhr, ajaxOptions) {
    teleboard.loadingData = true;
    setTimeout(function () {
        if (teleboard.loadingData) {
            teleboard.ui.showLoadingIcon(true);
        }
    }, 1000);
});

var teleboard = {
    loadingData: false,
    page: {},
    enums: {
        processResultType: {
            success: 1,
            error: 2,
            warning: 3
        },
        pushNotificationType: {
            NewNotification: 1,
            NewMessage: 2
        }
    },

    app: {
        initialize: function () {
            teleboard.ui.initialize();
        }
    },

    ui: {
        language: '',
        IsEnglish: function () {
            return teleboard.ui.language === 'English';
        },
        initialize: function () {
            teleboard.ui.language = $('#user_language').val();
            teleboard.ui.validation.initialize();
            teleboard.ui.showHelpMessage();
            // Persian Number Plugin
            //$('.number-fa').persiaNumber();

            // Bootstrap Switch Plugin
            if ($.fn.bootstrapSwitch)
                $('.switch-control').bootstrapSwitch({
                    onText: teleboard.strings.yes,
                    offText: teleboard.strings.no
                });

            // Bootstrap Toastr Plugin
            if (typeof toastr !== 'undefined')
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "positionClass": "toast-top-left",
                    "onclick": null,
                    "showDuration": "1000",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "1000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                };

            $('.container-fluid').on('click', teleboard.ui.toggleSidebar);

            teleboard.ui.initContentThumbPreview();
        },

        localization: {
            toPersianNumber(selector, context) {
                $(selector, context).persiaNumber();
            }
        },
        showMessage: function (message, title, messageType, hideAfter) {
            if (typeof $.toast === 'undefined') return;
            hideAfter = Number.isInteger(hideAfter) ? hideAfter : 4000;
            $.toast({
                heading: title,
                text: message,
                position: 'top-right',
                loaderBg: '#ff6849',
                icon: messageType,
                hideAfter: hideAfter
            });
        },

        showHelpMessage: function () {
            var alert = $('.page-help');
            var key = alert.data('page-name') + '-pageHelpCounter';
            if (typeof localStorage[key] === 'undefined')
                localStorage[key] = 0;
            if (parseInt(localStorage[key]) > 1)
                return;

            alert.removeClass('hide')
                .addClass('show')
                .on('closed.bs.alert', function () {
                    localStorage[key] = ++localStorage[key];
                });
        },
        showAlert: function (text, title, type = 'warning') {
            swal({
                text: text,
                title: title,
                type: type
            });
        },
        showLoadingIcon: function (show) {
            var i = $('.app-loading-icon');
            if (show)
                i.show();
            else
                i.hide();
        },
        validation: {
            initialize: function () {
                if (!$.fn.validate) return;

                var forms = $('.validatable-form');
                var error1 = $('.alert-danger', forms);
                var success1 = $('.alert-success', forms);

                forms.each(function (index, element) {
                    var settings = $(element).validate().settings;

                    settings.highlight = function (element) { // hightlight error inputs
                        var formGroup = $(element).closest('.form-group');
                        formGroup.removeClass('has-success').addClass('has-error'); // set error class to the control group
                        $('.help-block:not(.help-block-error)', formGroup).hide();
                    };
                    settings.unhighlight = function (element) { // revert the change done by hightlight
                        var formGroup = $(element).closest('.form-group');
                        formGroup.addClass('has-sucess').removeClass('has-error'); // set error class to the control group
                        $('.help-block:not(.help-block-error)', formGroup).show();
                    };
                    settings.ignore = ':hidden:not(.validatable-field)';
                    //settings.ignore = '.ignore';
                });
            },

            extendValidationRules() {
                jQuery.validator.addMethod("enforcetrue", function (value, element, param) {
                    return element.checked;
                });
                jQuery.validator.unobtrusive.adapters.addBool("enforcetrue");
            }
        },

        toggleSidebar: function () {
            //$(".sidebartoggler").click();
            var body = $('body');
            if (body.hasClass('mini-sidebar') && body.hasClass('show-sidebar'))
                $(".nav-toggler").click();
        },

        initContentThumbPreview() {
            $('.content-thumb .content-cover').click(function (e) {
                var cover = $(e.delegateTarget);
                var parentDiv = cover.parent();
                var img = $('img', parentDiv);
                var src = img.data('content-src');
                var videoTag = '<video src="{src}" preload="metadata" controls style="width:100%;" autoplay />'.replace('{src}', src);
                cover.remove();
                img.remove();
                parentDiv.prepend(videoTag);
            });
        }
    }
};

(function ($) {
    teleboard.app.initialize();
    if ($.fn.validate) {
        teleboard.ui.validation.extendValidationRules();
    }
}(jQuery));

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};