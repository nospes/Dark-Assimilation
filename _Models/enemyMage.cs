namespace MyGame;

public class enemyMage : enemyBase
{
    // Variaveis para Animações
    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _textureIdle, _textureHit, _textureWalk, _textureDeath, _textureAttack, _texturePreattack;  //Spritesheets

    private RandomGenerator spellChance;
    private int castSpellChance;

    public enemyMage(Vector2 pos)
    {
        //Atribuindo spritesheets a variaveis de Texture2D
        _textureIdle ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/necro_Idle");
        _textureHit ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/necro_Hit");
        _textureWalk ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/necro_Walk");
        _textureDeath ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/necro_Death");
        _texturePreattack ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/necro_Precast");
        _textureAttack ??= Globals.Content.Load<Texture2D>("Creatures/Necromancer/necro_Attack");


        //Definindo area,frames dos sprites sheets para fazer a animação
        _anims.AddAnimation("necro_Idle", new(_textureIdle, 8, 1, 0.1f, this, this));
        _anims.AddAnimation("necro_Hit", new(_textureHit, 5, 1, 0.1f, this, this));
        _anims.AddAnimation("necro_Walk", new(_textureWalk, 8, 1, 0.1f, this, this));
        _anims.AddAnimation("necro_Death", new(_textureDeath, 8, 1, 0.1f, this, this));
        _anims.AddAnimation("necro_Preattack", new(_texturePreattack, 5, 1, 0.1f, this, this));
        _anims.AddAnimation("necro_Attack", new(_textureAttack, 11, 1, 0.15f, this, this));

        //Define a posição, velocidade e tamanho do sprite respectivamente
        POSITION = pos;
        scale = 3;

        //Definição inicial de origem e tamanho da caixa de colisão
        basehitboxSize = new(50, 52); // Tamanho
        var frameWidth = _textureIdle.Width / 8; // Dividindo os frames de acordo com tamanho do spritesheet, usando como base a animação de Idle;
        var frameHeight = _textureIdle.Height / 1;
        origin = new(frameWidth / 2, frameHeight / 2); //Atribui o centro do frame X e Y a um vetor

        //Pré definição de atributos de combate e animação para evitar bugs
        HP = 110;
        speed = 100f;
        DAMAGE = 10; // Dano do projétil
        _preattackcdduration = 1.25f; // CD entre ataques
        castSpellChance = 50; // Tem 50% de chance de utilizar SLOW(dano fixo) caso contrário usa DARKPROJ

        DASHSTATE = false;
        ATTACKSTATE = false;
        PREATTACKSTATE = false;
        DEATHSTATE = false;
        INVULSTATE = false;

        PREATTACKHITCD = false;
        HEROATTACKPOS = Vector2.One;
        ATTACKTYPE = 1;
        ENEMYPROJHIT = false;

        enemydataType = 4;
        ALERT = false;
        SPAWN = true;


        switch (ProfileManager.enemyProfileType)
        {
            case 1: // Caso seja um player do tipo Berzerk
                castSpellChance = 70;
                HP = 140;
                break;
            case 2: // Caso seja um player do tipo Balanced
                DAMAGE = 20;
                _preattackcdduration = 1f;
                break;
            case 3: // Caso seja um player do tipo Strategist
                speed = 150f;
                castSpellChance = 30;
                break;
            default: // Inimigo base; sem mudanças
                break;
        }


    }

    private void CastProj()
    {
        //Definindo atributos do projétil
        ProjectileData pd = new()
        {
            Position = CENTER,
            Direction = Globals.HEROLASTPOS - CENTER,
            Lifespan = 3,
            Homing = true,
            ProjectileType = "DarkProj",
            Scale = 1.75f,
            Speed = 300,
            Friendly = false,
            Damage = DAMAGE
        };
        ProjectileManager.AddProjectile(pd);    // Adicionando o projétil ao gerenciador
        ENEMYSKILL_LOCK = false;    //Ao lançar o projétil ativa uma trava, impedindo que lance varios 
    }


    private void CastSkill()
    {
        //Habilidade que cria varios "projeteis" imoveis no jogador, mais detalhes da habilidade no gerenciador de colisões (CollisionManager.cs)
        ProjectileData pd1 = new()
        {
            Position = Globals.HEROLASTPOS - new Vector2(0, 32),
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
            Position = Globals.HEROLASTPOS + new Vector2(64, 0),
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
            Position = Globals.HEROLASTPOS - new Vector2(64, 0),
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
            Position = Globals.HEROLASTPOS + new Vector2(0, 32),
            Direction = Globals.HEROLASTPOS - CENTER,
            Lifespan = 3,
            Homing = false,
            ProjectileType = "DarkSpell",
            Scale = 2f,
            Speed = 0,
            Friendly = false,
            Damage = 10
        };
        ProjectileManager.AddProjectile(pd1); //Cria quatro projeteis na área de ação
        ProjectileManager.AddProjectile(pd2);
        ProjectileManager.AddProjectile(pd3);
        ProjectileManager.AddProjectile(pd4);
        ENEMYSKILL_LOCK = false;
    }

    //Função de calculo para caixas de colisão
    public override Rectangle GetBounds(string boundType)
    {
        //Limites do topo e da esquerda da caixa de colisão
        int _left = (int)CENTER.X - (int)(basehitboxSize.X * scale) / 2; //Define limites de acordo com centro, parametros da hitbox e sprite
        int _top = (int)CENTER.Y - (int)(basehitboxSize.Y * scale) / 2; //Mesma coisa, porem verticalmente

        int _reactionSize = 170 * scale;  //Tamanho da caixa de colisão

        Vector2 _attackOffset1 = new Vector2(_left - 32, _top - 28); //Define a posição dos golpes utilizando dos limites pré-definidos e valores absolutos definidos pela animação
        Vector2 _attackOffset1M = new Vector2(_left - 114, _top - 28); //Versão espelhada
        Vector2 _attackOffset2 = new Vector2(_left - 80, _top); //Segunda parte do golpe
        Vector2 _attackOffset2M = new Vector2(_left - 114, _top); //Versão espelhada
        Vector2 _attackSize1 = new Vector2(scale * 45, scale * 25); //Área definida de acordo com valores absolutos de animação e escalonamento de sprite
        Vector2 _attackSize2 = new Vector2(scale * 54, scale * 20); //Segunda parte do golpe

        switch (boundType) //Alterna o tipo de colisão de acordo com o valor passado
        {
            case "hitbox":
                //Caixa de colisão do monstro
                //Com base nas coordenadas _Top e _Left cria um retangulo de tamanho pré-definido multiplicado pelo scale
                return new Rectangle(_left + 28, _top, (int)(basehitboxSize.X * scale) - 60, (int)(basehitboxSize.Y * scale));
            case "reactionbox":
                //Caixa de colisão para Reação do monstro
                return new Rectangle((int)CENTER.X - _reactionSize / 2, (int)CENTER.Y - _reactionSize / 2, _reactionSize, _reactionSize);
            case "attackbox1":
                //Caixa de colisão para o 1º Golpe do monstro
                return new Rectangle(0, 0, 0, 0);
            default:
                //Caso nenhuma caixa de colisão válida seja selecionada
                throw new InvalidOperationException($"Tipo de caixa de colisão desconhecido: {boundType}"); ;
        }

    }

    public override void MapBounds(Point mapSize, Point tileSize) // Calcula bordas do mapa
    {
        _minPos = new((-tileSize.X / 2) - basehitboxSize.X * scale, (-tileSize.Y / 2) - 144); //Limite esquerda e cima (limites minimos)
        _maxPos = new(mapSize.X - (tileSize.X / 2) - CENTER.X - 120, mapSize.Y - (tileSize.X / 2) - CENTER.Y - 110); //Limite direita e baixo (limites minimos)
    }

    public override async Task SetInvulnerableTemporarily(int durationInMilliseconds)
    {
        INVULSTATE = true; // Ativa a ivulnerabilidade

        await Task.Delay(durationInMilliseconds); // Espera X tempo

        INVULSTATE = false; // Desativa após o tempo
    }

    // Variaveis e Funções atreladas ao IA / PythonBridge.cs



    //Variaveis para o temporizador entre pré-ataque e ataque
    float _preattacktimer = 0f, _preattackduration = 0f;

    //Variaveis para o temporizador entre ataques
    float _preattackcdtimer = 0f, _preattackcdduration;

    //Variaveis para tempo de recuo do knockback
    float _recoilingtimer = 0f, _recoilingduration = 0.1f;
    //Recarga entre danos recebidos de projeteis
    float _projHitTimer = 0f;


    public override async void Update()
    {
        // Definindo o centro do frame de acordo com a posição atual
        CENTER = POSITION + (origin + new Vector2(0, 25)) * scale;

        //Atualiza continuamente o status dos contabilizadores de combate
        UpdateBattleStats();

        //Conta a quantidade de Dashs/Avanços que o jogador fez em combate
        if (battleStats.inBattle) battleStats.IncrementDashCount();


        //Marcador de contusão; caso inimigo receba dano ele fica invulneravel e recebe Knockback/Recoiling/Recuo, caso seja durante um pré-ataque ele reinicia a ação.
        if (INVULSTATE)
        {
            if (!battleStats.firstHitReceived) battleStats.MarkFirstHit(); // Inicia o contabilizador de tempo apartir do primeiro golpe recebido
            Recoling = true; //Recuo se torna verdadeiro
            _preattacktimer = 0f;
            _recoilingtimer = 0f;
            if (ATTACKSTATE) //Se estiver em estado de ataque...
            {
                ATTACKSTATE = false; //Desativa o ataque
                PREATTACKHITCD = true; //Ativa o tempo de recarga de ataque
                _preattackcdtimer = 0.75f; //Restitue parte do seu tempo de recarga
                _anims.Reset("necro_Attack"); //Reseta a animação de ataque para começar ela do inicio novamente
            }
        }
        //Temporizador de transição da instancia de pré-ataque para ataque
        else if (PREATTACKSTATE && !ATTACKSTATE && !Recoling)
        {

            _preattacktimer += (float)Globals.TotalSeconds;
            if (_preattacktimer >= _preattackduration)
            {

                PREATTACKSTATE = false;
                ATTACKSTATE = true;
                _preattacktimer = 0f;
            }

        }

        //Caso inimigo esteja com vida e esteja em estado de Recoiling/Recuo/Knockback;
        if (Recoling && HP > 0)
        {
            Vector2 _knockbackdist;
            _knockbackdist = (Vector2.Normalize(CENTER - HEROATTACKPOS)) / 2; //define a direção do recuo, sendo ela contrária ao atacante
            if (!ATTACKSTATE) POSITION = new Vector2(POSITION.X + _knockbackdist.X, POSITION.Y); ; // Aplica o recuo apenas na horizontal
            _recoilingtimer += (float)Globals.TotalSeconds; //Por X tempo
            if (_recoilingtimer >= _recoilingduration)
            {
                Recoling = false;//No fim da duração, para de sofrer Recuo.
                _recoilingtimer = 0f;
            }

        }

        //Temporizador de tempo de recarga entre os ataques
        if (PREATTACKHITCD)
        {

            _preattackcdtimer += (float)Globals.TotalSeconds;
            if (_preattackcdtimer >= _preattackcdduration)
            {
                PREATTACKHITCD = false;
                _preattackcdtimer = 0f;
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

        //Ao terminar a ação de ataque ativa a trava ENEMYSKILL_LOCK disparando uma magia com a função CastProj() ou CastSpell();
        if (ENEMYSKILL_LOCK)
        {
            if (!ALERT) ALERT = true;
            spellChance = new RandomGenerator(RandomGenerator.GenerateSeedFromCurrentTime());
            if (spellChance.NextInt(0, 100) > castSpellChance) CastProj();
            else CastSkill();

        }

        //Aplica um comportamento ao inimigo definido pelo EnemyManager.cs
        if (MoveAI != null)
        {
            //Aplica um comportamento ao inimigo definido pelo GameManager.cs
            MoveAI.Move(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //Controladores de animação

        //Define as animações de acordo com os estados
        if (HP <= 0) //Caso de morte
        {
            _anims.Update("necro_Death");
            battleStats.EndBattle(); // Termina a batalha e contabiliza o tempo total
            await SerializeDataOnDeath();
        }
        else if (ATTACKSTATE) //Caso de Ataque
        {
            _anims.Update("necro_Attack");
        }
        else if (Recoling)//Caso de Dano Recebido
        {
            _anims.Update("necro_Hit");
        }
        else if (PREATTACKSTATE)//Caso de pré-ataque
        {
            _anims.Update("necro_Preattack");
        }
        else if (walkState)//Caso de Andar
        {
            _anims.Update("necro_Walk");
        }
        else//Caso de parado
        {
            _anims.Update("necro_Idle");
        };

        //Gerenciador de espelhamento, faz com que o inimigo sempre fique em direção ao jogador
        if (Globals.HEROLASTPOS.X - CENTER.X > 0)
            mirror = false;
        else if (Globals.HEROLASTPOS.X - CENTER.X < 0)
            mirror = true;

        //////////////////////////////////////////////////////////////////////////////////////////

        //Caso o inimigo esteja fazendo alguma ação que impede o movimento ele entra em estado de 'actionstate'
        if (INVULSTATE || PREATTACKSTATE || ATTACKSTATE || HP <= 0 || Recoling) actionstate = true;
        else actionstate = false; //Caso não esteja em nenhum desses estados ele pode voltar a se mover

        POSITION = Vector2.Clamp(POSITION, _minPos, _maxPos); // não permite que inimigo passe das bordas do mapa
    }


    public override void Draw()
    {
        //hitbox test
        //Rectangle Erect = GetBounds($"hitbox");
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

        //Reaction box test
        //Rectangle Erect = GetBounds($"reactionbox");
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);




        //Passa os parametros para o AnimationManager animar o Spritesheet
        _anims.Draw(POSITION, scale, mirror, 0, colorSet());

    }
}