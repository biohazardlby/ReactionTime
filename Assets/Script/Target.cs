using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    protected TestCtrl testCtrl;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButton("Fire2"))
        //{
        //    trigger();
        //}
    }
    public void setCtroller(TestCtrl t)
    {
        testCtrl = t;
    }
    public void trigger()
    {
        Debug.Log("clickedTarget");
        testCtrl.targetClicked();
        Destroy(gameObject);
    }
}
