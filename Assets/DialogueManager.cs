using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class PlayerSentence
{
    public bool m_active;
    string m_text;
    public List<Sentence> m_conversation;
    public Sentence m_sentence;

    //////////////////////////////////// BIG TODO ////////////////////////////////////
    // * always disable own index
    // * something off with player sentence y positions (I redid something with ordinary, look at that)
    // * indexing of responses should be reapplied on every presentation of player alternatives
    public List<int> m_enableIndices = new List<int>();
    public List<int> m_disableIndices = new List<int>();
    public PlayerSentence(string text, bool active, List<Sentence> conversation, List<int> enableIndices, List<int> disableIndices)
    {
        m_active = active;
        m_text = text;
        m_conversation = conversation;
        m_sentence = new Sentence(text, 0);
        m_enableIndices = enableIndices;
        m_disableIndices = disableIndices;
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
    private ResourceManager m_resourceManager;



    Queue<Sentence> m_allActiveSentences = new Queue<Sentence>();
    List<GameObject> m_allActiveSentenceObjs = new List<GameObject>();


    private PlayerSentence m_currentPlayerSentence;
    private int m_nextConversationSentence;


    // This dude is just to test the system
    Character m_DEBUGcharacter;

    public GameObject m_BUILDDEBUGOBJECT;

    // Use this for initialization
    void Start()
    {

        m_resourceManager = FindObjectOfType<ResourceManager>();
        //M_CreateSimpleDialogue();
        M_CreateTimbaDialogue();
    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            // StartConversation
            M_PlaceAllSentences();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (m_currentPlayerSentence != null)
            {
                M_ProgressConversation();
            }
        }
    }


    // Places all currently active sentences on the UI
    private void M_PlaceAllSentences()
    {
        // Always destroy and recreate all UI sentence objs
        M_ClearAllSentences();
        int currentRow = M_PlaceAllActivePlayerSentences();
        M_PlaceAllActiveSentences(currentRow);
    }

    private void M_ClearAllSentences()
    {
        // Always destroy and recreate all UI sentence objs
        foreach (GameObject sentenceObj in m_allActiveSentenceObjs)
        {
            Destroy(sentenceObj);
        }
        m_allActiveSentenceObjs.Clear();
    }

    // Places all player sentences of active character that is enabled
    private int M_PlaceAllActivePlayerSentences()
    {
        int currentRow = 0;
        // First place all player sentences, if any
        for (int i = m_DEBUGcharacter.m_playerSentences.Count - 1; i >= 0; i--)
        {
            PlayerSentence ps = m_DEBUGcharacter.m_playerSentences[i];
            if (ps.m_active)
            {
                GameObject psObj = Instantiate(m_playerSentencePrefab, m_conversationStartAnchor);
                currentRow += ps.m_sentence.m_numRows;
                psObj.GetComponent<RectTransform>().localPosition = new Vector3(0, (currentRow + 1) * m_rowHeight, 0);
                m_allActiveSentenceObjs.Add(psObj);
                psObj.GetComponent<Text>().text = (i + 1).ToString() + ": " + ps.m_sentence.m_text;
                psObj.GetComponent<Button>().onClick.AddListener(delegate { M_PlayerSentenceOnClick(ps); });

                // Just some security that we don't overflow. If it happens, it's a writing miss (for now)
                if (currentRow > m_totalNumRowsInWindow)
                {
                    Debug.LogError("Too many player sentences in dialogue");
                }
            }
        }
        return currentRow;
    }

    public void M_PlayerSentenceOnClick(PlayerSentence playerSentenceClicked)
    {
        // Set current conversation (it is derived)
        m_currentPlayerSentence = playerSentenceClicked;
        // Set conversation to be at start
        m_nextConversationSentence = 0;
        // Progress first sentence (typically identical to the player sentence text)
        M_ProgressConversation();
    }

    // Places all currently active sentinces, starting at startRow
    private void M_PlaceAllActiveSentences(int currentRow = 0)
    {
        Sentence[] sentences = m_allActiveSentences.ToArray();
        for (int i = m_allActiveSentences.Count - 1; i >= 0; i--)
        {
            Sentence currentSentence = sentences[i];
            currentRow += currentSentence.m_numRows + 1;
            // Full, remove from queue
            if (currentRow > m_totalNumRowsInWindow)
            {
                m_allActiveSentences.Dequeue();
            }
            // Place this sentence
            else
            {
                GameObject sObj = Instantiate(m_sentencePrefab, m_conversationStartAnchor);
                sObj.GetComponent<RectTransform>().localPosition = new Vector3(0, (currentRow + 1) * m_rowHeight, 0);
                Image portrait = sObj.GetComponentInChildren<Image>();
                portrait.sprite = m_resourceManager.M_GetPortrait(currentSentence.m_portraitIndex);

                sObj.GetComponent<Text>().text = currentSentence.m_text;
                m_allActiveSentenceObjs.Add(sObj);
                // Put player portrait on left side
                if (currentSentence.m_portraitIndex == 0 || currentSentence.m_portraitIndex == 3 || currentSentence.m_portraitIndex == 1)
                {
                    Vector3 portraitPos = portrait.rectTransform.localPosition;
                    portrait.rectTransform.localPosition = new Vector3(-1 * portraitPos.x, portraitPos.y, portraitPos.z);
                }
                // If there's only one row, put it right aligned (only npc portraits)
                if (currentSentence.m_numRows == 1 && currentSentence.m_portraitIndex != 0 && currentSentence.m_portraitIndex != 3 && currentSentence.m_portraitIndex != 1)
                {
                    sObj.GetComponent<Text>().alignment = TextAnchor.UpperRight;
                }
            }
        }
    }


    public void M_ProgressConversation()
    {
        // This if case should probably not be here
        if (m_nextConversationSentence < m_currentPlayerSentence.m_conversation.Count)
        {
            m_allActiveSentences.Enqueue(m_currentPlayerSentence.m_conversation[m_nextConversationSentence]);
            m_nextConversationSentence++;
            M_ClearAllSentences();
            M_PlaceAllActiveSentences();
        }
        else
        {
            // End of conversation, present new player options

            // Enable options
            foreach (int i in m_currentPlayerSentence.m_enableIndices)
            {
                m_DEBUGcharacter.m_playerSentences[i].m_active = true;
            }
            // Disable options
            foreach (int i in m_currentPlayerSentence.m_disableIndices)
            {
                m_DEBUGcharacter.m_playerSentences[i].m_active = false;
            }
            M_PlaceAllSentences();
        }
    }



    private void M_CreateSimpleDialogue()
    {
        m_DEBUGcharacter = new Character();
        m_DEBUGcharacter.m_playerSentences = new List<PlayerSentence>()
        {
            //0
            new PlayerSentence("Hello", true, new List<Sentence>()
            {
                new Sentence("Hello", 0),
                new Sentence("Hi there!", 1),
            },
            new List<int>(){1, 3},
            new List<int>(){0}),
            //1
            new PlayerSentence("Are you OK?", false, new List<Sentence>()
            {
                new Sentence("Are you OK", 0),
                new Sentence("No man, shit's bad. Can you help me? Some god damn dudes took some shit" +
                "and this is just yet another long ass sentence to test the system. But that should be it", 1),
                new Sentence("Maybe it's better to just hve multiple sentences instead of one big? I dunno", 1),
            },
            //2
            new List<int>(){2},
            new List<int>(){0, 1, 2, 3}),// End convo
            new PlayerSentence("Sure, I'll help out", false, new List<Sentence>()
            {
                new Sentence("Sure, I'll help out", 0),
                new Sentence("Thanks buddy! Go kick some ass", 1),
            },
            //3
            new List<int>(){ },
            new List<int>(){2}),
            new PlayerSentence("Got to go!", false, new List<Sentence>()
            {
                new Sentence("Got to go!", 0),
                new Sentence("OK, cya boyo!", 1),
            },
            new List<int>(){ },
            new List<int>(){0, 1, 2, 3}),// End convo
        };
    }

    
    private void M_CreateTimbaDialogue()
    {
        // Just to make coding the dialogue easier
        int protag = 0;
        int dick = 1;
        int quest = 2;
        int empty = 3;

        m_DEBUGcharacter = new Character();
        m_DEBUGcharacter.m_playerSentences = new List<PlayerSentence>()
        {
            // 0
            new PlayerSentence("You have to click here to start the conversation (I'll fix this soon).", true, new List<Sentence>()
            {
                new Sentence("Are you the one who posted the quest about the missing shipment of coal?", protag),
                new Sentence("Yes, yes I am. And first of all it's not missing it 's STOLEN, ok?", quest),
                new Sentence("Stolen?", dick),
                new Sentence("Ahuh, i am absolutly sure it was stolen by those thieving basterds over at the docks, you know the types?", quest),
                new Sentence("the usual hoodlums just looking for some quick cash?", dick),
                new Sentence("Nonono these aint your run of the mill hoodlums, these are foreign types. Queer looking folk... not from around here.", quest),
            },
            new List<int>(){1, 2},
            new List<int>(){0}),
            // 1
            new PlayerSentence("What proof do you have that they are the ones who stole the shipment?", false, new List<Sentence>()
            {
                new Sentence("What proof do you have that they are the ones who stole the shipment?", protag),
                new Sentence("What do you mean proof? If a shepard notices that a sheep has been ripped apart he knows it's the wolves ser.", quest),
                new Sentence("Well what says it wasn't a bear or even a cougar then, good sir?", protag),
                new Sentence("Or that the sheep might have just got tired of living?", dick),
                new Sentence("Well never mind the bloody sheep then! Look I am almost certain these types have my shipment, is a feeling is all. in my gut...", quest),
                new Sentence("And i might have a feeling that this is a waste of time when all you have are baseless accusations at the moment.", protag),
                new Sentence("Now now lets here the man's story, when was the shipment stolen?", protag),
            },
            new List<int>(){2},
            new List<int>(){1}),
            // 2
            new PlayerSentence("Ok... so when and where was this shipment stolen", false, new List<Sentence>()
            {
                new Sentence("Ok... so when and where was this shipment stolen", protag),
                new Sentence("Well that's the thing: i have a boy, a simpleton, who guards my warehouse gate for me. now he aint the smartest but he has a good heart and " +
                "can't tell a lie even if his life depended on it.Now he swears that no one was in or out the warehouse gate the night of the theft and there is no sign of " +
                "a break in on the backdoor.Yet my shipment is missing.Now tell me then who else could'ave done it but those foreign folk and their queer magicks?", quest),
                new Sentence("And are there any other suspects other then these... queer folk as you put it?", protag),
                new Sentence("Look I can assure you there is no other!", quest),
            },
            new List<int>(){3, 4},
            new List<int>(){2}),
            // 3
            new PlayerSentence("(press the dude on why he so certain)(fucking speech check 50)", false, new List<Sentence>()
            {
                new Sentence("And yet with no witnesses, no physical proof whatsoever you are certain.", protag),
                new Sentence("I agree you certainty is quite...certainnn", dick),
                new Sentence("alright alright, the shipment of coal was meant for these foreigners but it would seem they might have not have seen my pricing as...fair and I " +
                "am certain they chose to steal it instead. No honor at all them folk for a deal done.", quest),
            },
            new List<int>(){4, 5},
            new List<int>(){3}),
            // 4
            new PlayerSentence("(list other possible suspects on top of head)", false, new List<Sentence>()
            {
                new Sentence("Well IM certain that there are many others that might be suspected for a crime like this but since moving such a large shipment takes quite the " +
                "coordination of a larger team then I would say it is most likely a rival company or any other larger gang in this city looking to ship this off as their own and " +
                "get paid.", protag),
                new Sentence(".....", quest),
                new Sentence("Well either way we should start this investigation if we are to help this man, and to be frank there is still the talk of a reward to be had", dick),
            },
            new List<int>(){5, 6},
            new List<int>(){4}),
            // 5
            new PlayerSentence("(accuse the dood)", false, new List<Sentence>()
            {
                new Sentence("Im sorry but it is starting to sound like you might already know where your shipment might be and that it would be a waste of our time and resources " +
                "to try and help you.", dick),
                new Sentence("Well if I knew I wouldn't have bothered with a quest I would've gone to the law enforcement now wouldn't I?", quest),
                new Sentence("(intellisense 3000)", empty),
                new Sentence("You would, wouldn't you?", protag),
                new Sentence("A..ayee, what nonsense is this?", quest),
            },
            new List<int>(){6},
            new List<int>(){5}),
            // 6
            new PlayerSentence("Hm then may we check this warehouse and talk with this boy?", false, new List<Sentence>()
            {
                new Sentence("Hm then may we check this warehouse and talk with this boy?", protag),
                new Sentence("Before that though there is the question of the reward!", dick),
                new Sentence("Whatofit?", quest),
                new Sentence("HEJ TIMBA", protag),
                new Sentence("Well of what you say of foreign magicks and all then this sounds quite dangerous and that would mean a harm of not just the body is possible but also " +
                "the mind.", dick),
                new Sentence("The mind?", quest),
                new Sentence("You see my mind would be quite hurt by wondering and regretting of why we didn't ask for more coin when clearly it wasn't worth the measly sum offered at " +
                "first. Now luckily this can already be remedied by you even before the harm is done.", dick),
            },
            new List<int>(){},
            new List<int>(){6}),

        };
    }

    // Second, place the entire "log" of current sentences


    //// Then place all current responses
    //Sentence[] sentences = m_activeSentences.ToArray();



    //// Iterate from back of queue and put most recent messages first. If full, remove from queue
    //for (int i = m_activeSentences.Count - 1; i >= 0; i--)
    //{
    //    Sentence currentSentence = sentences[i];
    //    currentRow += currentSentence.m_numRows;
    //    // Full, remove from queue
    //    if (currentRow > m_totalNumRowsInWindow)
    //    {
    //        m_activeSentences.Dequeue();
    //    }
    //    // Print this sentence
    //    else
    //    {
    //        Vector3 sentencePos = new Vector3(0, previousYPos + (currentSentence.m_numRows + 1) * m_rowHeight, 0);
    //        currentSentence.m_sentenceObj.GetComponent<RectTransform>().localPosition = sentencePos;
    //        previousYPos = sentencePos.y;
    //    }
}



//private GameObject M_PutSentence(Sentence sentence, GameObject sentencePrefab)
//{
//    GameObject newSentenceObj = Instantiate(sentencePrefab, m_conversationStartAnchor);
//    newSentenceObj.GetComponentInChildren<Image>().sprite = m_resourceManager.M_GetPortrait(sentence.m_portraitIndex);
//    newSentenceObj.GetComponent<Text>().text = sentence.m_text;

//    // Put player portrait on left side
//    if (sentence.m_portraitIndex == 0 || sentence.m_portraitIndex == 3)
//    {
//        Vector3 currentPos = newSentenceObj.GetComponentInChildren<Image>().rectTransform.localPosition;
//        newSentenceObj.GetComponentInChildren<Image>().rectTransform.localPosition = new Vector3(-1 * currentPos.x, currentPos.y, currentPos.z);
//    }
//    // If there's only one row, put it right aligned (only npc portraits)
//    if (sentence.m_numRows == 1 && (sentence.m_portraitIndex != 0 || sentence.m_portraitIndex == 3))
//    {
//        newSentenceObj.GetComponent<Text>().alignment = TextAnchor.UpperRight;
//    }

//    sentence.m_sentenceObj = newSentenceObj;
//    m_activeSentences.Enqueue(sentence);
//    return newSentenceObj;
//}

//private void M_PutPlayerSentences()
//{
//    int nextrSentenceIndex = 1;
//    foreach (PlayerSentence playerSentence in m_DEBUGcharacter.m_playerSentences)
//    {
//        if (playerSentence.m_active)
//        {
//            playerSentence.m_sentence.UpdateText(nextrSentenceIndex.ToString() + ": " + playerSentence.m_sentence.m_text);
//            nextrSentenceIndex++;
//            GameObject playerSentenceObj = M_PutSentence(playerSentence.m_sentence, m_playerSentencePrefab);
//            playerSentenceObj.GetComponent<Button>().onClick.AddListener(delegate { M_PlayerSentenceOnClick(playerSentence); });
//        }
//    }
//    m_numActivePlayerSentences = nextrSentenceIndex - 1;
//    M_UpdateSentencePositions();
//}

//public void M_PlayerSentenceOnClick(PlayerSentence playerSentenceClicked)
//{
//    Sentence[] sentenceArray = m_activeSentences.ToArray();
//    // Remove all player sentence objects (the last few sentences in the queue)
//    for (int i = m_activeSentences.Count - 1; i >= 0; i--)
//    {
//        Destroy(sentenceArray[i].m_sentenceObj);
//    }

//    // Convert queue to array, remove last few sentences, then back to queue
//    m_activeSentences = new Queue<Sentence>(sentenceArray.Skip(m_activeSentences.Count - m_numActivePlayerSentences));

//    // Now start the conversation from this player sentence
//    m_currentPlayerSentence = playerSentenceClicked;
//    m_currentConversationIndex = 0;
//    M_ProgressConversation();
//}

//private void M_ProgressConversation()
//{
//    M_PutSentence(m_currentPlayerSentence.m_conversation[m_currentConversationIndex], m_sentencePrefab);
//    m_currentConversationIndex++;
//    M_UpdateSentencePositions();
//    // Check if conversation is done
//    if (m_currentConversationIndex > m_currentPlayerSentence.m_conversation.Count)
//    {
//        foreach (int index in m_currentPlayerSentence.m_enableIndices)
//        {
//            m_DEBUGcharacter.m_playerSentences[index].m_active = true;
//        }
//        foreach (int index in m_currentPlayerSentence.m_disableIndices)
//        {
//            m_DEBUGcharacter.m_playerSentences[index].m_active = false;
//        }
//    }
//    M_PutPlayerSentences();
//}


//private void M_UpdateSentencePositions()
//{
//    // Iterate from back of queue and put most recent messages first. If full, remove from queue
//    int currentRow = 0;
//    //Sentence[] sentences = new Sentence[m_activeSentenceObjs.Count];
//    Sentence[] sentences = m_activeSentences.ToArray();

//    float previousYPos = 0;

//    for (int i = m_activeSentences.Count; i > 0; i--)
//    {
//        Sentence currentSentence = sentences[i - 1];
//        currentRow += currentSentence.m_numRows;
//        // Full, remove from queue
//        if (currentRow > m_totalNumRowsInWindow)
//        {
//            Destroy(m_activeSentences.Dequeue().m_sentenceObj);
//        }
//        // Print this sentence
//        else
//        {
//            Vector3 sentencePos = new Vector3(0, previousYPos + (currentSentence.m_numRows + 1) * m_rowHeight, 0);
//            currentSentence.m_sentenceObj.GetComponent<RectTransform>().localPosition = sentencePos;
//            previousYPos = sentencePos.y;
//        }
//    }
//}
//}