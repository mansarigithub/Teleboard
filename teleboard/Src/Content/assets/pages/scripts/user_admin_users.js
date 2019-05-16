$(function () {
    teleboard.page.initialize();
});

teleboard.page = {
    strings: {
        areYouSureToDelete: 'are you sure to delete this user?',
        passwordResetLinkSent: 'Email with password reset link sent',
    },
    templates: {
    },
    initialize: function () {

    },

    onDeleteUserClick: function () {
        return confirm(teleboard.page.strings.areYouSureToDelete);
    },

    onSendResetPasswordClick: function (userId) {
        $.ajax({
            type: "POST",
            url: "/Admin/ResetPassword",
            data: {
                id: userId,
            },
            success: function (data) {
                alert(teleboard.page.strings.passwordResetLinkSent);
            }
        });
    },
}
