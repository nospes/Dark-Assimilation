namespace MyGame
{
    public class EnemyManager
    {
        // Referência ao Herói e ao Mapa para interação com inimigos e verificação de limites
        private Hero _hero;
        private Map _map;

        // Coleção de todos os inimigos no jogo
        public List<enemyCollection> Enemies { get; private set; } = new List<enemyCollection>();

        // Coleção para inimigos marcados para remoção
        private List<enemyCollection> _enemiesToRemove = new List<enemyCollection>();

        // Construtor para definir as referências do herói e do mapa
        public EnemyManager(Hero hero, Map map)
        {
            _hero = hero;
            _map = map;
        }

        // Atualiza o estado de todos os inimigos, remove aqueles que estão marcados para deleção
        public void Update()
        {
            // Acesso seguro a coleção de inimigos
            lock (Enemies)
            {
                foreach (var enemy in Enemies)
                {
                    enemy.Update();

                    // Marca inimigos para remoção se eles estão mortos ou não devem aparecer
                    if (enemy.DEATHSTATE || !enemy.SPAWN)
                    {
                        _enemiesToRemove.Add(enemy);
                    }
                }

                // Remove inimigos marcados da coleção principal
                foreach (var enemyToRemove in _enemiesToRemove)
                {
                    Enemies.Remove(enemyToRemove);
                }
                _enemiesToRemove.Clear(); // Limpa a lista de remoção após a deleção
            }

            ApplyAvoidance();

        }

        // Desenha todos os inimigos
        public void Draw()
        {
            lock (Enemies)
            {
                foreach (var enemy in Enemies)
                {
                    enemy.Draw(); // Desenha cada inimigo no mapa
                }
            }
        }

        // Engaja inimigos próximos se o jogador estiver dentro de um certo raio
        public void EnemyEngagement(Vector2 engagedCENTER)
        {
            const float engagementRadius = 400; // Raio dentro do qual os inimigos serão alertados
            lock (Enemies)
            {
                foreach (var enemy in Enemies)
                {
                    // Alerta o inimigo se estiver dentro do raio de engajamento e não estiver já engajado
                    if (Vector2.Distance(engagedCENTER, enemy.CENTER) <= engagementRadius && enemy.MoveAI.GetType() == typeof(IdleAI))
                    {
                        enemy.ALERT = true;
                    }
                }
            }
        }

        private const float MinimumDistance = 40f; // Distancia minima entre os inimigos
        private void ApplyAvoidance() // Metodo para evitar que os inimigos todos se agrupem na mesma posição
        {
            lock (Enemies)
            {
                foreach (var enemy1 in Enemies)
                {
                    foreach (var enemy2 in Enemies)
                    {
                        if (enemy1 == enemy2) continue; // Não checa caso seja duas unidades iguais

                        float distance = Vector2.Distance(enemy1.CENTER, enemy2.CENTER);
                        if (distance < MinimumDistance)
                        {
                            Vector2 direction = Vector2.Normalize(enemy1.POSITION - enemy2.POSITION);
                            enemy1.POSITION += direction * 2;
                            enemy2.POSITION -= direction * 2;
                        }
                    }
                }
            }

        }

        // Gera um único inimigo de um tipo especificado em uma posição especificada
        public void SpawnEnemy(Vector2 CENTER, EnemyType type)
        {
            enemyCollection enemy = type switch
            {
                EnemyType.Mage => new enemyMage(CENTER),
                EnemyType.Archer => new enemyArcher(CENTER),
                EnemyType.Swarm => new enemySwarm(CENTER),
                EnemyType.Skeleton => new enemySkeleton(CENTER),
                _ => throw new ArgumentException("Tipo de inimigo inválido", nameof(type)),
            };

            lock (Enemies)
            {
                enemy.ID = Enemies.Count + 1; // Atribui um ID único para cada inimigo
                if (type == EnemyType.Mage || type == EnemyType.Archer)
                {
                    enemy.MoveAI = new IdleAI
                    {
                        target = _hero, // Define o herói como alvo para o inimigo
                        AIenemyType = new DistanceMovementAI { target = _hero } // Especifica o tipo de IA de movimento
                    };
                }
                else
                {
                    enemy.MoveAI = new IdleAI
                    {
                        target = _hero,
                        AIenemyType = new FollowHeroAI { target = _hero } // Aplica um tipo diferente de IA para inimigos não à distância
                    };
                }

                Enemies.Add(enemy); // Adiciona o novo inimigo à coleção
            }
            enemy.MapBounds(_map.MapSize, _map.TileSize); // Garante que os inimigos adiram aos limites do mapa
        }

        // Deleta todos os inimigos do jogo
        public void DeleteEnemies()
        {
            lock (Enemies)
            {
                foreach (var enemy in Enemies)
                {
                    enemy.SPAWN = false; // Marca inimigos como não para gerar
                }
                Enemies.RemoveAll(enemy => !enemy.SPAWN); // Remove todos os inimigos marcados como não para gerar
            }
        }

        // Gera lotes de inimigos com base no tipo de geração
        public void EnemyBatch(int spawnType)
        {
            var pos1 = 363;
            var pos2 = 1525;

            switch (spawnType)
            {
                case 1:
                    SpawnEnemy(new Vector2(pos1, pos1), EnemyType.Skeleton);
                    SpawnEnemy(new Vector2(pos1, pos1), EnemyType.Archer);
                    SpawnEnemy(new Vector2(pos1, pos1), EnemyType.Swarm);

                    SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Skeleton);
                    SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Mage);
                    SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Swarm);

                    SpawnEnemy(new Vector2(pos2, pos1), EnemyType.Mage);
                    SpawnEnemy(new Vector2(pos2, pos1 + 100), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos2 + 75, pos1 + 100), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos2 - 75, pos1 + 100), EnemyType.Swarm);

                    SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Archer);
                    SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Mage);
                    break;
                case 2:
                    SpawnEnemy(new Vector2(pos1, pos1), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos1 - 12, pos1 + 50), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos1 - 73, pos1 + 132), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos1 + 125, pos1 + 50), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos1 + 173, pos1 + 87), EnemyType.Swarm);

                    SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos1 + 100, pos2 + 50), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos1 - 50, pos2 - 50), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Skeleton);

                    SpawnEnemy(new Vector2(pos2, pos1), EnemyType.Archer);
                    SpawnEnemy(new Vector2(pos2 - 50, pos1 + 25), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos2, pos1), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos2 + 100, pos1), EnemyType.Swarm);

                    SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos2 + 100, pos2 - 35), EnemyType.Skeleton);
                    SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Skeleton);
                    break;
                case 3:
                    SpawnEnemy(new Vector2(pos1, pos1), EnemyType.Archer);
                    SpawnEnemy(new Vector2(pos1 + 100, pos1 - 100), EnemyType.Archer);
                    SpawnEnemy(new Vector2(pos1 + 50, pos1), EnemyType.Swarm);


                    SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Mage);
                    SpawnEnemy(new Vector2(pos1 + 100, pos2 - 100), EnemyType.Mage);
                    SpawnEnemy(new Vector2(pos1 + 42, pos2 - 32), EnemyType.Swarm);

                    SpawnEnemy(new Vector2(pos2, pos1), EnemyType.Skeleton);
                    SpawnEnemy(new Vector2(pos2 + 50, pos1 - 50), EnemyType.Archer);
                    SpawnEnemy(new Vector2(pos2 + 42, pos1 - 32), EnemyType.Swarm);
                    SpawnEnemy(new Vector2(pos2 - 42, pos1 + 32), EnemyType.Swarm);

                    SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Skeleton);
                    SpawnEnemy(new Vector2(pos2 + 50, pos2 - 50), EnemyType.Archer);
                    SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Mage);
                    break;
            }
        }
    }
}
