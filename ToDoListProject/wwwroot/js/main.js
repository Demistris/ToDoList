window.THEURLLIST = {
    sortable: {
        init: function (id, component) {
            new Sortable(document.getElementById(id), {
                handle: ".toDo-drag-handle",
                forceFallback: true,
                animation: 350,
                onUpdate: (event) => {
                    //event.item.remove();
                    //event.to.insertBefore(event.item, event.to.children[event.oldIndex]);

                    component.invokeMethodAsync("Drop", event.oldDraggableIndex, event.newDraggableIndex);
                }
            });
        }
    }
};