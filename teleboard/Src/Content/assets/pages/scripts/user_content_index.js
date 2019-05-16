$(function () {
    teleboard.page.initialize();
});

teleboard.page = {
    grid,
    showApproveButton: false,
    strings: {
    },
    templates: {
        rowTemplate: null,
    },
    initialize: function () {
        teleboard.page.templates.rowTemplate = $('#rowTemplate');
        teleboard.page.showApproveButton = $('#showApproveButton').val() == 'True';
        this.initialGrid();
        $('input.filter').change(teleboard.page.onCheckboxesChange);
    },
    onCheckboxesChange: function () {
        teleboard.page.grid.reload();
    },
    getSelectedResourceTypes: function () {
        return {
            image: $('#imagesCheckbox').is(':checked') ? true : false,
            video: $('#videosCheckbox').is(':checked') ? true : false,
        };
    },
    initialGrid: function () {
        var grid = $('#grid').grid({
            dataSource: {
                url: '/Contents/ReadContents',
                beforeSend: function (jqXHR, settings) {
                    var types = teleboard.page.getSelectedResourceTypes();
                    jqXHR.setRequestHeader('include-images', types.image);
                    jqXHR.setRequestHeader('include-videos', types.video);
                    return true;
                }
            },
            locale: teleboard.ui.IsEnglish() ? 'en-us' : 'fa-ir',
            headerFilter: true,
            uiLibrary: "bootstrap3",
            iconsLibrary: 'fontawesome',
            pager: { limit: 50, sizes: [10, 20, 30, 40, 50, 100] },
            columns: [
                {
                    field: 'Id',
                    title: '',
                    sortable: true,
                    filterable: true,
                    renderer: function (value, record) {
                        return teleboard.page.createRow(record);
                    },
                    align: teleboard.strings.align
                },
            ]
        });

        grid.on('dataBound', function (e, records, totalRecords) {
            teleboard.ui.initContentThumbPreview();
            $('.gj-grid tr[data-role=filter] th input[data-field=Id]').attr('placeholder', teleboard.strings.search);
        });

        grid.on('rowDataBound', function (e, $row, id, record) {
            var thumbnail = $('.thumb-placeholder', $row).contentThumbnail({
                resourceType: record.ResourceTypeName,
                url: record.Url,
                thumbnailUrl: record.ThumbnailUrl,
                contentTypeName: record.ContentTypeName,
            });

            $('.content-tenant-name', $row).text(record.TenantName);
            $('.content-size', $row).text(record.FileSizeString);
            $('.content-description', $row).text(record.Description);
            $('.btn-edit-content', $row).attr('href', '/Contents/edit/' + record.Id);
            $('.btn-delete-content', $row).click(function () { teleboard.page.deleteContent(record.Id); });
            //var statusLabel = $('.content-approval-status', rowTemplate);
            var approveLink = $('.btn-approve-content', $row);

            if (record.Flag) {
                //statusLabel.remove();
                approveLink.remove();
            } else {
                //statusLabel.children('span').text(teleboard.strings.notApproved);
                approveLink.attr('href', '/Contents/Approve/' + record.Id);
            }

        });

        teleboard.page.grid = grid;
        //setTimeout(function () {
        //    $('.gj-grid input').prop('placeholder', teleboard.strings.search);
        //}, 100);
    },

    createRow: function (content) {
        return $(teleboard.page.templates.rowTemplate.clone().html()).prop('outerHTML');
    },

    deleteContent: function (id) {
        swal({
            title: teleboard.strings.deleteThisFile,
            text: teleboard.strings.deleteIsNotUndoable,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: teleboard.strings.yes,
            cancelButtonText: teleboard.strings.no,
            closeOnConfirm: false,
            closeOnCancel: true
        }, function (isConfirm) {
            if (!isConfirm) return;
            $.ajax({
                url: "/Contents/DeleteContent/" + id,
                type: "POST",
                beforeSend: function () {
                    $('button[class=confirm], button[class=cancel]').attr('disabled', 'true');
                },
                success: function (timeboxes) {
                    teleboard.page.grid.reload();
                    swal.close();
                }
            });
        });
    }
}

