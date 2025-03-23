import './split-grid.min.js'

function createConstraintObject(constraints) {
    let cs = {}
    constraints.forEach((c) => { cs[c.track] = c.constraint });
    return cs
}

let _splitterState = {}

export function updateSplitters(id, grid, rowSplitters, columnSplitters, rowMinConstraints, rowMaxConstraints, columnMinConstraints, columnMaxConstraints, rowSnapOffset, columnSnapOffset, rowDragInterval, columnDragInterval) {

    if (_splitterState[id] !== undefined) {
        _splitterState[id].destroy(true)
        delete _splitterState[id]
    }

    if (rowSplitters.length == 0 && columnSplitters.length == 0) {
        return
    }

    let lastDragTemplate = ""
    _splitterState[id] = Split({
        rowGutters: rowSplitters.map((row) => ({ track: row.track, element: document.querySelector(row.cssSelector) })),
        columnGutters: columnSplitters.map((column) => ({ track: column.track, element: document.querySelector(column.cssSelector) })),
        rowMinSizes: createConstraintObject(rowMinConstraints),
        rowMaxSizes: createConstraintObject(rowMaxConstraints),
        columnMinSizes: createConstraintObject(columnMinConstraints),
        columnMaxSizes: createConstraintObject(columnMaxConstraints),
        rowSnapOffset: rowSnapOffset,
        columnSnapOffset: columnSnapOffset,
        rowDragInterval: rowDragInterval,
        columnDragInterval: columnDragInterval,
        onDrag: (direction, track, gridTemplate) => lastDragTemplate = gridTemplate,
        onDragEnd: (direction, track) => grid.invokeMethodAsync("OnSplitterResizedGridAsync", direction == 'row', lastDragTemplate)
    })
}

export function disposeSplitters(id) {
    if (_splitterState[id] !== undefined) {
        delete _splitterState[id]
    }
}
