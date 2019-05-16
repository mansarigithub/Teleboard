$(function () {
    teleboard.page.initialize();
});

teleboard.page = {
    uploadHandler: null,
    isUploading: false,
    strings: {
        invalidFileTypeMsg: 'Selected file is not supported',
        invalidFileTypeTitle: 'Invalid file'
    },
    templates: {
    },
    initialize: function () {
        teleboard.page.initialUpload();
    },

    initialUpload: function () {
        $("#FileStream").prop("accept", "video/*,image/*");

        $("#FileStream").change((evt) => { teleboard.page.validateMimeType(evt) });
    },
    validateMimeType: (evt) => {
        let file = event.target.files[0]

        let filereader = new FileReader()

        filereader.onloadend = function (evt) {
            if (evt.target.readyState === FileReader.DONE) {
                let uint8Array = new Uint8Array(evt.target.result)
                let bytes = []
                uint8Array.forEach((byte) => {
                    bytes.push(byte.toString(16))
                })
                let signature = bytes.join('').toUpperCase()

                if (teleboard.page.getMimetype(signature) == 'Unknown')
                {
                    teleboard.ui.showMessage(teleboard.page.strings.invalidFileTypeMsg, teleboard.page.strings.invalidFileTypeTitle, 'error');
                    let tanentId = $('#TenantId').val();
                    let description = $('#Description').val();

                    // reset input file
                    var $el = $('#FileStream');
                    $el.wrap('<form>').closest('form').get(0).reset();
                    $el.unwrap();

                    // reload form previous state
                    $('#TenantId').val(tanentId);
                    $('#Description').val(description);
                }
            }
        }


        let signatureBytes = file.slice(0, 4);
        filereader.readAsArrayBuffer(signatureBytes);
    },
    getMimetype: (signature) => {
        switch (signature) {
            case '89504E47':
                return 'image/png'
            case '47494638':
                return 'image/gif'
            case 'FFD8FFE1':
            case 'FFD8FFE0':
            case 'FFD8FFDB':
                return 'image/jpeg'
            case '424D8644':
            case '424DA6C5':
            case '424DC6E4':
            case '424D1EBB':
                return 'image/bmp'
            case '00018':
            case '00020':
            case "00000018":
            case "00000020":
                return 'video/mp4'
            default:
                return 'Unknown'
        }
    },
    onStartUploadButtonClick: function () {
        if (!teleboard.page.validateForm())
            return;
        var uploadProgress = $('#uploadProgress');
        $('#FileStream').simpleUpload("/Contents/Create", {
            allowedTypes: ["image/jpeg", "image/png", "image/gif", "image/bmp", "video/mp4"],
            start: function (file) {
                teleboard.page.enableUploadForm(false);
                teleboard.page.uploadHandler = this;
                teleboard.page.isUploading = true;
            },
            progress: function (progress) {
                var prog = Math.round(progress), text;
                text = prog < 100 ? 'Uploading ... (' + prog + '%)' : teleboard.strings.processingContent;
                uploadProgress.text(text);
            },
            success: function (data) {
                teleboard.page.resetForm();
                teleboard.page.enableUploadForm(true);
                uploadProgress.text(teleboard.strings.successfullyUploaded);
                teleboard.page.isUploading = false;
                //teleboard.ui.showMessage(teleboard.strings.fileUploaded, teleboard.strings.success, 'success');
            },
            error: function (error) {
                teleboard.page.enableUploadForm(true);
                uploadProgress.text(teleboard.strings.errorOccured);
                teleboard.page.isUploading = false;
            },
            cancel: function (error) {
                teleboard.page.enableUploadForm(true);
                uploadProgress.text(teleboard.strings.uploadCanceled);
                teleboard.page.isUploading = false;
            },
            data: {
                TenantId: $('#TenantId').val(),
                Description: $('#Description').val(),
                __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val()
            },
        });
    },

    onCancelUploadButtonClick: function () {
        teleboard.page.uploadHandler.upload.cancel();
    },

    validateForm: function () {
        return $("#uploadForm").validate().form();
    },

    enableUploadForm: function (enable) {
        var form = $('#uploadForm');
        var formElements = $('#startButton, input[type=text], input[type=file], select', form);
        if (enable) {
            formElements.removeAttr('disabled');
            $('#cancelButton').hide();
        }
        else {
            formElements.attr('disabled', '');
            $('#cancelButton').show();
        }
    },

    resetForm: function () {
        $('input[type=text]', $('#uploadForm')).val('');
    }
}

window.onbeforeunload = function (e) {
    if (teleboard.page.isUploading)
        return false;
}
