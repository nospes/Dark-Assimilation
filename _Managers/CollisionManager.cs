namespace MyGame
{
    public class CollisionManager
    {
        private Hero _hero;
        private List<enemySkeleton> _inimigos;

        public CollisionManager(Hero hero, List<enemySkeleton> inimigos)
        {
            _hero = hero;
            _inimigos = inimigos;
        }

        public void CheckCollisions()
        {
            Rectangle boundsObject1 = _hero.GetBounds();
            Rectangle boundsObject3 = _hero.AttackBounds();

            foreach (var inimigo in _inimigos)
            {
                Rectangle boundsObject2 = inimigo.GetBounds();

                if (boundsObject1.Intersects(boundsObject2))
                {
                    Console.WriteLine("Colisão detectada com inimigo!");
                    // Faça qualquer coisa que você deseja quando houver uma colisão.
                }
            }
        }
    }
}