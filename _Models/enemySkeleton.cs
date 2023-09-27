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
    private readonly int _scale = 3;
    private bool _mirror;

    public enemySkeleton(Vector2 pos){
         //Definindo texturas
        _textureIdle ??= Globals.Content.Load<Texture2D>("Creatures/Skeleton/Idle");
        _textureHit ??= Globals.Content.Load<Texture2D>("Creatures/Skeleton/Take Hit");

        //Definindo area dos sprites sheets para fazer a animação
        _anims.AddAnimation("skelidle", new(_textureIdle, 4, 1, 0.1f, 1, false));
        _anims.AddAnimation("skelihurt", new(_textureHit, 4, 1, 0.1f, 1, false));

        //Define a posição
        _position = pos;
    }

        public void Update()
    {
        _anims.Update(0);
    }

    public void Draw()
    {
        //Passa os parametros de desenho apra AnimationManager.cs definir de fato os atributos do seu Spritesheet para então passar para Animation.cs
        _anims.Draw(_position,_scale,_mirror);
    }
}