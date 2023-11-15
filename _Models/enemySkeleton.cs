namespace MyGame;

public class enemySkeleton : enemyBase
{
    // Variaveis para Animações
    private readonly AnimationManager _anims = new();   //Cria uma nova classe de animação
    private static Texture2D _textureIdle, _textureHit, _textureWalk, _textureDeath, _textureAttack, _texturePreattack;  //Spritesheets
    //Definição de comportamento
    public MovementAI MoveAI { get; set; }
    public static SkillManager preattackCD;
    private bool _preattackCDlock = true;
    int[] _qntFrames = new int[] { 4, 3, 12, 13, 4, 9 };


    public enemySkeleton(Vector2 pos)
    {
        //Atribuindo spritesheets a variaveis de Texture2D
        _textureIdle ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Idle");
        _textureHit ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Hit");
        _textureWalk ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Walk");
        _textureDeath ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Death");
        _texturePreattack ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Preattack");
        _textureAttack ??= Globals.Content.Load<Texture2D>("Creatures/BigSkeleton/bigskel_Attack");

        //Vetor Contendo a quantidade de frames em cada animação

        //Definindo area,frames dos sprites sheets para fazer a animação
        _anims.AddAnimation("bigskel_Idle", new(_textureIdle, _qntFrames[0], 1, 0.1f, 1, false));
        _anims.AddAnimation("bigskel_Hit", new(_textureHit, _qntFrames[1], 1, 0.1f, 1, false));
        _anims.AddAnimation("bigskel_Walk", new(_textureWalk, _qntFrames[2], 1, 0.1f, 1, false));
        _anims.AddAnimation("bigskel_Death", new(_textureDeath, _qntFrames[3], 1, 0.1f, 1, false));
        _anims.AddAnimation("bigskel_Preattack", new(_texturePreattack, _qntFrames[4], 1, 0.1f, 1, false));
        _anims.AddAnimation("bigskel_Attack", new(_textureAttack, _qntFrames[5], 1, 0.1f, 1, false));

        //Define a posição, velocidade e tamanho do sprite respectivamente
        position = pos;
        speed = 100f;
        scale = 4;

        //Definição inicial de origem e tamanho da caixa de colisão
        basehitboxSize = new(14, 30); // Tamanho
        var frameWidth = _textureIdle.Width / _qntFrames[0]; // Dividindo os frames de acordo com tamanho do spritesheet
        var frameHeight = _textureIdle.Height / 1;
        origin = new(frameWidth / 2, frameHeight / 2); //Atribui o centro do frame X e Y a um vetor

        //Atributos de combate
        HP = 100;
        DANORECEBIDO = false;
        preattackCD = new SkillManager();

    }

    public override Rectangle GetBounds(string boundType)
    {
        //Limites do _topo e da esquerda da caixa de colisão
        int _left = (int)center.X - (int)(basehitboxSize.X * scale) / 2;
        int _top = (int)center.Y - (int)(basehitboxSize.Y * scale) / 2;
        int _reactionOffset = 25;
        int _reactionSize = 40 * scale;
        Vector2 _attackOffset1 = new Vector2(1, 1);
        Vector2 _attackOffset2 = new Vector2(1, 1);
        Vector2 _attackSize1 = new Vector2(1, 1);
        Vector2 _attackSize2 = new Vector2(1, 1);
        switch (boundType)
        {
            case "hitbox":
                //Caixa de colisão do monstro
                //Com base nas coordenadas _Top e _Left cria um retangulo de tamanho pré-definido multiplicado pelo scale
                return new Rectangle(_left, _top, (int)(basehitboxSize.X * scale), (int)(basehitboxSize.Y * scale));
            case "reactionbox":
                //Caixa de colisão para Reação do monstro
                if (!mirror) return new Rectangle(_left - _reactionOffset, _top, (_reactionSize), (int)(basehitboxSize.Y * scale));
                else return new Rectangle(_left - _reactionSize / 2, _top, (_reactionSize), (int)(basehitboxSize.Y * scale));
            case "attackbox1":
                //Caixa de colisão para o 1º Golpe do monstro
                return new Rectangle(_left, _top, (int)(basehitboxSize.X * scale), (int)(basehitboxSize.Y * scale));
            case "attackbox2":
                //Caixa de colisão para o 2º Golpe do monstro
                return new Rectangle(_left, _top, (int)(basehitboxSize.X * scale), (int)(basehitboxSize.Y * scale));
            default:
                throw new InvalidOperationException($"Tipo de caixa de colisão desconhecido: {boundType}"); ;
        }

    }






    public override void Update()
    {
        //Define as animações de acordo com os estados
        if (PREATTACKSTATE)
        {
            _anims.Update("bigskel_Preattack");
        }
        else if (DANORECEBIDO)
        {
            _anims.Update("bigskel_Hit");
        }
        else if (walkState)
        {
            _anims.Update("bigskel_Walk");
        }
        else
        {
            _anims.Update("bigskel_Idle");
        };

        // Definindo o centro do frame de acordo com a posição atual
        center = position + origin * scale;

        //Trava para evitar bugs relacionado a ordem de carregamento do jogo
        if (MoveAI != null)
        {
            //Aplica um comportamento ao inimigo definido pelo gameManager
            MoveAI.Move(this);
        }


        //algumas vezes os inimigos só não saem do PREATTACKSTATE
        preattackCD.skillCooldown(3.00f, () =>
            {
                this.PREATTACKSTATE = false;
            });
        if (PREATTACKSTATE)
        {
            if (!_preattackCDlock)
            {
                preattackCD.CheckCooldown = true;
                _preattackCDlock = true;
            }
        }
        else _preattackCDlock = false;

    }

    public override void Draw()
    {
        //hitbox test
        Rectangle Erect = GetBounds("reactionbox");
        Globals.SpriteBatch.Draw(Game1.pixel, Erect, Color.Red);

        //Passa os parametros para o AnimationManager animar o Spritesheet
        _anims.Draw(position, scale, mirror);


    }
}