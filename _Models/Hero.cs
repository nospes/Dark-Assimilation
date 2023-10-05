namespace MyGame;

public class Hero : playerBase
{
    // Animações
    private readonly AnimationManager _anims = new();
    private static Texture2D _textureIdle;
    private static Texture2D _textureMove;
    private static Texture2D _textureJab;
    private readonly int _scale = 3;


    //Atributos do hitbox
    private Vector2 _baseHitBoxsize;

    public static Vector2 _scaledHitBoxsize;

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

        //Define a posição e velocidade
        _position = pos;
        _speed = 200;


        // Tamanho base da hitbox
        _baseHitBoxsize.X = 10;
        _baseHitBoxsize.Y = 24;

        //Tamanho da Hitbox escalada
        _scaledHitBoxsize.X = _baseHitBoxsize.X * _scale;
        _scaledHitBoxsize.Y = _baseHitBoxsize.Y * _scale;

        //Centro do Frame
        var frameWidth = _textureIdle.Width / 10;
        var frameHeight = _textureIdle.Height / 1;
        _origin = new(frameWidth / 2, frameHeight / 2);

    }

    public Rectangle GetBounds()
    {

        float centerX = _position.X + _origin.X * _scale;
        float centerY = _position.Y + _origin.Y * _scale;

        int left = (int)centerX - (int)_scaledHitBoxsize.X / 2;
        int top = (int)centerY - (int)_scaledHitBoxsize.Y / 2;

        return new Rectangle(left, top, (int)_scaledHitBoxsize.X, (int)_scaledHitBoxsize.Y);
    }

    // 28 18 origin
    // 15 15 size
    public Rectangle AttackBounds()
    {
        if (!_mirror)
            return new Rectangle((int)_origin.X + 28, (int)_origin.Y + 15, 60, 30);
        else
            return new Rectangle((int)_origin.X - 52, (int)_origin.Y + 15, 60, 30);
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

        //hitbox
        //Rectangle rect = GetBounds();
        //Globals.SpriteBatch.Draw(Game1.pixel, rect, Color.Red);
        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        _anims.Draw(_position, _scale, _mirror);



    }
}