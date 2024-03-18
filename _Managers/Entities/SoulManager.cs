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
            // Define posições pré-determinadas
            var positions = new List<Vector2>
            {
                new Vector2(363, 363),
                new Vector2(363, 1525),
                new Vector2(1525, 363),
                new Vector2(1525, 1525)
            };

            var random = new RandomGenerator(RandomGenerator.GenerateSeedFromCurrentTime());

            // Seleciona aleatoriamente posições para soul1_pos
            int index1 = random.NextInt(0, positions.Count);
            Vector2 soul1_pos = positions[index1];

            // Remove a posição escolhida para garantir que não haja duplicação
            positions.RemoveAt(index1);

            // Seleciona aleatoriamente a posição da soul2_pos
            int index2 = random.NextInt(0, positions.Count);
            Vector2 soul2_pos = positions[index2];

            // Adiciona as almas nas posições determinadas
            lock (_souls)
            {
                _souls.Add(new Soul(soul1_pos)); // Adiciona uma alma na primeira posição
                _souls.Add(new Soul(soul2_pos)); // Adiciona uma alma na segunda posição
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

                // Remove Almas com tamanho maior que 0
                _souls = _souls.Where(s => s.scale > 0).ToList();
            }
        }

        public void DeleteSouls() 
        {
            lock (_souls)
            {
                _souls.RemoveAll(soul => soul.alive);//Delete almas em jogo
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
