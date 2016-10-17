function validateEmailandSubmit() {
    var email = $("#txtEmail").val();

    if (email != null && email != "") {
        var atpos = email.indexOf("@");
        var dotpos = email.lastIndexOf(".");
        if (atpos < 1 || dotpos < atpos + 2 || dotpos + 2 >= email.length) {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        return false;
    }
}

function save(thisElem) {
    var sendTestResults = $("#chkSendDailysummary").is(':checked');

    if (!sendTestResults)
    {
        alert("Please select 'Send me a daily test summary' checkbox to subscribe");
        event.preventDefault();
        return false;
    }   

    if (validateEmailandSubmit()) {
        var email = $("#txtEmail").val();
        document.getElementById('wait').style.display = 'block';
        document.getElementById('content').style.display = 'none';
        $(thisElem).attr('href', $(thisElem).attr('href') + "&email=" + email);
    }
    else {
        alert('Please enter valid email to subscribe');
        event.preventDefault();
    }
}

function update(thisElem) {
    var hdnEmail = $("#hdnEmail").val();
    var email = $("#txtEmail").val();
    var hdnSendTestResults = $("#hdnSendTestResults").val();
    var hdnTestResult = new Boolean(hdnSendTestResults);
    var sendTestResults = $("#chkSendDailysummary").is(':checked');

    if (hdnEmail == email && hdnTestResult == sendTestResults)
    {
        event.preventDefault();
        return false;
    }

    if (email != null && email != "") {
        if (validateEmailandSubmit()) {
            document.getElementById('wait').style.display = 'block';
            document.getElementById('content').style.display = 'none';
            $(thisElem).attr('href', $(thisElem).attr('href') + "&email=" + email + "&isSendTestResults=" + sendTestResults);
        }
        else {
            alert('Please enter valid email to subscribe');
            event.preventDefault();
        }
    }
}

$(document).ready(function () {
 
    $('#chkSendDailysummary').change(function () {
        if ($(this).is(':checked')) {
            document.getElementById('txtEmail').disabled = false;
        }
        else {
            document.getElementById('txtEmail').disabled = true;
        }
    });

});
