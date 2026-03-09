Projeto: HelpDesk


Este programa foi desenvolvido para automatizar a gestão de ativos de hardware e software em uma rede corporativa. Ele elimina a necessidade de coleta manual de dados, garantindo que as informações sempre atualizadas em um servidor central.


*HelpDesk v1.0:*


-Identificação Única: O programa usa o número de série da placa-mãe como chave principal para identificar cada notebook e não deixar criar cadastro duplicado no banco de dados;

-Coleta de Hardware: O programa coleta as informações de Processador, Memória RAM e qual Sistema Operacional a máquina está usando foram pegas via WMI;

-Espaço em Disco: O programa coleta o tamanho total do HD/SSD e já calcula a porcentagem de uso para saber quais máquinas estão ficando sem espaço;

-Quem está usando: O programa coleta o nome do usuário que está logado na hora e o nome da máquina (hostname);

-Acesso à Rede: O programa faz uma impersonação (credenciais administrativas) para conseguir salvar os dados na pasta da rede mesmo que o usuário logado não tenha permissão de acesso;

-Histórico de Nomes: Caso alguém mude o nome do notebook, o sistema percebe pelo número de série da placa-mãe. Acontece o armazenamento do nome antigo junto das informações em um arquivo de históricos e atualiza o novo nome junto das informações na planilha principal.

-Tudo em CSV: O programa deixa tudo organizado em arquivos CSV, o que facilita a visualização e a manipulação das informações.



*HelpDesk v2.0:*

-Instalação em Lote: O sistema agora permite a seleção múltipla de softwares e executa as instalações de forma simultânea. Isso reduz o tempo de preparação de uma máquina nova em até 70%.

-Segurança e Criptografia (SHA-256): Implementação de um Admin Center protegido por senha. O sistema não armazena a senha real, mas sim um hash criptográfico com Salt, impedindo que qualquer pessoa com acesso ao código-fonte descubra a credencial mestre.

-Redundância de Conexão: O repositório de rede agora possui inteligência de conexão. Caso o servidor não responda pelo nome de rede (Hostname), o programa alterna automaticamente para o endereço IP, garantindo que os logs sejam salvos mesmo com falhas de DNS.

-Execução Local Segura: Para evitar falhas de instalação por oscilação de rede, o sistema agora realiza o Download-to-Temp. O instalador é copiado para a pasta temporária local da máquina antes de ser executado, garantindo maior estabilidade no processo.

-Auditoria de Acessos: Foi criado um sistema de logs de auditoria que registra quem acessou o menu administrativo, por quanto tempo permaneceu e se houve tentativas de login sem sucesso, aumentando a governança sobre as ferramentas de TI.

-Arquitetura DTO: O código foi refatorado para usar objetos de transferência de dados, separando a lógica de coleta da lógica de persistência. Isso torna o sistema mais leve e fácil de dar manutenção.

-Tratamento de Exceções Robusto: O sistema agora isola falhas. Se uma instalação de software falhar ou um componente WMI não responder, o programa continua a execução dos demais módulos em vez de travar por completo.


