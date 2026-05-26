using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Character Cards")]
    [SerializeField] private Button[] characterSlotButtons;
    [SerializeField] private TextMeshProUGUI[] characterSlotTexts;

    [Header("Selected Character Details")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterDetailsText;

    [Header("Main Buttons")]
    [SerializeField] private Button enterWorldButton;
    [SerializeField] private Button createCharacterButton;
    [SerializeField] private Button deleteCharacterButton;
    [SerializeField] private Button quitButton;

    [Header("Creation Panel")]
    [SerializeField] private GameObject creationPanel;
    [SerializeField] private TMP_InputField characterNameInput;
    [SerializeField] private TMP_Dropdown speciesDropdown;
    [SerializeField] private TMP_Dropdown classDropdown;
    [SerializeField] private Button confirmCreateButton;
    [SerializeField] private Button cancelCreateButton;

    [Header("Delete Confirmation")]
    [SerializeField] private GameObject deleteConfirmationPanel;
    [SerializeField] private TextMeshProUGUI deleteConfirmationText;
    [SerializeField] private Button confirmDeleteButton;
    [SerializeField] private Button cancelDeleteButton;

    private CharacterData selectedCharacter;
    private int selectedSlotIndex;

    private void Awake()
    {
        selectedSlotIndex = CharacterSaveManager.GetSelectedSlot();
        selectedCharacter = CharacterSaveManager.LoadCharacter(selectedSlotIndex);

        SetupSlotButtons();
        SetupMainButtons();
        SetupDropdowns();

        HideCreationPanel();
        HideDeleteConfirmationPanel();

        RefreshAllSlots();
        RefreshSelectedCharacterCard();
    }

    private void SetupSlotButtons()
    {
        if (characterSlotButtons == null)
        {
            return;
        }

        for (int i = 0; i < characterSlotButtons.Length; i++)
        {
            int capturedIndex = i;

            if (characterSlotButtons[i] != null)
            {
                characterSlotButtons[i].onClick.AddListener(() => SelectSlot(capturedIndex));
            }
        }
    }

    private void SetupMainButtons()
    {
        if (enterWorldButton != null)
        {
            enterWorldButton.onClick.AddListener(EnterWorld);
        }

        if (createCharacterButton != null)
        {
            createCharacterButton.onClick.AddListener(ShowCreationPanel);
        }

        if (deleteCharacterButton != null)
        {
            deleteCharacterButton.onClick.AddListener(ShowDeleteConfirmationPanel);
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

        if (confirmDeleteButton != null)
        {
            confirmDeleteButton.onClick.AddListener(ConfirmDeleteSelectedCharacter);
        }

        if (cancelDeleteButton != null)
        {
            cancelDeleteButton.onClick.AddListener(HideDeleteConfirmationPanel);
        }
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

    private void SelectSlot(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        CharacterSaveManager.SetSelectedSlot(selectedSlotIndex);

        selectedCharacter = CharacterSaveManager.LoadCharacter(selectedSlotIndex);

        HideDeleteConfirmationPanel();
        RefreshAllSlots();
        RefreshSelectedCharacterCard();
    }

    private void RefreshAllSlots()
    {
        if (characterSlotTexts == null)
        {
            return;
        }

        for (int i = 0; i < characterSlotTexts.Length; i++)
        {
            CharacterData slotCharacter = CharacterSaveManager.LoadCharacter(i);

            if (characterSlotTexts[i] == null)
            {
                continue;
            }

            if (slotCharacter == null)
            {
                characterSlotTexts[i].text = i == selectedSlotIndex
                    ? $"<b>Slot {i + 1}</b>\nEmpty\n<color=#FFD45A>Selected</color>"
                    : $"<b>Slot {i + 1}</b>\nEmpty";
            }
            else
            {
                string selectedText = i == selectedSlotIndex
                    ? "\n<color=#FFD45A>Selected</color>"
                    : string.Empty;

                characterSlotTexts[i].text =
                    $"<b>{slotCharacter.CharacterName}</b>\n" +
                    $"Level {slotCharacter.Level} {slotCharacter.Species} {slotCharacter.ClassName}" +
                    selectedText;
            }
        }
    }

    private void RefreshSelectedCharacterCard()
    {
        if (selectedCharacter == null)
        {
            if (characterNameText != null)
            {
                characterNameText.text = $"Slot {selectedSlotIndex + 1}: Empty";
            }

            if (characterDetailsText != null)
            {
                characterDetailsText.text = "Create a character in this slot to begin.";
            }

            if (enterWorldButton != null)
            {
                enterWorldButton.interactable = false;
            }

            if (deleteCharacterButton != null)
            {
                deleteCharacterButton.interactable = false;
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

        if (deleteCharacterButton != null)
        {
            deleteCharacterButton.interactable = true;
        }
    }

    private void ShowCreationPanel()
    {
        HideDeleteConfirmationPanel();

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

        CharacterSaveManager.SaveCharacter(selectedSlotIndex, selectedCharacter);
        CharacterSaveManager.SetSelectedSlot(selectedSlotIndex);

        RefreshAllSlots();
        RefreshSelectedCharacterCard();
        HideCreationPanel();
    }

    private void ShowDeleteConfirmationPanel()
    {
        if (selectedCharacter == null || deleteConfirmationPanel == null)
        {
            return;
        }

        if (deleteConfirmationText != null)
        {
            deleteConfirmationText.text =
                $"Delete {selectedCharacter.CharacterName}?\n\nThis cannot be undone.";
        }

        deleteConfirmationPanel.SetActive(true);
    }

    private void HideDeleteConfirmationPanel()
    {
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }
    }

    private void ConfirmDeleteSelectedCharacter()
    {
        CharacterSaveManager.DeleteSavedCharacter(selectedSlotIndex);

        selectedCharacter = null;

        HideDeleteConfirmationPanel();
        RefreshAllSlots();
        RefreshSelectedCharacterCard();
    }

    private void EnterWorld()
    {
        if (selectedCharacter == null)
        {
            return;
        }

        CharacterSaveManager.SetSelectedSlot(selectedSlotIndex);
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