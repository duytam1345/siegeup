using GD.MinMaxSlider;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AnimatedValues;
using System.Linq;

public enum Mission
{
    Farm,
    Tree,
    Move,
    None,
}

//Tái tạo lại nếu xung quanh không có đối tượng khác phe, và vị trí đó trống. sau một khoảng thời gian.
[System.Serializable]
public class AutoBuild
{
    //Vị trí tạo
    public Vector3 v;

    //Vị trí để kiểm tra trống hay không.
    public UnitConstruct construct;

    //Tên công trình
    public string n;

    public EnemyController controller;

    public float t;
    public float tSecond;

    public bool Check()
    {
        if (RoundNull())
        {
            if (!construct)
            {
                tSecond -= Time.deltaTime;

                if (tSecond <= 0)
                {
                    tSecond = t;
                    Create();
                }

                return true;
            }
        }

        return false;
    }

    bool RoundNull()
    {
        List<Unit> units = GameObject.FindObjectsOfType<Unit>().ToList();

        foreach (var item in units)
        {
            if (item._property.colorTeam != controller.team && item._property.colorTeam != Team.None)
            {
                if (Vector3.Distance(controller.CityHallConstruct.transform.position, item.transform.position) <= 25)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void Create()
    {
        GameObject g = GameObject.Instantiate(Resources.Load("Construct/" + n + "_2") as GameObject, v, Quaternion.identity);

        construct = g.GetComponent<UnitConstruct>();

        construct._property.colorTeam = controller.team;
        construct.SetColorTeam();

        switch (n)
        {
            case "Farm":
                controller.farmConstructs.Add(construct.GetComponent<FarmConstruct>());
                break;
            case "Barracks":
                controller.barracksConstructs.Add(construct.GetComponent<BarracksConstruct>());
                break;
            case "Archer Range":
                controller.archerRangeConstructs.Add(construct.GetComponent<ArcherRangeConstruct>());
                break;
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
        if (controller.CityHallConstruct.createPeasant.t <= 0)
        {
            controller.CityHallConstruct.createPeasant.StartCreate();
            if (sTarget == "Farm")
            {
                if (controller.farmConstructs.Count <= 0)
                {
                    return;
                }

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

    [MinMaxSlider(0, 50)]
    public Vector2Int amountR;

    //


    public void UpdateToCreate()
    {
        if (tToCreateSecond > 0)
        {
            tToCreateSecond -= Time.deltaTime;
        }

        if (tToCreateSecond <= 0)
        {
            Manager.manager.StartCoroutine(CreateCo());
            SetTime();
        }
    }

    void SetTime()
    {
        if (typeWave == TypeWave.Default)
        {
            tToCreateSecond = tToCreate;
        }
        else
        {
            tToCreateSecond = Random.Range(tToCreateR.x, tToCreateR.y);
        }
    }

    IEnumerator CreateCo()
    {
        List<UnitSoldier> soldiers = new List<UnitSoldier>();

        int a = amountCreate;

        switch (typeWave)
        {
            case TypeWave.Default:
                break;
            case TypeWave.Random:
                a = Random.Range(amountR.x, amountR.y);
                break;
        }

        for (int i = 0; i < a; i++)
        {
            switch (nameCreate)
            {
                case "Archerman":

                    if (controller.archerRangeConstructs.Count > 0)
                    {
                        if (!constructToCreate)
                        {
                            if (controller.archerRangeConstructs.Count > 0)
                            {
                                constructToCreate = controller.archerRangeConstructs[0];
                            }
                        }

                        ArcherRangeConstruct archerRange = constructToCreate.GetComponent<ArcherRangeConstruct>();

                        yield return new WaitUntil(() => { return (archerRange.createArcherman.t <= 0 ? true : false); });

                        archerRange.createArcherman.StartCreate();

                        GameObject g = GameObject.Instantiate(Resources.Load("Soldier/" + nameCreate) as GameObject,
                            constructToCreate.posToCreateSoldier.position, Quaternion.identity);

                        Vector3 vMove = new Vector3();
                        vMove.x = Random.Range(-1f, 1f);
                        vMove.z = Random.Range(0, -2f);

                        g.GetComponent<UnitSoldier>().SetMove(g.transform.position + vMove);

                        g.GetComponent<UnitSoldier>()._property.colorTeam = controller.team;
                        g.GetComponent<UnitSoldier>().SetColorTeam();

                        soldiers.Add(g.GetComponent<UnitSoldier>());

                        SetTime();
                    }
                    break;
                case "Spearman":
                    if (controller.barracksConstructs.Count > 0)
                    {

                        BarracksConstruct barracks = constructToCreate.GetComponent<BarracksConstruct>();

                        yield return new WaitUntil(() => { return (barracks.createSpearman.t <= 0 ? true : false); });

                        barracks.createSpearman.StartCreate();

                        GameObject g2 = GameObject.Instantiate(Resources.Load("Soldier/" + nameCreate) as GameObject,
                            constructToCreate.posToCreateSoldier.position, Quaternion.identity);
                        g2.GetComponent<UnitSoldier>()._property.colorTeam = controller.team;
                        g2.GetComponent<UnitSoldier>().SetColorTeam();

                        soldiers.Add(g2.GetComponent<UnitSoldier>());

                        SetTime();
                    }
                    break;
            }
        }

        foreach (var item in soldiers)
        {
            item.GetComponent<UnitSoldier>().SetMove(vAttack);
            item.GetComponent<UnitSoldier>().mission = Mission.Move;
            item.GetComponent<UnitSoldier>().vMission = vAttack;
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
    public List<AutoBuild> autoBuilds;
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
        foreach (var item in autoBuilds)
        {
            item.controller = this;
        }

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
        if (Manager.manager.isPause)
        {
            return;
        }

        foreach (var item in autoBuilds)
        {
            if (item.Check())
            {
                break;
            }
        }

        if (CityHallConstruct)
        {
            foreach (var item in hWaves)
            {
                item.Check();
            }
        }
        foreach (var item in timeWaves)
        {
            item.UpdateToCreate();
        }
    }
}
