


function teamDistributionList() {
    var campaignId = $('#hidCampaignId').val();

    try {
        $("#teamDistributionList").load("/TeamDistribution/LoadTeamDistributionList",
            { campaignId: campaignId },
            function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "success")
                    alert("Team Distribution list data loaded successfully!");
                if (statusTxt == "error")
                    alert("Error: " + xhr.status + "Failed : " + responseTxt);
            }
          );
        }
    catch (err) {
        alert("Team Distribution Listing data failed to load: " + err);

    }
}

//render html data from server to modal dialog
function teamDistributionNew() {
    var campaignId = $("#hidCampaignId").val();

    $.ajax({
        type: "Post",
        url: '/TeamDistribution/TeamDistributionNewEditModalPage/0',
        data: { campaignId: campaignId },
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            $('#teamDistributionModal').modal();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Create  TeamDistribution Modal Form failed to load: " + jqXHR.responseText);
        }
    });
}

function teamDistributionDelete(Id, tLevel) {
    var campaignId = $("#hidCampaignId").val();
    var teamDistributionName = $('#tdN_' + Id).text().trim();
    var flag = confirm('Are you sure you want to delete  record - ' + teamDistributionName + ' ?');

    if (flag) {
        $.ajax({
            type: "Post",
            url: '/TeamDistribution/Delete/' + Id,
            data: { campaignId: campaignId, tLevel: tLevel },
            success: function (data) {
                alert("Team Distribution data deleted succesfully");
                teamDistributionList();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Team Distribution data failed to be deleted: " + jqXHR.responseText);
            }
        });
    }
}


$(document).ready(function () {
   
    $('#btnModalSave').click(function () {
       
        var campaignId = $("#hidCampaignId").val();
        var url = '/TeamDistribution/ProcessTeamDistributionModalPage';
        
        var frmdata = $("#myForm").serialize();
        var frmdata1 = frmdata + "&campaignId=" + campaignId;
        $.ajax({
            type: "Post",
            url: url ,
            data: frmdata1,
            success: function (data) {
                alert('Save Completed'+ data);
                $('#teamDistributionModal').modal("hide");
                teamDistributionList();
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("TeamDistribution data failed to save: " + jqXHR.responseText);
            }
        });
    });

   
});

