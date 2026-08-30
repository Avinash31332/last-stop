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
    private PickupItem currentPickup;

    private PlayerItemHolder itemHolder;

    private void Awake()
    {
        itemHolder =
            GetComponent<PlayerItemHolder>();
    }

    private void Start()
    {
        HideInteractionText();
    }

    private void Update()
    {
        CheckForInteractable();

        HandleInput();
    }

    private void HandleInput()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandlePrimaryAction();
        }
    }

    private void HandlePrimaryAction()
    {
        if (currentPickup != null)
        {
            if (itemHolder != null &&
                itemHolder.HasItem)
            {
                itemHolder.TrySwap(currentPickup);
            }
            else
            {
                itemHolder?.TryPickup(currentPickup);
            }

            return;
        }

        if (itemHolder != null &&
            itemHolder.HasItem)
        {
            itemHolder.DropCurrentItem();
        }
    }

    private void CheckForInteractable()
    {
        currentInteractable = null;
        currentPickup = null;

        if (playerCamera == null)
        {
            HideInteractionText();
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                interactionDistance,
                interactionLayers,
                QueryTriggerInteraction.Ignore))
        {
            HideInteractionText();
            return;
        }

        PickupItem pickup =
            hit.collider.GetComponentInParent<PickupItem>();

        if (pickup != null)
        {
            currentPickup = pickup;
            currentInteractable = pickup;

            if (itemHolder != null &&
                itemHolder.HasItem)
            {
                ShowInteractionText(
                    "Swap for " +
                    pickup.InteractionText
                        .Replace("Pick Up ", "")
                );
            }
            else
            {
                ShowInteractionText(
                    pickup.InteractionText
                );
            }

            return;
        }

        IInteractable interactable =
            hit.collider.GetComponentInParent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable =
                interactable;

            ShowInteractionText(
                interactable.InteractionText
            );

            return;
        }

        HideInteractionText();
    }

    private void Interact()
    {
        if (currentInteractable == null)
            return;

        currentInteractable.Interact();
    }

    private void ShowInteractionText(
        string text)
    {
        if (interactionText == null)
            return;

        interactionText.text =
            "[E / LMB] " + text;

        interactionText.gameObject.SetActive(
            true
        );
    }

    private void HideInteractionText()
    {
        if (interactionText == null)
            return;

        interactionText.gameObject.SetActive(
            false
        );
    }
}