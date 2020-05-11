// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function customerDetail() {
    alert('hello');

}
function customerDetail1(customerId) {
    $.ajax({
        url: '/Customer/ViewCustomerModalPage/' + customerId, // The method name + paramater
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            $('#customerModal').modal();
        },
        error: function () {
            alert("Customer Data failed to load.");
        });
}

$(function () {
    $('#approve-btn').click(function () {
        $('#customerModal').modal('hide');
    });
});

    $(function () {
        // Initialize numeric spinner input boxes
        //$(".numeric-spinner").spinedit();

        // Initalize modal dialog
        // attach modal-container bootstrap attributes to links with .modal-link class.
        // when a link is clicked with these attributes, bootstrap will display the href content in a modal dialog.
        $('body').on('click', '.modal-link', function (e) {
            e.preventDefault();
            $(this).attr('data-target', '#modal-container');
            $(this).attr('data-toggle', 'modal');
        });

    // Attach listener to .modal-close-btn's so that when the button is pressed the modal dialog disappears
            $('body').on('click', '.modal-close-btn', function () {
        $('#modal-container').modal('hide');
});

//clear modal cache, so that new content can be loaded
            $('#modal-container').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
});

            $('#CancelModal').on('click', function () {
                return false;
});
});
