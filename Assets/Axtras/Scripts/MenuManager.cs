using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour 
{
    private bool isPaused = false;
    private bool isDead = false;
    private Transform canviiTransform;

    [Header("Canvas Menus")]
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deadMenu;

    [Header("Values")]
    [SerializeField] private float transitionDuration = 0.5f;

    public bool IsPaused {
        get { return isPaused; }
        set { 
            isPaused = value;
            if(isPaused) {
                Image gameMenuTransitionImage = gameMenu.transform.Find("Transition Image").GetComponent<Image>();
                gameMenuTransitionImage
                .DOFade(1f, transitionDuration)
                .OnStart(() => {
                    gameMenuTransitionImage.DOFade(0f, 0f);
                })
                .OnComplete(() => {
                    gameMenu.SetActive(false);
                })
                .SetUpdate(true);

                Image pauseMenuTransitionImage = pauseMenu.transform.Find("Transition Image").GetComponent<Image>();
                pauseMenuTransitionImage
                .DOFade(1f, transitionDuration)
                .OnStart(() => {
                    pauseMenuTransitionImage.DOFade(1f, 0f);
                    pauseMenu.SetActive(true);
                })
                .SetUpdate(true);

                Time.timeScale = 0f;
            }
            else {
                Image pauseMenuTransitionImage = pauseMenu.transform.Find("Transition Image").GetComponent<Image>();
                pauseMenuTransitionImage
                .DOFade(0f, transitionDuration)
                .OnComplete(() => {
                    pauseMenu.SetActive(false);
                })
                .SetUpdate(true);

                Image gameMenuTransitionImage = gameMenu.transform.Find("Transition Image").GetComponent<Image>();
                gameMenuTransitionImage
                .DOFade(0f, transitionDuration)
                .OnStart(() => {
                    gameMenuTransitionImage.DOFade(0f, 0f);
                    gameMenu.SetActive(true);
                })
                .SetUpdate(true);

                Time.timeScale = 1f;
            }
        }
    }
    public bool IsDead {
        get { return isDead; }
        set { 
            isDead = value;
            if(isDead) {
                Image gameMenuTransitionImage = gameMenu.transform.Find("Transition Image").GetComponent<Image>();
                gameMenuTransitionImage
                .DOFade(1f, transitionDuration)
                .OnStart(() => {
                    gameMenuTransitionImage.DOFade(0f, 0f);
                })
                .OnComplete(() => {
                    gameMenu.SetActive(false);
                })
                .SetUpdate(true);

                Image deadMenuTransitionImage = deadMenu.transform.Find("Transition Image").GetComponent<Image>();
                deadMenuTransitionImage
                .DOFade(1f, transitionDuration)
                .OnStart(() => {
                    deadMenuTransitionImage.DOFade(1f, 0f);
                    deadMenu.SetActive(true);
                })
                .SetUpdate(true);

                Time.timeScale = 0f;
            }
            else {
                Image deadMenuTransitionImage = deadMenu.transform.Find("Transition Image").GetComponent<Image>();
                deadMenuTransitionImage
                .DOFade(0f, transitionDuration)
                .OnComplete(() => {
                    deadMenu.SetActive(false);
                })
                .SetUpdate(true);

                Image gameMenuTransitionImage = gameMenu.transform.Find("Transition Image").GetComponent<Image>();
                gameMenuTransitionImage
                .DOFade(0f, transitionDuration)
                .OnStart(() => {
                    gameMenuTransitionImage.DOFade(0f, 0f);
                    gameMenu.SetActive(true);
                })
                .SetUpdate(true);

                Time.timeScale = 1f;
            }
        }
    }

    private void Start() {
        canviiTransform = GameObject.Find("Canvii").transform;

        gameMenu = canviiTransform.Find("Game Canvas").gameObject;
        pauseMenu = canviiTransform.Find("Pause Canvas").gameObject;
        deadMenu = canviiTransform.Find("Dead Canvas").gameObject;

        IsPaused = false;
        IsDead = false;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !deadMenu.activeSelf) {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu() {
        IsPaused = !IsPaused;
    }
    public void ShowDeathMenu() {
        IsDead = true;
    }
}
