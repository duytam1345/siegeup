﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeasantSoldier : UnitSoldier
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
                if (Vector3.Distance(transform.position, targetFarm.transform.position) > 1)
                {
                    MoveTo(targetFarm.transform.position);
                }
                else
                {
                    agent.ResetPath();
                    _property.state = State.GetFarm;

                    tToGetSecond = 0;
                }
                break;
            case State.GetFarm:
                tToGetSecond += Time.deltaTime;
                if (tToGetSecond >= tToGet)
                {
                    tToGetSecond = 0;

                    curContain++;
                    if (curContain >= maxContain)
                    {
                        _property.state = State.BackCityHallFarm;
                    }
                }
                break;
            case State.BackCityHallFarm:
                if (cityHall)
                {
                    if (Vector3.Distance(transform.position, cityHall.transform.position) > 1)
                    {
                        MoveTo(cityHall.transform.position);
                    }
                    else
                    {
                        agent.ResetPath();

                        _property.state = State.PutFarm;
                    }
                }
                else
                {
                    agent.ResetPath();
                }
                break;
            case State.PutFarm:
                Manager.manager.resourcesGame._food += curContain;

                curContain = 0;

                Manager.manager.UpdateresourcesGame();

                _property.state = State.MoveFarm;
                break;
            case State.MoveTree:
                if (Vector3.Distance(transform.position, targetTree.transform.position) > 1)
                {
                    MoveTo(targetTree.transform.position);
                }
                else
                {
                    agent.ResetPath();
                    _property.state = State.GetTree;

                    tToGetSecond = 0;
                }
                break;
            case State.GetTree:
                tToGetSecond += Time.deltaTime;
                if (tToGetSecond >= tToGet)
                {
                    tToGetSecond = 0;

                    curContain++;
                    if (curContain >= maxContain)
                    {
                        _property.state = State.BackCityHallTree;
                    }
                }
                break;
            case State.BackCityHallTree:
                if (cityHall)
                {
                    if (Vector3.Distance(transform.position, cityHall.transform.position) > 1)
                    {
                        MoveTo(cityHall.transform.position);
                    }
                    else
                    {
                        agent.ResetPath();

                        _property.state = State.PutTree;
                    }
                }
                else
                {
                    agent.ResetPath();
                }
                break;
            case State.PutTree:
                Manager.manager.resourcesGame._wood += curContain;

                curContain = 0;

                Manager.manager.UpdateresourcesGame();

                _property.state = State.MoveTree;
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