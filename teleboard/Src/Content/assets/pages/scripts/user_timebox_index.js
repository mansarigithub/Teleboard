$(function () {
    page.init();
    page.autoSave = !page.isGroupScheduling;
});

var page = {
    deviceId: $('#deviceId').val(),
    deviceIds: $('#deviceIds').val().split(','),
    isGroupScheduling: $('#IsGroupScheduling').val() === 'True',
    getSelectedDeviceId: function () {
        return $('.device-button.selected').data('device-id');
    },
    dragginData: {
        eventStartPoint: {},
        eventTargetPoint: {},
        overlapOccurred: false
    },
    init: function () {
        page.initClockPicker();
        page.handleModalOKClick();
        page.handleModalCancelClick();
        page.handleModalDeleteClick();
        page.handleModalRadioClick();
        page.handleModalTimeInputChange();
        page.handleDeviceButtonClick();
        page.initCalendar();
        page.highlightCurrentTime();
        page.styleCalendarHeader();

        if (!page.isGroupScheduling) {
            page.readDeviceTimeboxes(page.deviceIds[0]);
        }
    },

    mapTimeboxesToEvents: function (timeBoxes) {
        return timeBoxes.map(function (timeBox) {
            return {
                id: timeBox.Id,
                title: timeBox.ChannelName,
                start: page.normilizeDate(timeBox.WeekDay, timeBox.FromHour, timeBox.FromMinute).format("YYYY[-]MM[-]DD[T]HH:mm:ss"),
                end: page.normilizeDate(timeBox.WeekDay, timeBox.ToHour, timeBox.ToMinute).format("YYYY[-]MM[-]DD[T]HH:mm:ss"),
                color: page.getChannelColor(timeBox.ChannelId),
                ChannelId: timeBox.ChannelId,
                timeBox: timeBox
            };
        });
    },
    readDeviceTimeboxes: function (deviceId) {
        return $.ajax({
            url: "/TimeBoxes/ReadDeviceTimeBoxes",
            type: "Get",
            data: {
                deviceId: deviceId,
            },
            success: function (timeboxes) {
                timeboxes.forEach(function (i, obj) { obj.color = page.getChannelColor(obj.Id); });
                $('#calendar').fullCalendar('removeEvents');
                $('#calendar').fullCalendar('renderEvents', page.mapTimeboxesToEvents(timeboxes));
            }
        });
    },
    styleCalendarHeader: function () {
        $('th.fc-day-header a').wrap("<span class='vertical-text'></span>");
    },
    initClockPicker: function () {
        $(".clockpicker").clockpicker({
            donetext: teleboard.strings.ok,
            autoclose: true,
            afterDone: function () {
                if (teleboard.ui.IsEnglish() == false) {
                    $('#fromTime').val($('#fromTime').val().toPersionNumber());
                    $('#toTime').val($('#toTime').val().toPersionNumber());
                }
            },
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

    handleDeviceButtonClick: function () {
        $('.device-button').click(function () {
            var button = $(this),
                deviceId = button.data('device-id'),
                deviceName = button.data('device-name');
            page.readDeviceTimeboxes(deviceId)
                .done(function (timeboxes) {
                    $('.device-button').addClass('btn-secondary').removeClass('btn-info selected');
                    $('.device-button[data-device-id=' + deviceId + ']').addClass('btn-info selected').removeClass('btn-secondary');
                    teleboard.ui.showMessage(teleboard.strings.deviceSchedulingLoaded.replace('{0}', deviceName), teleboard.strings.schedulingLoaded, 'success');
                });
        });
    },

    handleModalOKClick: function () {
        $('.btn-modal-ok').click(function (e) {
            var fromInput = $('.from-time'),
                toInput = $('.to-time'),
                channel = $('input[name=selectedChannel]:checked'),
                channelId = channel.val();
            if (!page.validateModal(fromInput, toInput, channel))
                return;
            var modalData = $('#eventModal').data('event'),
                startTime = moment.utc(modalData.start.format('YYYY-MM-DD ') + fromInput.val(), 'YYYY-MM-DD HH:mm'),
                endTime = moment.utc(modalData.end.format('YYYY-MM-DD ') + toInput.val(), 'YYYY-MM-DD HH:mm'),
                title = channel.data('channel-name'),
                event = modalData.mode === 'edit' ? modalData : { id: Math.floor(Math.random() * 1000000), timeBox: { PlayAdvertisement: false } },
                playAdvertisement = $('#allowAdsCheckbox').is(':checked');
            if (!page.validateTimes(startTime, endTime, event.id))
                return;

            event.start = startTime;
            event.end = endTime;
            event.title = title;
            event.color = page.getChannelColor(channel.val());
            event.ChannelId = channelId;
            event.timeBox.PlayAdvertisement = playAdvertisement;

            if (modalData.mode === 'add') {
                $('#calendar').fullCalendar('renderEvent', event, true);
            } else {
                $('#calendar').fullCalendar('updateEvent', event);
            }
            if (page.autoSave) {
                page.saveData().done(function () {
                    $('#eventModal').modal('hide');
                });
            } else {
                $('#eventModal').modal('hide');
            }
        });
    },

    handleModalCancelClick: function () {
        $('.btn-modal-cancel').click(function () {
            $('#eventModal').modal('hide');
        });
    },

    handleModalDeleteClick: function () {
        $('.btn-modal-delete').click(function (e) {
            var event = $('#eventModal').data('event');
            if (event.mode === 'add') {
                $('#eventModal').modal('hide');
                return;
            }
            $('#calendar').fullCalendar('removeEvents', event.id);
            if (page.autoSave) {
                page.saveData().done(function () {
                    $('#eventModal').modal('hide');
                });
            } else {
                $('#eventModal').modal('hide');
            }

        });
    },

    handleModalTimeInputChange: function () {
        $('#fromTime').change(function () {
            if (teleboard.ui.IsEnglish() == false)
                $('#fromTime').val($('#fromTime').val().toPersionNumber());
        });
        $('#toTime').change(function () {
            if (teleboard.ui.IsEnglish() == false)
                $('#toTime').val($('#toTime').val().toPersionNumber());
        });

    },

    getChannelColor: function (channelId) {
        var colors = ['aqua', 'lightcoral', 'grey', 'fuchsia', 'green', 'lime', 'mediumaquamarine', 'crimson', 'olive', 'orange', 'rebeccapurple', 'lightblue', 'silver', 'teal', 'yellow'];
        return colors[channelId % colors.length];
    },

    initCalendar: function () {
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
            editable: true,
            defaultView: 'agendaWeek',
            firstDay: teleboard.ui.IsEnglish() ? 0 : 6,
            eventLimit: true,
            columnFormat: 'ddd',
            axisFormat: 'HH:mm',
            selectable: true,
            selectLongPressDelay: 100,
            viewRender: function (view) {
            },
            eventRender: function (event, element) {
                element.bind('click', function () {
                    event.mode = 'edit';
                    $('#fromTime').val(moment(event.start).format('HH:mm'));
                    $('#toTime').val(moment(event.end).format('HH:mm'));
                    if (teleboard.ui.IsEnglish()) {
                        let format = '[(]YYYY/MM/DD[)]';
                        $('.from-date-time').text(event.start.format(format));
                        $('.to-date-time').text(event.end.format(format));
                    } else {
                        let format = '(YYYY/MM/DD)';
                        $('.from-date-time').text(new persianDate(event.start.toDate()).format(format));
                        $('.to-date-time').text(new persianDate(event.end.toDate()).format(format));
                    }

                    $('.selected-channel').text(event.title).css('background', event.color)
                        .parent().attr('href', '/Channels/Contents/' + event.ChannelId);
                    $('input[value=' + event.ChannelId + ']').prop('checked', 'true');

                    $('#allowAdsCheckbox').prop('checked', event.timeBox.PlayAdvertisement);
                    $('#eventModal').data('event', event).modal('show');
                });
            },
            select: function (start, end, allDay) {
                $('.selected-channel').html(teleboard.strings.pleaseSelectAChannel).css('background-color', '')
                    .parent().removeAttr('href');
                $('input[type=radio]').prop('checked', false);

                $('#fromTime').val(moment(start).format('HH:mm'));
                $('#toTime').val(moment(end).format('HH:mm'));
                if (teleboard.ui.IsEnglish()) {
                    let format = '[(]YYYY/MM/DD[)]';
                    $('.from-date-time').text(start.format(format));
                    $('.to-date-time').text(end.format(format));
                }
                else {
                    let format = '(YYYY/MM/DD)';
                    $('.from-date-time').text(new persianDate(start.toDate()).format(format));
                    $('.to-date-time').text(new persianDate(end.toDate()).format(format));
                }
                $('#eventModal').data('event', {
                    mode: 'add',
                    start: start,
                    end: end,
                    allDay: allDay
                }).modal('show');
            },
            eventDragStart: function (event, jsEvent, ui, view) {
                page.dragginData.eventStartPoint = event;
                window.navigator.vibrate(200);
            },
            eventDrop: function (event, delta, revertFunc, jsEvent, ui, view) {
                if (page.dragginData.overlapOccurred == true) {
                    if (event.end == null) {
                        revertFunc();
                        return;
                    }
                    if (page.validateTimes(event.start, event.end, event.id, false) == false) {
                        revertFunc();
                        if (page.dragginData.eventStartPoint.start.isBefore(page.dragginData.eventTargetPoint.start)) {
                            event.start = page.dragginData.eventStartPoint.start;
                            event.end = page.dragginData.eventTargetPoint.start.add(-1, 'seconds');
                        }
                        else {
                            event.start = page.dragginData.eventTargetPoint.end.add(1, 'seconds');
                            event.end = page.dragginData.eventStartPoint.end;
                        }
                        if (page.validateTimes(event.start, event.end, event.id, false)) {
                            $('#calendar').fullCalendar('updateEvent', event);
                        }
                    }
                    page.dragginData.overlapOccurred = false;
                }
                if (page.autoSave) page.saveData();
            },
            eventResize: function (event, delta, revertFunc, jsEvent, ui, view) {
                if (page.dragginData.overlapOccurred == true) {
                    if (page.validateTimes(event.start, event.end, event.id, false) == false) {
                        revertFunc();

                        event.end = page.dragginData.eventTargetPoint.start.add(-1, 'seconds');
                        if (page.validateTimes(event.start, event.end, event.id, false)) {
                            $('#calendar').fullCalendar('updateEvent', event);
                        }
                    }
                    page.dragginData.overlapOccurred = false;
                }
                if (page.autoSave) page.saveData();
            },
            eventOverlap: function (stillEvent, movingEvent) {
                page.dragginData.eventTargetPoint = stillEvent;
                page.dragginData.overlapOccurred = true;
                return true;
            },
            selectOverlap: function (event) {
                return false;
            },
            dayClick: function (date, jsEvent, view) {
            },
        });

        $('.fc-day').on('click', function (e) {
            var c = $(e.target);
            var day;
            if (c.hasClass('fc-sat'))
                day = 'saturday';
            else if (c.hasClass('fc-sun'))
                day = 'sunday';
            else if (c.hasClass('fc-mon'))
                day = 'monday';
            else if (c.hasClass('fc-tue'))
                day = 'tuesday';
            else if (c.hasClass('fc-wed'))
                day = 'wednesday';
            else if (c.hasClass('fc-thu'))
                day = 'thursday';
            else if (c.hasClass('fc-fri'))
                day = 'friday';
            var startDate = page.normilizeDate(day, 0, 0);
            var endDate = moment(startDate).add(1, 'd');

            if (page.IsAllowedToCreateEventForDay(startDate))
                $('#calendar').fullCalendar('select', startDate, endDate);
            else
                teleboard.ui.showAlert(teleboard.strings.CanNotCreateEventForAllDay, teleboard.strings.invalidOperation, 'error');
        });
    },

    IsAllowedToCreateEventForDay: function (date) {
        var events = $('#calendar').fullCalendar('clientEvents');
        for (var i = 0; i < events.length; i++) {
            var e = events[i];
            if (e.start.day() === date.day() || (e.start.isBefore(date) && e.end.isAfter(date))) {
                return false;
            }
        }
        return true;
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
        var offsetTellorance = 0;
        if (teleboard.ui.IsEnglish() == false) {
            offsetTellorance = 1;
        }

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

        offset = offset + offsetTellorance;
        return sunday.add(offset, 'd')
            .hour(hour)
            .minute(minute)
            .second(0);
    },

    validateModal: function (fromTime, toTime, channel) {
        var from = moment(fromTime.val().trim(), 'HH:mm', true);
        var to = moment(toTime.val().trim(), 'HH:mm', true);

        if (!from.isValid()) {
            teleboard.ui.showAlert(teleboard.strings.enterAValidTime, teleboard.strings.invalidTime, 'error');
            fromTime.focus();
            return false;
        }
        if (!to.isValid()) {
            teleboard.ui.showAlert(teleboard.strings.enterAValidTime, teleboard.strings.invalidTime, 'error');
            toTime.focus();
            return false;
        }

        if (typeof (channel.val()) === 'undefined') {
            teleboard.ui.showAlert(teleboard.strings.pleaseSelectAChannel, '', 'error');
            return false;
        }
        return true;
    },

    validateTimes: function (start, end, eventId, showAlert = true) {
        if (end == null)
            end = start.add(1, 'days');
        if (end.isBefore(start)) {
            if (showAlert) {
                teleboard.ui.showAlert(teleboard.strings.endTimeIsBeforeStartTime, '', 'error');
            }
            return false;
        }
        var events = $('#calendar').fullCalendar('clientEvents');

        for (var i = 0; i < events.length; i++) {
            var e = events[i];
            if (e.id !== eventId) {
                if (start.isBefore(e.end) && end.isAfter(e.end)) {
                    if (showAlert) {
                        teleboard.ui.showAlert(teleboard.strings.eventStartTimeHasConflict, teleboard.strings.invalidStartTime, 'error');
                    }
                    return false;
                }

                if (end.isAfter(e.start) && end.isBefore(e.end)) {
                    if (showAlert) {
                        teleboard.ui.showAlert(teleboard.strings.eventEndTimeHasConflict, teleboard.strings.invalidEndTime, 'error');
                    }
                    return false;
                }
            }
        }
        return true;
    },

    highlightCurrentTime: function () {
        var now = new Date();
        var hour = now.getHours();
        var min = now.getMinutes();
        var dateTimeString = (hour >= 10 ? hour.toString() : '0' + hour) + ':' + (min < 30 ? '00' : '30') + ':00';
        var row = $('tr[data-time="{0}"]'.replace('{0}', dateTimeString));
        row.children().eq(0).css('background-color', '#f8cb00');

        var timeIndicator = $('#timeIndicator').show().css('top', ((min % 30) / 30) * row.height());
        row.children().eq(1).css('position', 'relative').append(timeIndicator);
    },

    saveData: function () {
        var events = $('#calendar').fullCalendar('clientEvents').map(function (e) {
            var format = 'YYYY[-]MM[-]DD HH:mm:ss';
            return {
                Id: e.id,
                Start: e.start.format(format).toEnglishNumber(),
                End: (e.end == null) ? e.start.add(1, 'days').format(format).toEnglishNumber() : e.end.format(format).toEnglishNumber(),
                ChannelId: e.ChannelId,
                PlayAdvertisement: e.timeBox.PlayAdvertisement
            };
        });

        $('.btn-modal').attr('disabled', '');
        return $.ajax({
            url: "/TimeBoxes/SaveEvents",
            type: "Post",
            data: {
                deviceIds: page.deviceIds,
                events: events,
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

    onRemoveAllSchedulingsClick: function () {
        swal({
            title: teleboard.strings.removeAllSchedulings,
            text: teleboard.strings.thisOperationIsNotUndoable,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: teleboard.strings.yes,
            cancelButtonText: teleboard.strings.no,
            closeOnConfirm: false,
            closeOnCancel: true
        }, function (isConfirm) {
            if (!isConfirm) return;
            $('#calendar').fullCalendar('removeEvents');

            if (page.autoSave) {
                page.saveData()
                    .done(function () {
                        swal.close();
                    });
            } else {
                swal.close();
            }
        });
    },

    onSaveSchedulingClick: function () {
        swal({
            title: teleboard.strings.areYouSure,
            text: teleboard.strings.schedulingWillApplyToAllDevices,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: teleboard.strings.yes,
            cancelButtonText: teleboard.strings.no,
            closeOnConfirm: false,
            closeOnCancel: true
        }, function (isConfirm) {
            if (!isConfirm) return;
            page.saveData().done(function () {
                swal.close();
            });
        });
    }
};
