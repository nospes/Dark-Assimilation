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
Foi implementado tileset e limitações do mapa, agora o jogador tem um espaço limitado para se movimentar e a camera o segue enquanto ele explora o ambiente. 
Por fim foi adicionado comportamentos básicos para inimigos e sua herança relacionada, agora inimigos podem ter entre 3 comportamentos pré-definidos, entre eles; 'GuardMovement', que protege uma área especifica e sempre retorna a ela ao sair do alcance, 'FollowHero', que segue inimigo independente do alcance, e 'DistanceMovement', que segue o heroi com uma distancia minima entre eles.