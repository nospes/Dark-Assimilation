namespace MyGame;
//Essa AI segue o heroi até certo alcance
public class GuardMovementAI : MovementAI
{
    public Hero Target { get; set; }
    public Vector2 Guard { get; set; }
    public float Distance { get; set; }

    public override void Move(enemyBase enemy)
    {
        if (Target is null) return;

        var toTarget = (Guard - Target.Position).Length();
        Vector2 dir;



        if (toTarget < Distance)
        {
            dir = Target.Position - enemy.Position - enemy.Origin;
        }
        else
        {
            dir = Guard - enemy.Position - enemy.Origin;
        }

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