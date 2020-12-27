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

    public Animator anim;

    public override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();

        if (transform.GetChild(1).GetChild(0).GetComponent<Animator>())
        {
            anim = transform.GetChild(1).GetChild(0).GetComponent<Animator>();
        }
    }

    public override void MoveTo(Vector3 v)
    {
        anim.SetBool("Move", true);

        agent.SetDestination(v);
    }

    public void StopMove()
    {
        anim.SetBool("Move", false);

        agent.ResetPath();
    }

    public TreeUnit GetNearestTreeWithVector3(Vector3 v)
    {
        List<TreeUnit> trees = FindObjectsOfType<TreeUnit>().ToList();

        TreeUnit nearestTree = null;
        float minDistance = Mathf.Infinity;

        foreach (var item in trees)
        {
            if (item.curPeasant <= 0)
            {
                if (Vector3.Distance(item.transform.position, v) < minDistance)
                {
                    minDistance = Vector3.Distance(item.transform.position, v);
                    nearestTree = item;
                }
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
            if (item.curPeasant < item.maxPeasant)
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
        if (unit == null)
        {
            return;
        }

        Property p = unit._property;

        if (unit._property.colorTeam != _property.colorTeam && unit._property.colorTeam != Team.None)
        {
            if (targetTree)
            {
                targetTree.curPeasant = 0;
            }

            targetAttack = unit;
            _property.state = State.MoveFight;
        }
        else
        {
            if (p._name == "Tree")
            {
                if (targetTree)
                {
                    targetTree.curPeasant = 0;
                }

                TreeUnit tree = GetNearestTreeWithVector3(unit.transform.position);
                if (GetComponent<PeasantSoldier>())
                {
                    targetTree = tree;
                    targetTree.curPeasant++;

                    _property.state = State.MoveTree;
                }
            }
            else if (p._name == "Farm")
            {
                if (targetTree)
                {
                    targetTree.curPeasant = 0;
                }

                FarmConstruct farm = GetNearestFarmWithVector3(unit.transform.position);

                if (GetComponent<PeasantSoldier>())
                {
                    targetFarm = farm;
                    targetFarm.curPeasant++;

                    _property.state = State.MoveFarm;
                }
            }
        }
    }
}
