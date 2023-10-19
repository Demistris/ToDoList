function refreshToDoList() {
    $.ajax({
        url: '/ToDoes/BuildToDoTable',
        success: function (result) {
            var inputElement = document.getElementById("myInput");
            inputElement.value = "New Value";
            console.log("Input value refreshed to 'New Value'");
            $('#tableDiv').html(result);
        }
    });
}