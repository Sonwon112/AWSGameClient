using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTO
{
    public int id;
    public string type;
    public string msg;

    public DTO(int id, string type, string msg)
    {
        this.id = id;
        this.type = type;
        this.msg = msg;
    }
}
