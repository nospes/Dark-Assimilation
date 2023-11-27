namespace MyGame;

public class Hero
{

    //Posição e Velocidade
    public Vector2 POSITION { get; set; } //Posição do jogador
    private float _speed { get; set; } //Velocidade do jogador

    // Animações
    private readonly AnimationManager _anims = new(); //Gerenciador de animações
    private static Texture2D _textureIdle, _textureMove, _textureAttack, _textureCast, _textureDash, _textureDeath, _textureRecoil; //Texturas
    private readonly int _scale = 3; //Escalonamento do sprite
    private bool _mirror { get; set; } //Espelhamento do sprite
    private Vector2 _origin { get; set; } //Origem do centro do sprite


    //Atributos do hitbox
    private Vector2 _baseHitBoxsize, _minPos, _maxPos; //Tamanho da hitbox base e posições maximas e minimas do mapa
    public static Vector2 SCALEDHITBOXSIZE; //Hitbox escalonada
    public Vector2 CENTER; //Centro do sprite escalonado e atualizado com a posição atual

    //Estados
    public static bool ATTACKING = false, ATTACKHITTIME = false, CAST = false, DASH = false, RECOIL = false, DEATH = false; //Variaveis de estados do jogador

    //Atributos de combate
    public int HP; //Vida
    public static Vector2 lastHitpos; //Guarda posição do ultimo inimigo que acertou o heroi, utilizado no calculo de Knockback

    //Gerenciadores de tempo de recarga
    public static SkillManager dashCD, skillCD, attackCD; //Gerenciadores
    private bool _dashCDlock = true, _skillCDlock = true, _attackCDlock = true; //Variaveis de controle para os gerenciadores


    //Definindo bases do Hero/Jogador
    public Hero(Vector2 pos)
    {
        //Definindo texturas
        _textureIdle ??= Globals.Content.Load<Texture2D>("Player/hero.Idle");
        _textureMove ??= Globals.Content.Load<Texture2D>("Player/hero.Run");
        _textureAttack ??= Globals.Content.Load<Texture2D>("Player/hero.Attack");
        _textureCast ??= Globals.Content.Load<Texture2D>("Player/hero.Cast");
        _textureDash ??= Globals.Content.Load<Texture2D>("Player/hero.Dash");
        _textureDeath ??= Globals.Content.Load<Texture2D>("Player/hero.Death");
        _textureRecoil ??= Globals.Content.Load<Texture2D>("Player/hero.Hurt");



        //Definindo area dos sprites sheets para fazer a animação
        _anims.AddAnimation(0, new(_textureIdle, 4, 1, 0.2f, 1));
        _anims.AddAnimation(1, new(_textureMove, 6, 1, 0.1f, 1));
        _anims.AddAnimation(2, new(_textureAttack, 20, 1, 0.07f, 1));
        _anims.AddAnimation(3, new(_textureCast, 9, 1, 0.09f, 1));
        _anims.AddAnimation(4, new(_textureDash, 5, 1, 0.1f, 1));
        _anims.AddAnimation(5, new(_textureDeath, 5, 1, 0.2f, 1));
        _anims.AddAnimation(6, new(_textureRecoil, 4, 1, 0.04f, 1));

        //Definição de Atributos do jogador
        POSITION = pos;
        _speed = 200;
        HP = 100;


        // Tamanho base da hitbox
        _baseHitBoxsize.X = 10;
        _baseHitBoxsize.Y = 25;


        //Tamanho da Hitbox escalada
        SCALEDHITBOXSIZE.X = _baseHitBoxsize.X * _scale;
        SCALEDHITBOXSIZE.Y = _baseHitBoxsize.Y * _scale;

        //Centro do Frame
        var frameWidth = _textureIdle.Width / 4;
        var frameHeight = _textureIdle.Height / 1;
        _origin = new(frameWidth / 2, frameHeight / 2);


        //Cria objetos para gerenciar o cooldowns
        dashCD = new SkillManager();
        skillCD = new SkillManager();
        attackCD = new SkillManager();



    }

    //Pega os limites do jogador
    public Rectangle GetBounds()
    {
        int _centeroffsetX;//variavel para auxilhar no ajuste de posição
        //A seguir ele ajusta a hitbox de acordo com a ação e espelhamento
        //Hitbox se movimentando
        if (InputManager.Moving && !_mirror) _centeroffsetX = 20;
        //Hitbox espelhada
        else if (InputManager.Moving && _mirror) _centeroffsetX = -20;
        //Caso nenhum dos dois, não há ajuste
        else _centeroffsetX = 0;

        int _centeroffsetY;
        //Hitbox quando está atacando
        if (Hero.ATTACKING) _centeroffsetY = +10;
        else _centeroffsetY = 0;

        //Define os cantos superior e esquerdo da sprite.
        int left = (int)CENTER.X + _centeroffsetX - (int)SCALEDHITBOXSIZE.X / 2;
        int top = (int)CENTER.Y + _centeroffsetY - (int)SCALEDHITBOXSIZE.Y / 2;

        //Após todo o ajuste e casos aplicados, retorna um retangulo ajustado
        return new Rectangle(left, top, (int)SCALEDHITBOXSIZE.X, (int)SCALEDHITBOXSIZE.Y);
    }

    // Define até onde o jogador pode se movimentar
    public void MapBounds(Point mapSize, Point tileSize)
    {
        _minPos = new((-tileSize.X / 2) - SCALEDHITBOXSIZE.X, (-tileSize.Y / 2)); //Limite esquerda e cima (limites minimos)
        _maxPos = new(mapSize.X - (tileSize.X / 2) - CENTER.X - 120, mapSize.Y - (tileSize.X / 2) - CENTER.Y - 110); //Limite direita e baixo (limites minimos)
    }

    //Define os limites dos golpes do jogador
    public Rectangle AttackBounds()
    {

        var _hitsize = new Vector2(24 * _scale, 30 * _scale); //Define o tamanho

        if (!_mirror) //Caso não esteja espelhado ele retorna o seguinte retangulo
            return new Rectangle((int)CENTER.X, (int)(CENTER.Y - _hitsize.Y / 2), (int)_hitsize.X, (int)_hitsize.Y);
        else //Caso esteja
            return new Rectangle((int)(CENTER.X - _hitsize.X), (int)(CENTER.Y - _hitsize.Y / 2), (int)_hitsize.X, (int)_hitsize.Y);
    }


    //Variaveis de temporizadores
    float _recoiltimer = 0f, _recoiltimerduration = 0.3f; //Contador e Duração de invulnerabilidade
    public void Update()
    {
        //Tempo de invulnerabilidade após levar dano
        if (RECOIL)
        {
            POSITION += (Vector2.Normalize((CENTER - lastHitpos)))*2;
            //Temporizador para fim da invulnerabilidade
            _recoiltimer += (float)Globals.TotalSeconds;
            if (_recoiltimer >= _recoiltimerduration)
            {
                RECOIL = false;
                _recoiltimer = 0f;
            }
        }


        //define speed, aumentando-o caso esteja durante o dash
        if (DASH) _speed = 500;
        else _speed = 200;

        //Atualiza o centro do sprite utilizando posição atual + origem base + escalonamento da imagem
        CENTER = POSITION + _origin * _scale;

        //Movimenta o jogador com os comandos dado pelo Inputmanager.cs
        if (!ATTACKING && !CAST && !RECOIL)
        {
            if (InputManager.Moving) // Caso esteja se movendo ele anda nas direções do 'Direction' com base na speed e no tempo de jogo
            {
                POSITION += Vector2.Normalize(InputManager.Direction) * _speed * Globals.TotalSeconds;
            }
            else if (DASH) //A mesma coisa que movimento, porem como um avanço rapido
            {
                POSITION += Vector2.Normalize(InputManager.Lastdir) * _speed * Globals.TotalSeconds;
            }
            POSITION = Vector2.Clamp(POSITION, _minPos, _maxPos); //Caso o jogador tente ultrapassar os limites do mapa ele retorna
        }

        //Se a vida chega a 0 entra em estado de morte
        //if(HP<=0) DEATH = true;
        //Define uma animação de acordo com a tecla apertada, caso nenhuma esteja ele volta para Idle.
        if(DEATH) _anims.Update(5);
        else if (RECOIL) _anims.Update(6);
        else if (CAST) _anims.Update(3);
        else if (ATTACKING) _anims.Update(2);
        else if (DASH) _anims.Update(4);
        else if (InputManager.Direction != Vector2.Zero)
        {
            _anims.Update(1);
        }
        else
        {
            _anims.Update(0);
        }




        //Espelha o sprite de acordo com a direção
        if (InputManager.Direction.X > 0) _mirror = false;
        else if (InputManager.Direction.X < 0) _mirror = true;


        //atualiza o tempo de recarga da ação com base no valor passado
        //cooldown do DASH
        dashCD.skillCooldown(1.00f, () =>
            {
                //Console.WriteLine("Cooldown de 1 terminado. Você pode realizar a ação agora.");
            });
        if (!DASH)
        {
            if (!_dashCDlock)
            {
                dashCD.CheckCooldown = true;
                _dashCDlock = true;
            }
        }
        else _dashCDlock = false;

        //cooldown da skill
        skillCD.skillCooldown(3.00f, () =>
            {
                //Console.WriteLine("Cooldown de 3 terminado. Você pode realizar a ação agora.");
            });
        if (!CAST)
        {
            if (!_skillCDlock)
            {
                skillCD.CheckCooldown = true;
                _skillCDlock = true;
            }
        }
        else _skillCDlock = false;

        //cooldown do autoattack
        attackCD.skillCooldown(0.15f, () =>
            {
                //Console.WriteLine("Cooldown de 0,15 terminado. Você pode realizar a ação agora.");
            });
        if (!ATTACKING)
        {
            if (!_attackCDlock)
            {
                attackCD.CheckCooldown = true;
                _attackCDlock = true;
            }
        }
        else _attackCDlock = false;


    }



    public void Draw()
    {

        //hitbox check
        Rectangle rect = AttackBounds();
        if (ATTACKHITTIME) Globals.SpriteBatch.Draw(Game1.pixel, rect, Color.Red);

        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        _anims.Draw(POSITION, _scale, _mirror);




    }
}