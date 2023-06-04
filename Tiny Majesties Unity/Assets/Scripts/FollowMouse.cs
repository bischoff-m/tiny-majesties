using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    private NetworkObject _netObject;
    private Camera _camera;
    
    // Start is called before the first frame update
    private void Start()
    {
        _netObject = GetComponent<NetworkObject>();
        _camera = Camera.main;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!_netObject.IsOwner) return;
        var mouse2D = Mouse.current.position.ReadValue();
        var mouse3D = _camera.ScreenToWorldPoint(mouse2D);
        mouse3D.x = Math.Clamp(mouse3D.x, -5f, 5f);
        mouse3D.y = 0;
        mouse3D.z = Math.Clamp(mouse3D.z, -4.5f, 4.5f);
        
        transform.position = mouse3D;
    }
}
