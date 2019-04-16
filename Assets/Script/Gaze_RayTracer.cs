using UnityEngine;
using System;
using System.IO;
using System.Text;
using static PupilLabs.GazeData;

/// <summary>
/// This script records data each frame in a text file in the following tab-delimited format
/// Frame   Start		HeadX	HeadY	HeadZ	HandX   HandY   HandZ				
///------------------------------------------------------------------------------
/// 1       1241.806	float	float	float   float	float	float	
/// 2       4619.335	float	float	float   float	float	float	
/// </remark>
/// 

namespace PupilLabs
{
    public class Gaze_RayTracer : MonoBehaviour
    {
        public Transform cameraTransform;

        public SubscriptionsController PupilConnection;

        [Header("Debug")]
        public GameObject fake_eyeball;
        public bool use_fake_eyeball = false;

        //Initialize some containers

        GazeListener gazeListener = null;

        Vector3 plGiwVector_xyz;
        Vector3 plEIH0_xyz;
        Vector3 plEIH1_xyz;

        Vector3 plEyeCenter0_xyz;
        Vector3 plEyeCenter1_xyz;

        float plConfidence;
        float plTimeStamp;

        string mode;

        void Start()
        {
            gazeListener = new GazeListener(PupilConnection);
            gazeListener.OnReceive3dGaze += ReceiveGaze;
            if (use_fake_eyeball)
            {
                fake_eyeball = GameObject.Instantiate(fake_eyeball);
                fake_eyeball.transform.Translate(0, 2, 0);
            }
        }


        void ReceiveGaze(GazeData gazeData)
        {

            plConfidence = float.NaN;
            plTimeStamp = float.NaN;
            plGiwVector_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

            plEIH0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
            plEIH1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

            plEyeCenter0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
            plEyeCenter1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

            mode = "none";

            switch (gazeData.Mode)
            {
                
                case GazeData.GazeDataMode.Binocular:

                    plConfidence = gazeData.Confidence;
                    plTimeStamp = gazeData.Timestamp;
                    plGiwVector_xyz = gazeData.GazePoint3d;

                    plEIH0_xyz = gazeData.GazeNormal0;
                    plEIH1_xyz = gazeData.GazeNormal1;

                    plEyeCenter0_xyz = gazeData.EyeCenter0;
                    plEyeCenter1_xyz = gazeData.EyeCenter1;

                    mode = "binocular";
                    break;
                case GazeData.GazeDataMode.Monocular_0:

                    plConfidence = gazeData.Confidence;
                    plTimeStamp = gazeData.Timestamp;
                    plGiwVector_xyz = gazeData.GazePoint3d;

                    plEIH0_xyz = gazeData.GazeNormal0;
                    plEyeCenter0_xyz = gazeData.EyeCenter0;

                    plEIH1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
                    plEyeCenter1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

                    mode = "monocular_0";
                    break;
                case GazeData.GazeDataMode.Monocular_1:

                    plConfidence = gazeData.Confidence;
                    plTimeStamp = gazeData.Timestamp;
                    plGiwVector_xyz = gazeData.GazePoint3d;

                    plEIH0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
                    plEyeCenter0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

                    plEIH1_xyz = gazeData.GazeNormal0;
                    plEyeCenter1_xyz = gazeData.EyeCenter0;

                    mode = "monocular_1";
                    break;
            }
        }

        public void Update()
        {

            int layerMask = 1 << 8;
            //Vector3 eye_vec = Vector3.Normalize(plEIH1_xyz + plEIH1_xyz);
            if (!use_fake_eyeball)
            {
                RaycastHit hit;
                if (Physics.Raycast(cameraTransform.position, Vector3.Normalize(cameraTransform.rotation * plGiwVector_xyz), out hit, Mathf.Infinity, layerMask))
                //if (Physics.Raycast(cameraTransform.position, cameraTransform.rotation * eye_vec, out hit, Mathf.Infinity, layerMask))
                {
                    //Debug.DrawRay(cameraTransform.position, eye_vec * hit.distance, Color.cyan);
                    GameObject obj = hit.collider.gameObject;
                    if (obj.tag == "target")
                    {
                        Target mono = obj.GetComponent<Target>();
                        mono.looked();
                    }
                }
            }
            else
            {
                RaycastHit hit;
                Transform fake_eyeball_trans = fake_eyeball.transform;
                if (Physics.Raycast(fake_eyeball_trans.position, fake_eyeball_trans.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(fake_eyeball_trans.position, fake_eyeball_trans.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
                    GameObject obj = hit.collider.gameObject;
                    if (obj.tag == "target")
                    {
                        Target mono = obj.GetComponent<Target>();
                        mono.looked();
                    }
                }
                else
                {
                    Debug.DrawRay(fake_eyeball_trans.position, fake_eyeball_trans.TransformDirection(Vector3.forward) * 1000, Color.red);
                    Debug.Log("Did not look");
                }
            }
        }
    }
}