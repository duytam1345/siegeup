using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
    public static MasterManager master;

    public int soundPercent;
    public int musicPercent;
    public int cameraSpeed;

    private void Awake()
    {
        if(!master)
        {
            master = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SetSound(int i)
    {
        soundPercent = i;
    }

    public void SetMusic(int i)
    {
        soundPercent = i;
    }

    public void SetCameraSpeed(int i)
    {
        soundPercent = i;
    }
}
