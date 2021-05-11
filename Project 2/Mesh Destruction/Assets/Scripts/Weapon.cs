using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform barrel;
    public float range = 0f;
    public LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = new Ray(barrel.position, transform.forward);

            if (Physics.Raycast(ray, out hit, range))
            {
                if (hit.collider.tag == "Destructible")
                {
                    MeshDestroy destructible = hit.collider.GetComponent<MeshDestroy>();
                    destructible.DestroyMesh();
                }
            }

            Debug.DrawRay(barrel.position, transform.forward * range, Color.red, 1.0f, true);
        }
    }
}
