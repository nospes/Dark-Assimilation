namespace MyGame;
//Essa AI segue o heroi independente do alcance
public class FollowHeroAI : MovementAI
{
    public Hero target { get; set; }

    public override void Move(enemyBase enemy)
    {
        Vector2 dir;
        if (target is null || enemy.DANORECEBIDO || enemy.PREATTACKSTATE) return;

        dir = target.CENTER - enemy.center;

        if (dir.X > 0)
            enemy.mirror = false;
        else if (dir.X < 0)
            enemy.mirror = true;

        if (dir.Length() > 4 || dir.Length() < -4)
        {
            enemy.walkState = true;
            dir.Normalize();
            enemy.position += dir * enemy.speed * Globals.TotalSeconds;
        }
        else enemy.walkState = false;
    }

}