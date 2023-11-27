namespace MyGame
{
    public class CollisionManager
    {
        private Hero _hero;
        public List<enemyCollection> _inimigos;

        public CollisionManager(Hero hero, List<enemyCollection> inimigos)//Criando variaveis locais para unidades
        {
            _hero = hero;
            _inimigos = inimigos;
        }

        public void CheckCollisions()
        {
            Rectangle _herobounds = _hero.GetBounds(); //Hitbox do heroi
            Rectangle _heroAttackbounds = _hero.AttackBounds(); //Hitbox de ataque heroi

            foreach (var _inimigo in _inimigos)
            {
                Rectangle _enemybounds = _inimigo.GetBounds("hitbox"); // hitbox dos inimigos
                Rectangle _enemyReactionbounds = _inimigo.GetBounds("reactionbox"); //hitbox de reação dos inimigos
                Rectangle _enemyAttackbounds = _inimigo.GetBounds($"attackbox{_inimigo.ATTACKTYPE}"); //Ataque do inimigo, nota-se se ele ele tiver mais de um ataque esse parametro pode ser passado

                //Se o heroi entrar na area de reação do inimigo
                if (_herobounds.Intersects(_enemyReactionbounds))
                {
                    if (!_inimigo.PREATTACKHITCD)
                    {
                        if (!_inimigo.PREATTACKSTATE && !_inimigo.ATTACKSTATE)
                        {
                            _inimigo.PREATTACKSTATE = true;
                        }
                    }
                }

                //Caso um frame de golpe do player acerte o inimigo;
                if (_heroAttackbounds.Intersects(_enemybounds) && Hero.ATTACKHITTIME && !_inimigo.INVULSTATE && !Hero.RECOIL)
                {
                    _inimigo.HP -= 10;
                    _inimigo.HEROATTACKPOS = _hero.CENTER;
                    _inimigo.INVULSTATE = true;
                    Console.WriteLine(_inimigo.HP);

                }

                //Caso um frame de golpe do inimigo acerte o player;
                if (_enemyAttackbounds.Intersects(_herobounds) && _inimigo.ATTACKHITTIME && !Hero.RECOIL && !Hero.DASH)
                {
                    _hero.HP -= 10;
                    Hero.lastHitpos = _inimigo.CENTER;
                    Hero.RECOIL = true;
                    //Console.WriteLine(_hero.HP);
                }

                //Permite que o inimigo volte a levar golpes quando terminar a animação de um
                if (!Hero.ATTACKING) _inimigo.INVULSTATE = false;
            }
        }

        public void Knockbackmanager(enemyCollection _enemy)
        {
            var dir = _hero.CENTER - _enemy.CENTER;
            Vector2.Normalize(dir);
            /*while()
            {
                
            }*/
            }
        }
}