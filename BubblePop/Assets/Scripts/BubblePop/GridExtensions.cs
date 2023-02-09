using Entitas;
using System;
using UnityEngine;

public static class GridExtensions {

    public const int GRID_ROWS = 10; //bubble in 9th row is fine, 10th row is game over
    public const int GRID_COLUMNS = 6;
    public const int CELL_COUNT = GRID_ROWS * GRID_COLUMNS;
    public const float CELL_WIDTH = 720 / 6f + 11;
    public const float CELL_HEIGHT = CELL_WIDTH - 16;
    public const int CELL_NEIGHBOURS = 6;
    
    //tl = 0, 0
    //farmost left in row is always col0

    public static int GetGridIndexFromRowAndColumn(int row, int col) {
        return row * GRID_COLUMNS + col;
    }
    
    public static int GetRowFromGridIndex(int gridIdx, out int col) {
        return Math.DivRem(gridIdx, GRID_COLUMNS, out col);
    }

    public static bool IsGridIndexValid(int gridIdx) {
        return gridIdx >= 0 && gridIdx < CELL_COUNT;
    }

    public static bool IsFirstRowShiftedRight(Contexts c, bool useAnimState = false) {
        var rowsAdded = c.state.GetValueOrDefault<RowsAdded, int>() % 2;
        if (useAnimState && c.state.TryGet<SpawnRows>(out var sC)) {
            rowsAdded -= sC.value;
        }
        return rowsAdded == 0;
    }

    public static bool IsRowShiftedRight(Contexts c, int row, bool useAnimState = false) {
        return row % 2 == 0 == IsFirstRowShiftedRight(c, useAnimState);
    }

    public static Vector2 GetPositionFromGridIndex(Contexts c, int gridIdx, bool useAnimState = false) {
        var row = GetRowFromGridIndex(gridIdx, out var col);
        var x = col * CELL_WIDTH;
        if (IsRowShiftedRight(c, row, useAnimState)) {
            x += CELL_WIDTH / 2;
        }
        var y = -row * CELL_HEIGHT;
        return new Vector2(x, y);
    }

    //neighbourIdx is top-left clockwise (0-indexed)
    public static bool TryGetGridIndexOfNeighbour(Contexts c, int sourceIdx, int neighbourIdx, out int gridIdx) {
        gridIdx = -1;
        var row = GetRowFromGridIndex(sourceIdx, out var col);
        if (neighbourIdx == 5) {
            col--;
        } else if (neighbourIdx == 2) {
            col++;
        } else if (neighbourIdx == 0 || neighbourIdx == 4) {
            if (!IsRowShiftedRight(c, row)) {
                col--;
            }
        } else {
            if (IsRowShiftedRight(c, row)) {
                col++;
            }
        }
        if (col < 0 || col >= GRID_COLUMNS) return false;
        
        //row is used in column calculation so should be calculated 2nd
        if (neighbourIdx == 0 || neighbourIdx == 1) {
            row--;
        } else if (neighbourIdx == 3 || neighbourIdx == 4) {
            row++;
        }
        if (row < 0 || row >= GRID_ROWS) return false;
        
        gridIdx = GetGridIndexFromRowAndColumn(row, col);
        return true;
    }
    
    public static int GetEmptyNeighbourClosestToPoint(Contexts c, int gridIdx, Vector2 localPoint) {
        var grid = c.state.Get<GridC>().value;
        var closestIdx = -1;
        var minDistance = float.MaxValue;
        for (var n = 0; n < CELL_NEIGHBOURS; n++) {
            if (!TryGetGridIndexOfNeighbour(c, gridIdx, n, out var nGridIdx) || grid[nGridIdx] != 0) continue;
            var nPos = GetPositionFromGridIndex(c, nGridIdx);
            var distance = Vector2.Distance(localPoint, nPos);
            if (distance < minDistance) {
                minDistance = distance;
                closestIdx = nGridIdx;
            }
        }
        return closestIdx;
    }

}
