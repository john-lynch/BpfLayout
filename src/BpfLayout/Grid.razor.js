import './split-grid.js'

function createConstraintObject(constraints) {
    var cs = {}
    constraints.forEach((c) => { cs[c.track] = c.constraint });
    return cs
}

var _split = null
var _grid = null
var _lastDragTemplate = null

export function updateSplitters(grid, rowSplitters, columnSplitters, rowMinConstraints, rowMaxConstraints, columnMinConstraints, columnMaxConstraints, rowSnapOffset, columnSnapOffset, rowDragInterval, columnDragInterval) {

    _grid = grid

    if (_split != null) {
        _split.destroy(true)
        _split = null
    }

    if (rowSplitters.length == 0 && columnSplitters.length == 0) {
        return
    }

    _split = Split({
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
        onDrag: (direction, track, gridTemplate) => _lastDragTemplate = gridTemplate,
        onDragEnd: (direction, track) => _grid.invokeMethodAsync("OnSplitterResizedGridAsync", direction == 'row', track, _lastDragTemplate)
    })
}
