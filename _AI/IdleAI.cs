namespace MyGame;
//Essa AI padrão IDLE, quando o heroi entra no alcance o inimigo entra em combate
public class IdleAI : MovementAI
{
    public Hero target { get; set; }
    public float distance { get; set; }
    public MovementAI AIenemyType { get; set; }

    public override void Move(enemyBase enemy)
    {
        if (target is null || enemy.actionstate) return;

        distance = 300; // Distancia para sair do modo Idle
        var totarget = (target.POSITION - enemy.CENTER).Length(); // Pega a posição do heroi

        if (totarget < distance || enemy.ALERT)
        {
            enemy.battleStats.StartBattle(); // Começa o combate
            enemy.MoveAI = AIenemyType; // Pega a AI padrão do inimigo e aplica nele
            GameManager.EnemyEngagement(enemy.CENTER); // Alerta os inimigos próximos dessa unidade
        }

    }
}