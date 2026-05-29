using UnityEngine;

[RequireComponent(typeof(FakePlayerIdentity))]
public class FakePlayerIdleChatter : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float minDelaySeconds = 25f;
    [SerializeField] private float maxDelaySeconds = 60f;

    [Header("Behavior")]
    [SerializeField] private bool onlyChatWhenQuestingDisabled = true;

    private FakePlayerIdentity identity;
    private FakePlayerQuestController questController;
    private float timer;

    private void Awake()
    {
        identity = GetComponent<FakePlayerIdentity>();
        questController = GetComponent<FakePlayerQuestController>();
    }

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (identity == null)
        {
            return;
        }

        if (onlyChatWhenQuestingDisabled && questController != null && questController.enabled)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            return;
        }

        identity.SayRandom(identity.GetIdleMessages());
        ResetTimer();
    }

    private void ResetTimer()
    {
        timer = Random.Range(minDelaySeconds, maxDelaySeconds);
    }
}