using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayers;

    private IInteractable currentInteractable;

    private void Start()
    {
        HideInteractionText();
    }

    private void Update()
    {
        CheckForInteractable();

        if (InteractionPressed())
        {
            Interact();
        }
    }

    private bool InteractionPressed()
    {
        bool keyboardPressed =
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame;

        bool mousePressed =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        return keyboardPressed || mousePressed;
    }

    private void CheckForInteractable()
    {
        currentInteractable = null;

        if (playerCamera == null)
        {
            HideInteractionText();
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                interactionDistance,
                interactionLayers,
                QueryTriggerInteraction.Ignore))
        {
            PickupItem pickup =
                hit.collider.GetComponentInParent<PickupItem>();

            if (pickup != null)
            {
                currentInteractable = pickup;

                ShowInteractionText(
                    pickup.InteractionText
                );

                return;
            }
        }

        HideInteractionText();
    }

    private void Interact()
    {
        if (currentInteractable == null)
            return;

        currentInteractable.Interact();
    }

    private void ShowInteractionText(string text)
    {
        if (interactionText == null)
            return;

        interactionText.text =
            "[E / LMB] " + text;

        interactionText.gameObject.SetActive(true);
    }

    private void HideInteractionText()
    {
        if (interactionText == null)
            return;

        interactionText.gameObject.SetActive(false);
    }
}