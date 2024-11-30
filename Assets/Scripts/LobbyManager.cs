using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour, Manager
{

    public GameObject loadingText;
    public GameObject defaultGroup;
    public NetworkManager networkManager;

    public TMP_InputField inputId;
    public TMP_InputField inputPwd;
    public TMP_InputField inputNickname;
    public TMP_Text txtError;

    private bool occurError = false;
    private string errorMessage = "";
    private bool callNextScene = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (networkManager.isConnected)
        {
            loadingText.GetComponentInChildren<LoadingText>().setConnectStat(true);
            loadingText.SetActive(false);
            defaultGroup.SetActive(true);
        }
        
        if (occurError)
        {   
            txtError.text = errorMessage;
            txtError.gameObject.SetActive(true);
        }

        if (callNextScene)
        {
            SceneManager.LoadScene("Scenes/LobbyScene");
        }

    }

    public void Login() {
        string id = inputId.text;
        string pwd = inputPwd.text;

        networkManager.Send(Type.LOGIN, id + ";" + pwd);
    }

    public void SignUp()
    {
        string id = inputId.text;
        string pwd = inputPwd.text;
        string nickname = inputNickname.text;

        networkManager.Send(Type.SIGNUP, id+";"+ pwd+";"+nickname);
    }

    public void goNextScene()
    {
        callNextScene = true;
    }

    public void OnError(string msg)
    {
        switch (msg)
        {
            case "01":
                errorMessage = "등록되지 않은 사용자입니다.";
                break;
            case "02":
                errorMessage = "비밀번호가 잘못되었습니다.";
                break;
        }
        occurError = true;
    }

    public void OnMessage(Type type, string msg) { }

}

