namespace MyGame;

public class Hero
{
    // Animações
    private readonly AnimationManager _anims = new();
    private static Texture2D _textureIdle;
    private static Texture2D _textureMove;

    // Atributos
    private Vector2 _position;
    private readonly float _speed = 200f;
    private readonly int _scale = 3;
    private bool _mirror;

    public Hero(Vector2 pos)
    {
        //Definindo texturas
        _textureMove ??= Globals.Content.Load<Texture2D>("Player/run cycle 48x48");
        _textureIdle ??= Globals.Content.Load<Texture2D>("Player/PlayerIdle_spr");

        //Definindo area dos sprites sheets para fazer a animação
        _anims.AddAnimation(0, new(_textureIdle, 10, 1, 0.1f, 1));
        _anims.AddAnimation(1, new(_textureMove, 8, 1, 0.1f, 1));

        //Define a posição
        _position = pos;

    }

    public void Update()
    {
        //Movimenta o jogador com os comandos dado pelo Inputmanager.cs
        if (InputManager.Moving)
        {
            _position += Vector2.Normalize(InputManager.Direction) * _speed * Globals.TotalSeconds;
        }

        //Define uma animação de acordo com a tecla apertada, caso nenhuma esteja ele volta para Idle.
        if (InputManager.Direction.X > 0)
        {
            _anims.Update(1);
            //Espelha o spritesheet de acordo com a ultima direção andada
            _mirror = false;
        }
        else if(InputManager.Direction.X < 0)
        {
            _anims.Update(1);
            _mirror = true;
        }
        else if(InputManager.Direction.Y > 0 || InputManager.Direction.Y < 0) 
            _anims.Update(1);
        else 
            _anims.Update(0);
    }
//possível forma de solução para problema de como encaixar animação de atacar: colocar no else if a animação de atacar e condições nas outras
    public void Draw()
    {
        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        _anims.Draw(_position,_scale,_mirror);
    }
}