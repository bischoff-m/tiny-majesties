using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaise : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseOver()
    {
        print("mouse over");
        var go = this.gameObject;
        go.transform.localScale = (new Vector3(10, 10, 0));
    }

}
