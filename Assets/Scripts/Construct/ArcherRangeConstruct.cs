using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcherRangeConstruct : UnitConstruct
{
    public CountToCreateSoldier createArcherman;

    public CountToCreateSoldier createCrossbowman;

    private void Update()
    {
        if (createArcherman.t > 0)
        {
            createArcherman.t -= Time.deltaTime;
        }

        if (createCrossbowman.t > 0)
        {
            createCrossbowman.t -= Time.deltaTime;
        }

        if (healthBar)
        {
            UpdateHealthBar();
        }

        if (createArcherman.Update())
        {
            CreateArcherman();
        }

        if (createCrossbowman.Update())
        {
            CreateCrossbowman();
        }
    }

    public override void ShowPanel()
    {
        GameObject g = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreateArcherman(); });
        g.transform.GetChild(0).GetComponent<Text>().text = "Lính cung";

        Manager.manager.CreateSlotMaterial(g.transform.GetChild(2), "Thực", 15);
        Manager.manager.CreateSlotMaterial(g.transform.GetChild(2), "Gỗ", 15);

        createArcherman.fillAmount = g.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        createArcherman.textAmount = g.transform.GetChild(1).GetChild(1).GetComponent<Text>();

        GameObject g2 = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g2.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreateCrossbowman(); });
        g2.transform.GetChild(0).GetComponent<Text>().text = "Lính nỏ";

        Manager.manager.CreateSlotMaterial(g2.transform.GetChild(2), "Thực", 30);
        Manager.manager.CreateSlotMaterial(g2.transform.GetChild(2), "Sắt", 10);

        createCrossbowman.fillAmount = g2.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        createCrossbowman.textAmount = g2.transform.GetChild(1).GetChild(1).GetComponent<Text>();
    }

    void OnClickCreateArcherman()
    {
        if (Manager.manager.resourcesGame._food >= 15 && Manager.manager.resourcesGame._wood >= 15)
        {
            Manager.manager.resourcesGame._food -= 15;
            Manager.manager.resourcesGame._wood -= 15;
            Manager.manager.UpdateresourcesGame();

            createArcherman.currentCount++;
        }
        else
        {
            Manager.manager.CreateSlotNoti("Không đủ tài nguyên");
        }
    }

    void OnClickCreateCrossbowman()
    {
        if (Manager.manager.resourcesGame._food >= 30 && Manager.manager.resourcesGame._metal >= 10)
        {
            Manager.manager.resourcesGame._food -= 15;
            Manager.manager.resourcesGame._wood -= 15;
            Manager.manager.UpdateresourcesGame();

            createCrossbowman.currentCount++;
        }
        else
        {
            Manager.manager.CreateSlotNoti("Không đủ tài nguyên");
        }
    }

    void CreateArcherman()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Archerman") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z - 2));

        Manager.manager.AddToCurrentSoldier(g.GetComponent<UnitSoldier>());
    }

    void CreateCrossbowman()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Crossbowman") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z - 2));

        Manager.manager.AddToCurrentSoldier(g.GetComponent<UnitSoldier>());
    }

    public override void TakeDamage(Property property)
    {
        float before = (float)_property.curHealth / (float)_property.maxHealth * 100;
        float after = ((float)_property.curHealth - (float)property.dmgConstruct) / (float)_property.maxHealth * 100;

        CheckToEffectFire((int)before, (int)after);

        _property.curHealth -= property.dmgConstruct;
        UpdateHealthBar();
    }
}
