using System.Linq;

namespace MyGame
{
    public class SoulManager
    {
        public List<Soul> _souls = new List<Soul>(); // Lista de almas

        public SoulManager()
        {
        }

        public void AddSoul(Vector2 position)
        {
            lock (_souls) _souls.Add(new Soul(position)); // Adiciona uma alma na posição chamada
        }

        public void AddPrePositionSouls()
        {
            // Inicializa o gerenciador de aleatoriedade
            RandomGenerator randomGen = new RandomGenerator(RandomGenerator.GenerateSeedFromCurrentTime());

            var positions = new List<Vector2> // Cria lista com posições fixas das almas
            {
                new Vector2(1000, 300),
                new Vector2(363, 1525),
                new Vector2(1525, 1525)
            };

            // Organiza a lista com ajuda do gerenciador de aleatoriedade
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int swapIndex = randomGen.NextInt(0, i + 1);
                Vector2 temp = positions[i];
                positions[i] = positions[swapIndex];
                positions[swapIndex] = temp;
            }

            // adiciona as almas usando as primeiras posições da lista
            lock (_souls)
            {
                _souls.Add(new Soul(positions[0])); 
                _souls.Add(new Soul(positions[1])); 
            }
        }

        public void Update()
        {
            lock (_souls)
            {
                foreach (var soul in _souls)
                {
                    soul.Update(); // Atualiza Alma em jogo
                }

                // Remove Almas com tamanho menor que 0
                _souls = _souls.Where(s => s.scale > 0).ToList();
            }
        }

        public void DeleteSouls()
        {
            lock (_souls)
            {
                _souls.RemoveAll(soul => soul.alive);//Deleta almas em jogo
            }
        }

        public void Draw()
        {
            lock (_souls)
            {
                foreach (var soul in _souls)
                {
                    soul.Draw(); // Desenha Alma em jogo
                }
            }
        }

        // Add any additional methods as needed
    }
}
