$(document).ready(function () {
    $(".editable-description").click(function () {
        var id = $(this).data("id");
        var descriptionSpan = $(this).find("span");
        var originalText = descriptionSpan.text();

        // Replace the text with an input field
        var inputField = $('<input type="text" value="' + originalText + '" />');
        descriptionSpan.html(inputField);

        // Focus on the input field
        inputField.focus();

        inputField.blur(function () {
            // When the input field loses focus (user clicked outside), save the changes
            var newText = inputField.val();
            descriptionSpan.html(newText);

            // Send an AJAX request to update the description
            $.ajax({
                url: '/ToDoes/UpdateDescription',
                method: 'POST',
                data: { id: id, description: newText },
                success: function (result) {
                    $('#tableDiv').html(result);
                },
                error: function (error) {
                    // Handle error, if needed
                }
            });
        });

        inputField.keypress(function (e) {
            // When the user presses Enter, save the changes
            if (e.which == 13) {
                inputField.blur();
            }
        });
    });
});