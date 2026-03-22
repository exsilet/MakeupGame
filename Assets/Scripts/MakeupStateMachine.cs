using System;
using UnityEngine;

public class MakeupStateMachine : MonoBehaviour
{
    public enum State
    {
        Idle,
        PickingUp,
        Carrying,
        Applying,
        Returning
    }

    public enum Tool
    {
        None,
        Cream,
        Eyeshadow,
        Lipstick,
        Blush
    }

    private State _currentState = State.Idle;
    private Tool _currentTool = Tool.None;

    public event Action<State> OnStateChanged;
    public event Action<Tool> OnToolChanged;

    public State CurrentState => _currentState;
    public Tool CurrentTool => _currentTool;

    public void SetState(State newState)
    {
        if (_currentState == newState) return;
        _currentState = newState;
        OnStateChanged?.Invoke(_currentState);
        Debug.Log($"[StateMachine] State: {_currentState}");
    }

    public void SetTool(Tool newTool)
    {
        if (_currentTool == newTool) return;
        _currentTool = newTool;
        OnToolChanged?.Invoke(_currentTool);
        Debug.Log($"[StateMachine] Tool: {_currentTool}");
    }

    public bool CanInteract() => _currentState == State.Idle;
    public bool CanDrag() => _currentState == State.Carrying;

    public void Reset()
    {
        SetTool(Tool.None);
        SetState(State.Idle);
    }
}
