using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

    // 매치 서버
    private TcpClient tcpClient;
    private string matchServerIP = "127.0.0.1";
    private int port = 7777;
    private NetworkStream stream;
    private bool tryConnect = false;
    public bool isConnected = false;


    private int clientId = -2;
    public string clientNickname = "test";
    private Thread listenThread;
    private Manager manager;

    // 플레이 서버
    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;
    private string playServerIP = "127.0.0.1";
    private int serverPort = 9100;
    private int playPort = 9000;
    private bool isPlayMode = false;

    private int currParticipant = 0;


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

    public void setManager(Manager manager)
    {
        this.manager = manager;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (listenThread != null)
        {
            if (listenThread.IsAlive)
            {
                listenThread.Abort();
            }
        }

    }

    /// <summary>
    /// TCP Listen
    /// </summary>
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

                                if (manager == null)
                                {
                                    Debug.LogError("월드에서 Manager를 찾을 수 없습니다");
                                    return;
                                }
                                manager.goNextScene();
                            } else if (tmp[0] == "fail")
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
            string jsonFromDto = JsonUtility.ToJson(dto, true);
            byte[] buffer = Encoding.UTF8.GetBytes(jsonFromDto);
            stream.Write(buffer, 0, buffer.Length);
            Debug.Log("send" + msg);
        });
        thread.Start();
    }


    public void ConnectPlayerServer()
    {
        try
        {
            udpClient = new UdpClient(playPort);
            udpClient.Client.Blocking = false;
            serverEndPoint = new IPEndPoint(IPAddress.Parse(playServerIP), serverPort);

            DTO dto = new DTO(clientId, "CONNECT", clientNickname);
            string dtoToJson = JsonUtility.ToJson(dto);
            byte[] buffer = Encoding.UTF8.GetBytes(dtoToJson);
            udpClient.Send(buffer, buffer.Length,playServerIP,serverPort);

            if(listenThread != null)
                listenThread.Abort();
            isPlayMode = true;

            listenThread = new Thread(() => {
                ListenPlayServer();
            });
            listenThread.Start();
            Debug.Log("UDP Listen Start");

        }
        catch (SocketException e)
        {
            Debug.Log(e.ToString());
            udpClient.Close();
        }
    }

    /// <summary>
    /// UDP Listen
    /// </summary>
    public async void ListenPlayServer()
    {
        while (isPlayMode) {
            try
            {   
                UdpReceiveResult readBuffer = await udpClient.ReceiveAsync();
                string tmp = Encoding.UTF8.GetString(readBuffer.Buffer);
                DTO dto = JsonUtility.FromJson<DTO>(tmp);
                Type type = (Type)Enum.Parse(typeof(Type), dto.type);

                switch (type)
                {
                    case Type.CONNECT:
                        if (dto.msg.Equals("SUCCESS"))
                        {
                            manager.goNextScene();
                        }else if (dto.msg.Equals("COMPLETE"))
                        {

                        }
                        break;
                    case Type.INSTANTIATE:
                        manager.OnMessage(Type.INSTANTIATE,dto.msg);
                        break;
                    case Type.SEND_TRANSFORM:
                        break;
                    case Type.SEND_PARTICIPANT:
                        //Debug.Log(manager);
                        currParticipant = int.Parse(dto.msg);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                udpClient.Close();
            }
        }
    }

    public int getCurrParticipant()
    {
        return currParticipant;
    }


    public void Send(Type type, Vector3 position)
    {
        Thread thread = new Thread(() => {
            byte[] buffer = BinaryPack(position);
            stream.Write(buffer, 0, buffer.Length);
        });
        thread.Start();
    }

    public byte[] BinaryPack(Vector3 position)
    {
        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(0x01);
            writer.Write(clientId);
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(position.z);

            return stream.ToArray();
        }
    }

    public void Send(Type type)
    {
        Send(type, clientNickname);
    }
}

public enum Type
{
    CONNECT,
    LOGIN,
    SIGNUP,
    START,
    END,
    INSTANTIATE,
    SEND_TRANSFORM,
    SEND_PARTICIPANT
}