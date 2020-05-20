

function userSearch(currentPage) {
    try {

        var searchText = $('#SearchText').val();
    
        $("#userList").load("/User/LoadUserList",
            { searchText: searchText , currentPage: currentPage },
            function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "success")
                    alert("user list data loaded successfully!");
                if (statusTxt == "error")
                    alert("Error: " + xhr.status + "Failed : " + responseTxt);
            }
        );
        
    }
    catch (err) {
        alert('Error Search');
    }
}

//function userDetail(userId) {
//    //var SelectedWKCampaignId = $('#SelectedWKCampaignId').val();
//    $.ajax({
//        type: "Post",
//        url: '/Customer/ViewUserModalPage/' + userId, // The method name + paramater
//        //data: { SelectedWKCampaignId: SelectedWKCampaignId},
//        success: function (data) {
//            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
//            $('#userModal').modal();
//        },
//        error: function (jqXHR, textStatus, errorThrown) {
//            alert("User Data failed to load: " + jqXHR.responseText);
//        }
//    });
//}

function userList() {
    var userId = $('#hidUserId').val();

    try {
        $("#userList").load("/User/LoadUserList",
            { userId: userId },
            function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "success")
                    alert("User list data loaded successfully!");
                if (statusTxt == "error")
                    alert("Error: " + xhr.status + "Failed : " + responseTxt);
            }
        );
    }
    catch (err) {
        alert("User Listing data failed to load: " + err);

    }
}


function userNew() {
    //var userId = $("#hidUserId").val();

    $.ajax({
        type: "Post",
        url: '/User/UserNewEditModalPage/0',
        //data: { userId: userId },
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            var form = document.getElementById("myForm");
            $(form).removeData("validator")    // Added by jQuery Validation
                .removeData("unobtrusiveValidation");   // Added by jQuery Unobtrusive Validatio
            $.validator.unobtrusive.parse(form);
           
            $('#userModal').modal();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Create User Modal Form failed to load: " + jqXHR.responseText);
        }
    });
}
function userEdit(Id) {
    $.ajax({
        type: "Post",
        url: '/User/UserNewEditModalPage/' + Id, // The method name + paramater
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            var form = document.getElementById("myForm");
            $(form).removeData("validator")    // Added by jQuery Validation
                .removeData("unobtrusiveValidation");   // Added by jQuery Unobtrusive Validatio
            $.validator.unobtrusive.parse(form);
            $('#userModal').modal();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Edit User Modal Form failed to load: " + jqXHR.responseText);
        }
    });
}
function userDelete(Id) {
    var userName = $('#tdN_' + Id).text().trim();
    var flag = confirm('Are you sure you want to delete user record - ' + userName + ' ?');

    if (flag) {
        $.ajax({
            type: "Post",
            url: '/User/Delete/' + Id, // The method name + paramater
            success: function (data) {
                alert("User data deleted succesfully");
                userList();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("User data failed to be deleted: " + jqXHR.responseText);
            }
        });
    }
}

function userChangePassword(Id) {
    $.ajax({
        type: "Post",
        url: '/User/UserChangePasswordModalPage/' + Id, // The method name + paramater
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            var form = document.getElementById("myFormChangePassword");
            $(form).removeData("validator")    // Added by jQuery Validation
                .removeData("unobtrusiveValidation");   // Added by jQuery Unobtrusive Validatio
            $.validator.unobtrusive.parse(form);
            $('#userModal').modal();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Change User Password Modal Form failed to load: " + jqXHR.responseText);
        }
    });
}
$(document).ready(function () {

   
    //modal events

    $('#btnModalSave').click(function () {
        val1 = $('#cp_pageSize').val();
        val2 = $('#cp_currentPage').val();
        $('#PageSize').val(val1); 
        $('#CurrentPage').val(val2);

        //jQuery.validator.setDefaults({
        //    debug: true,
        //    success: "valid"
        //});
        var form;
        var url;
        var formId;
        let myFormChangePassword = document.querySelector('#myFormChangePassword');
        if (myFormChangePassword != null && myFormChangePassword != "undefined") {
            form = myFormChangePassword;
        }
        else
        {
            form = document.querySelector('#myForm');
        }
       
        if (form != null && form != "undefined") {
            formId = form.id;
            switch (formId) {
                case "myFormChangePassword":
                    url = '/User/ChangePassword';
                    break;

                default:
                    url = '/User/ProcessUserModalPage';
            }

        }
        else { alert('No Form found.'); return false; }

        if (!$('#' + formId).valid())
        {
            alert('Form Not Valid');
            return;
        }
        var frmdata = $('#' + formId).serialize();
        //if (!$("#myForm").valid())
        //{
        //    alert('Form Not Valid');
        //    return;
        //}
        //var frmdata = $("#myForm").serialize();
        $.ajax({
            type: "Post",
            url: url,
            data:  frmdata,
            success: function (data) {
                alert('Save Completed'+data);
                $('#userModal').modal("hide");
                userSearch(val2);
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("User data failed to save: " + jqXHR.responseText);
            }
        });
    });


});


