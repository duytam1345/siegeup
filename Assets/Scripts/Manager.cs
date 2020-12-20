using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ResourcesGame
{
    public int _gold;
    public int _metal;
    public int _wood;
    public int _food;
    public int _soldier;
    public int _maxSoldier;
}

public class Manager : MonoBehaviour
{
    public static Manager manager;

    public RectTransform boxSelection;

    public List<Unit> listSelected;

    public Vector2 startPosMouseDown;
    public Vector2 endPosMouseUp;

    public ResourcesGame resourcesGame;

    public Text textGold;
    public Text textWood;
    public Text textFood;
    public Text textMetal;
    public Text textSoldier;

    private void Awake()
    {

        if (manager == null)
        {
            manager = this;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosMouseDown = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            SetSelectionBox(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 endPosMouseDown = Input.mousePosition;

            //click
            if (Vector2.Distance(endPosMouseDown, startPosMouseDown) <= 1)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider.gameObject.layer == 9)
                    {
                        Unit unit = hit.collider.gameObject.GetComponent<Unit>();

                        foreach (var item in listSelected)
                        {
                            item.ActTo(unit);
                        }
                        DeSelectAll();
                    }
                    else
                    {
                        foreach (var item in listSelected)
                        {
                            //item.MoveTo(hit.point);
                            item.SetMove(hit.point);
                        }
                    }
                }
            }
            //drag
            else
            {
                foreach (var item in listSelected)
                {
                    item.OnDeSelect();
                }
                listSelected.Clear();


                Unit[] unitRts = FindObjectsOfType<Unit>();

                List<Unit> units = new List<Unit>();


                Vector2 min = new Vector2(
                        (startPosMouseDown.x < endPosMouseDown.x ? startPosMouseDown.x : endPosMouseDown.x),
                        (startPosMouseDown.y < endPosMouseDown.y ? startPosMouseDown.y : endPosMouseDown.y));

                Vector2 max = new Vector2(
                    (startPosMouseDown.x > endPosMouseDown.x ? startPosMouseDown.x : endPosMouseDown.x),
                    (startPosMouseDown.y > endPosMouseDown.y ? startPosMouseDown.y : endPosMouseDown.y));

                foreach (var item in unitRts)
                {
                    if (item._property.colorTeam == Team.Red)
                    {
                        Vector2 v = Camera.main.WorldToScreenPoint(item.transform.position);

                        if (v.x > min.x && v.x < max.x && v.y > min.y && v.y < max.y)
                        {
                            units.Add(item);
                        }
                    }
                }

                if (units.Count > 0)
                {
                    UnitSelect(units);
                }

                boxSelection.gameObject.SetActive(false);
            }
        }
    }

    void SetSelectionBox(Vector3 vector)
    {
        if (!boxSelection.gameObject.activeInHierarchy)
        {
            boxSelection.gameObject.SetActive(true);
        }

        float width = vector.x - startPosMouseDown.x;
        float height = vector.y - startPosMouseDown.y;

        boxSelection.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        boxSelection.position = startPosMouseDown + new Vector2(width / 2, height / 2);
    }

    void UnitSelect(List<Unit> units)
    {
        foreach (var item in units)
        {
            item.OnSelect();
            listSelected.Add(item);
        }
    }

    void DeSelectAll()
    {
        foreach (var item in listSelected)
        {
            item.OnDeSelect();
        }

        listSelected.Clear();
    }

    /// <summary>
    /// Cập nhật lại tài nguyên hiển thị trên UI.
    /// </summary>
    public void UpdateresourcesGame()
    {
        textGold.text = resourcesGame._gold + "";
        textFood.text = resourcesGame._food + "";
        textWood.text = resourcesGame._wood + "";
        textMetal.text = resourcesGame._metal + "";
        textSoldier.text = resourcesGame._soldier + "/" + resourcesGame._maxSoldier;
    }

    public void CreatePeasant()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Peasant") as GameObject, new Vector3(-7, 0, 0), Quaternion.identity);
        g.GetComponent<Unit>().SetMove(new Vector3(-7, 0, -2));
    }
}
