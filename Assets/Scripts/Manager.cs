﻿using System;
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
    //Button quay về menu, chọn màn chơi
    //Button sữa chữa

    public static Manager manager;

    public string nameLevel;

    public bool isPause;

    public bool testMode;

    public bool onMenu;

    public RectTransform boxSelection;

    public List<Unit> listSelected;

    public bool isMultiSelectMode;
    public Button multiSelectBtn;

    public List<UnitSoldier> currentSoldiers;

    public Vector2 startPosMouseDown;
    public Vector2 endPosMouseUp;

    public ResourcesGame resourcesGame;

    public Sprite spriteGold;
    public Sprite spriteMetal;
    public Sprite spriteWood;
    public Sprite spriteFood;

    public Text textGold;
    public Text textWood;
    public Text textFood;
    public Text textMetal;
    public Text textSoldier;

    public GameObject currentToBuild;
    public bool firstMouseDown;
    public GameObject btnCancelBuild;
    public GameObject btnApplyBuild;

    public bool setCurrentBuid;

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

    public RectTransform ScrollViewNoti;

    public GameObject fullMapPanel;


    public float originDistanceTwoTouch;

    public InputManager.MouseState mouseState;

    public GameObject cam;
    public Vector3 mousePosOrigin;

    public Text textTimeGame;

    public float minute;
    public float second;

    public CityHallConstruct cityHallConstruct;

    [Header("Menu")]
    public GameObject menuPanel;

    public GameObject optionPanel;

    public Text textMusic;

    public Text textSound;

    public Text textCameraSpeed;

    public GameObject confirmExitPanel;

    public Material[] materialsTeam;

    public TouchMovement touchMovement;

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
        mouseState = InputManager.GetMouseState();

        if (isPause)
        {
            return;
        }

        SetTimeInGame();

        if (onMenu)
        {
            return;
        }

        MoveCamera();

        //MoveCameraAndroid();

        if (mouseState == InputManager.MouseState.MouseDown)
        {
            if (MouseOnUI())
            {
                startPosMouseDown = Vector2.zero;
                return;
            }

            if (currentToBuild && !setCurrentBuid)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider.gameObject.layer == 10)
                    {
                        setCurrentBuid = true;
                        return;
                    }
                }
            }

            startPosMouseDown = Input.mousePosition;
            mousePosOrigin = cam.transform.position;
        }

        if (mouseState == InputManager.MouseState.LeftDragging)
        {
            if (setCurrentBuid)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask.GetMask("Nav")))
                {
                    currentToBuild.transform.position = hit.point;
                    return;
                }
            }

            if (isMultiSelectMode)
            {
                if (startPosMouseDown != Vector2.zero && !currentToBuild)
                {
                    SetSelectionBox(Input.mousePosition);
                }
            }
            else
            {
                if (startPosMouseDown != Vector2.zero)
                {
                    Vector3 dir = (Vector3)startPosMouseDown - Input.mousePosition;

                    Vector3 pos = cam.transform.position;

                    float camSpeed = (float)MasterManager.master.cameraSpeed / (float)100;
                    dir *= (camSpeed);

                    cam.transform.position = (new Vector3(dir.x, 0, dir.y) / 50) + mousePosOrigin;
                }
            }
        }

        if (mouseState == InputManager.MouseState.LeftUp)
        {
            if (!MouseOnUI())
            {
                if (setCurrentBuid)
                {
                    setCurrentBuid = false;
                }


                endPosMouseUp = Input.mousePosition;

                //Kéo chọn
                if (isMultiSelectMode)
                {
                    if (startPosMouseDown == Vector2.zero)
                    {
                        return;
                    }

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
                            (startPosMouseDown.x < endPosMouseUp.x ? startPosMouseDown.x : endPosMouseUp.x),
                            (startPosMouseDown.y < endPosMouseUp.y ? startPosMouseDown.y : endPosMouseUp.y));

                    Vector2 max = new Vector2(
                        (startPosMouseDown.x > endPosMouseUp.x ? startPosMouseDown.x : endPosMouseUp.x),
                        (startPosMouseDown.y > endPosMouseUp.y ? startPosMouseDown.y : endPosMouseUp.y));

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
                    startPosMouseDown = Vector2.zero;

                    MultiSelectBtn();

                }
                //Nhấp chọn
                else
                {
                    if (startPosMouseDown == endPosMouseUp)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                        {
                            if (hit.collider.gameObject.layer == 9)
                            {
                                Unit unit = hit.collider.gameObject.GetComponent<Unit>();

                                //Đã có chọn
                                if (listSelected.Count > 0)
                                {
                                    //đã chọn là nhà
                                    if (listSelected[0].GetComponent<UnitConstruct>() || listSelected[0].GetComponent<TreeUnit>())
                                    {
                                        if (unit._property.colorTeam == Team.Red || unit._property.colorTeam == Team.None)
                                        {
                                            DeSelectAll();
                                            listSelected.Add(unit);

                                            infoPanel.SetActive(true);
                                            unit.OnSelect();

                                            foreach (Transform item in contentInfoPanel)
                                            {
                                                Destroy(item.gameObject);
                                            }

                                            if (unit.GetComponent<UnitConstruct>())
                                            {
                                                unit.GetComponent<UnitConstruct>().ShowPanel();
                                            }
                                            descriptionBtn.SetActive(true);

                                            buildPanel.SetActive(false);
                                        }
                                    }
                                    //Đã chọn là người
                                    else if (listSelected[0].GetComponent<UnitSoldier>())
                                    {
                                        //Đang chọn là người
                                        if (unit.GetComponent<UnitSoldier>())
                                        {
                                            if (unit._property.colorTeam == Team.Red)
                                            {
                                                DeSelectAll();
                                                listSelected.Add(unit);

                                                infoPanel.SetActive(true);
                                                unit.OnSelect();

                                                foreach (Transform item in contentInfoPanel)
                                                {
                                                    Destroy(item.gameObject);
                                                }

                                                if (unit.GetComponent<UnitConstruct>())
                                                {
                                                    unit.GetComponent<UnitConstruct>().ShowPanel();
                                                }
                                                descriptionBtn.SetActive(true);

                                                buildPanel.SetActive(false);
                                            }
                                        }
                                        //Đang chọn là nhà hoặc cây
                                        else
                                        {
                                            if (unit._property.colorTeam == Team.Red || unit._property.colorTeam == Team.None)
                                            {
                                                foreach (var item in listSelected)
                                                {
                                                    item.ActTo(unit);

                                                    descriptionBtn.SetActive(false);
                                                }
                                            }

                                            DeSelectAll();
                                        }
                                    }
                                }
                                else
                                {
                                    if (unit._property.colorTeam == Team.Red || unit._property.colorTeam == Team.None)
                                    {
                                        DeSelectAll();
                                        listSelected.Add(unit);

                                        infoPanel.SetActive(true);
                                        unit.OnSelect();

                                        foreach (Transform item in contentInfoPanel)
                                        {
                                            Destroy(item.gameObject);
                                        }

                                        if (unit.GetComponent<UnitConstruct>())
                                        {
                                            unit.GetComponent<UnitConstruct>().ShowPanel();
                                        }
                                        descriptionBtn.SetActive(true);

                                        buildPanel.SetActive(false);
                                    }
                                }
                            }
                            else
                            {
                                if (listSelected.Count > 0)
                                {
                                    if (listSelected[0].GetComponent<UnitSoldier>())
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
                                    }
                                    else if (listSelected[0].GetComponent<UnitConstruct>())
                                    {
                                        print("a");
                                        DeSelectAll();
                                        infoPanel.SetActive(false);
                                    }
                                    else
                                    {
                                        DeSelectAll();
                                        infoPanel.SetActive(false);
                                    }
                                }
                                descriptionBtn.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }

    private void MoveCameraAndroid()
    {
        float pinchAmount = 0;
        Quaternion desiredRotation = transform.rotation;

        DetectTouchMovement.Calculate();

        if (Mathf.Abs(DetectTouchMovement.pinchDistanceDelta) > 0)
        { // zoom
            pinchAmount = DetectTouchMovement.pinchDistanceDelta;
        }

        if (Mathf.Abs(DetectTouchMovement.turnAngleDelta) > 0)
        { // rotate
            Vector3 rotationDeg = Vector3.zero;
            rotationDeg.z = -DetectTouchMovement.turnAngleDelta;
            desiredRotation *= Quaternion.Euler(rotationDeg);
        }


        // not so sure those will work:
        cam.transform.rotation = desiredRotation;
        cam.transform.position += Vector3.forward * pinchAmount;
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
                item.UpdateHealthBar();

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
            item.DestroyHealthBar();
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
        if (!isPause)
        {
            if (!currentToBuild)
            {
                if (!EnoughResource(n))
                {
                    CreateSlotNoti("Không đủ tài nguyên");
                    return;
                }

                currentToBuild = Instantiate(Resources.Load("Construct/" + n + "_1") as GameObject, Vector2.zero, Quaternion.identity);

                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)), out hit);
                currentToBuild.transform.position = new Vector3(hit.point.x, 0, hit.point.z);

                currentToBuild.GetComponent<Building>().n = n;

                btnCancelBuild.SetActive(true);
                btnApplyBuild.SetActive(true);
                multiSelectBtn.gameObject.SetActive(false);
                buildPanel.SetActive(false);
                buildBtn.SetActive(false);

                firstMouseDown = true;
            }
        }
    }

    bool EnoughResource(string s)
    {
        int needWood = 0;
        int needMetal = 0;
        int needGold = 0;

        switch (s)
        {
            case "Farm":
                needWood = 40;
                break;
            case "House":
                needWood = 30;
                break;
            case "Barracks":
                needGold = 10;
                needMetal = 25;
                break;
            case "Archer Range":
                needGold = 20;
                needWood = 20;
                break;
        }

        if (resourcesGame._wood >= needWood && resourcesGame._metal >= needMetal && resourcesGame._gold >= needGold)
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
        if (!isPause)
        {
            buildBtn.SetActive(false);
            buildPanel.SetActive(true);
            infoPanel.SetActive(false);
        }
    }

    public void DescriptionBtnOnClick()
    {
        if (!isPause)
        {
            if (listSelected.Count > 0 && listSelected.Count < 2)
            {
                descriptionText.text = listSelected[0]._property.description;
                descriptionPanel.SetActive(true);
            }
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
        touchMovement.SetUpdate();
        touchMovement.UpdateMoveCam();
        touchMovement.UpdateZoomCam();

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
            if (cam.transform.position.z < 100)
            {
                Vector3 pos = cam.transform.position;
                pos += cam.transform.forward * .5f;

                cam.transform.position = pos;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (cam.transform.position.z > 0)
            {
                Vector3 pos = cam.transform.position;
                pos -= cam.transform.forward * .5f;

                cam.transform.position = pos;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (cam.transform.position.x > 0)
            {
                Vector3 pos = cam.transform.position;
                pos -= cam.transform.right * .5f;

                cam.transform.position = pos;
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (cam.transform.position.x < 100)
            {
                Vector3 pos = cam.transform.position;
                pos += cam.transform.right * .5f;

                cam.transform.position = pos;
            }
        }
    }

    public void CancelBuild()
    {
        DestroyImmediate(currentToBuild);

        btnCancelBuild.SetActive(false);
        btnApplyBuild.SetActive(false);
        buildPanel.SetActive(true);
    }

    public void ApplyBuild()
    {
        if (currentToBuild)
        {
            if (currentToBuild.GetComponent<Building>().OnClickBuild())
            {
                int r = UnityEngine.Random.Range(5, 20);
                float minX = currentToBuild.transform.position.x - 2;
                float minY = currentToBuild.transform.position.z - 2;
                float maxX = currentToBuild.transform.position.x + 2;
                float maxY = currentToBuild.transform.position.z + 2;

                for (int i = 0; i < r; i++)
                {
                    CreateSmokeEffect(new Vector3(UnityEngine.Random.Range(minX, maxX), 1, UnityEngine.Random.Range(minY, maxY)));
                }

                if (!testMode)
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
                }

                startPosMouseDown = Vector2.zero;

                currentToBuild.GetComponent<Building>().SetConstruct();

                GameObject g = Instantiate(Resources.Load("Construct/" + currentToBuild.GetComponent<Building>().n + "_2") as GameObject, currentToBuild.transform.position, Quaternion.identity);

                Destroy(currentToBuild);
                btnCancelBuild.SetActive(false);
                btnApplyBuild.SetActive(false);
                multiSelectBtn.gameObject.SetActive(true);
                buildPanel.SetActive(true);
                return;
            }
            //}
        }
    }

    public void ClosePanelBtn(GameObject g)
    {
        DeSelectAll();
        infoPanel.SetActive(false);

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
        Transform noti = GameObject.Find("Noti").transform;

        GameObject g = Instantiate(Resources.Load("UI/SlotNoti") as GameObject, noti);
        g.GetComponent<Text>().text = "[" + minute.ToString("00") + ":" + second.ToString("00") + "]" + s;

        noti.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Mathf.Clamp(noti.childCount * 30, 300, 30000));
    }

    public void CreateSlotMaterial(Transform trans, string s, int i)
    {
        GameObject g1 = Instantiate(Resources.Load("UI/Slot Material") as GameObject, trans);
        g1.transform.GetChild(1).GetComponent<Text>().text = i.ToString();

        if (s == "Thực")
        {
            g1.transform.GetChild(0).GetComponent<Image>().sprite = spriteFood;
        }
        else if (s == "Gỗ")
        {
            g1.transform.GetChild(0).GetComponent<Image>().sprite = spriteWood;
        }
        else if (s == "Vàng")
        {
            g1.transform.GetChild(0).GetComponent<Image>().sprite = spriteGold;
        }
        else if (s == "Sắt")
        {
            g1.transform.GetChild(0).GetComponent<Image>().sprite = spriteMetal;
        }
    }

    public void CreateBloodEffect(Vector3 v)
    {
        GameObject g = Instantiate(Resources.Load("Blood Effect") as GameObject, v, Quaternion.identity);
        Destroy(g, 1);
    }

    public void CreateSmokeEffect(Vector3 v)
    {
        GameObject g = Instantiate(Resources.Load("Smoke Effect") as GameObject, v, Quaternion.identity);
        Destroy(g, UnityEngine.Random.Range(.5f, 2f));
    }

    void SetTimeInGame()
    {
        second += Time.deltaTime;

        if (second >= 60)
        {
            second = 0;
            minute++;
        }

        if (textTimeGame)
        {
            textTimeGame.text = minute.ToString("00") + ":" + second.ToString("00");
        }
    }

    public void MenuBtn()
    {
        menuPanel.SetActive(!menuPanel.activeInHierarchy);
        isPause = menuPanel.activeInHierarchy;
        Time.timeScale = menuPanel.activeInHierarchy ? 0 : 1;
    }

    public void BtnHideListNoti()
    {
        if (ScrollViewNoti.anchoredPosition3D.x > -600)
        {
            StartCoroutine(HideListNotiCo());
        }
        else if (ScrollViewNoti.anchoredPosition3D.x < -900)
        {
            StartCoroutine(ShowListNotiCo());
        }
    }

    IEnumerator HideListNotiCo()
    {
        Vector3 v = ScrollViewNoti.anchoredPosition3D;
        while (ScrollViewNoti.anchoredPosition3D.x > -1000)
        {
            v.x -= 20;
            ScrollViewNoti.anchoredPosition3D = v;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator ShowListNotiCo()
    {
        Vector3 v = ScrollViewNoti.anchoredPosition3D;
        while (ScrollViewNoti.anchoredPosition3D.x < -550)
        {
            v.x += 20;
            ScrollViewNoti.anchoredPosition3D = v;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    /// <summary>
    /// Tạo cây
    /// </summary>
    /// <param name="v">Vị trí tạo</param>
    public void RespawnTree(Vector3 v)
    {
        StartCoroutine(RespawnTreeCo(v));
    }

    IEnumerator RespawnTreeCo(Vector3 v)
    {
        float t = UnityEngine.Random.Range(30f, 60f);

        yield return new WaitForSeconds(t);

        GameObject g = Instantiate(Resources.Load("Tree") as GameObject, v, Quaternion.identity);
    }

    /// <summary>
    /// Tạo hiệu ứng lửa
    /// </summary>
    /// <param name="v">Vị trí</param>
    /// <param name="parent">Parent</param>
    public void CreateFireEffect(Vector3 v, Transform parent)
    {
        GameObject g = Instantiate(Resources.Load("Fire Effect") as GameObject, v, Quaternion.identity, parent);
    }

    public void MultiSelectBtn()
    {
        isMultiSelectMode = !isMultiSelectMode;

        ColorBlock color = multiSelectBtn.colors;
        Text t = multiSelectBtn.transform.GetChild(0).GetComponent<Text>();
        if (isMultiSelectMode)
        {
            color.normalColor = new Color(0, .75f, 0, 1);
            t.color = Color.white;
        }
        else
        {
            color.normalColor = new Color(1, 1, 1, 1);
            t.color = Color.black;
        }

        multiSelectBtn.colors = color;
    }

    public void SaveBtn()
    {
        print("Saved");
    }

    public void LoadBtn()
    {
        print("Loaded");
    }

    public void OptionBtn()
    {
        optionPanel.SetActive(!optionPanel.activeInHierarchy);

    }

    public void RestartLevelBtn()
    {
        SceneManager.LoadScene(nameLevel);
    }

    public void OnValueChangeMusic(Scrollbar scrollbar)
    {
        int i = Mathf.RoundToInt(scrollbar.value * 100);

        textMusic.text = i + "%";

        MasterManager.master.SetMusic(i);
    }

    public void OnValueChangeSound(Scrollbar scrollbar)
    {
        int i = Mathf.RoundToInt(scrollbar.value * 100);

        textSound.text = i + "%";

        MasterManager.master.SetSound(i);
    }

    public void OnValueChangeCameraSpeed(Scrollbar scrollbar)
    {
        int i = Mathf.RoundToInt((scrollbar.value + .5f) * 100);

        textCameraSpeed.text = i + "%";

        MasterManager.master.cameraSpeed = i;
    }

    public void ExitBtn()
    {
        confirmExitPanel.SetActive(true);
    }

    public void YesExitBtn()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void NoExitBtn()
    {
        confirmExitPanel.SetActive(false);
    }

    public void ViewMapBtn()
    {
        fullMapPanel.SetActive(true);
    }
}
