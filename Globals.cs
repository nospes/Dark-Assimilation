namespace MyGame;

public static class Globals
{

    //Define tempo de jogo, as sprites e os conteudos adicionados dentro do jogo, utilizados globalmente.
    public static float TotalSeconds { get; set; }
    public static ContentManager Content { get; set; }
    public static SpriteBatch SpriteBatch { get; set; }

    public static void Update(GameTime gt)
    {
        TotalSeconds = (float)gt.ElapsedGameTime.TotalSeconds;
    }
}