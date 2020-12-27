using UnityEngine;

public class PeasantSoldier : UnitSoldier
{
    public float tToGet = 1;
    public float tToGetSecond;

    public int curContain;
    public int maxContain = 5;

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

                Unit u = GetNearestEnemy(10);

                if (u)
                {
                    ActTo(u);
                }

                break;
            case State.Move:
                if (Vector3.Distance(transform.position, targetVector) > 1)
                {
                    MoveTo(targetVector);
                }
                else
                {
                    anim.SetBool("Move", false);

                    StopMove();
                    _property.state = State.None;
                }
                break;
            case State.MoveFarm:
                if (targetFarm)
                {
                    if (Vector3.Distance(transform.position, targetFarm.transform.position) > 1)
                    {
                        MoveTo(targetFarm.transform.position);
                    }
                    else
                    {
                        StopMove();
                        _property.state = State.GetFarm;

                        tToGetSecond = 0;
                    }
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
                        StopMove();

                        _property.state = State.PutFarm;
                    }
                }
                else
                {
                    StopMove();
                }
                break;
            case State.PutFarm:
                if (_property.colorTeam == Team.Red)
                {
                    Manager.manager.resourcesGame._food += curContain;
                }

                curContain = 0;

                Manager.manager.UpdateresourcesGame();

                _property.state = State.MoveFarm;
                break;
            case State.MoveTree:
                if (targetTree)
                {
                    if (Vector3.Distance(transform.position, targetTree.transform.position) > 2)
                    {
                        MoveTo(targetTree.transform.position);
                    }
                    else
                    {
                        StopMove();
                        _property.state = State.GetTree;

                        tToGetSecond = 0;
                    }
                }
                break;
            case State.GetTree:
                if (targetTree)
                {
                    tToGetSecond += Time.deltaTime;
                    if (tToGetSecond >= tToGet)
                    {
                        tToGetSecond = 0;

                        curContain++;
                        targetTree.OnDecreamentWood();

                        if (curContain >= maxContain)
                        {
                            _property.state = State.BackCityHallTree;
                        }
                    }
                }
                else
                {
                    TreeUnit tree = GetNearestTreeWithVector3(transform.position);
                    if(tree)
                    {
                        ActTo(tree);
                    }
                    else
                    {
                        _property.state = State.None;
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
                        StopMove();

                        _property.state = State.PutTree;
                    }
                }
                else
                {
                    StopMove();
                }
                break;
            case State.PutTree:
                if (_property.colorTeam == Team.Red)
                {
                    Manager.manager.resourcesGame._wood += curContain;
                }

                curContain = 0;

                Manager.manager.UpdateresourcesGame();

                _property.state = State.MoveTree;
                break;
            case State.MoveFight:
                if (targetAttack)
                {
                    if (Vector3.Distance(transform.position, targetAttack.transform.position) > 3)
                    {
                        MoveTo(targetAttack.transform.position);
                    }
                    else
                    {
                        StopMove();
                        _property.state = State.Fight;
                    }
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
                    StopMove();
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
