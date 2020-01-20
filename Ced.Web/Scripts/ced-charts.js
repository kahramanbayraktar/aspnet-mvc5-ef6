/* Options for Bar chart */
var barOptions = {
    scaleBeginAtZero: true,
    scaleShowGridLines: true,
    scaleGridLineColor: "rgba(0,0,0,.05)",
    scaleGridLineWidth: 1,
    barShowStroke: false,
    barStrokeWidth: 0.8,
    barValueSpacing: 10,
    barDatasetSpacing: 1,
    responsive: true,
    showTooltips: false,
    onAnimationComplete: function () {

        var ctx = this.chart.ctx;
        ctx.font = this.scale.font;
        ctx.fillStyle = this.scale.textColor;
        ctx.textAlign = "center";
        ctx.textBaseline = "bottom";

        var maxY = this.scale.max;

        this.datasets.forEach(function (dataset) {
            dataset.bars.forEach(function (bar) {
                if (bar.value > 0) {
                    if (bar.value / maxY > 0.95) {
                        ctx.fillText(bar.value, bar.x, bar.y + 20);
                    } else {
                        ctx.fillText(bar.value, bar.x, bar.y - 5);
                    }
                }
            });
        });
    }
};

//SQM SALES
function BarChartSqmSales(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            {
                label: "Local",
                fillColor: "rgba(253,210,76,0.5)",
                strokeColor: "rgba(253,210,76,0.8)",
                highlightFill: "rgba(253,210,76,0.75)",
                highlightStroke: "rgba(253,210,76,1)",
                data: data.LocalSqms
            },
            {
                label: "International",
                fillColor: "rgba(138,177,196,0.5)",
                strokeColor: "rgba(138,177,196,0.8)",
                highlightFill: "rgba(138,177,196,0.75)",
                highlightStroke: "rgba(138,177,196,1)",
                data: data.IntlSqms
            },
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalSqms
            }
        ]
    };

    var ctx = document.getElementById("barSqmSales").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendSqmSales').appendChild(legendHolder.firstChild);
}

//E-TICKET CONVERSION RATIO
var barOptionsEticket = {
    scaleBeginAtZero: true,
    scaleShowGridLines: true,
    scaleGridLineColor: "rgba(0,0,0,.05)",
    scaleGridLineWidth: 1,
    barShowStroke: false,
    barStrokeWidth: 0.8,
    barValueSpacing: 10,
    barDatasetSpacing: 1,
    responsive: true,
    showTooltips: false,
    onAnimationComplete: function () {

        var ctx = this.chart.ctx;
        ctx.font = this.scale.font;
        ctx.fillStyle = this.scale.textColor;
        ctx.textAlign = "center";
        ctx.textBaseline = "bottom";

        var maxY = this.scale.max;

        this.datasets.forEach(function (dataset) {
            dataset.bars.forEach(function (bar) {
                if (bar.value > 0) {
                    if (bar.value / maxY > 0.95) {
                        ctx.fillText(bar.value * 100 + "%", bar.x, bar.y + 20);
                    } else {
                        ctx.fillText(bar.value * 100 + "%", bar.x, bar.y - 5);
                    }
                }
            });
        });
    }
};

function BarChartActVisRat(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barActVisRat").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptionsEticket);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendActVisRat').appendChild(legendHolder.firstChild);
}

//SPONSORSHIP STATS
function BarChart2(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barSponsor").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendSponsor').appendChild(legendHolder.firstChild);
}

//NUMBER OF EXHIBITORS
function BarChartExhibitor(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            {
                label: "Local",
                fillColor: "rgba(253,210,76,0.5)",
                strokeColor: "rgba(253,210,76,0.8)",
                highlightFill: "rgba(253,210,76,0.75)",
                highlightStroke: "rgba(253,210,76,1)",
                data: data.LocalNumbers
            },
            {
                label: "International",
                fillColor: "rgba(138,177,196,0.5)",
                strokeColor: "rgba(138,177,196,0.8)",
                highlightFill: "rgba(138,177,196,0.75)",
                highlightStroke: "rgba(138,177,196,1)",
                data: data.IntlNumbers
            },
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barExhibitor").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendExhibitor').appendChild(legendHolder.firstChild);
}

//NUMBER OF VISITORS
function BarChart5(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            {
                label: "Local",
                fillColor: "rgba(253,210,76,0.5)",
                strokeColor: "rgba(253,210,76,0.8)",
                highlightFill: "rgba(253,210,76,0.75)",
                highlightStroke: "rgba(253,210,76,1)",
                data: data.LocalNumbers
            },
            {
                label: "International",
                fillColor: "rgba(138,177,196,0.5)",
                strokeColor: "rgba(138,177,196,0.8)",
                highlightFill: "rgba(138,177,196,0.75)",
                highlightStroke: "rgba(138,177,196,1)",
                data: data.IntlNumbers
            },
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barVisitor").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendVisitor').appendChild(legendHolder.firstChild);
}

//NUMBER OF EXHIBITORS PER SQM
function BarChartSqmPerExhibitor(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            {
                label: "Local",
                fillColor: "rgba(253,210,76,0.5)",
                strokeColor: "rgba(253,210,76,0.8)",
                highlightFill: "rgba(253,210,76,0.75)",
                highlightStroke: "rgba(253,210,76,1)",
                data: data.LocalNumbers
            },
            {
                label: "International",
                fillColor: "rgba(138,177,196,0.5)",
                strokeColor: "rgba(138,177,196,0.8)",
                highlightFill: "rgba(138,177,196,0.75)",
                highlightStroke: "rgba(138,177,196,1)",
                data: data.IntlNumbers
            },
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barSqmPerExhibitor").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendSqmPerExhibitor').appendChild(legendHolder.firstChild);
}

//NUMBER OF EXHIBITORS PER SQM
function BarChartVisitorPerSqm(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            //{
            //    label: "Local",
            //    fillColor: "rgba(253,210,76,0.5)",
            //    strokeColor: "rgba(253,210,76,0.8)",
            //    highlightFill: "rgba(253,210,76,0.75)",
            //    highlightStroke: "rgba(253,210,76,1)",
            //    data: data.LocalNumbers
            //},
            //{
            //    label: "International",
            //    fillColor: "rgba(138,177,196,0.5)",
            //    strokeColor: "rgba(138,177,196,0.8)",
            //    highlightFill: "rgba(138,177,196,0.75)",
            //    highlightStroke: "rgba(138,177,196,1)",
            //    data: data.IntlNumbers
            //},
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barVisitorPerSqm").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendVisitorPerSqm').appendChild(legendHolder.firstChild);
}

//NUMBER OF EXHIBITORS PER EXHIBITOR
function BarChartVisitorPerExhibitor(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            {
                label: "Local",
                fillColor: "rgba(253,210,76,0.5)",
                strokeColor: "rgba(253,210,76,0.8)",
                highlightFill: "rgba(253,210,76,0.75)",
                highlightStroke: "rgba(253,210,76,1)",
                data: data.LocalNumbers
            },
            {
                label: "International",
                fillColor: "rgba(138,177,196,0.5)",
                strokeColor: "rgba(138,177,196,0.8)",
                highlightFill: "rgba(138,177,196,0.75)",
                highlightStroke: "rgba(138,177,196,1)",
                data: data.IntlNumbers
            },
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barVisitorPerExhibitor").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendVisitorPerExhibitor').appendChild(legendHolder.firstChild);
}

//NUMBER OF EXHIBITORS PER SQM PER DAY
function BarChartVisitorPerSqmAndDay(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            //{
            //    label: "Local",
            //    fillColor: "rgba(253,210,76,0.5)",
            //    strokeColor: "rgba(253,210,76,0.8)",
            //    highlightFill: "rgba(253,210,76,0.75)",
            //    highlightStroke: "rgba(253,210,76,1)",
            //    data: data.LocalNumbers
            //},
            //{
            //    label: "International",
            //    fillColor: "rgba(138,177,196,0.5)",
            //    strokeColor: "rgba(138,177,196,0.8)",
            //    highlightFill: "rgba(138,177,196,0.75)",
            //    highlightStroke: "rgba(138,177,196,1)",
            //    data: data.IntlNumbers
            //},
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barVisitorPerSqmAndDay").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendVisitorPerSqmAndDay').appendChild(legendHolder.firstChild);
}

//NUMBER OF DELEGATES
function BarChart6(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            {
                label: "Local",
                fillColor: "rgba(253,210,76,0.5)",
                strokeColor: "rgba(253,210,76,0.8)",
                highlightFill: "rgba(253,210,76,0.75)",
                highlightStroke: "rgba(253,210,76,1)",
                data: data.LocalNumbers
            },
            {
                label: "International",
                fillColor: "rgba(138,177,196,0.5)",
                strokeColor: "rgba(138,177,196,0.8)",
                highlightFill: "rgba(138,177,196,0.75)",
                highlightStroke: "rgba(138,177,196,1)",
                data: data.IntlNumbers
            },
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barDelegate").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendDelegate').appendChild(legendHolder.firstChild);
}

//NUMBER OF PAID DELEGATES
function BarChart7(data) {
    var barData = {
        labels: data.Years,
        datasets: [
            {
                label: "Local",
                fillColor: "rgba(253,210,76,0.5)",
                strokeColor: "rgba(253,210,76,0.8)",
                highlightFill: "rgba(253,210,76,0.75)",
                highlightStroke: "rgba(253,210,76,1)",
                data: data.LocalNumbers
            },
            {
                label: "International",
                fillColor: "rgba(138,177,196,0.5)",
                strokeColor: "rgba(138,177,196,0.8)",
                highlightFill: "rgba(138,177,196,0.75)",
                highlightStroke: "rgba(138,177,196,1)",
                data: data.IntlNumbers
            },
            {
                label: "Total",
                fillColor: "rgba(226,82,16,0.5)",
                strokeColor: "rgba(226,82,16,0.8)",
                highlightFill: "rgba(226,82,16,0.75)",
                highlightStroke: "rgba(226,82,16,1)",
                data: data.TotalNumbers
            }
        ]
    };

    var ctx = document.getElementById("barPaidDelegate").getContext("2d");
    window.myBar = new Chart(ctx).Bar(barData, barOptions);

    var legendHolder = document.createElement('div');
    legendHolder.innerHTML = window.myBar.generateLegend();

    document.getElementById('legendPaidDelegate').appendChild(legendHolder.firstChild);
}

//NPS VISITOR
function BarChart8(data) {
    // Stacked horizontal bar
    Chartist.Bar('#ct-chartVisitorNps', {
        labels: data.Years,
        series: [
            data.TotalNumbers
        ]
    }, {
        seriesBarDistance: 10,
        reverseData: true,
        horizontalBars: true,
        axisX: {
            onlyInteger: true
        },
        axisY: {
            offset: 70
        },
        plugins: [
            Chartist.plugins.ctBarLabels({
                position: {
                    x: function (data) {
                        if (data.value.x > 0)
                            return data.x2 - 30;
                        else
                            return data.x2 + 30;
                    }
                },
                labelOffset: {
                    y: 7
                },
                labelInterpolationFnc: function (text) {
                    if (text == null)
                        text = '';
                    return text;
                }
            })
        ]
    });
}

//NPS EXHIBITOR
function BarChart9(data) {
    // Stacked horizontal bar
    Chartist.Bar('#ct-chart5', {
        labels: data.Years,
        series: [
            data.TotalNumbers
        ]
    }, {
        //high: 10,
        //low: -10,
        seriesBarDistance: 10,
        reverseData: true,
        horizontalBars: true,
        axisX: {
            onlyInteger: true
        },
        axisY: {
            offset: 70
        },
        plugins: [
            Chartist.plugins.ctBarLabels({
                position: {
                    x: function (data) {
                        if (data.value.x > 0)
                            return data.x2 - 30;
                        else
                            return data.x2 + 30;
                    }
                },
                labelOffset: {
                    y: 7
                },
                labelInterpolationFnc: function (text) {
                    if (text == null)
                        text = '';
                    return text;
                }
            })
        ]
    });
}

//NPS SATISFACTION VISITOR
function BarChart10(data) {
    // Stacked horizontal bar
    Chartist.Bar('#ct-chart6', {
        labels: data.Years,
        series: [
            data.TotalNumbers
        ]
    }, {
        seriesBarDistance: 10,
        reverseData: true,
        horizontalBars: true,
        axisX: {
            onlyInteger: true
        },
        axisY: {
            offset: 70
        },
        plugins: [
            Chartist.plugins.ctBarLabels({
                position: {
                    x: function (data) {
                        if (data.value.x > 0)
                            return data.x2 - 30;
                        else
                            return data.x2 + 30;
                    }
                },
                labelOffset: {
                    y: 7
                },
                labelInterpolationFnc: function (text) {
                    if (text == null)
                        text = '';
                    return text;
                }
            })
        ]
    });
}

//NPS SATISFACTION EXHIBITOR
function BarChart11(data) {
    // Stacked horizontal bar
    Chartist.Bar('#ct-chart7', {
        labels: data.Years,
        series: [
            data.TotalNumbers
        ]
    }, {
        seriesBarDistance: 10,
        reverseData: true,
        horizontalBars: true,
        axisX: {
            onlyInteger: true
        },
        axisY: {
            offset: 70
        },
        plugins: [
            Chartist.plugins.ctBarLabels({
                position: {
                    x: function(data) {
                        if (data.value.x > 0)
                            return data.x2 - 30;
                        else
                            return data.x2 + 30;
                    }
                },
                labelOffset: {
                    y: 7
                },
                labelInterpolationFnc: function (text) {
                    if (text == null)
                        text = '';
                    return text;
                }
            })
        ]
    });
}

//NPS AVERAGE VISITOR
//function BarChart12(data) {
//    // Stacked horizontal bar
//    new Chartist.Bar('#ct-chart8', {
//        labels: data.Years,
//        series: [
//            data.TotalNumbers
//        ]
//    }, {
//        seriesBarDistance: 10,
//        reverseData: true,
//        horizontalBars: true,
//        axisY: {
//            offset: 70
//        }
//    });

//    //var legendHolder = document.createElement('div');
//    //legendHolder.innerHTML = chartistBar.generateLegend();

//    //document.getElementById('legendSqmSales').appendChild(legendHolder.firstChild);
//}

//NPS AVERAGE EXHIBITOR
//function BarChart13(data) {
//    // BI-POLAR BAR CHART
//    var options = {
//        high: 10,
//        low: -10,
//        //axisX: {
//        //    labelInterpolationFnc: function(value, index) {
//        //        return index % 1 === 0 ? value : null;
//        //    }
//        //}
//    };

//    new Chartist.Bar('#ct-chart9', {
//        labels: data.Years,
//        series: [
//            data.TotalNumbers
//        ]
//    }, options);

//    //var legendHolder = document.createElement('div');
//    //legendHolder.innerHTML = chartistBar.generateLegend();

//    //document.getElementById('legendSqmSales').appendChild(legendHolder.firstChild);
//}