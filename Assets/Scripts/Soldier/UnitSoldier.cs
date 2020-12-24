using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitSoldier : Unit
{
    public NavMeshAgent agent;

    public Mission mission;
    public Vector3 vMission;

    public override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
    }

    public override void MoveTo(Vector3 v)
    {
        agent.SetDestination(v);
    }

    public TreeUnit GetNearestTreeWithVector3(Vector3 v)
    {
        List<TreeUnit> trees = FindObjectsOfType<TreeUnit>().ToList();

        TreeUnit nearestTree = null;
        float minDistance = Mathf.Infinity;

        foreach (var item in trees)
        {
            if (Vector3.Distance(item.transform.position, v) < minDistance)
            {
                minDistance = Vector3.Distance(item.transform.position, v);
                nearestTree = item;
            }
        }

        return nearestTree;
    }

    public FarmConstruct GetNearestFarmWithVector3(Vector3 v)
    {
        List<FarmConstruct> farms = FindObjectsOfType<FarmConstruct>().ToList();

        FarmConstruct nearestFarm = null;
        float minDistance = Mathf.Infinity;

        foreach (var item in farms)
        {
            if (item._property.colorTeam == _property.colorTeam)
            {
                if (Vector3.Distance(item.transform.position, v) < minDistance)
                {
                    minDistance = Vector3.Distance(item.transform.position, v);
                    nearestFarm = item;
                }
            }
        }

        return nearestFarm;
    }

    public Unit GetNearestEnemy(float range)
    {
        Unit u = null;
        float minDistance = range;

        foreach (var item in FindObjectsOfType<Unit>())
        {
            if (item._property.colorTeam != _property.colorTeam && item._property.colorTeam != Team.None)
            {
                if (Vector3.Distance(item.transform.position, transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(item.transform.position, transform.position);
                    u = item;
                }
            }
        }

        return u;
    }

    public override void ActTo(Unit unit)
    {
        if(unit == null)
        {
            return;
        }

        Property p = unit._property;

        if (unit._property.colorTeam != _property.colorTeam && unit._property.colorTeam != Team.None)
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
