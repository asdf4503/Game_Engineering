using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneMove : MonoBehaviour
{
    public GameObject startPagePanel; // ���� ������ �г�
    public GameObject explainPagePanel;
    public Button exitButton; // ���� ���� ��ư

    public void GameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExplainPanel()
    {
        startPagePanel.SetActive(false); // ���� ������ �г� ��Ȱ��ȭ
        explainPagePanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit(); // ���� ����
    }

    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
