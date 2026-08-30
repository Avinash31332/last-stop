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
            worldRigidbody = GetComponent<Rigidbody>();
        }

        SetWorldPhysics(false);
    }

    public void Interact()
    {
        PlayerItemHolder holder =
            PlayerItemHolder.Instance;

        if (holder == null)
            return;

        holder.TryPickup(this);
    }

    public GameObject GetHeldPrefab()
    {
        return heldPrefab;
    }

    public void OnPickedUp()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(
                pickupSound,
                transform.position
            );
        }

        SetWorldPhysics(false);

        gameObject.SetActive(false);
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

        SetWorldPhysics(true);
    }

    private void SetWorldPhysics(bool active)
    {
        if (worldRigidbody == null)
            return;

        worldRigidbody.isKinematic = !active;
        worldRigidbody.useGravity = active;

        if (active)
        {
            worldRigidbody.linearVelocity =
                Vector3.zero;

            worldRigidbody.angularVelocity =
                Vector3.zero;

            worldRigidbody.WakeUp();
        }
        else
        {
            worldRigidbody.linearVelocity =
                Vector3.zero;

            worldRigidbody.angularVelocity =
                Vector3.zero;
        }
    }
}