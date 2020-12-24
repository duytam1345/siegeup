using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    //Làm thêm kẻ địch và hiệu ứng
    //Các button, hình ảnh ui

    //Làm thông báo
    //

    public static Manager manager;

    public bool isPause;

    public bool testMode;

    public bool onMenu;

    public RectTransform boxSelection;

    public List<Unit> listSelected;

    public List<UnitSoldier> currentSoldiers;

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
    public Text textInfoPanel;
    public Transform contentInfoPanel;

    public GameObject buildPanel;
    public GameObject buildBtn;

    public GameObject descriptionBtn;
    public GameObject descriptionPanel;
    public Text descriptionText;

    public GameObject winLosePanel;
    public Text textWinLose;

    public GameObject cam;

    private void Awake()
    {
        if (manager == null)
        {
            manager = this;
        }

        cam = Camera.main.transform.parent.gameObject;
    }

    private void Start()
    {
        UnitSoldier[] unitSoldiers = FindObjectsOfType<UnitSoldier>();

        foreach (var item in unitSoldiers)
        {
            if (item._property.colorTeam == Team.Red)
            {
                AddToCurrentSoldier(item);
            }
        }

        UpdateresourcesGame();
    }

    private void Update()
    {
        if (isPause)
        {
            return;
        }

        if (onMenu)
        {
            return;
        }

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

        if (isPause)
        {
            return;
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
                        switch (currentToBuild.GetComponent<Building>().n)
                        {
                            case "Archer Range":
                                resourcesGame._gold -= 10;
                                resourcesGame._wood -= 20;
                                break;
                            case "Farm":
                                resourcesGame._wood -= 40;
                                break;
                            case "House":
                                resourcesGame._wood -= 30;
                                break;
                            case "Barracks":
                                resourcesGame._gold -= 20;
                                resourcesGame._metal -= 20;
                                break;
                        }

                        UpdateresourcesGame();

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

            if (startPosMouseDown == Vector2.zero)
            {
                return;
            }

            //click
            if (Vector2.Distance(endPosMouseDown, startPosMouseDown) <= 10)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider.gameObject.layer == 9)
                    {
                        Unit unit = hit.collider.gameObject.GetComponent<Unit>();

                        if (listSelected.Count > 0)
                        {
                            foreach (var item in listSelected)
                            {
                                item.ActTo(unit);

                                descriptionBtn.SetActive(false);
                            }
                        }
                        else
                        {
                            if (unit.GetComponent<UnitConstruct>())
                            {
                                listSelected.Clear();
                                listSelected.Add(unit);

                                infoPanel.SetActive(true);
                                unit.OnSelect();

                                foreach (Transform item in contentInfoPanel)
                                {
                                    Destroy(item.gameObject);
                                }

                                unit.GetComponent<UnitConstruct>().ShowPanel();
                                descriptionBtn.SetActive(true);

                                buildPanel.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        int c = listSelected.Count;
                        float sqrt = Mathf.Sqrt(c);
                        while ((int)sqrt != sqrt)
                        {
                            c++;
                            sqrt = Mathf.Sqrt(c);
                        }

                        List<Vector3> v = new List<Vector3>();
                        for (int i = 0; i < sqrt; i++)
                        {
                            for (int j = 0; j < sqrt; j++)
                            {
                                Vector3 newV = hit.point - listSelected[0].transform.position + new Vector3(i, 0, j);
                                v.Add(newV);
                            }
                        }

                        for (int i = 0; i < listSelected.Count; i++)
                        {
                            listSelected[i].SetMove(listSelected[0].transform.position + v[i]);
                        }

                        descriptionBtn.SetActive(false);
                    }
                }
            }
            //drag
            else
            {
                descriptionBtn.SetActive(false);


                if (startPosMouseDown != Vector2.zero)
                {
                    infoPanel.SetActive(false);
                    buildBtn.SetActive(true);

                    DeSelectAll();
                }

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
        if (boxSelection)
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
    }

    void UnitSelect(List<UnitSoldier> units)
    {
        if (units.Count > 0)
        {
            infoPanel.SetActive(true);
            buildBtn.SetActive(false);
            buildPanel.SetActive(false);

            Dictionary<string, int> keyValues = new Dictionary<string, int>();

            foreach (var item in units)
            {
                if (keyValues.ContainsKey(item._property._name))
                {
                    keyValues[item._property._name]++;
                }
                else
                {
                    keyValues.Add(item._property._name, 1);
                }
            }


            foreach (Transform item in contentInfoPanel)
            {
                Destroy(item.gameObject);
            }

            foreach (var item in keyValues)
            {
                GameObject g = Instantiate(Resources.Load("UI/Slot Info") as GameObject, contentInfoPanel);
                g.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = item.Key;
                g.GetComponent<SlotInfo>().unitSoldiers = new List<UnitSoldier>();
                g.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = item.Value.ToString();
                foreach (var jtem in units)
                {
                    if (jtem._property._name == item.Key)
                    {
                        g.GetComponent<SlotInfo>().unitSoldiers.Add(jtem.GetComponent<UnitSoldier>());
                    }
                }
            }
        }
        else
        {
            infoPanel.SetActive(false);
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
        if (textGold)
        {
            textGold.text = resourcesGame._gold + "";
        }
        if (textFood)
        {
            textFood.text = resourcesGame._food + "";
        }
        if (textWood)
        {
            textWood.text = resourcesGame._wood + "";
        }
        if (textMetal)
        {
            textMetal.text = resourcesGame._metal + "";
        }
        if (textSoldier)
        {
            textSoldier.text = currentSoldiers.Count + "/" + resourcesGame._maxSoldier;
        }
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
            if (!EnoughResource(n))
            {
                return;
            }

            currentToBuild = Instantiate(Resources.Load("Construct/" + n + "_1") as GameObject, Vector2.zero, Quaternion.identity);
            currentToBuild.GetComponent<Building>().n = n;

            btnCancelBuild.SetActive(true);
            buildPanel.SetActive(false);
            buildBtn.SetActive(false);

            firstMouseDown = true;
        }
    }

    bool EnoughResource(string s)
    {
        int needWood = 0;
        int needMetal = 0;

        switch (s)
        {
            case "Farm":
                needWood = 40;
                break;
            case "House":
                needWood = 30;
                break;
            case "Barracks":
                needMetal = 25;
                break;
            case "Archer Range":
                needWood = 20;
                break;
        }

        if (resourcesGame._wood >= needWood && resourcesGame._metal >= needMetal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void BuildBtnOnClick()
    {
        buildBtn.SetActive(false);
        buildPanel.SetActive(true);
        infoPanel.SetActive(false);
    }

    public void DescriptionBtnOnClick()
    {
        if (listSelected.Count > 0 && listSelected.Count < 2)
        {
            descriptionText.text = listSelected[0]._property.description;
            descriptionPanel.SetActive(true);
        }
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
            euler.y -= 1;

            cam.transform.eulerAngles = euler;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Vector3 euler = cam.transform.eulerAngles;
            euler.y += 1;

            cam.transform.eulerAngles = euler;
        }

        if (Input.GetKey(KeyCode.W))
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
            pos -= cam.transform.right * .5f;

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
        DeSelectAll();

        g.SetActive(false);
        buildBtn.SetActive(true);
    }

    public void SetPanelWinLose(string s)
    {
        isPause = true;

        winLosePanel.SetActive(true);
        textWinLose.text = "You " + s + " !";
    }

    public void RestartBtn()
    {
        SceneManager.LoadScene("Level1");
    }

    public void BackMenuBtn()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void AddToCurrentSoldier(UnitSoldier soldier)
    {
        if (!currentSoldiers.Contains(soldier))
        {
            currentSoldiers.Add(soldier);
        }

        UpdateresourcesGame();
    }

    public void RemoveToCurrentSoldier(UnitSoldier soldier)
    {
        if (currentSoldiers.Contains(soldier))
        {
            currentSoldiers.Remove(soldier);
        }

        UpdateresourcesGame();
    }

    public void CreateSlotNoti(string s)
    {
        GameObject g = Instantiate(Resources.Load("UI/SlotNoti") as GameObject, GameObject.Find("Noti").transform);
        g.GetComponent<Text>().text = s;
    }
}
