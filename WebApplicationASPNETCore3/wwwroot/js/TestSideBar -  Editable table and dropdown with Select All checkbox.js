function DBXPclick(e) {
    e.preventDefault();
    var listbutton = $("#selectedDBXP");
    if ($(listbutton).attr("aria-expanded") == "true") {
        $(listbutton).attr("aria-expanded", "false");
        $("#choicelist").css("display", "none");
        $("#selectedDBXP .fa-chevron-down").removeClass("fa-chevron-down--open");
    } else {
        $(listbutton).attr("aria-expanded", "true");
        $("#choicelist").css("display", "block");
        $("#selectedDBXP .fa-chevron-down").addClass("fa-chevron-down--open");
    }
}

function CheckAll(divId, sourceCheckbox) {
    divElement = document.getElementById(divId);
    inputElements = divElement.getElementsByTagName('input');
    for (i = 0; i < inputElements.length; i++) {
        if (inputElements[i].type != 'checkbox')
            continue;

        inputElements[i].checked = sourceCheckbox.checked;

    }
}

function DBXPSubmit(e) {
   
    var s = $('#myForm2').serialize();
    alert(s);
}

$(function () {
    $("input[name='DBXP']").on("change", function () {
        var itemCount = $("input[name='DBXP']:checked").length;
        if (itemCount == 0)
            $("#DBXPList").html("");
        else if (itemCount == 1)
            $("#DBXPList").html($("label[for='" + $("input[name='DBXP']:checked")[0].id + "']").text());
        else
            $("#DBXPList").html(itemCount + " items selected");
    });

    $("input[name='DBXP']").on("keyup", function (e) {

        if (e.keyCode == 27) {
            DBXPclick();
            $("#selectedDBXP").focus();
        }
    });

   
    
    //$("#checkAll").change(function () {
    //    divElement = document.getElementById("choicelist");
    //    inputElements = divElement.getElementsByTagName('input');
    //    for (i = 0; i < inputElements.length; i++) {
    //        if (inputElements[i].type == 'checkbox' )
    //            inputElements[i].checked =  ! inputElements[i].checked;
    //            };
    //});
});

