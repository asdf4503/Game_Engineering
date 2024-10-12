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
    public GameObject settingsPanel; // 설정 창 패널
    public GameObject gameOverPanel; // 게임오버 창 패널
    public GameObject gameClearPanel; // 게임클리어 창 패널


    private bool isInvincible = false; // 일정 시간 동안 충돌 무시를 위한 변수

    void Start()
    {
        settingsPanel.SetActive(false); // 시작할 때 설정 패널 비활성화
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
            Debug.Log("게임 클리어");
            gameClearPanel.SetActive(true);
        }


        totalPoint += stagePoint;
        stagePoint = 0;

        // 체력 증가 및 UI 업데이트
        if (health < UIhealth.Length)
        {
            health++;
            UIhealth[health - 1].color = new Color(1, 1, 1, 1); // 기본 색으로 변경
        }
    }

    public void HealthDown()
    {
        if (health > 0)
        {
            health--; // health를 감소시킴
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);

            if (health == 0) // health가 0이면
            {
                UIRestartBtn.SetActive(true);
                gameOverPanel.SetActive(true);
                player.OnDie(); // OnDie 메서드 호출
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
            StartCoroutine(InvincibilityCoroutine(0.5f)); // 0.5초 동안 충돌 무시

            if (health > 0)
            {
                PlayerReposition();
            }

            HealthDown(); // 충돌했을 때 HealthDown 메서드 호출
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
        // 초기화할 상태들을 GameManager에서 처리
        SceneManager.LoadScene("GameScene");

        // 현재 스테이지 패널을 보여줍니다
        ShowCurrentStagePanel();

        // 게임 오버 패널 등 비활성화
        UIRestartBtn.SetActive(false);
        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(false);
        settingsPanel.SetActive(false);

        // UI 요소 활성화
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
        // 모든 스테이지 비활성화
        foreach (var stage in stages)
        {
            stage.SetActive(false);
        }

        // 현재 스테이지 활성화
        stages[stageIndex].SetActive(true);
    }

    void ToggleSettingsPanel()
    {
        bool isActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isActive);

        if (isActive)
        {
            Time.timeScale = 0; // 게임을 일시정지
            // UI 요소 비활성화
            foreach (var health in UIhealth)
            {
                health.gameObject.SetActive(false);
            }
            UIPoint.gameObject.SetActive(false);
            UIStage.gameObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 1; // 게임을 다시 시작
            for (int i = 0; i < UIhealth.Length; i++)
            {
                UIhealth[i].gameObject.SetActive(true);
                UIhealth[i].color = i < health ? new Color(1, 1, 1, 1) : new Color(1, 0, 0, 0.4f);
            }
            UIPoint.gameObject.SetActive(true);
            UIStage.gameObject.SetActive(true);
        }
    }

    // 계속하기 버튼 클릭 시 호출되는 메서드
    public void OnContinueButton()
    {
        ToggleSettingsPanel();
    }

    public void MainHome()
    {
        SceneManager.LoadScene("MainScene");
    }
}
