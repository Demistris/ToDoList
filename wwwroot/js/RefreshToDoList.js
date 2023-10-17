function refreshToDoList() {
    $.ajax({
        url: '/ToDoes/BuildToDoTable',
        success: function (result) {
            $('#tableDiv').html(result);
        }
    });
}