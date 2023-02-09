using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Entitas;
using static GridExtensions;
using Random = UnityEngine.Random;

public static class LogicExtensions {
    //2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 1024
    public const int MAX_VALUE = 2048; //2^11
    public const int MAX_EXPONENT = 11;

    public static int GetValue(int exponent) {
        return (int)Math.Pow(2, exponent);
    }

    public static int GetExponent(int val) {
        return (int)Math.Log(val, 2);
    }

    public static void RegenerateGrid(Contexts c) {
        var grid = LogicExtensions.GenerateGrid();
        c.state.Replace<GridC>(grid);
    }

    public static int[] GenerateGrid() {
        var grid = new int[CELL_COUNT]; 
        for (var i = 0; i < 4; i++) {
            for (var j = 0; j < GRID_COLUMNS; j++) {
                var gridIdx = GetGridIndexFromRowAndColumn(i, j);
                var value = GetRandomBubbleValue();
                grid[gridIdx] = value;
            }
        }
        return grid;
    }

    public static int GetRandomBubbleValue() {
        var exponent = Random.Range(1, 7);
        return GetValue(exponent);
    }

    static void MarkShot(Contexts c, int gridIdx) {
        var shootE = c.state.CreateEntity();
        shootE.Set<Shooting>(true);
        shootE.Add<GridIndex>(gridIdx);
    }

    static void RefillShooterBubble(Contexts c) {
        var shooterE = c.state.GetSingleEntity<ShooterBubble>(); 
        shooterE.Remove<GridIndex>();
        var newShooterVal = GetRandomBubbleValue();
        shooterE!.Replace<ShooterBubble>(newShooterVal);
    }

    public static void HandleShot(Contexts c, int gridIdx, int value, int[] grid) {
        grid[gridIdx] = value;
        
        MarkShot(c, gridIdx);
        
        var totalMerges = 0;
        TryMerge(c, gridIdx, value, grid, ref totalMerges);
        
        TryAddNewRows(c, grid); 
        
        RefillShooterBubble(c); 

        var gameOver = TryGameOver(c, grid);
        if (!gameOver) {
            TryCompleteLevel(c);
            c.state.Replace<GridC>(grid);
        }
    }

    static int TryMerge(Contexts c, int gridIdx, int value, int[] grid, ref int totalMerges) {
        var matches = new HashSet<int>() { gridIdx };
        AccumulateMatches(c, gridIdx, value, grid, matches);
        
        var matchAmt = matches.Count;
        if (matchAmt > 1) {
            var mergeRef = totalMerges++;
            var currExponent = GetExponent(value);
            var newValue = GetValue(currExponent + matchAmt - 1);
            newValue = Math.Min(newValue, MAX_VALUE);
            var mergeIdx = PickMergeTarget(c, matches.ToList(), newValue, grid);
            
            MergeCore(c, mergeIdx, value, newValue, grid, matches, mergeRef);
            
            TryClearSeparations(c, grid, mergeRef);
            if (grid[mergeIdx] == 0) { //merged, then fell with separations
                return mergeIdx; 
            }
            
            if (newValue == MAX_VALUE) {
                mergeRef++;
                ExplodeNeighbours(c, mergeIdx, grid, mergeRef); 
                TryClearSeparations(c, grid, mergeRef);
                return mergeIdx;
            }
            
            return TryMerge(c, mergeIdx, newValue, grid, ref totalMerges);
        }
        if (totalMerges > 0) {
            return gridIdx;
        }
        return -1;
    }

    static void MergeCore(Contexts c, int mergeIdx, int value, int newValue, int[] grid, HashSet<int> matches, int mergeRef) {
        foreach (var mGridIdx in matches) {
            grid[mGridIdx] = 0;
        }
        grid[mergeIdx] = newValue;
        IncrementScore(c, value * matches.Count, mergeRef);
        
        void MarkMergeEvent() {
            foreach (var mGridIdx in matches) {
                if (mGridIdx == mergeIdx) {
                    var e = c.state.CreateEntity();
                    e.Add<MergeEvent>(mergeRef);
                    e.Add<GridIndex>(mergeIdx);
                    e.Add<BubbleValue>(newValue);
                    e.Add<OldBubbleValue>(value);
                } else {
                    var e = c.state.CreateEntity();
                    e.Add<Popping>(mergeRef);
                    e.Add<GridIndex>(mGridIdx);
                    e.Add<BubbleValue>(value);
                }
            }
        }
        MarkMergeEvent();
    }
    
    static void AccumulateMatches(Contexts c, int gridIdx, int value, int[] grid, HashSet<int> matches) {
        for (var n = 0; n < CELL_NEIGHBOURS; n++) {
            if (!TryGetGridIndexOfNeighbour(c, gridIdx, n, out var nGridIdx)) continue;
            if (grid[nGridIdx] == value) {
                if (matches.Add(nGridIdx)) {
                    AccumulateMatches(c, nGridIdx, value, grid, matches);
                }    
            }
        }
    }

    static int PickMergeTarget(Contexts c, List<int> matches, int newVal, int[] grid) {
        var mergeRight = Random.Range(0, 1f) > .5f;
        var targetIdx = -1;
        var targetMergeAmt = 0;
        matches.Sort(SortGridIndex); //sort highest prio matches first
        var nextMatches = new HashSet<int>();
        foreach (var idx in matches) {
            nextMatches.Clear();
            nextMatches.Add(idx);
            AccumulateMatches(c, idx, newVal, grid, nextMatches);
            var nextMatchAmt = nextMatches.Count();
            if (nextMatchAmt > targetMergeAmt) { //try merge into most matches
                targetIdx = idx;
                targetMergeAmt = nextMatchAmt;
            }
        }
        if (targetIdx == -1) {
            targetIdx = matches.First();
        }
        return targetIdx;

        //try merge upwards (it's intuitive you always shoot upwards)
        //merge sideways randomly (avoids user needing to optimize for an arbitrary algorithm)
        int SortGridIndex(int i1, int i2) {
            var row1 = GetRowFromGridIndex(i1, out var col1);
            var row2 = GetRowFromGridIndex(i2, out var col2);
            var comp = row1.CompareTo(row2);
            if (comp != 0) return comp;
            comp = col1.CompareTo(col2);
            if (mergeRight) comp *= -1;
            return comp;
        }
    }
    
    
    static void AccumulateConnections(Contexts c, int gridIdx, int[] grid, HashSet<int> connections) {
        for (var n = 0; n < CELL_NEIGHBOURS; n++) {
            if (!TryGetGridIndexOfNeighbour(c, gridIdx, n, out var nGridIdx)) continue;
            var nValue = grid[nGridIdx];
            if (nValue != 0) {
                if (connections.Add(nGridIdx)) {
                    AccumulateConnections(c, nGridIdx, grid, connections);
                }    
            }
        }
    }

    static void ExplodeNeighbours(Contexts c, int mergeIdx, int[] grid, int mergeRef) {
        grid[mergeIdx] = 0;
        IncrementScore(c, MAX_VALUE, mergeRef);
        
        for (var n = 0; n < CELL_NEIGHBOURS; n++) {
            if (!TryGetGridIndexOfNeighbour(c, mergeIdx, n, out var nGridIdx)) continue;
            var val = grid[nGridIdx];
            if (val == 0) continue;
            
            grid[nGridIdx] = 0;
            IncrementScore(c, val, mergeRef);

            void MarkExplosionCollateral() {
                var e = c.state.CreateEntity();
                e.Add<Popping>(mergeRef);
                e.Add<GridIndex>(nGridIdx);
                e.Add<BubbleValue>(val);
            }
            MarkExplosionCollateral();
        }
        
        void MarkExplodeEvent() {
            var e = c.state.CreateEntity();
            e.Set<Explosion2048>(true);
            e.Add<MergeEvent>(mergeRef);
            e.Add<GridIndex>(mergeIdx);
            e.Add<BubbleValue>(MAX_VALUE);
            e.Add<OldBubbleValue>(MAX_VALUE);
        }
        MarkExplodeEvent();
    }

    static void TryClearSeparations(Contexts c, int[] grid, int mergeRef) {
        var connections = new HashSet<int>();
        for (var i = 0; i < GRID_COLUMNS; i++) { //anything that touches top row is connected
            if (grid[i] != 0) {
                AccumulateConnections(c, i, grid, connections);
            }
        }
        for (var i = 0; i < CELL_COUNT; i++) {
            var value = grid[i];
            if (!connections.Contains(i) && value != 0) {
                grid[i] = 0;
                IncrementScore(c, value, mergeRef);
                void MarkBallFalling() {
                    var e = c.state.CreateEntity();
                    e.Add<Falling>(mergeRef);
                    e.Add<GridIndex>(i);
                    e.Add<BubbleValue>(value);
                }
                MarkBallFalling();
            }
        }
    }

    static int GetMaxRow(int[] grid, out bool missingRoot) {
        var maxRow = 0;
        missingRoot = false;
        for (var col = 0; col < GRID_COLUMNS; col++) {
            for (var row = 0; row < GRID_ROWS; row++) {
                var gridIdx = GetGridIndexFromRowAndColumn(row, col);
                var val = grid[gridIdx];
                if (val != 0) {
                    if (row > maxRow) {
                        maxRow = row;
                    }
                } else {
                    if (row == 0) {
                        missingRoot = true;
                    }
                }
            }
        }
        return maxRow;
    }

    static void TryAddNewRows(Contexts c, int[] grid) {
        var maxRow = GetMaxRow(grid, out var missingRoot);
        
        const int minAllowedRow = 3;
        const int maxAllowedRow = GRID_ROWS - 1; 
        var rowsToAdd = Math.Max(0, minAllowedRow - maxRow);
        if (missingRoot) {
            rowsToAdd = Math.Max(1, rowsToAdd);
        }
        rowsToAdd = Math.Min(rowsToAdd, maxAllowedRow - maxRow);
        
        if (rowsToAdd == 0) return;
        var maxNewRow = rowsToAdd - 1;
        //looping backwards and moving balls down. min start operation: row[8] -> row[9]
        for (var row = maxAllowedRow - rowsToAdd; row >= 0; row--) {
            for (var col = 0; col < GRID_COLUMNS; col++) {
                var gridIdx = GetGridIndexFromRowAndColumn(row, col);
                var newIdx = GetGridIndexFromRowAndColumn(row + rowsToAdd, col);
                grid[newIdx] = grid[gridIdx];
                if (row <= maxNewRow) {
                    grid[gridIdx] = GetRandomBubbleValue();
                }
            }
        }

        c.state.Increment<RowsAdded>(rowsToAdd);
        c.state.CreateEntity().Add<SpawnRows>(rowsToAdd);
    }

    static bool TryGameOver(Contexts c, int[] grid) {
        //game over when there is a bubble in max row at the end of the round
        var gameOverIdx = GetGridIndexFromRowAndColumn(GRID_ROWS - 1, 0);
        for (var i = gameOverIdx; i < CELL_COUNT; i++) {
            if (grid[i] != 0) {
                c.state.Set<GameOver>(true);
                c.state.Replace<LevelScore>(0);
                RegenerateGrid(c); 
                return true;
            }
        }
        return false;
    }

    static bool TryCompleteLevel(Contexts c) {
        var levelScore = c.state.GetValueOrDefault<LevelScore, long>();
        var currentLevel = c.state.GetValueOrDefault<Level, int>();
        var targetScore = GetLevelUnlockRequirement(currentLevel + 1);
        if (levelScore > targetScore) {
            c.state.Set<LevelComplete>(true);
            c.state.Replace<Level>(currentLevel + 1);
            c.state.Replace<LevelScore>(0);
            return true;
        }
        return false;
    }

    public static void IncrementScore(Contexts c, long increment, int mergeRef) {
        // var amplifier = mergeRef + 1; could implmenent amplifier
        c.state.Increment<Score>(increment);
        c.state.Increment<LevelScore>(increment);
    }

    public static long GetLevelUnlockRequirement(int level) {
        return 1000 * (int)Math.Pow(1.5, level);
    }
}
