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

