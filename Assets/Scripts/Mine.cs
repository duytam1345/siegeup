using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Unit
{
    public enum TypeMine
    {
        Gold,
        Metal,
    }

    public TypeMine typeMine;

    public float t;

    public MeshRenderer thisRenderer;

    private void Update()
    {
        if (Manager.manager.isPause)
        {
            return;
        }

        t -= Time.deltaTime;

        if (typeMine == TypeMine.Gold)
        {
            if (t <= 0)
            {
                CityHallConstruct[] cityHalls = FindObjectsOfType<CityHallConstruct>();

                CityHallConstruct cityHall = null;

                foreach (var item in cityHalls)
                {
                    if (item._property.colorTeam == _property.colorTeam)
                    {
                        cityHall = item;
                        break;
                    }
                }

                t = 15;
                GameObject g = Instantiate(Resources.Load("Mover") as GameObject, transform.position, Quaternion.identity);
                g.GetComponent<Mover>().vTarget = cityHall.transform.position;

                if (_property.colorTeam == Team.Red)
                {
                    Manager.manager.resourcesGame._gold += 15;
                    Manager.manager.UpdateresourcesGame();
                }
            }
        }
        else
        {
            if (t <= 0)
            {
                CityHallConstruct[] cityHalls = FindObjectsOfType<CityHallConstruct>();

                CityHallConstruct cityHall = null;

                foreach (var item in cityHalls)
                {
                    if (item._property.colorTeam == _property.colorTeam)
                    {
                        cityHall = item;
                        break;
                    }
                }

                t = 20;
                GameObject g = Instantiate(Resources.Load("Mover") as GameObject, transform.position, Quaternion.identity);
                g.GetComponent<Mover>().vTarget = cityHall.transform.position;

                if (_property.colorTeam == Team.Red)
                {
                    Manager.manager.resourcesGame._metal += 20;
                    Manager.manager.UpdateresourcesGame();
                }
            }
        }
    }

    public override void SetColorTeam()
    {
        if (thisRenderer)
        {
            switch (_property.colorTeam)
            {
                case Team.Red:
                    thisRenderer.material = Manager.manager.materialsTeam[0];
                    break;
                case Team.Green:
                    thisRenderer.material = Manager.manager.materialsTeam[1];
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
}
