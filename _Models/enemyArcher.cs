namespace MyGame;

public class enemyArcher : enemyBase
{
    // Variaveis para Animações
    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _textureIdle, _textureHit, _textureWalk, _textureDeath, _textureAttack, _textureDash, _textureSkill;  //Spritesheets
    //Definição de comportamento
    public MovementAI MoveAI { get; set; }


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
        _anims.AddAnimation("archer_Idle", new(_textureIdle, 4, 1, 0.1f, 1, this));
        _anims.AddAnimation("archer_Walk", new(_textureWalk, 8, 1, 0.1f, 1, this));
        _anims.AddAnimation("archer_Death", new(_textureDeath, 8, 1, 0.1f, 1, this));
        _anims.AddAnimation("archer_Attack", new(_textureAttack, 8, 1, 0.1f, 1, this));
        _anims.AddAnimation("archer_Dash", new(_textureDash, 7, 1, 0.07f, 1, this));
        _anims.AddAnimation("archer_Hit", new(_textureHit, 4, 1, 0.1f, 1, this));
        _anims.AddAnimation("archer_Skill", new(_textureSkill, 4, 1, 0.1f, 1, this));

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
                if (!mirror) return new Rectangle((int)_attackOffset1.X, (int)_attackOffset1.Y, (int)_attackSize1.X, (int)_attackSize1.Y);
                else return new Rectangle((int)_attackOffset1M.X, (int)_attackOffset1M.Y, (int)_attackSize1.X, (int)_attackSize1.Y);
            default:
                //Caso nenhuma caixa de colisão válida seja selecionada
                throw new InvalidOperationException($"Tipo de caixa de colisão desconhecido: {boundType}"); ;
        }

    }

    //Variaveis para o temporizador entre pré-ataque e ataque
    float _preattacktimer = 0f, _preattackduration = 2f;
    //Variaveis para o temporizador entre ataques
    float _preattackcdtimer = 0f, _preattackcdduration = 1f;

    //Variaveis para tempo de recuo do knockback
    float _recoilingtimer = 0f, _recoilingduration = 0.1f;

    //Variaveis para recarga do avanço/dash
    float _dashtimer = 0f, _dashduration = 2f; bool _dashcdlock = false;

    public override void Update()
    {

        //Marcador de contusão; caso inimigo receba dano ele fica invulneravel e recebe Knockback/Recoiling/Recuo, caso seja durante um pré-ataque ele reinicia a ação.
        if (INVULSTATE)
        {
            Recoling = true; //Recuo se torna verdadeiro
            _recoilingtimer = 0f; // Reinicia a duração do recuo
            PREATTACKSTATE = false; // Cancela o pré ataque e seu temporizador
            _preattacktimer = 0f;
            _preattackcdtimer = 0f;

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
            _preattacktimer = 0f;
            PREATTACKSTATE = false;

            PREATTACKHITCD = true;
            Vector2 _dashdir;
            _dashdir = (Vector2.Normalize(CENTER - HEROATTACKPOS)); //define a direção do recuo, sendo ela contrária ao atacante
            position += _dashdir * 7; // Aplica o recuo durante o DASHSTATE
            _dashcdlock = true;

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

        // Definindo o centro do frame de acordo com a posição atual
        CENTER = position + origin * scale;

        //Trava para evitar bugs relacionado a ordem de carregamento do jogo
        if (MoveAI != null)
        {
            //Aplica um comportamento ao inimigo definido pelo GameManager.cs
            MoveAI.Move(this);
        }


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

        //Caso o inimigo esteja fazendo alguma ação que impede o movimento ele entra em estado de 'actionstate'
        if (INVULSTATE || PREATTACKSTATE || ATTACKSTATE || HP <= 0 || Recoling || DASHSTATE) actionstate = true;
        else actionstate = false; //Caso não esteja em nenhum desses estados ele pode voltar a se mover

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