# Monogame

Monogame + AI

Projeto para desenvolvimento da minha iniciação cientifica: APLICABILIDADE DE INTELIGÊNCIA ARTIFICIAL NA CUSTOMIZAÇÃO DE CENÁRIOS E AGENTES EM JOGOS DIGITAIS

![cronograma](https://github.com/nospes/Dark-Assimilation/assets/57772398/4501cb85-e7fb-4a43-96bf-9da11e35de4a)

Versão 0.1- foi implementado os inputs de movimento(WASD), controladores básicos do jogo e de animações. O jogador atualmente anda e fica parado

//////////////////////////////////////////////////////////////////////////////////////

Versão 0.2- foi implementado o input de ataque básico(K), animação de soco, sistema para alternar entre golpes e movimento, primeiro inimigo, esqueletos(ainda sem animação) e uma lista para poder adicionar diversos inimigos do mesmo tipo.

Versão 0.2.1- Atualizando alguns parametros para poder utilizar em outras máquinas, agora é possivel mexer no código e no repositorio remotamente de outros pontos.

Versão 0.2.2- esqueletos agora tem animação, foi implementado o lógica básica para detectar colisões(ainda precisa ser aprimorado).

//////////////////////////////////////////////////////////////////////////////////////

Versão 0.3 - Sistema de detecção e as Areas de colisões foram aprimorados, foi implementado um sistema para ver área de colisões das unidades.

Versão 0.3.1 - Implementação da colisão dos golpes, colisão entre golpes e inimigos.

Versão 0.3.2 - Movimentação dos inimigos, referencia de posição do heroi para movimento(precisa ser aprimorado)

Versão 0.3.3 - Adicionado Herança para comportamentos, inimigos e jogador. Agora é possivel ter diversos comportamentos e chama-los para objetos com herança 'enemyBase', a herança base para inimigos comuns. Melhorado o sistema de colisões, agora eles calculam com base no centro do sprite reduzindo a quantidade de valores absolutos para formaçao da caixa. Agora é possível desenhar os retangulos de teste de área de colisão diretamente no Draw() das unidades.

Versão 0.3.4 - Área e Lógica da coslião dos golpes atualizadas.

Versão 0.3.5 - Heranças de inimigos atualizadas para suportar funções das unidades, agora é possivel colocar todos os inimigos de diferentes tipos mas de heranças compatilhadas na mesma lista e chamar suas funções nos gerenciadores de jogo. Lógica das variaveis de colisão dos inimigos atualizadas. Foi retirada uma condição obsoleta do gerenciador de animações. Diversos códigos tiveram seus comentários atualizados para explicação mais clara do seu funcionamento.

////////////////////////////////////////////////////////////////////////////////////

Versão 0.4 -

Foi adicionado a animação de 'conjuração', mecanica de 'dash' e tempo de recarga para todas as essas ações, foi corrigida a caixa de colisão para os estados do heroi, incluindo os golpes e movimentações.
Foi implementado tilesets, camera e limitações do mapa, agora o jogador tem um espaço limitado para se movimentar e a camera o segue enquanto ele explora o ambiente finito. Algumas partes do código foram simplificadas e compactadas para deixar mais claro ao entendimento e leitura dele.

Tambem foram adicionados os comportamentos básicos para inimigos e sua herança relacionada, agora inimigos podem ter entre 3 comportamentos pré-definidos, entre eles; 'GuardMovement', que protege uma área especifica e sempre retorna a ela ao sair do alcance, 'FollowHero', que segue inimigo independente do alcance, e 'DistanceMovement', que segue o heroi com uma distancia minima entre eles.


Versão 0.4.1 -

Adicionado nos arquivos do jogo diversos sprites dos futuros inimigos, com isso o 'enemySkeleton' foi alterado para sprite do 'BigSkeleton', uma tropa que tem intuito de ser grande, lenta, resistente e de alto dano. Foi otimizada a lógica do alvo posição para movimentação das IA para alvejar o centro do heroi.

Inimigos agora perdem vida ao serem atingidos e entram em estado de 'Hit'/'Machucados' parando algumas ações deles temporariamente, quando um alvo alcança 0 de vida ele entra em animação de morte e logo depois dela são devidamente deletados.
Também foi refinado a lógica na prioridade de ações, caixas e tempo de colisões dos golpes do herói, visando uma jogabilidade mais fluida, além da padronização de certos nomes de variáveis e simplificação de condições complexas. 

Agora os inimigos têm uma caixa de colisão para reações fazendo com que ao herói entrar na área eles ativem o estado de PRÉ-ATAQUE, caso o ataque esteja fora do tempo de recarga, geralmente acionando um intervalo antes do ataque real. Após o intervalo o inimigo entra em estado de ATAQUE criando caixas de colisão em momentos especificos, similar ao sistema implementado no heroi nessa mesma atualização, caso entre em contato com a caixa de colisão do heroi faz com que o mesmo receba dano. 
Foi implementado nos tempos de recarga a lógica de 'Action' e 'Invoke', permitindo que ao ser chamado ele possa executar alguma ação especifica quando terminar o seu Cooldown.


Versão 0.4.2 -

Sistema de 'Knockback' foi adicionado ao jogo, quando um inimigo ou jogador entra em estado de 'Hit' ele sofre um leve recuo na direção oposta do atacante e não pode agir temporariamente, foi presenciado um bug que faz com que receber dano em meio a um golpe deixa a caixa de colisão do ataque pra sempre causando danos fenomenais, embora já tenha sido corrigido é preciso ficar de olho em testes futuros.

Foi adicionado a animação de receber dano ao heroi, ela entra em ação enquanto o jogador recebe knockback. 
Foi adicionado a animação de morte do heroi, quando ele recebe dano suficiente o jogador não pode mais agir e 'morre'.

Dash agora ignora frames de colisão durante o avanço.


Versão 0.4.3 -

Ajustada a área de reação do enemySkeleton para ser mais condizente com a área de dano. 
Tamanho total do enemySkeleton foi aumentado e com isso sua caixa de colisão tambem foi ajustada, ele estava com tamanho muito similar ao jogador e outros inimigos com isso ele pode se tornar uma ameaça maior(literalmente) como era a ideia inicial.

Foi adicionado o enemyArcher, ele mantem a distancia do personagem entrando em estado de pré ataque caso ele entre no alcance e após a duração do ataque ele lança uma flecha em direção a ultima posição do heroi, quando leva dano ele rola pro lado contrário com 2 segundos de recarga entre os rolamentos.

Foi adicionado o enemySwarm, são inimigos que geralmente andam acompanhados, eles causam dano só de entrar em contato com o heroi com seu corpo, ao entrar no alcance de ataque eles avançam em sua direção após um curto intervalo, seu avanço e preparo podem ser parados com golpes. Atualmente esses inimigos  podem ser gerados com 3 tipos de cores diferentes, mas futuramente suas cores vão ser usadas para diferenciar suas variações selecionadas pela IA.


Versão 0.4.4 -

Foi adicionado um sistema complexo para gerenciar projéteis, primeiramente o ProjectileData.cs guarda todos os atributos dos projéteis que são passados pelo seu conjurador, Projectile.cs define e atualiza todas as funções atreladas ao projétil especifico e o ProjectileManager.cs gerencia os projéteis lançados com ajuda de uma lista permitindo que o Gerenciador do jogo possa lidar com eles de forma mais limpa.

Projeteis agora tem uma caixa de colisão coerente com sua posição e a origem de desenho deles foi alterada no construtor de animação e agora se da início no centro do 'sprite'. Essa lógica não foi aplicada para os demais objetos do jogo pois a lógica de colisão deles já está atrelada a origem do canto superior esquerdo no 'spritesheet' e o motivo dessa decisão é que as caixas de colisões dos projeteis não acompanhavam corretamente a sua rotação.

A posição atual do heroi foi adicionado a uma variavel global, agora podendo ser acessado mais fácilmente. 
Criada a função reset() no gerenciador de animações permitindo que resete animações no meio delas, a maioria dos inimigos foram afetados por essa mudança.

Todos os temporizadores dos inimigos foram testados e ajustados:
enemySkeleton - Tem tempo de recarga reduzido entre ataques, tempo de pré ataque inalterado, agora ao receber dano no fim do pré-ataque restitui o tempo dele para 0.9 dando uma breve reação ao jogador. 
enemyArcher - Tempo entre flechas reduzido para 1.6 segundos, agora não tem pré-ataque mas pode ser impedido por golpes, os ataques agora seguem a mesma lógica do enemyEskeleton e redefine o tempo entre ataques para 0.9 segundos, a habilidade de 'Recuar' agora restitue 100% do tempo de recarga do ataque. 
enemySwarm - Agora usa avanços a cada 2.5 segundos e tem o preparo reduzido para 1.25 segundos, caso um avanço seja parado no meio restitui 20% do tempo de recarga, agora tambem redefine a duração do pré-ataque caso seja parado no meio dele.

Foi adicionado o ultimo inimigo básico do jogo, enemyMage, ao entrar no alcance de reação ele usa uma magia aleatoria dentre um projétil perseguidor e uma área que causa lentidão no jogador até o fim do efeito dela ou até ele usar DASH, caso o alvo fique até alcançar uma velocidade de 1 ou menor na área recebe dano. Tambem foi adicionado um sistema para pintar sprites dentro do jogo com inutito de melhorar feedbacks para o jogador, atualmente só o heroi usa deste efeito quando está sob lentidão. 

////////////////////////////////////////////////////////////////////////////////////

Versão 0.5 -

Foi adicionado a Inteligencia Artificial para selecionar o perfil do jogador de acordo com os dados coletados durante a gameplay. Atualmente ele apenas recebe dados criados do C#, processa via Python e retorna o perfil do jogador. P modelo leva em consideração os seguintes parametros para definir o perfil; As médias de: tempo total, tempo para derrotar o inimigo depois do primeiro golpe, quantidade de dash's, durante o combate com cada tipo de inimigo. Nas atualizações futuras os dados vão ser coletados in-game e o perfil do jogador será utilizado para definição dos inimigos, que serão pequenas variações com atributos ou habilidades alteradas.


Versão 0.5.1 -

Foi adicionado IdleAI para todos os inimigos, agora eles ficam em estado de espera até o jogador entrar no alcance deles, ao entrar é acionado um novo comportamento pré-selecionado.
A ação de Cast/Conjuração agora lança um projétil na direção do mouse com tempo de recarga que causa dano ao entrar em contato com inimigos.

Foi adicionado os controles com o mouse, agora botão esquerdo da o ataque básico, o do meio usa o avanço e o direito usa magias, todos os controles se usam de orientação a posição do mouse
Está sendo adicionado o sistema para receber dados dos inimigos mas atualmente ocorre um erro por tentar acessar o arquivo de muitas partes do codigo.


Versão 0.5.2 -

O erro de acesso do arquivo foi concertado e agora a data de combate dos inimigos é passada ao JSON sempre que eles morrem, atualmente ao teclar a letra P é iniciado o script de python para calcular os perfis do jogador com cada inimigo.
Dados de combate são devidamente coletados durante combate atualmente são (Combat Time - Tempo de combate / Damage Window - Tempo de combate APÓS o primeiro golpe / Total Dashes - Totais de dash / Enemy Type - Tipo do inimigo)

Agora o ProfileSelector.py organiza e faz as predições utilizando como base cada tipo de inimigo, tendo em vista que cada um se comporta e tem atributos diferentes é mais justo que eles tenham diferentes pesos.
Foi adicionado a variavel KNOCKBACK no jogador que admnistra o tempo de recuo e de trava para ações durante o mesmo, a variavel RECOIL agora admnistra apenas o tempo de IVULNERABILIDADE, isso permite que o jogador tenha um tempo maior de resposta após receber danos, atualmente a diferença é de 0.2 para recuo e 0.4 para ivulnerabilidade.

Agora inimigos próximos são alertados quando um deles entra em combate com a função EnemyEngagement() presente no GameManager.cs, acionando a IA de combate respectiva deles.
Foram ajustados o alcance de ativação da IA de combate de todos os inimigos e ajustadas as caixas de reações para não ficarem longes da ideia original; EnemySwarm agora entra em combate antes de utilizar seu avanço impedindo bugs relacionados a ativação do alerta, EnemyArcher propositalmente atira fora do alcance de sua área de combate, EnemyMage tambem ativa habilidades fora do seu alcance de combate mas diferente do EnemyArcher ele entra em combate quando as ativa e alerta inimigos próximos.

Os projéteis do Enemymage agora perdem efeito de seguir o jogador ao colidir com o mesmo enquanto ele está durante alguma ivulnerabilidade(DASH ou RECOIL).


Versão 0.5.3 -

Foi introduzido o spawn e despawn de inimigos em horda, deixando a lógica de gerar inimigos mais clara.

Adicionado uma 'trava' de segurança para lista dos inimigos evitando bugs de acesso simultaneos na mesma.

Adicionado recurso de Pause através do botão 'P'.

Ajustado diversos atributos dos inimigos como tempos de reação, vida total e etc...

Todos os Spawns tiveram suas posições alteradas, agora a fase 2 contem spawns diferentes e mais populados.

Implementado efeitos de Fadein e Fadeout para uma transição visual mais suave entre cenas.

Desenvolvido sistema de "Mudança de Sala", que pausa o jogo, ajusta a posição do herói, executa o script de predição do Python, remove inimigos existentes e gera uma nova leva de adversários.
Introduzido o teleportador para mudança de sala, atualmente é nescessário matar 4 inimigos para ativar o portal.

Foi adicionado o Gerenciador de Aleatoriedade(RandomManager) para lidar com as chances aleatorias do jogo.

Foram adicionadas diversas variaveis do jogador em relação aos seus atributos, como dano, velocidade de movimento e de ataque etc...essas variaveis já foram implementadas e seus valores levemente ajustados.

Implementação da chance de critico aos golpes do jogador, inicialmente começa em 10% e causa 1.4x a mais de dano.

As imagens e o Gerenciador dos futuros Aprimoramentos foram adicionadas mas ainda não foram implementados.

Corrigidos e ajustados diversos inputs do jogador, incluindo:
>Correção do bug de dano infinito na janela de ataque e ajuste na animação de ataque para não mudar de direção enquanto ataca.
>Corrigido o bug de avanço inconsistente; agora, ele tem duração de 0.3 segundos, velocidade de 700, e o cooldown do dash foi aumentado de 0.7 para 0.9 segundos.
>Ajustado o bug da conjuração sempre seguir o mouse; agora, leva em consideração a orientação no momento do conjuro.


Versão 0.5.4 -

Essa é uma versão intermediaria criada para melhorar a experiencia do jogador nos proximos cenários de testes.

Inicalmente toda lógica de criação de inimigos no jogo foram realocadas para um novo construtor, EnemyManager.cs, deixando o gerenciador de jogo(GameManager.cs) mais limpo. 

Implementado agora 3 fases com Spawns cada vez mais distintos e desafiadores de acordo com o avanço do jogador

Inimigos agora entram em alerta ao receber danos fora do alcance de detecção do jogador.

Adicionado interface temporaria de feedback para o jogador, contendo vida, tempos de recarga e total de FPS que o jogo está rodando.

Foi implementado um sistema que dificulta o agrupamento de inimigos na mesma posição.


Versão 0.5.5 -

Atualizado para versão .NET 7.0 com intuito de suportar a nova biblioteca de interface GeonBit.UI.

Foi adicionado uma pré fase contendo apenas um inimigo, o motivo é que os construtores do jogo 'travam' sempre que o primeiro inimigo é derrotado, a fase 0 permite que o jogador derrote o primeiro inimigo e não seja punido pelo travamento da partida.

Projéteis em curso agora são deletados ao mudar de cena.

Esse update foi criado para ter um ponto de restauração no GIT antes da adição da Biblioteca GeonBit.UI no projeto.

//////////////////////////////////////////////////////////////////////////////////////

Versão 0.6 - 

Foi implementada a Biblioteca de construções de UI Myra, a decisão por ela ao invés de previamente citada GeonBit.UI é que ela é muito mais leve e adiciona APENAS elementos para organizar e gerar UI's.

Foram adicionadas janelas, botões e fontes nos arquivos do jogo para a construção de uma UI coesa.

Alguns códigos foram rearranjados em novas pastas para melhorar a organização dos mesmos.

Foi implementado o sistema UpgradeManager.cs, essa classe gerencia a janela de aprimoramentos e toda a lógica relacionado aos dados do jogador. Os seguintes aprimoramentos foram implementados:
> Damage Upgrade - Aumenta o dano dos ataques e magias.
> Speed Upgrade - Aumenta velocidade de movimento e reduz a recarga do avanço.
> Critical Upgrade - Aumenta chance e dano critico
> Vitality Upgrade - Aumenta a vida máxima e a restauração de vida entre fases.
> Spell Upgrade - Reduz tempo de recarga e aprimora os efeitos de Magias.
> Fierce Upgrade - Reduz tempo total de animação de Ataque e Conjuro.

Foram criadas diversas condições e funções para implementação dos novos efeitos:
>Heroi agora recupera passivamente 10 de vida por sala, aumentando com os aprimoramentos
>Foi adicionado uma função no AnimationManager.cs que atualiza a velocidade dos frames de qualquer animação já criada
>Magia do heroi agora pode ser aprimorada para lançar até 3 novos projéteis no mesmo conjuro

A implementação do objeto Soul.cs foi concluida, esse objeto gerencia quando o heroi ganha seus aprimoramentos. Ela é ativada ao usar um ataque básico dentro dos limites de colisão dela, fazendo com que o jogo pause e apareça 3 opções aleatorias de aprimoramento para o jogador, após consumi-la ela some. Podem aparecer até 2 almas por fase e são comumente atreladas a uma posição proxima aos Spawns dos inimigos.

Junto ao Soul.cs tambem foi criado um SoulManager.cs para gerenciar as funções de adição, atualização, desenho e exclusão de Almas/Soul.cs.

Agora collisionManagers.cs é chamado entre as trocas de fase para manipular suas listas de colisão de forma coesa.


Versão 0.6.1 - Redução no tempo de recarga da magia em 1 segundo.


Versão 0.6.2 - 

Os spawns foram alterados para ficarem mais ou menos do mesmo nivel de habilidade, permitindo com que seja mais clara a coleta de dados.

Agora a alma é ativada usando a caixa de colisão do ataque do heroi e não a o heroi em si.


Versão 0.6.3 - 

Adicionado algumas etapas para evitar erros na coleta de dados, inimigos mortos não parecem computar o tempo total da luta e tempo de janela de dano, o erro parece persistir com o primeiro Mago(Inimigo tipo 4) morto por gameplay.

//////////////////////////////////////////////////////////////////////////////////////

Versão 0.7 -

A biblioteca k-Nearest Neighbors (knn) agora ignora dados corrompidos, melhorando a precisão do processamento.

Perfis de jogadores da biblioteca foram atualizados e calibrados.

Ajuste na caixa de colisão do arqueiro.

A lógica do enemyswarm foi alterado para incluir um tempo de espera antes de poder causar dano novamente após receber dano.

Os atributos base dos inimigos do tipo swarm foram modificados para refletir melhor a dificuldade pretendida.

O dano infligido por todos os inimigos agora varia de acordo com seus próprios atributos.

Inimigos agora se adaptam ao perfil do jogador, alterando seus atributos base; estas mudanças tornam-se efetivas a partir do estágio 3 e tem o seguinte padrão:
> Perfil agressivo: torna os inimigos mais reativos, diminuindo efetividade de investidas diretas.
> Perfil balanceado: trazem pequenas melhorias variadas, incluindo aumento de vida dos inimigos.
> Perfil evasivo: torna os inimigos mais rápidos e propensos a atacar diretamente a distancia.

Perfis com contagens altas e similares (margem de 4%) são selecionados aleatoriamente entre si para adicionar variedade ao jogo. (ProfileManager.cs)

Uma nova tela de Game Over foi adicionada, exibindo gráficos com as estatísticas do jogador e indicando o perfil predominante do mesmo ao final da sessão. (ProfileChartsManager.cs)

//////////////////////////////////////////////////////////////////////////////////////

Versão 0.8 -

A funcionalidade de entrada e fluxo dos menus foi implementada.

Os dados do jogador agora são guardados em um arquivo aparte e deletados automaticamente ao final de cada sessão.

Os spawns de inimigos foram ajustados de acordo com o perfil do jogador depois do estágio 2 para criar desafios personalizados.

O número de spawns foi reduzido para três, as unidades em cada um deles foram alteradas. Agora, Os requisitos de inimigos por fase foram ajustados para equilibrar a progressão e o desafio ao longo do jogo. Almas ainda aparecem em dois spawns por estágio.

Introdução de novas magias:
> Explosão: Causa dano na posição do mouse, aumenta o tamanho de acordo com spellTier. 
> Raios: Cria 4 trovões na direção alvo com posicionamento levemente aleatorios, diminui a aleatoriedade de acordo com o spellTier deixando eles cada vez mais alinhados. 

As magias foram adicionadas às rolagens e não é possivel tirar mais de um tipo diferente por rolagem de aprimoramento, aprimorar magias aumenta levemente o SpellDamage do heroi.

As magias foram aprimoradas para afetar múltiplos inimigos dentro do alcance de dano(esta alteração não se aplica a projéteis).

//////////////////////////////////////////////////////////////////////////////////////

Versão 0.9 -

Corrigido o bug que os inimigos continuavam com os atributos do perfil após o game over.

Corrigido o bug em que 'Explosão' e 'Trovoes' causavam dano no mesmo inimigo mais de uma vez por conjuro. (Magias de projéteis ainda pode acertar o mesmo inimigo com mais de um projétil por conjuro)

Posição inicial do heroi e do portal para mudar de estágio foram atualizados.

Reduzido o tempo total que projeteis perseguidores seguem o jogador, eles ainda continuam seguindo a ultima posição recebida até o fim da duração.

Reduzido a força dos aprimoramentos de chance critica, valores de dano critico não foi alterado

Chefão final adicionado: 

>é um combate que contem 3 inimigos estáticos que ficam alternando entre si para lançar habilidades, cada um tem um ataque unico que usam em todos os combates porem de acordo com o perfil do jogador um deles é selecionado para ter mais vida e usar uma habilidade mais forte apos uma certa quantidade de conjuros.

Habilidades dos chefes:

> Mago Vermelho cria explosões, na versão melhorada cria diversas delas em pontos próximos. 
> Mago Azul lança uma saraivada de projeteis, lança diversas ondas de projeteis na versão melhorada. 
> Mago roxo lança áreas de lentidão e um projétil perseguidor, na versão melhorada o projétil perseguidor é substituido por um inimigo frágil. 

Fluxo das fases alterados, nova ordem:

>Fase 1 contem 3 almas > Fase 2 contem 2 almas > fase 3 contem inimigos alterados e 2 almas > fase do chefão não contem almas > Fim do jogo.

Atualmente o jogo trava se o jogador vai direto para fase do chefe enquanto faz os calculos do perfil, para lidar com o problema foi adicionado uma fase entre elas para fazer o calculo antes de entrar no estágio final, essa fase não tem requerimento minimo de inimigos então o jogador passa dela automaticamente.

//////////////////////////////////////////////////////////////////////////////////////

Versão 1.0 -

Implementado a Soundtrack do jogo para cada tela, incluindo menu, game over, in-game e sala final.

Texturas do cenário agora alteram de acordo com a fase.

Corrigido bug que enemySkeleton mudava de direção durante o ataque, agora é possivel esquivar do seu golpe indo para direção oposta dele.

//////////////////////////////////////////////////////////////////////////////////////

MIT License

Copyright (c) 2024 José Felipe Paganelli Velloso

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CON


