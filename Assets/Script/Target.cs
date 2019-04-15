using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    protected TestCtrl testCtrl;
    bool isLooked = false;
    bool isAimed = false;
    public bool react_gaze = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (isLooked && isAimed)
        //{
        //    Destroy(gameObject);
        //}
    }
    public void set_controller(TestCtrl t)
    {
        testCtrl = t;
    }
    public void aimed()
    {
        Debug.Log("Target Aimed");
        testCtrl.target_aimed();
        Destroy(gameObject);
    }
    public void looked()
    {
        Debug.Log("Target Looked");
        if (react_gaze)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
        testCtrl.target_looked();
    }
}
