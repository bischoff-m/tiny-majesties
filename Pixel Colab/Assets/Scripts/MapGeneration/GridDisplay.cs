using UnityEngine;

namespace MapGeneration
{
    public abstract class GridDisplay : MonoBehaviour
    {
        // The model which this display is attached to
        public GridModel model;
        // The state of the grid
        protected GridState State;
    
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
        
        public void SetState(GridState newState)
        {
            State = newState;
            Draw();
        }

        private void AddSelfAsDisplay()
        {
            if (model)
                model.AddDisplay(this);
        }

        public abstract void Initialize();

        protected abstract void Draw();
    }
}