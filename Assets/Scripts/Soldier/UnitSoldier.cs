using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitSoldier : Unit
{
    public NavMeshAgent agent;

    public override void Start()
    {
        base.Start()
        agent = GetComponent<NavMeshAgent>();
    }

    public override void MoveTo(Vector3 v)
    {
        agent.SetDestination(v);
    }

    public Tree GetNearestTree(Vector3 v)
    {
        return null;
    }

    public override void ActTo(Unit unit)
    {
        Property p = unit._property;

        if (unit._property.colorTeam != Team.Red && unit._property.colorTeam != Team.None)
        {
            targetAttack = unit;
            _property.state = State.MoveFight;
        }
        else
        {
            if (p._name == "Tree")
            {
                targetTree = unit.GetComponent<TreeUnit>();
                _property.state = State.MoveTree;
            }
            else if (p._name == "Farm")
            {
                targetFarm = unit.GetComponent<FarmConstruct>();
                _property.state = State.MoveFarm;
            }
        }
    }
}
