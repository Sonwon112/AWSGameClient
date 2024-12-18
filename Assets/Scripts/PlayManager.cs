using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class PlayManager : MonoBehaviour, Manager
{
    [Header("UI")]
    public GameObject LoadUI;
    public GameObject PlayUI;
    public GameObject DeadUI;
    public TMP_Text cntText;
    [Header("Player")]
    public GameObject Player;

    private NetworkManager networkManager;
    private bool isPlaying = false;
    private int totalCount = 1;
    private int currCount = 0;

    private bool isTest = false;
    private bool isSpawnedOwn = false;
    private Vector2 BOUNDARY_MIN = new Vector2(-49, -49);
    private Vector2 BOUNDARY_MAX = new Vector2(49, 49);

    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkManager.instance;
        cntText.text = currCount+"/"+totalCount;
        if (networkManager == null || !networkManager.isConnected) // 테스트 환경
        {   
            isTest = true;
            StartGame();
            //instantiateOwnPlayer();
        }
        else
        {
            networkManager.setManager(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying && !isTest)
        {
            setPlayerCnt(networkManager.getCurrParticipant());
            if (networkManager.isStartGame && !isSpawnedOwn) { 
                StartGame();
                networkManager.setManager(this);
                isSpawnedOwn=true;
            }
        }
    }

    public void setPlayerCnt(int currCount)
    {
        this.currCount = currCount;
        cntText.text = currCount + "/" + totalCount;
    }

    public void goNextScene()
    {

    }

    public void OnError(string msg)
    {

    }

    public void OnMessage(Type type, string msg)
    {
        switch (type)
        {
            case Type.CONNECT:
                StartGame();
                break;
            case Type.INSTANTIATE:
                string[] tmp = msg.Split(';');
                instantiateOtherPlayer(int.Parse(tmp[0]), tmp[1]);
                break;
            case Type.SEND_TRANSFORM:
                setOthrePlayTransform(msg);
                break;
            case Type.SEND_PARTICIPANT:
                int id = int.Parse(msg);
                Destroy(players[id].gameObject);
                players.Remove(id);
                break;
        }
    }

    public void StartGame()
    {
        LoadUI.SetActive(false);
        PlayUI.SetActive(true);
        instantiateOwnPlayer();
    }
    public void instantiateOwnPlayer()
    {
        float x = Random.Range(BOUNDARY_MIN.x, BOUNDARY_MAX.x);
        float z = Random.Range(BOUNDARY_MIN.y, BOUNDARY_MAX.y);
        Vector3 position = new Vector3(x,1,z);
        GameObject tmp = Instantiate(Player, position, Quaternion.identity);
        tmp.GetComponent<Player>().setNickname(networkManager.clientNickname);

        tmp.GetComponent<Player>().isMine = true;
        if (isPlaying)
        {
            networkManager.Send(Type.INSTANTIATE);
            SendOwnPlayerTransform(tmp.transform.position);
        }
    }

    public void SendOwnPlayerTransform(Vector3 position)
    {
        string msg = position.x + ";" + position.y + ";" + position.z;
        networkManager.SendPlayServer(Type.SEND_TRANSFORM, msg);
    }

    public void instantiateOtherPlayer(int id, string nickname)
    {
        GameObject tmp = Instantiate(Player);
        tmp.GetComponent<Player>().setNickname(nickname);
        players.Add(id, tmp);
    }

    public void setOthrePlayTransform(string data)
    {
        string[] tmp = data.Split(";");
        players[int.Parse(tmp[0])].transform.position = new Vector3(float.Parse(tmp[1]), float.Parse(tmp[2]), float.Parse(tmp[3]));
        players[int.Parse(tmp[0])].GetComponent<Player>().setPlayerPosition(players[int.Parse(tmp[0])].transform.position);
    }

    public void OwnPlayerDead()
    {
        PlayUI.SetActive(false);
        DeadUI.SetActive(true);
        if (isPlaying)
        {
            networkManager.SendPlayServer(Type.SEND_PARTICIPANT, "die");
        }
    }

    public int getIndex() { return 2; }
}
