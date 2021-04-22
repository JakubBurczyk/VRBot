using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;
using System.IO;

public class IpCamScript : MonoBehaviour
{
    public MeshRenderer frame;
    public Text CameraURL;
    [HideInInspector]
    public Byte[] JpegData;
    [HideInInspector]
    public string resolution = "640x480";

    private Texture2D texture;
    private Stream stream;
    private WebResponse resp;
    

    public void StartStream()
    {
        //Debug.Log("Start IP cam");
        GetVideo();
    }

    public void StopStream()
    {
        stream.Close();
        resp.Close();
    }

    public void GetVideo()
    {
        //Debug.Log("GetVideo");
        texture = new Texture2D(2, 2);

        string url = "";
        if(CameraURL.text != "")
        {
            url = CameraURL.text;   
        }
        //string url = "http://24.172.4.142/mjpg/video.mjpg?COUNTER";
        //Debug.Log("creating webrequest");
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //Debug.Log("created webrequest");
        //For testing
        // req.ProtocolVersion = HttpVersion.Version10;

        // get response
        //Debug.Log("Getting response");
        resp = req.GetResponse();
        //Debug.Log("Got response");
        // get response stream
        //Debug.Log("Getting response stream");
        stream = resp.GetResponseStream();
        //Debug.Log("Got response stream");
        frame.material.color = Color.white;
        StartCoroutine(GetFrame());
    }

    public IEnumerator GetFrame()
    {
        // Byte [] JpegData = new Byte[105536];
        //   Byte [] JpegData = new Byte[205536];
        
        Byte[] JpegData = new Byte[505536];
        while (true)
        {
            //Debug.Log("GetFrame Main Loop");
            int bytesToRead = FindLength(stream);
            if (bytesToRead == -1)
            {
                yield break;
            }

            int leftToRead = bytesToRead;
            //Debug.Log(bytesToRead);
            while (leftToRead > 0)
            {
                //Debug.Log("Left To Read" + leftToRead);
                leftToRead -= stream.Read(JpegData, bytesToRead - leftToRead, leftToRead);
                yield return null;
            }

            MemoryStream ms = new MemoryStream(JpegData, 0, bytesToRead, false, true);

            texture.LoadImage(ms.GetBuffer());
            frame.material.mainTexture = texture;
            frame.material.color = Color.white;
            stream.ReadByte(); // CR after bytes
            stream.ReadByte(); // LF after bytes

            //Debug.Log("Frame done");
        }
    }

    int FindLength(Stream stream)
    {
        //Debug.Log("Find Lenght");
        int b;
        string line = "";
        int result = -1;
        bool atEOL = false;
        string content = "";
        while ((b = stream.ReadByte()) != -1)
        {


            if (b == 10) continue; // ignore LF char

            if (b == 13)
            { // CR
                if (atEOL)
                {  // two blank lines means end of header
                    stream.ReadByte(); // eat last LF
                    //Debug.Log("Find Lenghth result = " + result);
                    //Debug.Log("CONTENT: " + content);
                    return result;
                }
                if (line.StartsWith("Content-Length:"))
                {
                    result = Convert.ToInt32(line.Substring("Content-Length:".Length).Trim());
                }
                else
                {
                    line = "";
                }
                atEOL = true;
            }
            else
            {
                atEOL = false;
                line += (char)b;
            }
        }

        //Debug.Log("CONTENT: " + content);
        return -1;
    }
}
