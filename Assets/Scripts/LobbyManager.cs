using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    private TcpClient tcpClient;
    private string matchServerIP = "127.0.0.1";
    private int port = 7777;
    private NetworkStream stream;
    private bool tryConnect = false;
    private static bool isConnected = false;

    public GameObject loadingText;
    public GameObject defaultGroup;


    // Start is called before the first frame update
    void Start()
    {
        tcpClient = new TcpClient(matchServerIP, port);
        stream = tcpClient.GetStream();
        tryConnect = true;
        Thread thread = new Thread(() => Listen());
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected)
        {
            loadingText.GetComponentInChildren<LoadingText>().setConnectStat(true);
            loadingText.SetActive(false);
            defaultGroup.SetActive(true);
        }
    }

    public void Listen()
    {
        while (tryConnect) { 
            byte[] buffer = new byte[1024];
            try
            {
                int len = stream.Read(buffer, 0, buffer.Length);
                if (len <= 0) continue;
                string dataStr = Encoding.UTF8.GetString(buffer);
                Debug.Log(dataStr);
                DTO dto = JsonUtility.FromJson<DTO>(dataStr);
                switch (dto.msg)
                {
                    case "success":
                        isConnected = true;
                        break;
                    default:
                        break;
                }
            }catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

}
