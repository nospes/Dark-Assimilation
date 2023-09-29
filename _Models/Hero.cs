namespace MyGame;

public class Hero
{
    // Animações
    private readonly AnimationManager _anims = new();
    private static Texture2D _textureIdle;
    private static Texture2D _textureMove;
    private static Texture2D _textureJab;



    // Atributos
    private Vector2 _position;
    private readonly float _speed = 200f;
    private readonly int _scale = 3;
    private bool _mirror;

    //Atributos do hitbox
    private static Vector2 _originHitbounds;
    public static Vector2 _posHitbounds;
    public static Vector2 _hitBounds;



    public Hero(Vector2 pos)
    {
        //Definindo texturas
        _textureMove ??= Globals.Content.Load<Texture2D>("Player/run cycle 48x48");
        _textureIdle ??= Globals.Content.Load<Texture2D>("Player/PlayerIdle_spr");
        _textureJab ??= Globals.Content.Load<Texture2D>("Player/Player Jab 48x48");


        //Definindo area dos sprites sheets para fazer a animação
        _anims.AddAnimation(0, new(_textureIdle, 10, 1, 0.1f, 1, true));
        _anims.AddAnimation(1, new(_textureMove, 8, 1, 0.1f, 1, true));
        _anims.AddAnimation(2, new(_textureJab, 10, 1, 0.09f, 1, true));

        //Define a posição
        _position = pos;

        //Ponto inicial da Hitbox
        _originHitbounds.X = 19 * _scale;
        _originHitbounds.Y = 15 * _scale;

        //Tamanho da hitbox
        _hitBounds.X = 10 * _scale;
        _hitBounds.Y = 24 * _scale;



    }

    public Rectangle GetBounds()
    {
        _posHitbounds.X = _position.X + _originHitbounds.X;
        _posHitbounds.Y = _position.Y + _originHitbounds.Y;
        return new Rectangle((int)_posHitbounds.X, (int)_posHitbounds.Y, (int)_hitBounds.X, (int)_hitBounds.Y);
    }

    // 28 18 origin
    // 15 15 size
    public Rectangle AttackBounds()
    {
        if(!_mirror)
        return new Rectangle((int)_posHitbounds.X+28, (int)_posHitbounds.Y+15, 60, 30); 
        else
        return new Rectangle((int)_posHitbounds.X-52, (int)_posHitbounds.Y+15, 60, 30);
    }



    public void Update()
    {
        //Movimenta o jogador com os comandos dado pelo Inputmanager.cs
        if (InputManager.Moving && !InputManager._attacking)
        {
            _position += Vector2.Normalize(InputManager.Direction) * _speed * Globals.TotalSeconds;
        }

        //Define uma animação de acordo com a tecla apertada, caso nenhuma esteja ele volta para Idle.
        if (InputManager._attacking) _anims.Update(2);
        else if (InputManager.Direction.X < 0)
        {
            _anims.Update(1);
            _mirror = true;
        }
        else if (InputManager.Direction.X > 0)
        {
            _anims.Update(1);
            //Espelha o spritesheet de acordo com a ultima direção andada
            _mirror = false;
        }
        else if (InputManager.Direction.Y > 0 || InputManager.Direction.Y < 0)
            _anims.Update(1);
        else
            _anims.Update(0);
    }
    //possível forma de solução para problema de como encaixar animação de atacar: colocar no else if a animação 
    //de atacar e condições nas outras OU dar um return ao final do update em animation.cs e colocar alguma 
    //trava que só libere o sprite ao receber o RETURN do fim da animação é talvez colocar uma trava 
    //bool no animation Manager 
    public void Draw()
    {
        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        _anims.Draw(_position, _scale, _mirror);
    }
}