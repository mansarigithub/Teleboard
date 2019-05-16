$(function () {
    teleboard.page.initialize();
});

teleboard.page = {
    strings: {
    },
    helpers: {
        replaceAll: function (str, find, replace) {
            return str.replace(new RegExp(find, 'g'), replace);
        }
    },
    templates: {
        actionButtons: null,
    },
    initialize: function () {
        this.initialGrid();
        this.templates.actionButtons = $('#actionButtonsTemplate');
    },

    initialGrid: function () {
        var grid = $('#grid').grid({
            dataSource: '/Channels/ReadChannels',
            detailTemplate: '<div></div>',
            responsive: true,
            showHiddenColumnsAsDetails: true,
            locale: teleboard.ui.IsEnglish() ? 'en-us' : 'fa-ir',
            uiLibrary: "bootstrap4",
            iconsLibrary: 'fontawesome',
            columns: [
                { field: 'Name', title: teleboard.strings.name, sortable: true, tmpl: '<a href="/Channels/Contents/{Id}">{Name}</a>', align: teleboard.strings.align },
                { field: 'TenantName', title: teleboard.strings.tenant, sortable: true, priority: 1, align: teleboard.strings.align },
                { field: 'Description', title: teleboard.strings.description, sortable: true, priority: 2, align: teleboard.strings.align},
                {
                    field: 'Id', align: 'center',
                    title: '. . .', //teleboard.strings.contentManagement_Edit,
                    tmpl: '<a href="/Channels/Edit/{Id}" title="' + teleboard.strings.edit + '"><span class="fa fa-edit fa-2x grid-command"></span></a>' +
                    '<a href="/Channels/Contents/{Id}" title="' + teleboard.strings.ChannelContents + '"><span class="fa fa-picture-o fa-2x grid-command"></span></a>',
                },
            ]
        });

        grid.on('dataBound', function (e, records, totalRecords) {
            $('#grid').show();
        });
    },
}
