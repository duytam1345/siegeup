using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeUnit : Unit
{
    public int maxPeasant;
    public int curPeasant;

    public int leftWood;

    public void OnDecreamentWood()
    {
        leftWood--;
        if (leftWood <= 0)
        {
            DestroyTree();
        }
    }

    public void DestroyTree()
    {
        //Hồi phục cây
        Manager.manager.RespawnTree(transform.position);

        //Tạo cây ngã
        GameObject g = Instantiate(Resources.Load("Tree Fall") as GameObject, transform.position, Quaternion.identity);
        Destroy(g, 5f);
        Vector3 v = new Vector3();
        while (Mathf.Abs(v.x - 0) < .5f)
        {
            v.x = Random.Range(-1f, 1f);
        }
        while (Mathf.Abs(v.z - 0) < .5f)
        {
            v.z = Random.Range(-1f, 1f);
        }

        g.GetComponent<Rigidbody>().AddForce(v * 200);

        Destroy(gameObject);
    }
}
