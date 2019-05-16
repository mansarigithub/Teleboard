$(function() {
    teleboard.page.initialize();
});

teleboard.page = {
    strings: {
    },
    templates: {
        actionButtons: null,
    },
    initialize: function () {
        this.initialGrid();
        //$('.delete-articles').click(this.deleteArticles);
        this.templates.actionButtons = $('#actionButtonsTemplate');

    },

    initialGrid: function () {
        $('#grid').grid({
            dataSource: '/Log/ReadLogs',
            detailTemplate: '<div></div>',
            responsive: true,
            showHiddenColumnsAsDetails: true,
            uiLibrary: "fontawesome",
            iconsLibrary: 'fontawesome',
            columns: [
                { field: 'Id', title: 'Id', sortable: true },
                { field: 'TypeString', title: 'Type', sortable: true, priority: 1 },
                { field: 'CreateDateString', title: 'Date', sortable: true, priority: 2 },
                { field: 'Title', title: 'Title', sortable: true, priority: 3 },
                { field: 'Description', title: 'Description', sortable: true, priority: 4 },
                {
                    field: 'Id',
                    title: '',
                    tmpl: '<a href="/Log/Delete/{Id}" title="Delete log"><span class="fa fa-trash gijgo-action"></span></a>',
                },
            ]
        });
    },
}
