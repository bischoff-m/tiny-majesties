using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ApplyRotation : MonoBehaviour
{
    private void Apply()
    {
        var transform1 = transform;
        var t = transform1.position;
        Debug.Log(new Vector3(MathF.Floor(t.x), MathF.Floor(t.y), 0));
        transform1.position = new Vector3(MathF.Floor(t.x), 0, MathF.Floor(t.z));
        var tilesObj = GameObject.Find("tiles");
        transform1.SetParent(tilesObj.transform);
        transform1.localScale = new Vector3(6.25f, 6.25f, 6.25f);
        transform1.localEulerAngles = new Vector3(0, 0, 0);
    }

#if UNITY_EDITOR
    [CustomEditor (typeof(ApplyRotation))]
    public class TransformEditor : Editor {
        public override void OnInspectorGUI () {
            var applyRotation = (ApplyRotation)target;
            if(GUILayout.Button("Apply")){
                applyRotation.Apply();
            }
            DrawDefaultInspector ();
        }
    }
#endif
}
