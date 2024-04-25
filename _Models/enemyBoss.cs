namespace MyGame;

public class enemyBoss : enemyBase
{
    // Variaveis para Animações
    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _textureIdle_blue, _textureHit_blue, _textureDeath_blue, _textureAttack_blue;  //Spritesheets
    private static Texture2D _textureIdle_red, _textureHit_red, _textureDeath_red, _textureAttack_red;
    private static Texture2D _textureIdle_purple, _textureHit_purple, _textureDeath_purple, _textureAttack_purple;
    private static Texture2D textureIdle, textureAttack, textureDeath, textureHit;
    private static int _colorselect, _spellcount = 0, _totalspellcount = 3;
    private int _thisMageColor;
    private bool _ultimatemage = false;
    public static bool BOSSBATTLE = false;

    public enemyBoss(Vector2 pos)
    {
        _colorselect = (_colorselect % 3) + 1;
        _thisMageColor = _colorselect;
        LoadTextures(); // Metodo com comandos que adicionam as animações
        InitializeAnimations();

        //Define a posição e tamanho do sprite respectivamente
        POSITION = pos;

        scale = 3;

        //Definição inicial de origem e tamanho da caixa de colisão
        basehitboxSize = new(64, 64); // Tamanho
        var frameWidth = _textureIdle_blue.Width / 14; // Dividindo os frames de acordo com tamanho do spritesheet, usando como base a animação de Idle;
        var frameHeight = _textureIdle_blue.Height / 1;
        origin = new(frameWidth / 2, frameHeight / 2); //Atribui o centro do frame X e Y a um vetor

        //Pré definição de atributos de combate e animação para evitar bugs

        HP = 250; // Vida Base
        speed = 0f; //Velocidade de movimento base
        DAMAGE = 10; // Dano dos projeteis


        ATTACKSTATE = false;
        PREATTACKSTATE = false;
        DEATHSTATE = false;
        INVULSTATE = false;
        ENEMYPROJHIT = false;


        enemydataType = 5;
        ALERT = true;
        SPAWN = true;

        if (_thisMageColor == 2 && ProfileManager.enemyProfileType == 1) // Mago Vermelho e Perfil Agressivo
        {
            HP = 350;
            _ultimatemage = true;
        }
        else if (_thisMageColor == 1 && ProfileManager.enemyProfileType == 2)// Mago Azul e Perfil Balanceado
        {
            HP = 350;
            _ultimatemage = true;

        }
        else if (_thisMageColor == 3 && ProfileManager.enemyProfileType == 3)// Mago Roxo e Perfil Evasivo
        {
            HP = 350;
            _ultimatemage = true;
            BOSSBATTLE = true;

        };



    }

    private static void LoadTextures()
    {
        //Atribuindo spritesheets a variaveis de Texture2D
        _textureIdle_blue ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianBlue_idle");
        _textureAttack_blue ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianBlue_attack");
        _textureHit_blue ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianBlue_hurt");
        _textureDeath_blue ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianBlue_death");

        _textureIdle_red ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianRed_idle");
        _textureAttack_red ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianRed_attack");
        _textureHit_red ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianRed_hurt");
        _textureDeath_red ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianRed_death");

        _textureIdle_purple ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianPurple_idle");
        _textureAttack_purple ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianPurple_attack");
        _textureHit_purple ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianPurple_hurt");
        _textureDeath_purple ??= Globals.Content.Load<Texture2D>("Creatures/Boss/mageguardianPurple_death");
    }



    private void InitializeAnimations()
    {
        // Initialize animations based on current `_colorselect`
        switch (_colorselect)
        {
            case 1:
                textureIdle = _textureIdle_blue;
                textureAttack = _textureAttack_blue;
                textureHit = _textureHit_blue;
                textureDeath = _textureDeath_blue;
                break;
            case 2:
                textureIdle = _textureIdle_red;
                textureAttack = _textureAttack_red;
                textureHit = _textureHit_red;
                textureDeath = _textureDeath_red;
                break;
            case 3:
                textureIdle = _textureIdle_purple;
                textureAttack = _textureAttack_purple;
                textureHit = _textureHit_purple;
                textureDeath = _textureDeath_purple;
                break;
            default:
                throw new InvalidOperationException("Unknown color selection");
        }

        _anims.AddAnimation("Idle", new Animation(textureIdle, 14, 1, 0.1f, this, this));
        _anims.AddAnimation("Attack", new Animation(textureAttack, 5, 1, 0.2f, this, this));
        _anims.AddAnimation("Hit", new Animation(textureHit, 5, 1, 0.1f, this, this));
        _anims.AddAnimation("Death", new Animation(textureDeath, 8, 1, 0.1f, this, this));


    }


    //Função de calculo para caixas de colisão
    public override Rectangle GetBounds(string boundType)
    {
        //Limites do topo e da esquerda da caixa de colisão
        int _left = (int)CENTER.X - (int)(basehitboxSize.X * scale) / 2; //Define limites de acordo com centro, parametros da hitbox e sprite
        int _top = (int)CENTER.Y - (int)(basehitboxSize.Y * scale) / 2; //Mesma coisa, porem verticalmente


        int _reactionSize = 180 * scale;  //Tamanho da caixa de colisão

        switch (boundType) //Alterna o tipo de colisão de acordo com o valor passado
        {
            case "hitbox":
                //Caixa de colisão do monstro
                //Com base nas coordenadas _Top e _Left cria um retangulo de tamanho pré-definido multiplicado pelo scale
                return new Rectangle(_left + 48, _top, (int)(basehitboxSize.X * scale) / 2, (int)(basehitboxSize.Y * scale) - 10);
            case "reactionbox":
                //Caixa de colisão para Reação do monstro
                return new Rectangle(0, 0, 0, 0);
            case "attackbox0":
                //Caixa de colisão para o 1º Golpe do monstro
                return new Rectangle(0, 0, 0, 0);
            default:
                //Caso nenhuma caixa de colisão válida seja selecionada
                throw new InvalidOperationException($"Tipo de caixa de colisão desconhecido: {boundType}"); ;
        }

    }

    public override void MapBounds(Point mapSize, Point tileSize) // Calcula bordas do mapa
    {
        _minPos = new((-tileSize.X / 2) - basehitboxSize.X * scale, (-tileSize.Y / 2) + 34); //Limite esquerda e cima (limites minimos)
        _maxPos = new(mapSize.X - (tileSize.X / 2) - CENTER.X - 120, mapSize.Y - (tileSize.X / 2) - CENTER.Y - 110); //Limite direita e baixo (limites minimos)
    }

    public override async Task SetInvulnerableTemporarily(int durationInMilliseconds)
    {
        INVULSTATE = true; // Ativa a ivulnerabilidade

        await Task.Delay(durationInMilliseconds); // Espera X tempo

        INVULSTATE = false; // Desativa após o tempo
    }


    //Variaveis para tempo de recuo do knockback
    float _recoilingtimer = 0f, _recoilingduration = 0.1f;
    private bool _isPreparingAttack = false;
    private float _preAttackTimer = 0f;
    private const float _preAttackDuration = 2.0f; // duration in seconds before casting the spell

    //Recarga entre danos recebidos de projeteis
    float _projHitTimer = 0;

    public override async void Update()
    {

        // Definindo o centro do frame de acordo com a posição atual
        CENTER = POSITION + origin * scale;

        //Marcador de contusão; caso inimigo receba dano ele fica invulneravel e recebe Knockback/Recoiling/Recuo, caso seja durante um pré-ataque ele reinicia a ação.
        if (INVULSTATE)
        {
            Recoling = true; //Recuo se torna verdadeiro
            _recoilingtimer = 0f; // Reinicia a duração do recuo

        }



        //Caso inimigo esteja com vida e esteja em estado de Recoiling/Recuo/Knockback;
        if (Recoling && HP > 0)
        {
            _recoilingtimer += (float)Globals.TotalSeconds; //Por X tempo
            if (_recoilingtimer >= _recoilingduration)
            {
                Recoling = false;//No fim da duração, para de sofrer Recuo.
                _recoilingtimer = 0f;
            }
        }


        //Recarga entre danos recebidos de projeteis
        if (ENEMYPROJHIT)
        {
            _projHitTimer += (float)Globals.TotalSeconds;
            if (_projHitTimer >= 1)
            {
                ENEMYPROJHIT = false;
                _projHitTimer = 0f;
            }
        }


        //Trava para evitar bugs relacionado a ordem de carregamento do jogo
        if (MoveAI != null)
        {
            //Aplica um comportamento ao inimigo definido pelo EnemyManager.cs
            MoveAI.Move(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //Controladores de animação
        if (HP <= 0) //Caso de morte
        {
            if (_thisMageColor == 3) BOSSBATTLE = false;

            _anims.Update("Death");
            await SerializeDataOnDeath();
        }
        else if (PREATTACKSTATE)//Caso de pré-ataque
        {
            _anims.Update("Attack");
        }
        else if (Recoling)//Caso de Dano Recebido
        {
            _anims.Update("Hit");
        }
        else//Caso de parado
        {
            _anims.Update("Idle");
        };
        //////////////////////////////////////////////////////////////////////////////////////////
        // Temporizador de ataques
        if (_isPreparingAttack)
        {
            _preAttackTimer += (float)Globals.TotalSeconds;
            if (_preAttackTimer >= _preAttackDuration)
            {
                _isPreparingAttack = false;
                if (_spellcount > _totalspellcount && _ultimatemage) // Caso o mago em questão seja um Ultimate e o contador de magias lançadas seja maior que uma quantidade....
                {
                    CastUltimate(); // Conjura ultimate
                    _spellcount = 0; // Zera o contador
                }
                else // Caso não
                {
                    CastSpell(); // Conjura magia normal
                    _spellcount++; // Aumenta o contador
                };
            }
        }


        //Caso o inimigo esteja fazendo alguma ação que impede o movimento ele entra em estado de 'actionstate'
        if (INVULSTATE || PREATTACKSTATE || ATTACKSTATE || HP <= 0 || Recoling || DASHSTATE) actionstate = true;
        else actionstate = false; //Caso não esteja em nenhum desses estados ele pode voltar a se mover

        POSITION = Vector2.Clamp(POSITION, _minPos, _maxPos); // não permite que inimigo passe das bordas do mapa

    }

    public void EnterAttackState()
    {
        if (!DEATHSTATE && !_isPreparingAttack)
        {
            PREATTACKSTATE = true;
            _isPreparingAttack = true; // Start the pre-attack timer
            _preAttackTimer = 0f;

        }
    }

    private RandomGenerator _randomGen;
    private void CastSpell() // Magias Básicas
    {
        _randomGen = new RandomGenerator(RandomGenerator.GenerateSeedFromCurrentTime());
        switch (_thisMageColor)
        {
            case 1://Blue
                IceSpell(1);
                IceSpell(2);
                break;
            case 2://Red
                FireSpell(1);
                break;
            case 3://Purple
                DarkSpell();
                break;
            default:
                throw new InvalidOperationException($"Tipo de mago não encontrado! Você está usando numeros de 1 a 3?"); ;
        }
        PREATTACKSTATE = false;

    }

    private void CastUltimate()
    {
        if (_thisMageColor == 1)
        {
            IceUltimate();
        }
        else if (_thisMageColor == 2)
        {
            FireUltimate();
        }
        else if (_thisMageColor == 3)
        {
            DarkUltimate();
        };

        PREATTACKSTATE = false;
        _spellcount = 0;
    }

    private async void IceUltimate()
    {
        IceSpell(1);
        await Task.Delay(200);
        IceSpell(2);
        await Task.Delay(500);
        IceSpell(3);
        await Task.Delay(200);
        IceSpell(4);
        await Task.Delay(500);
        IceSpell(1);
        await Task.Delay(200);
        IceSpell(2);
    }

    private async void FireUltimate()
    {
        FireSpell(1);
        await Task.Delay(350);
        FireSpell(2);
        await Task.Delay(350);
        FireSpell(3);
        await Task.Delay(350);
        FireSpell(4);
        await Task.Delay(350);
        FireSpell(5);
        await Task.Delay(350);
        FireSpell(6);
    }

    private void DarkUltimate()
    {
        DarkSpell(1);
        GameManager.SummonEnemy();
    }

    private void IceSpell(int wave)
    {
        int lifespan = 9;
        float scale = 1.75f;
        int speed = 300;
        int damage = 10;
        bool friendly = false;
        string projectileType = "IceProj";

        Vector2[] directions = new Vector2[8];
        if (wave == 1 || wave == 3)
        {
            directions = new Vector2[]
            {
            new Vector2(1, 0),     // East
            new Vector2(1, 1),     // Northeast
            new Vector2(0, 1),     // North
            new Vector2(-1, 1),    // Northwest
            new Vector2(-1, 0),    // West
            new Vector2(-1, -1),   // Southwest
            new Vector2(0, -1),    // South
            new Vector2(1, -1)     // Southeast
            };
        }
        else
        {
            // Generate intermediate directions by averaging neighboring primary directions
            Vector2[] primaryDirections = new Vector2[]
            {
            new Vector2(1, 0),     // East
            new Vector2(1, 1),     // Northeast
            new Vector2(0, 1),     // North
            new Vector2(-1, 1),    // Northwest
            new Vector2(-1, 0),    // West
            new Vector2(-1, -1),   // Southwest
            new Vector2(0, -1),    // South
            new Vector2(1, -1)     // Southeast
            };

            for (int i = 0; i < primaryDirections.Length; i++)
            {
                // Calculate intermediate direction between current and next (or first if at last index)
                int nextIndex = (i + 1) % primaryDirections.Length;
                Vector2 nextDirection = primaryDirections[nextIndex];
                Vector2 currentDirection = primaryDirections[i];

                // Averaging vectors
                Vector2 intermediateDirection = new Vector2(
                    (currentDirection.X + nextDirection.X) / 2.0f,
                    (currentDirection.Y + nextDirection.Y) / 2.0f
                );

                // Normalize to ensure consistent projectile behavior
                intermediateDirection.Normalize();
                directions[i] = intermediateDirection;
            }
        }

        foreach (Vector2 direction in directions)
        {
            ProjectileData iceProjectile = new()
            {
                Position = CENTER,
                Direction = direction,
                Lifespan = lifespan,
                Homing = false,
                ProjectileType = projectileType,
                Scale = scale,
                Speed = speed,
                Friendly = friendly,
                Damage = damage,
                Wave = wave
            };

            ProjectileManager.AddProjectile(iceProjectile);
        }
    }

    private void DarkSpell(int wave = 0)
    {
        ProjectileData pd1 = new()
        {
            Position = Globals.HEROLASTPOS,
            Direction = Globals.HEROLASTPOS - CENTER,
            Lifespan = 3,
            Homing = false,
            ProjectileType = "DarkSpell",
            Scale = 2f,
            Speed = 0,
            Friendly = false,
            Damage = 10
        };
        ProjectileData pd2 = new()
        {
            Position = Globals.HEROLASTPOS + new Vector2(_randomGen.NextInt(-192, 192), _randomGen.NextInt(-192, 192)),
            Direction = Globals.HEROLASTPOS - CENTER,
            Lifespan = 3,
            Homing = false,
            ProjectileType = "DarkSpell",
            Scale = 2f,
            Speed = 0,
            Friendly = false,
            Damage = 10
        };
        ProjectileData pd3 = new()
        {
            Position = Globals.HEROLASTPOS - new Vector2(_randomGen.NextInt(-128, 192), _randomGen.NextInt(-128, 192)),
            Direction = Globals.HEROLASTPOS - CENTER,
            Lifespan = 3,
            Homing = false,
            ProjectileType = "DarkSpell",
            Scale = 2f,
            Speed = 0,
            Friendly = false,
            Damage = 10
        };
        ProjectileData pd4 = new()
        {
            Position = Globals.HEROLASTPOS + new Vector2(_randomGen.NextInt(-96, 192), _randomGen.NextInt(-96, 192)),
            Direction = Globals.HEROLASTPOS - CENTER,
            Lifespan = 3,
            Homing = false,
            ProjectileType = "DarkSpell",
            Scale = 2f,
            Speed = 0,
            Friendly = false,
            Damage = 10
        };

        ProjectileManager.AddProjectile(pd1);
        ProjectileManager.AddProjectile(pd2);
        ProjectileManager.AddProjectile(pd3);
        ProjectileManager.AddProjectile(pd4);

        if (wave == 0)
        {
            ProjectileData pdh = new() // Magia perseguidora
            {
                Position = CENTER - new Vector2(0,100),
                Direction = Globals.HEROLASTPOS - CENTER,
                Lifespan = 4,
                Homing = true,
                ProjectileType = "DarkProj",
                Scale = 1.75f,
                Speed = 300,
                Friendly = false,
                Damage = DAMAGE
            };
            ProjectileManager.AddProjectile(pdh);
        }

    }

    private RandomGenerator explosionPosOffset;

    private void FireSpell(int wave)
    {
        // Posição base
        Vector2 basePosition = Globals.HEROLASTPOS;

        // Inicializando Gerenciador de aleatoriedade
        explosionPosOffset = new RandomGenerator(RandomGenerator.GenerateSeedFromCurrentTime());

        // Determina a magnetude de aleatoriedade da explosão de acordo com a wave
        int offsetMagnitude = (wave > 1) ? 50 * (wave - 1) : 0;

        // Gera a aleatoreidade caso o offset senha maior que 0, ou seja em todos os casos menos o 1
        Vector2 randomOffset = Vector2.Zero;
        if (offsetMagnitude > 0)
        {
            int randomOffsetX = explosionPosOffset.NextInt(-offsetMagnitude, offsetMagnitude);
            int randomOffsetY = explosionPosOffset.NextInt(-offsetMagnitude, offsetMagnitude);
            randomOffset = new Vector2(randomOffsetX, randomOffsetY);
        }

        // Ajusta a posição com o offset criado
        Vector2 finalPosition = basePosition + randomOffset;

        // Cria e adiciona o projétil na posição
        ProjectileData explosion = new()
        {
            Position = finalPosition,
            Direction = new Vector2(1, 0),
            Lifespan = 1.7f,
            Homing = false,
            ProjectileType = "Explosion",
            Scale = 4,
            Speed = 0,
            Friendly = false,
            Damage = 10,
            Wave = wave
        };
        ProjectileManager.AddProjectile(explosion);
    }

    public override void Draw()
    {
        //hitbox test
        //Rectangle Erect = GetBounds($"hitbox");
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

        //Passa os parametros para o AnimationManager animar o Spritesheet
        _anims.Draw(POSITION, scale, mirror, 0, Color.White);


    }
}