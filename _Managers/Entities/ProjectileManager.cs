namespace MyGame;

public static class ProjectileManager
{
    private static AnimationManager _anims = new AnimationManager();
    public static List<Projectile> Projectiles { get; } = new(); //Cria lista de projeteis para serem gerenciados e colocados no campo com Gamemanager

    public static void AddProjectile(ProjectileData data, float delay = 0)
    {
        
        lock (Projectiles)
        {
            Projectiles.Add(new(data)); //Adicona o projétil a lista com os atributos passados pelo Data
        }
    }

    public static void Update()
    {
        lock (Projectiles)
        {
            foreach (var p in Projectiles)  //Todos os projeteis...
            {
                p.Update();//Atualiza os projeteis

            }
            Projectiles.RemoveAll((p) => p.Lifespan <= 0); //Remove todos projeteis com Lifespan menor que zero
        }
    }


    public static void Draw()
    {
        lock (Projectiles)
        {
            foreach (var p in Projectiles) //Desenha os projeteis
            {
                p.Draw();
            }
        }
    }

    public static void DeleteAll()
    {
        lock (Projectiles)
        {
            foreach (var p in Projectiles)
            {
                p.Lifespan = 0;
            }
        }
    }
}