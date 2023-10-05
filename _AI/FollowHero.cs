namespace MyGame;

public class FollowHero : MovementAI
{
    public Hero Target { get; set; }

    public override void Move(enemyBase enemy)
    {
        if (Target is null) return;

        var dir = Target._position - enemy._position - enemy._origin;

        if(dir.X>0)
        enemy._mirror = false;
        else if(dir.X<0)
        enemy._mirror = true;

        if (dir.Length() > 4)
        {
            dir.Normalize();
            enemy._position += dir * enemy._speed * Globals.TotalSeconds;
        }
    }

}