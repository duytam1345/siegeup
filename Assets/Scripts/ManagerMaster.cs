using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
    public static MasterManager master;

    private void Awake()
    {
        if(!master)
        {
            master = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}
