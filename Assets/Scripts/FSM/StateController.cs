using UnityEngine;

public abstract class StateController : MonoBehaviour
{
    public State startState;
    public State previousState;
    public State currentState;
    public State remainState;

    public bool isActive = true;
    public bool transitionStateChanged;

    [HideInInspector]
    public float stateTimeElapsed;

    public virtual void Start()
    {
        ResetStateMachine();
    }

    protected virtual void Update()
    {
        if (!isActive || currentState == null)
            return;

        stateTimeElapsed += Time.deltaTime;
        transitionStateChanged = false;
        currentState.UpdateState(this);
    }

    public void TransitionToState(State nextState)
    {
        if (nextState == null ||
            nextState == remainState ||
            nextState == currentState)
            return;

        OnExitState();

        previousState = currentState;
        currentState = nextState;
        stateTimeElapsed = 0f;
        transitionStateChanged = true;

        OnSetupState();
    }

    public virtual void OnSetupState()
    {
        if (currentState != null)
            currentState.DoSetupActions(this);
    }

    public virtual void OnExitState()
    {
        if (currentState != null)
            currentState.DoExitActions(this);
    }

    public void ResetStateMachine()
    {
        OnExitState();

        previousState = null;
        currentState = startState;
        stateTimeElapsed = 0f;
        transitionStateChanged = true;

        OnSetupState();
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        return stateTimeElapsed >= duration;
    }

    public virtual void OnDrawGizmos()
    {
        if (currentState == null)
            return;

        Gizmos.color = currentState.sceneGizmoColor;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}