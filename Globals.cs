namespace MyGame;

public static class Globals
{

    //Define tempo de jogo, as sprites e os conteudos adicionados dentro do jogo, utilizados globalmente.
    public static float TotalSeconds { get; set; } //Tempo do jogo
    public static ContentManager Content { get; set; } //Define os conteudos presentes no jogo, no caso imagens
    public static SpriteBatch SpriteBatch { get; set; } // Gerenciador de sprites na tela
    public static Point WindowSize { get; set; } //Define tamanho da tela
    public static Vector2 HEROLASTPOS { get; set; } // Posição atual do jogador, atualizada constantemente no gamemanager
    public static bool Exitgame = false;


    public static void Update(GameTime gt)
    {
        TotalSeconds = (float)gt.ElapsedGameTime.TotalSeconds;

    }

}