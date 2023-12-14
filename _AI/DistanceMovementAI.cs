namespace MyGame;

public class DistanceMovementAI : MovementAI
{
    public Hero target { get; set; }
    public float distance = 280;

    public override void Move(enemyBase enemy)
    {
        Vector2 dir;
        if (target is null || enemy.actionstate) return;

        dir = target.CENTER - enemy.CENTER;



        var length = dir.Length();
        if (length > distance + 2)
        {
            dir.Normalize();
            enemy.walkState = true;
            enemy.position += dir * enemy.speed * Globals.TotalSeconds;
        }

        else if (length < distance - 2)
        {
            dir.Normalize();
            enemy.walkState = true;
            enemy.position -= dir * enemy.speed * Globals.TotalSeconds;
        }
        else enemy.walkState = false;


    }
}