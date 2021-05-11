using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEmmitter : MonoBehaviour
{
    public GameObject projectile;
    private int speed = 10;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GameObject proj = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
            Rigidbody projRB = proj.GetComponent<Rigidbody>();
            projRB.velocity = (transform.forward * speed);
            Destroy(proj, 1.0f);
        }
    }
}
