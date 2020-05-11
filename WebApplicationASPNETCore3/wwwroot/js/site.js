// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.



function changeWKCampaign() {
    //change & update
    var wkCampaignId = $("#campaignDDL").val();
    //var jqxhr = $.post("/Campaign/ChangeWKCampaign", { wkCampaignId: wkCampaignId }, function (data) {
    //    alert("Success Working Campaign changed.");
    //});
        //.done(function () {
        //    alert("second success");
        //})
        //.fail(function (err) {
        //    alert("Change Working Campaign failed." + err);
        //})
        //.always(function () {
        //    alert("finished");
        //});

    $.ajax({
        type: "Post",
        url: '/Campaign/ChangeWKCampaign',
        data: { wkCampaignId: wkCampaignId },
        success: function (data) {
            alert("Success Working Campaign changed.");
        },
        error: function (xhr, status, err) {
            alert("Change Working Campaign failed." + err);
            alert(status);

        }
    });

    $('#campaignDropDown div.dropdown-menu').toggle('fast');

}

$(document).ready(function () {

    //$.ajax({
    //    type: "Get",
    //    url: '/Campaign/GetCampaignList',
    //    success: function (data) {
    //        var s = '<option value="-1">Please Select a Campaign</option>';
    //        for (var i = 0; i < data.length; i++) {
    //            s += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
    //        }
    //        $("#campaignDDL").html(s);
    //    },
    //    error: function (err) {
    //        alert("Campaign List Data failed to load." + err);
    //    }
    //    });

    


    //stop event from closing dropdown menu when click dropdown list
    $('.dropdown').on("click", function (e) {

      //  e.stopPropagation();
      //  e.preventDefault();
    });
});