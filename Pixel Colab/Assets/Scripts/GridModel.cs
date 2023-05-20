using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    // Subclasses of GridDisplay which should be called when the grid is updated
    private readonly List<GridDisplay> _displays = new();
    // Whether the algorithm is running
    protected bool IsRunning;

    protected abstract void InitializeModel();
    public abstract void Step();
    protected abstract bool IsDone();

    public abstract GridUpdateEventArgs GetData();
    
    public void Initialize()
    {
        InitializeModel();
        EditorApplication.delayCall += delegate
        {
            foreach (var display in _displays)
                display.Initialize();
        };
        Show();
    }

    public void Show()
    {
        OnUpdate(GetData());
    }

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
            if (IsDone())
            {
                IsRunning = false;
                Show();
            }
        }
    }

    public void FixedUpdate()
    {
        if (IsRunning)
            Show();
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

    private void OnUpdate(GridUpdateEventArgs args)
    {
        foreach (var display in _displays)
            display.OnUpdate(args);
    }
    
    public void AddDisplay(GridDisplay display)
    {
        if (!_displays.Contains(display))
            _displays.Add(display);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridModel))]
public class GridModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var grid = (GridModel)target;
        
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