namespace MyGame
{
    public class CollisionManager
    {
        private Hero _hero;
        private List<enemyCollection> __inimigos;

        public CollisionManager(Hero hero, List<enemyCollection> _inimigos)
        {
            _hero = hero;
            __inimigos = _inimigos;
        }

        public void CheckCollisions()
        {
            Rectangle boundsObject1 = _hero.GetBounds();
            Rectangle boundsObject3 = _hero.AttackBounds();

            foreach (var _inimigo in __inimigos)
            {
                Rectangle boundsObject2 = _inimigo.GetBounds();

                if (boundsObject1.Intersects(boundsObject2))
                {
                    Console.WriteLine("Colisão detectada com inimigo!");
                    // Faça qualquer coisa que você deseja quando houver uma colisão.
                }

                if (boundsObject3.Intersects(boundsObject2)&&InputManager._attacking)
                {
                    Console.WriteLine("Golpe desferido no inimigo!");
                    // Faça qualquer coisa que você deseja quando houver uma colisão.
                }
            }
        }
    }
}