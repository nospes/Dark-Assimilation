namespace MyGame;
//Essa AI segue o heroi independente do alcance, segue uma logica similar do DistanceMovementAI.cs porem sem distancia minima
public class FollowHeroAI : MovementAI
{

    public Hero target { get; set; }

    public override void Move(enemyBase enemy)
    {

            Vector2 dir;
            if (target is null || enemy.actionstate) return;

            dir = target.CENTER - enemy.CENTER;

            if (dir.Length() > 4 || dir.Length() < -4)
            {
                enemy.walkState = true;
                dir.Normalize();
                enemy.position += dir * enemy.speed * Globals.TotalSeconds;
            }
            else enemy.walkState = false;
        }

    }