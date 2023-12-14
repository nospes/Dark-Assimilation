namespace MyGame;
//Essa AI segue o heroi até certo alcance
public class GuardMovementAI : MovementAI
{
    public Hero target { get; set; }
    public Vector2 guardpos { get; set; }
    public float distance { get; set; }

    public override void Move(enemyBase enemy)
    {
        if (target is null || enemy.actionstate) return;

        var totarget = (guardpos - target.POSITION).Length();
        Vector2 dir;



        if (totarget < distance)
        {
            dir = target.CENTER - enemy.CENTER;
        }
        else
        {
            dir = guardpos - enemy.CENTER;
        }

        if (dir.Length() > 4 || dir.Length() < -4)
        {
            dir.Normalize();
            enemy.position += dir * enemy.speed * Globals.TotalSeconds;
            enemy.walkState = true;
        }
        else enemy.walkState = false;
    }
}