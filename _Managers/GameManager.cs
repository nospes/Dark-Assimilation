namespace MyGame;

public class GameManager
{
    //Em geral esse código serve apenas para adicionar, remover e atualizar coisas dentro do jogo
    //Sua propriedade Draw() e Update() é diretamente chamada pelo update do Game1.cs

    private Hero _hero;
    private Map _map;
    private List<enemyCollection> inimigos = new List<enemyCollection>();
    private CollisionManager _collisionManager;
    private Matrix _translation;


    public void Init()
    {
        _map = new();
        _hero = new(new(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2));

        inimigos.Add(new enemySkeleton(new(600, 600))
        {
            MoveAI = new GuardMovementAI
            {
                Target = _hero,
                Guard = new(600,600),
                Distance = 300
            }
        });


        inimigos.Add(new enemySkeleton(new(100, 300))
        {
            MoveAI = new FollowHeroAI
            {
                Target = _hero
            }
        });



        inimigos.Add(new enemySkeleton(new(100, 600))
        {
            MoveAI = new DistanceMovementAI
            {
                Target = _hero,
                Distance = 250
            }
        });
        _collisionManager = new CollisionManager(_hero, inimigos);
        _hero.MapBounds(_map.MapSize, _map.TileSize);


    }

    private void CalculateTranslation()
    {
        var dx = (Globals.WindowSize.X / 2) - _hero.Center.X;
        dx = MathHelper.Clamp(dx, -_map.MapSize.X + Globals.WindowSize.X + (_map.TileSize.X / 2), _map.TileSize.X / 2);
        var dy = (Globals.WindowSize.Y / 2) - _hero.Center.Y;
        dy = MathHelper.Clamp(dy, -_map.MapSize.Y + Globals.WindowSize.Y + (_map.TileSize.Y / 2), _map.TileSize.Y / 2);
        _translation = Matrix.CreateTranslation(dx, dy, 0f);
    }



    public void Update()
    {

        InputManager.Update();
        _hero.Update();
        foreach (var inimigo in inimigos)
        {
            inimigo.Update();
        }

        _collisionManager.CheckCollisions();
        CalculateTranslation();

    }

    public void Draw()
    {
        Globals.SpriteBatch.Begin(transformMatrix: _translation);
        _map.Draw();
        foreach (var inimigo in inimigos)
        {
            inimigo.Draw();
        }
        _hero.Draw();
        Globals.SpriteBatch.End();
    }
}