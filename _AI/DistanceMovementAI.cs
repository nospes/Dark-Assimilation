namespace MyGame;
//Essa AI segue o heroi mantendo distancia
public class DistanceMovementAI : MovementAI
{
    public Hero target { get; set; } // Seleciona um objeto do tipo HERO como alvo
    public float distance = 270; // Alcance minimo que ele tenta manter do jogador

    public override void Move(enemyBase enemy)
    {
        Vector2 dir;
        if (target is null || enemy.actionstate) return; // Se está agindo ou alvo é nulo ele não anda

        dir = target.CENTER - enemy.CENTER; // Define a direção usando o heroi como referencia


        //Anda na direção do heroi com essas condições
        var length = dir.Length();
        if (length > distance + 2)
        {
            dir.Normalize();
            enemy.walkState = true;
            enemy.POSITION += dir * enemy.speed * Globals.TotalSeconds;
        }

        else if (length < distance - 2)
        {
            dir.Normalize();
            enemy.walkState = true;
            enemy.POSITION -= dir * enemy.speed * Globals.TotalSeconds;
        }
        else enemy.walkState = false; // Se não está andando, desativa a animação de andar.


    }
}