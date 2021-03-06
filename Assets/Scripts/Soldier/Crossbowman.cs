﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbowman : UnitSoldier
{
    private void Update()
    {
        if (healthBar)
        {
            UpdateHealthBar();
        }

        if (Manager.manager.isPause)
        {
            return;
        }

        switch (_property.state)
        {
            case State.None:
                if (tToCheckNearEnemySecond <= 0)
                {
                    Unit u = GetNearestEnemy(10);

                    if (u)
                    {
                        ActTo(u);
                    }
                }
                else
                {
                    tToCheckNearEnemySecond -= Time.deltaTime;
                }

                switch (mission)
                {
                    case Mission.Farm:
                        break;
                    case Mission.Tree:
                        break;
                    case Mission.Move:
                        SetMove(vMission);
                        break;
                    case Mission.None:
                        break;
                }

                break;
            case State.Move:
                if (tToCheckNearEnemySecond <= 0)
                {
                    Unit u = GetNearestEnemy(10);

                    if (u)
                    {
                        ActTo(u);
                    }
                }
                else
                {
                    tToCheckNearEnemySecond -= Time.deltaTime;
                }

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
                if (targetAttack)
                {
                    if (Vector3.Distance(transform.position, targetAttack.transform.position) > 8)
                    {
                        MoveTo(targetAttack.transform.position);
                    }
                    else
                    {
                        agent.ResetPath();
                        _property.state = State.Fight;
                    }
                }
                else
                {
                    _property.state = State.None;
                }
                break;
            case State.Fight:
                if (targetAttack)
                {
                    if (Vector3.Distance(transform.position, targetAttack.transform.position) <= 8)
                    {
                        if (Attack1())
                        {
                            //Tạo mũi tên
                            CreateArrow("Normal Arrow", _property, 10, targetAttack);
                        }
                    }
                    else
                    {
                        _property.state = State.None;
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
        Manager.manager.CreateBloodEffect(transform.position);

        _property.curHealth -= property.dmgSoldier;
        UpdateHealthBar();
    }
}
