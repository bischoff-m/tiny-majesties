using UnityEditor;
using UnityEngine;

public abstract class GridDisplay : MonoBehaviour
{
    // The model which this display is attached to
    public GridModel model;

    protected GridDisplay()
    {
        EditorApplication.delayCall += Initialize;
    }
    
    public void Start()
    {
        if (model == null)
            Debug.LogError("GridDisplay has no model");
        else
        {
            AddSelfAsDisplay();
            Initialize();
        }
    }

    public void OnValidate()
    {
        AddSelfAsDisplay();
    }

    private void AddSelfAsDisplay()
    {
        if (model != null)
            model.AddDisplay(this);
    }

    public abstract void Initialize();
    
    public abstract void OnUpdate(GridUpdateEventArgs args);
}