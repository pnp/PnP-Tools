$(document).ready(function () {
    $('#Configurations').multiselect({
        numberDisplayed: 0,
        includeSelectAllOption: true
    });
    $("#txtFromDate").datepicker({
        dateFormat: "mm/dd/yy",
        onSelect: function (selected) {
            $("#txtToDate").datepicker("option", "minDate", selected)
        }
    });
    $("#txtToDate").datepicker({
        dateFormat: "mm/dd/yy",
        onSelect: function (selected) {
            $("#txtFromDate").datepicker("option", "maxDate", selected)
        }
    });
});

function updatePageLinkHref(thisElem) {
    $(thisElem).attr('href', $(thisElem).attr('href') + "&fromDate=" + $("#txtFromDate").val() + "&toDate=" + $("#txtToDate").val() + "&ConfigurationId=" + $("#Configurations").val());
}
