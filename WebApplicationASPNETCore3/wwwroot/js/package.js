

function packageList() {
    var campaignId = $('#hidCampaignId').val();

    try {
        $("#packageList").load("/Package/LoadPackageList",
            { campaignId: campaignId },
            function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "success")
                    alert("Package list data loaded successfully!");
                if (statusTxt == "error")
                    alert("Error: " + xhr.status + "Failed : " + responseTxt);
            }
          );
        }
    catch (err) {
        alert("Package Listing data failed to load: " + err);

    }
}

function packageNew() {
    var campaignId = $("#hidCampaignId").val();

    $.ajax({
        type: "Post",
        url: '/Package/PackageNewEditModalPage/0',
        data: { campaignId: campaignId },
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            $('#packageModal').modal();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Package Data failed to load: " + jqXHR.responseText);
        }
    });
}
function packageEdit(Id) {
    $.ajax({
        type: "Post",
        url: '/Package/PackageNewEditModalPage/' + Id, // The method name + paramater
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            $('#packageModal').modal();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Package Data failed to load: " + jqXHR.responseText);
        }
    });
}

$(document).ready(function () {
   
    $('#btnModalSave').click(function () {
        //var packageId = $('#Id').val();
        var campaignId = $("#hidCampaignId").val();
        var url = '/Package/ProcessPackageModalPage';

        var frmdata = $("#myForm").serialize();
        var frmdata1 = frmdata + "&campaignId=" + campaignId;
        $.ajax({
            type: "Post",
            url: url ,
            data: frmdata1,
            success: function (data) {
                alert('Save Completed'+ data);
                $('#packageModal').modal("hide");
                packageList();
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Package data failed to save: " + jqXHR.responseText);
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

