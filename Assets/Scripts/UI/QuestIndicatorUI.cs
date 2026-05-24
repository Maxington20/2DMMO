using TMPro;
using UnityEngine;

public class QuestIndicatorUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private QuestGiver questGiver;
    [SerializeField] private TextMeshProUGUI indicatorText;

    [Header("Position")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.65f, 0f);

    private Camera mainCamera;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        mainCamera = Camera.main;
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void LateUpdate()
    {
        if (target == null || questGiver == null || indicatorText == null || mainCamera == null)
        {
            Hide();
            return;
        }

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position + worldOffset);
        transform.position = screenPosition;

        RefreshIndicator();
    }

    private void RefreshIndicator()
    {
        QuestIndicatorState state = questGiver.GetIndicatorState();

        switch (state)
        {
            case QuestIndicatorState.Available:
                indicatorText.text = "!";
                indicatorText.color = new Color32(255, 220, 70, 255);
                Show();
                break;

            case QuestIndicatorState.ReadyToTurnIn:
                indicatorText.text = "?";
                indicatorText.color = new Color32(255, 220, 70, 255);
                Show();
                break;

            case QuestIndicatorState.None:
            default:
                Hide();
                break;
        }
    }

    private void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}