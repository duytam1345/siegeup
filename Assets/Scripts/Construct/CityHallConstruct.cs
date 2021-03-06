﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityHallConstruct : UnitConstruct
{
    public CountToCreateSoldier createPeasant;

    private void Update()
    {
        if (createPeasant.t > 0)
        {
            createPeasant.t -= Time.deltaTime;
        }

        if (healthBar)
        {
            UpdateHealthBar();
        }

        if (createPeasant.Update())
        {
            CreatePeasant();
        }
    }

    public override void ShowPanel()
    {
        GameObject g = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreatePeasant(); });
        g.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Nông dân";

        Manager.manager.CreateSlotMaterial(g.transform.GetChild(2), "Thực", 15);

        createPeasant.fillAmount = g.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        createPeasant.textAmount = g.transform.GetChild(1).GetChild(1).GetComponent<Text>();
    }

    void OnClickCreatePeasant()
    {
        if (Manager.manager.resourcesGame._food >= 15)
        {
            createPeasant.currentCount++;
            Manager.manager.resourcesGame._food -= 15;
            Manager.manager.UpdateresourcesGame();
        }
        else
        {
            Manager.manager.CreateSlotNoti("Không đủ tài nguyên");
        }
    }

    void CreatePeasant()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Peasant") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z - 2));

        Manager.manager.AddToCurrentSoldier(g.GetComponent<UnitSoldier>());
    }

    public override void TakeDamage(Property property)
    {
        if (Manager.manager.testMode)
        {
            if (_property.colorTeam == Team.Red)
            {
                return;
            }
        }

        float before = (float)_property.curHealth / (float)_property.maxHealth * 100;
        float after = ((float)_property.curHealth - (float)property.dmgConstruct) / (float)_property.maxHealth * 100;

        CheckToEffectFire((int)before, (int)after);

        _property.curHealth -= property.dmgSoldier;
        UpdateHealthBar();
    }
}
