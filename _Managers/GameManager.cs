namespace MyGame;

public class GameManager
{
    //Em geral esse código serve apenas para adicionar, remover e atualizar coisas dentro do jogo
    //Sua propriedade Draw() é diretamente chamada pelo update do Game1.cs
    private Hero _hero;

    public void Init()
    {
        _hero = new(new(500, 300));
    }

    public void Update()
    {
        InputManager.Update();
        _hero.Update();
    }

    public void Draw()
    {
        _hero.Draw();
    }
}