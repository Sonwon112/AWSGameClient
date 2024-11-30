using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour, Manager
{
    public TMP_Text txtNickname;
    private NetworkManager networkManager;
    private bool callNextScene = false;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkManager.instance;
        txtNickname.text = networkManager.clientNickname;
        networkManager.setManager(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (callNextScene)
        {
            SceneManager.LoadScene(2);
        }
    }

    public void goNextScene()
    {
        callNextScene = true;
    }

    public void CallMatch()
    {
        // 매치 서버에게 플레이서버 IP 요청
        if (networkManager.isConnected)
        {
            networkManager.ConnectPlayerServer();// 테스트용
        }
        else
        {
            networkManager.ConnectPlayerServer();
        }
    }
    public void OnError(string msg)
    {

    }
    public void OnMessage(Type type, string msg)
    {
        
    }
}
