# Teste 'Slice It All' - IzyPlay

[Download APK](https://github.com/lcscout/izyplay-test/releases/tag/v1.0.11)

O teste consistia em fazer um clone do jogo **Slice It All** num prazo de 60 horas. O processo foi bem difícil, principalmente por conta do pouco tempo. Meu approach foi basicamente o mesmo de quando eu faço um protótipo, analisar o jogo/pensar nos sistemas essenciais → mecânicas principais → UI simples → outros sistemas pertinentes → refatorar.

Primeiro fiz a camera seguir o player e aí fui fazer a mecânica mais importante, a movimentação do jogador. O player sendo uma faca, sua movimentação consiste em rotacionar em seu próprio eixo e ao mesmo tempo mover-se para frente. Foi bem difícil chegar em modelo que me agradou e passei muito tempo aqui já que é a mecânica central do jogo.

Com a movimentação pronta, fui fazer a interação do jogador com o mundo. São dois tipos de interação principais, a faca/player ficar presa no objeto ou cortá-lo. Ficar preso foi mais fácil de implementar, já o corte foi uma dificuldade maior. Meu primeiro pensamento foi usar a faca como plano de corte e criar duas novas meshes a partir daí, porém como nunca mexi com manipulação de meshes por script eu achei melhor investir esse tempo pensando em outra ideia. A saída que encontrei foi fazer com que o objeto já esteja cortado em dois, e ao “cortá-lo” simplesmente separar as duas metades.

A partir daí fiz o primeiro nível e a UI simples, e adicionei outros sistemas propícios, por exemplo: o final dos níveis, pausa, perder o jogo quando cair ou bater em um espinho, mais níveis e skins de player e a troca desses.

## Sistemas

Dá pra dividir os sistemas em três áreas maiores: elementos de gameplay, elementos de UI e gerenciamento. Os sistemas se conversam por eventos, seguindo majoritariamente o Observer design pattern, o GameManager.cs porém, é Singleton e persiste pelas cenas.

### Gameplay

O ‘core loop’ da gameplay é basicamente, o player seguir pra frente, não morrer, e cortar os objetos e receber moedas por isso e ao final do nível ele tem a chance de multiplicar suas moedas. Grande parte do loop é entregada pelo próprio player, PlayerController.cs.

#### Camera

A câmera utiliza seu posicionamento no editor como base, e simplesmente segue seu alvo (player) utilizando de interpolação linear pra ter uma movimentação suave.

#### Player

A movimentação começa quando o PlayerController.cs recebe o evento de que um clique foi realizado na tela. Após uma verificação se o jogo não está pausado, é feita uma preparação para rotacionar (coloca todos os parâmetros usado no método de rotação de volta para seus valores padrões e aciona a flag para que a rotação possa ser feita) e é adicionado as forças vertical e horizontal do movimento.

Com a flag acionada, a rotação é feita durante o FixedUpdate, isso para garantir que a rotação seja constante. Quando o controlador identifica que a volta está perto de completar ele aumenta a arrasto angular para desacelerar o movimento, e ao completar a volta ele para a rotação completamente.

Existem três interações do player com o ambiente, quando o jogador colide com o chão ou com um objeto Spike, o jogo entra no estado de game over e deve ser reiniciado. As outras duas interações dependem de qual parte da faca entra em contato com o objeto:

#### Blade

A lâmina pode ficar presa ou cortar objetos, dependendo de seu tipo. Ao colidir com um objeto do tipo Stuckable, ela fica presa, isso é, uma FixedJoint é criada juntando os dois. Já ao colidir com um objeto do tipo Cuttable, ele é cortado, como esse tipo de objeto já é preparado para ter duas metades separadas, o método de corte apenas cria rigidbodies para cada metade (caso eles já não tenham) e aplica uma força pra jogar eles longe da lâmina, dando a impressão de corte. Ao cortar também é adicionado às metades um script que destrói o objeto quando ele fica longe do player, para reduzir a quantidade de objetos em cena.

#### Handle

O cabo possui a mesma interação seja com Stuckables ou Cuttables, ele irá chamar o método de rotação com a direção invertida, o movimento horizontal também se inverterá, dando a chance de consertar um erro feito.

#### Player Spawn

SpawnPlayer.cs foi uma forma de manter a skin do player quando o nível é mudado. Na inicialização da cena o SpawnPlayer.cs instancia um player com a skin guardada no GameManager.cs.

### UI

A UI é especial porque ela é quem recebe o input do player. Escolhi fazer dessa forma pois existe apenas um comando possível no jogo, e ele é recebido através de qualquer ponto tocado da tela. Dessa forma, para evitar conflito entre o toque-em-qualquer-ponto-da-tela e por exemplo um toque em um botão (que está em qualquer ponto da tela) o input é recebido através do toque em uma imagem de fundo transparente de qualquer outro elemento da UI. Elementos que não são interativos têm sua interatividade removida para que o input seja recebido corretamente.

A UI é dividida entre menus e parte independente. A parte independente é composta pela área clicável (que recebe o input para a movimentação do player), a quantidade atual de moedas, e o botão de pausa (só aparece quando o jogo de fato começa). O menu inicial possui o nível atual, para localização do player, os botões que abrem os menus de para a troca de níveis e skins, e a instrução de como jogar. Os menus de níveis e de skins são similares, são compostos de uma ScrollView que possui os botões referentes a cada escolha de troca. O menu de gameover aparece ao terminar um nível, e possui um background que escurece a tela, a quantidade de moedas que o player terminou e um botão para continuar para o próximo nível. O menu de pause/restart é similar ao de gameover, com a diferença de não mostrar as moedas e não possuir botão, ao clicar na tela o nível é reiniciado.

### Gerenciamento

#### Da UI

O UIManager.cs mantém a referência dos elementos da interface e os modifica de acordo com os eventos em que é inscrito. Possui métodos para gerenciar os elementos referenciados, principalmente os diferentes menus, além de também receber o input do player para métodos que alteram estados do jogo como Continue, para passar para o próximo nível, Restart para reiniciar o jogo e Pause para pausar e despausar. Uma outra função importante do manager é enviar o evento de clique na área de jogo, e caso algum menu esteja aberto, fechá-lo.

#### Do Jogo

O GameManager.cs é um Singleton e persiste pelas cenas para fácil acesso das propriedades e métodos que são utilizados em vários lugares. O manager é responsável principalmente por de fato alterar os estados do jogo, ao receber o evento OnFirstTap, ou primeiro clique, ele altera a propriedade HasGameStarted para que os outros sistemas identifiquem o jogo começou. De forma análoga, ao pausar ele altera o Time.timeScale para 0, e para 1 ao despausar. No fim de jogo, além da pausa é multiplicado às moedas uma quantidade definida pelo EndGameCollider.cs que o player acertou. O manager ainda é responsável pela troca de skins, ao spawnar um novo player com a skin definida por ChangeSkin.cs e deletando o player antigo, e pela troca de cenas.
