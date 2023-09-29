namespace MyGame;

public class enemySkeleton
{
    // Animações
    private readonly AnimationManager _anims = new();
    private static Texture2D _textureIdle;
    private static Texture2D _textureHit;

    // Atributos
    private Vector2 _position;
    private readonly float _speed = 200f;
    private readonly int _scale = 2;
    private bool _mirror;

    // Atributos do Hitbox
    private static Vector2 _originHitbounds;
    public static Vector2 _posHitbounds;
    public static Vector2 _hitBounds;

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

        //Ponto inicial da Hitbox
        _originHitbounds.X = 60 * _scale;
        _originHitbounds.Y = 50 * _scale;

        //60 50
        //Tamanho da Hitbox
        _hitBounds.X = 38 * _scale;
        _hitBounds.Y = 52 * _scale;

        //38 52
    }

    public Rectangle GetBounds()
    {
        _posHitbounds.X = _position.X + _originHitbounds.X;
        _posHitbounds.Y = _position.Y + _originHitbounds.Y;
        return new Rectangle((int)_posHitbounds.X, (int)_posHitbounds.Y, (int)_hitBounds.X, (int)_hitBounds.Y);

    }

    public void Update()
    {
        _anims.Update("skeliidle");


    }

    public void Draw()
    {
        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        _anims.Draw(_position, _scale, _mirror);
    }
}