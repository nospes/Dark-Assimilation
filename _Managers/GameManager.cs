namespace MyGame;

public class GameManager
{
    //Em geral esse código serve apenas para adicionar, remover e atualizar coisas dentro do jogo
    //Sua propriedade Draw() e Update() é diretamente chamada pelo update do Game1.cs
    private Hero _hero;
    private List<enemySkeleton> inimigos = new List<enemySkeleton>();


    public void Init()
    {
        _hero = new(new(500, 300));
        inimigos.Add(new enemySkeleton(new(100, 400)));
        inimigos.Add(new enemySkeleton(new(100, 0)));

    }



    public void Update()
    {

        InputManager.Update();
        _hero.Update();
        foreach (var inimigo in inimigos)
        {
            inimigo.Update();
        }

        Rectangle boundsObject1 = _hero.GetBounds();
        Rectangle boundsObject3 = _hero.AttackBounds();

        foreach (var inimigo in inimigos)
        {
            Rectangle boundsObject2 = inimigo.GetBounds();

            if (boundsObject3.Intersects(boundsObject2)&&InputManager._attacking)
            {
                Console.WriteLine("Colisão detectada com inimigo!");
                // Faça qualquer coisa que você deseja quando houver uma colisão.
            }

        }
    }

    public void Draw()
    {
        _hero.Draw();
        foreach (var inimigo in inimigos)
        {
            inimigo.Draw();
        }

    }
}