using System.Linq;
using System.Collections.Concurrent;

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

        // Intervalo de ataque dos chefes
        private float attackStateTimer = 0f;
        public static float attackInterval;
        private bool summoningEnemy = false;

        // Construtor para definir as referências do herói e do mapa
        public EnemyManager(Hero hero, Map map)
        {
            _hero = hero;
            _map = map;
            attackInterval = 3f;
        }

        // Atualiza o estado de todos os inimigos, remove aqueles que estão marcados para deleção
        public void Update()
        {

            attackStateTimer += (float)Globals.TotalSeconds;

            if (attackStateTimer >= attackInterval)
            {
                TriggerRandomBossAttack();
                attackStateTimer = 0; // Reset the timer after triggering attack
            }

            // Acesso seguro a coleção de inimigos
            lock (Enemies)
            {
                if (summoningEnemy)
                {
                    var rand = new Random();
                    var choice = rand.Next(0,2);
                    if(choice == 0)SpawnEnemy(new Vector2(698, 900), EnemyType.Swarm);
                    else SpawnEnemy(new Vector2(1275, 900), EnemyType.Swarm);
                    
                    summoningEnemy = false;
                }
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

                ApplyAvoidance();
            }



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
                        if (enemy1 == enemy2 || enemy1 is enemyBoss) break; // Não checa caso seja duas unidades iguais
                        if (enemy1 is enemySwarm && enemy1.ATTACKSTATE) break;

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
            lock (Enemies)
            {
                enemyCollection enemy = type switch
                {
                    EnemyType.Mage => new enemyMage(CENTER),
                    EnemyType.Archer => new enemyArcher(CENTER),
                    EnemyType.Swarm => new enemySwarm(CENTER),
                    EnemyType.Skeleton => new enemySkeleton(CENTER),
                    EnemyType.Boss => new enemyBoss(CENTER),
                    _ => throw new ArgumentException("Tipo de inimigo inválido", nameof(type)),
                };


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

                enemy.MapBounds(_map.MapSize, _map.TileSize); // Garante que os inimigos adiram aos limites do mapa
            }
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

        private enemyBoss _lastAttackedBoss = null;
        private RandomGenerator attackChance;
        private void TriggerRandomBossAttack()
        {
            var bosses = Enemies.OfType<enemyBoss>().ToList();
            enemyBoss randomBoss = null;

            // Filter out the last attacked boss if there are multiple bosses available
            if (bosses.Count > 1 && _lastAttackedBoss != null)
            {
                bosses.Remove(_lastAttackedBoss);
            }

            if (bosses.Any())
            {
                attackChance = new RandomGenerator(RandomGenerator.GenerateSeedFromCurrentTime());
                randomBoss = bosses[attackChance.NextInt(0, bosses.Count)];
                randomBoss.EnterAttackState(); // Trigger the attack
                _lastAttackedBoss = randomBoss; // Update the last attacked boss
            }
        }


        // Gera lotes de inimigos com base no tipo de geração
        public void EnemyBatch(int spawnType)
        {
            lock (Enemies)
            {
                var pos1 = 363;
                var pos2 = 1525;
                var pos3 = new Vector2(1000, 300);

                switch (spawnType)
                {
                    case 0: // Fase 1
                        SpawnEnemy(pos3, EnemyType.Skeleton);
                        SpawnEnemy(pos3, EnemyType.Swarm);

                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos1 - 100, pos2 - 100), EnemyType.Mage);

                        SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Archer);
                        SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Swarm);
                        break;
                    case 1: // Fase 2
                        SpawnEnemy(pos3, EnemyType.Skeleton);
                        SpawnEnemy(pos3, EnemyType.Archer);
                        SpawnEnemy(pos3, EnemyType.Swarm);

                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Skeleton);
                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Mage);
                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Swarm);

                        SpawnEnemy(new Vector2(pos2 - 100, pos2 - 100), EnemyType.Mage);
                        SpawnEnemy(new Vector2(pos2, pos2 + 100), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2 + 75, pos2 + 100), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2 - 75, pos2 + 100), EnemyType.Swarm);
                        break;
                    case 2: // Variação 1
                        SpawnEnemy(pos3, EnemyType.Archer);
                        SpawnEnemy(pos3, EnemyType.Skeleton);
                        SpawnEnemy(new Vector2(pos3.X + 50, pos3.Y + 25), EnemyType.Archer);

                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Skeleton);
                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Mage);
                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos1 + 50, pos2 + 25), EnemyType.Swarm);

                        SpawnEnemy(new Vector2(pos2 + 40, pos2), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2 - 40, pos2), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2, pos2 + 40), EnemyType.Archer);
                        SpawnEnemy(new Vector2(pos2 - 100, pos2 - 100), EnemyType.Mage);
                        break;
                    case 3: // Variação 2
                        SpawnEnemy(pos3, EnemyType.Skeleton);
                        SpawnEnemy(pos3, EnemyType.Mage);
                        SpawnEnemy(new Vector2(pos3.X + 50, pos3.Y + 25), EnemyType.Skeleton);

                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Skeleton);
                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos1 - 50, pos2 - 50), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos1 - 100, pos2 - 100), EnemyType.Mage);

                        SpawnEnemy(new Vector2(pos2 - 100, pos2 - 100), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2 - 50, pos2 - 50), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Skeleton);
                        break;
                    case 4: // Variação 3
                        SpawnEnemy(pos3, EnemyType.Skeleton);
                        SpawnEnemy(pos3, EnemyType.Archer);
                        SpawnEnemy(pos3, EnemyType.Mage);

                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Archer);
                        SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Mage);
                        SpawnEnemy(new Vector2(pos1 + 25, pos2), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos1 - 25, pos2), EnemyType.Swarm);

                        SpawnEnemy(new Vector2(pos2 + 100, pos2), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2 + 75, pos2 + 100), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2 - 75, pos2 + 100), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2 + 75, pos2 - 100), EnemyType.Swarm);
                        SpawnEnemy(new Vector2(pos2 - 75, pos2 - 100), EnemyType.Swarm);
                        break;
                    case 5:
                        SpawnEnemy(new Vector2(698, 1037), EnemyType.Boss);
                        SpawnEnemy(new Vector2(1275, 1037), EnemyType.Boss);
                        SpawnEnemy(new Vector2(985, 556), EnemyType.Boss);
                        break;
                }
            }
        }

        public void SummonEnemy()
        {
            summoningEnemy = true;
        }
    }

}