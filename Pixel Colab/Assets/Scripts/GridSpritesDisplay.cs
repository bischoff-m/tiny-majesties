using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridSpritesDisplay : GridDisplay
{
    public List<Sprite> sprites = new();
    [Range(0.01f, 10)]
    public float scale = 1;

    private int _width;
    private int _height;
    private GameObject[,] _tiles;
    private int[,] _lastOutput;

    public new void OnValidate()
    {
        base.OnValidate();
        EditorApplication.delayCall += delegate
        {
            if (!model
                || model.width != _width
                || model.height != _height)
                Initialize();
            else
            {
                var newData = model.GetData().Output;
                for (var x = 0; x < _width; x++)
                for (var y = 0; y < _height; y++)
                    SetTile(x, y, newData[x, y]);
            }
        };
    }
    
    public override void Initialize()
    {
        _width = !model ? 0 : model.width;
        _height = !model ? 0 : model.height;
        
        // Destroy old tiles
        for (var i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        
        // Instantiate new tiles
        _tiles = new GameObject[_width, _height];
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
        {
            _tiles[x, y] = new GameObject($"Tile ({x}, {y})")
            {
                transform =
                {
                    parent = transform,
                    localPosition = new Vector3((_width - x - 0.5f) * scale, (_height - y - 0.5f) * scale, 0),
                    localScale = new Vector3(scale, scale, scale),
                    localRotation = Quaternion.identity,
                }
            };
            _tiles[x, y].AddComponent<SpriteRenderer>();
            if (model && _lastOutput != null)
                SetTile(x, y, _lastOutput[x, y]);
        }
        
        _lastOutput = new int[_width, _height];
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
            _lastOutput[x, y] = -1;
    }
    
    public override void OnUpdate(GridUpdateEventArgs args)
    {
        if (sprites.Count == 0)
            return;
        if (args.Width != _width || args.Height != _height)
            Initialize();
        
        for (var x = 0; x < args.Width; x++)
        for (var y = 0; y < args.Height; y++)
            if (_lastOutput[x, y] != args.Output[x, y])
                SetTile(x, y, args.Output[x, y]);
    }

    private void SetTile(int x, int y, int segment)
    {
        _tiles[x, y].transform.localPosition = new Vector3((_width - x - 0.5f) * scale, (_height - y - 0.5f) * scale, 0);
        _tiles[x, y].transform.localScale = new Vector3(scale, scale, scale);
        var spriteRenderer = _tiles[x, y].GetComponent<SpriteRenderer>();
        if (segment != -1)
        {
            spriteRenderer.sprite = sprites[segment % sprites.Count];
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.sprite = null;
            spriteRenderer.color = Color.black;
        }
        _lastOutput[x, y] = segment;
    }
}