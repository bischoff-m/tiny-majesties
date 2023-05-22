using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MapGeneration
{
    public class GridTextureDisplay : GridDisplay
    {
        private bool _showNoise;
    
        public new void OnValidate()
        {
            base.OnValidate();
            if (GetComponent<Renderer>() == null)
                gameObject.AddComponent<MeshRenderer>();
        }
    
        public override void Initialize()
        {
            if (model)
                OnUpdate(model.GetData());
            else
                GetComponent<Renderer>().sharedMaterial.mainTexture = Texture2D.blackTexture;
        }

        public void ToggleNoise()
        {
            _showNoise = !_showNoise;
            if (model)
                OnUpdate(model.GetData());
        }
    
        public override void OnUpdate(GridUpdateEventArgs args)
        {
            var ratio = (float)args.Width / args.Height;
            transform.localScale = ratio > 1 ? new Vector3(ratio, 1, 1) : new Vector3(1, 1 / ratio, 1);
            if (_showNoise)
                ShowNoise(args);
            else
                ShowOutput(args);
        }

        private void ShowOutput(GridUpdateEventArgs args)
        {
            // Indices of lowest and highest segments
            var min = 0;
            var max = args.N - 1;
        
            // Convert the values to grayscale pixels
            var pix = new Color[args.Width * args.Height];
            var hueScaling = (args.N - 1) / ((float)args.N);
            for (var x = 0; x < args.Width; x++)
            for (var y = 0; y < args.Height; y++)
                pix[y * args.Width + x] = args.Output[x, y] != -1
                    ? Color.HSVToRGB(((float)args.Output[x, y] - min) / (max - min) * hueScaling, 0.8f, 0.8f)
                    : Color.black;
        
            // Set up the texture
            var texture = new Texture2D(args.Width, args.Height);
            var rend = GetComponent<Renderer>();
            rend.sharedMaterial.mainTexture = texture;
        
            // Copy the pixel data to the texture
            texture.SetPixels(pix);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
        }

        private void ShowNoise(GridUpdateEventArgs args)
        {
            var noise = args.Channels["Noise"];
            var max = noise.Cast<float>().Max();
            var min = noise.Cast<float>().Min();
        
            // Set up the texture and a Color array to hold pixels during processing
            var texture = new Texture2D(args.Width, args.Height);
            var pix = new Color[args.Width * args.Height];
            var rend = GetComponent<Renderer>();
            rend.sharedMaterial.mainTexture = texture;
        
            // Convert the values to grayscale pixels
            for (var x = 0; x < args.Width; x++)
            for (var y = 0; y < args.Height; y++)
                pix[y * args.Width + x] = Color.Lerp(Color.black, Color.white, (noise[x, y] - min) / (max - min));
        
            // Copy the pixel data to the texture and load it into the GPU
            texture.SetPixels(pix);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GridTextureDisplay))]
    public class GridTextureDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var grid = (GridTextureDisplay)target;
        
            if (GUILayout.Button("Toggle Noise"))
            {
                grid.ToggleNoise();
            }
            DrawDefaultInspector();
        }
    }
#endif
}