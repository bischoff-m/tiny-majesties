using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class TileMapManager : MonoBehaviour
{
    public GameObject block;

    private TileMapManager _instance;

    private int _nextUpdate = 1;


    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        // Vector3Int position = new Vector3Int(0, 0, 0);
        // GenerateTile(ground1, position, block);
    }


    //
    // // Update is called once per frame
    private void Update()
    {
        if (!(Time.time >= _nextUpdate)) return; // update only every second
        if (!Input.GetKey("space")) return; // update only when spacebar is pressed
        Debug.Log(Time.time+">="+_nextUpdate);
        // Change the next update (current second+1)
        _nextUpdate=Mathf.FloorToInt(Time.time)+1;
        var ground1 = GameObject.Find("Ground1").GetComponent<Tilemap>();
        ground1.ClearAllTiles();
        Debug.Log(ground1.GetUsedTilesCount());
        var rnd = new Random();
        var gridWidth = rnd.Next(8, 15);
        var gridOriginX = rnd.Next(-3, 3);
        var gridOriginY = rnd.Next(-3, 3);
        for (var x = gridOriginX; x < gridWidth; x++)
        {
            for (var y = gridOriginY; y < gridWidth; y++)
            {
                var position = new Vector3Int(x, -y, ground1.origin.z);
                GenerateTile(ground1, position, block);
            }
        }

        Debug.Log(ground1.GetUsedTilesCount());
    }

    private void Awake()
    {
        _instance = this;
    }

    private static void GenerateTile(Tilemap tileMap, Vector3Int gridPosition, GameObject tileObject)
    {
        var tile = ScriptableObject.CreateInstance<Tile>();
        // var tileBase = ScriptableObject.CreateInstance<TileBase>();
        tile.gameObject = tileObject;
        // tile.AddComponent<TestRaise>();
        tileMap.SetTile(gridPosition, tile);
        // tileMap.GetTile(gridPosition).AddComponent<TestRaise>(); // doesn't work
        // var tb = tileMap.GetTile(gridPosition); // doesn't work
        // tb.AddComponent<TestRaise>(); // doesn't work
    }
}