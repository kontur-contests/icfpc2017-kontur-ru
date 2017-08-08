var apiPrefix = 'http://c3980b18.ngrok.io/api/';

var apiHealth = apiPrefix + 'health.json';
var apiReplays = apiPrefix + 'replays.json';


(function() {
    var updateLiveIndicator = function (live) {
        d3.select('.live-indicator')
            .classed('live-indicator_online', live)
            .classed('live-indicator_offline', !live);
    };
    var checkOnlineOrOffline = function() {
        d3.json(apiHealth, function () {
                updateLiveIndicator(true);
            })
            .on('error', function () {
                updateLiveIndicator(false);
            });
    };
    setInterval(checkOnlineOrOffline, 1000);
})();