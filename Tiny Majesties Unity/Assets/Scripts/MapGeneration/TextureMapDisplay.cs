using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public class TextureMapDisplay : MapDisplay
    {
        public List<Texture2D> textures = new();

        private List<Material> _materials = new();

        // protected override List<GameObject> GetPrefabs()
        // {
        //     return textures.ConvertAll(texture =>
        //     {
        //         var prefab = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //         prefab.name = texture.name;
        //         prefab.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
        //         return prefab;
        //     });
        // var prefabs = new List<GameObject>();
        // foreach (var texture in textures)
        // {
        //     var prefab = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //     prefab.name = texture.name;
        //     prefab.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
        //     prefabs.Add(prefab);
        // }
        // return prefabs;
        // }

#if UNITY_EDITOR
        public new void OnValidate()
        {
            base.OnValidate();
            _materials = textures.ConvertAll(texture => new Material(Shader.Find("Standard"))
            {
                mainTexture = texture
            });
        }
#endif

        protected override GameObject InstantiateTile(int segment)
        {
            var index = ChooseTile(segment, _materials);
            var texture = textures[index];
            var material = _materials[index];

            var prefab = GameObject.CreatePrimitive(PrimitiveType.Plane);
            prefab.GetComponent<MeshRenderer>().sharedMaterial = material;
            prefab.name = texture.name;
            prefab.transform.parent = transform;
            prefab.transform.localScale = 0.1f * Vector3.one;
            return prefab;
        }

        protected override bool HasTilePrefabs()
        {
            return textures.Count > 0;
        }
    }
}