using UnityEngine;

public class PlayerCharacterInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NameplateController playerNameplate;
    [SerializeField] private PlayerFrameUI playerFrameUI;
    [SerializeField] private CombatEntity combatEntity;

    private void Start()
    {
        CharacterData savedCharacter = CharacterSaveManager.LoadCharacter();

        if (savedCharacter == null)
        {
            return;
        }

        ApplyCharacter(savedCharacter);
    }

    private void ApplyCharacter(CharacterData characterData)
    {
        if (playerNameplate != null)
        {
            playerNameplate.SetName(characterData.CharacterName);
        }

        if (playerFrameUI != null)
        {
            playerFrameUI.SetPlayerName(characterData.CharacterName);
        }

        if (combatEntity != null)
        {
            combatEntity.SetDisplayName(characterData.CharacterName);
        }

        ChatManager.Instance?.AddSystemMessage(
            $"Entered world as {characterData.CharacterName}, Level {characterData.Level} {characterData.Species} {characterData.ClassName}."
        );
    }
}