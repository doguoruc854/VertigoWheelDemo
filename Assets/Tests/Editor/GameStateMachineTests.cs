using NUnit.Framework;

public class GameStateMachineTests
{
    [Test]
    public void Starts_Idle()
    {
        var sm = new GameStateMachine();
        Assert.AreEqual(GameState.Idle, sm.CurrentState);
    }

    [Test]
    public void Idle_To_Spinning_And_LeaveEnded_Allowed()
    {
        var sm = new GameStateMachine();
        Assert.IsTrue(sm.TryStateTransition(GameState.Spinning));
        Assert.AreEqual(GameState.Spinning, sm.CurrentState);

        sm = new GameStateMachine();
        Assert.IsTrue(sm.TryStateTransition(GameState.Ended));
        Assert.AreEqual(GameState.Ended, sm.CurrentState);
    }

    [Test]
    public void Ended_Only_To_Idle()
    {
        var sm = new GameStateMachine();
        sm.TryStateTransition(GameState.Ended);

        Assert.IsFalse(sm.TryStateTransition(GameState.Spinning));
        Assert.AreEqual(GameState.Ended, sm.CurrentState);

        Assert.IsTrue(sm.TryStateTransition(GameState.Idle));
        Assert.AreEqual(GameState.Idle, sm.CurrentState);
    }

    [Test]
    public void Spinning_Only_To_Result()
    {
        var sm = new GameStateMachine();
        sm.TryStateTransition(GameState.Spinning);

        Assert.IsFalse(sm.TryStateTransition(GameState.Idle));
        Assert.AreEqual(GameState.Spinning, sm.CurrentState);

        Assert.IsFalse(sm.TryStateTransition(GameState.GameOver));
        Assert.AreEqual(GameState.Spinning, sm.CurrentState);

        Assert.IsTrue(sm.TryStateTransition(GameState.Result));
        Assert.AreEqual(GameState.Result, sm.CurrentState);
    }

    [Test]
    public void Result_To_Idle_Or_GameOver()
    {
        var sm = new GameStateMachine();
        sm.TryStateTransition(GameState.Spinning);
        sm.TryStateTransition(GameState.Result);

        Assert.IsFalse(sm.TryStateTransition(GameState.Spinning));
        Assert.AreEqual(GameState.Result, sm.CurrentState);

        Assert.IsTrue(sm.TryStateTransition(GameState.GameOver));
        Assert.AreEqual(GameState.GameOver, sm.CurrentState);

        sm = new GameStateMachine();
        sm.TryStateTransition(GameState.Spinning);
        sm.TryStateTransition(GameState.Result);
        Assert.IsTrue(sm.TryStateTransition(GameState.Idle));
        Assert.AreEqual(GameState.Idle, sm.CurrentState);
    }

    [Test]
    public void GameOver_Only_To_Idle()
    {
        var sm = new GameStateMachine();
        sm.TryStateTransition(GameState.GameOver);

        Assert.IsFalse(sm.TryStateTransition(GameState.Spinning));
        Assert.AreEqual(GameState.GameOver, sm.CurrentState);

        Assert.IsTrue(sm.TryStateTransition(GameState.Idle));
        Assert.AreEqual(GameState.Idle, sm.CurrentState);
    }

    [Test]
    public void Same_State_Returns_False()
    {
        var sm = new GameStateMachine();
        Assert.IsFalse(sm.TryStateTransition(GameState.Idle));
        Assert.AreEqual(GameState.Idle, sm.CurrentState);
    }
}