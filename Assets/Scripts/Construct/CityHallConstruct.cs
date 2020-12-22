using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityHallConstruct : UnitConstruct
{
    public CountToCreateSoldier createPeasant;

    private void Update()
    {
        if (healthBar)
        {
            UpdateHealthBar();
        }

        if (createPeasant.currentCount > 0)
        {
            CreatePeasant();
            createPeasant.currentCount--;
        }
    }

    public override void ShowPanel()
    {
        GameObject g = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreatePeasant(); });
        g.transform.GetChild(0).GetComponent<Text>().text = "Peasant";
    }

    void OnClickCreatePeasant()
    {
        createPeasant.currentCount++;
    }

    void CreatePeasant()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Peasant") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x,g.transform.position.y,g.transform.position.z-2));
    }

    public override void TakeDamage(Property property)
    {
        _property.curHealth -= property.dmgSoldier;
        UpdateHealthBar();
    }
}
