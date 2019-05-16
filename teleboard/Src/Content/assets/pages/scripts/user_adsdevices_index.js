$(function () {
    teleboard.page.initialize();
});

teleboard.page = {
    strings: {
    },
    templates: {
        actionButtons: null,
        onlineTagTemplate: null,
        playTemplate: null,
    },
    initialize: function () {
        $('.delete-articles').click(this.deleteArticles);
        this.templates.actionButtons = $('#actionButtonsTemplate');
        this.templates.onlineTagTemplate = $('#onlineTagTemplate');
        this.templates.playTemplate = $('#playTemplate');
        this.initialGrid();
        setInterval(function () { $('#grid').grid().reload() }, 60000);
    },

    initialGrid: function () {
        $('#grid').grid({
            dataSource: '/AdsDevices/ReadDevices',
            detailTemplate: '<div></div>',
            responsive: true,
            //headerFilter: true,
            locale: teleboard.ui.IsEnglish() ? 'en-us' : 'fa-ir',
            showHiddenColumnsAsDetails: true,
            uiLibrary: "fontawesome",
            //pager: { limit: 20, sizes: [10, 20, 30, 40, 50, 100] },
            iconsLibrary: 'fontawesome',
            columns: [
                {
                    field: 'Name',
                    title: teleboard.strings.name,
                    sortable: true,
                    filterable: true,
                    renderer: function (value, record) {
                        var icon = '<span class="fa fa-television online-icon" title="Online"></span>';
                        var deviceLink = '<a href="/AdsTimeBoxes/Index/{Id}">{Name}</a>'.replace('{Id}', record.Id).replace('{Name}', record.Name);
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
                    minWidth: 100,
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
                    field:'Description',
                    title: teleboard.strings.description,
                    sortable: true,
                    priority: 6,
                    filterable: false,
                    minWidth: 10000,
                    align: teleboard.strings.align
                },
                {
                    field: 'Id',
                    title: teleboard.strings.scheduling,
                    tmpl: '<a href="/AdsTimeBoxes/Index/{Id}" title="Schedule Ads"><span class="fa fa-calendar fa-2x"></span></a>',
                    filterable: false,
                    align: teleboard.strings.align
                },
            ]
        });
    },
}
