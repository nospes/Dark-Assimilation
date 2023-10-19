namespace MyGame;

public class Hero
{

    //Posição e Velocidade
    public Vector2 Position { get; set; }
    private float _speed { get; set; }

    // Animações
    private readonly AnimationManager _anims = new();
    private static Texture2D _textureIdle, _textureMove, _textureAttack, _textureCast, _textureDash;
    private readonly int _scale = 3;
    private bool _mirror { get; set; }
    private Vector2 _origin { get; set; }


    //Atributos do hitbox
    private Vector2 _baseHitBoxsize, _minPos, _maxPos;
    public static Vector2 ScaledHitboxSize;
    public Vector2 Center;

    //Estados
    public static bool Attacking = false, Cast = false, Dash = false;

    //Gerenciadores de tempo de recarga
    public static SkillManager dashCD, skillCD;
    private bool _dashCDlock = false, _skillCDlock = false;

    public Hero(Vector2 pos)
    {
        //Definindo texturas
        _textureIdle ??= Globals.Content.Load<Texture2D>("Player/hero.Idle");
        _textureMove ??= Globals.Content.Load<Texture2D>("Player/hero.Run");
        _textureAttack ??= Globals.Content.Load<Texture2D>("Player/hero.Attack");
        _textureCast ??= Globals.Content.Load<Texture2D>("Player/hero.Cast");
        _textureDash ??= Globals.Content.Load<Texture2D>("Player/hero.Dash");


        //Definindo area dos sprites sheets para fazer a animação
        _anims.AddAnimation(0, new(_textureIdle, 4, 1, 0.2f, 1, true));
        _anims.AddAnimation(1, new(_textureMove, 6, 1, 0.1f, 1, true));
        _anims.AddAnimation(2, new(_textureAttack, 20, 1, 0.09f, 1, true));
        _anims.AddAnimation(3, new(_textureCast, 9, 1, 0.09f, 1, true));
        _anims.AddAnimation(4, new(_textureDash, 5, 1, 0.1f, 1, true));

        //Define a posição e velocidade
        Position = pos;
        _speed = 200;


        // Tamanho base da hitbox
        _baseHitBoxsize.X = 10;
        _baseHitBoxsize.Y = 25;


        //Tamanho da Hitbox escalada
        ScaledHitboxSize.X = _baseHitBoxsize.X * _scale;
        ScaledHitboxSize.Y = _baseHitBoxsize.Y * _scale;

        //Centro do Frame
        var frameWidth = _textureIdle.Width / 4;
        var frameHeight = _textureIdle.Height / 1;
        _origin = new(frameWidth / 2, frameHeight / 2);


        //Cria um objeto para gerenciar o cooldown do dash e da magia respectivamente
        dashCD = new SkillManager();
        skillCD = new SkillManager();



    }

    public Rectangle GetBounds()
    {
        int _centerOffsetX;
        if (InputManager.Moving && !_mirror) _centerOffsetX = 20;
        else if (InputManager.Moving && _mirror) _centerOffsetX = -20;
        else _centerOffsetX = 0;

        int _centerOffsetY;
        if (Hero.Attacking) _centerOffsetY = +10;
        else _centerOffsetY = 0;


        int left = (int)Center.X + _centerOffsetX - (int)ScaledHitboxSize.X / 2;
        int top = (int)Center.Y + _centerOffsetY - (int)ScaledHitboxSize.Y / 2;

        return new Rectangle(left, top, (int)ScaledHitboxSize.X, (int)ScaledHitboxSize.Y);
    }

    public void MapBounds(Point mapSize, Point tileSize)
    {
        _minPos = new((-tileSize.X / 2) + Center.X, (-tileSize.Y / 2) + Center.Y);
        _maxPos = new(mapSize.X - (tileSize.X / 2) - Center.X - 120, mapSize.Y - (tileSize.X / 2) - Center.Y - 120);
    }


    public Rectangle AttackBounds()
    {

        var _hitsize = new Vector2(25 * _scale, 30 * _scale);

        if (!_mirror)
            return new Rectangle((int)Center.X, (int)(Center.Y - _hitsize.Y / 2), (int)_hitsize.X, (int)_hitsize.Y);
        else
            return new Rectangle((int)(Center.X - _hitsize.X), (int)(Center.Y - _hitsize.Y / 2), (int)_hitsize.X, (int)_hitsize.Y);
    }



    public void Update()
    {

        Center = Position + _origin * _scale;

        //Movimenta o jogador com os comandos dado pelo Inputmanager.cs
        if (InputManager.Moving && !Attacking && !Cast)
        {
            Position += Vector2.Normalize(InputManager.Direction) * _speed * Globals.TotalSeconds;
            Position = Vector2.Clamp(Position, _minPos, _maxPos);
        }

        //Define uma animação de acordo com a tecla apertada, caso nenhuma esteja ele volta para Idle.
        if (Cast) _anims.Update(3);
        else if (Attacking) _anims.Update(2);
        else if (Dash) _anims.Update(4);
        else if (InputManager.Direction != Vector2.Zero)
        {
            _anims.Update(1);
        }
        else
        {
            _anims.Update(0);
        }

        if (Dash) _speed = 500;
        else _speed = 200;


        //Espelha o sprite de acordo com a direção
        if (InputManager.Direction.X > 0) _mirror = false;
        else if (InputManager.Direction.X < 0) _mirror = true;


        //atualiza o tempo de recarga da ação com base no valor passado
        dashCD.skillCooldown(1.00f);
        if (!Dash)
        {
            if (!_dashCDlock)
            {
                dashCD.CheckCooldown = true;
                _dashCDlock = true;
            }
        }
        else _dashCDlock = false;

        skillCD.skillCooldown(3.00f);
        if (!Cast)
        {
            if (!_skillCDlock)
            {
                skillCD.CheckCooldown = true;
                _skillCDlock = true;
            }
        }
        else _skillCDlock = false;
    }

    public void Draw()
    {

        //hitbox check

        /*Rectangle rect = AttackBounds();
        Globals.SpriteBatch.Draw(Game1.pixel, rect, Color.Red);*/

        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        _anims.Draw(Position, _scale, _mirror);




    }
}