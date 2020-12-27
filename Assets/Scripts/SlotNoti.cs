using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotNoti : MonoBehaviour
{
    public RectTransform rect;
    public Text text;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<Text>();
    }

    void Update()
    {
        //Vector3 v = rect.position;
        //v.y += 2;
        //rect.position = v;

        //Color c = text.color;
        //c.a -= .5f * Time.deltaTime;
        //text.color = c;
    }
}
