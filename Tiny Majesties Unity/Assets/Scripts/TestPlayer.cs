using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float speed = 1f;
    
    private CharacterController _controller;
    
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _controller.Move(speed * Time.deltaTime * move);
    }
}
