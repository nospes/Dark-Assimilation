namespace MyGame;

public class enemySkeleton : enemyBase
{
    // Variaveis para Animações
    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _textureIdle;  //Spritesheets
    private static Texture2D _textureHit;

    //Definição de comportamento
    public MovementAI MoveAI { get; set; }

    public enemySkeleton(Vector2 pos)
    {
        //Atribuindo spritesheets a variaveis de Texture2D
        _textureIdle ??= Globals.Content.Load<Texture2D>("Creatures/Skeleton/Walk");
        _textureHit ??= Globals.Content.Load<Texture2D>("Creatures/Skeleton/Take Hit");

        //Definindo area,frames dos sprites sheets para fazer a animação
        _anims.AddAnimation("skeliidle", new(_textureIdle, 4, 1, 0.1f, 1, false));
        _anims.AddAnimation("skelihurt", new(_textureHit, 4, 1, 0.1f, 1, false));

        //Define a posição, velocidade e tamanho do sprite respectivamente
        _position = pos;
        _speed = 100f;
        _scale = 2;

        //Definição de origem e tamanho da caixa de colisão
        _baseHitBoxsize = new(38,53); // Tamanho
        var frameWidth = _textureIdle.Width / 4; // Dividindo os frames de acordo com tamanho do spritesheet
        var frameHeight = _textureIdle.Height / 1;
        _origin = new(frameWidth / 2, frameHeight / 2); //Atribui o centro do frame X e Y a um vetor
    }

    public override Rectangle GetBounds()
    {
        // Definindo o centro do frame de acordo com a posição atual
        float centerX = _position.X + _origin.X*_scale; 
        float centerY = _position.Y + _origin.Y*_scale;

        //Limites do topo e da esquerda da caixa de colisão
        int left = (int)centerX-(int)(_baseHitBoxsize.X*_scale)/2;
        int top = (int)centerY-(int)(_baseHitBoxsize.Y*_scale)/2;

        //Com base nas coordenadas Top e Left cria um retangulo de tamanho pré-definido multiplicado pelo scale
        return new Rectangle(left, top, (int)(_baseHitBoxsize.X*_scale), (int)(_baseHitBoxsize.Y*_scale));
    }

    public override void Update()
    {
        _anims.Update("skeliidle");

        //Trava para evitar bugs relacionado a ordem de carregamento do jogo
        if (MoveAI != null)
        {   
            //Aplica um comportamento ao inimigo definido pelo gameManager
            MoveAI.Move(this);
        }

    }

    public override void Draw()
    {
        //Passa os parametros para o AnimationManager animar o Spritesheet
        _anims.Draw(_position, _scale, _mirror);

        //hitbox test
        //Rectangle Erect = GetBounds();
        //Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);
    }
}