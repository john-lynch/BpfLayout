import './split-grid.js'

function createConstraintObject(constraints) {
    var cs = {}
    constraints.forEach((c) => { cs[c.track] = c.constraint });
    return cs
}

export function initializeSplitters(rowSplitters, columnSplitters, rowMinConstraints, rowMaxConstraints, columnMinConstraints, columnMaxConstraints, rowSnapOffset, columnSnapOffset, rowDragInterval, columnDragInterval) {

    Split({
        rowGutters: rowSplitters.map((row) => ({ track: row.track, element: document.querySelector(row.cssSelector) })),
        columnGutters: columnSplitters.map((column) => ({ track: column.track, element: document.querySelector(column.cssSelector) })),
        rowMinSizes: createConstraintObject(rowMinConstraints),
        rowMaxSizes: createConstraintObject(rowMaxConstraints),
        columnMinSizes: createConstraintObject(columnMinConstraints),
        columnMaxSizes: createConstraintObject(columnMaxConstraints),
        rowSnapOffset: rowSnapOffset,
        columnSnapOffset: columnSnapOffset,
        rowDragInterval: rowDragInterval,
        columnDragInterval: columnDragInterval
    })
}
