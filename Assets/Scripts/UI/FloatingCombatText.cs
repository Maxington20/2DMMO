using TMPro;
using UnityEngine;

public class FloatingCombatText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("Movement")]
    [SerializeField] private float floatSpeed = 45f;
    [SerializeField] private float lifetime = 0.8f;

    private float timer;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnEnable()
    {
        timer = lifetime;
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }
}