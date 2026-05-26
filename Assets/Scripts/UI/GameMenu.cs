using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject menuRoot;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button characterSelectButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(Hide);
        }

        if (characterSelectButton != null)
        {
            characterSelectButton.onClick.AddListener(ReturnToCharacterSelect);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        Hide();
    }

    private void OnDestroy()
    {
        GameplayInputLock.ClearLock(this);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            Toggle();
        }
    }

    private void Toggle()
    {
        if (menuRoot == null)
        {
            return;
        }

        if (menuRoot.activeSelf)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        if (menuRoot == null)
        {
            return;
        }

        menuRoot.SetActive(true);
        GameplayInputLock.SetLocked(this, true);
    }

    private void Hide()
    {
        if (menuRoot == null)
        {
            return;
        }

        menuRoot.SetActive(false);
        GameplayInputLock.SetLocked(this, false);
    }

    private void ReturnToCharacterSelect()
    {
        GameplayInputLock.ClearLock(this);
        SceneManager.LoadScene("CharacterSelect");
    }

    private void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}