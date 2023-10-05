namespace MyGame;

public class enemySkeleton : enemyBase
{
    // Animações
    private readonly AnimationManager _anims = new();
    private static Texture2D _textureIdle;
    private static Texture2D _textureHit;

    private readonly int _scale = 2;


    // Atributos do Hitbox
    public static Vector2 _posHitbounds;
    public static Vector2 _scaledHitBoxsize;
    private static Vector2 _baseHitBoxsize;

    public MovementAI MoveAI { get; set; }

    public enemySkeleton(Vector2 pos)
    {
        //Definindo texturas
        _textureIdle ??= Globals.Content.Load<Texture2D>("Creatures/Skeleton/Walk");
        _textureHit ??= Globals.Content.Load<Texture2D>("Creatures/Skeleton/Take Hit");

        //Definindo area dos sprites sheets para fazer a animação
        _anims.AddAnimation("skeliidle", new(_textureIdle, 4, 1, 0.1f, 1, false));
        _anims.AddAnimation("skelihurt", new(_textureHit, 4, 1, 0.1f, 1, false));

        //Define a posição
        _position = pos;
        _speed = 100f;

        //Definição da origem e tamanho da hitbox

        // Tamanho
        _baseHitBoxsize.X = 38;
        _baseHitBoxsize.Y = 53;

        //Tamanho da Hitbox
        _scaledHitBoxsize.X = _baseHitBoxsize.X * _scale;
        _scaledHitBoxsize.Y = _baseHitBoxsize.Y * _scale;

        //Centro do Frame
        var frameWidth = _textureIdle.Width / 4;
        var frameHeight = _textureIdle.Height / 1;
        _origin = new(frameWidth / 2, frameHeight / 2);
    }

    public Rectangle GetBounds()
    {
        
        float centerX = _position.X + _origin.X*_scale;
        float centerY = _position.Y + _origin.Y*_scale;

        int left = (int)centerX-(int)_scaledHitBoxsize.X/2;
        int top = (int)centerY-(int)_scaledHitBoxsize.Y/2;

        return new Rectangle(left, top, (int)_scaledHitBoxsize.X, (int)_scaledHitBoxsize.Y);
    }

    public void Update()
    {
        _anims.Update("skeliidle");

        if (MoveAI != null)
        {
            MoveAI.Move(this);
        }

    }

    public void Draw()
    {
        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        _anims.Draw(_position, _scale, _mirror);

        //hitbox
        //Rectangle Erect = GetBounds();
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);
    }
}