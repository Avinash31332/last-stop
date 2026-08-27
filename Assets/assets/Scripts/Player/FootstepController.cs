using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class FootstepController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepSounds;

    [Header("Walking")]
    [SerializeField] private float walkStepInterval = 0.45f;

    [Header("Sprinting")]
    [SerializeField] private float sprintStepInterval = 0.3f;

    [Header("Volume")]
    [SerializeField] private float volume = 0.8f;

    [Header("Pitch Variation")]
    [SerializeField] private bool randomizePitch = true;
    [SerializeField] private float minimumPitch = 0.95f;
    [SerializeField] private float maximumPitch = 1.05f;

    private PlayerMovement movement;
    private float stepTimer;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        UpdateFootsteps();
    }

    private void UpdateFootsteps()
    {
        if (!movement.IsMoving ||
            !movement.IsGrounded)
        {
            stepTimer = 0f;
            return;
        }

        float currentInterval =
            movement.IsSprinting
            ? sprintStepInterval
            : walkStepInterval;

        stepTimer += Time.deltaTime;

        if (stepTimer >= currentInterval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (audioSource == null)
            return;

        if (footstepSounds == null ||
            footstepSounds.Length == 0)
            return;

        AudioClip clip =
            footstepSounds[
                Random.Range(
                    0,
                    footstepSounds.Length
                )
            ];

        if (clip == null)
            return;

        audioSource.pitch = randomizePitch
            ? Random.Range(
                minimumPitch,
                maximumPitch
            )
            : 1f;

        audioSource.PlayOneShot(
            clip,
            volume
        );
    }
}