using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button enterWorldButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (enterWorldButton != null)
        {
            enterWorldButton.onClick.AddListener(EnterWorld);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    private void EnterWorld()
    {
        SceneManager.LoadScene("StarterTown");
    }

    private void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}