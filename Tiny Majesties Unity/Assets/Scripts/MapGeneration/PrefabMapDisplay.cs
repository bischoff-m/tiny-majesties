using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration
{
    public class PrefabMapDisplay : MapDisplay
    {
        public List<GameObject> prefabs = new();

        protected override GameObject InstantiateTile(int segment)
        {
            var index = ChooseTile(segment, prefabs);
            var prefab = prefabs[index];
            var tile = Instantiate(prefab, transform, true);
            tile.name = prefab.name;
            return tile;
        }

        protected override bool HasTilePrefabs()
        {
            return prefabs.Count > 0;
        }
    }
}