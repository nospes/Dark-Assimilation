namespace MyGame;

public interface enemyCollection
{
    int HP { get; set; }
    bool DANORECEBIDO { get; set; }
    bool PREATTACKSTATE{get; set; }
    int ID { get; set; }
    //Permite chamar funções de qualquer enemyBase em outras classes
    void Update();
    void Draw();
    Rectangle GetBounds(string boundType);
}

public abstract class enemyBase : enemyCollection
{
    //ID
    public int ID { get; set; }
    //Variaveis de estado do sprite
    public bool mirror { get; set; }   //Espelhamento 
    public int scale { get; set; } //Tamanho
    public bool walkState { get; set; }
    public bool PREATTACKSTATE{get; set;}

    //Variaveis de posição e movimento
    public Vector2 position { get; set; }  //Posição
    public Vector2 center { get; set; } // Centro do
    public float speed { get; set; }   //Velocidade


    //Variaveis de colisão
    public Vector2 basehitboxSize { get; set; } // Tamanho base do hitbox
    public Vector2 origin { get; set; }    //Centro do frame atual no spritesheet
    public int HP { get; set; }
    public bool DANORECEBIDO { get; set; }

    //Referencia-se as funções presentes dentro dos portadores dessa herança
    public abstract void Update();
    public abstract void Draw();
    public abstract Rectangle GetBounds(string boundType);
}