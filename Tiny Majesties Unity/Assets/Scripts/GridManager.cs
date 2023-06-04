using UnityEngine;
using Mirror;

public class GridManager : NetworkBehaviour
{
    private CustomGrid _customGrid;

    // /// <summary>
    // /// Server side handler for grid update.
    // /// </summary>
    // [Command]
    // public void UpdatePixel(int x, int y, Color newColor)
    // {
    //     RpcUpdatePixel(x, y, newColor);
    // }
    //
    // /// <summary>
    // /// Client side handler for grid update.
    // /// </summary>
    // [ClientRpc]
    // // ReSharper disable once MemberCanBePrivate.Global
    // public void RpcUpdatePixel(int x, int y, Color newColor)
    // {
    //     
    // }

    // Start is called before the first frame update
    private void Start()
    {
        _customGrid = new CustomGrid(10, 10, new Vector3(0, 0));
    }

    // Update is called once per frame
    private void Update()
    {
        var mainCam = Camera.main;
        if (!mainCam) return;

        var mousePos = Input.mousePosition;
        mousePos.z = 10; // Distance of the plane from the camera
        mousePos = mainCam.ScreenToWorldPoint(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            _customGrid.SetCellValue(mousePos, 1);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(_customGrid.GetCellValue(mousePos));
        }
    }
}