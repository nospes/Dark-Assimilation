namespace MyGame;
//Essa AI segue o heroi até certo alcance
public class IdleAI : MovementAI
{
    public Hero target { get; set; }
    public float distance { get; set; }
    public MovementAI AIenemyType {get ;set;}

    public override void Move(enemyBase enemy)
    {
        if (target is null || enemy.actionstate) return;

        distance = 250;
        var totarget = (target.POSITION - enemy.CENTER).Length();

            if (totarget < distance)
            {
                enemy.MoveAI = AIenemyType;
            }

    }
}