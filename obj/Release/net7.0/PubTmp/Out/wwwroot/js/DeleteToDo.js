$(document).ready(function () {
    $(document).on("click", ".btnDelete", function () {

        var data = $(this).data("id");

        $.ajax({
            type: "POST",
            url: '/ToDoes/AJAXDelete/',
            data: data,
            success: function (data) {
                $('#tableDiv').html(data);
            }
        });

    });
});