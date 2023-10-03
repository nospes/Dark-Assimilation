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
 
    //Variaveis para Direção de movimento
    private Vector2 _direction;
    private Vector2 _playerpos;

    // Atributos do Hitbox
    private static Vector2 _originHitbounds;
    public static Vector2 _posHitbounds;
    public static Vector2 _hitBounds;
    private static Vector2 _hitboxOrigin;
    private static Vector2 _hitboxSize;

    public enemySkeleton(Vector2 pos)
    {
        //Definindo texturas
        _textureIdle ??= Globals.Content.Load<Texture2D>("Creatures/Goblin/Run");
        _textureHit ??= Globals.Content.Load<Texture2D>("Creatures/Skeleton/Take Hit");

        //Definindo area dos sprites sheets para fazer a animação
        _anims.AddAnimation("skeliidle", new(_textureIdle, 8, 1, 0.1f, 1, false));
        _anims.AddAnimation("skelihurt", new(_textureHit, 4, 1, 0.1f, 1, false));

        //Define a posição
        _position = pos;

        //Definição da origem e tamanho da hitbox
        // Origem
        _hitboxOrigin.X = 60;
        _hitboxOrigin.Y = 50;
        // Tamanho
        _hitboxSize.X = 38;
        _hitboxSize.Y = 52;

        //Ponto inicial da Hitbox
        _originHitbounds.X = _hitboxOrigin.X * _scale;
        _originHitbounds.Y = _hitboxOrigin.Y * _scale;

        //Tamanho da Hitbox
        _hitBounds.X = _hitboxSize.X * _scale;
        _hitBounds.Y = _hitboxSize.Y * _scale;

        //Direção de movimento
        _direction.X = 0;
        _direction.Y = 0;


    }

    public Rectangle GetBounds()
    {
        _posHitbounds.X = _position.X + _originHitbounds.X / _scale;
        _posHitbounds.Y = _position.Y + _originHitbounds.Y / _scale;
        return new Rectangle((int)_posHitbounds.X, (int)_posHitbounds.Y, (int)_hitBounds.X, (int)_hitBounds.Y);

    }

    public void Update()
    {
        _anims.Update("skeliidle");
        _playerpos = Hero._position;
        _direction.X = _playerpos.X - _position.X - _originHitbounds.X + _hitboxSize.X;
        _direction.Y = _playerpos.Y - _position.Y - _originHitbounds.Y + _hitboxSize.Y/2;
        /*
        if(_position.X <= 600)
        _direction.X = -1;
        else if(_position.X <= 0)
        _direction.X = 1;
        */
        if (_direction.X >= 3 || _direction.X <= -3 || _direction.Y >= 3 || _direction.Y <= -3)
        {
            _direction.Normalize();
            _position += _direction * _speed * Globals.TotalSeconds;
            if (_direction.X > 0)
                _mirror = false;
            else if (_direction.X < 0)
                _mirror = true;
        }


    }

    public void Draw()
    {
        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        _anims.Draw(_position, _scale, _mirror);
    }
}