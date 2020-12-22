using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotInfo : MonoBehaviour
{
    public List<UnitSoldier> unitSoldiers;

    public RectTransform t;
    public CanvasGroup cvGroup;

    Vector2 startMouse;

    private void Start()
    {
        t = transform.GetChild(0).GetComponent<RectTransform>();
        cvGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
    }

    public void Down()
    {
        startMouse = Input.mousePosition;
    }

    public void Up()
    {
        t.anchoredPosition3D = Vector3.zero;
        cvGroup.alpha = 1;
    }

    public void Drag()
    {
        SetPos();
    }

    void SetPos()
    {
        float y = Input.mousePosition.y - startMouse.y;

        t.anchoredPosition3D = new Vector3(t.anchoredPosition3D.x, y);

        cvGroup.alpha = (float)(250-y) / (float)250;

        if (t.anchoredPosition3D.y >= 250)
        {
            foreach (var item in unitSoldiers)
            {
                if (Manager.manager.listSelected.Contains(item))
                {
                    item.OnDeSelect();
                    Manager.manager.listSelected.Remove(item);
                }
            }

            Destroy(gameObject);
        }
    }
}
