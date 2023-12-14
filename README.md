# Monogame

Monogame + AI

Projeto para desenvolvimento da minha iniciação cientifica: APLICABILIDADE DE INTELIGÊNCIA ARTIFICIAL NA CUSTOMIZAÇÃO DE CENÁRIOS E AGENTES EM JOGOS DIGITAIS

![image](https://github.com/nospes/Monogame/assets/57772398/ca974406-1c68-4ce2-9438-1ef752751e42)

Versão 0.1- foi implementado os inputs de movimento(WASD), controladores básicos do jogo e de animações. O jogador atualmente anda e fica parado

Versão 0.2- foi implementado o input de ataque básico(K), animação de soco, sistema para alternar entre golpes e movimento, primeiro inimigo, esqueletos(ainda sem animação) e uma lista para poder adicionar diversos inimigos do mesmo tipo.

Versão 0.2.1- Atualizando alguns parametros para poder utilizar em outras máquinas, agora é possivel mexer no código e no repositorio remotamente de outros pontos.

Versão 0.2.2- esqueletos agora tem animação, foi implementado o lógica básica para detectar colisões(ainda precisa ser aprimorado).

Versão 0.3 - Sistema de detecção e as Areas de colisões foram aprimorados, foi implementado um sistema para ver área de colisões das unidades.

Versão 0.3.1 - Implementação da colisão dos golpes, colisão entre golpes e inimigos.

Versão 0.3.2 - Movimentação dos inimigos, referencia de posição do heroi para movimento(precisa ser aprimorado)

Versão 0.3.3 - Adicionado Herança para comportamentos, inimigos e jogador. Agora é possivel ter diversos comportamentos e chama-los para objetos com herança 'enemyBase', a herança base para inimigos comuns. Melhorado o sistema de colisões, agora eles calculam com base no centro do sprite reduzindo a quantidade de valores absolutos para formaçao da caixa. Agora é possível desenhar os retangulos de teste de área de colisão diretamente no Draw() das unidades.

Versão 0.3.4 - Área e Lógica da coslião dos golpes atualizadas.

Versão 0.3.5 - Heranças de inimigos atualizadas para suportar funções das unidades, agora é possivel colocar todos os inimigos de diferentes tipos mas de heranças compatilhadas na mesma lista e chamar suas funções nos gerenciadores de jogo. Lógica das variaveis de colisão dos inimigos atualizadas. Foi retirada uma condição obsoleta do gerenciador de animações. Diversos códigos tiveram seus comentários atualizados para explicação mais clara do seu funcionamento.

Versão 0.4 - Foi adicionado a animação de 'conjuração', mecanica de 'dash' e tempo de recarga para todas as essas ações, foi corrigida a caixa de colisão para os estados do heroi, incluindo os golpes e movimentações.
Foi implementado tilesets, camera e limitações do mapa, agora o jogador tem um espaço limitado para se movimentar e a camera o segue enquanto ele explora o ambiente finito. Algumas partes do código foram simplificadas e compactadas para deixar mais claro ao entendimento e leitura dele.

Tambem foram adicionados os comportamentos básicos para inimigos e sua herança relacionada, agora inimigos podem ter entre 3 comportamentos pré-definidos, entre eles; 'GuardMovement', que protege uma área especifica e sempre retorna a ela ao sair do alcance, 'FollowHero', que segue inimigo independente do alcance, e 'DistanceMovement', que segue o heroi com uma distancia minima entre eles.

Versão 0.4.1 - Adicionado nos arquivos do jogo diversos sprites dos futuros inimigos, com isso o 'enemySkeleton' foi alterado para sprite do 'BigSkeleton', uma tropa que tem intuito de ser grande, lenta, resistente e de alto dano. Foi otimizada a lógica do alvo posição para movimentação das IA para alvejar o centro do heroi.

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

Versão 0.4.4
Foi adicionado um sistema complexo para gerenciar projéteis, primeiramente o ProjectileData.cs guarda todos os atributos dos projéteis que são passados pelo seu conjurador, Projectile.cs define e atualiza todas as funções atreladas ao projétil especifico e o ProjectileManager.cs gerencia os projéteis lançados com ajuda de uma lista permitindo que o Gerenciador do jogo possa lidar com eles de forma mais limpa. 
Projeteis agora tem uma caixa de colisão coerente com sua posição e a origem de desenho deles foi alterada no construtor de animação e agora se da início no centro do 'sprite'. Essa lógica não foi aplicada para os demais objetos do jogo pois a lógica de colisão deles já está atrelada a origem do canto superior esquerdo no 'spritesheet' e o motivo dessa decisão é que as caixas de colisões dos projeteis não acompanhavam corretamente a sua rotação.

A posição atual do heroi foi adicionado a uma variavel global, agora podendo ser acessado mais fácilmente. 
Criada a função reset() no animation manager permitindo que resete animações no meio delas, a maioria dos inimigos foram afetados por essa mudança.

Todos os temporizadores dos inimigos foram testados e ajustados:
enemySkeleton - Tem tempo de recarga reduzido entre ataques, tempo de pré ataque inalterado, agora ao receber dano no fim do pré-ataque restitui o tempo dele para 0.9 dando uma breve reação ao jogador. 
enemyArcher - Tempo entre flechas reduzido para 1.6 segundos, agora não tem pré-ataque mas pode ser impedido por golpes, os ataques agora seguem a mesma lógica do enemyEskeleton e redefine o tempo entre ataques para 0.9 segundos, a habilidade de Recuar agora restitue 100% do tempo de recarga do ataque. 
enemySwarm - Agora usa avanços a cada 2.5 segundos e tem o preparo reduzido para 1.25 segundos, caso um avanço seja parado no meio restitui 20% do tempo de recarga.

Foi adicionado o ultimo inimigo básico do jogo, enemyMage, quando entra no alcance de reação ele usa uma magia aleatoria dentre um projétil perseguidor e uma área que causa lentidão no jogador até o fim do efeito dela ou até ele usar DASH, caso o alvo fique até ficar com velocidade menor que 1 na área, recebe dano. Tambem foi adicionado um sistema que muda a cor do heroi quando ele está sob o efeito de lentidão, permitindo um feedback mais claro ao jogador. 

