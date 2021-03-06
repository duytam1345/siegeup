﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Team
{
    Red, // player
    Green,
    Blue,
    Yellow,
    Pink,
    Gray,
    None,// tree, fish,..
}

public enum State
{
    None,//khong lam gi 
    Move,//di chuyen
    MoveFarm, // di toi farm
    GetFarm,//thu hoach lua
    BackCityHallFarm,//di ve kho
    PutFarm,//cho lua vao kho
    MoveTree,//di toi tree
    GetTree,//chat cay
    BackCityHallTree,//di ve kho
    PutTree,//cho cay vao kho
    MoveFight,//di toi danh nhau
    Fight,//danh nhau
}

[System.Serializable]
public class Property
{
    public string _name;

    public int id;

    [TextArea]
    public string description;

    public int maxHealth;
    public int curHealth;

    public int dmgConstruct;
    public int dmgSoldier;

    public Team colorTeam;
    public State state;
}

public class Unit : MonoBehaviour
{
    public Property _property;

    public GameObject circleSelect;

    public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

    public TreeUnit targetTree;
    public FarmConstruct targetFarm;
    public Vector3 targetVector;
    public Unit targetAttack;

    public CityHallConstruct cityHall;

    //if la enemy
    public EnemyController controller;

    public float tToAttack;
    public float tToAttackSecond;

    public int h;// Chiều cao, hiển thị healthbar;
    public int m;//chiều dài, hiển thị healthbar;
    public int r;//bán kính, tìm đường

    public GameObject healthBar;

    public float tToCheckNearEnemy;
    public float tToCheckNearEnemySecond;

    public virtual void Start()
    {
        circleSelect = transform.GetChild(0).gameObject;
        circleSelect.SetActive(false);

        foreach (Transform item in transform.GetChild(1).transform)
        {
            if (item.GetComponent<MeshRenderer>() != null)
            {
                meshRenderers.Add(item.GetComponent<MeshRenderer>());
            }
        }

        SetColorTeam();

        foreach (var item in FindObjectsOfType<CityHallConstruct>())
        {
            if (item._property.colorTeam == _property.colorTeam)
            {
                cityHall = item;
                return;
            }
        }
    }

    public void OnSelect()
    {
        if (circleSelect)
        {
            circleSelect.SetActive(true);
        }
    }

    public void OnDeSelect()
    {
        if (circleSelect)
        {
            circleSelect.SetActive(false);
        }
    }

    public void SetMove(Vector3 v)
    {
        if (targetTree)
        {
            targetTree.curPeasant = 0;
        }

        if (targetFarm)
        {
            targetFarm.curPeasant = 0;
        }

        if (_property.state == State.Fight)
        {
            tToCheckNearEnemySecond = 5;
        }

        _property.state = State.Move;
        targetVector = v;
    }

    public virtual void MoveTo(Vector3 v) { }

    public virtual void SetColorTeam()
    {
        //Material m = null;

        //switch (_property.colorTeam)
        //{
        //    case Team.Red:
        //        m = Resources.Load("Material/Color/Red") as Material;
        //        break;
        //    case Team.Green:
        //        m = Resources.Load("Material/Color/Green") as Material;
        //        break;
        //    case Team.Blue:
        //        m = Resources.Load("Material/Color/Blue") as Material;
        //        break;
        //    case Team.Yellow:
        //        m = Resources.Load("Material/Color/Yellow") as Material;
        //        break;
        //    case Team.Pink:
        //        m = Resources.Load("Material/Color/Pink") as Material;
        //        break;
        //    case Team.Gray:
        //        m = Resources.Load("Material/Color/Gray") as Material;
        //        break;
        //    case Team.None:
        //        break;
        //}

        //if (m != null)
        //{
        //    foreach (var item in meshRenderers)
        //    {
        //        item.material = m;
        //    }
        //}
    }

    public virtual void ActTo(Unit unit) { }

    /// <summary>
    /// tốc độ đánh -= time.deltatime;
    /// Gọi hàm Attack2();
    /// </summary>
    public bool Attack1()
    {
        tToAttackSecond -= Time.deltaTime;
        if (tToAttackSecond <= 0)
        {
            tToAttackSecond = tToAttack;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gây sát thương lên Unit
    /// </summary>
    public void Attack2(Unit unit)
    {
        unit.TakeDamage(_property);
    }

    public virtual void TakeDamage(Property property) { }

    public void UpdateHealthBar()
    {
        if (!healthBar)
        {
            healthBar = Instantiate(Resources.Load("UI/HealthBar") as GameObject, GameObject.Find("Canvas 2").transform);
        }

        healthBar.transform.position = new Vector3(transform.position.x, transform.position.y + h, transform.position.z);

        healthBar.transform.GetChild(1).GetComponent<Image>().fillAmount = (float)_property.curHealth / (float)_property.maxHealth;

        if (_property.curHealth <= 0)
        {
            if (_property._name == "City Hall")
            {
                if (_property.colorTeam == Team.Red)
                {
                    Manager.manager.SetPanelWinLose("Lose");
                }
                else
                {
                    Manager.manager.SetPanelWinLose("Win");
                }
            }

            OnDeath();
            Destroy(healthBar.gameObject);
            Destroy(gameObject);
        }
    }

    public void DestroyHealthBar()
    {
        if (healthBar)
        {
            Destroy(healthBar.gameObject);
        }
    }

    /// <summary>
    /// Tạo đạn
    /// </summary>
    /// <param name="nameArrow">Tên</param>
    /// <param name="p">Thuộc tính</param>
    /// <param name="speed">Tốc độ</param>
    /// <param name="target">Mục tiêu</param>
    public void CreateArrow(string nameArrow, Property p, float speed, Unit target)
    {
        GameObject arrow = Instantiate(Resources.Load("Orther/" + nameArrow) as GameObject,
            new Vector3(transform.position.x, transform.position.y + h, transform.position.z), Quaternion.identity);

        Bullet b = arrow.GetComponent<Bullet>();
        b.speed = speed;
        b.property = p;
        b.unitTarget = target;
    }

    public void OnDeath()
    {
        if (targetTree)
        {
            targetTree.curPeasant = 0;
        }

        if (Manager.manager.listSelected.Contains(this))
        {
            Manager.manager.listSelected.Remove(this);
        }

        if (GetComponent<UnitSoldier>())
        {
            int r = Random.Range(10, 20);

            for (int i = 0; i < r; i++)
            {
                GameObject g = Instantiate(Resources.Load("Cube") as GameObject,
                    transform.position + new Vector3(Random.Range(-.5f, .5f), 0, Random.Range(-.5f, .5f)), Quaternion.identity);

                switch (_property.colorTeam)
                {
                    case Team.Red:
                        g.GetComponent<MeshRenderer>().material = Resources.Load("Material/Color/Red") as Material;
                        break;
                    case Team.Green:
                        g.GetComponent<MeshRenderer>().material = Resources.Load("Material/Color/Green") as Material;
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

                float scale = Random.Range(.05f, .3f);

                g.transform.localScale = new Vector3(scale, scale, scale);

                Destroy(g, 5f);
            }

            Manager.manager.RemoveToCurrentSoldier(GetComponent<UnitSoldier>());
        }

        if (_property._name == "House")
        {
            Manager.manager.resourcesGame._maxSoldier -= 10;
            Manager.manager.UpdateresourcesGame();
        }
    }
}
