using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject birdViewGrid = GameObject.Find("BirdViewGrid");
        var myTileMap = birdViewGrid.GetComponent<Tilemap>();
        Tile t = new Tile();
        // t.
        // myTileMap.SetTile(new Vector3Int(1,1,1), new Tile());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
