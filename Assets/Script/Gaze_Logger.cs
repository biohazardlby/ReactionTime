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
    public class Gaze_Logger : MonoBehaviour
    {

        public string FolderName = "D:\\Data\\Boyuan_ReactionTime";
        private string OutputDir;

        //Things you want to write out, set them in the inspector
        public Transform cameraTransform;
        //public GameObject gazeTarget;
        public SubscriptionsController PupilConnection;

        //Gives user control over when to start and stop recording, trigger this with spacebar;
        public bool startWriting;

        //Initialize some containers
        FileStream streams;
        FileStream trialStreams;
        StringBuilder stringBuilder = new StringBuilder();
        String writeString;
        Byte[] writebytes;

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
            // create a folder 
            string OutputDir = Path.Combine(FolderName, string.Concat(DateTime.Now.ToString("MM-dd-yyyy")));
            Directory.CreateDirectory(OutputDir);

            // create a file to record data
            String trialOutput = Path.Combine(OutputDir, DateTime.Now.ToString("HH-mm") + "-Gaze_Info.txt");
            //String trialOutput = Path.Combine(OutputDir, "test.txt");
            trialStreams = new FileStream(trialOutput, FileMode.Create, FileAccess.Write);

            //Call the function below to write the column names
            WriteHeader();

            gazeListener = new GazeListener(PupilConnection);
            gazeListener.OnReceive3dGaze += ReceiveGaze;
        }


        void WriteHeader()
        {

            //output file-- order of column names here should match the order you use when writing out each value 
            stringBuilder.Length = 0;

            //add column names
            stringBuilder.Append(
                "FrameNumber\t"
                + "uTime\t"
                + "cameraPos_X\t"
                + "cameraPos_Y\t"
                + "cameraPos_Z\t"
                + "cameraMat_R0C0\t"
                + "cameraMat_R0C1\t"
                + "cameraMat_R0C2\t"
                + "cameraMat_R0C3\t"
                + "cameraMat_R1C0\t"
                + "cameraMat_R1C1\t"
                + "cameraMat_R1C2\t"
                + "cameraMat_R1C3\t"
                + "cameraMat_R2C0\t"
                + "cameraMat_R2C1\t"
                + "cameraMat_R2C2\t"
                + "cameraMat_R2C3\t"
                + "cameraMat_R3C0\t"
                + "cameraMat_R3C1\t"
                + "cameraMat_R3C2\t"
                + "cameraMat_R3C3\t"
                //+ "targetPos_X\t"
                //+ "targetPos_Y\t"
                //+ "targetPos_Z\t"
                + "plConfidence\t"
                + "plTimestamp\t"
                + "plGiwVector_X\t"
                + "plGiwVector_Y\t"
                + "plGiwVector_Z\t"
                + "plEihVector0_X\t"
                + "plEihVector0_Y\t"
                + "plEihVector0_Z\t"
                + "plEihVector1_X\t"
                + "plEihVector1_Y\t"
                + "plEihVector1_Z\t"
                + "plEyeCenter0_X\t"
                + "plEyeCenter0_Y\t"
                + "plEyeCenter0_Z\t"
                + "plEyeCenter1_X\t"
                + "plEyeCenter1_Y\t"
                + "plEyeCenter1_Z\t"
                + "mode\t"
                + Environment.NewLine
                            );


            writeString = stringBuilder.ToString();
            writebytes = Encoding.ASCII.GetBytes(writeString);
            trialStreams.Write(writebytes, 0, writebytes.Length);

        }

        void ReceiveGaze(GazeData gazeData)
        {

            //if (startWriting)
            //{

            //    float a = 1;

            //    if (gazeData!=null)
            //    {
            //        if(gazeData.Mode == GazeData.GazeDataMode.Binocular)
            //        {

            //            Debug.Log("Moooop");
            //        }


            //    }
            //}


            if (startWriting)
            {

                plConfidence = float.NaN;
                plTimeStamp = float.NaN;
                plGiwVector_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

                plEIH0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
                plEIH1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

                plEyeCenter0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
                plEyeCenter1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

                mode = "none";

                if (startWriting)
                {

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


            }
        }
            void WriteFile()
        {


            Vector4 camRow0 = cameraTransform.localToWorldMatrix.GetRow(0);
            Vector4 camRow1 = cameraTransform.localToWorldMatrix.GetRow(1);
            Vector4 camRow2 = cameraTransform.localToWorldMatrix.GetRow(2);
            Vector4 camRow3 = cameraTransform.localToWorldMatrix.GetRow(3);

            stringBuilder.Length = 0;
            stringBuilder.Append(

                        Time.frameCount + "\t"
                        + Time.realtimeSinceStartup * 1000 + "\t"

                        + cameraTransform.position.x.ToString() + "\t"
                        + cameraTransform.position.y.ToString() + "\t"
                        + cameraTransform.position.z.ToString() + "\t"
                        
                        + camRow0[0].ToString() + "\t"
                        + camRow0[1].ToString() + "\t"
                        + camRow0[2].ToString() + "\t"
                        + camRow0[3].ToString() + "\t"

                        + camRow1[0].ToString() + "\t"
                        + camRow1[1].ToString() + "\t"
                        + camRow1[2].ToString() + "\t"
                        + camRow1[3].ToString() + "\t"

                        + camRow2[0].ToString() + "\t"
                        + camRow2[1].ToString() + "\t"
                        + camRow2[2].ToString() + "\t"
                        + camRow2[3].ToString() + "\t"

                        + camRow3[0].ToString() + "\t"
                        + camRow3[1].ToString() + "\t"
                        + camRow3[2].ToString() + "\t"
                        + camRow3[3].ToString() + "\t"

                        //+ gazeTarget.transform.position.x.ToString() + "\t"
                        //+ gazeTarget.transform.position.y.ToString() + "\t"
                        //+ gazeTarget.transform.position.z.ToString() + "\t"

                        + plConfidence.ToString() + "\t"
                        + plTimeStamp.ToString() + "\t"

                        + plGiwVector_xyz.x.ToString() + "\t"
                        + plGiwVector_xyz.y.ToString() + "\t"
                        + plGiwVector_xyz.z.ToString() + "\t"

                        + plEIH0_xyz.x.ToString() + "\t"
                        + plEIH0_xyz.y.ToString() + "\t"
                        + plEIH0_xyz.z.ToString() + "\t"

                        + plEIH1_xyz.x.ToString() + "\t"
                        + plEIH1_xyz.y.ToString() + "\t"
                        + plEIH1_xyz.z.ToString() + "\t"

                        + plEyeCenter0_xyz.x.ToString() + "\t"
                        + plEyeCenter0_xyz.y.ToString() + "\t"
                        + plEyeCenter0_xyz.z.ToString() + "\t"

                        + plEyeCenter1_xyz.x.ToString() + "\t"
                        + plEyeCenter1_xyz.y.ToString() + "\t"
                        + plEyeCenter1_xyz.z.ToString() + "\t"

                        + mode + "\t"

                        + Environment.NewLine
                    );
            writeString = stringBuilder.ToString();
            writebytes = Encoding.ASCII.GetBytes(writeString);
            trialStreams.Write(writebytes, 0, writebytes.Length);
        }

        public void Update()
        {

            //Use spacebar to initiate/stop recording values, you can change this if you want 
            if (Input.GetKeyDown(KeyCode.Space))
            {
                startWriting = !startWriting;
                if (startWriting)
                {
                    Debug.Log("Start writing");
                }
                else
                {
                    Debug.Log("Stop writing");
                }
            }
            if (startWriting)
            {
                WriteFile();
            }


        }

        public void OnApplicationQuit()
        {
            trialStreams.Close();

        }

    }
}