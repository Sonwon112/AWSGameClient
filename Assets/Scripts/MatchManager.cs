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
        // ��ġ �������� �÷��̼��� IP ��û
        if (networkManager.isConnected)
        {
            networkManager.ConnectPlayerServer();// �׽�Ʈ��
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
