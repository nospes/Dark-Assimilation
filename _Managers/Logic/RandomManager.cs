namespace MyGame
{
    // Uma classe simples para lidar com a geração de números aleatórios com sementes
    public class RandomGenerator
    {
        private Random _random;

        // O construtor inicializa o objeto Random com uma semente
        public RandomGenerator(int seed)
        {
            _random = new Random(seed);
        }

        // Gera um número inteiro aleatório entre min e max (inclusivo)
        public int NextInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        // Gera um número flutuante aleatório entre 0.0 e 1.0
        public float NextFloat()
        {
            // Random.NextDouble retorna um double entre 0.0 e 1.0
            // Converte para float antes de retornar
            return (float)_random.NextDouble();
        }

        // Método estático para gerar uma semente a partir do horário atual
        public static int GenerateSeedFromCurrentTime()
        {
            return (int)DateTime.Now.Ticks;
        }
    }
}
