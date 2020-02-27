using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO move to its own file (or same as other rpg stuff?)
public class Sentence
{

}

// TODO move to its own file
public class Character
{
    List<Sentence> m_followUpSentences = new List<Sentence>();
    public Character()
    {
        
    }

    // Talks to the 
    public void M_Talk(int dialogueOptionIndex)
    {

    }

    public void M_StartConversation()
    {

    }
}

public class Settlement : MonoBehaviour
{
    // List of characters that live in this settlement
    public List<Character> m_characters;
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
