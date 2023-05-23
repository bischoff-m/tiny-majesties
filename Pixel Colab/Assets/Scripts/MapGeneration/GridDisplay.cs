using UnityEditor;
using UnityEngine;

namespace MapGeneration
{
    public abstract class GridDisplay : MonoBehaviour
    {
        // The model which this display is attached to
        public GridModel model;
    
        public void Start()
        {
            if (model)
            {
                AddSelfAsDisplay();
                Initialize();
            }
            else
                Debug.LogError("GridDisplay has no model");
        }

        public void OnValidate()
        {
            AddSelfAsDisplay();
        }

        private void AddSelfAsDisplay()
        {
            if (model)
                model.AddDisplay(this);
        }

        public abstract void Initialize();
    
        public abstract void OnUpdate(GridUpdateEventArgs args);
    }
}