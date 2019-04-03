using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class TestCtrl : MonoBehaviour
{
    public GameObject origin_prefab;
    public GameObject target_prefab;

    public string FolderName = "D:\\Data\\WriteoutTest\\1";
    public string FileName = "Test1";


    private string OutputDir;
    public float currentDist;
    protected GameObject origin_gameObj;

    FileStream streams;
    FileStream trialStreams;
    StringBuilder stringBuilder = new StringBuilder();
    String writeString;
    Byte[] writebytes;

    float startTime;
    float endTime;

    bool isAiming;
    // Start is called before the first frame update
    void Start()
    {
        // create a folder 
        string OutputDir = Path.Combine(FolderName, string.Concat(DateTime.Now.ToString("MM-dd-yyyy"), FileName));
        Directory.CreateDirectory(OutputDir);

        // create a file to record data
        String trialOutput = Path.Combine(OutputDir, DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "_Results.txt");
        trialStreams = new FileStream(trialOutput, FileMode.Create, FileAccess.Write);


        //Call the function below to write the column names
        WriteHeader();

        //initiate origin
        origin_gameObj = GameObject.Instantiate(origin_prefab);
        origin_gameObj.transform.Translate(new Vector3(-4,2,0));
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
        "The time is using Unity time." + Environment.NewLine
        );
        stringBuilder.Append("-------------------------------------------------" +
            Environment.NewLine
            );
        //add column names
        stringBuilder.Append(
            "FrameNumber\t" + "StartTime\t" + "EndTime\t" + "ReactionTime\t" + "DistanceToOrigin"+ Environment.NewLine
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
                    + endTime + "\t"
                    + (endTime - startTime) + "\t"
                    + currentDist
                    + Environment.NewLine
                );
        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        trialStreams.Write(writebytes, 0, writebytes.Length);
    }
    
    void Update()
    {

    }
    /// <summary>
    /// Returns a random transform for target spawn in vector3
    /// </summary>
    /// <returns></returns>
    Vector3 randomTransform()
    {
        int dist_level = UnityEngine.Random.Range(1, 4);
        currentDist = dist_level * 0.5f;
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
        if (!isAiming)
        {
            isAiming = true;
            StartCoroutine(wait(1f));
        }
    }
    public void targetClicked()
    {
        if (isAiming)
        {
            endTime = Time.time * 1000;
            WriteFile();
            isAiming = false;
        }
    }
    IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject tar = GameObject.Instantiate(target_prefab);
        tar.transform.Translate(randomTransform());
        Target target_mono = tar.GetComponent<Target>();
        target_mono.setCtroller(this);
        startTime = Time.time * 1000;
    }
}
