using UnityEditor;
using UnityEngine;

namespace MapGeneration
{
    public abstract class GridDisplay : MonoBehaviour
    {
        // Mirrored from public GridModel Model to handle updates in the editor
        [SerializeField] private GridModel model;

        // The state of the grid
        protected GridState State;

        // The model which this display is attached to
        public GridModel Model { get; private set; }

        public void Start()
        {
            if (Model) Initialize();
            else Debug.LogError("GridDisplay has no model");
        }

#if UNITY_EDITOR
        // Called when the script is loaded or a value is changed in the inspector
        public void OnValidate()
        {
            if (Model != model)
            {
                DetachSelfAsDisplay();
                Model = model;
            }

            AttachSelfAsDisplay();
        }
#endif

        public abstract void Initialize();
        protected abstract void Draw();

        public void SetState(GridState newState)
        {
            State = newState;
            Draw();
        }

        private void AttachSelfAsDisplay()
        {
            if (model) model.AttachDisplay(this);
        }

        private void DetachSelfAsDisplay()
        {
            if (model) model.DetachDisplay(this);
        }
    }

#if UNITY_EDITOR
    // This class is used to detach the display from the model when the component is removed in the editor
    [CustomEditor(typeof(GridDisplay), true)]
    public class GridDisplayEditor : Editor
    {
        // Target of the editor
        private GridDisplay _display;

        // Model that the display is attached to, used to detach the display when the component is removed
        private GridModel _model;

        private void Awake()
        {
            _display = (GridDisplay)target;
            _model = _display.Model;
        }

        public void OnDisable()
        {
            // If component is removed from game object
            if (!_display && _model)
                OnRemove();
        }

        public override void OnInspectorGUI()
        {
            _model = _display.Model;
            DrawDefaultInspector();
        }

        // Called when the component is removed from the game object
        // Can be implemented in child classes
        protected virtual void OnRemove()
        {
            _model.DetachDisplay(_display);
        }
    }
#endif
}