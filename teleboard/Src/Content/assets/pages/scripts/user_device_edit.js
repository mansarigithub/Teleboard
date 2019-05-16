$(function() {
    teleboard.page.initialize();
});

teleboard.page = {
    strings: {
    },

    initialize: function () {
    },

    onDeviceDelete: function (e) {
        return confirm('Are you sure?');
    }
}
