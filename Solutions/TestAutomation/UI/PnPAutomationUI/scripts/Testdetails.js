function updateFlex() {
    // Internet Explorer 6-11 & // Edge 20+  !isIE && window.StyleMedia==Edge
    var isIE = document.documentMode;
    if (isIE) {
        $(".ms-Grid-row").css('display', '-ms-flexbox');
    }
        //works for chrome and edge browser
    else {
        $(".ms-Grid-row").css('display', 'flex');
    }
}
function drawChart(datatable,rootSiteURL,branch) {
    var dataTable = datatable;
    var data = new google.visualization.DataTable();
    data.addColumn('string', '');
    data.addColumn('number', 'Passed');
    data.addColumn('number', 'Failed');
    data.addColumn('string', 'TestrunID');
    for (var i = 0; i < dataTable.length; i++) {
        var date = ToJavaScriptDate(dataTable[i].Testdate);
        var testRunID = dataTable[i].TestRunID.toString();
        data.addRow([date, dataTable[i].Passed, dataTable[i].Failed, testRunID]);
    }
    
    var chart = new google.charts.Line(document.getElementById('line_top_x'));

    function selectHandler() {
        var selectedItem = chart.getSelection()[0];
        if (selectedItem) {
            var TestRunID = data.getValue(selectedItem.row, 3);
            var outcome=null;
            if (selectedItem.column == 1) {
                outcome = 0;
            }
            if (selectedItem.column == 2) {
                outcome=2
            }
            var url = rootSiteURL + "Testsummary/AllTests/" + TestRunID + "?outcome=" + outcome + "&branch=" + branch
            window.location.href = url;
        }
    }

    google.visualization.events.addListener(chart, 'select', selectHandler);
    //Hide the TestRunID column in browser
    var view = new google.visualization.DataView(data);
    view.setColumns([0, 1, 2]);
    chart.draw(view);
}
function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}
