using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum State
    {
        Outside,
        InsideCar,
        Dead,
        Cutscene
    }

    [Header("Current State")]
    [SerializeField] private State currentState = State.Outside;

    public State CurrentState => currentState;

    public bool IsOutside =>
        currentState == State.Outside;

    public bool IsInsideCar =>
        currentState == State.InsideCar;

    public bool IsDead =>
        currentState == State.Dead;

    public bool IsInCutscene =>
        currentState == State.Cutscene;

    public bool CanMove =>
        currentState == State.Outside ||
        currentState == State.InsideCar;

    public void SetState(State newState)
    {
        currentState = newState;
    }
}