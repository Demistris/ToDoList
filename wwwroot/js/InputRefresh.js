function handleKeyPress(event) {
    if (event.key === 'Enter') {
        var inputElement = document.getElementById("myInput");
        inputElement.value = "";
    }
}