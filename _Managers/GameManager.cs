namespace MyGame
{
    public class GameManager
    {
        //Referencia direta a unidades
        private Hero _hero;
        private Pentagram _pentagram;
        private SoulManager _soulManager;
        //Gerenciador de mapa e camera
        private Map _map;
        private Matrix _translation;
        //Referencia aos gerenciadores de jogo
        private CollisionManager _collisionManager;
        private static FadeEffectManager _fadeEffectManager;
        private static EnemyManager _enemyManager;
        public static EnemyManager EnemyMgr => _enemyManager;

        public void Init()
        {
            ProjectileManager.DeleteAll();

            _map = new Map(); // Cria o mapa

            _hero = new Hero(new Vector2(1000, 1000)); // Cria o jogador
            _enemyManager = new EnemyManager(_hero, _map); // Passa parametros de mapa e jogador para o gerenciador de inimigos

            _pentagram = new Pentagram(new Vector2(1000, 800)); // Cria o teleportador

            _soulManager = new SoulManager();
            UpgradeManagerUI.UpgradeHero.Init(_hero);

            _fadeEffectManager = new FadeEffectManager(); // Gerenciador de fade de tela
            _collisionManager = new CollisionManager(_hero, _enemyManager.Enemies, _pentagram, _soulManager._souls); // Gerenciador de colisões

            _hero.MapBounds(_map.MapSize, _map.TileSize); // Limites do mapa

            _enemyManager.EnemyBatch(0); // Inimigos do estagio 0
            _soulManager.AddPrePositionSouls(); // Adiciona almas no mapa

        }

        public void Update()
        {
            InputManager.Update(); // Atualiza os inputs do jogador

            if (!PAUSE) // Caso não esteja pausado, atualiza todas as unidades do jogo
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

                _soulManager.Update();

                _enemyManager.Update();
                ProjectileManager.Update();
                _collisionManager.CheckCollisions();

                CalculateTranslation();
                _hero._heromatrix = _translation;

                //if(GAMEOVER || !Game1.GAMESTART) PauseGame();
            }

            // Ui's
            _fadeEffectManager.Update();

            //Temporizador para ações atrasadas
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

            //Props
            _pentagram.Draw();
            _soulManager.Draw();

            //Unidades
            _enemyManager.Draw();
            _hero.Draw();
            ProjectileManager.Draw();

            //UI
            _fadeEffectManager.Draw(_hero.CENTER);
            Globals.SpriteBatch.End();
        }

        ////////////////////////////////////////////////////////////////////////
        // METODOS PARA MENUS/UI
        ////////////////////////////////////////////////////////////////////////

        //Metodo para Pausar o jogo
        public static bool PAUSE = false, GAMEOVER = false;
        private static DateTime lastPauseTime = DateTime.MinValue;
        public static void PauseGame()
        {
            TimeSpan timeSinceLastPause = DateTime.Now - lastPauseTime;
            if (timeSinceLastPause.TotalSeconds < 0.3)
            {
                return;
            }
            PAUSE = !PAUSE;
            lastPauseTime = DateTime.Now;
            if (PAUSE) _fadeEffectManager.StartFadeOut();
            else _fadeEffectManager.StartFadeIn();
        }

        //Inicio da troca de fase
        public void initChangeArea()
        {
            PAUSE = true;
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
            if (_pentagram.gamearea > 2)// Termina o jogo ao chegar em certa fase
            {
                _fadeEffectManager.StartFadeOut();
                GameManager.GAMEOVER = true;
            }
            else
            {
                _hero.POSITION = new Vector2(1000, 1000); // Arruma a posição do heroi
                _hero.HP += _hero.hpRegen; // Regenera Hp do heroi

                if (_pentagram.gamearea > 1)
                {
                    await ExecutePythonScriptAsync(); // Executa a predição de perfil
                    ProfileManager.UpdateEnemyProfileType(); // Seleciona o perfil de acordo com as contagens
                }
                _enemyManager.DeleteEnemies(); // Deleta os inimigos
                ProjectileManager.DeleteAll(); // Deleta os projéteis
                _soulManager.DeleteSouls(); // Delete as almas

                _soulManager.AddPrePositionSouls(); // Adiciona almas da fase

                switch (_pentagram.gamearea) // Adiciona leva de inimigos de acordo com a fase
                {
                    case 1:
                        _enemyManager.EnemyBatch(1);
                        break;
                    case 2:
                        switch (ProfileManager.enemyProfileType)
                        {
                            case 1:
                                _enemyManager.EnemyBatch(2);
                                break;
                            case 2:
                                _enemyManager.EnemyBatch(3);
                                break;
                            case 3:
                                _enemyManager.EnemyBatch(4);
                                break;
                        }
                        break;
                    case 3:
                        switch (ProfileManager.enemyProfileType)
                        {
                            case 1:
                                _enemyManager.EnemyBatch(3);
                                break;
                            case 2:
                                _enemyManager.EnemyBatch(4);
                                break;
                            case 3:
                                _enemyManager.EnemyBatch(2);
                                break;
                        }
                        break;
                }



                //Atualiza o gerenciador de colisões para as novas unidades
                _collisionManager = new CollisionManager(_hero, _enemyManager.Enemies, _pentagram, _soulManager._souls);

                _fadeEffectManager.StartFadeIn(); // Tira a camada de transição da tela
                PAUSE = false; // Despausa o jogo
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