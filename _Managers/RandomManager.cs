namespace MyGame
{
    // A simple class to handle random number generation with seeds
    public class RandomGenerator
    {
        private Random _random;

        // Constructor initializes the Random object with a seed
        public RandomGenerator(int seed)
        {
            _random = new Random(seed);
        }

        // Generate a random integer between min and max (inclusive)
        public int NextInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        // Generate a random float between 0.0 and 1.0
        public float NextFloat()
        {
            // Random.NextDouble returns a double between 0.0 and 1.0
            // Convert it to float before returning
            return (float)_random.NextDouble();
        }

        // Static method to generate a seed from the current time
        public static int GenerateSeedFromCurrentTime()
        {
            return (int)DateTime.Now.Ticks;
        }
    }
}
