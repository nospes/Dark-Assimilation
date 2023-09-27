
namespace MyGame;

public class Animation
{
    //Variavel para receber texturas
    private readonly Texture2D _texture;

    //Area do Spritesheet que define o sprite
    private readonly List<Rectangle> _sourceRectangles = new();

    //Variaveis relacionadas a quantidade de frames, tempo entre frames e se o sprite está ativo ou não.
    private readonly int _frames;
    private int _frame;
    private readonly float _frameTime;
    private float _frameTimeLeft;
    private bool _active = true;
    private bool _isaplayer = false;


    public Animation(Texture2D texture, int framesX, int framesY, float frameTime, int row = 1, bool isaplayer = false)
    {
        //Aplicando Valores a um objeto único pra criação de sprites com a mesma solução
        _texture = texture;
        _frameTime = frameTime;
        _frameTimeLeft = _frameTime;
        _frames = framesX;
        _isaplayer = isaplayer;

        //Definindo área do sprite para animação
        var frameWidth = _texture.Width / framesX;
        var frameHeight = _texture.Height / framesY;


        //Define a área de cada animação no spritesheet de acordo com o proprio tamanho dele
        for (int i = 0; i < _frames; i++)
        {
            _sourceRectangles.Add(new(i * frameWidth, (row - 1) * frameHeight, frameWidth, frameHeight));
        }
    }

    

    public void Stop()
    {
        // Para a animação / Desativa
        _active = false;
    }

    public void Start()
    {
        // Inicia a animação / Permite que ela continue
        _active = true;
    }

    public void Reset()
    {
        //reinicia a animação
        _frame = 0;
        _frameTimeLeft = _frameTime;

    }

    public void Update()
    {
        //Se não está ativo, não atualiza o sprite
        if(!_active) return;

        //Tempo da animação é reduzida pelo tempo de jogo
        _frameTimeLeft -= Globals.TotalSeconds;

        //Utiliza o tempo de jogo para avançar de sprite para sprite no spritesheet
        if(_frameTimeLeft <= 0)
        {
            _frameTimeLeft += _frameTime;
            _frame = (_frame + 1) % _frames;
        }

        if(_isaplayer&&_frame==_frames-1&&InputManager._attacking) InputManager._attacking = false;
        //if(_frame==_frames)

    }

    public void Draw(Vector2 pos,int scale,bool fliped)
    {
        //Utilizando SpriteBatch localizado em Global.cs ele desenha na tela um sprite com parametros passados pelo Animation Manager.
        if(!fliped)
        Globals.SpriteBatch.Draw(_texture, pos, _sourceRectangles[_frame],Color.White, 0, Vector2.Zero, new Vector2(scale,scale), SpriteEffects.None, 1);
        else
        Globals.SpriteBatch.Draw(_texture, pos, _sourceRectangles[_frame],Color.White, 0, Vector2.Zero, new Vector2(scale,scale), SpriteEffects.FlipHorizontally, 1);
    }
}