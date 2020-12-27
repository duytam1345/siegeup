using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarracksConstruct : UnitConstruct
{
    public CountToCreateSoldier createSpearman;

    public CountToCreateSoldier createSwordsman;

    private void Update()
    {
        if (createSpearman.t > 0)
        {
            createSpearman.t -= Time.deltaTime;
        }

        if (createSwordsman.t > 0)
        {
            createSwordsman.t -= Time.deltaTime;
        }

        if (healthBar)
        {
            UpdateHealthBar();
        }

        if (createSpearman.Update())
        {
            CreateSpearman();
        }

        if (createSwordsman.Update())
        {
            CreateSwordsman();
        }
    }

    public override void ShowPanel()
    {
        GameObject g = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreateSpearman(); });
        g.transform.GetChild(0).GetComponent<Text>().text = "Lính giáo";

        createSpearman.fillAmount = g.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        createSpearman.textAmount = g.transform.GetChild(1).GetChild(1).GetComponent<Text>();

        Manager.manager.CreateSlotMaterial(g.transform.GetChild(2), "Gỗ", 20);
        Manager.manager.CreateSlotMaterial(g.transform.GetChild(2), "Vàng", 5);

        GameObject g2 = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g2.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreateSwordsman(); });
        g2.transform.GetChild(0).GetComponent<Text>().text = "Lính kiếm";

        Manager.manager.CreateSlotMaterial(g2.transform.GetChild(2), "Sắt", 10);
        Manager.manager.CreateSlotMaterial(g2.transform.GetChild(2), "Vàng", 10);

        createSwordsman.fillAmount = g2.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        createSwordsman.textAmount = g2.transform.GetChild(1).GetChild(1).GetComponent<Text>();
    }

    void OnClickCreateSpearman()
    {
        if (Manager.manager.resourcesGame._wood >= 20 && Manager.manager.resourcesGame._gold >= 5)
        {
            Manager.manager.resourcesGame._wood -= 20;
            Manager.manager.resourcesGame._gold -= 5;
            Manager.manager.UpdateresourcesGame();

            createSpearman.currentCount++;
        }
        else
        {
            Manager.manager.CreateSlotNoti(" Không đủ tài nguyên");
        }
    }

    void OnClickCreateSwordsman()
    {
        if (Manager.manager.resourcesGame._metal >= 10 && Manager.manager.resourcesGame._gold >= 10)
        {
            Manager.manager.resourcesGame._metal -= 10;
            Manager.manager.resourcesGame._gold -= 10;
            Manager.manager.UpdateresourcesGame();

            createSwordsman.currentCount++;
        }
        else
        {
            Manager.manager.CreateSlotNoti("Không đủ tài nguyên");
        }
    }

    void CreateSpearman()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Spearman") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z - 2));

        Manager.manager.AddToCurrentSoldier(g.GetComponent<UnitSoldier>());
    }

    void CreateSwordsman()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Swordsman") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z - 2));

        Manager.manager.AddToCurrentSoldier(g.GetComponent<UnitSoldier>());
    }

    public override void TakeDamage(Property property)
    {
        float before = (float)_property.curHealth / (float)_property.maxHealth * 100;
        float after = ((float)_property.curHealth - (float)property.dmgConstruct) / (float)_property.maxHealth * 100;

        CheckToEffectFire((int)before, (int)after);

        _property.curHealth -= property.dmgSoldier;
        UpdateHealthBar();
    }
}
