


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
            alert("Create Package Modal Form failed to load: " + jqXHR.responseText);
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
            alert("Edit Package Modal Form failed to load: " + jqXHR.responseText);
        }
    });
}

function packageDelete(Id) {
    var packageName = $('#tdN_' + Id).text().trim();
    var flag = confirm('Are you sure you want to delete package record - ' + packageName + ' ?'); 
 
    if (flag) {
        $.ajax({
            type: "Post",
            url: '/Package/Delete/' + Id, // The method name + paramater
            success: function (data) {
                alert("Package data deleted succesfully");
                packageList();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Package data failed to be deleted: " + jqXHR.responseText);
            }
        });
    }
}

function packageCopy() {
    var campaignId = $("#hidCampaignId").val();

    $.ajax({
        type: "Post",
        url: '/Package/PackageCopyModalPage',
        data: { campaignId: campaignId },
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            $('#packageModal').modal();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Create Package Modal Form failed to load: " + jqXHR.responseText);
        }
    });
}
$(document).ready(function () {
   
    $('#btnModalSave').click(function () {
        //var packageId = $('#Id').val();
        var campaignId = $("#hidCampaignId").val();
        var url = '/Package/ProcessPackageModalPage';
        var actionId = $("#hidAction").val();
        if (typeof actionId === "undefined")
            actionId = "";

        var frmdata;
        switch (actionId) {
            case "COPY":
                url = '/Package/ProcessCopyPackageModalPage';
                frmdata = $("#myForm2").serialize();
                if (frmdata.search("&sp=") == -1)  //validate any checkbox click or not
                    {
                    alert('No selection made.');
                    return;
                    }
                break;
            default:
                frmdata = $("#myForm").serialize();
        } 
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

   
});

