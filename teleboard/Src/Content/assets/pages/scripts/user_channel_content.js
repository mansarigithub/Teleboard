$(function () {
    teleboard.page.initialize();
});

teleboard.page = {
    strings: {
    },

    initialize: function () {

    },
}

function throttle(f, delay) {
    var timer = null;
    return function () {
        var context = this, args = arguments;
        clearTimeout(timer);
        timer = window.setTimeout(function () {
            f.apply(context, args);
        },
            delay || 1000);
    };
}

function swap($firstContentBox, $secondContentBox) {

    var firstContentId = $firstContentBox.data("contentid");
    var secondContentId = $secondContentBox.data("contentid");
    var channelId = $firstContentBox.data("channelid");

    $.ajax({
        type: "POST",
        url: "/Channels/Swap",
        data: {
            channelId: channelId,
            firstContentId: firstContentId,
            secondContentId: secondContentId
        },
        success: function (data) {
            teleboard.ui.showMessage(teleboard.strings.saved, '', 'success', 3000);
        }
    });
}

$('#selectedContents').on('keyup', '.delay-input', throttle(function () {
    var $contentBox = $(this).parents('.content-box');
    var delay = $(this).val();
    $.ajax({
        type: "POST",
        url: "/Channels/Delay",
        data: {
            contentId: $contentBox.data("contentid"),
            channelId: $contentBox.data("channelid"),
            delay: delay
        },
        success: function (data) {
            teleboard.ui.showMessage(teleboard.strings.saved, '', 'success', 3000);
        }
    });
}));

$('#selectedContents').on('click', '.move-up', function () {
    $(this).parents('.content-box').insertBefore($(this).parents('.content-box').prev()).show(function () {

        var $firstContentBox = $(this);
        var $secondContentBox = $firstContentBox.next('.content-box');

        swap($firstContentBox, $secondContentBox);
    });
});

$('#selectedContents').on('click', '.move-down', function () {
    $(this).parents('.content-box').insertAfter($(this).parents('.content-box').next()).show(function () {

        var $firstContentBox = $(this);
        var $secondContentBox = $firstContentBox.prev('.content-box');

        swap($firstContentBox, $secondContentBox);

    });
});

function toggleSelect(button, contentId, channelId, tenantId) {
    var element = $('[data-contentid="' + contentId + '"][data-channelid="' + channelId + '"][data-tenantid="' + tenantId + '"]');
    var selected = element.data("selected") == 'True';

    if (!selected) {
        $('.form-group-wait', element).removeClass('display-none');
        $('.move-up', element).removeClass('display-none');
        $('.move-down', element).removeClass('display-none');

    } else {
        $('.form-group-wait', element).addClass('display-none');
        $('.move-up', element).addClass('display-none');
        $('.move-down', element).addClass('display-none');
    }

    element.hide("fast", function () {
        if (!selected) {
            element.data("selected", "True");
            $("#selectedContents").prepend(element);
        }
        else {
            element.data("selected", "False");
            $("#allContents").prepend(element);
        }
    }).show("fast", function () {
        var url = "/Channels/" + (element.data("selected") == "True" ? "Select" : "Deselect");
        $.ajax({
            type: "POST",
            url: url,
            data: {
                contentId: contentId,
                channelId: channelId,
                delay: $('.delay-input', element).val(),
            },
            success: function (data) {
                teleboard.ui.showMessage( teleboard.strings.saved,'', 'success', 3000);
            }
        });
    });
}
