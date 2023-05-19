using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Next
// TODO: Cut down system into smaller pieces
// TODO: Generate map with tiles from output
// Later
// TODO: Use different noise function for each segment
// TODO: Do not save all neighbors to make more efficient
// TODO: Rewrite as shader if parallelization is possible
// TODO: Fix problem that most segments are long and thin (by penalizing growth when only a few neighbors are available?)

        
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
    public bool showNoise;
    [Range(0, 50)]
    public float noiseScale = 0.5f;
    public float noiseXOrigin;
    public float noiseYOrigin;
    // public List<Sprite> sprites = new();

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

    public void OnValidate()
    {
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
            Initialize();
        if (showNoise)
            SampleNoise();
        Show();
    }

    public override void Initialize()
    {
        // showNoise
        showNoise = false;
        
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
        if (showNoise) showNoise = false;
        for (var i = 0; i < n; i++)
        {
            if (_neighbors[i].Count == 0) _isDone[i] = true;
            if (_isDone[i]) continue;
            
            var choice = ChooseNeighbor(i);
            _isDone[i] = Mark(choice, i);
        }
    }
    
    protected override bool IsDone() => _isDone.All(x => x);

    public override void Show()
    {
        // Create a dictionary that holds _noise
        var channels = new Dictionary<string, float[,]>();
        channels.Add("Noise", _noise);
        Show(_tilemap, channels);
    }

    private void SampleNoise()
    {
        // Sample perlin noise
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            float xCoord = noiseXOrigin + x / (float)width * noiseScale;
            float yCoord = noiseYOrigin + y / (float)height * noiseScale;
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

    // public void Preview()
    // {
    //     if (showNoise) PreviewNoise();
    //     else PreviewTilemap();
    // }
    //
    // public void PreviewTilemap()
    // {
    //     var min = 0;
    //     var max = n - 1;
    //     
    //     // Set up the texture and a Color array to hold pixels during processing.
    //     var texture = new Texture2D(width, height);
    //     var pix = new Color[width * height];
    //     var rend = GetComponent<Renderer>();
    //     rend.sharedMaterial.mainTexture = texture;
    //     
    //     // Convert the values to grayscale pixels.
    //     for (var x = 0; x < width; x++)
    //     for (var y = 0; y < height; y++)
    //         pix[y * width + x] = _tilemap[x, y] != -1
    //         ? Color.HSVToRGB(((float)_tilemap[x, y] - min) / (max - min) / n * (n - 1), 0.8f, 0.8f)
    //         : Color.black;
    //     
    //     // Copy the pixel data to the texture and load it into the GPU.
    //     texture.SetPixels(pix);
    //     texture.filterMode = FilterMode.Point;
    //     texture.Apply();
    // }
    //
    // public void PreviewNoise()
    // {
    //     var max = _noise.Cast<float>().Max();
    //     var min = _noise.Cast<float>().Min();
    //     
    //     // Set up the texture and a Color array to hold pixels during processing.
    //     var texture = new Texture2D(width, height);
    //     var pix = new Color[width * height];
    //     var rend = GetComponent<Renderer>();
    //     rend.sharedMaterial.mainTexture = texture;
    //     
    //     // Convert the values to grayscale pixels.
    //     for (var x = 0; x < width; x++)
    //     for (var y = 0; y < height; y++)
    //         pix[y * width + x] = Color.Lerp(Color.black, Color.white, (_noise[x, y] - min) / (max - min));
    //     
    //     // Copy the pixel data to the texture and load it into the GPU.
    //     texture.SetPixels(pix);
    //     texture.Apply();
    // }

    // private void PreviewAsTexture(int[,] values)
    // {
    //     var converted = new float[values.GetLength(0), values.GetLength(1)];
    //     for (var x = 0; x < values.GetLength(0); x++)
    //     for (var y = 0; y < values.GetLength(1); y++)
    //         converted[x, y] = (float)values[x, y];
    //     PreviewAsTexture(converted);
    // }
    //
    // // Preview a 2D array as a texture.
    // private void PreviewAsTexture(float[,] values)
    // {
    //     var valWidth = values.GetLength(0);
    //     var valLength = values.GetLength(1);
    //     var max = values.Cast<float>().Max();
    //     var min = values.Cast<float>().Min();
    //     
    //     // Set up the texture and a Color array to hold pixels during processing.
    //     var texture = new Texture2D(valWidth, valLength);
    //     var pix = new Color[valWidth * valLength];
    //     var rend = GetComponent<Renderer>();
    //     rend.sharedMaterial.mainTexture = texture;
    //     
    //     // Convert the values to grayscale pixels.
    //     for (var x = 0; x < valWidth; x++)
    //     for (var y = 0; y < valLength; y++)
    //         // pix[y * valWidth + x] = Color.Lerp(Color.black, Color.white, ((float)values[x, y] - min) / (max - min));
    //         pix[y * valWidth + x] = Color.HSVToRGB((values[x, y] - min) / (max - min), 0.8f, 0.8f);
    //     
    //     // Copy the pixel data to the texture and load it into the GPU.
    //     texture.SetPixels(pix);
    //     texture.Apply();
    // }
    //
    // // Print a 2D array to the console.
    // private void PreviewInConsole<T>(T[,] values)
    // {
    //     var valWidth = values.GetLength(0);
    //     var valHeight = values.GetLength(1);
    //     var str = "";
    //     for (var y = 0; y < valHeight; y++)
    //     {
    //         var line = "";
    //         for (var x = 0; x < valWidth; x++)
    //             line += values[x, y] + " ";
    //         str += line + "\n";
    //     }
    //     Debug.Log(str);
    // }

    // Useful with a bigger tileset to get all the sprites in a folder
    // private static IEnumerable<T> GetAssetsAtPath<T>(string path) where T : Object
    // {
    //     var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });
    //     foreach (var guid in assets)
    //     {
    //         yield return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
    //     }
    // }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HighestDescentModel))]
public class HighestDescentModelEditor : GridModelEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HighestDescentModel grid = (HighestDescentModel)target;
        
        if (GUILayout.Button("Toggle Noise Preview"))
        {
            grid.showNoise = !grid.showNoise;
            grid.Show();
        }
        DrawDefaultInspector();
    }
}
#endif