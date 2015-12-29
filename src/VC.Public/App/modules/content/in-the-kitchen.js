$(function ()
{
    var youtubePlayer;
    var root = $('.content-page');

    $(document).ready(function ()
    {
        root.find("a.youtube-item").click(function (e)
        {
            showSection(this);
            if (youtubePlayer !== undefined)
            {
                youtubePlayer.loadVideoById($(this).attr("data-video-id"));
                //youtubePlayer.playVideo();
            }
            else
            {
                $("#youtube-container").show();
                youtubePlayer = new YT.Player('youtube-player', {
                    height: '260',
                    width: '468',
                    videoId: $(this).attr("data-video-id"),
                    playerVars: {
                        autoplay: 1,
                        iv_load_policy: 3,
                        rel: 0,
                        showinfo: 0,
                        wmode: "opaque"
                    },
                    events: {
                        'onStateChange': onYouTubePlayerStatusChange
                    }
                });
            }
            e.preventDefault();
            return false;
        });

        root.find("#youtube-container .youtube-close").click(function (e)
        {
            closeYoutubeVideo();
            showSection(root.find('.right-content-pane .youtube-item:first').get(0))
            e.preventDefault();
            return false;
        });

        showSection(root.find('.right-content-pane .youtube-item:first').get(0))
    });

    function showSection(link)
    {
        var title = $(link).data('tooltip-title');
        var body = $(link).data('tooltip-body');
        var href = $(link).attr('href');

        var curerntItem = root.find('.current-item-wrapper');
        curerntItem.find('h4').text(title);
        curerntItem.find('.body').text(body);
        curerntItem.find('a').attr('href', href);
        curerntItem.show();
    }

    function onYouTubePlayerStatusChange(event)
    {
        if (event.data == YT.PlayerState.ENDED)
        {
            closeYoutubeVideo();
        }
    }

    function destroyPlayer()
    {
        if (youtubePlayer !== undefined)
        {
            youtubePlayer.destroy();
            youtubePlayer = undefined;
        }
    }

    function closeYoutubeVideo()
    {
        destroyPlayer();
        $("#youtube-container").hide();
    }
});