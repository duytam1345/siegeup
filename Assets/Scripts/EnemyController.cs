using GD.MinMaxSlider;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AnimatedValues;

public enum Mission
{
    Farm,
    Tree,
    Move,
    None,
}

[System.Serializable]
public class NewWave
{
    public enum TypeWave
    {
        LocalWave,
        TimeWave
    }

    public TypeWave typeWave;

    public string nameCreate;
    public int amountCreate;

    public Vector3 v;

    public float tToCreate;
    public float tToCreateSecond;

    public bool done;

    public Mission mission;

    public void Create(EnemyController controller)
    {
        for (int i = 0; i < amountCreate; i++)
        {
            GameObject g = GameObject.Instantiate(Resources.Load("Soldier/" + nameCreate) as GameObject,
                    controller.CityHallConstruct.posToCreateSoldier.position, Quaternion.identity);

            UnitSoldier cUnit = g.GetComponent<UnitSoldier>();
            cUnit.controller = controller;

            cUnit._property.colorTeam = controller.team;
            cUnit.SetColorTeam();

            switch (mission)
            {
                case Mission.Farm:
                    cUnit.ActTo(cUnit.GetNearestFarmWithVector3(v));
                    break;
                case Mission.Tree:
                    cUnit.ActTo(cUnit.GetNearestTreeWithVector3(v));
                    break;
                case Mission.Move:
                    cUnit.SetMove(v);
                    break;
                case Mission.None:
                    break;
            }
        }
    }
}

//Kiểm tra và tái tạo nếu vị trí binh lính trống
[System.Serializable]
public class HWave
{
    public EnemyController controller;

    //Độ ưu tiên
    public int index;

    public Unit targetUnit;
    public string sTarget;
    public Vector3 vTarget;

    public UnitSoldier soldier;

    public void Check()
    {
        if (!soldier)
        {
            Create();
        }
    }

    void Create()
    {
        //if (targetUnit._property._name == "Tree")
        //{
        //    GameObject g = GameObject.Instantiate(Resources.Load("Soldier/Peasant") as GameObject,
        //        controller.CityHallConstruct.posToCreateSoldier.position,
        //        Quaternion.identity);

        //    soldier = g.GetComponent<UnitSoldier>();
        //    soldier._property.colorTeam = controller.team;
        //    soldier.SetColorTeam();

        //    soldier.ActTo(targetUnit);
        //}
        //else if (targetUnit._property._name == "Farm")
        //{
        //    GameObject g = GameObject.Instantiate(Resources.Load("Soldier/Peasant") as GameObject,
        //        controller.CityHallConstruct.posToCreateSoldier.position,
        //        Quaternion.identity);

        //    soldier = g.GetComponent<UnitSoldier>();
        //    soldier._property.colorTeam = controller.team;
        //    soldier.SetColorTeam();

        //    soldier.ActTo(targetUnit);
        //}

        if (sTarget == "Farm")
        {
            GameObject g = GameObject.Instantiate(Resources.Load("Soldier/Peasant") as GameObject,
                controller.CityHallConstruct.posToCreateSoldier.position,
                Quaternion.identity);

            soldier = g.GetComponent<UnitSoldier>();
            soldier._property.colorTeam = controller.team;
            soldier.SetColorTeam();

            soldier.ActTo(soldier.GetNearestFarmWithVector3(vTarget));
        }
        else if (sTarget == "Tree")
        {
            GameObject g = GameObject.Instantiate(Resources.Load("Soldier/Peasant") as GameObject,
                controller.CityHallConstruct.posToCreateSoldier.position,
                Quaternion.identity);

            soldier = g.GetComponent<UnitSoldier>();
            soldier._property.colorTeam = controller.team;
            soldier.SetColorTeam();

            soldier.ActTo(soldier.GetNearestTreeWithVector3(vTarget));
        }
    }
}

//Sau một khoảng thời gian, binh lính sẽ được tạo để tấn công
[System.Serializable]
public class TimeWave
{
    public enum TypeWave
    {
        Default,//Theo cài đặt
        Random,//Ngẫu nhiên thời gian tạo, lượng lính, loại lính, vị trí tấn công
    }

    public TypeWave typeWave;
    public EnemyController controller;

    public Vector3 vStart;

    public Vector3 vAttack;

    public UnitConstruct constructToCreate;

    //None
    public float tToCreate;
    public float tToCreateSecond;

    public int amountCreate;

    public string nameCreate;
    //

    // Random
    [MinMaxSlider(0, 60)]
    public Vector2Int tToCreateR;

    [MinMaxSlider(0, 10)]
    public Vector2Int amountR;

    //


    public void UpdateToCreate()
    {
        tToCreateSecond -= Time.deltaTime;

        if (tToCreateSecond <= 0)
        {
            Create();
            tToCreateSecond = tToCreate;
        }
    }

    public void Create()
    {
        switch (typeWave)
        {
            case TypeWave.Default:

                for (int i = 0; i < amountCreate; i++)
                {
                    GameObject g = GameObject.Instantiate(Resources.Load("Soldier/" + nameCreate) as GameObject,
                        constructToCreate.posToCreateSoldier.position, Quaternion.identity);
                    g.GetComponent<UnitSoldier>()._property.colorTeam = controller.team;
                    g.GetComponent<UnitSoldier>().SetColorTeam();

                    g.GetComponent<UnitSoldier>().SetMove(vAttack);
                    g.GetComponent<UnitSoldier>().mission = Mission.Move;
                    g.GetComponent<UnitSoldier>().vMission = vAttack;
                }

                break;
            case TypeWave.Random:
                break;
            default:
                break;
        }
    }
    public void SetStart()
    {
        switch (typeWave)
        {
            case TypeWave.Default:
                //tToCreateSecond = tToCreate;
                break;
            case TypeWave.Random:
                break;
        }
    }
}

public class EnemyController : MonoBehaviour
{

    //public NewWave[] newWaves;

    public List<HWave> hWaves;
    public List<TimeWave> timeWaves;

    public CityHallConstruct CityHallConstruct;

    public List<FarmConstruct> farmConstructs;

    public List<BarracksConstruct> barracksConstructs;

    public List<ArcherRangeConstruct> archerRangeConstructs;

    public List<HouseConstruct> houseConstructs;

    public Team team;

    public List<PeasantSoldier> peasantSoldiers;
    public List<ArcherSoldier> archerSoldiers;
    public List<SpearmanSoldier> spearmanSoldiers;

    private void Start()
    {
        foreach (var item in timeWaves)
        {
            item.controller = this;
            item.SetStart();
        }

        foreach (var item in hWaves)
        {
            item.controller = this;
        }
    }

    private void Update()
    {
        foreach (var item in hWaves)
        {
            item.Check();
        }

        foreach (var item in timeWaves)
        {
            item.UpdateToCreate();
        }
    }
}
