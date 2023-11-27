
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

    //Metodo de acesso de objetos especificos(inimigos)
    private readonly enemyCollection _enemy;
    private Type _objType;

    public Animation(Texture2D texture, int framesX, int framesY, float frameTime, int row = 1, enemyCollection enemyInstance = null)
    {
        //Definição dos parametros para montagem do spritesheet
        _texture = texture;
        _frameTime = frameTime;
        _frameTimeLeft = _frameTime;
        _frames = framesX;

        //Definindo área do sprite para animação
        var frameWidth = _texture.Width / framesX;
        var frameHeight = _texture.Height / framesY;


        //Define a área de cada animação no spritesheet de acordo com o proprio tamanho dele
        for (int i = 0; i < _frames; i++)
        {
            _sourceRectangles.Add(new(i * frameWidth, (row - 1) * frameHeight, frameWidth, frameHeight));
        }

        //Salva obj especifico para trabalhar com ele, usado para gerenciar inimigos
        _enemy = enemyInstance;
        //Salva o TIPO do objeto, no caso inimigos, para poder comparar condições
        _objType = enemyInstance?.GetType();

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
        if (!_active) return;

        //Tempo da animação é reduzida pelo tempo de jogo
        _frameTimeLeft -= Globals.TotalSeconds;

        //Utiliza o tempo de jogo para avançar de sprite para sprite no spritesheet
        if (_frameTimeLeft <= 0)
        {
            _frameTimeLeft += _frameTime;
            _frame = (_frame + 1) % _frames;
        }


        //GERENCIADOR DE ANIMAÇÕES PARA O PLAYER
        if (_enemy == null)
        {
            if (Hero.ATTACKING && !Hero.RECOIL) //Calculos de frames especificos para janelas de golpes
            {
                if(Hero.RECOIL) Reset(); //Adicionado RECOIL nas condições para não bugar com ataque infinito
                //Gerenciador de animação entre os golpes
                if (_frame == 6 || _frame == 11 || _frame == _frames - 1) Hero.ATTACKING = false;
                //Gerenciador de janela de colisão para os golpes
                if (_frame == 3 || _frame == 7 || _frame == 12) Hero.ATTACKHITTIME = true;
                else Hero.ATTACKHITTIME = false;


            }
            //Conjuração
            if (_frame == _frames - 1 && Hero.CAST) Hero.CAST = false;
            //Dash
            if (_frame == _frames - 1 && Hero.DASH)
            {
                Reset();
                Hero.DASH = false;
            }
        }
        else // GERENCIADOR DE ANIMAÇÕES PARA INIMIGOS
        {
            //Deleta o inimigo do jogo quando animação de morte termina e HP está menor que 0
            if (_frame == _frames - 1 && _enemy.HP <= 0)
            {
                _enemy.DEATHSTATE = true;
            }
            //Reseta a animação e retorna da animação de ataque após completar a animação
            else if (_frame == _frames - 1 && _enemy.ATTACKSTATE)
            {
                Reset();
                _enemy.ATTACKSTATE = false;
                _enemy.PREATTACKHITCD = true;
            }
            //Caso o inimigo seja do tipo enemySkeleton...
            if (_objType == typeof(enemySkeleton) && _enemy.ATTACKSTATE && _enemy.HP > 0)
            {
                if (_frame == 1) //Mesma lógica do heroi, calculos de frames especificos para janelas de golpes
                {
                    _enemy.ATTACKHITTIME = true;
                    _enemy.ATTACKTYPE = 1;
                }
                else if (_frame == 5) //Parte 2 do golpe pois esse inimigo tem duas caixas de colisão
                {
                    _enemy.ATTACKHITTIME = true;
                    _enemy.ATTACKTYPE = 2;
                }
                else _enemy.ATTACKHITTIME = false; //Caso não esteja nesses frames, o golpe não tem hitbox
            }

        }


    }

    public void Draw(Vector2 pos, int scale, bool fliped)
    {
        //Utilizando SpriteBatch localizado em Global.cs ele desenha na tela um sprite com parametros passados pelo Animation Manager.
        if (!fliped)
            Globals.SpriteBatch.Draw(_texture, pos, _sourceRectangles[_frame], Color.White, 0, Vector2.Zero, new Vector2(scale, scale), SpriteEffects.None, 1);
        else
            Globals.SpriteBatch.Draw(_texture, pos, _sourceRectangles[_frame], Color.White, 0, Vector2.Zero, new Vector2(scale, scale), SpriteEffects.FlipHorizontally, 1);
    }
}