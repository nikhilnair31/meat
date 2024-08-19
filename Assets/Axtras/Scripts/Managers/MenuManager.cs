using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour 
{
    private bool isPaused = false;
    private bool isDead = false;
    private Transform canviiTransform;
    private Image gameMenuTransitionImage;
    private Image pauseMenuTransitionImage;     
    private Image deadMenuTransitionImage;       

    [Header("Canvas Menus")]
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deadMenu;

    [Header("Values")]
    [SerializeField] private float transitionDuration = 0.5f;

    // FIXME: Improve this code to make it modular using Helper static function for transitions
    public bool IsPaused {
        get { return isPaused; }
        set { 
            isPaused = value;

            // HandleMenuTransition(
            //     isPaused, 
            //     gameMenuTransitionImage, 
            //     gameMenu, 
            //     pauseMenuTransitionImage, 
            //     pauseMenu
            // );

            // Time.timeScale = isPaused ? 0f : 1f;

            if(isPaused) {
                gameMenuTransitionImage
                .DOFade(1f, transitionDuration)
                .OnStart(() => gameMenuTransitionImage.DOFade(0f, 0f))
                .OnComplete(() => gameMenu.SetActive(false))
                .SetUpdate(true);

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
                pauseMenuTransitionImage
                .DOFade(0f, transitionDuration)
                .OnComplete(() => pauseMenu.SetActive(false))
                .SetUpdate(true);

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

            // HandleMenuTransition(
            //     isDead, 
            //     gameMenuTransitionImage, 
            //     gameMenu, 
            //     deadMenuTransitionImage, 
            //     deadMenu
            // );
            // Time.timeScale = isDead ? 0f : 1f;

            if(isDead) {
                gameMenuTransitionImage
                .DOFade(1f, transitionDuration)
                .OnStart(() => gameMenuTransitionImage.DOFade(0f, 0f))
                .OnComplete(() => gameMenu.SetActive(false))
                .SetUpdate(true);

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
                deadMenuTransitionImage
                .DOFade(0f, transitionDuration)
                .OnComplete(() => deadMenu.SetActive(false))
                .SetUpdate(true);

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

        gameMenuTransitionImage = gameMenu.transform.Find("Transition Image").GetComponent<Image>();
        pauseMenuTransitionImage = pauseMenu.transform.Find("Transition Image").GetComponent<Image>();  
        deadMenuTransitionImage = deadMenu.transform.Find("Transition Image").GetComponent<Image>();
                
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

    // private void HandleMenuTransition(bool isTransitionActive, Image gameMenuTransitionImage, GameObject gameMenu, Image targetMenuTransitionImage, GameObject targetMenu) {
    //     if (isTransitionActive) {
    //         TransitionToMenu(gameMenuTransitionImage, gameMenu, targetMenuTransitionImage, targetMenu, true);
    //     } else {
    //         TransitionToMenu(targetMenuTransitionImage, targetMenu, gameMenuTransitionImage, gameMenu, false);
    //     }
    // }

    // private void TransitionToMenu(Image fadeOutImage, GameObject fadeOutMenu, Image fadeInImage, GameObject fadeInMenu, bool setActive) {
    //     fadeOutImage
    //         .DOFade(1f, transitionDuration)
    //         .OnStart(() => fadeOutImage.DOFade(0f, 0f))
    //         .OnComplete(() => fadeOutMenu.SetActive(false))
    //         .SetUpdate(true);

    //     fadeInImage
    //         .DOFade(1f, transitionDuration)
    //         .OnStart(() => {
    //             fadeInImage.DOFade(1f, 0f);
    //             fadeInMenu.SetActive(setActive);
    //         })
    //         .SetUpdate(true);
    // }
}
