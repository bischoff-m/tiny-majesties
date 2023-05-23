using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapGeneration
{
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
        // Whether changes should be reflected to the outputs (subclasses of GridDisplay)
        public bool EditingEnabled { get; set; }

        // Whether the algorithm is running
        protected bool IsRunning;
        // Subclasses of GridDisplay which should be called when the grid is updated
        private readonly List<GridDisplay> _displays = new();

        protected abstract void InitializeModel();
        public abstract void Step();
        protected abstract bool IsDone();
        protected abstract GridState GetState();
        public abstract bool IsInitialized();

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
    
        public void Initialize()
        {
            IsRunning = false;
            InitializeModel();
            
            // Delayed call because displays could call Instantiate() which should not be done during OnValidate()
            EditorApplication.delayCall += delegate
            {
                foreach (var display in _displays)
                    display.Initialize();
                Show();
            };
        }

        public void Show()
        {
            foreach (var display in _displays)
                display.SetState(GetState());
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
    
        public void AddDisplay(GridDisplay display)
        {
            if (!_displays.Contains(display))
                _displays.Add(display);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GridModel), true)]
    public class GridModelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var grid = (GridModel)target;
            if (grid.EditingEnabled)
            {
                if (GUILayout.Button("Disable Editing"))
                {
                    grid.EditingEnabled = false;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Reset"))
                {
                    grid.Initialize();
                    grid.Show();
                }
                if (GUILayout.Button("Reset To Next Seed"))
                {
                    grid.seed = new System.Random().Next();
                    grid.Initialize();
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
            else
            {
                if (GUILayout.Button("Enable Editing"))
                {
                    grid.EditingEnabled = true;
                    if (!grid.IsInitialized())
                        grid.Initialize();
                    grid.Show();
                }
            }
        }
    }
#endif
}