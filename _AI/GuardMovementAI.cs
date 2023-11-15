namespace MyGame;
//Essa AI segue o heroi até certo alcance
public class GuardMovementAI : MovementAI
{
    public Hero target { get; set; }
    public Vector2 guard { get; set; }
    public float distance { get; set; }

    public override void Move(enemyBase enemy)
    {
        if (target is null || enemy.DANORECEBIDO) return;

        var totarget = (guard - target.POSITION).Length();
        Vector2 dir;



        if (totarget < distance)
        {
            dir = target.CENTER - enemy.center;
        }
        else
        {
            dir = guard - enemy.center;
        }

        if (dir.X > 0)
            enemy.mirror = false;
        else if (dir.X < 0)
            enemy.mirror = true;

        if (dir.Length() > 4 || dir.Length() < -4)
        {
            dir.Normalize();
            enemy.position += dir * enemy.speed * Globals.TotalSeconds;
            enemy.walkState = true;
        }
        else enemy.walkState = false;
    }
}