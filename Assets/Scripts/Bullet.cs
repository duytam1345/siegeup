using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 vTarget;
    public Unit unitTarget;

    public float speed;

    public Property property;

    bool b;//Effect

    private void Start()
    {
        vTarget = transform.position + (unitTarget.transform.position - transform.position);
        vTarget += (transform.position - vTarget).normalized * unitTarget.r;
    }

    private void Update()
    {
        if (!b)
        {
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
                StopParticle();
                Destroy(gameObject);
                b = true;
            }
        }
    }

    void StopParticle()
    {
        foreach (Transform item in transform)
        {
            if (item.GetComponent<ParticleSystem>())
            {
                item.GetComponent<ParticleSystem>().Stop();
                return;
            }
        }
    }
}
