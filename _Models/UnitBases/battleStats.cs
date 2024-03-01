namespace MyGame;

public class BattleStats
{
    private TimeSpan TotalBattleTime { get; set; }
    private TimeSpan TimeAfterFirstHit { get; set; }
    private bool firstHitReceived = false;
    private DateTime battleStartTime, firstHitTime;
    private int DashCount { get; set; }
    public TimeSpan FinalBattleTime, FinalTimeAfterFirstHit;
    public bool inBattle = false;
    public int FinalDashCount { get; set; }


    // Starts tracking the battle time
    public void StartBattle()
    {
        TotalBattleTime = TimeSpan.Zero;
        TimeAfterFirstHit = TimeSpan.Zero;
        battleStartTime = DateTime.Now;
        firstHitReceived = false;
        inBattle = true;
    }

    // Marks the moment the first hit is received and starts tracking time after first hit
    public void MarkFirstHit()
    {
        if (!firstHitReceived)
        {
            firstHitReceived = true;
            firstHitTime = DateTime.Now;
        }
    }

    bool dashend = true;
    public void IncrementDashCount()
    {
        if (Hero.DASH && dashend)
        {
            DashCount++;
            dashend = false;
        }
        else if(!Hero.DASH) dashend = true;
    }

    // Updates the battle and first hit times
    public void Update()
    {
        if (battleStartTime != DateTime.MinValue)
        {
            TotalBattleTime = DateTime.Now - battleStartTime;
            if (firstHitReceived)
            {
                TimeAfterFirstHit = DateTime.Now - firstHitTime;
            }
        }
    }

    // Optionally, call this method to stop tracking if needed
    public void EndBattle()
    {
        Update(); // Ensure the final times are updated before stopping
        FinalBattleTime = TotalBattleTime;// Reset or process statistics here as needed
        FinalTimeAfterFirstHit = TimeAfterFirstHit;
        FinalDashCount = DashCount;
    }
}