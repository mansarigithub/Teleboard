$(function () {
    page.init();
});

var page = {
    deviceId: $('#deviceId').val(),
    dragginData: {
        eventStartPoint: {},
        eventTargetPoint: {},
        overlapOccurred: false
    },
    init: function () {
        return $.ajax({
            url: "/AdsTimeBoxes/ReadDeviceTimeBoxes",
            type: "Get",
            data: {
                deviceId: page.deviceId,
            },
            success: function (timeboxes) {
                timeboxes.forEach(function (i, obj) { obj.color = page.getChannelColor(obj.Id); });
                page.initCalendar(page.deviceId, timeboxes);
                page.handleModalOKClick();
                page.handleModalCancelClick();
                page.handleModalRadioClick();
            }
        });
    },

    handleModalRadioClick: function () {
        $('input[name="selectedChannel"]').click(function () {
            var radio = $(this),
                channelId = radio.attr('value');
            $('.selected-channel')
                .css('background-color', page.getChannelColor(channelId))
                .text(radio.data('channel-name'))
                .parent().attr('href', '/Channels/Contents/' + channelId);
        });
    },

    handleModalOKClick: function () {
        $('.ok-button').click(function (e) {
            if (!page.validateModal()) {
                e.preventDefault();
                e.cancelBubble = true;
                return false;
            }
            var event = $('#eventModal').data('event');
            var selectedContents = [];

            $('.modal input:checked').each(function (index, elem) {
                var contentId = $(elem).data('content-id');
                var delaySec = $('input[class=content-delay-sec][data-content-id=' + contentId + ']').val();
                selectedContents.push({
                    Id: contentId,
                    DelaySeconds: delaySec,
                });
            });

            page.SaveData(event.timeBox.Id, selectedContents)
                .done(function (data) {
                    $('#eventModal').modal('hide');
                    event.timeBox = data.TimeBox;
                })
                .fail(function (data) {
                    teleboard.ui.showMessage(data.responseJSON.Message, teleboard.strings.error, 'error');
                });
        });
    },

    handleModalCancelClick: function () {
        $('.btn-modal-cancel').click(function () {
            $('#eventModal').modal('hide');
        });
    },

    handleEventClick: function (event) {
        var e = event.data;
        var t = e.timeBox;
        $('.timebox-info-day').text(t.WeekDay);
        $('.timebox-info-from').text(t.FromString);
        $('.timebox-info-to').text(t.ToString);
        $('.timebox-info-duration').text(t.DurationString);
        $('.timebox-info-free-time').text(t.FreeTimeString);
        $('.timebox-info-used-time').text(t.UsedTimeString);
        $('.progress-bar').css('width', t.UsedTimePercentage + '%');
        $('#eventModal').data('event', e).modal('show');
        $('.content-checkbox').prop('checked', false).change();
        page.readChannelContents(e.timeBox.ChannelId).then(function (data) {
            data.forEach(function (value, index) {
                var contentId = value.ContentId;
                var checkBox = $('input[type=checkbox][data-content-id=' + contentId + ']');
                var delaySec = $('input[class=content-delay-sec][data-content-id=' + contentId + ']');

                if (checkBox.length == 1) {
                    checkBox.prop('checked', 'true').change();
                    delaySec.val(value.DelaySeconds);
                }
            });
        });
    },

    handlContentCheckBoxChange: function (contentId, e) {
        var delaySec = $('input[class=content-delay-sec][data-content-id=' + contentId + ']');
        var checked = $(e.target).is(':checked');
        if (checked) {
            delaySec.removeAttr('disabled');
        } else {
            delaySec.attr('disabled', 'true');
        }
    },


    onDelaySecondsFocusOut: function (e) {
        var elem = $(e.target);
        var val = elem.val();
        if (!Number.isInteger(parseFloat(val))) {
            swal({});

        }
    },


    getChannelColor: function (channelId) {
        var colors = ['aqua', 'lightcoral', 'grey', 'fuchsia', 'green', 'lime', 'mediumaquamarine', 'crimson', 'olive', 'orange', 'rebeccapurple', 'lightblue', 'silver', 'teal', 'yellow'];
        return colors[channelId % colors.length];
    },

    initCalendar: function (deviceId, timeBoxes) {
        $('#calendar').fullCalendar({
            header: {
                left: '',
                right: 'agendaWeek,agendaDay'
            },
            navLinks: true,
            locale: teleboard.ui.IsEnglish() ? 'en' : 'fa',
            snapDuration: '00:01:00',
            aspectRatio: 1.0,
            scrollTime: '12:00 AM',
            editable: false,
            defaultView: 'agendaWeek',
            firstDay: teleboard.ui.IsEnglish() ? 0 : 6,
            eventLimit: true,
            columnFormat: 'ddd',
            axisFormat: 'HH:mm',
            selectable: false,
            selectLongPressDelay: 100,
            viewRender: function (view) {
            },
            eventRender: function (event, element) {
                if (event.timeBox.PlayAdvertisement === false) return;
                element.bind('click', event, page.handleEventClick);
            },
            events: timeBoxes.map(function (timeBox) {
                return {
                    id: timeBox.Id,
                    title: '',
                    start: page.normilizeDate(timeBox.WeekDay, timeBox.FromHour, timeBox.FromMinute).format("YYYY[-]MM[-]DD[T]HH:mm:ss"),
                    end: page.normilizeDate(timeBox.WeekDay, timeBox.ToHour, timeBox.ToMinute).format("YYYY[-]MM[-]DD[T]HH:mm:ss"),
                    color: timeBox.PlayAdvertisement ? 'lightgreen' : '#808080',
                    className: timeBox.PlayAdvertisement ? 'enabled-timebox' : 'disabled-timebox',
                    timeBox: timeBox,
                };
            }),
        });
    },



    normilizeDate: function (dayName, hour, minute) {
        var dayName = dayName.toLowerCase();

        if (teleboard.ui.IsEnglish()) {
            return moment().day(dayName)
                .hour(hour)
                .minute(minute)
                .second(0);
        }

        var offset;
        var sunday = moment().day() === 6 ? moment().add(1, 'd') : moment().startOf('week');

        if (dayName === 'saturday')
            offset = -1;
        else if (dayName === 'sunday')
            offset = 0;
        else if (dayName === 'monday')
            offset = 1;
        else if (dayName === 'tuesday')
            offset = 2;
        else if (dayName === 'wednesday')
            offset = 3;
        else if (dayName === 'thursday')
            offset = 4;
        else if (dayName === 'friday')
            offset = 5;

        return sunday.add(offset, 'd')
            .hour(hour)
            .minute(minute)
            .second(0);
    },

    validateModal: function () {
        var valid = true;
        $('input[class=content-delay-sec]')
            .each(function (index, elem) {
                var val = $(elem).val();
                if (valid && !elem.disabled && (!Number.isInteger(parseFloat(val)) || parseInt(val) <= 0)) {
                    teleboard.ui.showMessage(teleboard.strings.enterIntegerGreaterThanZero, teleboard.strings.error, 'error');
                    $(elem).focus();
                    valid = false;
                }
            });
        return valid;
    },

    readChannelContents: function (channelId) {
        return $.ajax({
            url: "/AdsTimeBoxes/ReadChannelContents",
            type: "GET",
            data: {
                channelId: channelId,
            },
            beforeSend: function (jqXHR, settings) {
            },
            complete: function (jqXHR, textStatus) {
            },
            success: function (data) {
            }
        });
    },

    SaveData: function (timeBoxId, selectedContents) {
        $('.btn-modal').attr('disabled', '');
        return $.ajax({
            url: "/AdsTimeBoxes/SaveTimeBox",
            type: "Post",
            data: {
                timeBoxId: timeBoxId,
                selectedContents: selectedContents,
            },
            beforeSend: function (jqXHR, settings) {
            },
            complete: function (jqXHR, textStatus) {
                $('.btn-modal').removeAttr('disabled');
            },
            success: function (data) {
                teleboard.ui.showMessage(data.Time, teleboard.strings.saved, 'success', 8000);
            }
        });
    },
};
