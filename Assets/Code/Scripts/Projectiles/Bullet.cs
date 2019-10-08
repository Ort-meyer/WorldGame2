using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BaseProjectile
{

    // Use this for initialization
    protected override void Start()
    {

    }

    protected override void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Unit firingUnit = other.gameObject.GetComponentInParent<Unit>();
        if (firingUnit)
        {
            GameObject firingUnitObject = firingUnit.gameObject;
            if (firingUnitObject == m_firingUnitObject)
            {
                return;
            }
        }
        Destroy(this.gameObject);
        // Possibly helpfull code from some example somewhere
        //foreach (ContactPoint contact in collision.contacts)
        //{
        //    Debug.DrawRay(contact.point, contact.normal, Color.white);
        //}
    }
}
