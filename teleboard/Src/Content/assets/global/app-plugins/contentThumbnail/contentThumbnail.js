/*
Sample:
$("#div1").contentThumb({
    thumbnailUrl : string,
    url : string,
    guid : string,
    id : string,
})
*/

(function ($) {
    $.fn.contentThumbnail = function (config) {
        var resourceType = config.resourceType.toLowerCase();
        var contentThumbDiv = $('<div class="content-thumbnail"></div>')
            .data('config', config);

        var img = $('<img class="img-thumbnail" />').attr('src', config.thumbnailUrl);

        if (resourceType === 'video') {
            var contentCoverDiv = $('<div class="content-cover"><span class="fa fa-play-circle"></span></div>');
            contentThumbDiv.append(contentCoverDiv, img);
            this.prepend(contentThumbDiv);
            $(contentCoverDiv).click(function (e) {
                contentThumbDiv.prepend('<video src="SRC" preload="metadata" controls style="width:100%;" autoplay />'.replace('SRC', config.url));
                contentCoverDiv.remove();
                img.remove();
            });
        } else if (resourceType === 'image') {
            contentThumbDiv.append(img);
            this.prepend(contentThumbDiv);
        }

        return this;
    };
}(jQuery));
