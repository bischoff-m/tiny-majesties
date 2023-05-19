using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class GridModel : MonoBehaviour
{
    // The seed for the random number generator
    public int seed;
    // The width of the output channels
    public int width = 10;
    // The height of the output channels
    public int height = 10;
    // The number of segments in the grid
    [Range(1, 100)]
    public int n = 6;

    // Functions to display the grid and should be called when the grid is updated
    // Input: The output and additional output channels of the model
    // Output: Should not return anything
    private List<Func<int[,], Dictionary<string, float[,]>, object>> _updateHandlers = new();
    // Whether the algorithm is running
    protected bool IsRunning;
    
    public abstract void Initialize();
    public abstract void Step();
    protected abstract bool IsDone();

    public abstract void Show();

    public void Start()
    {
        RunComplete();
    }

    public void Update()
    {
        // Run until all segments are done, triggered by inspector button
        if (IsRunning && !IsDone())
        {
            Step();
            Show();
            if (IsDone())
                IsRunning = false;
        }
    }

    public void RunComplete()
    {
        if (Application.isPlaying)
            IsRunning = true;
        else
            while (!IsDone())
            {
                Step();
                Show();
            }
    }

    protected void Show(int[,] output, Dictionary<string, float[,]> channels)
    {
        var min = 0;
        var max = n - 1;
        
        // Set up the texture and a Color array to hold pixels during processing.
        var texture = new Texture2D(width, height);
        var pix = new Color[width * height];
        var rend = GetComponent<Renderer>();
        rend.sharedMaterial.mainTexture = texture;
        
        // Convert the values to grayscale pixels.
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            pix[y * width + x] = output[x, y] != -1
            ? Color.HSVToRGB(((float)output[x, y] - min) / (max - min) / n * (n - 1), 0.8f, 0.8f)
            : Color.black;
        
        // Copy the pixel data to the texture and load it into the GPU.
        texture.SetPixels(pix);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HighestDescentModel))]
public class GridModelEditor : Editor
{
    public void OnEnable()
    {
        ((GridModel)target).Initialize();
    }

    public override void OnInspectorGUI()
    {
        GridModel grid = (GridModel)target;
        
        if (GUILayout.Button("Reset"))
        {
            grid.Initialize();
            grid.Show();
        }
        if (GUILayout.Button("Step"))
        {
            grid.Step();
            grid.Show();
        }
        if (GUILayout.Button("Run"))
        {
            grid.RunComplete();
        }
        if (GUILayout.Button("Run Next Seed"))
        {
            grid.seed = new System.Random().Next();
            grid.Initialize();
            grid.RunComplete();
        }
        DrawDefaultInspector();
    }
}
#endif