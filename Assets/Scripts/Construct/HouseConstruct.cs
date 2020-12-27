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
        float before = (float)_property.curHealth / (float)_property.maxHealth * 100;
        float after = ((float)_property.curHealth - (float)property.dmgConstruct) / (float)_property.maxHealth * 100;

        CheckToEffectFire((int)before, (int)after);

        _property.curHealth -= property.dmgConstruct;
        UpdateHealthBar();
    }
}
