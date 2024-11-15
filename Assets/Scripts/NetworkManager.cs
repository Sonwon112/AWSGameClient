using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private TcpClient tcpClient;
    private string matchServerIP = "127.0.0.1";
    private int port = 7777;
    private NetworkStream stream;
    private bool tryConnect = false;

    public bool isConnected = false;
    

    private int clientId = -1;
    public string clientNickname { get; set; }
    private Thread listenThread;
    private Manager manager;

    // Start is called before the first frame update
    void Start()
    {
        tcpClient = new TcpClient(matchServerIP, port);
        stream = tcpClient.GetStream();
        tryConnect = true;
        listenThread = new Thread(() => Listen());
        listenThread.Start();
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (listenThread !=null)
        {
            if (listenThread.IsAlive)
            {
                listenThread.Abort();
            }
        }
        
    }

    public void Listen()
    {
        while (tryConnect)
        {
            byte[] buffer = new byte[1024];
            try
            {
                int len = stream.Read(buffer, 0, buffer.Length);
                if (len <= 0) continue;
                string dataStr = Encoding.UTF8.GetString(buffer);
                Debug.Log(dataStr);
                DTO dto = JsonUtility.FromJson<DTO>(dataStr);
                Type dtoType = (Type)Enum.Parse(typeof(Type), dto.type);
                switch (dtoType)
                {
                    case Type.CONNECT:
                        {
                            string[] tmp = dto.msg.Split(';');
                            if (tmp[0] == "success")
                            {
                                isConnected = true;
                                clientId = int.Parse(tmp[1]);
                            }
                            break;
                        }
                    case Type.LOGIN:
                        {
                            string[] tmp = dto.msg.Split(';');
                            if (tmp[0] == "success")
                            {
                                clientNickname = tmp[1];
                                
                                if(manager == null)
                                {
                                    Debug.LogError("월드에서 Manager를 찾을 수 없습니다");
                                    return;
                                }
                                manager.goNextScene();
                            }else if(tmp[0] == "fail")
                            {
                                manager.OnError(tmp[1]);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public void Send(Type type, string msg)
    {
        Thread thread = new Thread(() => {
            DTO dto = new DTO(clientId, type.ToString(), msg);
            string jsonFromDto = JsonUtility.ToJson(dto,true);
            byte[] buffer = Encoding.UTF8.GetBytes(jsonFromDto);
            stream.Write(buffer, 0, buffer.Length);
            Debug.Log("send" + msg);
        });
        thread.Start();
    }
}

public enum Type
{
    CONNECT,
    LOGIN,
    SIGNUP,
    START,
    END
}