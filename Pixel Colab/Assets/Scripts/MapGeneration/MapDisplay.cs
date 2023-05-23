using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapGeneration
{
    public class MapDisplay : GridDisplay
    {
        public List<GameObject> prefabs = new();
        [Range(0.01f, 10)]
        public float scale = 1;

        // Holds the tile game objects that are currently in the scene
        private GameObject[,] _tiles;
        // Holds the segment indices that correspond to the tiles that are currently in the scene
        private int[,] _lastOutput;
        
        public MapDisplay()
        {
            _tiles = new GameObject[0, 0];
            _lastOutput = new int[0, 0];
        }

        public new void OnValidate()
        {
            base.OnValidate();
            if (prefabs.Count == 0)
                return;
            
            EditorApplication.delayCall += delegate
            {
                if (!model
                    || model.width != _tiles.GetLength(0)
                    || model.height != _tiles.GetLength(1))
                    Initialize();
                else
                {
                    // Update existing tiles with new display data (e.g. position and scale)
                    for (var x = 0; x < State.Width; x++)
                    for (var y = 0; y < State.Height; y++)
                        SetTile(x, y, State.Output[x, y]);
                }
            };
        }
    
        public override void Initialize()
        {
            if (prefabs.Count == 0 || !model || !model.EditingEnabled)
                return;
        
            // Initialize tiles with null
            _tiles = new GameObject[model.width, model.height];
            // Initialize last output with -1
            _lastOutput = new int[model.width, model.height];
            for (var x = 0; x < model.width; x++)
            for (var y = 0; y < model.height; y++)
                _lastOutput[x, y] = -1;
        
            // Destroy old tiles
            for (var i = this.transform.childCount; i > 0; --i)
                DestroyImmediate(this.transform.GetChild(0).gameObject);
        }

        protected override void Draw()
        {
            if (prefabs.Count == 0)
                return;
            if (State.Width != _tiles.GetLength(0) || State.Height != _tiles.GetLength(1))
                Initialize();
        
            for (var x = 0; x < State.Width; x++)
            for (var y = 0; y < State.Height; y++)
                if (_lastOutput[x, y] != State.Output[x, y])
                    SetTile(x, y, State.Output[x, y]);
        }

        private void SetTile(int x, int y, int segment)
        {
            _lastOutput[x, y] = segment;
            var oldTile = _tiles[x, y];
            if (oldTile)
                DestroyImmediate(oldTile);
            
            if (segment == -1)
                return;

            var newPrefab = ChoosePrefab(segment);
            var newTile = Instantiate(newPrefab, transform, true);
            newTile.transform.localPosition = new Vector3(
                (model.width / 2f - x - 0.5f) * scale, 
                (model.height / 2f - y - 0.5f) * scale,
                0);
            newTile.transform.localScale = scale * newPrefab.transform.localScale;
            newTile.name = $"Tile {x}, {y} ({newPrefab.name})";
            _tiles[x, y] = newTile;
        }

        private GameObject ChoosePrefab(int segment)
        {
            return prefabs[segment % prefabs.Count];
        }
    }
}