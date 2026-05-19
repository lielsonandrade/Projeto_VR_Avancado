# Projeto 3D no Unity – Curso de Metaverso Web3

**Autor:** Liélson Dos Santos Andrade

**Trilha:** 1

## Sobre o Projeto

Este projeto foi desenvolvido utilizando a engine Unity como atividade prática para o curso de Metaverso Web3. O objetivo foi criar um ambiente 3D inspirado em um ponto turístico da cidade de Campina Grande, conhecida por sediar “O Maior São João do Mundo”, especificamente o Parque Evaldo Cruz.

O cenário foi construído buscando representar um ambiente urbano agradável, contendo uma fonte central, bancos ao redor, postes de iluminação e elementos naturais, como árvores e vegetação, organizados de maneira semelhante ao parque utilizado como inspiração.

## Elementos Principais do Cenário

O projeto possui 4 modelos 3D principais:

*   Banco
*   Poste de luz
*   Fonte
*   Árvore

Todos os objetos foram posicionados estrategicamente para criar uma ambientação mais realista e próxima da proposta do parque urbano inspirado em Campina Grande.

## Assets e Recursos Utilizados

Durante o desenvolvimento, foram utilizados alguns recursos externos para auxiliar na construção do cenário.

### Assets da Unity Asset Store

Foram utilizados assets disponibilizados pela Unity Asset Store para:

*   Fonte
*   Banco
*   Poste de iluminação
*   Árvores
*   Skybox
*   Texturas

Algumas texturas adicionais foram obtidas através do site Poliigon para melhorar a qualidade visual do ambiente.

## Dificuldades Encontradas

Durante o desenvolvimento do projeto, ocorreram diversas dificuldades técnicas, principalmente relacionadas às limitações de hardware.

O computador utilizado possui apenas 16GB de memória RAM, e o projeto na Unity estava consumindo muitos recursos, ocasionando travamentos e crashes frequentes por falta de memória. Em alguns momentos, foi necessário refazer partes do projeto devido à perda de alterações não salvas após os fechamentos inesperados da engine.

Além disso, houve dificuldades na utilização do simulador de VR da Meta. Mesmo seguindo corretamente os tutoriais de configuração, o simulador apresentou problemas de funcionamento e não foi possível demonstrar o personagem andando pelo cenário em realidade virtual. As limitações de hardware também contribuíram para esse problema.

Apesar dessas dificuldades, o projeto foi concluído com sucesso e serviu como uma importante experiência prática no desenvolvimento de ambientes 3D voltados para aplicações relacionadas ao metaverso e à realidade virtual.

## Configuração Técnica

*   **Versão do Unity:** 6.3 LTS (6000.3.13f1)
*   **Plataforma:** Android (Meta Quest)
*   **SDK de XR:** Meta XR SDK (instalado e configurado corretamente)
*   **Gerenciamento de Plugins XR:** Configurado adequadamente

## Tecnologias Utilizadas

*   Unity
*   Assets da Unity Asset Store
*   Texturas da Poliigon

## Objetivo do Projeto

O principal objetivo deste projeto foi aplicar conceitos de modelagem e ambientação 3D dentro do contexto de Metaverso Web3, explorando a criação de cenários virtuais inspirados em locais reais e desenvolvendo experiência prática com a engine Unity.

## Atividade Avançada: Interação com Bancos

Para aprimorar a experiência do usuário, foi implementada uma funcionalidade de interação com os bancos presentes no cenário. Ao se aproximar de um banco, um `Canvas` é exibido com a mensagem instruindo o usuário a pressionar a tecla 'E' para sentar. Esta interação adiciona um nível de imersão e dinamismo ao ambiente 3D.

## Solução Técnica para o XR Simulator da Meta

Uma dificuldade significativa foi encontrada com o XR Simulator da Meta, que apresentava falhas (`crashava`) consistentemente durante as tentativas de execução. Após investigação aprofundada, verificou-se que o problema estava relacionado a um bug na versão recente do simulador. A solução implementada para contornar essa questão foi a seguinte:

1.  Acessar `Edit` > `Project Settings` > `XR Plug-in Management`.
2.  Na aba de configuração para `PC`, desmarcar todas as opções relacionadas ao XR.
3.  Na aba de configuração para `Android`, manter a configuração correta, com `OpenXR` marcado.

Essa abordagem permitiu que o projeto fosse executado e testado diretamente no editor do Unity, sem a necessidade do simulador da Meta. Dessa forma, a interação com o ambiente foi possível utilizando os movimentos padrões de teclado (A, S, W, D) para locomoção e a tecla 'E' para interagir com os bancos.

## Scripts C# Desenvolvidos

Para viabilizar a interação e movimentação no ambiente, foram desenvolvidos três scripts em C#:

*   `BenchInteraction`: Responsável por gerenciar a lógica de interação com os bancos, permitindo que o usuário "sente" ao pressionar a tecla designada.
*   `InteractionPrompt`: Exibe uma mensagem (`Canvas`) na tela quando o usuário se aproxima de um objeto interativo (como um banco), instruindo-o sobre como interagir.
*   `PCMovement`: Controla a movimentação do personagem no ambiente utilizando as teclas de teclado (A, S, W, D).

## Configuração do Player para Teste no Editor

Para possibilitar os testes diretamente no editor do Unity, foi necessário realizar uma modificação na configuração do Player. O `BuildingBlock Camera Rig` original do XR Meta foi desativado, e um novo Player foi criado, incorporando os seguintes componentes:

*   `RigidBody`
*   `Character Controller`
*   `Camera`
*   Script de movimento (`PCMovement`)

Essa configuração personalizada permitiu simular a experiência de jogo e testar as interações no ambiente sem depender do simulador XR da Meta, que apresentava instabilidades.
