$(document).ready(function () {
    $(document).on("click", ".btnCheckAll", function () {
        var id = $(this).data("id");

        console.log("id: " + id);

        $.ajax({
            type: 'GET',
            url: '/ToDoes/AJAXEditAll',
            data: {
                value: true
            },
            success: function (result) {
                $('#tableDiv').html(result);
            }
        });
    });
});