using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO maybe not have player hardcoded to 1?
        // TODO maybe use main unit? This would result in any unit entering the settlement to trigger
        if(other.gameObject.GetComponent<Unit>().m_convoy.m_faction == 1)
        {
            // Present list of characters to human. Human picks character to talk to
        }
    }
}
