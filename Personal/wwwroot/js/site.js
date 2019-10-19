
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

