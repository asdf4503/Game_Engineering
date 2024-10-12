using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;
    public GameObject settingsPanel; // ���� â �г�
    public GameObject gameOverPanel; // ���ӿ��� â �г�
    public GameObject gameClearPanel; // ����Ŭ���� â �г�


    private bool isInvincible = false; // ���� �ð� ���� �浹 ���ø� ���� ����

    void Start()
    {
        settingsPanel.SetActive(false); // ������ �� ���� �г� ��Ȱ��ȭ
        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(false);

    }

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsPanel();
        }
    }

    public void NextStage()
    {
        if (stageIndex < stages.Length - 1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            Time.timeScale = 0;
            Debug.Log("���� Ŭ����");
            gameClearPanel.SetActive(true);
        }


        totalPoint += stagePoint;
        stagePoint = 0;

        // ü�� ���� �� UI ������Ʈ
        if (health < UIhealth.Length)
        {
            health++;
            UIhealth[health - 1].color = new Color(1, 1, 1, 1); // �⺻ ������ ����
        }
    }

    public void HealthDown()
    {
        if (health > 0)
        {
            health--; // health�� ���ҽ�Ŵ
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);

            if (health == 0) // health�� 0�̸�
            {
                UIRestartBtn.SetActive(true);
                gameOverPanel.SetActive(true);
                player.OnDie(); // OnDie �޼��� ȣ��
            }
        }
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isInvincible)
        {
            StartCoroutine(InvincibilityCoroutine(0.5f)); // 0.5�� ���� �浹 ����

            if (health > 0)
            {
                PlayerReposition();
            }

            HealthDown(); // �浹���� �� HealthDown �޼��� ȣ��
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-16, 7.5f, -1);
        player.VelocityZero();
    }

    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        PlayerReposition();
        // �ʱ�ȭ�� ���µ��� GameManager���� ó��
        SceneManager.LoadScene("GameScene");

        // ���� �������� �г��� �����ݴϴ�
        ShowCurrentStagePanel();

        // ���� ���� �г� �� ��Ȱ��ȭ
        UIRestartBtn.SetActive(false);
        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(false);
        settingsPanel.SetActive(false);

        // UI ��� Ȱ��ȭ
        for (int i = 0; i < UIhealth.Length; i++)
        {
            UIhealth[i].gameObject.SetActive(true);
            UIhealth[i].color = new Color(1, 1, 1, 1);
        }
        UIPoint.gameObject.SetActive(true);
        UIStage.gameObject.SetActive(true);
    }

    void ShowCurrentStagePanel()
    {
        // ��� �������� ��Ȱ��ȭ
        foreach (var stage in stages)
        {
            stage.SetActive(false);
        }

        // ���� �������� Ȱ��ȭ
        stages[stageIndex].SetActive(true);
    }

    void ToggleSettingsPanel()
    {
        bool isActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isActive);

        if (isActive)
        {
            Time.timeScale = 0; // ������ �Ͻ�����
            // UI ��� ��Ȱ��ȭ
            foreach (var health in UIhealth)
            {
                health.gameObject.SetActive(false);
            }
            UIPoint.gameObject.SetActive(false);
            UIStage.gameObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 1; // ������ �ٽ� ����
            for (int i = 0; i < UIhealth.Length; i++)
            {
                UIhealth[i].gameObject.SetActive(true);
                UIhealth[i].color = i < health ? new Color(1, 1, 1, 1) : new Color(1, 0, 0, 0.4f);
            }
            UIPoint.gameObject.SetActive(true);
            UIStage.gameObject.SetActive(true);
        }
    }

    // ����ϱ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void OnContinueButton()
    {
        ToggleSettingsPanel();
    }

    public void MainHome()
    {
        SceneManager.LoadScene("MainScene");
    }
}
