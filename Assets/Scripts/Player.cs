using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isMine = false;
    public float movingSpeed = 2f;
    public TMP_Text nicknameText;

    private PlayManager playManager;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("PlayManager") != null)
        {
            playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMine)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            transform.Translate(Vector3.forward * vertical * movingSpeed * Time.deltaTime);
            transform.Translate(Vector3.right*horizontal*movingSpeed* Time.deltaTime);
            playManager.SendOwnPlayerTransform(transform.position);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //죽는 기능 구현
                playManager.OwnPlayerDead();
                Destroy(gameObject);
            }
        }
        else
        {
            GetComponentInChildren<CinemachineVirtualCamera>().Priority = 10;
        }

    }

    public void setPlayerPosition(Vector3 pos)
    {
        if (!isMine)
        {
            transform.position = pos;
        }
    }

    public void setNickname(string nickname)
    {
        nicknameText.text = nickname;
    }
}
