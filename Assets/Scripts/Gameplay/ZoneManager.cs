public class ZoneManager {

public int CurrentZone { get; private set; } = 1;

private const int SuperEvery = 30;
public bool IsSuperZone => CurrentZone > 0 && CurrentZone % SuperEvery == 0;

private const int SafeEvery = 5;
public bool IsSafeZone => CurrentZone > 0 && CurrentZone % SafeEvery == 0
&& !IsSuperZone;

public ZoneType CurrentType =>
    IsSuperZone ? ZoneType.Super :
    IsSafeZone ? ZoneType.Safe :
                ZoneType.Normal;

public void AdvanceZone()
{
    CurrentZone++;
}

public void Reset()
{
    CurrentZone = 1;
}
}