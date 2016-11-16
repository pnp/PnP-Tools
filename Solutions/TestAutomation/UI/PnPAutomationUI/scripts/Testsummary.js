$(document).ready(function () {
    // Internet Explorer 6-11 & // Edge 20+  !isIE && window.StyleMedia==Edge
    var isIE = document.documentMode;
    if (isIE) {
        $(".table.ms-Grid-row").css('display', '-ms-flexbox');
    }
        //works for chrome and edge browser
    else {
        $(".table.ms-Grid-row").css('display', 'flex');
    }
});
