using UnityEngine;

public class CarEnterExit : MonoBehaviour, IInteractable
{
    [Header("Car Points")]
    [SerializeField] private Transform enterPoint;
    [SerializeField] private Transform interiorPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Player")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private PlayerState playerState;

    [Header("Settings")]
    [SerializeField] private bool playerStartsOutside = true;

    private void Start()
    {
        if (playerState == null)
        {
            playerState =
                playerTransform
                    .GetComponent<PlayerState>();
        }

        if (!playerStartsOutside)
        {
            playerState.SetState(
                PlayerState.State.InsideCar
            );
        }
    }

    public string InteractionText
    {
        get
        {
            if (playerState != null &&
                playerState.IsInsideCar)
            {
                return "Exit Car";
            }

            return "Enter Car";
        }
    }

    public void Interact()
    {
        if (playerState == null)
            return;

        if (playerState.IsInsideCar)
        {
            ExitCar();
        }
        else
        {
            EnterCar();
        }
    }

    private void EnterCar()
    {
        if (playerTransform == null ||
            interiorPoint == null)
        {
            return;
        }

        playerState.SetState(
            PlayerState.State.InsideCar
        );

        MovePlayer(
            interiorPoint
        );
    }

    private void ExitCar()
    {
        if (playerTransform == null ||
            exitPoint == null)
        {
            return;
        }

        MovePlayer(
            exitPoint
        );

        playerState.SetState(
            PlayerState.State.Outside
        );
    }

    private void MovePlayer(
        Transform target)
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        playerTransform.SetPositionAndRotation(
            target.position,
            target.rotation
        );

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}