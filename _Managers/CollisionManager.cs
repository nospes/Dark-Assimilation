namespace MyGame
{
    public class CollisionManager
    {
        private Hero _hero;
        public List<enemyCollection> _inimigos;
        public List<Projectile> _projeteis;

        private Type _objType;

        public CollisionManager(Hero hero, List<enemyCollection> inimigos)//Criando variaveis locais para unidades
        {
            _hero = hero;
            _inimigos = inimigos;

        }

        public void CheckCollisions()
        {
            Rectangle _herobounds = _hero.GetBounds(); //Hitbox do heroi
            Rectangle _heroAttackbounds = _hero.AttackBounds(); //Hitbox de ataque heroi
            _projeteis = ProjectileManager.Projectiles;

            foreach (var _inimigo in _inimigos)
            {
                Rectangle _enemybounds = _inimigo.GetBounds("hitbox"); // hitbox dos inimigos
                Rectangle _enemyReactionbounds = _inimigo.GetBounds("reactionbox"); //hitbox de reação dos inimigos
                Rectangle _enemyAttackbounds = _inimigo.GetBounds($"attackbox{_inimigo.ATTACKTYPE}"); //Ataque do inimigo, nota-se se ele ele tiver mais de um ataque esse parametro pode ser passado
                _objType = _inimigo.GetType();

                //Se o heroi entrar na area de reação do inimigo
                if (_herobounds.Intersects(_enemyReactionbounds))
                {
                    if (!_inimigo.PREATTACKHITCD)
                    {
                        if (!_inimigo.PREATTACKSTATE && !_inimigo.ATTACKSTATE && !_inimigo.DASHSTATE)
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
                if (_enemyAttackbounds.Intersects(_herobounds) && _inimigo.ATTACKHITTIME && _inimigo.HP > 0 && !Hero.RECOIL && !Hero.DASH)
                {
                    _hero.HP -= 10;
                    Hero.lastHitpos = _inimigo.CENTER;
                    Hero.RECOIL = true;
                    //Console.WriteLine(_hero.HP);

                    // Caso inimigo seja do tipo enemySwarm...
                    if (_objType == typeof(enemySwarm)) _inimigo.ATTACKHITTIME = false;
                }

                //Permite que o inimigo volte a levar golpes quando terminar a animação de um
                if (!Hero.ATTACKING) _inimigo.INVULSTATE = false;
            }
            foreach (var _projetil in _projeteis) //Gerenciador para casos de colisões com Projeteis
            {
                Rectangle _projectileBounds = _projetil.GetBounds(); // Caixa de colisão do projétil
                if (_projectileBounds.Intersects(_herobounds) && !Hero.RECOIL && !Hero.DASH) //Caso entre em contato com heroi...
                {
                    if (_projetil.ProjectileType == "Arrow") // Caso seja do tipo 'Arrow'
                    {
                        _hero.HP -= 10;
                        Hero.lastHitpos = _projetil.Position;
                        Hero.RECOIL = true;
                        _projetil.Lifespan = 0; //Destroe o projétil
                    }
                    else if (_projetil.ProjectileType == "DarkProj" && _projetil.Lifespan > 1) // Caso seja do tipo 'Arrow'
                    {
                        _hero.HP -= 10;
                        Hero.lastHitpos = _projetil.Position;
                        Hero.RECOIL = true;
                        _projetil.Lifespan = 1; //Destroe o projetil, nesse caso ele para de causar dano no ultimo segundo de vida
                    }
                    else if (_projetil.ProjectileType == "DarkSpell") // Caso seja do tipo 'DarkSpell'
                    {
                        Hero.SLOWED = true; // Causa lentidão
                        if (_hero.SPEED <= 1) //Caso jogador ainda esteja na area com speed 1 ou menor...
                        {
                            _hero.HP -= 10; //Causa dano
                            Hero.SLOWED = false; // Termina a lentidão
                            Hero.lastHitpos = _projetil.Position;
                            Hero.RECOIL = true;
                        }

                    }
                }
            }
        }

    }
}