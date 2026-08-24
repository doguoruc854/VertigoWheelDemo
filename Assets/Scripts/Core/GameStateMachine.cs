public class GameStateMachine
{
    public GameState CurrentState { get; private set; } = GameState.Idle;

    public bool TryStateTransition(GameState next)
    {
        if (next == CurrentState)
            return false;

        bool allowed =
            (CurrentState == GameState.Idle && (next == GameState.Spinning || next == GameState.GameOver || next == GameState.Ended)) ||
            (CurrentState == GameState.Spinning && next == GameState.Result) ||
            (CurrentState == GameState.Result && (next == GameState.Idle || next == GameState.GameOver)) ||
            (CurrentState == GameState.GameOver && next == GameState.Idle) ||
            (CurrentState == GameState.Ended && next == GameState.Idle);

        if (!allowed)
            return false;

        CurrentState = next;
        return true;
    }
}
