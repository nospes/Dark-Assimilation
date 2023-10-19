namespace MyGame;
//Essa AI segue o heroi independente do alcance
public class FollowHeroAI : MovementAI
{
    public Hero Target { get; set; }

    public override void Move(enemyBase enemy)
    {
        if (Target is null) return;

        var dir = Target.Position - enemy.Position - enemy.Origin;

        if (dir.X > 0)
            enemy.Mirror = false;
        else if (dir.X < 0)
            enemy.Mirror = true;

        if (dir.Length() > 4)
        {
            dir.Normalize();
            enemy.Position += dir * enemy.Speed * Globals.TotalSeconds;
        }
    }

}