using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmConstruct : UnitConstruct
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
        _property.curHealth -= property.dmgSoldier;
        UpdateHealthBar();
    }
}
