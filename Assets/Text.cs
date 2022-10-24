using UnityEngine;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.IO;

public class Text : MonoBehaviour
{

    private UnityEngine.UI.Text textContent;
    private float time = 5f;

    private Thread thread;
    private Process process;

    public string text = "";

    public class Json
    {
        [Newtonsoft.Json.JsonProperty("status")]
        public string status { get; set; }
        [Newtonsoft.Json.JsonProperty("translatedText")]
        public string text { get; set; }
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

        time -= Time.deltaTime;

        if (time < 0)
        {
            textContent.text = "";
            text = "";

            time = 5;
        }
    }

    private void OnApplicationQuit()
    {
        process.Kill();
    }

    void GetInfo()
    {
        UnityEngine.Debug.Log("Start");
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = string.Format("E:\\vosk-api-master\\python\\example\\test_microphone.py -d 5"); // 
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.WindowStyle = ProcessWindowStyle.Hidden;
        start.StandardOutputEncoding = System.Text.Encoding.UTF8;
        UnityEngine.Debug.Log("1");

        process = new Process();
        process.StartInfo = start;

        process.OutputDataReceived += new DataReceivedEventHandler((sender, i) =>
        {
            if (!System.String.IsNullOrEmpty(i.Data))
            {
                if(i.Data.Contains("text"))
                {
                    Json content = new Json();

                    try
                    {

                        string data = i.Data.Split('"')[3];
                        //data = data.Substring(1, data.LastIndexOf('"')-1);

                        //int x = 0;
                        //foreach (string str in data)
                        //{
                        //    x++;
                        //    UnityEngine.Debug.Log("nr: " + x + ", " + str);
                        //}

                        UnityEngine.Debug.Log(SwitcherToPolishCharacters("{ " + data + " }"));
                        if (data != "")
                        {
                            data = Get(@"https://script.google.com/macros/s/AKfycbyXf4lSBuiJqXHdiZp6mz3NxEorEFY-EHoyDTY3CEEyuZb4vhVHQcHG9brbgCrrwMhT/exec?text=" + SwitcherToPolishCharacters(data).ToString());
                            Json js = new Json();
                            js = JsonConvert.DeserializeObject<Json>(data);
                            if(js.status == "success")
                            {
                                text = js.text;
                                time = 5f;
                            }
                        }
                    } catch(System.Exception e)
                    {
                        UnityEngine.Debug.Log(e);
                    }
                }
            }
        });

        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();
        process.Close();
    }

    private string SwitcherToPolishCharacters(string text)
    {
        System.Collections.Generic.Dictionary<string, string> letter = new System.Collections.Generic.Dictionary<string, string>()
        {
            {@"\xc4\x85", "ą"},
            {@"\xc4\x87", "c"},
            {@"\xc4\x99", "ę"},
            {@"\xc5\x82", "ł"},
            {@"\xc5\x84", "ń"},
            {@"\xc3\xb3", "ó"},
            {@"\xc5\x9b", "ś"},
            {@"\xc5\xbc", "ż"},
            {@"\xc5\xba", "ź"},
        };

        foreach(string letterKey in letter.Keys)
        {
            text = text.Replace(letterKey, letter[letterKey]);
        }

        return text;

    }

    public string Get(string uri)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}
