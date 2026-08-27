using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemHolder : MonoBehaviour
{
    public static PlayerItemHolder Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform hand;

    [Header("Drop Settings")]
    [SerializeField] private float dropForwardDistance = 0.5f;
    [SerializeField] private float dropUpwardOffset = 0.1f;

    private GameObject currentHeldObject;
    private PickupItem currentItem;
    private IUsableItem usableItem;

    public bool HasItem =>
        currentItem != null;

    public PickupItem CurrentItem =>
        currentItem;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        HandleItemInput();
    }

    private void HandleItemInput()
    {
        if (Keyboard.current == null)
            return;

        if (Mouse.current != null &&
            Mouse.current.rightButton.wasPressedThisFrame)
        {
            UseCurrentItem();
        }
    }

    public bool TryPickup(PickupItem item)
    {
        if (item == null)
            return false;

        if (HasItem)
            return false;

        return PickupItemInternal(item);
    }

    public bool TrySwap(PickupItem newItem)
{
    if (newItem == null)
        return false;

    if (!HasItem)
        return PickupItemInternal(newItem);

    GameObject newHeldPrefab =
        newItem.GetHeldPrefab();

    if (newHeldPrefab == null)
        return false;

    DropCurrentItem();

    return PickupItemInternal(newItem);
}

    private bool PickupItemInternal(
        PickupItem item)
    {
        GameObject heldPrefab =
            item.GetHeldPrefab();

        if (heldPrefab == null)
            return false;

        currentHeldObject =
            Instantiate(
                heldPrefab,
                hand
            );

        currentHeldObject.transform.localPosition =
            Vector3.zero;

        currentHeldObject.transform.localRotation =
            Quaternion.identity;

        currentHeldObject.transform.localScale =
            Vector3.one;

        currentItem = item;

        usableItem =
            currentHeldObject.GetComponent<IUsableItem>();

        item.OnPickedUp();

        return true;
    }

    private void UseCurrentItem()
    {
        if (!HasItem)
            return;

        if (usableItem == null)
            return;

        usableItem.Use();
    }

    public void DropCurrentItem()
    {
        if (!HasItem)
            return;

        Vector3 dropPosition =
            hand.position +
            hand.forward *
            dropForwardDistance +
            Vector3.up *
            dropUpwardOffset;

        Quaternion dropRotation =
            hand.rotation;

        currentItem.DropIntoWorld(
            dropPosition,
            dropRotation
        );

        Destroy(currentHeldObject);

        currentHeldObject = null;
        currentItem = null;
        usableItem = null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}