using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button m_startGameButton;
    public Button m_quitButton;

    // Use this for initialization
    void Start()
    {
        m_startGameButton.onClick.AddListener(delegate { M_StartGameButton(); });
        m_quitButton.onClick.AddListener(delegate { M_QuitButton(); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void M_StartGameButton()
    {
        SceneManager.LoadScene("MainGame");
    }

    void M_QuitButton()
    {
        Application.Quit();
    }
}
