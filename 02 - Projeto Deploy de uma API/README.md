

# Como Fazer o Deploy de uma API na Nuvem na Prática

## DESCRIÇÃO
Criação e deploy de uma API utilizando o Azure, com foco na integração com o Azure Container Registry (ACR) e Web App. Implantação de uma API no Azure sem downtime, permitindo mudanças de versão contínuas. Este tutorial é focado em mostrar como a automação de deploy pode ser feita de forma eficiente, usando ferramentas do Azure.
 


## Instrutor
**Henrique Eduardo Souza**  
Microsoft MVP, Microsoft  
[LinkedIn](https://www.linkedin.com/in/hsouzaeduardo/?locale=pt_BR)


## Ferramentas Necessárias

- **Visual Studio Code ou Visual Studio 22**: Utilizado para criar a API base, que será publicada no Azure.  
  [Visual Studio Code](https://code.visualstudio.com/)  
  [Visual Studio](https://visualstudio.microsoft.com/pt-br/)
- **Azure DevOps**: Usado para gerenciamento de código, criação de pipelines e automação do deploy.  
- **Docker**: Utilizado para containerizar a API.  
- **Azure Container Registry (ACR)**  
- **Azure Web App**  
-  [**C#**](https://learn.microsoft.com/pt-br/dotnet/csharp/)
-  [**ASP.NET Core**](https://learn.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-5.0)
-  [**OpenWeatherMap**](https://openweathermap.org/api)
-  [**Swagger**](https://swagger.io/)
-  [**Newtonsoft.Json**  ](https://www.newtonsoft.com/json)


## Passo 1:  Criar um conta de Visual Studio associada aos Devops: 

<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


A API é integrada com o Azure DevOps para realizar o deploy, utilizando um repositório Git e pipelines para automação do processo.


<p align="center">
    <img  src="imagens/imagem002.png" width="1000"/>  
</p>


 


<p align="center">
    <img  src="imagens/imagem003.png" width="1000"/>  
</p>


 


## Passo 2:  Criar uma organização no Visual Studio Devops

 [Link de Referência](https://learn.microsoft.com/pt-br/azure/devops/organizations/accounts/create-organization?view=azure-devops)  


<p align="center">
    <img  src="imagens/imagem004.png" width="1000"/>  
</p>

 


## Passo 3:  Criação de um Repositório: 
O nome do repositório é fundamental para o fluxo de trabalho. É onde as imagens da API serão armazenadas. A sugestão é nomear o repositório com algo como DIO- API, que armazenará as imagens Docker.


<p align="center">
    <img  src="imagens/imagem005.png" width="1000"/>  
</p>


 



<p align="center">
    <img  src="imagens/imagem006.png" width="1000"/>  
</p>


 



## Passo 4:  Criação de um projeto no Visual Studio: 
Criação de um projeto no Visual Studio e configuração de um Dockerfile para rodar a API. O processo começa com um desenvolvedor criando um código de API, hospedando-o no Azure DevOps e realizando o deploy no ACR. 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

A API é baseada em previsões de tempo, com temperatura em Celsius e Fahrenheit


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

## Passo 5:  Configuração do Dockerfile para rodar remotamente


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>


 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


## Passo 6:  Configuração no Azure: 
Utilizando o Marketplace do Azure, o instrutor cria dois recursos principais no Azure: o grupo de recursos e o Azure Container Registry (ACR), onde a imagem do container será armazenada. Demora alguns minutos.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


Criação de um Grupo de Recursos e um Container Registry (ACR) para armazenar as imagens Docker da API.

<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

## Passo 7:  Criação do Web App: 
Em seguida, é criado um Web App para rodar o container na nuvem, configurando-o para usar o ACR como repositório do container.  

<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


## Passo 8:  Configuração do Deploy para hospedar a API. 
O deploy foi concluído, e agora é necessário configurar o ambiente no Deployment Center do Azure.  
- **Escolher o tipo de container**: Single Container, Docker Compose, ou Private Container.  
- **Configurar o Container Registry (ACR)**, selecionando a assinatura e identificando qual ACR API será utilizada.  
- No ACR, pode não haver imagens ainda, então será necessário prepará-las.  
![Imagem Passo 8](path/to/image8)

<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

## Passo 9:  Criar um novo Azure DevOps Pipeline: 
Acessar o Azure DevOps e garantir que o repositório (exemplo: DIO-API) esteja pronto para trabalhar com o pipeline. O código já está no repositório, então o próximo passo é configurar o pipeline.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

## Passo 10:  Configuração do Pipeline: 
Utilizar o editor de YAML, semelhante ao GitHub Actions, para configurar o pipeline. O pipeline é configurado com YAML para controle de versões, compilação e testes da solução  Definir o trigger do pipeline: qual branch deve ser monitorada para disparar o pipeline, ou quando o código for movido via Pull Request.  O pipeline é configurado para automatizar a construção e o deploy da API, utilizando comandos dotnet restore e dotnet build. 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


Escrever e configurar o pipeline de forma que ele execute as tarefas necessárias, como build e deploy da aplicação.  
- **Variáveis Usadas na Pipeline**:  
  - `solution`: Referência para o arquivo *.sln, a solução do projeto.  
  - `ncpu`: Plataforma de build (CPU).  
  - `build configuration`: Configuração de compilação, com valor **release** para builds de produção.

- **Etapas da Pipeline**:
  - Instalação do .NET SDK: Usando uma tarefa do Azure DevOps para instalar o SDK necessário para compilar o projeto.  
  - Restaurar Dependências: Usando o comando `dotnet restore` para restaurar os pacotes NuGet.  
  - Compilação do Projeto: Usando o comando `dotnet build` para compilar a solução.  
  - Execução de Testes (opcional): Se necessário, pode-se rodar os testes automatizados usando o comando `dotnet test`.  

- **Utilização de Assistentes**: O uso de assistentes facilita a configuração do pipeline, como o assistente Docker para o registro e gerenciamento de imagens.  



```yaml
trigger:
- main

pool:
  vmImage: ubuntu-latest

variables:
  solution: '**/*.sln'
  buildPlatform: 'Any CPU'
  buildConfiguration: 'release'

steps:
- script: echo Hello, world!
  displayName: 'Run a one-line script'

- script: 
- task: UseDotNet@2
  displayName: 'Instal Net SDK'
  inputs:
    packageType: 'sdk'
    version: '8.x'

- script: dotnet restore $(solution)
  displayName: 'Restore Solution'

- script: dotnet build $(solution) --configuration $(buildConfiguration)
  displayName: 'Build Solution'

- script: dotnet test $(solution) --configuration $(buildConfiguration) --no-build --collect: 'XPlat Code Coverage"
  displayName: 'Build Solution' 

- task: Docker@2
  inputs:
    containerRegistry: 'acrapidemohsouza'
    repository: 'api-dio-test'
    command: 'buildAndPush'
    Dockerfile: './APITempoDIO/Dockerfile'

```


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

Durante a configuração do pipeline, é necessário apontar corretamente o caminho para o Dockerfile e ajustar o contexto de build (diretório onde o Dockerfile está localizado).


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


Corrigimos a localização da solução dentro do repositório, ajustando o caminho no arquivo YAML da pipeline para garantir que os comandos funcionem corretamente. O comando é ajustado para apontar para a pasta correta.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

O pipeline também prepara a aplicação para ser implantada em um container Docker, utilizando a ferramenta Docker para fazer o "pull" e "push" da imagem no Azure Container Registry (ACR).


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 
 

## **Passo 11:  Conexão de Serviço: **
Para interagir com o ACR, o pipeline precisa de uma "Conexão de Serviço" configurada no Azure DevOps, permitindo a autenticação sem senha através do Azure Service Principal.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


## **Passo 12:  Build e Push da Imagem Docker: **
O processo de construção e envio da imagem Docker para o ACR pode ser separado em duas etapas: build e push. Isso ajuda a identificar rapidamente onde ocorrem falhas. A pipeline é organizada de forma modular, permitindo que cada parte seja facilmente ajustada sem afetar todo o fluxo, o que facilita a manutenção e modificação.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>


 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

Geração das imagens


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 




## **Passo 12:  Deploy Contínuo com Webhook: **
O ACR é configurado para enviar um webhook para o Azure DevOps, que atualiza automaticamente a versão da API sem causar downtime. Você pode criar um webhook para vincular o Container Registry (ACR) ao seu webapp.

<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 



Para isso, vá até o webapp e obtenha a URL do webhook gerada para você, a qual será usada no ACR.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


No ACR, você precisará configurar o webhook apontando para essa URL para que o container possa ser atualizado automaticamente.


## **Passo 13:  Configuração do ACR e Problema com Imagens: **
Ao tentar configurar o webhook, você percebe que as imagens ainda não estão aparecendo no ACR. Isso ocorre porque, ao criar o ACR, o comando de admin não foi executado, o que impede o acesso correto.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


Para resolver, você deve rodar um comando de atualização que habilite o usuário administrador para o ACR. Isso é importante para garantir que as imagens possam ser empurradas para o registro.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


O comando correto para habilitar o acesso de administrador no Azure Container Registry (ACR) é:

```yaml
az acr update -n <acr-name> --admin-enabled true

```


•	-n <acr-name>: Substitua <acr-name> pelo nome do seu ACR.
•	--admin-enabled true: Habilita a conta de administrador para o ACR.

Este comando vai permitir que você habilite a autenticação de administrador para o ACR, o que pode ser necessário para realizar operações como push/pull de imagens de containers de forma simplificada, usando as credenciais de administrador. Com o webhook configurado corretamente e a autenticação básica ativada, o webapp deve começar a listar as imagens do ACR.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

## **Passo 14:  Habilitação de Autenticação Básica: **
Caso o webapp não tenha autenticação, é necessário habilitar a autenticação básica no webapp para permitir a publicação das imagens.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


Isso é feito nas configurações do webapp, onde você habilita a autenticação básica e salva as configurações. 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


## **Passo 15:  Deploy e Testes: **
O deploy do container para o Web App é realizado, e o instrutor detalha os passos necessários para integrar as imagens no ACR e configurar o deploy contínuo. Após o deploy, você pode testar o funcionamento da API, acessando o endpoint do webapp para verificar se o serviço está rodando como esperado.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>


 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 



## **Passo 16:  Atualização Automática da API: **
O pipeline é configurado no Azure DevOps, utilizando YAML, e é acionado sempre que há uma mudança no código. 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

No caso de mudanças no código (por exemplo, a adição de uma nova funcionalidade como a conversão de temperatura para Kelvin), o pipeline é disparado automaticamente.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


Após o push da imagem para o ACR, o Web App pode puxar automaticamente a nova versão da API e, se configurado corretamente, atualizar a versão sem downtime. 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 

 


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>


 

 


O instrutor mostra a operação da API, que deve funcionar sem downtime. Ele faz ajustes no código e observa as atualizações no pipeline e no Web App, garantindo que a API continue operando durante o processo. Porém, o processo de atualização automática não ocorreu como esperado porque o nome da tag da imagem não foi alterado, o que impediu o Web App de pegar a nova versão automaticamente.


<p align="center">
    <img  src="imagens/imagem001.png" width="1000"/>  
</p>

 


O uso de versões e a criação de novos pacotes ao invés de sobrescrever as versões existentes facilitam o rollback. Isso é útil caso haja problemas com a nova versão, pois o Azure permite retornar facilmente a uma versão anterior.

## Contribuições      

Siga os passos abaixo para contribuir:

1. Faça o *fork* do projeto (<https://github.com/shyoutarou/Microsoft-Azure-AI-Fundamentals.git>)

2. Clone o seu *fork* para sua maquína (`git clone https://github.com/user_name/Microsoft-Azure-AI-Fundamentals.git`)

3. Crie uma *branch* para realizar sua modificação (`git checkout -b feature/name_new_feature`)

4. Adicione suas modificações (`git add *`)

5. Faça o *commit* (`git commit -m "Descreva sua modificação"`)

6. *Push* (`git push origin feature/name_new_feature`)

7. Crie um novo *Pull Request*

8. Pronto, agora só aguardar a análise 

## 📜 License

O projeto publicado em 2025 sobre a licença [MIT](./LICENSE) ❤️ 

Made with ❤️ by Shyoutarou

Gostou? Deixe uma estrelinha para ajudar o projeto ⭐

