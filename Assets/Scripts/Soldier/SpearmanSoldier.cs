using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearmanSoldier : UnitSoldier
{
    public float tToGet = 1;
    public float tToGetSecond;

    public int curContain;
    public int maxContain = 5;

    private void Update()
    {
        switch (_property.state)
        {
            case State.None:
                break;
            case State.Move:
                if (Vector3.Distance(transform.position, targetVector) > 1)
                {
                    MoveTo(targetVector);
                }
                else
                {
                    agent.ResetPath();
                    _property.state = State.None;
                }
                break;
            case State.MoveFarm:
                break;
            case State.GetFarm:
                break;
            case State.BackCityHallFarm:
                break;
            case State.PutFarm:
                break;
            case State.MoveTree:
                break;
            case State.GetTree:
                break;
            case State.BackCityHallTree:
                break;
            case State.PutTree:
                break;
            case State.MoveFight:
                if (Vector3.Distance(transform.position, targetAttack.transform.position) > 3)
                {
                    MoveTo(targetAttack.transform.position);
                }
                else
                {
                    agent.ResetPath();
                    _property.state = State.Fight;
                }
                break;
            case State.Fight:
                if (targetAttack)
                {
                    if (Attack1())
                    {
                        Attack2(targetAttack);
                    }
                }
                else
                {
                    agent.ResetPath();
                    _property.state = State.None;
                }
                break;
        }
    }

    public override void TakeDamage(Property property)
    {
        _property.curHealth -= property.dmgSoldier;
        UpdateHealthBar();
    }
}
