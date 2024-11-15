using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchManager : MonoBehaviour, Manager
{
    public TMP_Text txtNickname;
    private NetworkManager networkManager;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkManager.instance;
        txtNickname.text = networkManager.clientNickname;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goNextScene()
    {

    }

    public void OnError(string msg)
    {

    }
}
