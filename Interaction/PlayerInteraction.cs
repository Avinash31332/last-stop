using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InteractionPrompt interactionPrompt;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayers;

    [Header("Detection")]
    [SerializeField] private float detectionInterval = 0.033f;

    private IInteractable currentInteractable;
    private PickupItem currentPickup;

    private PlayerItemHolder itemHolder;

    private float detectionTimer;

    private void Awake()
    {
        itemHolder = GetComponent<PlayerItemHolder>();
    }

    private void Start()
    {
        ClearTarget();
    }

    private void Update()
    {
        detectionTimer += Time.deltaTime;

        if (detectionTimer >= detectionInterval)
        {
            detectionTimer = 0f;
            CheckForInteractable();
        }

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
            if (itemHolder == null)
                return;

            if (itemHolder.HasItem)
            {
                itemHolder.TrySwap(currentPickup);
            }
            else
            {
                itemHolder.TryPickup(currentPickup);
            }

            return;
        }

        if (itemHolder != null &&
            itemHolder.HasItem)
        {
            itemHolder.DropCurrentItem();
        }
    }

    private void Interact()
    {
        if (currentInteractable == null)
            return;

        currentInteractable.Interact();
    }

    private void CheckForInteractable()
    {
        IInteractable newInteractable = null;
        PickupItem newPickup = null;

        if (playerCamera != null)
        {
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
                newPickup =
                    hit.collider
                        .GetComponentInParent<PickupItem>();

                if (newPickup != null)
                {
                    newInteractable = newPickup;
                }
                else
                {
                    newInteractable =
                        hit.collider
                            .GetComponentInParent<IInteractable>();
                }
            }
        }

        if (newInteractable == currentInteractable)
            return;

        currentInteractable = newInteractable;
        currentPickup = newPickup;

        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (interactionPrompt == null ||
            currentInteractable == null)
        {
            interactionPrompt?.Hide();
            return;
        }

        string actionText =
            currentInteractable.InteractionText;

        if (currentPickup != null &&
            itemHolder != null &&
            itemHolder.HasItem)
        {
            string itemName =
                currentPickup.InteractionText
                    .Replace("Pick Up ", "");

            actionText =
                "Swap for " + itemName;
        }

        interactionPrompt.Show(
            "[E / LMB] " + actionText
        );
    }

    private void ClearTarget()
    {
        currentInteractable = null;
        currentPickup = null;

        if (interactionPrompt != null)
        {
            interactionPrompt.Hide();
        }
    }
}