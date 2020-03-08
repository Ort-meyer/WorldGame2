using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class PlayerSentence
{
    public bool m_active;
    string m_text;
    List<Sentence> m_conversation;
    public Sentence m_sentence;
    public PlayerSentence(string text, bool active, List<Sentence> conversation)
    {
        m_active = active;
        m_text = text;
        m_conversation = conversation;
        m_sentence = new Sentence(text, 0);
    }
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
        m_text = "";
        m_portraitIndex = portraitIndex;
        UpdateText(text);
    }

    public void UpdateText(string newText)
    {
        m_text = newText;
        m_numRows = newText.Length / m_charsPerRow + 1;
    }
}

// TODO move to its own file
public class Character
{
    public List<PlayerSentence> m_playerSentences = new List<PlayerSentence>();
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

    Queue<Sentence> m_activeSentences = new Queue<Sentence>();

    private ResourceManager m_resourceManager;

    // This dude is just to test the system
    Character m_DEBUGcharacter;

    private int DEBUGindex = 0;


    private int m_numActivePlayerSentences = 0;

    // Use this for initialization
    void Start()
    {

        for (int i = 10; i > 0; i++)
        {
            Debug.Log(i);
        }

        m_resourceManager = FindObjectOfType<ResourceManager>();

        m_DEBUGcharacter = new Character();
        m_DEBUGcharacter.m_playerSentences = new List<PlayerSentence>()
        {
            new PlayerSentence("Hello", true, new List<Sentence>()
            {
                new Sentence("Hello", 0),
                new Sentence("Hi there!", 1),
            }),
            new PlayerSentence("Are you OK?", false, new List<Sentence>()
            {
                new Sentence("Are you OK", 0),
                new Sentence("No man, shit's bad. Can you help me? Some god damn dudes took some shit" +
                "and this is just yet another long ass sentence to test the system. But that should be it", 1),
                new Sentence("Maybe it's better to just hve multiple sentences instead of one big? I dunno", 1),
            }),
            new PlayerSentence("Sure, I'll help out", false, new List<Sentence>()
            {
                new Sentence("Sure, I'll help out", 0),
                new Sentence("Thanks buddy! Go kick some ass", 1),
            }),
            new PlayerSentence("Got to go!", false, new List<Sentence>()
            {
                new Sentence("Got to go!", 0),
                new Sentence("OK, cya boyo!", 1),
            }),
        };

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.P))
        {
            //// Obviously very debuggy
            //Sentence[] sentences =
            //{
            //    // 1 is npc, 0 is player
            //    new Sentence("Hey, how are you?", 1),
            //    new Sentence("Fine, just fine", 0),
            //    new Sentence("Good to hear! Now some really long text to test multiple lines. " +
            //                 "Now some really long text to test multiple lines. " +
            //                 "Now some really long text to test multiple lines.", 1),
            //    new Sentence("That's good to know!", 0),
            //    new Sentence("Now Im gonna say something really long!" +
            //                 "Now Im gonna say something really long!" +
            //                 "Now Im gonna say something really long!", 0),
            //                    // 1 is npc, 0 is player
            //    new Sentence("Hey, how are you?", 1),
            //    new Sentence("Fine, just fine", 0),
            //    new Sentence("Good to hear! Now some really long text to test multiple lines. " +
            //                 "Now some really long text to test multiple lines. " +
            //                 "Now some really long text to test multiple lines.", 1),
            //    new Sentence("That's good to know!", 0),
            //    new Sentence("Now Im gonna say something really long!" +
            //                 "Now Im gonna say something really long!" +
            //                 "Now Im gonna say something really long!", 0),
            //};
            //M_PutSentence(sentences[DEBUGindex]);
            M_PutPlayerSentences();
            M_UpdateSentencePositions();
            DEBUGindex++;
        }
    }

    private GameObject M_PutSentence(Sentence sentence, GameObject sentencePrefab)
    {
        GameObject newSentenceObj = Instantiate(sentencePrefab, m_conversationStartAnchor);
        newSentenceObj.GetComponentInChildren<Image>().sprite = m_resourceManager.M_GetPortrait(sentence.m_portraitIndex);
        newSentenceObj.GetComponent<Text>().text = sentence.m_text;

        // Put player portrait on left side
        if (sentence.m_portraitIndex == 0 || sentence.m_portraitIndex == 3)
        {
            Vector3 currentPos = newSentenceObj.GetComponentInChildren<Image>().rectTransform.localPosition;
            newSentenceObj.GetComponentInChildren<Image>().rectTransform.localPosition = new Vector3(-1 * currentPos.x, currentPos.y, currentPos.z);
        }
        // If there's only one row, put it right aligned (only npc portraits)
        if (sentence.m_numRows == 1 && (sentence.m_portraitIndex != 0 || sentence.m_portraitIndex == 3))
        {
            newSentenceObj.GetComponent<Text>().alignment = TextAnchor.UpperRight;
        }

        sentence.m_sentenceObj = newSentenceObj;
        m_activeSentences.Enqueue(sentence);
        return newSentenceObj;
    }

    private void M_PutPlayerSentences()
    {
        int sentenceIndex = 1;
        foreach (PlayerSentence playerSentence in m_DEBUGcharacter.m_playerSentences)
        {
            if (playerSentence.m_active)
            {
                playerSentence.m_sentence.UpdateText(sentenceIndex.ToString() + ": " + playerSentence.m_sentence.m_text);
                sentenceIndex ++;
                GameObject playerSentenceObj = M_PutSentence(playerSentence.m_sentence, m_playerSentencePrefab);
                playerSentenceObj.GetComponent<Button>().onClick.AddListener(delegate { M_PlayerSentenceOnClick(playerSentence.m_sentence.m_text); });
            }
        }
        m_numActivePlayerSentences = sentenceIndex;
    }

    public void M_PlayerSentenceOnClick(string sentenceString)
    {
        // Remove all player sentence objects
        for (int i = m_activeSentences.Count; i > 0; i++)
        {

        }

        // Convert queue to array, remove last few sentences, then back to queue
        m_activeSentences = new Queue<Sentence>(m_activeSentences.ToArray().Skip(m_activeSentences.Count - m_numActivePlayerSentences));
        


        M_UpdateSentencePositions();
    }
    
    // Updates the positions of all sentences. TODO consider refactoring to always clean and add sentence objects
    private void M_UpdateSentencePositions()
    {
        // Iterate from back of queue and put most recent messages first. If full, remove from queue
        int currentRow = 0;
        //Sentence[] sentences = new Sentence[m_activeSentenceObjs.Count];
        Sentence[] sentences = m_activeSentences.ToArray();

        float previousYPos = 0;

        for (int i = m_activeSentences.Count; i > 0; i--)
        {
            Sentence currentSentence = sentences[i - 1];
            currentRow += currentSentence.m_numRows;
            // Full, remove from queue
            if (currentRow > m_totalNumRowsInWindow)
            {
                Destroy(m_activeSentences.Dequeue().m_sentenceObj);
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
