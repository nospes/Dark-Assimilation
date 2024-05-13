namespace MyGame;

public class enemySkeleton : enemyBase
{
    // Variaveis para Animações
    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _textureIdle, _textureHit, _textureWalk, _textureDeath, _textureAttack, _texturePreattack;  //Spritesheets



    public enemySkeleton(Vector2 pos)
    {
        //Atribuindo spritesheets a variaveis de Texture2D
        _textureIdle ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Idle");
        _textureHit ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Hit");
        _textureWalk ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Walk");
        _textureDeath ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Death");
        _texturePreattack ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Preattack");
        _textureAttack ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Attack");


        //Definindo area,frames dos sprites sheets para fazer a animação
        _anims.AddAnimation("bigskel_Idle", new(_textureIdle, 4, 1, 0.1f, this, this));
        _anims.AddAnimation("bigskel_Hit", new(_textureHit, 3, 1, 0.1f, this, this));
        _anims.AddAnimation("bigskel_Walk", new(_textureWalk, 12, 1, 0.1f, this, this));
        _anims.AddAnimation("bigskel_Death", new(_textureDeath, 13, 1, 0.1f, this, this));
        _anims.AddAnimation("bigskel_Preattack", new(_texturePreattack, 4, 1, 0.1f, this, this));
        _anims.AddAnimation("bigskel_Attack", new(_textureAttack, 10, 1, 0.1f, this, this));

        //Define a posição, velocidade e tamanho do sprite respectivamente
        POSITION = pos;
        scale = 5;

        //Definição inicial de origem e tamanho da caixa de colisão
        basehitboxSize = new(14, 30); // Tamanho
        var frameWidth = _textureIdle.Width / 4; // Dividindo os frames de acordo com tamanho do spritesheet, usando como base a animação de Idle;
        var frameHeight = _textureIdle.Height / 1;
        origin = new(frameWidth / 2, frameHeight / 2); //Atribui o centro do frame X e Y a um vetor

        //Pré definição de atributos de combate e animação para evitar bugs
        HP = 150;
        speed = 100f;
        DAMAGE = 10;
        _preattackcdduration = 1f;
        _preattackduration = 0.8f;

        DASHSTATE = false;
        ATTACKSTATE = false;
        PREATTACKSTATE = false;
        DEATHSTATE = false;
        INVULSTATE = false;

        PREATTACKHITCD = false;
        HEROATTACKPOS = Vector2.One;
        ATTACKTYPE = 1;
        ENEMYPROJHIT = false;

        enemydataType = 1;
        ALERT = false;
        SPAWN = true;

        switch (ProfileManager.enemyProfileType)
        {
            case 1: // Caso seja um player do tipo Berzerk
                _preattackduration = 0.5f;
                DAMAGE = 15;
                break;
            case 2: // Caso seja um player do tipo Balanced
                HP = 190;
                _preattackcdduration = 0.5f;
                break;
            case 3:// Caso seja um player do tipo Strategist
                speed = 140f;
                _preattackcdduration = 0.8f;
                _preattackduration = 0.65f;
                break;
            default:
                break;
        }
    }

    //Função de calculo para caixas de colisão
    public override Rectangle GetBounds(string boundType)
    {
        //Limites do topo e da esquerda da caixa de colisão
        int _left = (int)CENTER.X - (int)(basehitboxSize.X * scale) / 2; //Define limites de acordo com centro, parametros da hitbox e sprite
        int _top = (int)CENTER.Y - (int)(basehitboxSize.Y * scale) / 2; //Mesma coisa, porem verticalmente

        int _reactionOffset = 25; //Ajuste de posição para caixa de colisão
        int _reactionSize = 40 * scale;  //Tamanho da caixa de colisão

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
                return new Rectangle(_left, _top, (int)(basehitboxSize.X * scale), (int)(basehitboxSize.Y * scale));
            case "reactionbox":
                //Caixa de colisão para Reação do monstro
                if (!mirror) return new Rectangle(_left - _reactionOffset, _top, _reactionSize, (int)((basehitboxSize.Y - 14) * scale));
                else return new Rectangle(_left - _reactionSize / 2, _top, _reactionSize, (int)((basehitboxSize.Y - 14) * scale));
            case "attackbox1":
                //Caixa de colisão para o 1º Golpe do monstro
                if (!mirror) return new Rectangle((int)_attackOffset1.X, (int)_attackOffset1.Y, (int)_attackSize1.X, (int)_attackSize1.Y);
                else return new Rectangle((int)_attackOffset1M.X, (int)_attackOffset1M.Y, (int)_attackSize1.X, (int)_attackSize1.Y);
            case "attackbox2":
                //Caixa de colisão para o 2º Golpe do monstro
                if (!mirror) return new Rectangle((int)_attackOffset2.X, (int)_attackOffset2.Y, (int)_attackSize2.X, (int)_attackSize2.Y);
                else return new Rectangle((int)_attackOffset2M.X, (int)_attackOffset2M.Y, (int)_attackSize2.X, (int)_attackSize2.Y);
            default:
                //Caso nenhuma caixa de colisão válida seja selecionada
                throw new InvalidOperationException($"Tipo de caixa de colisão desconhecido: {boundType}"); ;
        }

    }
    public override void MapBounds(Point mapSize, Point tileSize) // Calcula bordas do mapa
    {
        _minPos = new((-tileSize.X / 2) - basehitboxSize.X * scale, (-tileSize.Y / 2)); //Limite esquerda e cima (limites minimos)
        _maxPos = new(mapSize.X - (tileSize.X / 2) - CENTER.X - 120, mapSize.Y - (tileSize.X / 2) - CENTER.Y - 110); //Limite direita e baixo (limites minimos)
    }

    public override async Task SetInvulnerableTemporarily(int durationInMilliseconds)
    {
        INVULSTATE = true; // Ativa a ivulnerabilidade

        await Task.Delay(durationInMilliseconds); // Espera X tempo

        INVULSTATE = false; // Desativa após o tempo
    }

    //Variaveis para o temporizador entre pré-ataque e ataque
    float _preattacktimer = 0f, _preattackduration;

    //Variaveis para o temporizador entre ataques
    float _preattackcdtimer = 0f, _preattackcdduration;

    //Variaveis para tempo de recuo do knockback
    float _recoilingtimer = 0f, _recoilingduration = 0.1f;

    //Recarga entre danos recebidos de projeteis
    float _projHitTimer = 0f;

    public override async void Update()
    {

        // Definindo o centro do frame de acordo com a posição atual
        CENTER = POSITION + origin * scale;

        //Atualiza continuamente o status dos contabilizadores de combate
        UpdateBattleStats();

        //Conta a quantidade de Dashs/Avanços que o jogador fez em combate
        if (battleStats.inBattle) battleStats.IncrementDashCount();

        //Marcador de contusão; caso inimigo receba dano ele fica invulneravel e recebe Knockback/Recoiling/Recuo, caso cancele o pré-ataque ele reduz ou reinicia o temporizador
        if (INVULSTATE)
        {
            if (!battleStats.firstHitReceived) battleStats.MarkFirstHit(); // Inicia o contabilizador de tempo apartir do primeiro golpe recebido
            Recoling = true; //Recuo se torna verdadeiro
            _recoilingtimer = 0f;
            if (PREATTACKSTATE && _preattacktimer >= _preattackduration * 0.4f) _preattacktimer = _preattackduration * 0.99f;
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

        //Trava para evitar bugs relacionado a ordem de carregamento do jogo
        if (MoveAI != null)
        {
            //Aplica um comportamento ao inimigo definido pelo EnemyManager.cs
            MoveAI.Move(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //Controladores de animação

        //Define as animações de acordo com os estados
        if (HP <= 0) //Caso de morte
        {
            _anims.Update("bigskel_Death");
            battleStats.EndBattle(); // Termina a batalha e contabiliza o tempo total
            await SerializeDataOnDeath(); // Escreve os dados no JSON
        }
        else if (ATTACKSTATE) //Caso de Ataque
        {
            _anims.Update("bigskel_Attack");
        }
        else if (Recoling)//Caso de Dano Recebido
        {
            _anims.Update("bigskel_Hit");
        }
        else if (PREATTACKSTATE)//Caso de pré-ataque
        {
            _anims.Update("bigskel_Preattack");
        }
        else if (walkState)//Caso de Andar
        {
            _anims.Update("bigskel_Walk");
        }
        else//Caso de parado
        {
            _anims.Update("bigskel_Idle");
        };

        //Gerenciador de espelhamento, faz com que o inimigo sempre fique em direção ao jogador caso não esteja atacando
        if (!ATTACKSTATE && !PREATTACKSTATE)
        {
            if (Globals.HEROLASTPOS.X - CENTER.X > 0)
                mirror = false;
            else if (Globals.HEROLASTPOS.X - CENTER.X < 0)
                mirror = true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        //Caso o inimigo esteja fazendo alguma ação que impede o movimento ele entra em estado de 'actionstate'
        if (INVULSTATE || PREATTACKSTATE || ATTACKSTATE || HP <= 0 || Recoling) actionstate = true;
        else actionstate = false; //Caso não esteja em nenhum desses estados ele pode voltar a se mover

        POSITION = Vector2.Clamp(POSITION, _minPos, _maxPos); // não permite que inimigo passe das bordas do mapa

    }


    public override void Draw()
    {
        //hitbox test
        //Rectangle Erect = GetBounds($"attackbox{ATTACKTYPE}");
        //if (ATTACKHITTIME) Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

        //Reaction box test
        //Rectangle Erect = GetBounds($"reactionbox");
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

        //Passa os parametros para o AnimationManager animar o Spritesheet
        _anims.Draw(POSITION, scale, mirror, 0, colorSet());


    }
}