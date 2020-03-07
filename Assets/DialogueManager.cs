using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject m_dialoguePanel;
    public GameObject m_sentencesPanel;

    public Vector2 m_DialogueOffset = new Vector2(-200, 0);
    public float m_spaceBetweenDialogues = 100;
    public Vector2 m_sentencesOffset = new Vector2(0, 0);

    public GameObject m_sentencePrefab;
    

    // Use this for initialization
    void Start()
    {

        string[] texts = { "Hello, how are you?", "Fine, just fine. You?", "Die motherfucker" };
        for (int i = 0; i < texts.Length; i++)
        {
            //GameObject newText = Instantiate(m_dialoguePrefab, m_dialoguePanel.GetComponent<RectTransform>());
            //newText.GetComponent<RectTransform>().localPosition = m_DialogueOffset + new Vector2(0, -m_spaceBetweenDialogues * i);
            //newText.GetComponent<Text>().text = texts[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {

            ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
            GameObject newSentenceObj = Instantiate(m_sentencePrefab, m_dialoguePanel.GetComponent<RectTransform>());
            newSentenceObj.GetComponent<RectTransform>().localPosition = new Vector3(300, 300);
            newSentenceObj.GetComponentInChildren<Image>().sprite = resourceManager.M_GetPortrait(0);
            newSentenceObj.GetComponent<Text>().text = "Hello World!";

        }
    }
}
