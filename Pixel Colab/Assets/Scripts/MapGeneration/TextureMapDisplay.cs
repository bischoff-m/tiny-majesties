using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public class TextureMapDisplay : MapDisplay
    {
        public List<Texture2D> textures = new();

        public override void Initialize()
        {
            prefabs = new List<GameObject>();
            foreach (var texture in textures)
            {
                var prefab = new GameObject(texture.name);
                var rend = prefab.AddComponent<MeshRenderer>();
                rend.sharedMaterial.mainTexture = texture;
                prefabs.Add(prefab);
            }

            base.Initialize();
        }
    }
}