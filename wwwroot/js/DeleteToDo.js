$(document).ready(function () {
    $('.DeleteItem').on('click', function () {
        var self = $(this);
        var id = self.attr('id');

        $.ajax({
            url: '/ToDoes/AJAXDelete',
            type: 'POST',
            data: { id: id },
            success: function (result) {
                $('#tableDiv').html(result);
            }
        });
    });
});