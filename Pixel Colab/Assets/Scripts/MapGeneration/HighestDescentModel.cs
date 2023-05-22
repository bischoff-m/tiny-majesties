using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// TODO: Use different noise function for each segment
// TODO: Do not save all neighbors to make more efficient
// TODO: Rewrite as shader if parallelization is possible
// TODO: Fix problem that most segments are long and thin (by penalizing growth when only a few neighbors are available?)

namespace MapGeneration
{
    // ALGORITHM
    // Inputs
    //     - n: Desired number of segments
    //     - width/height: Dimensions of the map
    // Method:
    //  - Sample Perlin noise into a 2D array with the desired size
    //  - Find the n highest points in the array (each represents the origin of a segment)
    //  - Initialize a 2D array to hold the segment index for each point
    //  - While there is a point that has no segment index
    //  - For each of the segments
    //      - If there is no direct neighbor unoccupied
    //          - Continue
    //      - Assign the current segment to an unoccupied neighbor with the highest value
    // - Return the segmentation

    public class HighestDescentModel : GridModel
    {
        [Range(0, 50)]
        public float noiseScale = 0.5f;
        public float noiseXOrigin;
        public float noiseYOrigin;

        // Values of the noise at each point
        private float[,] _noise;
        // Index of the corresponding segment for each point (-1 is unoccupied)
        private int[,] _tilemap;
        // List of unoccupied neighbors for each segment
        private List<Vector2Int>[] _neighbors;
        // Whether a segment is done
        private bool[] _isDone;
        // Random number generator
        private System.Random _random;

        public new void OnValidate()
        {
            base.OnValidate();
            if (_noise == null
                || _tilemap == null
                || _neighbors == null
                || _isDone == null
                || _noise.GetLength(0) != width
                || _noise.GetLength(1) != height
                || _tilemap.GetLength(0) != width
                || _tilemap.GetLength(1) != height
                || _neighbors.Length != n
                || _isDone.Length != n)
            {
                EditorApplication.delayCall += delegate
                {
                    Initialize();
                    SampleNoise();
                    Show();
                };
            }
            else
            {
                SampleNoise();
                Show();
            }
        }

        protected override void InitializeModel()
        {
            // _random
            _random = new System.Random(seed);
        
            // isRunning
            IsRunning = false;
        
            // _noise
            _noise = new float[width, height];
        
            // _isDone
            _isDone = Enumerable.Repeat(false, n).ToArray();
        
            // _tilemap
            _tilemap = new int[width, height];
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                _tilemap[x, y] = -1;
        
            // _neighbors
            _neighbors = new List<Vector2Int>[n];
            for (var i = 0; i < n; i++)
                _neighbors[i] = new List<Vector2Int>();
        
            SampleNoise();
            ChooseInitPoints();
        }
    
        public override void Step()
        {
            for (var i = 0; i < n; i++)
            {
                if (_neighbors[i].Count == 0) _isDone[i] = true;
                if (_isDone[i]) continue;
            
                var choice = ChooseNeighbor(i);
                _isDone[i] = Mark(choice, i);
            }
        }
    
        protected override bool IsDone() => _isDone.All(x => x);

        public override GridUpdateEventArgs GetData()
        {
            return new GridUpdateEventArgs
            {
                N = n,
                Width = width,
                Height = height,
                Output = _tilemap ?? new int[width, height],
                Channels = new Dictionary<string, float[,]> { { "Noise", _noise } }
            };
        }

        private void SampleNoise()
        {
            // Sample perlin noise
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var xCoord = x / 100f * noiseScale + noiseXOrigin;
                var yCoord = y / 100f  * noiseScale + noiseYOrigin;
                _noise[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }

        // Chooses n random points and assign them to segments
        private void ChooseInitPoints()
        {
            for (var i = 0; i < n; i++)
            {
                var x = _random.Next(0, width);
                var y = _random.Next(0, height);
                _isDone[i] = Mark(new Vector2Int(x, y), i);
            }
        }

        // Sets the chosen point to the segment and updates neighbors
        // Returns true if the segment is done
        private bool Mark(Vector2Int choice, int segment)
        {
            // Assign chosen point to segment
            _tilemap[choice.x, choice.y] = segment;

            // Remove chosen point from neighbors of all segments
            for (var i = 0; i < n; i++)
                _neighbors[i].RemoveAll(choice.Equals);

            // Add neighbors of chosen point to current neighbors
            var neighborsOfChoice = new[]
            {
                new Vector2Int(choice.x - 1, choice.y),
                new Vector2Int(choice.x + 1, choice.y),
                new Vector2Int(choice.x, choice.y - 1),
                new Vector2Int(choice.x, choice.y + 1),
            };
            foreach (var neighbor in neighborsOfChoice)
            {
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height) continue;
                if (_tilemap[neighbor.x, neighbor.y] == -1) _neighbors[segment].Add(neighbor);
            }

            return _neighbors[segment].Count == 0;
        }

        private Vector2Int ChooseNeighbor(int segment)
        {
            var neighbors = _neighbors[segment];
            if (neighbors.Count == 0) throw new Exception("No neighbors to choose from");
        
            // Sample a random neighbor based on the noise value
            var total = neighbors.Select(point => _noise[point.x, point.y]).Sum();
            var rnd = _random.NextDouble();
            foreach (var neighbor in neighbors)
            {
                var value = _noise[neighbor.x, neighbor.y] / total;
                if (rnd < value)
                    return neighbor;
                rnd -= value;
            }
            throw new InvalidOperationException("The proportions in the collection do not add up to 1.");
        
            // ALTERNATIVE ALGORITHM
            // Choose the neighbor with the highest noise value
            // var max = neighbors.Max(point => _noise[point.x, point.y]);
            // return neighbors.First(x => _noise[x.x, x.y] == max);
        }
    }
}