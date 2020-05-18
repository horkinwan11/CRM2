
function campaignSearch(currentPage) {
    try {

        var searchText = $('#SearchText').val();
    
        $("#campaignList").load("/Campaign/LoadCampaignList",
            { searchText: searchText , currentPage: currentPage },
            function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "success")
                    alert("campaign list data loaded successfully!");
                if (statusTxt == "error")
                    alert("Error: " + xhr.status + "Failed : " + responseTxt);
            }
        );
        
    }
    catch (err) {
        alert('Error Search');
    }
}

function customerDetail(customerId) {
    var SelectedWKCampaignId = $('#SelectedWKCampaignId').val();
    $.ajax({
        type: "Post",
        url: '/Customer/ViewCustomerModalPage/' + customerId, // The method name + paramater
        data: { SelectedWKCampaignId: SelectedWKCampaignId},
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            $('#customerModal').modal();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Customer Data failed to load: " + jqXHR.responseText);
        }
    });
}

$(document).ready(function () {

   // $("#customerList").load("/Customer/LoadCustomerList"); //NOTE: this will recall again customer controller

    //modal events

    $('#btnModalSave').click(function () {
        val1 = $('#cp_pageSize').val();
        val2 = $('#cp_currentPage').val();
        var SelectedWKCampaignId = $('#SelectedWKCampaignId').val();
        $('#PageSize').val(val1); 
        $('#CurrentPage').val(val2);

        //$("#btnModalSubmit").click();

        var frmdata = $("#myForm").serialize();
        $.ajax({
            type: "Post",
            url: '/Customer/ProcessCustomerModalPage/' + SelectedWKCampaignId,
            data:  frmdata,
            success: function (data) {
                alert('Save Completed'+ data);
                $('#customerModal').modal("hide");
                customerSearch(val2);
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Customer data failed to save: " + jqXHR.responseText);
            }
        });
    });

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });

});

$("#menu-toggle").click(function (e) {
    e.preventDefault();
    $("#wrapper").toggleClass("toggled");
});

//$(function () {
//    $('.toggle-menu').click(function (e) {
//        e.preventDefault();
//        $('.sidebar').toggleClass('toggled');
//    });
//});

