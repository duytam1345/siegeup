using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseConstruct : UnitConstruct
{
    private void Update()
    {
        if (healthBar)
        {
            UpdateHealthBar();
        }
    }

    public override void TakeDamage(Property property)
    {
        _property.curHealth -= property.dmgConstruct;
        UpdateHealthBar();
    } 
}
