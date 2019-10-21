
var page = 2;
var pageSize = 10;
var isGettingMorePosts = false;

if ($('#postlisting').length != 0) {
    $(window).scroll(function () {
        if (($(window).scrollTop() == $(document).height() - $(window).height()) && !isGettingMorePosts) {
            isGettingMorePosts = true;
            $.ajax({
                url: '/posts/moreposts?page=' + page + '&pageSize=' + pageSize,
                type: 'GET',
                beforeSend: function () {
                    $('#loadingtext').addClass('text-info');
                    $('#loadingtext').text('Loading ...');
                    $('#loadingtext').show();
                },
                success: function (data) {
                    if (data != null && data.trim() != "") {
                        $("#postList").append(data);
                        page++;
                    }
                    $('#loadingtext').hide();
                },
                error: function (data) {
                    $('#loadingtext').text('Unable to load posts');
                    $('#loadingtext').addClass('text-danger');
                    $('#loadingtext').fadeIn().delay(2000).fadeOut();
                },
                complete: function () {
                    isGettingMorePosts = false;
                }
            });
        } else {
            $("#loadingtext").hide();
        }
    });
}

if ($('.post-input').length != 0) {
    $('.post-input').focus(function () {
        $(this).animate({ rows: 7 }, 200, 'swing');
    });
    $('.post-input').focusout(function () {
        $(this).animate({ rows: 1 }, 200, 'swing');
    });
}

$(document).ready(function () {
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
    
});