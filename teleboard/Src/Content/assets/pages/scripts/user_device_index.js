$(function () {
    teleboard.page.initialize();
});

teleboard.page = {
    grid: null,
    templates: {
        actionButtons: null,
        onlineTagTemplate: null,
        playTemplate: null,
    },
    initialize: function () {
        $('#groupSchedulingButton').click(teleboard.page.onGroupSchedulingClick);
        this.templates.actionButtons = $('#actionButtonsTemplate');
        this.templates.onlineTagTemplate = $('#onlineTagTemplate');
        this.templates.playTemplate = $('#playTemplate');
        this.grid = this.initialGrid();


    },

    initialGrid: function () {
        var autoUpdateEnabled = false;
        $('#grid').hide();
        var grid = $('#grid').grid({
            dataSource: '/Scheduler/ReadDevices',
            primaryKey: 'Id',
            detailTemplate: '<div></div>',
            responsive: true,
            //headerFilter: true,
            locale: teleboard.ui.IsEnglish() ? 'en-us' : 'fa-ir',
            showHiddenColumnsAsDetails: true,
            uiLibrary: "bootstrap4",
            pager: { limit: 20, sizes: [10, 20, 30, 40, 50, 100] },
            iconsLibrary: 'fontawesome',
            selectionType: 'multiple',
            selectionMethod: 'checkbox',
            columns: [
                {
                    field: 'Name',
                    title: teleboard.strings.name,
                    sortable: true,
                    filterable: true,
                    renderer: function (value, record) {
                        var icon = '<span class="fa fa-television online-icon" title="Online"></span>';
                        var deviceLink = '<a href="/Scheduler/Edit/{Id}">{Name}</a>'.replace('{Id}', record.Id).replace('{Name}', record.Name);
                        return record.IsOnline ? icon + deviceLink : deviceLink;
                    },
                    align: teleboard.strings.align
                },
                {
                    field: 'TenantName',
                    title: teleboard.strings.tenant,
                    sortable: true,
                    priority: 1,
                    filterable: false,
                    align: teleboard.strings.align
                },
                {
                    field: 'CurrentPlayingChannelName',
                    title: teleboard.strings.scheduledChannel,
                    tmpl: '<a href="/Channels/Contents/{CurrentPlayingChannelId}">{CurrentPlayingChannelName}</a>',
                    //priority: 2,
                    filterable: false,
                    Width: 50,
                    align: teleboard.strings.align
                },
                {
                    field: 'DeviceId',
                    title: teleboard.strings.ID,
                    minWidth: 100,
                    priority: 3,
                    align: teleboard.strings.align
                },
                {
                    title: teleboard.strings.timeZone,
                    field: 'TimeZoneId',
                    sortable: true,
                    priority: 4,
                    filterable: false,
                    minWidth: 100,
                    align: teleboard.strings.align
                },
                {
                    title: teleboard.strings.lastConnectivity,
                    field: 'LastConnectedString',
                    sortable: true,
                    priority: 5,
                    filterable: false,
                    minWidth: 10000,
                    align: teleboard.strings.align
                },
                {
                    field: 'Description',
                    title: teleboard.strings.description,
                    sortable: true,
                    priority: 6,
                    filterable: false,
                    minWidth: 10000,
                    align: teleboard.strings.align
                },
                {
                    field: 'Id',
                    title: '. . .', // teleboard.strings.scheduling,
                    tmpl: '<a href="/TimeBoxes/Index/{Id}" title="Schedule device play list"><span class="fa fa-calendar fa-2x"></span></a>',
                    filterable: false,
                    align: teleboard.strings.align,
                    width: 50
                },
            ]
        });

        grid.on('dataBound', function (e, records, totalRecords) {
            $('#grid').show();
            if (!autoUpdateEnabled) {
                teleboard.page.enableGridAutoUpdate();
                autoUpdateEnabled = true;
            }
        });

        grid.on('rowSelect', function (e, $row, id, record) {
            if (grid.getSelections().length > 1) {
                $('#groupSchedulingButton').removeClass('disabled');
            }
        });

        grid.on('rowUnselect', function (e, $row, id, record) {
            if (grid.getSelections().length < 2) {
                $('#groupSchedulingButton').addClass('disabled');
            }
        });

        $('#grid').show(2000);
        return grid;
    },

    enableGridAutoUpdate: function () {
        var timeoutFunc = function () {
            teleboard.page.updateGridData()
                .done(function () {
                    setTimeout(timeoutFunc, 5000);
                });
        }
        setTimeout(timeoutFunc, 3000);
    },

    onGroupSchedulingClick: function () {
        var grid = $('#grid').grid();
        var selectedDeviceIds = grid.getSelections();
        if (selectedDeviceIds.length == 0) return;
        var selectedTenantIds = selectedDeviceIds.map(function (value, index) {
            return grid.getById(value).TenantId;
        });

        var first = selectedTenantIds[0];
        for (var i = 0; i < selectedTenantIds.length; i++) {
            if (selectedTenantIds[i] !== first) {
                teleboard.ui.showAlert(teleboard.strings.selectDevicesFromSameTanentMsg, teleboard.strings.selectDevicesFromSameTanent, 'error');
                return;
            }
        }

        window.location = '/timeboxes/index/' + selectedDeviceIds.join(',');
    },

    updateGridData: function () {
        return $.ajax({
            url: "/Scheduler/ReadDevices",
            type: "Get",
            data: {
                includeDetails: true
            },
            success: function (data) {
                var grid = teleboard.page.grid;
                var selectedRows = grid.getSelections();
                grid.render(data.records);
                selectedRows.forEach(function (item, index) {
                    grid.setSelected(item);
                });
            }
        });
    }
}
