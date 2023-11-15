namespace MyGame;

public class Hero
{

    //Posição e Velocidade
    public Vector2 POSITION { get; set; }
    private float _speed { get; set; }

    // Animações
    private readonly AnimationManager _anims = new();
    private static Texture2D _textureIdle, _textureMove, _textureAttack, _textureCAST, _textureDASH;
    private readonly int _scale = 3;
    private bool _mirror { get; set; }
    private Vector2 _origin { get; set; }


    //Atributos do hitbox
    private Vector2 _baseHitBoxsize, _minPos, _maxPos;
    public static Vector2 SCALEDHITBOXSIZE;
    public Vector2 CENTER;

    //Estados
    public static bool ATTACKING = false, ATTACKHITTIME = false, CAST = false, DASH = false;

    //Gerenciadores de tempo de recarga
    public static SkillManager dashCD, skillCD, attackCD;
    private bool _dashCDlock = true, _skillCDlock = true, _attackCDlock = true;

    public Hero(Vector2 pos)
    {
        //Definindo texturas
        _textureIdle ??= Globals.Content.Load<Texture2D>("Player/hero.Idle");
        _textureMove ??= Globals.Content.Load<Texture2D>("Player/hero.Run");
        _textureAttack ??= Globals.Content.Load<Texture2D>("Player/hero.Attack");
        _textureCAST ??= Globals.Content.Load<Texture2D>("Player/hero.CAST");
        _textureDASH ??= Globals.Content.Load<Texture2D>("Player/hero.DASH");


        //Definindo area dos sprites sheets para fazer a animação
        _anims.AddAnimation(0, new(_textureIdle, 4, 1, 0.2f, 1, true));
        _anims.AddAnimation(1, new(_textureMove, 6, 1, 0.1f, 1, true));
        _anims.AddAnimation(2, new(_textureAttack, 20, 1, 0.07f, 1, true));
        _anims.AddAnimation(3, new(_textureCAST, 9, 1, 0.09f, 1, true));
        _anims.AddAnimation(4, new(_textureDASH, 5, 1, 0.1f, 1, true));

        //Define a posição e velocidade
        POSITION = pos;
        _speed = 200;


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


        //Cria um objeto para gerenciar o cooldown do DASH e da magia respectivamente
        dashCD = new SkillManager();
        skillCD = new SkillManager();
        attackCD = new SkillManager();



    }

    public Rectangle GetBounds()
    {
        int _centeroffsetX;
        if (InputManager.Moving && !_mirror) _centeroffsetX = 20;
        else if (InputManager.Moving && _mirror) _centeroffsetX = -20;
        else _centeroffsetX = 0;

        int _centeroffsetY;
        if (Hero.ATTACKING) _centeroffsetY = +10;
        else _centeroffsetY = 0;


        int left = (int)CENTER.X + _centeroffsetX - (int)SCALEDHITBOXSIZE.X / 2;
        int top = (int)CENTER.Y + _centeroffsetY - (int)SCALEDHITBOXSIZE.Y / 2;

        return new Rectangle(left, top, (int)SCALEDHITBOXSIZE.X, (int)SCALEDHITBOXSIZE.Y);
    }

    public void MapBounds(Point mapSize, Point tileSize)
    {
        _minPos = new((-tileSize.X / 2) - SCALEDHITBOXSIZE.X, (-tileSize.Y / 2));
        _maxPos = new(mapSize.X - (tileSize.X / 2) - CENTER.X - 120, mapSize.Y - (tileSize.X / 2) - CENTER.Y - 110);
    }


    public Rectangle AttackBounds()
    {

        var _hitsize = new Vector2(24 * _scale, 30 * _scale);

        if (!_mirror)
            return new Rectangle((int)CENTER.X, (int)(CENTER.Y - _hitsize.Y / 2), (int)_hitsize.X, (int)_hitsize.Y);
        else
            return new Rectangle((int)(CENTER.X - _hitsize.X), (int)(CENTER.Y - _hitsize.Y / 2), (int)_hitsize.X, (int)_hitsize.Y);
    }



    public void Update()
    {
        //define speed
        if (DASH) _speed = 500;
        else _speed = 200;

        //Atualiza o centro
        CENTER = POSITION + _origin * _scale;

        //Movimenta o jogador com os comandos dado pelo Inputmanager.cs
        if (!ATTACKING && !CAST)
        {
            if (InputManager.Moving)
            {
                POSITION += Vector2.Normalize(InputManager.Direction) * _speed * Globals.TotalSeconds;
            }
            else if (DASH)
            {
                POSITION += Vector2.Normalize(InputManager.Lastdir) * _speed * Globals.TotalSeconds;
            }
            POSITION = Vector2.Clamp(POSITION, _minPos, _maxPos);
        }

        //Define uma animação de acordo com a tecla apertada, caso nenhuma esteja ele volta para Idle.
        if (CAST) _anims.Update(3);
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
                Console.WriteLine("Cooldown de 1 terminado. Você pode realizar a ação agora.");
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
        //Console.WriteLine($"Cooldown de {cooldownDuration} terminado. Você pode realizar a ação agora.");

        //cooldown da skill
        skillCD.skillCooldown(3.00f, () =>
            {
                Console.WriteLine("Cooldown de 3 terminado. Você pode realizar a ação agora.");
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
                Console.WriteLine("Cooldown de 0,15 terminado. Você pode realizar a ação agora.");
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