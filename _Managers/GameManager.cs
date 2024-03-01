namespace MyGame;

public class GameManager
{
    //Em geral esse código serve apenas para adicionar, remover e atualizar coisas dentro do jogo
    //Suas funções são diretamente chamadas pelo update do Game1.cs

    private static Hero _hero;
    private Map _map;
    private static List<enemyCollection> inimigos = new List<enemyCollection>();
    private CollisionManager _collisionManager;
    private Matrix _translation;

    public enum EnemyType
    {
        Mage,
        Archer,
        Swarm,
        Skeleton
    }

    public void Init()
    {
        //Cria o mapa
        _map = new();

        //Cria o heroi
        _hero = new(new(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2));

        //Cria o inimigo
        //Spawn 1
        SpawnEnemy(new Vector2(160, 99), EnemyType.Skeleton);
        SpawnEnemy(new Vector2(249, 99), EnemyType.Archer);

        //Spawn 2
        SpawnEnemy(new Vector2(1209, 250), EnemyType.Mage);
        SpawnEnemy(new Vector2(1209, 292), EnemyType.Swarm);
        SpawnEnemy(new Vector2(1259, 292), EnemyType.Swarm);

        //Spawn 3
        SpawnEnemy(new Vector2(370, 1500), EnemyType.Swarm);
        SpawnEnemy(new Vector2(340, 1500), EnemyType.Swarm);
        SpawnEnemy(new Vector2(310, 1500), EnemyType.Swarm);


        //Spawn 4
        SpawnEnemy(new Vector2(1453, 800), EnemyType.Skeleton);
        SpawnEnemy(new Vector2(1453, 850), EnemyType.Mage);

        //spawn 5
        SpawnEnemy(new Vector2(319, 650), EnemyType.Archer);
        SpawnEnemy(new Vector2(319, 600), EnemyType.Mage);



        SpawnEnemy(new Vector2(280, 1800), EnemyType.Skeleton);
        SpawnEnemy(new Vector2(600, 1700), EnemyType.Mage);
        SpawnEnemy(new Vector2(900, 1800), EnemyType.Archer);
        SpawnEnemy(new Vector2(1200, 1800), EnemyType.Swarm);




        _collisionManager = new CollisionManager(_hero, inimigos); //Cria gerenciador de colisões entre inimigos e jogador
        _hero.MapBounds(_map.MapSize, _map.TileSize); //Atrela o Heroi aos limites do mapa
        foreach (var inimigo in inimigos) //Atrela os inimigos aos limites do mapa
        {
            inimigo.MapBounds(_map.MapSize, _map.TileSize);
        }


    }

    private void CalculateTranslation() // Gerenciador da camera
    {
        var dx = (Globals.WindowSize.X / 2) - _hero.CENTER.X;
        dx = MathHelper.Clamp(dx, -_map.MapSize.X + Globals.WindowSize.X + (_map.TileSize.X / 2), _map.TileSize.X / 2);
        var dy = (Globals.WindowSize.Y / 2) - _hero.CENTER.Y;
        dy = MathHelper.Clamp(dy, -_map.MapSize.Y + Globals.WindowSize.Y + (_map.TileSize.Y / 2), _map.TileSize.Y / 2);
        _translation = Matrix.CreateTranslation(dx, dy, 0f);

    }

    //Alerta inimigos próximos fazendo-os entrar em combate tambem.
    public static void EnemyEngagement(Vector2 engagedPosition)
    {
        const float engagementRadius = 400; // Define o alcance alertar outros inimigos
        foreach (var enemy in inimigos)
        {
            if (Vector2.Distance(engagedPosition, enemy.CENTER) <= engagementRadius)
            {
                // Checa se o inimigo já não está em combate
                if (enemy.MoveAI.GetType() == typeof(IdleAI))
                {
                    enemy.ALERT = true;
                    //if(enemy.GetType() == typeof(enemyArcher) || enemy.GetType() == typeof(enemyMage)) enemy.MoveAI = new DistanceMovementAI { target = _hero }; // Switch to aggressive AI, implement AggressiveAI accordingly
                    //else enemy.MoveAI = new FollowHeroAI{ target = _hero };
                }
            }
        }
    }

    private void SpawnEnemy(Vector2 position, EnemyType type)
    {
        enemyCollection enemy = type switch
        {
            EnemyType.Mage => new enemyMage(position),
            EnemyType.Archer => new enemyArcher(position),
            EnemyType.Swarm => new enemySwarm(position),
            EnemyType.Skeleton => new enemySkeleton(position),
            _ => throw new ArgumentException("Invalid enemy type", nameof(type)),
        };

        enemy.ID = inimigos.Count + 1; // Example of setting ID, adjust as needed
        if (type == EnemyType.Mage || type == EnemyType.Archer)
        {
            enemy.MoveAI = new IdleAI
            {
                target = _hero,
                AIenemyType = new DistanceMovementAI { target = _hero }
            };
        }
        else
        {
            enemy.MoveAI = new IdleAI
            {
                target = _hero,
                AIenemyType = new FollowHeroAI { target = _hero }
            };
        }


        inimigos.Add(enemy);
        enemy.MapBounds(_map.MapSize, _map.TileSize); // Assuming this needs to be done for all enemies
    }

    static bool enemyLock = false;

    public static void EnemySpawnlock()
    {
        enemyLock = true;

    }


    public void Update()
    {
        Globals.HEROLASTPOS = _hero.CENTER;
        InputManager.Update(); //Atualiza os botões
        _hero.Update(); //Atualiza os herois
        List<enemyCollection> inimigosParaRemover = new List<enemyCollection>(); //Atualiza a lista de inimigos mortos
        foreach (var inimigo in inimigos) //Para cada inimigo...
        {
            inimigo.Update(); //Atualiza

            if (inimigo.DEATHSTATE)
            {
                inimigosParaRemover.Add(inimigo); //Se esta morto adiciona a lista de inimigos derrotados
            }
        }

        foreach (var inimigoParaRemover in inimigosParaRemover) //Para cada inimigo derrotado...
        {
            inimigos.Remove(inimigoParaRemover); //Remova o inimigo
        }

        ProjectileManager.Update();
        _collisionManager.CheckCollisions(); //Checa as colisões
        CalculateTranslation(); //Atualiza a posição da camera
        _hero._heromatrix = _translation;

        if (enemyLock)
        {
            enemyLock = false;
            SpawnEnemy(new Vector2(1050, 1000), EnemyType.Swarm);
        }

    }

    public void Draw()
    {

        Globals.SpriteBatch.Begin(transformMatrix: _translation); //Cria os sprites dentro dos limites do mapa
        _map.Draw(); //Desenha o mapa
        foreach (var inimigo in inimigos)
        {
            inimigo.Draw(); //Para cada inimigo no mapa, ele é desenhado
        }
        _hero.Draw(); //Desenha o heroi
        ProjectileManager.Draw();
        Globals.SpriteBatch.End();
    }
}