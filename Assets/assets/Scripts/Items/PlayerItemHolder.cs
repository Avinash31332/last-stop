using UnityEngine;

public class PlayerItemHolder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform hand;

    [Header("Drop Settings")]
    [SerializeField] private float dropForwardDistance = 0.5f;
    [SerializeField] private float dropUpwardOffset = 0.1f;

    private GameObject currentHeldObject;
    private PickupItem currentItem;

    public bool HasItem => currentItem != null;

    public PickupItem CurrentItem => currentItem;

    private void Update()
    {
        HandleDropInput();
    }

    private void HandleDropInput()
    {
        if (!HasItem)
            return;

        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.gKey.wasPressedThisFrame)
        {
            DropCurrentItem();
        }
    }

    public bool TryPickup(PickupItem item)
    {
        if (item == null)
            return false;

        // Only one item can be held.
        if (HasItem)
            return false;

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

        return true;
    }

    public void DropCurrentItem()
    {
        if (!HasItem)
            return;

        Vector3 dropPosition =
            hand.position +
            hand.forward * dropForwardDistance +
            Vector3.up * dropUpwardOffset;

        Quaternion dropRotation =
            hand.rotation;

        currentItem.DropIntoWorld(
            dropPosition,
            dropRotation
        );

        Destroy(currentHeldObject);

        currentHeldObject = null;
        currentItem = null;
    }
}