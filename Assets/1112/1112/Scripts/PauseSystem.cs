using UnityEngine;
using SupanthaPaul;
public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    void Start()
    {
        // 3. 게임 시작 시에는 항상 메뉴가 꺼져있도록 함
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
        if (InputSystem.Pause())
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // UI 숨기기
        }
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("게임 재개");
    }

    void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("게임 일시 정지");
    }
}