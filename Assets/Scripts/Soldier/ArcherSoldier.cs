using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSoldier : UnitSoldier
{
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
                if (Vector3.Distance(transform.position, targetAttack.transform.position) > 8)
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
                        //Tạo mũi tên
                        CreateArrow("Normal Arrow", _property, 10, targetAttack);
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
}
