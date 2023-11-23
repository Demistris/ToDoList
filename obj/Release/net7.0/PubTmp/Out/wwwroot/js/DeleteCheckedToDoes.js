$(document).ready(function () {
    $(document).on("click", ".btnDeleteAll", function () {

        $.ajax({
            type: "POST",
            url: '/ToDoes/AJAXDeleteAllChecked',
            success: function (result) {
                $('#tableDiv').html(result);
            }
        });

    });
});