# Servidor de Controle de Acesso

Este projeto é um servidor de controle de acesso que utiliza câmeras LPR (Reconhecimento de Placas) Alphadigi para ler placas de veículos, verificar essas leituras em um banco de dados Firebird e armazenar dados relevantes em um banco de dados SQLite.

## Funcionalidades

- **Reconhecimento de Placas**: Integra-se com câmeras LPR Alphadigi para ler placas de veículos.
- **Verificação de Banco de Dados**: Verifica as placas lidas em um banco de dados Firebird.
- **Armazenamento de Dados**: Armazena logs de acesso e outros dados relevantes em um banco de dados SQLite.
- **Endpoints RESTful**: Fornece endpoints para interagir com o sistema.
- **Middleware**: Inclui middleware para monitoramento de tempo de requisição.