using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject pauseButton;
    PlayerCollision playerCollision;

    public static UiManager uiManager;

    void Start()
    {
        uiManager = this;
        playerCollision = FindFirstObjectByType<PlayerCollision>();
    }

    public void OnPause()
    {
        Time.timeScale = 0f;
        menu.SetActive(true);
        continueButton.SetActive(true);
    }

    public void OnContinue()
    {
        Time.timeScale = 1f;
        menu.SetActive(false);
        continueButton.SetActive(false);
    }

    public void OnPowerActivator()
    {
        if(playerCollision == null) return;
        playerCollision.OnPowerPressed();
    }

    public void Dead()
    {
        pauseButton.SetActive(false);
        menu.SetActive(true);
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        pauseButton.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnExitButton()
    {
       Application.Quit();
    }
}
