var consolestatus = "less";
var errorstatus = "less"
function toggleDisplay(id, a) {
    var elaboration = document.getElementById(id);
    if (elaboration.style.display == "block") {
        elaboration.style.display = "none";
        if (a) a.innerHTML = "Read more..."; // won't work with target=_self
    }
    else {
        elaboration.style.display = "block";
        if (a) a.innerHTML = "Less...";
    }
}
function drawChart(dataTable) {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Execution Time');
    data.addColumn('number', 'Passed');
    data.addColumn('number', 'Failed');
    data.addColumn('number', 'Skipped');
    for (var i = 0; i < dataTable.length; i++) {
        var date = ToJavaScriptDate(dataTable[i].Testdate);
        data.addRow([date, dataTable[i].Passed, dataTable[i].Failed, dataTable[i].Skipped]);
    }
    var options = {
        chart: {
        },

        axes: {
            x: {
                0: { side: 'bottom' }
            }
        }
    };
    var chart = new google.charts.Line(document.getElementById('line_top_x'));
    chart.draw(data, options);
}
function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}