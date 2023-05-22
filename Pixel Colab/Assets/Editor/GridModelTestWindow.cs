using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    // https://docs.unity3d.com/2021.2/Documentation/Manual/UIE-HowTo-CreateEditorWindow.html
    // If implementation is finished, this can be used to show a preview of the output(s) of a grid model
    public class GridModelTestWindow : EditorWindow
    {
        private VisualElement _rightPane;
        
        [MenuItem("Tools/Grid Model Preview")]
        public static void ShowMyEditor()
        {
            // This method is called when the user selects the menu item in the Editor
            EditorWindow wnd = GetWindow<GridModelTestWindow>();
            wnd.titleContent = new GUIContent("Grid Model Preview");
        }

        public void CreateGUI()
        {
            // Create a two-pane view with the left pane being fixed with
            var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);

            // Add the view to the visual tree by adding it as a child to the root element
            rootVisualElement.Add(splitView);

            // A TwoPaneSplitView always needs exactly two child elements
            var leftPane = new ListView();
            splitView.Add(leftPane);
            _rightPane = new VisualElement();
            splitView.Add(_rightPane);
            
            leftPane.makeItem = () => new Label();
            leftPane.bindItem = (item, index) => { ((Label)item).text = "Test"; };
            leftPane.itemsSource = new string[] { "Test" };
            
            leftPane.onSelectionChange += OnSelected;
        }

        private void OnSelected(IEnumerable<object> selected)
        {
            _rightPane.Clear();
            // Set texture of grid model as content of right side
            Debug.Log($"Selection changed: {selected.FirstOrDefault()}");
        }
    }
}