$(function() {
    // setup tooltips
    $('[title]').tooltipster({ theme: 'tooltipster-custom', trigger: 'hover' })
                .addClass('with-tooltip');

    // setup width
    $('.content').wrapInner('<div class="content-wrapper"></div>')
                 .css('max-width', (model.libraryCount * 100 + 190) + 'px');
    
    // setup waypoints
    (function() {
        var $selected = $();
        var waypointsBlocked = false;

        var select = function($link) {
            $selected.removeClass('selected');
            $selected = $link;
            $selected.addClass('selected');
        };

        var blockWaypoints = function(duration) {
            waypointsBlocked = true;
            window.setTimeout(function() { waypointsBlocked = false; }, duration);
        };

        var getCurrentLink = function() {
            return $('.main-nav a').filter(function() {
                return this.href === window.location.href;
            });
        };

        $('.main-nav a[data-same-page]').each(function() {
            var $this = $(this);
            var match = $this.prop('href').match(/#.+/);
            
            var $target = $(match ? match[0] : 'h1');
            $target.waypoint(function() {
                if (!waypointsBlocked)
                    select($this);
            });
        });

        select(getCurrentLink());
        blockWaypoints(2000);

        $(window).on('hashchange', function() {
            select(getCurrentLink());
            blockWaypoints(2000);
        });
    })();

    // setup narrow detection (not currently used)
    (function() {
        var $body = $('body');
        var $widthTestTd = $('td:not(.row-name)').eq(0);
        var reactToWidth = function() {
            $body.toggleClass('narrow-table-headers', $widthTestTd.width() < 74);
        };

        reactToWidth();
        $(window).resize(reactToWidth);
    })();
});