using UnityEngine;
using System.Threading;
using System.Diagnostics;
//using System;
//using Vosk;

public class Text : MonoBehaviour
{

    private UnityEngine.UI.Text textContent;
    private float time = 5f;

    private Thread thread;

    public string text = "";

    public class Json
    {
        [Newtonsoft.Json.JsonProperty("text")]
        public int content { get; set; }
    }

    void Start()
    {
        textContent = GetComponent<UnityEngine.UI.Text>();
        thread = new Thread(GetInfo);
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        textContent.text = text;

        //time -= Time.deltaTime;

        //if (time < 0)
        //{
        //    textContent.text = "";

        //    time = 5;
        //}
    }

    private void OnApplicationQuit()
    {
        thread.Abort();
    }

    void GetInfo()
    {
        UnityEngine.Debug.Log("Start");
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = string.Format("E:\\vosk-api-master\\python\\example\\test_microphone.py -d 5"); // 
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        //start.WindowStyle = ProcessWindowStyle.Hidden;
        UnityEngine.Debug.Log("1");
        using (Process process = Process.Start(start))
        {
            UnityEngine.Debug.Log("2");
            using (System.IO.StreamReader reader = process.StandardOutput)
            {
                UnityEngine.Debug.Log("3");
                while (!reader.EndOfStream)
                {
                    UnityEngine.Debug.Log("4");
                    string result = reader.ReadLine();

                    try
                    {
                        if(result.Contains("text"))
                        {
                            UnityEngine.Debug.Log("Result:" + result);
                        } else
                        {
                            continue;
                        }
                    }
                    catch(System.Exception e)
                    {
                        //UnityEngine.Debug.Log(e);
                    }
                }
                UnityEngine.Debug.Log("5");

            }
            process.Kill();
            thread.Abort();
        }
    }
}
