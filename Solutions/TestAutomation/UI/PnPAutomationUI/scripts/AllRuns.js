$(document).ready(function () {
    $('#Configurations').multiselect({
        numberDisplayed: 0,
        includeSelectAllOption: true,
        selectAllValue: 'Select-All'
    });   
    $('#Category').multiselect({
        numberDisplayed: 0,
        includeSelectAllOption: true,
        selectAllValue: 'Select All',
        onChange: function (option, checked) {
            $.fn.GetConfigurationData();            
        },             
        onSelectAll: function (checked) {
            $.fn.GetConfigurationData();         
        },
        onDeselectAll: function () {            
            $('#Configurations option[value]').remove();
            $('#Configurations').multiselect('rebuild');
        }
    });
   

    $('#txtFromDate').datepicker({
        dateFormat: "mm/dd/yy",
        onSelect: function (selected) {
            $('#txtToDate').datepicker("option", "minDate", selected)
        }
    });

    $('#txtToDate').datepicker({
        dateFormat: "mm/dd/yy",
        onSelect: function (selected) {
            $('#txtFromDate').datepicker("option", "maxDate", selected)
        }
    });

    $('#btnSearch').click(function () {
        var Category = $('#Category').val();
        var Configuration = $('#Configurations').val();
        if (Category == null) {
            alert("Please select Category");
            return false;
        }
        else if (Configuration == null) {
            alert("Please select Configuration");
            return false;
        }
        updatePageLinkHref(this);
    });

    var Category = $('#Category option:selected');
    var Configuration = $('#Configurations option:selected');
    if (Category.length > 0 && Configuration.length == 0) {
        $('#Configurations').multiselect('selectAll', false);
        $('#Configurations').multiselect('refresh');
    }

});

$.fn.GetConfigurationData = function () {
    $.ajax({
        url: MyAppUrlSettings.PnPAutomationUrl,
        type: 'post',
        dataType: 'html',
        data: {
            CategoryIds: $('#Category').val(),
        }
    }).done(function (response) {
        $('#Configurations option[value]').remove();
        $('#Configurations').append(response);
        $('#Configurations').multiselect('rebuild');
        $('#Configurations').multiselect('selectAll', false);
        $('#Configurations').multiselect('refresh');
    });
}

function updatePageLinkHref(thisElem) {
    $(thisElem).attr('href', $(thisElem).attr('href') + "&fromDate=" + $("#txtFromDate").val() + "&toDate=" + $("#txtToDate").val() + "&ConfigurationId=" + $("#Configurations").val() + "&CategoryId=" + $("#Category").val());
}

