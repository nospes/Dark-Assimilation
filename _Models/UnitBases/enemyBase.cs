namespace MyGame;

public interface enemyCollection
{
    //Permite chamar funções de qualquer enemyBase em outras classes
    void Update();
    void Draw();
    Rectangle GetBounds();
}

public abstract class enemyBase : enemyCollection
{
    //Variaveis de estado do sprite
    public bool Mirror { get; set; }   //Espelhamento 
    public int Scale { get; set; } //Tamanho
    //Variaveis de posição e movimento
    public Vector2 Position { get; set; }  //Posição
    public float Speed { get; set; }   //Velocidade

    //Variaveis de colisão
    public Vector2 baseHitboxSize {get; set;} // Tamanho base do hitbox
    public Vector2 Origin { get; set; }    //Centro do frame atual no spritesheet


    //Referencia-se as funções presentes dentro dos portadores dessa herança
    public abstract void Update();
    public abstract void Draw();
    public abstract Rectangle GetBounds();
}