using UnityEditor;
using UnityEngine;

public abstract class GridModel : MonoBehaviour
{
    public int seed = 0;
        
    // Whether the algorithm is running
    private bool _isRunning;

    protected GridModel()
    {
        _isRunning = false;
    }
        
    public abstract void Initialize();
    public abstract void Step();
    public abstract void Preview();
    protected abstract bool IsDone();

    public void Start()
    {
        RunComplete();
    }

    public void Update()
    {
        // Run until all segments are done, triggered by inspector button
        if (_isRunning && !IsDone())
        {
            Step();
            Preview();
            if (IsDone())
                _isRunning = false;
        }
    }

    public void RunComplete()
    {
        if (Application.isPlaying)
            _isRunning = true;
        else
            while (!IsDone())
            {
                Step();
                Preview();
            }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridGenerator))]
public class GridModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridModel grid = (GridModel)target;
            
        if (GUILayout.Button("Reset"))
        {
            grid.Initialize();
            grid.Preview();
        }
        if (GUILayout.Button("Step"))
        {
            grid.Step();
            grid.Preview();
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