var updateData;

(function () {
    var percent = function(a, b) {
        return Math.round(1.0 * a / b * 100) || 0;
    };

    var getMinuteText = function (moment) {
        return moment.format('HH:mm');
    };

    var drawBars = function (chart, options, data, xScale, yScale, topValues, bottomValues, daysValues, hoverHandler) {
        var barWidth = xScale(data.minutes[1]) - xScale(data.minutes[0]);

        data.minutes.forEach(minute => {
            var bar = chart.append('g')
                .classed('chart__bar', true)
                .classed('chart__bar__' + minute, true)
                .attr('transform', 'translate(' + xScale(minute) + ', 0)')
                .on('mouseover', function () {
                    hoverHandler(minute);
                });

            bar.append('rect')
                .classed('chart__bar_one', true)
                .attr('y', function (d) { return yScale(topValues(data, minute)); })
                .attr('height', function (d) {
                    return bottomValues(data, minute) < topValues(data, minute)
                        ? options.height - yScale(topValues(data, minute))
                        : 0;
                })
                .attr('width', barWidth);

            if (bottomValues(data, minute)) {
                bar.append('rect')
                    .classed('chart__bar_two', true)
                    .attr('y', function (d) { return yScale(bottomValues(data, minute)); })
                    .attr('height', function (d) {
                        return options.height - yScale(bottomValues(data, minute));
                    })
                    .attr('width', barWidth);
            }
        });
    };

    var setBarSelected = function (chart, minute) {
        chart.selectAll('.chart__bar').classed('chart__bar_selected', false);
        chart.selectAll('.chart__bar__' + minute).classed('chart__bar_selected', true);
    };

    var drawBarChart = function (selections, selection, data, maxValue, topValues, bottomValues, daysValues) {
        var options = {
            width: 525,
            height: 35,
            xScaleLineHeight: 1,
            usualWorkDays: 5
        };

        var chart = selection
            .html('')
            .append('svg')
            .attr('width', options.width)
            .attr('height', options.height);

        var x = d3.scaleLinear()
            .range([0, options.width])
            .domain([+data.min.minute, +data.max.minute + (+data.max.minute - +data.min.minute) / data.minutes.length]);

        var y = d3.scaleLinear()
            .range([options.height, 0])
            .domain([0, maxValue(data)]);

        drawBars(chart, options, data, x, y, topValues, bottomValues, daysValues, function (minute) {
            highlightMinute(selections, data, minute);
        });

        var xScaleLine = chart.append('g')
            .attr('transform', 'translate(0, ' + options.height + ')');

        xScaleLine.append('rect')
            .classed('chart__bar__text', true)
            .attr('x', 0)
            .attr('y', 0)
            .attr('height', 1)
            .attr('width', options.width);

        chart.on('mouseout', function () {
            highlightLastMinute(selections, data);
        });

        highlightLastMinute(selections, data);
    };

    var setCounters = function (data, minute) {
        d3.select('.placeholder__replays').text(Object.values(data.replays).reduce(function(a, b) { return a + b }, 0));
        d3.select('.placeholder__replaysWonRate').text(percent(Object.values(data.replaysWon).reduce(function (a, b) { return a + b }, 0), Object.values(data.replays).reduce(function (a, b) { return a + b }, 0)));

        d3.select('.time-start').text(getMinuteText(moment.unix(data.min.minute * 60)));
        d3.select('.time-end').text(getMinuteText(moment.unix(data.max.minute * 60)));
    };

    var highlightMinute = function (selections, data, minute) {
        setCounters(data, minute);

        selections.forEach(function (selection) {
            setBarSelected(selection, minute);
        });
    };

    var highlightLastMinute = function (chart, data) {
        var minute = data.minutes[data.minutes.length - 1];
        highlightMinute(chart, data, minute);
    };

    var drawBarCharts = function (data, options) {
        var selections = [];

        options.map(function (option) {
            option.selection = d3.select(option.selector);
            selections.push(option.selection);
            return option;
        }).forEach(function (option) {
            drawBarChart(selections, option.selection, data, option.max, option.top, option.bottom, option.day);
        });
    };

    updateData = function () {
        d3.json(apiReplays, function (data) {
            if (data) {
                var options = [{
                    selector: '.chart__replays',
                    max: function (data) { return data.max.replays; },
                    top: function (data, minute) { return data.replays[minute]; },
                    bottom: function (data, minute) { return data.replaysWon[minute]; },
                    day: function (data, minute) { return 1; }
                }];

                var charts = d3.select('.chart-insertion-point').html('');

                var winRates = {};

                for (var ai in Object.keys(data.replaysByAi)) {
                    var aiName = Object.keys(data.replaysByAi)[ai];

                    var wins = Object
                        .values(data.replaysByAiWon[aiName])
                        .reduce(function (a, b) { return a + b; }, 0);

                    var total = Object
                        .values(data.replaysByAi[aiName])
                        .reduce(function (a, b) { return a + b; }, 0);

                    winRates[ai] = total === 0 ? 0 : wins / total;
                }

                var sortable = [];
                for (var i in winRates) {
                    sortable.push([i, winRates[i]]);
                }

                sortable.sort(function (a, b) {
                    return - a[1] + b[1];
                });

                for (var i in sortable) {
                    var ai = sortable[i][0];
                    var aiName = Object.keys(data.replaysByAi)[ai];

                    (function (ai, aiName) {


                        var wins = Object
                            .values(data.replaysByAiWon[aiName])
                            .reduce(function (a, b) { return a + b; }, 0);

                        var total = Object
                            .values(data.replaysByAi[aiName])
                            .reduce(function (a, b) { return a + b; }, 0);

                        var cols = charts.append('div').classed('columns', true);
                        cols.append('div')
                            .classed('columns__column', true)
                            .append('div')
                            .classed('chart', true)
                            .classed('chart__replaysByAi_' + ai, true);
                        var counters = cols.append('div')
                            .classed('columns__column', true)
                            .classed('columns__column_value', true)
                            .append('div')
                            .classed('counters', true);
                        counters.append('span')
                            .classed('counters__counter', true)
                            .classed('counters__counter__value', true)
                            .html('AI:&nbsp;')
                            .append('span')
                            .classed('placeholder__replaysByAi_' + ai + '_name', true)
                            .text(aiName);
                        var counter2 = counters.append('span')
                            .classed('counters__counter', true)
                            .classed('counters__counter__value', true)
                            .html('Won:&nbsp;' + percent(wins, total) + '&thinsp;% &nbsp;of&nbsp;' + total);



                        options.push({
                            selector: '.chart__replaysByAi_' + ai,
                            max: function (data) { return 1; },
                            top: function (data, minute) { return total > wins ? 1 : 0; },
                            bottom: function (data, minute) { return data.replaysByAiWon[aiName][minute] / data.replaysByAi[aiName][minute]; },
                            day: function (data, minute) { return 0.5; }
                        });
                    })(ai, aiName);
                }

                for (var ai in Object.keys(data.replaysByMapSize)) {
                    var aiName = Object.keys(data.replaysByMapSize)[ai];

                    (function (ai, aiName) {
                        d3.select('.placeholder__replaysByMapSize_' + ai + '_name')
                            .text(aiName);

                        var wins = Object
                            .values(data.replaysByMapSizeWon[aiName])
                            .reduce(function (a, b) { return a + b; }, 0);

                        var total = Object
                            .values(data.replaysByMapSize[aiName])
                            .reduce(function (a, b) { return a + b; }, 0);

                        d3.select('.placeholder__replaysByMapSize_' + ai + '_wonRate')
                            .text(percent(wins, total));

                        d3.select('.placeholder__replaysByMapSize_' + ai + '_count')
                            .text(total);

                        options.push({
                            selector: '.chart__replaysByMapSize_' + ai,
                            max: function (data) { return 1; },
                            top: function (data, minute) { return total > wins ? 1 : 0; },
                            bottom: function (data, minute) { return data.replaysByMapSizeWon[aiName][minute] / data.replaysByMapSize[aiName][minute]; },
                            day: function (data, minute) { return 0.5; }
                        });
                    })(ai, aiName);
                }

                drawBarCharts(data, options);
            };
        });
    };
})();