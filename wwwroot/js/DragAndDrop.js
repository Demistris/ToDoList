function OnDragStart(event, itemId) {
    event.dataTransfer.setData("text/plain", itemId);
    console.log("ON DRAG START");
}

function OnDrop(event, targetIndex) {
    event.preventDefault();

    // Get the itemId from the data transfer.
    var itemId = event.dataTransfer.getData("text/plain");
    console.log("ON DROP");

    // Now, you can use 'itemId' and 'targetIndex' in your AJAX request or other logic for reordering.
}