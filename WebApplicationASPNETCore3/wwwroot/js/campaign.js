
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



$(document).ready(function () {

   // $("#customerList").load("/Customer/LoadCustomerList"); //NOTE: this will recall again customer controller


});

//$(function () {
//    $('.toggle-menu').click(function (e) {
//        e.preventDefault();
//        $('.sidebar').toggleClass('toggled');
//    });
//});

