using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingText : MonoBehaviour
{
    private TMP_Text LoadText;
    private string[] loadText = {"연결 중", "연결 중." , "연결 중.." , "연결 중..." };
    private bool isConnect = false;

    private int index = 0;
    private float prevTime = 0f;
    private float currTime = 0f;
    public float delay = 3f;


    // Start is called before the first frame update
    void Start()
    {
        LoadText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isConnect)
        {
            currTime += Time.deltaTime;

            if (currTime > prevTime + delay)
            {
                if (index + 1 > loadText.Length - 1) index = -1;
                LoadText.text = loadText[++index];

                prevTime = currTime;
            }
        }
    }

    public void setConnectStat(bool isConnect)
    {
        this.isConnect = isConnect;
    }
}
