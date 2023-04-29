using System;
using UnityEngine;

/// <summary>
/// From this tutorial: https://www.youtube.com/watch?v=waEsGu--9P8
/// </summary>
public class CustomGrid {

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs {
        public int X;
        public int Y;
    }

    private readonly int _width;
    private readonly int _height;
    private readonly Vector3 _originPosition;
    private readonly int[,] _gridArray;

    public CustomGrid(int width, int height, Vector3 originPosition) {
        this._width = width;
        this._height = height;
        this._originPosition = originPosition;

        _gridArray = new int[width, height];

        if (Globals.ShowDebug) {
            var debugTextArray = new TextMesh[width][];
            for (var index = 0; index < width; index++)
            {
                debugTextArray[index] = new TextMesh[height];
            }

            for (var x = 0; x < _gridArray.GetLength(0); x++) {
                for (var y = 0; y < _gridArray.GetLength(1); y++) {
                    // Create Text in the World 
                    var gameObject = new GameObject("World_Text", typeof(TextMesh)); 
                    var transform = gameObject.transform; 
                    transform.localPosition = GridToWorld(x, y) + new Vector3(Globals.CellSize, Globals.CellSize) * .5f;
                    var textMesh = gameObject.GetComponent<TextMesh>();
                    textMesh.anchor = TextAnchor.MiddleCenter; 
                    textMesh.alignment = TextAlignment.Center; 
                    textMesh.text = _gridArray[x, y].ToString(); 
                    textMesh.fontSize = 30;
                    textMesh.color = Color.white;
                    textMesh.transform.localScale = Vector3.one * Globals.CellSize * .1f;
                    debugTextArray[x][y] =  textMesh;

                    Debug.DrawLine(GridToWorld(x, y), GridToWorld(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GridToWorld(x, y), GridToWorld(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GridToWorld(0, height), GridToWorld(width, height), Color.white, 100f);
            Debug.DrawLine(GridToWorld(width, 0), GridToWorld(width, height), Color.white, 100f);

            OnGridValueChanged += (sender, eventArgs) => {
                debugTextArray[eventArgs.X][eventArgs.Y].text = _gridArray[eventArgs.X, eventArgs.Y].ToString();
            };
        }
    }

    public int GetWidth() {
        return _width;
    }

    public int GetHeight() {
        return _height;
    }

    private Vector3 GridToWorld(int x, int y) {
        return new Vector3(x, y) * Globals.CellSize + _originPosition;
    }

    private void WorldToGrid(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - _originPosition).x / Globals.CellSize);
        y = Mathf.FloorToInt((worldPosition - _originPosition).y / Globals.CellSize);
    }

    private void SetCellValue(int x, int y, int value) {
        if (x >= 0 && y >= 0 && x < _width && y < _height) {
            _gridArray[x, y] = value;
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { X = x, Y = y });
        }
    }

    public void SetCellValue(Vector3 worldPosition, int value) {
        Debug.Log("SetCellValue");
        Debug.Log(worldPosition);
        Debug.Log(value);
        WorldToGrid(worldPosition, out var x, out var y);
        SetCellValue(x, y, value);
    }

    private int GetCellValue(int x, int y) {
        if (x >= 0 && y >= 0 && x < _width && y < _height) {
            return _gridArray[x, y];
        } else {
            return 0;
        }
    }

    public int GetCellValue(Vector3 worldPosition) {
        WorldToGrid(worldPosition, out var x, out var y);
        return GetCellValue(x, y);
    }

}