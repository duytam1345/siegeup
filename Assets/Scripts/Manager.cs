using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public GameObject currentToBuild;
    public bool firstMouseDown;

    public GameObject btnCancelBuild;
    public GameObject infoPanel;
    public GameObject buildPanel;
    public GameObject buildBtn;

    public GameObject cam;

    private void Awake()
    {
        if (manager == null)
        {
            manager = this;
        }

        cam = Camera.main.transform.parent.gameObject;
    }

    private void Update()
    {
        MoveCamera();

        if (currentToBuild)
        {
            if (!MouseOnUI())
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask.GetMask("Nav")))
                {
                    currentToBuild.transform.position = hit.point;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (MouseOnUI())
            {
                return;
            }
            startPosMouseDown = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (startPosMouseDown != Vector2.zero && !currentToBuild)
            {
                SetSelectionBox(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentToBuild && !MouseOnUI())
            {
                if (firstMouseDown)
                {
                    firstMouseDown = false;
                }
                else
                {
                    if (currentToBuild.GetComponent<Building>().colliders.Count <= 0)
                    {
                        startPosMouseDown = Vector2.zero;

                        currentToBuild.GetComponent<Building>().SetConstruct();

                        GameObject g = Instantiate(Resources.Load("Construct/" + currentToBuild.GetComponent<Building>().n + "_2") as GameObject, currentToBuild.transform.position, Quaternion.identity);

                        Destroy(currentToBuild);
                        btnCancelBuild.SetActive(false);
                        buildPanel.SetActive(true);
                        return;
                    }
                }
            }

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


                UnitSoldier[] unitRts = FindObjectsOfType<UnitSoldier>();

                List<UnitSoldier> units = new List<UnitSoldier>();


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
            startPosMouseDown = Vector2.zero;
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

    void UnitSelect(List<UnitSoldier> units)
    {
        if(units.Count>0)
        {
            infoPanel.SetActive(true);
        }

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

    /// <summary>
    /// Nhập tên, tạo.
    /// </summary>
    public void BuildConstruct(string n)
    {
        if (!currentToBuild)
        {
            currentToBuild = Instantiate(Resources.Load("Construct/" + n + "_1") as GameObject, Vector2.zero, Quaternion.identity);
            currentToBuild.GetComponent<Building>().n = n;

            btnCancelBuild.SetActive(true);
            buildPanel.SetActive(false);
            buildBtn.SetActive(false);

            firstMouseDown = true;
        }
    }

    public void BuildBtnOnClick()
    {
        buildBtn.SetActive(false);
        buildPanel.SetActive(true);
    }

    public bool MouseOnUI()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, results);

        if (results.Count > 0)
        {
            return true;
        }
        return false;
    }

    void MoveCamera()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 euler = cam.transform.eulerAngles;
            euler.y -=1;

            cam.transform.eulerAngles = euler;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Vector3 euler = cam.transform.eulerAngles;
            euler.y += 1;

            cam.transform.eulerAngles = euler;
        }

        if(Input.GetKey(KeyCode.W))
        {
            Vector3 pos = cam.transform.position;
            pos += cam.transform.forward * .5f;

            cam.transform.position = pos;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 pos = cam.transform.position;
            pos -= cam.transform.forward * .5f;

            cam.transform.position = pos;
        }

        if (Input.GetKey(KeyCode.A))
        {
            Vector3 pos = cam.transform.position;
            pos -= cam.transform.right*.5f;

            cam.transform.position = pos;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 pos = cam.transform.position;
            pos += cam.transform.right * .5f;

            cam.transform.position = pos;
        }
    }

    public void CancelBuild()
    {
        DestroyImmediate(currentToBuild);

        btnCancelBuild.SetActive(false);
        buildPanel.SetActive(true);
    }

    public void ClosePanelBtn(GameObject g)
    {
        g.SetActive(false);
        buildBtn.SetActive(true);
    }
}
