namespace MyGame;


public static class ProfileManager
{
    public static int aggressiveCount = 0;
    public static int balancedCount = 0;
    public static int evasiveCount = 0;

    public static int enemyProfileType = 0;

    // Threshold to determine if percentages are considered close
    private const double percentageThreshold = 0.04;


    public static void UpdateEnemyProfileType()
    {
        int totalCount = aggressiveCount + balancedCount + evasiveCount;
        Random rnd = new Random();


        if (totalCount > 0)
        {
            double aggressivePercentage = (double)aggressiveCount / totalCount;
            double balancedPercentage = (double)balancedCount / totalCount;
            double evasivePercentage = (double)evasiveCount / totalCount;

            // Using a tuple list for easier sorting and identification
            var profilePercentages = new List<(int ProfileType, double Percentage)>
            {
                (1, aggressivePercentage),
                (2, balancedPercentage),
                (3, evasivePercentage)
            };

            // Sort by percentage in descending order
            profilePercentages.Sort((a, b) => b.Percentage.CompareTo(a.Percentage));

            // Check if the top two percentages are within the threshold
            if (profilePercentages[0].Percentage - profilePercentages[1].Percentage <= percentageThreshold)
            {
                // Randomly choose between the top two profile types
                enemyProfileType = rnd.Next(0, 2) == 0 ? profilePercentages[0].ProfileType : profilePercentages[1].ProfileType;
            }
            else
            {
                // Otherwise, choose the profile with the highest percentage
                enemyProfileType = profilePercentages[0].ProfileType;
            }
        }
    }

    public static void ClearCounts()
    {
        aggressiveCount = 0;
        balancedCount = 0;
        evasiveCount = 0;
        enemyProfileType = 0;
    }

    

    
}

