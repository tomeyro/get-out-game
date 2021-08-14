using Godot;
using System.Collections;
using System;

public struct AvailableConnections {
    public bool up;
    public bool down;
    public bool right;
    public bool left;

    public override string ToString()
    {
        return $"U:{up} R:{down} D:{right} L:{left}";
    }
}

public class MazeGenerator
{
    int GridSize;
    int[,] Grid;
    ArrayList[,] Connections;

    RandomNumberGenerator Random;

    public MazeGenerator()
    {
        Random = new RandomNumberGenerator();
    }

    int[] _NextCell(int[] cell, int[,] grid)
    {
        var availableCells = new ArrayList();
        if (cell[0] - 1 >= 0 && grid[cell[0] - 1, cell[1]] == 0) {
            availableCells.Add(new int[] {cell[0] - 1, cell[1]});
        }
        if (cell[0] + 1 < GridSize && grid[cell[0] + 1, cell[1]] == 0) {
            availableCells.Add(new int[] {cell[0] + 1, cell[1]});
        }
        if (cell[1] - 1 >= 0 && grid[cell[0], cell[1] - 1] == 0) {
            availableCells.Add(new int[] {cell[0], cell[1] - 1});
        }
        if (cell[1] + 1 < GridSize && grid[cell[0], cell[1] + 1] == 0) {
            availableCells.Add(new int[] {cell[0], cell[1] + 1});
        }
        if (availableCells.Count == 0) {
            return null;
        }
        return (int[]) availableCells[Random.RandiRange(0, availableCells.Count - 1)];
    }

    bool _CheckConnection(ArrayList connection, int[] cell)
    {
        foreach (int[] connectedCell in connection) {
            if ($"{cell[0]},{cell[1]}" == $"{connectedCell[0]},{connectedCell[1]}") {
                return true;
            }
        }
        return false;
    }

    void _PrintMaze(int[,] grid, ArrayList[,] connections)
    {
        var maze = "";
        for (int row = 0; row < GridSize; row ++) {
            for (int col = 0; col < GridSize; col ++) {
                if (connections[row, col] == null) {
                    maze += "····  ";
                    continue;
                }
                var conn = "";
                conn += _CheckConnection(connections[row, col], new int[] {row - 1, col}) ? '↑' : '·';
                conn += _CheckConnection(connections[row, col], new int[] {row, col + 1}) ? '→' : '·';
                conn += _CheckConnection(connections[row, col], new int[] {row + 1, col}) ? '↓' : '·';
                conn += _CheckConnection(connections[row, col], new int[] {row, col - 1}) ? '←' : '·';
                maze += conn + "  ";
            }
            maze += "\n";
        }
        GD.Print(maze);
    }

    public void Generate(int gridSize)
    {
        GridSize = gridSize;
        Random.Randomize();

        Grid = new int[GridSize, GridSize];
        Connections = new ArrayList[GridSize, GridSize];

        var firstCell = new int[] {Random.RandiRange(0, GridSize - 1), Random.RandiRange(0, GridSize - 1)};
        Grid[firstCell[0], firstCell[1]] += 1;

        var trackback = new ArrayList();
        trackback.Add(firstCell);

        while (trackback.Count > 0) {
            var lastIndex = trackback.Count - 1;
            var currentCell = (int[]) trackback[lastIndex];
            var nextCell = _NextCell(currentCell, Grid);
            if (nextCell != null) {
                Grid[nextCell[0], nextCell[1]] += 1;
                trackback.Add(nextCell);
                if (Connections[currentCell[0], currentCell[1]] == null) {
                    Connections[currentCell[0], currentCell[1]] = new ArrayList();
                }
                if (!_CheckConnection(Connections[currentCell[0], currentCell[1]], nextCell)) {
                    Connections[currentCell[0], currentCell[1]].Add(nextCell);
                }
                if (Connections[nextCell[0], nextCell[1]] == null) {
                    Connections[nextCell[0], nextCell[1]] = new ArrayList();
                }
                if (!_CheckConnection(Connections[nextCell[0], nextCell[1]], currentCell)) {
                    Connections[nextCell[0], nextCell[1]].Add(currentCell);
                }
                continue;
            }
            trackback.RemoveAt(lastIndex);
        }

        // for (int row = 0; row < GridSize; row ++) {
        //     for (int col = 0; col < GridSize; col ++) {
        //         var con = Connections[row, col];
        //         var x = $"{row},{col} => ";
        //         foreach (int[] c in con) {
        //             x += $" {c[0]},{c[1]}";
        //         }
        //         GD.Print(x);
        //     }
        // }
        // GD.Print("");

        // _PrintMaze(Grid, Connections);
    }

    public AvailableConnections GetConnections(int row, int col)
    {
        var ac = new AvailableConnections();
        ac.up = _CheckConnection(Connections[row, col], new int[] {row - 1, col});
        ac.down = _CheckConnection(Connections[row, col], new int[] {row + 1, col});
        ac.right = _CheckConnection(Connections[row, col], new int[] {row, col + 1});
        ac.left = _CheckConnection(Connections[row, col], new int[] {row, col - 1});
        return ac;
    }
}
