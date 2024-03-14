namespace MyGame;

public enum EnemyType
{
    Mage,
    Archer,
    Swarm,
    Skeleton
}

public interface enemyCollection
{
    //Interface da coleção de inimigos permite acessar variaveis de qualquer enemyBase em outras classes
    //Atributos de indentificação, combate e posição
    int ID { get; set; }
    int HP { get; set; }
    Vector2 CENTER { get; set; } 
    Vector2 POSITION {get;set;}
    Vector2 HEROATTACKPOS { get; set; }
    bool ENEMYSKILL_LOCK { get; set; }
    bool ALERT { get; set; }
    //Estados
    MovementAI MoveAI { get; set; }
    bool DEATHSTATE { get; set; }
    bool PREATTACKSTATE { get; set; }
    bool ATTACKSTATE { get; set; }
    bool INVULSTATE { get; set; }
    int ATTACKTYPE { get; set; }
    bool DASHSTATE { get; set; }
    bool SPAWN { get; set; }
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
    public Vector2 POSITION {get; set;} //Posição
    public Vector2 CENTER { get; set; } // Centro do inimigo
    public float speed;   //Velocidade
    public Vector2 _minPos, _maxPos; // Limites do mapa
    public MovementAI MoveAI { get; set; } // Tipo de AI do inimigo
    public int enemydataType; // Tipo de inimigo, usado na seleção de perfil com KNN

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
    public bool SPAWN { get; set; } //Se o inimigo está Spawnado


    //Variaveis de colisão e combate
    public int HP { get; set; } // Pontos de vida do inimigo
    public Vector2 HEROATTACKPOS { get; set; } //Posição do jogador ao acertar um golpe
    public Vector2 basehitboxSize; // Tamanho base do hitbox
    public Vector2 origin;   //Centro do frame atual no spritesheet
    public bool ENEMYSKILL_LOCK { get; set; } // Variavel para travas de habilidades inimigas
    public bool Recoling = false; //Recuo para danos
    public bool ALERT { get; set; } // Variavel usada para inimigos serem alertados por outros




    //Referencia-se as funções presentes dentro dos portadores dessa herança
    public abstract void Update(); //Função de update dos inimigos
    public abstract void Draw(); //Função de desenho dos inimigos
    public abstract Rectangle GetBounds(string boundType); //Função para pegar os limites dos inimigos
    public abstract void MapBounds(Point mapSize, Point tileSize); // Limites do mapa
    public abstract Task SetInvulnerableTemporarily(int durationInMilliseconds); // Frame de ivulnerabilidade após dano

    //Variaveis para calculo de perfil do jogador

    public bool _dataPassed = false; // Verifica se a data já foi passada
    public BattleStats battleStats = new BattleStats(); // Cria a classe para guardar os dados de combate em cad a inimigo


    //Usado para atualizar os Dados de Combate de cada inimigo
    protected void UpdateBattleStats()
    {
        battleStats.Update();
    }

    //Passa os dados coletados para serem convertidos em um JSON no PythonBridge.cs
    protected async Task SerializeDataOnDeath()
    {
        if (!_dataPassed)
        {
            // Use actual metrics from battleStats
            var averageCombatTime = (int)battleStats.FinalBattleTime.TotalSeconds; // Tempo total de combate
            var timeAfterFirstHit = (int)battleStats.FinalTimeAfterFirstHit.TotalSeconds; //Tempo de combate depois do primeiro dano
            int totalDashes = (int)battleStats.FinalDashCount; // Total de avanços durante combate
            int enemyType = enemydataType; // Tipo do inimigo derrotado

            await PythonBridge.UpdateCombatDataAsync(enemydataType, averageCombatTime, timeAfterFirstHit, totalDashes); //Passando os dados
            Pentagram.enemyCount += 1; // Variavel para portal
            _dataPassed = true;
        }
    }


}

