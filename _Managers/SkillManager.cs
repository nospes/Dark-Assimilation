namespace MyGame;

//Serve para gerenciar o tempo de recarga de ações
public class SkillManager
{

    private float cooldownTimer = 0f;
    public bool CheckCooldown = false;

    public void skillCooldown(float cooldownDuration,  Action onCooldownEnd)
    {
        if(CheckCooldown)cooldownTimer += (float)Globals.TotalSeconds;

        if (cooldownTimer >= cooldownDuration)
        {
            // Ação a ser executada quando o cooldown terminar
            onCooldownEnd?.Invoke();
            

            // Reinicie o timer de cooldown
            cooldownTimer = 0f;
            CheckCooldown = false;
        }
    }
}