using UnityEngine;
using System.Collections;

public class Controller_RayTracer : MonoBehaviour
{
    public Transform controller_trans;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(controller_trans.position, controller_trans.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(controller_trans.position, controller_trans.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            GameObject obj = hit.collider.gameObject;
            if (obj.tag == "origin")
            {
                Origin mono = obj.GetComponent<Origin>();
                mono.originReset();
            }
            if (obj.tag == "target")
            {
                Target mono = obj.GetComponent<Target>();
                mono.trigger();
            }
        }
        else
        {
            Debug.DrawRay(controller_trans.position, controller_trans.TransformDirection(Vector3.forward) * 1000, Color.red);
            Debug.Log("Did not Hit");
        }
    }
}
