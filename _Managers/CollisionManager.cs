namespace MyGame
{
    public class CollisionManager
    {
        private Hero _hero;
        private List<enemyCollection> _inimigos;

        public CollisionManager(Hero hero, List<enemyCollection> inimigos)
        {
            _hero = hero;
            _inimigos = inimigos;
        }

        public void CheckCollisions()
        {
            Rectangle _herobounds = _hero.GetBounds();
            Rectangle _attackbounds = _hero.AttackBounds();

            foreach (var _inimigo in _inimigos)
            {
                Rectangle _enemybounds = _inimigo.GetBounds("hitbox");
                Rectangle _enemyReactionbounds = _inimigo.GetBounds("reactionbox");

                if (_herobounds.Intersects(_enemyReactionbounds))
                {
                    _inimigo.PREATTACKSTATE = true;
                }

                if (_attackbounds.Intersects(_enemybounds)&&Hero.ATTACKHITTIME&&!_inimigo.DANORECEBIDO)
                {
                    _inimigo.HP -= 10;
                    _inimigo.DANORECEBIDO = true;
                    Console.WriteLine(_inimigo.HP);
                    // Faça qualquer coisa que você deseja quando houver uma colisão.

                }

                if(!Hero.ATTACKING) _inimigo.DANORECEBIDO = false;
            }
        }
    }
}