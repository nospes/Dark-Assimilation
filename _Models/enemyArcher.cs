namespace MyGame;

public class enemyArcher : enemyBase
{
    // Variaveis para Animações
    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _textureIdle, _textureHit, _textureWalk, _textureDeath, _textureAttack, _textureDash, _textureSkill;  //Spritesheets
    public MovementAI MoveAI { get; set; } //Definição de comportamento



    public enemyArcher(Vector2 pos)
    {
        //Atribuindo spritesheets a variaveis de Texture2D
        _textureIdle ??= Globals.Content.Load<Texture2D>("Creatures/Archer/archer_Idle");
        _textureWalk ??= Globals.Content.Load<Texture2D>("Creatures/Archer/archer_Walk");
        _textureDeath ??= Globals.Content.Load<Texture2D>("Creatures/Archer/archer_Death");
        _textureAttack ??= Globals.Content.Load<Texture2D>("Creatures/Archer/archer_Attack");
        _textureDash ??= Globals.Content.Load<Texture2D>("Creatures/Archer/archer_Dash");
        _textureHit ??= Globals.Content.Load<Texture2D>("Creatures/Archer/archer_Hit");
        _textureSkill ??= Globals.Content.Load<Texture2D>("Creatures/Archer/archer_Skill");


        //Definindo area,frames dos sprites sheets para fazer a animação
        _anims.AddAnimation("archer_Idle", new(_textureIdle, 4, 1, 0.1f, this, this));
        _anims.AddAnimation("archer_Walk", new(_textureWalk, 8, 1, 0.1f, this, this));
        _anims.AddAnimation("archer_Death", new(_textureDeath, 8, 1, 0.1f, this, this));
        _anims.AddAnimation("archer_Attack", new(_textureAttack, 8, 1, 0.2f, this, this));
        _anims.AddAnimation("archer_Dash", new(_textureDash, 7, 1, 0.07f, this, this));
        _anims.AddAnimation("archer_Hit", new(_textureHit, 4, 1, 0.1f, this, this));
        _anims.AddAnimation("archer_Skill", new(_textureSkill, 4, 1, 0.1f, this, this));

        //Define a posição, velocidade e tamanho do sprite respectivamente
        position = pos;
        speed = 100f;
        scale = 3;

        //Definição inicial de origem e tamanho da caixa de colisão
        basehitboxSize = new(14, 30); // Tamanho
        var frameWidth = _textureIdle.Width / 4; // Dividindo os frames de acordo com tamanho do spritesheet, usando como base a animação de Idle;
        var frameHeight = _textureIdle.Height / 1;
        origin = new(frameWidth / 2, frameHeight / 2); //Atribui o centro do frame X e Y a um vetor

        //Pré definição de atributos de combate e animação para evitar bugs
        HP = 100;
        DASHSTATE = false;
        ATTACKSTATE = false;
        PREATTACKSTATE = false;
        DEATHSTATE = false;
        INVULSTATE = false;
        PREATTACKHITCD = false;
        HEROATTACKPOS = Vector2.One;
        ATTACKTYPE = 1;

    }

    //Função utilizada para definir como é o projétil que vai ser adicionado ao gerenciador de projeteis
    private void Fire()
    {
        //Definindo atributos do projétil
        ProjectileData pd = new()
        {
            Position = CENTER,
            Direction = Globals.HEROLASTPOS - CENTER,
            Lifespan = 3,
            Homing = false,
            ProjectileType = "Arrow",
            Scale = 1.75f,
            Speed = 500
        };
        ProjectileManager.AddProjectile(pd);    // Adicionando o projétil ao gerenciador
        ENEMYSKILL_LOCK = false;    //Ao lançar o projétil ativa a trava impedindo que lance varias flechas 
    }


    //Função de calculo para caixas de colisão
    public override Rectangle GetBounds(string boundType)
    {
        //Limites do topo e da esquerda da caixa de colisão
        int _left = (int)CENTER.X - (int)(basehitboxSize.X * scale) / 2; //Define limites de acordo com centro, parametros da hitbox e sprite
        int _top = (int)CENTER.Y - (int)(basehitboxSize.Y * scale) / 2; //Mesma coisa, porem verticalmente


        int _reactionSize = 180 * scale;  //Tamanho da caixa de colisão

        Vector2 _attackOffset1 = new Vector2(_left - 32, _top - 28); //Define a posição dos golpes utilizando dos limites pré-definidos e valores absolutos definidos pela animação
        Vector2 _attackOffset1M = new Vector2(_left - 90, _top - 28); //Versão espelhada
        Vector2 _attackOffset2 = new Vector2(_left - 80, _top); //Segunda parte do golpe
        Vector2 _attackOffset2M = new Vector2(_left - 84, _top); //Versão espelhada
        Vector2 _attackSize1 = new Vector2(scale * 45, scale * 25); //Área definida de acordo com valores absolutos de animação e escalonamento de sprite
        Vector2 _attackSize2 = new Vector2(scale * 58, scale * 20); //Segunda parte do golpe

        switch (boundType) //Alterna o tipo de colisão de acordo com o valor passado
        {
            case "hitbox":
                //Caixa de colisão do monstro
                //Com base nas coordenadas _Top e _Left cria um retangulo de tamanho pré-definido multiplicado pelo scale
                return new Rectangle(_left, _top, (int)(basehitboxSize.X * scale), (int)(basehitboxSize.Y * scale));
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
        _minPos = new((-tileSize.X / 2) - basehitboxSize.X * scale, (-tileSize.Y / 2)); //Limite esquerda e cima (limites minimos)
        _maxPos = new(mapSize.X - (tileSize.X / 2) - CENTER.X - 120, mapSize.Y - (tileSize.X / 2) - CENTER.Y - 110); //Limite direita e baixo (limites minimos)
    }


    //Variaveis para o temporizador entre ataques
    float _preattackcdtimer = 0f, _preattackcdduration = 1.6f;

    //Variaveis para tempo de recuo do knockback
    float _recoilingtimer = 0f, _recoilingduration = 0.1f;

    //Variaveis para recarga do avanço/dash
    float _dashtimer = 0f, _dashduration = 3f; bool _dashcdlock = false;

    public override void Update()
    {

        // Definindo o centro do frame de acordo com a posição atual
        CENTER = position + origin * scale;

        //Marcador de contusão; caso inimigo receba dano ele fica invulneravel e recebe Knockback/Recoiling/Recuo, caso seja durante um pré-ataque ele reinicia a ação.
        if (INVULSTATE)
        {
            Recoling = true; //Recuo se torna verdadeiro
            _recoilingtimer = 0f; // Reinicia a duração do recuo
            PREATTACKSTATE = false; // Cancela o pré ataque e seu temporizador
            if (ATTACKSTATE) //Se estiver em estado de ataque...
            {
                ATTACKSTATE = false; //Desativa o ataque
                PREATTACKHITCD = true; //Ativa o tempo de recarga de ataque
                if(_preattackcdtimer>0.6)_preattackcdtimer = 0.9f; //Restitue parte do seu tempo
                _anims.Reset("archer_Attack"); //Reseta a animação de ataque para começar ela do inicio novamente
            }

        }

        //Temporizador de transição da instancia de pré-ataque para ataque
        else if (PREATTACKSTATE && !ATTACKSTATE && !Recoling)
        {
            PREATTACKSTATE = false;
            ATTACKSTATE = true;
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

        //Caso inimigo esteja com vida e esteja em estado de Recoiling/Recuo/Knockback;
        if (Recoling && HP > 0)
        {
            Vector2 _knockbackdist;
            _knockbackdist = (Vector2.Normalize(CENTER - HEROATTACKPOS)) / 2; //define a direção do recuo, sendo ela contrária ao atacante
            if (!ATTACKSTATE) position.X += _knockbackdist.X; // Aplica o recuo apenas na horizontal
            _recoilingtimer += (float)Globals.TotalSeconds; //Por X tempo
            if (_recoilingtimer >= _recoilingduration)
            {
                Recoling = false;//No fim da duração, para de sofrer Recuo.
                _recoilingtimer = 0f;
                if (!_dashcdlock) DASHSTATE = true; //Entra em modo de dash após receber dano
            }
        }

        //Ao entrar em DASHSTATE  faz um recuo e aplica tempo de recarga dele
        if (DASHSTATE)
        {
            ATTACKSTATE = false; // Cancela o ataque
            PREATTACKSTATE = false; // Cancela o pré ataque
            PREATTACKHITCD = true;  //Ativa o CD porem...
            _preattackcdtimer = 1.6f; //Restitue 100% do tempo de recarga do golpe
            position += Vector2.Normalize(CENTER - HEROATTACKPOS) * 7; // Aplica o recuo durante o DASHSTATE
            _dashcdlock = true; //Ativa o tempo de recarga o dash

        }

        //Tempo de recarga do DASHSTATE
        if (_dashcdlock)
        {
            _dashtimer += (float)Globals.TotalSeconds;
            if (_dashtimer >= _dashduration)
            {
                _dashcdlock = false;
                _dashtimer = 0f;
            }
        }

        //Ao terminar a ação de ataque ativa a trava ENEMYSKILL_LOCK disparando uma flecha com a função Fire();
        if (ENEMYSKILL_LOCK) Fire();


        //Trava para evitar bugs relacionado a ordem de carregamento do jogo
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
            _anims.Update("archer_Death");
        }
        else if (ATTACKSTATE) //Caso de Ataque
        {
            _anims.Update("archer_Attack");
        }
        else if (Recoling)//Caso de Dano Recebido
        {
            _anims.Update("archer_Hit");
        }
        else if (DASHSTATE)
        {
            _anims.Update("archer_Dash");
        }
        else if (PREATTACKSTATE)
        {
            _anims.Update("archer_Hit");
        }
        else if (walkState)//Caso de Andar
        {
            _anims.Update("archer_Walk");
        }
        else//Caso de parado
        {
            _anims.Update("archer_Idle");
        };

        //Gerenciador de espelhamento, faz com que o inimigo sempre fique em direção ao jogador
        if (Globals.HEROLASTPOS.X - CENTER.X > 0)
            mirror = false;
        else if (Globals.HEROLASTPOS.X - CENTER.X < 0)
            mirror = true;

        //////////////////////////////////////////////////////////////////////////////////////////

        //Caso o inimigo esteja fazendo alguma ação que impede o movimento ele entra em estado de 'actionstate'
        if (INVULSTATE || PREATTACKSTATE || ATTACKSTATE || HP <= 0 || Recoling || DASHSTATE) actionstate = true;
        else actionstate = false; //Caso não esteja em nenhum desses estados ele pode voltar a se mover

        position = Vector2.Clamp(position, _minPos, _maxPos); // não permite que inimigo passe das bordas do mapa

    }




    public override void Draw()
    {
        //hitbox test
        //Rectangle Erect = GetBounds($"reactionbox");
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

        //Passa os parametros para o AnimationManager animar o Spritesheet
        _anims.Draw(position, scale, mirror);


    }
}