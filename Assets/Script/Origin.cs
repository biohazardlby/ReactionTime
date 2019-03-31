using UnityEngine;
using System.Collections;

public class Origin : MonoBehaviour
{
    protected TestCtrl testCtrl;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            originReset();
        }
    }
    public void setCtroller(TestCtrl t)
    {
        testCtrl = t;
    }
    public void originReset()
    {
        Debug.Log("clickedOrigin");
        testCtrl.aimReset();
    }
}
