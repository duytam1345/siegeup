using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSoldier : UnitSoldier
{
    public MeshRenderer thisRenderer;
    public Material[] materialsTeam;

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
                    Unit u = GetNearestEnemy(20);

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
                    if (Vector3.Distance(transform.position, targetAttack.transform.position) > 8 + targetAttack.r)
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
                    if (Vector3.Distance(transform.position, targetAttack.transform.position) <= 8 + targetAttack.r)
                    {
                        if (Attack1())
                        {
                            //Tạo mũi tên
                            CreateArrow("Normal Arrow", _property, 50, targetAttack);
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

    public override void SetColorTeam()
    {
        switch (_property.colorTeam)
        {
            case Team.Red:
                thisRenderer.material = materialsTeam[0];
                break;
            case Team.Green:
                thisRenderer.material = materialsTeam[1];
                break;
            case Team.Blue:
                break;
            case Team.Yellow:
                break;
            case Team.Pink:
                break;
            case Team.Gray:
                break;
            case Team.None:
                break;
        }
    }
}
