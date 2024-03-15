namespace MyGame
{
    public class GameManager
    {
        //Referencia direta a unidades
        private Hero _hero;
        private Pentagram _pentagram;
        private Soul _soul;
        //Gerenciador de mapa e camera
        private Map _map;
        private Matrix _translation;
        //Referencia aos gerenciadores de jogo
        private CollisionManager _collisionManager;
        private static FadeEffectManager _fadeEffectManager;
        private static Upgrademanager _upgradeManager;
        private static EnemyManager _enemyManager;
        public static EnemyManager EnemyMgr => _enemyManager;

        public void Init()
        {
            _map = new Map(); // Cria o mapa
            _hero = new Hero(new Vector2(1000, 1000)); // Cria o jogador
            _enemyManager = new EnemyManager(_hero, _map); // Passa parametros de mapa e jogador para o gerenciador de inimigos
            _pentagram = new Pentagram(new Vector2(1000, 800)); // Cria o teleportador
            _soul = new Soul(new Vector2(1000, 1300)); // Cria o metodo para upgrades
            _fadeEffectManager = new FadeEffectManager(); // Gerenciador de fade de tela
            _upgradeManager = new Upgrademanager(); // Gerenciador de upgrades
            _collisionManager = new CollisionManager(_hero, _enemyManager.Enemies, _pentagram, _soul); // Gerenciador de colisões
            _hero.MapBounds(_map.MapSize, _map.TileSize); // Limites do mapa

            _enemyManager.SpawnEnemy(new Vector2(1200, 1000), EnemyType.Mage); // Cria os primeiros spawns de inimigos
        }

        public void Update()
        {
            InputManager.Update(); // Atualiza os inputs do jogador

            if (!pause) // Caso não esteja pausado, atualiza todas as unidades do jogo
            {
                Globals.HEROLASTPOS = _hero.CENTER;
                _hero.Update();
                _pentagram.Update();
                if (_pentagram.teleport)
                {
                    _pentagram.teleport = false;
                    Pentagram.enemyCount = 0;
                    initChangeArea();
                }

                //_soul.Update();
                _enemyManager.Update();
                ProjectileManager.Update();
                _collisionManager.CheckCollisions();
                CalculateTranslation();
                _hero._heromatrix = _translation;
            }

            // Ui's
            _fadeEffectManager.Update();
            _upgradeManager.Update();

            //Temporizador de jogo
            if (_isActionScheduled)
            {
                _delayTimer -= (float)Globals.TotalSeconds;
                if (_delayTimer <= 0)
                {
                    _isActionScheduled = false;
                    _delayedAction?.Invoke();
                }
            }
        }

        public void Draw()
        {
            //Desenha todas as unidades no mapa aplicando a diferença de camera
            Globals.SpriteBatch.Begin(transformMatrix: _translation);
            _map.Draw();
            _pentagram.Draw();
            _enemyManager.Draw();
            _hero.Draw();
            ProjectileManager.Draw();
            _upgradeManager.Draw(_hero.CENTER);
            _fadeEffectManager.Draw(_hero.CENTER);
            Globals.SpriteBatch.End();
        }

        ////////////////////////////////////////////////////////////////////////
        // METODOS PARA MENUS/UI
        ////////////////////////////////////////////////////////////////////////

        //Metodo para Pausar o jogo
        static bool pause = false;
        private static DateTime lastPauseTime = DateTime.MinValue;
        public static void PauseGame()
        {
            TimeSpan timeSinceLastPause = DateTime.Now - lastPauseTime;
            if (timeSinceLastPause.TotalSeconds < 0.3)
            {
                return;
            }
            pause = !pause;
            lastPauseTime = DateTime.Now;
            if (pause) _fadeEffectManager.StartFadeOut();
            else _fadeEffectManager.StartFadeIn();
        }

        //Inicio da troca de fase
        public void initChangeArea()
        {
            pause = true;
            _fadeEffectManager.StartFadeOut();
            ScheduleAction(endChangeArea, 0.5f);
        }

        //Metodo para esperar o Script de Python rodar antes de continuar o jogo
        public async Task ExecutePythonScriptAsync()
        {
            await Task.Run(() =>
            {
                PythonBridge.ExecutePythonScript();
            });
        }

        //Metodo para completar a mudança de fase
        public async void endChangeArea()
        {
            if (_pentagram.gamearea > 3) Globals.Exitgame = true;
            else
            {
                _hero.POSITION = new Vector2(1000, 1000);
                await ExecutePythonScriptAsync();
                _enemyManager.DeleteEnemies();
                ProjectileManager.DeleteAll();
                _fadeEffectManager.StartFadeIn();
                pause = false;

                switch (_pentagram.gamearea)
                {
                    case 1:
                        _enemyManager.EnemyBatch(1);
                        break;
                    case 2:
                        _enemyManager.EnemyBatch(2);
                        break;
                    case 3:
                        _enemyManager.EnemyBatch(3);
                        break;
                }
            }
        }

        //Temporizador para ações com atraso
        private Action _delayedAction;
        private float _delayTimer = 0f;
        private bool _isActionScheduled = false;
        public void ScheduleAction(Action action, float delayduration)
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
}