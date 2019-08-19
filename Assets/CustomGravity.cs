using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    float m_hoverDistance = 1;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray rayDown = new Ray(transform.position, new Vector3(0, -1, 0));
        //RaycastHit hits = Physics.RaycastAll()
        transform.position += new Vector3(0, 9.82f, 0) * Time.deltaTime;
    }
}
