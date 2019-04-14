using UnityEngine;
using System.Collections;

public class Controller_RayTracer : MonoBehaviour
{
    public Transform controller_trans;
    LineRenderer line_render;
    // Use this for initialization
    void Start()
    {
        line_render = GetComponent<LineRenderer>();
        if (line_render == null)
        {
            Debug.LogWarning("No line renderer found in controller raytracer");
        }
        else
        {
            line_render.positionCount = 2;
            line_render.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 8;
        if (line_render != null)
        {

            line_render.SetPosition(0, controller_trans.position);
            line_render.SetPosition(1, controller_trans.position + (controller_trans.forward*10));
        }

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(controller_trans.position, controller_trans.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            //Debug.DrawRay(controller_trans.position, controller_trans.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            GameObject obj = hit.collider.gameObject;
            if (obj.tag == "origin")
            {
                Origin mono = obj.GetComponent<Origin>();
                mono.originReset();
            }
            if (obj.tag == "target")
            {
                Target mono = obj.GetComponent<Target>();
                mono.aimed();
            }
        }
        else
        {
            //Debug.DrawRay(controller_trans.position, controller_trans.TransformDirection(Vector3.forward) * 1000, Color.red);
            Debug.Log("Did not Hit");
        }
    }
}
