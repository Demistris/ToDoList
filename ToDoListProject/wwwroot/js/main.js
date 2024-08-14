window.THEURLLIST = {
    sortable: {
        init: function (id, component) {
            new Sortable(document.getElementById(id), {
                handle: ".drag-handle",
                forceFallback: true,
                animation: 350,
                onUpdate: (event) => {
                    component.invokeMethodAsync("Drop", event.oldDraggableIndex, event.newDraggableIndex);
                }
            });
        }
    }
};