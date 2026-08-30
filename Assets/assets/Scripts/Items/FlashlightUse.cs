using UnityEngine;

public class FlashlightUse : MonoBehaviour, IUsableItem
{
    [Header("Light")]
    [SerializeField] private Light flashlightLight;

    [Header("Settings")]
    [SerializeField] private bool startsOn = false;

    private bool isOn;

    private void Awake()
    {
        isOn = startsOn;

        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;
        }
    }

    public void Use()
    {
        isOn = !isOn;

        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;
        }
    }
}