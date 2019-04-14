using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class TestCtrl : MonoBehaviour
{
    public GameObject origin_prefab;
    public GameObject target_prefab;

    public string FolderName = "D:\\Data\\Boyuan_ReactionTime";
    public string FileName = "ReactionTime";

    public float distance_gap = 0.5f;
    public int distance_level_min = 1;
    public int distance_level_max = 4;

    public float target_appear_time = 1.5f;
    public float target_scale = 1.0f;
    public Transform origin_trans;

    private string OutputDir;
    public float currentDist;
    protected GameObject origin_gameObj;

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
        string OutputDir = Path.Combine(FolderName, string.Concat(DateTime.Now.ToString("MM-dd-yyyy") + "-" + FileName));
        Directory.CreateDirectory(OutputDir);

        // create a file to record data
        String trialOutput = Path.Combine(OutputDir, DateTime.Now.ToString("HH-mm") + ".txt");
        trialStreams = new FileStream(trialOutput, FileMode.Create, FileAccess.Write);


        //Call the function below to write the column names
        WriteHeader();

        //initiate origin
        origin_gameObj = GameObject.Instantiate(origin_prefab);
        if (origin_trans == null)
        {
            origin_gameObj.transform.Translate(new Vector3(-4, 3, 0));
        }
        else
        {
            origin_gameObj.transform.Translate(origin_trans.position);
        }
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
        int dist_level = UnityEngine.Random.Range(distance_level_min, distance_level_max);
        currentDist = dist_level * distance_gap;
        float rotate_deg = UnityEngine.Random.Range(0, 360);
        GameObject trans = new GameObject("targetTrans");
        trans.transform.position = origin_gameObj.transform.position;
        trans.transform.Translate(new Vector3(0, 0, currentDist));
        trans.transform.RotateAround(origin_gameObj.transform.position, Vector3.left, rotate_deg);
        Vector3 res = trans.transform.position;
        Destroy(trans);
        return res;
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
        startTime = Time.realtimeSinceStartup;
    }
}
