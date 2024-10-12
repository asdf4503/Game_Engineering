using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneMove : MonoBehaviour
{
    public GameObject startPagePanel; // 시작 페이지 패널
    public GameObject explainPagePanel;
    public Button exitButton; // 게임 종료 버튼

    public void GameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExplainPanel()
    {
        startPagePanel.SetActive(false); // 시작 페이지 패널 비활성화
        explainPagePanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit(); // 게임 종료
    }

    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
