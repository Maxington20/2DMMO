using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class VendorNPC : MonoBehaviour
{
    [SerializeField] private string vendorName = "Merchant";
    [SerializeField] private float interactionRange = 2f;

    public string VendorName => vendorName;

    private void Update()
    {
        if (GameplayInputLock.ShouldBlockWorldClick())
        {
            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector2 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());

        RaycastHit2D hit =
            Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit.collider == null || hit.collider.gameObject != gameObject)
        {
            return;
        }

        TryInteract();
    }

    private void TryInteract()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            return;
        }

        float distance =
            Vector2.Distance(transform.position, player.transform.position);

        if (distance > interactionRange)
        {
            ChatManager.Instance?.AddSystemMessage("You are too far away.");
            return;
        }

        VendorWindowUI.Instance?.Show(this);
    }
}