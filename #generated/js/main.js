$(function() {
    var $body = $('body');

    // setup tooltips
    (function() {
        var $currentTrigger = $();
        $('[title]').tooltipster({ theme: 'tooltipster-custom', trigger: 'custom' })
                    .addClass('with-tooltip')
                    .click(function() {
                        var $this = $(this);
                        $this.data('clicked', true);

                        if ($this === $currentTrigger)
                            return;

                        $this.tooltipster('show');
                        $currentTrigger = $this;
                    })
                    .hover(
                        function() {
                            var $this = $(this);
                            
                            $currentTrigger.data('clicked', false)
                                           .tooltipster('hide');
                            $this.tooltipster('show');
                            $currentTrigger = $this;
                        },
                        function() {
                            var $this = $(this);
                            if ($this.data('clicked'))
                                return;

                            $this.tooltipster('hide');
                            $currentTrigger = $();
                        }
                    );


        $('body').click(function(e) {
            var $target = $(e.target);
            if ($target.is('.with-tooltip') || $target.parents('.tooltipster-base').length > 0)
                return;
            
            $currentTrigger.data('clicked', false)
                           .tooltipster('hide');
            
            $currentTrigger = $();
        });
    })();

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
        var $widthTestTd = $('td:not(.row-name)').eq(0);
        var reactToWidth = function() {
            $body.toggleClass('narrow-table-headers', $widthTestTd.width() < 74);
        };

        reactToWidth();
        $(window).resize(reactToWidth);
    })();
});