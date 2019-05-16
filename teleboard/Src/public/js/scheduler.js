calendarChanged: false;

$(document).ready(function () {
    initScheduler($('#deviceId').val());

    $(".dropdown").click(function () {
        if ($(this).hasClass("open")) {
            $(this).removeClass("open");
        }
        else {
            $(this).addClass("open");
        }
    });
});

function initScheduler(deviceId) {
    var request =
        $.ajax({
            url: "/Devices/" + deviceId + "/TimeBoxes/SchedulerData/",
            type: "Get",
            cache: false
        });

    request.done(function (data) {
        if (data.Successful) {

            ///////////
            var request2 =
                $.ajax({
                    url: "/Devices/" + deviceId + "/TimeBoxes/SchedulerTaskData/",
                    type: "Get",
                    cache: false
                });

            request2.done(function (data2) {

                if (data2.Successful) {
                    var arrColors = ['aqua', 'lightcoral', 'grey', 'fuchsia', 'green', 'lime', 'mediumaquamarine', 'crimson', 'olive', 'orange', 'rebeccapurple', 'lightblue', 'silver', 'teal', 'yellow'];

                    for (var j = 0; j < data2.Channels.length; j++) {
                        data2.Channels[j].color = arrColors[j % arrColors.length];
                    }

                    initInternal(data.TimeSheet, deviceId, data2.Channels);
                } else {

                    alert(data2.Message);
                }

            });

            request2.fail(function (jqXHR2, textStatus2) {
                alert("Error occured! Error text status : " + textStatus2);
            });
            ///////////

        } else {
            alert(data.Message);
        }
    });

    request.fail(function (jqXHR, textStatus) {
        alert("Error occured! Error text status : " + textStatus);
    });
}

function fixTimeNumbers(num) {
    if (num < 10) {
        return "0" + num
    }
    return num;
}

function initInternal(scheduleInput, deviceId, listOfAllAvailableChannels) {
    var scheduleJson = [];
    var channelList = [];
    var daysOfWeek = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

    for (var i = 0; i < scheduleInput.length; i++) {
        scheduleTimeBox = scheduleInput[i].TimeBox;
        channelList = scheduleInput[i].Channel;
        var dateStart = moment(scheduleTimeBox.WeekDay);
        var dow = dateStart.day();

        scheduleJson.push({
            timeBoxId: scheduleTimeBox.Id,
            id: scheduleJson.length,
            title: channelList.Name,
            start: daysOfWeek[dow] + ' ' + fixTimeNumbers(scheduleTimeBox.FromHour) + ':' + fixTimeNumbers(scheduleTimeBox.FromMinute),
            end: daysOfWeek[dow] + ' ' + fixTimeNumbers(scheduleTimeBox.ToHour) + ':' + fixTimeNumbers(scheduleTimeBox.ToMinute)
        });
    }



    function updateServer(data) {
        var newData = [];
        jQuery.extend(true, newData, data);

        for (var i = 0; i < newData.length; i++) {
            if (newData[i]['start'].length && newData[i]['end'].length) {
                newData[i]['start'] = daysOfWeek[new Date(newData[i]['start'].split(' ')[0]).getDay()] + ' ' + scheduleJson[i]['start'].split(' ')[1];
                newData[i]['end'] = daysOfWeek[new Date(newData[i]['end'].split(' ')[0]).getDay()] + ' ' + scheduleJson[i]['end'].split(' ')[1];
            }
        }

        return newData;
    }


    (function () {
        /**
		 * Colors array, you can change or add new colors
		 * @type {string[]}
		 */
        var arrColors = ['aqua', 'lightcoral', 'grey', 'fuchsia', 'green',
            'lime', 'mediumaquamarine', 'crimson', 'olive', 'orange', 'rebeccapurple', 'lightblue',
            'silver', 'teal', 'yellow'];

        var templater = function (template, data) {
            var html = '';

            for (var k in data) {
                html = template.split('{{' + k + '}}').join(data[k]);
                template = html;
            }
            return html;
        };

        var generateDate = function (indexDay) {
            var mD = moment().day(indexDay),
                date = mD.toDate(),
                values;

            date = date.setTime(date.getTime());

            date = new Date(date);

            values = [date.getDate(), date.getMonth() + 1];
            for (var id in values) {
                values[id] = values[id].toString().replace(/^([0-9])$/, '0$1');
            }

            return date.getFullYear() + '-' + values[1] + '-' + values[0];


        };


        /**
		 * html template for list of task
		 * @type {string}
		 */
        var editViewListHtml = '<ul class="bottom-ul list-unstyled">',
            listItem = '<li>' +
                '<label class="radio-inline">' +
                '<input type="radio" name="optradio" class="{{NameClass}}" data-task="{{NameClass}}" data-color="{{color}}">&nbsp;' +
                '<span class="btn custom-button" style="background:{{color}};width:257px">{{Name}}</span>' +
                '</label>' +
                '</li><br>';

        /**
		 * loop which generate list of task
		 */
        for (var i = 0; i < scheduleJson.length; i++) {
            if (daysOfWeek.indexOf(scheduleJson[i]['start'].split(' ')[0]) != -1 && daysOfWeek.indexOf(scheduleJson[i]['end'].split(' ')[0]) != -1) {
                scheduleJson[i]['start'] = generateDate(daysOfWeek.indexOf(scheduleJson[i]['start'].split(' ')[0])) + ' ' + scheduleJson[i]['start'].split(' ')[1];
                scheduleJson[i]['end'] = generateDate(daysOfWeek.indexOf(scheduleJson[i]['end'].split(' ')[0])) + ' ' + scheduleJson[i]['end'].split(' ')[1];
            }

            //if (scheduleJson.length <= arrColors.length) {
            //    scheduleJson[i]['color'] = arrColors[i];
            //} else {
            //    scheduleJson[i]['color'] = arrColors[i % (arrColors.length)];
            //}
            //editViewListHtml += templater(listItem, scheduleJson[i]);
        }
        var timeAndColor = [];
        for (var i = 0; i < listOfAllAvailableChannels.length; i++) {
            item = {};
            item["NameClass"] = listOfAllAvailableChannels[i].Name.replace(/\s/g, '');
            item["Name"] = listOfAllAvailableChannels[i].Name;
            item["color"] = listOfAllAvailableChannels[i].color;
            timeAndColor.push(item);
        }

        for (var i = 0; i < listOfAllAvailableChannels.length; i++) {
            editViewListHtml += templater(listItem, timeAndColor[i]);
        }

        editViewListHtml += '</ul>';
        $('#EditView').html(editViewListHtml);


        (function (d) {
            var
                ce = function (e, n) {
                    var a = document.createEvent("CustomEvent");
                    a.initCustomEvent(n, true, true, e.target);
                    e.target.dispatchEvent(a);
                    a = null;
                    return false
                },
                nm = true, sp = { x: 0, y: 0 }, ep = { x: 0, y: 0 },
                touch = {
                    touchstart: function (e) {
                        sp = { x: e.touches[0].pageX, y: e.touches[0].pageY }
                    },
                    touchmove: function (e) {
                        nm = false;
                        ep = { x: e.touches[0].pageX, y: e.touches[0].pageY }
                    },
                    touchend: function (e) {
                        if (nm) {
                            ce(e, 'fc')
                        } else {
                            var x = ep.x - sp.x, xr = Math.abs(x), y = ep.y - sp.y, yr = Math.abs(y);
                            if (Math.max(xr, yr) > 20) {
                                ce(e, (xr > yr ? (x < 0 ? 'swl' : 'swr') : (y < 0 ? 'swu' : 'swd')))
                            }
                        }
                        ;
                        nm = true
                    },
                    touchcancel: function (e) {
                        nm = false
                    }
                };
            for (var a in touch) {
                d.addEventListener(a, touch[a], false);
            }
        })(document);

        document.body.addEventListener('swr', function (e) {
            if ($('.fc-agendaDay-view').length) {
                $('.day-checked').prev().click();
            }
        }, false);
        document.body.addEventListener('swl', function (e) {
            if ($('.fc-agendaDay-view').length) {
                $('.day-checked').next().click();
            }
        }, false);

    })();


    var cr;
    var calendar = $('#calendar').fullCalendar({
        header: {
            left: '',
            right: 'agendaWeek, agendaDay'
        },
        navLinks: true,
        aspectRatio: 1.0,
        scrollTime: '12:00 AM',
        editable: true,
        defaultView: 'agendaWeek',
        eventLimit: true,
        allDay: false,
        columnFormat: 'ddd',
        axisFormat: 'HH:mm',
        selectable: true,
        viewRender: function (view) {
            setTimeout(function () {
                if (view.name === 'agendaDay') {
                    $('.fc-view.fc-agendaDay-view.fc-agenda-view .fc-row.fc-widget-header tr th').each(function () {
                        if ($(this).html().length) {
                            var gv = 'fc-' + moment(view.title, 'MMMM DD, YYYY').format('ddd').toLowerCase();
                            $(this).find('a').removeAttr('data-goto');
                            $(this).find('a').attr('style', 'color:#ddd');
                            if ($(this).hasClass(gv)) {
                                $('.fc-view.fc-agendaDay-view.fc-agenda-view .fc-row.fc-widget-header tr th').removeClass('day-checked');
                                $(this).addClass('day-checked');
                            }
                            $(this).unbind('click');
                            $(this).click(function () {
                                var f = $('.fc-view.fc-agendaDay-view.fc-agenda-view .fc-row.fc-widget-header tr').html();

                                var d = new Date($(this).data('date'));
                                var dateWithTimeZone = d.setTime(d.getTime() + d.getTimezoneOffset() * 60 * 1000);

                                $('#calendar').fullCalendar('gotoDate', new Date(dateWithTimeZone));
                                if ($(this).hasClass('day-checked')) {
                                    $('#calendar').fullCalendar('changeView', 'agendaWeek');
                                    $('.weekView').prop('checked', true);
                                } else {
                                    $('#calendar').fullCalendar('changeView', 'agendaDay');
                                    $('.fc-view.fc-agendaDay-view.fc-agenda-view .fc-row.fc-widget-header table thead tr').html(f);
                                    $('.dayView').prop('checked', true);
                                }

                            });
                        }
                    });
                }
            }, 0);

        },
        navLinkDayClick: function (date, jsEvent) {
            var f = $('.fc-view.fc-agendaWeek-view.fc-agenda-view .fc-row.fc-widget-header tr').html();
            $('#calendar').fullCalendar('changeView', 'agendaDay');
            $('#calendar').fullCalendar('gotoDate', date);
            $('.fc-view.fc-agendaDay-view.fc-agenda-view .fc-row.fc-widget-header table thead tr').html(f);
            $('.dayView').prop('checked', true);
        },
        eventRender: function (event, element) {
            $('#addEventModal').data('event', event);
            element.bind('dblclick', function () {
                $('#addEventModal').modal('show');

                $('#fromTime').timepicker('setTime', moment(event.start).format('HH:mm A'));

                var ntime = moment(event.end, 'HH:mm A').format('HH:mm');
                if (ntime == '23:59') {
                    $('#toTime').timepicker('setTime', '00:00 AM');
                } else {
                    $('#toTime').timepicker('setTime', moment(event.end).format('HH:mm A'));
                }

                $('#exampleInputEmail1').text(event.title).css('background', event.color);
                $('#addEventModal').data('event', event);

                if (event.title) {
                    $('.' + event.title).click();
                }

                $('input:radio').each(function () {
                    //debugger;
                    var $this = $(this);
                    var showTask = $(this).next('span').text();
                    var taskName = $this.data('task');
                    var colorName = $this.data('color');
                    if (taskName) {
                        $('body').on('click', '.' + taskName, function () {
                            $('#exampleInputEmail1').text(showTask).css('background', colorName);
                            cr = colorName;
                        })
                    }
                });

            });
        },
        select: function (start, end, allDay) {
            $('#addEventModal').modal('show');

            $('input:radio').each(function () {
                var $this = $(this);
                var showTask = $(this).next('span').text();
                var taskName = $this.data('task');
                var colorName = $this.data('color');
                if (taskName) {
                    $('body').on('click', '.' + taskName, function () {
                        $('#exampleInputEmail1').text(showTask).css('background', colorName);
                        cr = colorName;
                    })
                }
            });

            var title = $('#exampleInputEmail1').text();
            $('#fromTime').timepicker('setTime', moment(start).format('HH:mm A'));
            $('#toTime').timepicker('setTime', moment(end).format('HH:mm A'));

            var addData;
            // if (title) {
            addData = {
                id: scheduleJson.length + 1,
                title: title,
                start: moment(start).format('YYYY-MM-DD HH:mm'),
                end: moment(end).format('YYYY-MM-DD HH:mm')
            };

            scheduleJson.push(addData);
            $('#calendar').fullCalendar('renderEvent', addData, true);
            $('#calendar').fullCalendar('unselect');
            // }
            //     // --
            //     // POST request send in webservice

            console.log('Function updateServer Add Json Object ', updateServer(scheduleJson));
            console.log('Add Json Object ', scheduleJson);
        },
        eventDrop: function (event, delta, minuteDelta, allDay, revertFunc) {
            calendarChanged = true;

            for (var key in scheduleJson) {
                if (scheduleJson[key].id == event.id) {
                    scheduleJson.splice(key, 1);
                }
            }

            var setEvent = {};
            setEvent.id = event.id;
            setEvent.title = event.title;
            setEvent.start = moment(event.start.format()).format('YYYY-MM-DD HH:mm');
            setEvent.end = moment(event.end.format()).format('YYYY-MM-DD HH:mm');
            setEvent.color = event.color;

            scheduleJson.push(setEvent);
            $('#calendar').fullCalendar('rendarEvent', setEvent);

            // --
            // Drag and Drop request send in webservice
            console.log('Function updateServer Drag and Drop ', updateServer(scheduleJson));
            console.log('Drag and Drop ', scheduleJson);

        },
        eventResize: function (event, delta, minuteDelta, allDay, revertFunc) {
            calendarChanged = true;

            for (var key in scheduleJson) {
                if (scheduleJson[key].id == event.id) {
                    scheduleJson.splice(key, 1);
                }
            }

            var setEvent = {};
            setEvent.id = event.id;
            setEvent.title = event.title;
            setEvent.start = moment(event.start.format()).format('YYYY-MM-DD HH:mm');
            setEvent.end = moment(event.end.format()).format('YYYY-MM-DD HH:mm');
            setEvent.color = event.color;

            scheduleJson.push(setEvent);
            $('#calendar').fullCalendar('rendarEvent', setEvent);

            // --
            // Drag and Drop request send in webservice
            console.log('Function updateServer Drag and Drop ', updateServer(scheduleJson));
            console.log('Drag and Drop ', scheduleJson);

        },
        eventOverlap: function(stillEvent, movingEvent) {
            return false;
        },
        selectOverlap: function(event) {
            return false;
        },
        events: scheduleJson
    });

    //////////////////////

    $('#saveAllDataToServer').click(function (e) {

        e.preventDefault();
        var request =
            $.ajax({
                url: "/Devices/" + deviceId + "/TimeBoxes/SaveSchedulerData/",
                type: "Post",
                data: { "timeBoxList": scheduleJson }
            });

        request.done(function (data) {

            if (data.Successful) {
                calendarChanged = false;
                alert("done!");
            } else {
                alert(data.Message);
            }

        });

        request.fail(function (jqXHR, textStatus) {
            alert("Error occured! Error text status : " + textStatus);
        });


    })


    /**************************
	 Edit Task
	 */
    $('.editEventBtn').click(function (e) {
        calendarChanged = true;
        e.preventDefault();

        var title = $('#exampleInputEmail1').text();
        if (!title) {
            alert('Select task for list below');
            return;
        }

        var setEvent = $('#addEventModal').data('event');

        var start;
        var end;
        if ($('.from-time').val()) {
            var date = moment(setEvent.start).format('YYYY-MM-DD');
            var time = moment($('.from-time').val(), 'HH:mm A').format('HH:mm');
            var d = date + ' ' + time;
            start = moment(d).format('YYYY-MM-DD HH:mm');
        } else {
            start = setEvent.start;
        }

        if ($('.to-time').val()) {
            var edate = moment(setEvent.end).format('YYYY-MM-DD');
            var etime = moment($('.to-time').val(), 'HH:mm A').format('HH:mm');
            if (etime == '00:00') {
                etime = '23:59';
            }
            var dd = edate + ' ' + etime;

            //end = moment(dd).format('YYYY-MM-DD kk:mm');
            end = dd;
        } else {
            end = setEvent.end;
        }

        var ff = moment($('.to-time').val(), 'HH:mm A').format('HH:mm');
        if (ff == '00:00') {
            ff = '23:59';
        }

        //if (moment($('.from-time').val(), 'HH:mm A').format('HH:mm') > ff) {
        //    alert('start or end time wrong for today');
        //    return;
        //}
        //debugger;

        setEvent.title = title;
        setEvent.start = start;
        setEvent.end = end;
        setEvent.color = (cr) ? cr : setEvent.color;


        $('#calendar').fullCalendar('updateEvent', setEvent);

        for (var key in scheduleJson) {
            if (scheduleJson[key].id == setEvent.id) {
                scheduleJson[key].id = setEvent.id;
                scheduleJson[key].title = title;
                scheduleJson[key].start = start;
                scheduleJson[key].end = end;
            }
        }

        // --
        // PUT request send in webservice
        console.log('Function updateServer Update Json Object ', updateServer(scheduleJson));
        console.log('Update Json Object ', scheduleJson);

        $('#addEventModal').modal('hide');
        $('#exampleInputEmail1').text('');
        $('#exampleInputEmail1').attr('style', 'background:none');
        $('.from-time').val('');
        cr = '';
        $('.to-time').val('');
        start = '';
        end = '';
    });


    /**************************
	 Delete Task
	 */

    $('.deleteEventBtn').click(function (e) {
        calendarChanged = true;
        e.preventDefault();

        var deleteEvent = $('#addEventModal').data('event');
        if ($('#exampleInputEmail1').text() == deleteEvent.title) {
            for (var key in scheduleJson) {
                if (scheduleJson[key].id == deleteEvent.id) {
                    scheduleJson.splice(key, 1);
                }
            }
            $('#calendar').fullCalendar('removeEvents', deleteEvent.id);
        }

        $('#addEventModal').modal('hide');
        $('#exampleInputEmail1').text('');
        $('#exampleInputEmail1').attr('style', 'background:none');
    });

    $('.cancelBtn').click(function () {
        $('#exampleInputEmail1').attr('style', 'background:none');
        $('#exampleInputEmail1').text('');

        var cancelEvent = $('#addEventModal').data('event');
        if (!cancelEvent.title) {
            for (var key in scheduleJson) {
                if (scheduleJson[key].id == cancelEvent.id) {
                    scheduleJson.splice(key, 1);
                }
            }
            $('#calendar').fullCalendar('removeEvents', cancelEvent.id);
        }
    });

    $('.dayView').click(function () {
        var f = $('.fc-view.fc-agendaWeek-view.fc-agenda-view .fc-row.fc-widget-header tr').html();
        $('#calendar').fullCalendar('changeView', 'agendaDay');
        $('#calendar').fullCalendar('gotoDate', new Date());
        $('.fc-view.fc-agendaDay-view.fc-agenda-view .fc-row.fc-widget-header table thead tr').html(f);
        // $('.fc-agendaDay-button.fc-button.fc-state-default.fc-corner-left.fc-corner-right').click();
        $('.dayView').prop('checked', true);

        switchView();
    });

    $('.weekView').click(function () {
        $('.fc-agendaWeek-button.fc-button.fc-state-default.fc-corner-left.fc-corner-right').click();
        switchView();
    });


    var switchView = function () {
        var arrayUIDays = ['mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun'];
        for (var m = 0; m < arrayUIDays.length; m++) {
            var dim = '.fc-day-header.fc-widget-header.fc-' + arrayUIDays[m] + ' a';

            $(dim).click(function () {
                $('.dayView').prop('checked', true);
            });
        }
    };

    switchView();

    // Set color of vertical bar
    highlightCurrentTime();
};

function highlightCurrentTime() {
    var now = new Date();
    var hour = now.getHours();
    var min = now.getMinutes();
    var dateTimeString = (hour >= 10 ? hour.toString() : '0' + hour) + ':' + (min < 30 ? '00' : '30') + ':00';
    var row = $('tr[data-time="{0}"]'.replace('{0}', dateTimeString));
    row.children().eq(0).css('background-color', '#f8cb00');

    var timeIndicator = $('#timeIndicator').show().css('top', ((min % 30) / 30 ) * row.height());
    row.children().eq(1).css('position', 'relative').append(timeIndicator);
}

window.onbeforeunload = function (e) {
    if (calendarChanged)
        return 'Are u sure? you have unsaved changes!';
}