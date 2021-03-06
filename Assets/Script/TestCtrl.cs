﻿using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class TestCtrl : MonoBehaviour
{
    public GameObject origin_prefab;
    public GameObject target_prefab;
    [Header("Connection")]
    public PupilLabs.Gaze_Logger gaze_logger;

    [Header("Parameter Setting")]

    public float distance_level_min = 1;
    public float distance_level_max = 4;

    public float target_appear_time = 1.5f;
    public float target_scale = 1.0f;

    public bool react_to_gaze = false;

    private string OutputDir;

    float currentDist;
    GameObject origin_gameObj;

    FileStream streams;
    FileStream trialStreams;
    StringBuilder stringBuilder = new StringBuilder();
    String writeString;
    Byte[] writebytes;

    float startTime;
    float gazeTime;
    float aimTime;

    bool aim_ready = false;
    bool hasLooked = false;
    bool hasAimed = false;
    // Start is called before the first frame update
    void Start()
    {
        // create a folder 
        string OutputDir = Path.Combine(Application.dataPath, DateTime.Now.ToString("MM-dd-yyyy"));
        Directory.CreateDirectory(OutputDir);

        // create a file to record data
        String trialOutput = Path.Combine(OutputDir, DateTime.Now.ToString("HH-mm") + "-Reaction_Time.txt");
        trialStreams = new FileStream(trialOutput, FileMode.Create, FileAccess.Write);


        //Call the function below to write the column names
        WriteHeader();

        //initiate origin
        origin_gameObj = GameObject.Instantiate(origin_prefab);

        origin_gameObj.transform.Translate(transform.position);
        origin_gameObj.GetComponent<Origin>().setCtroller(this);
    }


    void WriteHeader()
    {

        //output file-- order of column names here should match the order you use when writing out each value 
        stringBuilder.Length = 0;
        //add header info
        stringBuilder.Append(
        DateTime.Now.ToString() + "\t" +
        "The file contains time data of each successful point movement " + Environment.NewLine +
        "The time is using Unity real time." + Environment.NewLine
        );
        stringBuilder.Append("-------------------------------------------------" +
            Environment.NewLine
            );
        //add column names
        stringBuilder.Append(
            "FrameNumber\t" + "StartTime\t" + "GazeTime\t" + "AimTime\t" + "GazeDuration\t" + "AimDuration\t" + "DistanceToOrigin"+ Environment.NewLine
                        );


        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        trialStreams.Write(writebytes, 0, writebytes.Length);

    }

    void WriteFile()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(
                    Time.frameCount + "\t"
                    + startTime + "\t" 
                    + gazeTime + "\t"
                    + aimTime + "\t"
                    + Math.Max((gazeTime - startTime),0) + "\t"
                    + (aimTime - startTime) + "\t"
                    + currentDist
                    + Environment.NewLine
                );
        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        trialStreams.Write(writebytes, 0, writebytes.Length);
    }
    
    void Update()
    {

        if (aim_ready && hasAimed)
        {
            WriteFile();
            reset_test();
        }
    }
    /// <summary>
    /// Reset the test
    /// </summary>
    void reset_test()
    {
        startTime = float.NaN;
        gazeTime = float.NaN;
        aimTime = float.NaN;
        aim_ready = false;
        hasLooked = false;
        hasAimed = false;
    }

    /// <summary>
    /// Returns a random transform for target spawn in vector3
    /// </summary>
    /// <returns></returns>
    Vector3 randomTransform()
    {
        currentDist = UnityEngine.Random.Range(distance_level_min, distance_level_max);
        float rotate_deg = UnityEngine.Random.Range(0, 360);
        Vector3 tar_pos = Vector3.forward * currentDist;
        tar_pos = transform.rotation * tar_pos;
        tar_pos = Quaternion.Euler(rotate_deg, 0, 0) * tar_pos;
        tar_pos = tar_pos + transform.position;
        return tar_pos;
    }
    public void aimReset()
    {
        if (!aim_ready)
        {
            aim_ready = true;
            StartCoroutine(wait_n_generate_target(target_appear_time));
        }
    }
    public void target_looked()
    {
        if (aim_ready && !hasLooked)
        {
            gazeTime = Time.realtimeSinceStartup;
            hasLooked = true;
        }
    }
    public void target_aimed()
    {
        if (aim_ready && !hasAimed)
        {
            aimTime = Time.realtimeSinceStartup;
            gaze_logger.stop_record();
            hasAimed = true;
        }
    }
    IEnumerator wait_n_generate_target(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject tar = GameObject.Instantiate(target_prefab);
        tar.transform.Translate(randomTransform());
        tar.transform.localScale = new Vector3(target_scale, target_scale, target_scale);
        Target target_mono = tar.GetComponent<Target>();
        target_mono.set_controller(this);
        if (react_to_gaze)
        {
            target_mono.react_gaze = true;
        }
        startTime = Time.realtimeSinceStartup;
        gaze_logger.record();
    }
    public void OnApplicationQuit()
    {
        trialStreams.Close();

    }
}
