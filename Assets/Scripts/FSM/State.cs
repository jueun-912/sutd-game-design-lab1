using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/State")]
public class State : ScriptableObject
{
    public Action[] setupActions = new Action[0];
    public Action[] actions = new Action[0];
    public EventAction[] eventTriggeredActions = new EventAction[0];
    public Action[] exitActions = new Action[0];
    public Transition[] transitions = new Transition[0];

    public Color sceneGizmoColor = Color.grey;

    public void DoSetupActions(StateController controller)
    {
        RunActions(setupActions, controller);
    }

    public void DoExitActions(StateController controller)
    {
        RunActions(exitActions, controller);
    }

    public void DoEventTriggeredActions(
        StateController controller,
        ActionType type = ActionType.Default)
    {
        foreach (EventAction entry in eventTriggeredActions)
        {
            if (entry.type == type && entry.action != null)
                entry.action.Act(controller);
        }
    }

    public void UpdateState(StateController controller)
    {
        RunActions(actions, controller);

        foreach (Transition transition in transitions)
        {
            if (transition == null || transition.decision == null)
                continue;

            State next = transition.decision.Decide(controller)
                ? transition.trueState
                : transition.falseState;

            controller.TransitionToState(next);

            if (controller.currentState != this)
                break;
        }
    }

    private void RunActions(
        Action[] list,
        StateController controller)
    {
        foreach (Action action in list)
        {
            if (action != null)
                action.Act(controller);
        }
    }
}
