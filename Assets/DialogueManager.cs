using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Conversation
{

}

// TODO move to its own file (or same as other rpg stuff?)
public class Sentence
{
    public int m_portraitIndex;
    public int m_numRows;
    public string m_text;
    // The in game object of this. Probably not a good way of tracking this
    public GameObject m_sentenceObj = null;
    // How many characters before a new row
    const int m_charsPerRow = 50; // TODO this probably isn't the best place to keep this
    public Sentence(string text, int portraitIndex)
    {
        m_portraitIndex = portraitIndex;
        m_text = text;
        m_numRows = text.Length / m_charsPerRow + 1;
    }
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


public class DialogueManager : MonoBehaviour
{
    public GameObject m_dialoguePanel;
    public GameObject m_sentencesPanel;

    public RectTransform m_conversationStartAnchor;
    // Height of each row in pixels
    public float m_rowHeight;


    public GameObject m_sentencePrefab;
    public GameObject m_playerSentencePrefab;

    // How many tows can exist at any one time. If this is exceeded, the first displayed sentence is removed
    public int m_totalNumRowsInWindow = 10;

    Queue<Sentence> m_activeSentenceObjs = new Queue<Sentence>();

    private ResourceManager m_resourceManager;


    private int DEBUGindex = 0;

    // Use this for initialization
    void Start()
    {
        m_resourceManager = FindObjectOfType<ResourceManager>();

        //for (int i = 0; i < texts.Length; i++)
        //{
        //GameObject newText = Instantiate(m_dialoguePrefab, m_dialoguePanel.GetComponent<RectTransform>());
        //newText.GetComponent<RectTransform>().localPosition = m_DialogueOffset + new Vector2(0, -m_spaceBetweenDialogues * i);
        //newText.GetComponent<Text>().text = texts[i];
        //}
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.P))
        {
            // Obviously very debuggy
            Sentence[] sentences =
            {
                // 1 is npc, 0 is player
                new Sentence("Hey, how are you?", 1),
                new Sentence("Fine, just fine", 0),
                new Sentence("Good to hear! Now some really long text to test multiple lines. " +
                             "Now some really long text to test multiple lines. " +
                             "Now some really long text to test multiple lines.", 1),
                new Sentence("That's good to know!", 0),
                new Sentence("Now Im gonna say something really long!" +
                             "Now Im gonna say something really long!" +
                             "Now Im gonna say something really long!", 0),
                                // 1 is npc, 0 is player
                new Sentence("Hey, how are you?", 1),
                new Sentence("Fine, just fine", 0),
                new Sentence("Good to hear! Now some really long text to test multiple lines. " +
                             "Now some really long text to test multiple lines. " +
                             "Now some really long text to test multiple lines.", 1),
                new Sentence("That's good to know!", 0),
                new Sentence("Now Im gonna say something really long!" +
                             "Now Im gonna say something really long!" +
                             "Now Im gonna say something really long!", 0),
            };
            M_PutSentence(sentences[DEBUGindex]);
            M_UpdateSentencePositions();
            DEBUGindex++;
        }
    }

    private void M_PutSentence(Sentence sentence)
    {
        GameObject newSentenceObj = Instantiate(m_sentencePrefab, m_conversationStartAnchor);
        newSentenceObj.GetComponentInChildren<Image>().sprite = m_resourceManager.M_GetPortrait(sentence.m_portraitIndex);
        newSentenceObj.GetComponent<Text>().text = sentence.m_text;

        // Put player portrait on left side
        if (sentence.m_portraitIndex == 0)
        {
            Vector3 currentPos = newSentenceObj.GetComponentInChildren<Image>().rectTransform.localPosition;
            newSentenceObj.GetComponentInChildren<Image>().rectTransform.localPosition = new Vector3(-1 * currentPos.x, currentPos.y, currentPos.z);
        }
        // If there's only one row, put it right aligned (only npc portraits)
        if (sentence.m_numRows == 1 && sentence.m_portraitIndex !=0)
        {
            newSentenceObj.GetComponent<Text>().alignment = TextAnchor.UpperRight;
        }
        
        sentence.m_sentenceObj = newSentenceObj;
        m_activeSentenceObjs.Enqueue(sentence);

    }

    private void M_UpdateSentencePositions()
    {
        // Iterate from back of queue and put most recent messages first. If full, remove from queue
        int currentRow = 0;
        //Sentence[] sentences = new Sentence[m_activeSentenceObjs.Count];
        Sentence[] sentences = m_activeSentenceObjs.ToArray();

        float previousYPos = 0;

        for (int i = m_activeSentenceObjs.Count; i > 0; i--)
        {
            Sentence currentSentence = sentences[i - 1];
            currentRow += currentSentence.m_numRows;
            // Full, remove from queue
            if (currentRow > m_totalNumRowsInWindow)
            {
                Destroy(m_activeSentenceObjs.Dequeue().m_sentenceObj);
            }
            // Print this sentence
            else
            {
                Vector3 sentencePos = new Vector3(0, previousYPos + (currentSentence.m_numRows + 1) * m_rowHeight, 0);
                currentSentence.m_sentenceObj.GetComponent<RectTransform>().localPosition = sentencePos;
                previousYPos = sentencePos.y;
            }
        }
    }
}
