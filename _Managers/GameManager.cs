namespace MyGame;

public class GameManager
{
    //Em geral esse código serve apenas para adicionar, remover e atualizar coisas dentro do jogo
    //Sua propriedade Draw() é diretamente chamada pelo update do Game1.cs
    private Hero _hero;
    private List<enemySkeleton> inimigos = new List<enemySkeleton>();

    public void Init()
    {
        _hero = new(new(500, 300));
        inimigos.Add(new enemySkeleton(new(100,200)));
        inimigos.Add(new enemySkeleton(new(100,400)));
        inimigos.Add(new enemySkeleton(new(100,0)));
    }

    public void Update()
    {
        InputManager.Update();
        _hero.Update();
        foreach (var inimigo in inimigos)
        {
            inimigo.Update();
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