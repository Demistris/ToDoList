$(document).ready(function () {
    $(document).on("click", ".btnCheckAll", function () {
        var id = $(this).data("id");
        var value;

        console.log("id: " + id);

        if (id == 0) {
            value = false;
        }

        if (id == 1) {
            value = true;
        }

        console.log("Value: " + value);

        $.ajax({
            type: "POST",
            url: '/ToDoes/AJAXEditAll',
            data: {
                value: value
            },
            success: function (result) {
                $('#tableDiv').html(result);
            }
        });

    });
});