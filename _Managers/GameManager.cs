namespace MyGame;

public class GameManager
{
    //Em geral esse código serve apenas para adicionar, remover e atualizar coisas dentro do jogo
    //Suas funções são diretamente chamadas pelo update do Game1.cs

    private Hero _hero;
    private Map _map;
    private List<enemyCollection> inimigos = new List<enemyCollection>();
    private CollisionManager _collisionManager;
    private Matrix _translation;


    public void Init()
    {
        //Cria o mapa
        _map = new();

        //Cria o heroi
        _hero = new(new(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2));

        //Cria o inimigo e define algumas variaveis, como ID e tipo de AI
        inimigos.Add(new enemySkeleton(new(600, 600))
        {
            ID = 1,
            MoveAI = new GuardMovementAI
            {
                target = _hero,
                guard = new(600, 600),
                distance = 300
            }
        });


        inimigos.Add(new enemySkeleton(new(100, 300))
        {
            ID = 2,
            MoveAI = new FollowHeroAI
            {
                target = _hero
            }
        });



        inimigos.Add(new enemySkeleton(new(100, 600))
        {
            ID = 3,
            MoveAI = new DistanceMovementAI
            {
                target = _hero,
                distance = 250
            }
        });
        _collisionManager = new CollisionManager(_hero, inimigos); //Cria gerenciador de colisões entre inimigos e jogador
        _hero.MapBounds(_map.MapSize, _map.TileSize); //Atrela o Heroi aos limites do mapa


    }

    private void CalculateTranslation() // Gerenciador dos limites de camera de acordo com o mapa.
    {
        var dx = (Globals.WindowSize.X / 2) - _hero.CENTER.X;
        dx = MathHelper.Clamp(dx, -_map.MapSize.X + Globals.WindowSize.X + (_map.TileSize.X / 2), _map.TileSize.X / 2);
        var dy = (Globals.WindowSize.Y / 2) - _hero.CENTER.Y;
        dy = MathHelper.Clamp(dy, -_map.MapSize.Y + Globals.WindowSize.Y + (_map.TileSize.Y / 2), _map.TileSize.Y / 2);
        _translation = Matrix.CreateTranslation(dx, dy, 0f);
    }

    public void Update()
    {

        InputManager.Update(); //Atualiza os botões
        _hero.Update(); //Atualiza os herois
        List<enemyCollection> inimigosParaRemover = new List<enemyCollection>(); //Atualiza a lista de inimigos mortos
        foreach (var inimigo in inimigos) //Para cada inimigo...
        {
            inimigo.Update(); //Atualiza
            if (inimigo.DEATHSTATE) 
            {
                inimigosParaRemover.Add(inimigo); //Se esta morto adiciona a lista de inimigos derrotados
            }
        }

        foreach (var inimigoParaRemover in inimigosParaRemover) //Para cada inimigo derrotado...
        {
            inimigos.Remove(inimigoParaRemover); //Remova o inimigo
        }

        _collisionManager.CheckCollisions(); //Checa as colisões
        CalculateTranslation(); //Atualiza a posição da camera

    }

    public void Draw()
    {
        Globals.SpriteBatch.Begin(transformMatrix: _translation); //Cria os sprites dentro dos limites do mapa
        _map.Draw(); //Desenha o mapa
        foreach (var inimigo in inimigos)
        {
            inimigo.Draw(); //Para cada inimigo no mapa, ele é desenhado
        }
        _hero.Draw(); //Desenha o heroi
        Globals.SpriteBatch.End();
    }
}