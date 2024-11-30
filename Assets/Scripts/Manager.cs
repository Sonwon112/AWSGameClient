using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Manager
{
    public void goNextScene();
    public void OnError(string msg);

    public void OnMessage(Type type,string msg);
}
