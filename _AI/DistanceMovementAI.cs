namespace MyGame;

public class DistanceMovementAI : MovementAI
{
    public Hero Target { get; set; }
    public float Distance { get; set; }

    public override void Move(enemyBase enemy)
    {
        if (Target is null) return;

        var dir = Target.Position - enemy.Position - enemy.Origin;
        var length = dir.Length();

        if (length > Distance + 2)
        {
            dir.Normalize();
            enemy.Position += dir * enemy.Speed * Globals.TotalSeconds;
        }
        else if (length < Distance - 2)
        {
            dir.Normalize();
            enemy.Position -= dir * enemy.Speed * Globals.TotalSeconds;
        }

        if (dir.X > 0)
            enemy.Mirror = false;
        else if (dir.X < 0)
            enemy.Mirror = true;
    }
}