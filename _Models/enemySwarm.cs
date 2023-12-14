namespace MyGame;

public class enemySwarm : enemyBase
{
    // Variaveis para Animações
    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _textureIdle_blue, _textureIdle_green, _textureIdle_purple, _textureIdle_yellow, _textureHit_blue, _textureHit_green, _textureHit_purple, _textureHit_yellow, _textureAttack, _texturePreattack, _textureDeath_blue, _textureDeath_yellow, _textureDeath_purple, _textureDeath_green;  //Spritesheets
    //Definição de comportamento
    public MovementAI MoveAI { get; set; }

    private int _colorselect;


    public enemySwarm(Vector2 pos)
    {
        //Atribuindo spritesheets a variaveis de Texture2D
        _textureIdle_blue ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_blue");
        _textureHit_blue ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_blue_hurt");
        _textureDeath_blue ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_blue_death");

        _textureIdle_green ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_green");
        _textureHit_green ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_green_hurt");
        _textureDeath_green ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_green_death");

        _textureIdle_purple ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_purple");
        _textureHit_purple ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_purple_hurt");
        _textureDeath_purple ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_purple_death");

        _textureIdle_yellow ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_yellow");
        _textureHit_yellow ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_yellow_hurt");
        _textureDeath_yellow ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_yellow_death");

        _texturePreattack ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_red");
        _textureAttack ??= Globals.Content.Load<Texture2D>("Creatures/Swarm/swarm_red");


        //Definindo area,frames dos sprites sheets para fazer a animação
        _anims.AddAnimation("swarm_blue", new(_textureIdle_blue, 16, 1, 0.1f, this, this));
        _anims.AddAnimation("swarm_blue_hurt", new(_textureHit_blue, 16, 1, 0.1f, this, this));
        _anims.AddAnimation("swarm_blue_death", new(_textureDeath_blue, 16, 1, 0.1f, this, this));

        _anims.AddAnimation("swarm_green", new(_textureIdle_green, 16, 1, 0.1f, this, this));
        _anims.AddAnimation("swarm_green_hurt", new(_textureHit_green, 16, 1, 0.1f, this, this));
        _anims.AddAnimation("swarm_green_death", new(_textureDeath_green, 16, 1, 0.1f, this, this));

        _anims.AddAnimation("swarm_purple", new(_textureIdle_purple, 16, 1, 0.1f, this, this));
        _anims.AddAnimation("swarm_purple_hurt", new(_textureHit_purple, 16, 1, 0.1f, this, this));
        _anims.AddAnimation("swarm_purple_death", new(_textureDeath_purple, 16, 1, 0.1f, this, this));

        _anims.AddAnimation("swarm_yellow", new(_textureIdle_yellow, 16, 1, 0.1f, this, this));
        _anims.AddAnimation("swarm_yellow_hurt", new(_textureHit_yellow, 16, 1, 0.1f, this, this));
        _anims.AddAnimation("swarm_yellow_death", new(_textureDeath_yellow, 16, 1, 0.1f, this, this));

        _anims.AddAnimation("swarm_Preattack", new(_texturePreattack, 16, 1, 0.1f, this, this));
        _anims.AddAnimation("swarm_Attack", new(_textureAttack, 16, 1, 0.1f, this, this));

        //Define a posição, velocidade e tamanho do sprite respectivamente
        position = pos;
        speed = 100f;
        scale = 2;

        //Definição inicial de origem e tamanho da caixa de colisão
        basehitboxSize = new(25, 25); // Tamanho
        var frameWidth = _textureIdle_blue.Width / 16; // Dividindo os frames de acordo com tamanho do spritesheet, usando como base a animação de Idle;
        var frameHeight = _textureIdle_blue.Height / 1;
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
        ATTACKHITTIME = true;

        _colorselect = new Random().Next(1, 3);

    }

    //Função de calculo para caixas de colisão
    public override Rectangle GetBounds(string boundType)
    {
        //Limites do topo e da esquerda da caixa de colisão
        int _left = (int)CENTER.X - (int)(basehitboxSize.X * scale) / 2; //Define limites de acordo com centro, parametros da hitbox e sprite
        int _top = (int)CENTER.Y - (int)(basehitboxSize.Y * scale) / 2; //Mesma coisa, porem verticalmente

        int _reactionSize = 150 * scale;  //Tamanho da caixa de colisão

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
                return new Rectangle((int)CENTER.X - _reactionSize / 2, (int)CENTER.Y - _reactionSize / 2, _reactionSize, _reactionSize);
            case "attackbox1":
            //Tem uma Attackbox levemente menor que sua Hitbox, dando mais janela para jogador atacar sem receber dano
                return new Rectangle(_left + 5, _top + 5, (int)((basehitboxSize.X - 5) * scale), (int)((basehitboxSize.Y - 5) * scale));
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

    //Variaveis para o temporizador entre pré-ataque e ataque (nesse caso avanço)
    float _preattacktimer = 0f, _preattackduration = 1.25f;

    //Variaveis para o temporizador entre ataques (nesse caso avanços)
    float _preattackcdtimer = 0f, _preattackcdduration = 2.5f;

    //Variaveis para tempo de recuo do knockback
    float _recoilingtimer = 0f, _recoilingduration = 0.1f;

    //Variaveis entre janela de danos
    float _attackhittimetimer = 0f, _attackhittimeduration = 2f;

    //Variavel para direção do avanço ao jogador
    Vector2 _dashdir;

    public override void Update()
    {
        //Se HP for 0, ou seja morrer, não causa dano.
        if (HP <= 0) ATTACKHITTIME = false;

        // Definindo o centro do frame de acordo com a posição atual
        CENTER = position + origin * scale;

        //Marcador de contusão; caso inimigo receba dano ele fica invulneravel e recebe Knockback/Recoiling/Recuo, caso seja durante um pré-ataque ele reinicia a ação.
        if (INVULSTATE)
        {
            Recoling = true; //Recuo se torna verdadeiro
            _recoilingtimer = 0f; // Tempo de recuo
            PREATTACKSTATE = false; // Cancela o pré ataque e seu temporizador
            _preattacktimer = 0f; // Se ele estiver no fim de um pré ataque, restitue parte da recarga
            PREATTACKHITCD = true;
            if (ATTACKSTATE) //Caso esteja avançando....
            {
                ATTACKSTATE = false; //Cancela o avanço
                _preattackcdtimer = 0.5f; //Caso esteja no meio de um avanço, restitue parte da recarga
                _anims.Reset("swarm_Attack"); //Reinicia a animação de avanço
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

        //Esta unidade causa dano ao tocar no heroi estando em instancia de ataque ou não, ela tem tempo de recarga entre os danos causados no heroi.
        if (!ATTACKHITTIME)
        {
            _attackhittimetimer += (float)Globals.TotalSeconds;
            if (_attackhittimetimer >= _attackhittimeduration)
            {
                ATTACKHITTIME = true;
                _attackhittimetimer = 0f;
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
            }

        }



        if (PREATTACKSTATE) //Quando entre em préataque...
        {
            _dashdir = (Vector2.Normalize(CENTER - Globals.HEROLASTPOS)); //define a direção do avanço, sendo ele em direção ao heroi
        }
        else if (ATTACKSTATE && !Recoling) //Ao entrar em estado de Ataque e caso não esteja recebendo Dano...
        {
            int _dashspeed = 6; //Define velocidade do avanço
            position -= _dashdir * _dashspeed; //Avança na direção pré definida
        }

        //Trava para evitar bugs relacionado a ordem de carregamento do jogo
        if (MoveAI != null)
        {
            //Aplica um comportamento ao inimigo definido pelo GameManager.cs
            MoveAI.Move(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //Controladores de animação

        if (ATTACKSTATE && HP >= 0 && !Recoling) //Caso de Ataque
        {
            _anims.Update("swarm_Attack");
        }
        else if (PREATTACKSTATE && HP >= 0 && !Recoling)//Caso de pré-ataque
        {
            _anims.Update("swarm_yellow_hurt");
        }
        else
        {
            switch (_colorselect) // Muda a cor do esqueleto
            {
                case 1:
                    if (HP <= 0) //Caso de morte
                    {
                        _anims.Update("swarm_blue_death");
                    }
                    else if (Recoling)//Caso de Dano Recebido
                    {
                        _anims.Update("swarm_blue_hurt");
                    }
                    else if (walkState)//Caso de Andar
                    {
                        _anims.Update("swarm_blue");
                    }
                    else//Caso de parado
                    {
                        _anims.Update("swarm_blue");
                    };
                    break;
                case 2:
                    if (HP <= 0) //Caso de morte
                    {
                        _anims.Update("swarm_green_death");
                    }
                    else if (Recoling)//Caso de Dano Recebido
                    {
                        _anims.Update("swarm_green_hurt");
                    }
                    else if (walkState)//Caso de Andar
                    {
                        _anims.Update("swarm_green");
                    }
                    else//Caso de parado
                    {
                        _anims.Update("swarm_green");
                    };
                    break;
                case 3:
                    if (HP <= 0) //Caso de morte
                    {
                        _anims.Update("swarm_purple_death");
                    }
                    else if (Recoling)//Caso de Dano Recebido
                    {
                        _anims.Update("swarm_purple_hurt");
                    }
                    else if (walkState)//Caso de Andar
                    {
                        _anims.Update("swarm_purple");
                    }
                    else//Caso de parado
                    {
                        _anims.Update("swarm_purple");
                    };
                    break;
            }
        }

        //Gerenciador de espelhamento, faz com que o inimigo sempre fique em direção ao jogador
        if (!ATTACKSTATE)
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

        position = Vector2.Clamp(position, _minPos, _maxPos); // não permite que inimigo passe das bordas do mapa

    }


    public override void Draw()
    {


        //Reaction box test
        //Rectangle Erect = GetBounds($"reactionbox");
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

        //Passa os parametros para o AnimationManager animar o Spritesheet
        _anims.Draw(position, scale, mirror);

        //hitbox test
        //Rectangle Erect = GetBounds($"hitbox");
        //if (ATTACKHITTIME) Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

    }
}