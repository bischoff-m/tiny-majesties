using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapGeneration
{
    public abstract class MapDisplay : GridDisplay
    {
        // The scale of the tiles
        [Range(0.01f, 10)] public float scale = 1;

        // Holds the segment indices that correspond to the tiles that are currently in the scene
        private int[,] _lastOutput;

        // Holds the tile game objects that are currently in the scene
        private GameObject[,] _tiles;

        protected MapDisplay()
        {
            _tiles = new GameObject[0, 0];
            _lastOutput = new int[0, 0];
        }

#if UNITY_EDITOR
        public new void OnValidate()
        {
            base.OnValidate();
            if (!HasTilePrefabs() || State == null)
                return;

            EditorApplication.delayCall += delegate
            {
                if (State.Width != _tiles.GetLength(0) || State.Height != _tiles.GetLength(1))
                    Initialize();
                else
                    // Update existing tiles with new display data (e.g. position and scale)
                    for (var x = 0; x < State.Width; x++)
                    for (var y = 0; y < State.Height; y++)
                        SetTile(x, y, State.Output[x, y]);
            };
        }
#endif

        // Returns the prefab that should be spawned for the given segment with dimensions (1, 1)
        protected abstract GameObject InstantiateTile(int segment);

        // Returns whether there are tile prefabs for every segment that can be spawned
        protected abstract bool HasTilePrefabs();

        // Every MapDisplay has a set of tiles to choose from for every segment
        // This method returns the index of the tile that should be spawned for the given segment
        protected static int ChooseTile<T>(int segment, List<T> tileList)
        {
            return segment % tileList.Count;
        }

        public override void Initialize()
        {
            if (!HasTilePrefabs() || State == null || !Model.EditingEnabled)
                return;

            // Initialize tiles with null
            _tiles = new GameObject[State.Width, State.Height];
            // Initialize last output with -1
            _lastOutput = new int[State.Width, State.Height];
            for (var x = 0; x < State.Width; x++)
            for (var y = 0; y < State.Height; y++)
                _lastOutput[x, y] = -1;

            // Destroy old tiles
            for (var i = transform.childCount; i > 0; --i)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        protected override void Draw()
        {
            if (!HasTilePrefabs())
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

            // var newPrefab = ChoosePrefab(segment);
            var newTile = InstantiateTile(segment);
            // var newTile = Instantiate(newPrefab, transform, true);
            newTile.transform.localPosition = new Vector3(
                (State.Width / 2f - x - 0.5f) * scale,
                (State.Height / 2f - y - 0.5f) * scale,
                0);
            newTile.transform.localScale *= scale;
            // newTile.transform.localScale = scale * newPrefab.transform.localScale;
            newTile.name = $"Tile {x}, {y} ({newTile.name})";
            _tiles[x, y] = newTile;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MapDisplay))]
    public class MapDisplayEditor : GridDisplayEditor
    {
        private Transform _root;

        private void OnEnable()
        {
            _root = ((MapDisplay)target).transform;
        }

        protected override void OnRemove()
        {
            base.OnRemove();
            if (_root.childCount > 0
                && EditorUtility.DisplayDialog(
                    $"{name}",
                    $"Remove all child objects of {name}?",
                    "Remove",
                    "Cancel"))
                for (var i = _root.childCount; i > 0; --i)
                    DestroyImmediate(_root.GetChild(0).gameObject);
        }
    }
#endif
}