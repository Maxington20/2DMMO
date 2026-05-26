using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Character Card")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterDetailsText;

    [Header("Main Buttons")]
    [SerializeField] private Button enterWorldButton;
    [SerializeField] private Button createCharacterButton;
    [SerializeField] private Button quitButton;

    [Header("Creation Panel")]
    [SerializeField] private GameObject creationPanel;
    [SerializeField] private TMP_InputField characterNameInput;
    [SerializeField] private TMP_Dropdown speciesDropdown;
    [SerializeField] private TMP_Dropdown classDropdown;
    [SerializeField] private Button confirmCreateButton;
    [SerializeField] private Button cancelCreateButton;

    private CharacterData selectedCharacter;

    private void Awake()
    {
        selectedCharacter = CharacterSaveManager.LoadCharacter();

        if (selectedCharacter == null)
        {
            selectedCharacter = new CharacterData("Thalen", "Human", "Warrior");
            CharacterSaveManager.SaveCharacter(selectedCharacter);
        }

        if (enterWorldButton != null)
        {
            enterWorldButton.onClick.AddListener(EnterWorld);
        }

        if (createCharacterButton != null)
        {
            createCharacterButton.onClick.AddListener(ShowCreationPanel);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        if (confirmCreateButton != null)
        {
            confirmCreateButton.onClick.AddListener(CreateCharacter);
        }

        if (cancelCreateButton != null)
        {
            cancelCreateButton.onClick.AddListener(HideCreationPanel);
        }

        SetupDropdowns();
        HideCreationPanel();
        RefreshCharacterCard();
    }

    private void SetupDropdowns()
    {
        if (speciesDropdown != null)
        {
            speciesDropdown.ClearOptions();
            speciesDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Human",
                "Elf",
                "Dwarf",
                "Orc",
                "Goblin",
                "Troll"
            });
        }

        if (classDropdown != null)
        {
            classDropdown.ClearOptions();
            classDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Warrior",
                "Wizard",
                "Priest",
                "Ranger",
                "Rogue",
                "Necromancer",
                "Artificer"
            });
        }
    }

    private void RefreshCharacterCard()
    {
        if (selectedCharacter == null)
        {
            if (characterNameText != null)
            {
                characterNameText.text = "No Character";
            }

            if (characterDetailsText != null)
            {
                characterDetailsText.text = "Create a character to begin.";
            }

            if (enterWorldButton != null)
            {
                enterWorldButton.interactable = false;
            }

            return;
        }

        if (characterNameText != null)
        {
            characterNameText.text = selectedCharacter.CharacterName;
        }

        if (characterDetailsText != null)
        {
            characterDetailsText.text =
                $"Level {selectedCharacter.Level} {selectedCharacter.Species} {selectedCharacter.ClassName}\n" +
                $"{selectedCharacter.CurrentXp} / {selectedCharacter.XpToNextLevel} XP\n" +
                $"{selectedCharacter.StartingZone}";
        }

        if (enterWorldButton != null)
        {
            enterWorldButton.interactable = true;
        }
    }

    private void ShowCreationPanel()
    {
        if (creationPanel != null)
        {
            creationPanel.SetActive(true);
        }

        if (characterNameInput != null)
        {
            characterNameInput.text = string.Empty;
            characterNameInput.Select();
        }
    }

    private void HideCreationPanel()
    {
        if (creationPanel != null)
        {
            creationPanel.SetActive(false);
        }
    }

    private void CreateCharacter()
    {
        if (characterNameInput == null || speciesDropdown == null || classDropdown == null)
        {
            return;
        }

        string characterName = characterNameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(characterName))
        {
            characterName = "New Adventurer";
        }

        string species = speciesDropdown.options[speciesDropdown.value].text;
        string className = classDropdown.options[classDropdown.value].text;

        selectedCharacter = new CharacterData(characterName, species, className);

        CharacterSaveManager.SaveCharacter(selectedCharacter);

        RefreshCharacterCard();
        HideCreationPanel();
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