using UnityEngine;

public class MenuManager : MonoBehaviour 
{
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deadMenu;

    private bool isPaused = false;

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !deadMenu.activeSelf) 
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu() 
    {
        isPaused = !isPaused;
        UpdateMenuStates();
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void UpdateMenuStates() 
    {
        gameMenu.SetActive(!isPaused);
        pauseMenu.SetActive(isPaused);
        deadMenu.SetActive(false);
    }

    public void ShowDeathMenu() 
    {
        gameMenu.SetActive(false);
        pauseMenu.SetActive(false);
        deadMenu.SetActive(true);
        Time.timeScale = 0f;
    }
}
