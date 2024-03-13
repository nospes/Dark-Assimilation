namespace MyGame;

public class GameManager
{
    //Em geral esse código serve apenas para adicionar, remover e atualizar coisas dentro do jogo
    //Suas funções são diretamente chamadas pelo update do Game1.cs

    private static Hero _hero;
    private static Pentagram _pentagram;
    private static Soul _soul;
    private Map _map;
    private static List<enemyCollection> inimigos = new List<enemyCollection>();
    List<enemyCollection> inimigosParaRemover = new List<enemyCollection>();
    private CollisionManager _collisionManager;
    private Matrix _translation;
    private static FadeEffectManager _fadeEffectManager;
    private static Upgrademanager _upgradeManager;

    public enum EnemyType // Definindo os tipos de inimigos com ENUM
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
        _hero = new(new(1000, 1000));

        //Cria o inimigo
        enemyBatch(1);

        //Cria portal para transição de fase
        _pentagram = new(new(1000, 800));

        //Criar seletor de upgrades
        _soul = new(new(1000, 1300));


        _fadeEffectManager = new FadeEffectManager(); // Inicia o gerenciador de transição
        _upgradeManager = new Upgrademanager(); // Inicia o gerenciador de Upgrade
        _collisionManager = new CollisionManager(_hero, inimigos, _pentagram, _soul); //Cria gerenciador de colisões entre inimigos e jogador
        _hero.MapBounds(_map.MapSize, _map.TileSize); //Atrela o Heroi aos limites do mapa


    }


    public void Update()
    {
        InputManager.Update(); //Atualiza os botões


        ///////////////////////////////////////////////////////////////////////////////
        // LOGICA PARA UNIDADES/PROJETEIS/OBJETOS
        ///////////////////////////////////////////////////////////////////////////////

        if (!pause) //Caso o jogo não esteja pausado roda os seguintes metodos...
        {
            //Todos as unidades do jogo ficam dentro desta condição

            Globals.HEROLASTPOS = _hero.CENTER;

            _hero.Update(); //Atualiza os herois

            _pentagram.Update(); // Atualiza o teleportador
            if (_pentagram.teleport)
            {
                _pentagram.teleport = false;
                Pentagram.enemyCount = 0;
                initChangeArea();
            }

            _soul.Update(); //Atualiza o Seletor de Upgrades

            lock (inimigos) // Aplicando LOCK quando acessar a lista para evitar problemas de acessos simultaneos a ela
            {
                foreach (var inimigo in inimigos) //Para cada inimigo...
                {
                    inimigo.Update(); //Atualiza

                    if (inimigo.DEATHSTATE || !inimigo.SPAWN)
                    {
                        inimigosParaRemover.Add(inimigo); //adiciona a lista de inimigos para remover
                    }

                }
            }

            lock (inimigos)
            {
                foreach (var inimigoParaRemover in inimigosParaRemover) //Para cada inimigo na lista de removido...
                {
                    inimigos.Remove(inimigoParaRemover); //Remova o inimigo
                }
            }

            ProjectileManager.Update();//Atualiza os updates
            _collisionManager.CheckCollisions(); //Checa as colisões
            CalculateTranslation(); //Atualiza a posição da camera
            _hero._heromatrix = _translation; //Movimentação da camera usando posição do heroi

        }

        ///////////////////////////////////////////////////////////////////////////////
        //Lógica para MENUS e CARREGAMENTOS
        ///////////////////////////////////////////////////////////////////////////////
        _fadeEffectManager.Update(); // Fadein e Fadeout da tela
        _upgradeManager.Update(); // 

        if (_isActionScheduled) // Temporizador de ações
        {
            _delayTimer -= (float)Globals.TotalSeconds;

            if (_delayTimer <= 0)
            {
                _isActionScheduled = false;
                _delayedAction?.Invoke(); // Executa o metodo passado
            }
        }
        ///////////////////////////////////////////////////////////////////////////////
    }

    public void Draw()
    {

        Globals.SpriteBatch.Begin(transformMatrix: _translation); //Cria os sprites dentro dos limites do mapa
        _map.Draw(); //Desenha o mapa
        _pentagram.Draw(); // Desenha o teleportador debaixo de todas as unidades, mas acima do mapa

        lock (inimigos)
        {
            foreach (var inimigo in inimigos)
            {
                inimigo.Draw(); //Para cada inimigo no mapa, ele é desenhado
            }
        }
        _hero.Draw(); //Desenha o heroi
        //_soul.Draw(); // Desenha o seletor de upgrades

        ProjectileManager.Draw(); //Desenha projeteis
        _upgradeManager.Draw(_hero.CENTER); // Desenha janela de upgrades
        _fadeEffectManager.Draw(_hero.CENTER); //Desenha o fade de pause
        Globals.SpriteBatch.End(); //Termina o spritebatch
    }

    ///////////////////////////////////////////////////////////////////////////////////////
    // METODOS PARA INIMIGOS
    ///////////////////////////////////////////////////////////////////////////////////////

    //Alerta inimigos próximos fazendo-os entrar em combate tambem.
    public static void EnemyEngagement(Vector2 engagedPosition)
    {
        const float engagementRadius = 400; // Define o alcance alertar outros inimigos
        lock (inimigos)
        {
            foreach (var enemy in inimigos)
            {
                if (Vector2.Distance(engagedPosition, enemy.CENTER) <= engagementRadius)
                {
                    // Checa se o inimigo já não está em combate
                    if (enemy.MoveAI.GetType() == typeof(IdleAI))
                    {
                        enemy.ALERT = true;

                    }
                }
            }
        }
    }

    // Metodo para invocar inimigos
    private void SpawnEnemy(Vector2 position, EnemyType type)
    {
        enemyCollection enemy = type switch // Define todos os possiveis inimigos uasndo ENUM
        {
            EnemyType.Mage => new enemyMage(position),
            EnemyType.Archer => new enemyArcher(position),
            EnemyType.Swarm => new enemySwarm(position),
            EnemyType.Skeleton => new enemySkeleton(position),
            _ => throw new ArgumentException("Invalid enemy type", nameof(type)),
        };

        lock (inimigos) enemy.ID = inimigos.Count + 1; //Coloca um ID em cada inimigo
        if (type == EnemyType.Mage || type == EnemyType.Archer) // Caso inimigo seja desse tipo
        {
            enemy.MoveAI = new IdleAI
            {
                target = _hero, //Marca o heroi como alvo
                AIenemyType = new DistanceMovementAI { target = _hero } // Aplica este comportamento a ele
            };
        }
        else // Caso não
        {
            enemy.MoveAI = new IdleAI
            {
                target = _hero,
                AIenemyType = new FollowHeroAI { target = _hero } // Aplica este
            };
        }

        lock (inimigos) inimigos.Add(enemy);
        enemy.MapBounds(_map.MapSize, _map.TileSize); // Aplicando limites do mapa nos inimigos


    }

    //Deleta inimigos
    public static void DeleteEnemies()
    {
        lock (inimigos)
        {
            foreach (var inimigo in inimigos)
            {
                inimigo.SPAWN = false;
            }
            inimigos.RemoveAll(inimigo => !inimigo.SPAWN);
        }
    }

    //Metodo para invocar hordas de inimigos
    public void enemyBatch(int spawnType)
    {
        var pos1 = 363;
        var pos2 = 1525;

        switch (spawnType)
        {
            case 1:
                SpawnEnemy(new Vector2(pos1, pos1), EnemyType.Skeleton);
                SpawnEnemy(new Vector2(pos1, pos1), EnemyType.Archer);

                SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Skeleton);
                SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Mage);

                SpawnEnemy(new Vector2(pos2, pos1), EnemyType.Mage);
                SpawnEnemy(new Vector2(pos2, pos1+100), EnemyType.Swarm);
                SpawnEnemy(new Vector2(pos2+75, pos1+100), EnemyType.Swarm);

                SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Archer);
                SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Mage);
                break;
            case 2:
                SpawnEnemy(new Vector2(pos1, pos1), EnemyType.Swarm);
                SpawnEnemy(new Vector2(pos1-50, pos1+50), EnemyType.Swarm);
                SpawnEnemy(new Vector2(pos1-100, pos1+150), EnemyType.Swarm);
                SpawnEnemy(new Vector2(pos1+100, pos1+50), EnemyType.Swarm);
                SpawnEnemy(new Vector2(pos1+200, pos1+100), EnemyType.Swarm);

                SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Swarm);
                SpawnEnemy(new Vector2(pos1+100, pos2+50), EnemyType.Swarm);
                SpawnEnemy(new Vector2(pos1-50, pos2-50), EnemyType.Swarm);
                SpawnEnemy(new Vector2(pos1, pos2), EnemyType.Skeleton);

                SpawnEnemy(new Vector2(pos2, pos1), EnemyType.Archer);
                SpawnEnemy(new Vector2(pos2+100, pos1), EnemyType.Archer);
                SpawnEnemy(new Vector2(pos2, pos1), EnemyType.Swarm);
                SpawnEnemy(new Vector2(pos2+100, pos1), EnemyType.Swarm);

                SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Mage);
                SpawnEnemy(new Vector2(pos2+100, pos2), EnemyType.Mage);
                SpawnEnemy(new Vector2(pos2, pos2), EnemyType.Skeleton);
                break;
        }

    }

    ////////////////////////////////////////////////////////////////////////
    // METODOS PARA MENUS/UI
    ////////////////////////////////////////////////////////////////////////
    // Pause do jogo
    static bool pause = false; // Variavel para pausar
    private static DateTime lastPauseTime = DateTime.MinValue; //Variavel para marcar o tempo do pause
    public static void PauseGame() //Metodo para pausar o jogo
    {
        TimeSpan timeSinceLastPause = DateTime.Now - lastPauseTime; // Checa o ultimo pause feito
        // Se é menor que o tempo minimo, ele não pausa/despausa
        if (timeSinceLastPause.TotalSeconds < 0.3)
        {
            return;
        }
        pause = !pause; // Liga/Desliga o pause
        lastPauseTime = DateTime.Now;  // Salva o tempo do ultimo pause
        if (pause) _fadeEffectManager.StartFadeOut();
        else _fadeEffectManager.StartFadeIn();
    }

    //Troca de Área inicio, espera completar o Fadeout da tela antes de fazer as ações
    public void initChangeArea()
    {
        pause = true;
        _fadeEffectManager.StartFadeOut();
        ScheduleAction(endChangeArea, 0.5f);
    }

    //Metodo para esperar o Python terminar de fazer suas predições antes de continuar o jogo.
    public async Task ExecutePythonScriptAsync()
    {
        await Task.Run(() =>
        {
            PythonBridge.ExecutePythonScript();
        });
    }

    //Troca de área final, começa fazendo as predições e então ao terminar elas despausa o jogo.
    public async void endChangeArea()
    {
        if (_pentagram.gamearea > 1) Globals.Exitgame = true;
        else
        {
            _hero.POSITION = new Vector2(1000, 1000);
            await ExecutePythonScriptAsync();
            DeleteEnemies();
            enemyBatch(2);
            _fadeEffectManager.StartFadeIn();
            pause = false;
        }
    }

    //Temporizador para troca de cenas
    private Action _delayedAction; // Ação
    private float _delayTimer = 0f; // Contador de tempo
    private bool _isActionScheduled = false; // Variavel de controle
    public void ScheduleAction(Action action, float delayduration) // Metodo para criar temporizadores
    {
        _delayedAction = action;
        _delayTimer = delayduration;
        _isActionScheduled = true;
    }

    // Gerenciador da camera
    private void CalculateTranslation()
    {
        var dx = (Globals.WindowSize.X / 2) - _hero.CENTER.X;
        dx = MathHelper.Clamp(dx, -_map.MapSize.X + Globals.WindowSize.X + (_map.TileSize.X / 2), _map.TileSize.X / 2);
        var dy = (Globals.WindowSize.Y / 2) - _hero.CENTER.Y;
        dy = MathHelper.Clamp(dy, -_map.MapSize.Y + Globals.WindowSize.Y + (_map.TileSize.Y / 2), _map.TileSize.Y / 2);
        _translation = Matrix.CreateTranslation(dx, dy, 0f);

    }

    ////////////////////////////////////////////////////////////////////////////////
}