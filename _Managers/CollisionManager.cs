namespace MyGame
{
    public class CollisionManager
    {
        private Hero _hero;
        private Pentagram _pentagram;
        private Soul _soul;
        public List<enemyCollection> _inimigos;
        public List<Projectile> _projeteis;

        private Type _objType;
        private RandomGenerator critHit;

        public CollisionManager(Hero hero, List<enemyCollection> inimigos, Pentagram pentagram, Soul soul)//Criando variaveis locais para unidades
        {
            _hero = hero;
            _inimigos = inimigos;
            _pentagram = pentagram;
            _soul = soul;

        }

        public void CheckCollisions()
        {
            Rectangle _heroBounds = _hero.GetBounds(); //Hitbox do heroi
            Rectangle _heroAttackbounds = _hero.AttackBounds(); //Hitbox de ataque heroi
            Rectangle _pentagramBounds = _pentagram.GetBounds(); // Hitbox do teleportador
            Rectangle _soulBounds = _soul.GetBounds(); // Hitbox do seletor de upgrades
            _projeteis = ProjectileManager.Projectiles;


            foreach (var _inimigo in _inimigos)
            {
                Rectangle _enemybounds = _inimigo.GetBounds("hitbox"); // hitbox dos inimigos
                Rectangle _enemyReactionbounds = _inimigo.GetBounds("reactionbox"); //hitbox de reação dos inimigos
                Rectangle _enemyAttackbounds = _inimigo.GetBounds($"attackbox{_inimigo.ATTACKTYPE}"); //Ataque do inimigo, nota-se se ele ele tiver mais de um ataque esse parametro pode ser passado
                _objType = _inimigo.GetType();

                //Se o heroi entrar na area de reação do inimigo
                if (_heroBounds.Intersects(_enemyReactionbounds))
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
                if (_heroAttackbounds.Intersects(_enemybounds) && Hero.ATTACKHITTIME && !_inimigo.INVULSTATE && !Hero.KNOCKBACK)
                {
                    critHit = new RandomGenerator(RandomGenerator.GenerateSeedFromCurrentTime());
                    if (critHit.NextInt(0, 100) < _hero.critChance)
                    {
                        var totaldmg = Math.Round(_hero.heroAAdmg * _hero.critMult);
                        _inimigo.HP -= (int)totaldmg;
                    }
                    else _inimigo.HP -= _hero.heroAAdmg;
                    _inimigo.HEROATTACKPOS = _hero.CENTER;
                    _inimigo.SetInvulnerableTemporarily(300);

                    //Console.WriteLine(_inimigo.HP);

                }

                //Caso um frame de golpe do inimigo acerte o player;
                if (_enemyAttackbounds.Intersects(_heroBounds) && _inimigo.ATTACKHITTIME && _inimigo.HP > 0 && !Hero.RECOIL && !Hero.DASH)
                {
                    _hero.HP -= 10;
                    Hero.lastHitpos = _inimigo.CENTER;
                    Hero.RECOIL = true;
                    //Console.WriteLine(_hero.HP);

                    // Caso inimigo seja do tipo enemySwarm...
                    if (_objType == typeof(enemySwarm)) _inimigo.ATTACKHITTIME = false;
                } //else if(_inimigo.ENGAGED) 


                //Projeteis aliados
                foreach (var _projetil in _projeteis)
                {
                    if (_projetil.Friendly)
                    {
                        Rectangle _projectileBounds = _projetil.GetBounds(); // hitbox dos inimigos
                        if (_projectileBounds.Intersects(_enemybounds) && !_inimigo.DEATHSTATE) // Caso entre em contato com HITBOX inimiga...
                        {
                            if (!_projetil.enemyHited)
                            {
                                _inimigo.HP -= _hero.heroSpelldmg;
                                _projetil.enemyHited = true;
                                _inimigo.HEROATTACKPOS = _hero.CENTER;
                                _inimigo.SetInvulnerableTemporarily(300);
                                _projetil.Lifespan = 0.5f;
                                _inimigo.ALERT = true;
                            }

                        }
                    }
                }

            }
            foreach (var _projetil in _projeteis) //Gerenciador para casos de colisões com Projeteis
            {
                Rectangle _projectileBounds = _projetil.GetBounds(); // Caixa de colisão do projétil
                if (_projectileBounds.Intersects(_heroBounds) && !_projetil.Friendly && !Hero.RECOIL && !Hero.DASH) //Caso entre em contato com heroi...
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
                else if (Hero.RECOIL || Hero.DASH)
                {
                    if (_projectileBounds.Intersects(_heroBounds) && !_projetil.Friendly)
                    {
                        if (_projetil.ProjectileType == "DarkProj")
                        {
                            _projetil.Homing = false; // Ao colidir com jogador para de segui-lo
                        }
                    }
                }
            }

            //Gerenciador de Áreas
            if (_heroBounds.Intersects(_pentagramBounds))
            {
                if (_pentagram.teleportON)
                {
                    _pentagram.teleport = true;
                    Pentagram.enemyCount = 0;
                    _pentagram.gamearea += 1;

                }
            }
            if (_heroBounds.Intersects(_soulBounds) && Hero.ATTACKHITTIME && _soul.alive)
            {
                _soul.alive = false;
            }
        }

    }
}