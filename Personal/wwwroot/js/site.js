
var page = 2;
var pageSize = 10;
var isGettingMorePosts = false;

$(document).ready(function () {
    initJqueryFiler();
    initInfiniteScroll();
    initContentCounter();
    initPostImages();
});

function initJqueryFiler() {
    if ($('#inputImages').length != 0) {
        $('#inputImages').filer({
            limit: 3,
            maxSize: 10,
            extensions: ["jpg", "png", "gif"],
            showThumbs: true,
            addMore: true,
            changeInput: '<button class="btn upload-image" onclick="return false;"><span class="fas fa-images text-warning"></span></button>',
            uploadFile: null,
            onSelect: null
        });
    }
}
function initInfiniteScroll() {
    if ($('#postContainer').length != 0) {
        $(window).scroll(function () {
            if (($(window).scrollTop() == $(document).height() - $(window).height()) && !isGettingMorePosts) {
                isGettingMorePosts = true;
                var searchKeyword = $('#searchKeyword').val();
                $.ajax({
                    url: '/posts/moreposts?page=' + page + '&pageSize=' + pageSize+'&search='+searchKeyword,
                    type: 'GET',
                    beforeSend: function () {
                        $('#loadingtext').show();
                    },
                    success: function (data) {
                        if (data != null && data.trim() != "") {
                            $("#postList").append(data);
                            page++;
                            initPostImages();
                        }
                        $('#loadingtext').hide();
                    },
                    error: function (data) {
                        $('#loadingtext').hide();
                        $('#errortext').fadeIn().delay(3000).fadeOut();
                    },
                    complete: function () {
                        isGettingMorePosts = false;
                    }
                });
            }
        });
    }
}
function initContentCounter() {
    if ($('#txtContent').length != 0) {
        $('#textRemaining').text(1000);
        $('#txtContent').on('keyup change', function () {
            var max = 1000;
            var len = $(this).val().length;
            $('#textRemaining').text(max - len);
        });

    }
}
function initPostImages() {
    if ($(".photoList").length != 0) {
        $(".photoList").justifiedGallery({
            rowHeight: 120,
            maxRowHeight: null,
            margins: 1,
            border: 1,
            rel: 'photolist',
            lastRow: 'justify',
            captions: false,
            randomize: false
        });

        $('.photoList').each(function () { // the containers for all your galleries
            $(this).magnificPopup({
                delegate: 'a',
                type: 'image',
                gallery: {
                    enabled: true,
                    preload: [0, 2]
                }
            });
        });
    }
   

    
}


