namespace MyGame;

public interface enemyCollection
{
    //Interface da coleção de inimigos permite acessar variaveis de qualquer enemyBase em outras classes
    //Atributos de indentificação, combate e posição
    int ID { get; set; }
    int HP { get; set; }
    Vector2 CENTER { get; set; }
    Vector2 HEROATTACKPOS { get; set; }
    bool ENEMYSKILL_LOCK { get; set; }
    //Estados
    bool DEATHSTATE { get; set; }
    bool PREATTACKSTATE { get; set; }
    bool ATTACKSTATE { get; set; }
    bool INVULSTATE { get; set; }
    int ATTACKTYPE { get; set; }
    bool DASHSTATE { get; set; }
    //Temporizadores
    bool ATTACKHITTIME { get; set; }
    bool PREATTACKHITCD { get; set; }

    //Permite chamar funções de qualquer enemyBase em outras classes
    void Update();
    void Draw();
    void MapBounds(Point mapSize, Point tileSize);
    Task SetInvulnerableTemporarily(int durationInMilliseconds);
    Rectangle GetBounds(string boundType);
}

public abstract class enemyBase : enemyCollection
{
    //Variaveis de posição e movimento e indentificação
    public int ID { get; set; }
    public Vector2 position; //Posição
    public Vector2 CENTER { get; set; } // Centro do inimigo
    public float speed;   //Velocidade
    public Vector2 _minPos, _maxPos;
    public MovementAI MoveAI { get; set; }

    //Variaveis de estado do sprite
    public bool mirror;  //Espelhamento 
    public int scale;//Tamanho
    public bool walkState; //Estado de 'andar', se for falso não entra no sprite de caminhada
    public bool actionstate; //Estado de ação, se está fazendo ações ele não anda


    //Variaveis de estado, todas estão atreladas a interface, usados para acessar mais fácil certas condições dos inimigos
    public bool DEATHSTATE { get; set; }//Estado de morte - se morto deleta
    public bool PREATTACKSTATE { get; set; }//Estado de pré ataque - apos um intervalo ativa o ataque
    public bool ATTACKSTATE { get; set; }//Estado de ataque - ataca após pre-ataque
    public int ATTACKTYPE { get; set; } //Tipo de ataque - define a hitbox do tipo de ataque;
    public bool INVULSTATE { get; set; }//Estado de recuo - tempo de recuo após levar um dano
    public bool ATTACKHITTIME { get; set; } //Estado de tempo de dano - Permite heroi levar dano enquanto verdadeiro
    public bool PREATTACKHITCD { get; set; }//Trava para tempo de recarga entre ataques
    public bool DASHSTATE { get; set; }//Estado de Avanço - Presente em alguns inimigos


    //Variaveis de colisão e combate
    public int HP { get; set; } // Pontos de vida do inimigo
    public Vector2 HEROATTACKPOS { get; set; } //Posição do jogador ao acertar um golpe
    public Vector2 basehitboxSize; // Tamanho base do hitbox
    public Vector2 origin;   //Centro do frame atual no spritesheet
    public bool ENEMYSKILL_LOCK { get; set; } // Variavel para travas de habilidades inimigas
    public bool Recoling = false; //Recuo para danos


    //Referencia-se as funções presentes dentro dos portadores dessa herança
    public abstract void Update(); //Função de update dos inimigos
    public abstract void Draw(); //Função de desenho dos inimigos
    public abstract Rectangle GetBounds(string boundType); //Função para pegar os limites dos inimigos
    public abstract void MapBounds(Point mapSize, Point tileSize);
    public abstract Task SetInvulnerableTemporarily(int durationInMilliseconds);


}