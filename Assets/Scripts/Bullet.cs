using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 vTarget;
    public Unit unitTarget;

    public float speed;

    public Property property;

    private void Update()
    {
        if (unitTarget)
        {
            vTarget = unitTarget.transform.position;
        }

        if (Vector3.Distance(transform.position, vTarget) > .1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, vTarget, speed * Time.deltaTime);
        }
        else
        {
            if (unitTarget)
            {
                unitTarget.TakeDamage(property);
            }
            Destroy(gameObject);
        }
    }
}
