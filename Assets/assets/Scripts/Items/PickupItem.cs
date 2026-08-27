using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private string itemName = "Item";

    [Header("Held Version")]
    [SerializeField] private GameObject heldPrefab;

    [Header("Pickup Audio")]
    [SerializeField] private AudioClip pickupSound;

    [Header("World Physics")]
    [SerializeField] private Rigidbody worldRigidbody;

    public string InteractionText =>
        "Pick Up " + itemName;

    private void Awake()
    {
        if (worldRigidbody == null)
        {
            worldRigidbody =
                GetComponent<Rigidbody>();
        }

        if (worldRigidbody != null)
        {
            worldRigidbody.isKinematic = true;
            worldRigidbody.useGravity = false;
        }
    }

    public void Interact()
    {
        PlayerItemHolder holder =
            FindFirstObjectByType<PlayerItemHolder>();

        if (holder == null)
            return;

        if (!holder.TryPickup(this))
            return;

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(
                pickupSound,
                transform.position
            );
        }

        gameObject.SetActive(false);
    }

    public GameObject GetHeldPrefab()
    {
        return heldPrefab;
    }

    public void DropIntoWorld(
        Vector3 position,
        Quaternion rotation)
    {
        transform.SetPositionAndRotation(
            position,
            rotation
        );

        gameObject.SetActive(true);

        if (worldRigidbody != null)
        {
            worldRigidbody.isKinematic = false;
            worldRigidbody.useGravity = true;

            worldRigidbody.linearVelocity =
                Vector3.zero;

            worldRigidbody.angularVelocity =
                Vector3.zero;

            worldRigidbody.WakeUp();
        }
    }
}