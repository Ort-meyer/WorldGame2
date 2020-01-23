using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DemoAi : MonoBehaviour
{
    public Transform m_convoyMoveTo;

    private Player m_player;

    private WorldManager m_worldManager;

    private float m_timeUntilConvoyForm = 1;
    private float m_timeUntilMoveout = 5;

    private enum AIState { idle, convoyMoving };
    private AIState m_aiState = AIState.idle;

    // Use this for initialization
    void Start()
    {
        m_player = GetComponent<Player>();
        m_worldManager = FindObjectOfType<WorldManager>();

        Invoke("M_FormConvoy", m_timeUntilConvoyForm);
        Invoke("M_MoveOut", m_timeUntilMoveout);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void M_FormConvoy()
    {
        m_player.M_SelectConvoys(m_player.m_ownedConvoys.Values.ToList());
        m_player.M_FormConvoy();
    }

    private void M_MoveOut()
    {
        m_player.M_MoveSelectedConvoys(m_convoyMoveTo.position);
    }

    private void M_SplitToEngage()
    {

    }
}
