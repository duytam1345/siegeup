using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public string n;

    public List<MeshRenderer> meshRenderers;

    public List<Collider> colliders;

    public List<GameObject> trees;

    private void Start()
    {
        meshRenderers = new List<MeshRenderer>();

        foreach (Transform item in transform)
        {
            if (item.GetComponent<MeshRenderer>())
            {
                meshRenderers.Add(item.GetComponent<MeshRenderer>());
            }
        }

        colliders = new List<Collider>();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, Manager.manager.cityHallConstruct.transform.position) > 30)
        {
            SetColor("Red");
            return;
        }

        if(transform.position.y>.5f)
        {
            SetColor("Red");
            return;
        }

        if (colliders.Count > 0)
        {
            SetColor("Red");
        }
        else
        {
            SetColor("Green");
        }
    }

    void SetColor(string s)
    {
        foreach (var item in meshRenderers)
        {
            item.material = Resources.Load("Material/Color/" + s) as Material;
        }
    }

    public void SetConstruct()
    {
        if (n == "House")
        {
            Manager.manager.resourcesGame._maxSoldier += 10;
            Manager.manager.UpdateresourcesGame();
        }

        foreach (var item in trees)
        {
            Destroy(item);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Plane")
        {
            if (other.GetComponent<Unit>())
            {
                if (other.GetComponent<Unit>()._property._name != "Tree")
                {
                    colliders.Add(other);
                }
                else
                {
                    if (!trees.Contains(other.gameObject))
                    {
                        trees.Add(other.gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (colliders.Contains(other))
        {
            colliders.Remove(other);
        }

        if (trees.Contains(other.gameObject))
        {
            trees.Remove(other.gameObject);
        }
    }

    public bool OnClickBuild()
    {
        if (Vector3.Distance(transform.position, Manager.manager.cityHallConstruct.transform.position) > 30)
        {
            Manager.manager.CreateSlotNoti("Quá xa nhà chính");
            return false;
        }

        if (transform.position.y > .5f)
        {
            Manager.manager.CreateSlotNoti("Không thể xây trên địa hình này");
            return false;
        }

        return true;
    }
}
