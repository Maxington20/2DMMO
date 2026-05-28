using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClearUISelectionOnGameplayInput : MonoBehaviour
{
    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null || EventSystem.current == null)
        {
            return;
        }

        if (keyboard.wKey.wasPressedThisFrame ||
            keyboard.aKey.wasPressedThisFrame ||
            keyboard.sKey.wasPressedThisFrame ||
            keyboard.dKey.wasPressedThisFrame)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}