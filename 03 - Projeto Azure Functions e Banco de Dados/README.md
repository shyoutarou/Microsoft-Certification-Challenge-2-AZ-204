# Criando um Gerenciador de Catálogos da Netflix com Azure Functions e Banco de Dados

## DESCRIÇÃO
Projeto de criação de uma série de Azure Functions para gerenciar catálogos, similar ao da Netflix. O foco é integrar serviços como Redis, banco de dados SQL e Storage Account, visando uma arquitetura escalável.  O objetivo é garantir que o sistema tenha uma resposta mais rápida e possa lidar com um grande número de usuários.
Ele detalha o uso de tecnologias como o Streamlit para frontend, API Gateway para gerenciar requisições e Azure Functions para se comunicar com o banco de dados e Storage Account. Um ponto importante mencionado é a utilização de cache (como o Redis) para melhorar a escalabilidade do sistema, já que o banco de dados pode não escalar tão rapidamente quanto o resto da infraestrutura. 

 

## Instrutor
**Henrique Eduardo Souza**  
Microsoft MVP, Microsoft  
[LinkedIn](https://www.linkedin.com/in/hsouzaeduardo/?locale=pt_BR)


## Ferramentas Necessárias

Tecnologias como o Streamlit para frontend, API Gateway para gerenciar requisições e Azure Functions para se comunicar com o banco de dados e Storage Account. Um ponto importante mencionado é a utilização de cache (como o Redis) para melhorar a escalabilidade do sistema, já que o banco de dados pode não escalar tão rapidamente quanto o resto da infraestrutura. 

1. Streamlit (para Frontend): Streamlit é uma ferramenta popular para criar interfaces de usuário interativas e rápidas com Python.

https://streamlit.io/


2. Azure API Management (para gerenciar requisições via API Gateway): Azure API Management permite criar, gerenciar e proteger APIs, funcionando como um gateway.

https://learn.microsoft.com/pt-br/azure/api-management/

3. Azure Functions (para comunicação com banco de dados e Storage Account): Azure Functions permite a execução de código sem gerenciar infraestrutura, ideal para conectar-se a outros serviços como banco de dados e Storage.

https://learn.microsoft.com/pt-br/azure/azure-functions/


4. Redis (para Cache): Redis é um sistema de gerenciamento de dados em memória utilizado como cache para melhorar a performance de aplicações.

https://redis.io/

5. Banco de Dados SQL (para armazenar dados): O Azure SQL Database é um serviço gerenciado de banco de dados que pode ser usado para armazenar e consultar dados de maneira escalável.
Azure SQL Database


## Passo 1:  Criação de recursos na Azure:

Inicia a criação de um Resource Group chamado "Flix", onde serão agrupados os recursos necessários.
 


Criação de um API Management para gerenciar as APIs da aplicação, configurando o nome da organização e o e-mail. Opta por uma camada de consumo mais barata para o API Management.

 


## Passo 2:  Escolha de Banco de Dados:
Inicialmente, planeja-se usar um banco SQL, mas decide-se por usar o Cosmos DB, um banco de dados não relacional, por ser mais adequado ao projeto.
 

 

## Passo 3:  Criação de outros recursos:
Criação de um Storage Account para armazenar arquivos como vídeos e imagens (thumbnails) com configurações de redundância e proteção de dados.
 

Durante o processo, é importante habilitar o acesso anônimo aos containers de armazenamento para facilitar o upload e o download dos arquivos.
 

## Passo 4:  Fluxo de Upload:
O fluxo de upload inclui o envio de vídeos e thumbnails para o Storage Account e o registro dessas informações no Cosmos DB, criando um "registro" para cada vídeo com título, descrição, tipo de conteúdo, etc.
 


## Passo 5:  Criação de uma Azure Function:
Durante o desenvolvimento, serão criadas e publicadas várias Azure Functions para lidar com diferentes tarefas, como:
  - Upload de vídeos e thumbnails.
  - Criação de registros no Cosmos DB com informações sobre os vídeos.
  - Recuperação de vídeos e seus detalhes.
 
fnPostDataStorage: Esta função é responsável por armazenar arquivos (imagens) no Azure Blob Storage.
  - Endpoint: POST /api/dataStorage
  - Descrição: Recebe uma requisição HTTP com um arquivo e o armazena no Azure Blob Storage.
  - Arquivo: FunctionDataStorage.cs


As funções criadas interagem com o Cosmos DB e o Storage Account para realizar as operações de upload, persistência de dados e recuperação de informações. O primeiro passo é criar uma Azure Function utilizando o template HTTP Trigger, para que a função aceite chamadas HTTP POST.
 

 

Para enviar arquivos (como imagens ou vídeos), é necessário configurar a função para receber essas informações através de um formulário (form data). A função recebe os arquivos e realiza validações como verificar o tipo de arquivo (imagem ou vídeo).




 



## Passo 7:  Configuração de limites de tamanho de arquivo:
Por padrão, uma função do Azure pode ter limites para o tamanho de arquivos recebidos. 
 

No caso de grandes arquivos (como vídeos), é necessário aumentar esses limites configurando a função para aceitar tamanhos maiores (como 100MB).
 

 


## Passo 8:  Configuração do Azure Storage: cria containers públicos no Azure para armazenar vídeos e imagens. 
 

 

Ele também obtém a connection string para conectar sua aplicação ao armazenamento.
 

 

## Passo 9:  Criação de Função Assíncrona: Ele configura uma função assíncrona para processar os arquivos recebidos. A função valida os arquivos, verifica se são válidos e de acordo com o tipo (imagem ou vídeo).


```yaml
namespace fnPostDataStorage
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("dataStorage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processando a imagem no Storage");

                
        if (!req.Heagers.TryGetValue("file-type", out var fileTypeHeader))
        {
            return new BadRequestObjectResult("O cabeçalho 'file-type' é obrigatório");
        }

        var fileType = fileTypeHeader.ToString();
        var form = await req.ReadFormAsync();
        var file = form.Files["file"];

        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("O arquivo não foi enviado");
        }

        string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        string containerName = fileType;
        BlobClient blobClient = new BlobClient(connectionString, containerName, file.FileName);
        BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
                
        string blobName = file.FileName;
        var blob = containerClient.GetBlobClient(blobName);

        using (var stream = file.OpenReadStream())
        {
            await blob.UploadAsync(stream, true);
        }

        _logger.LogInformation($"Arquivo {file.FileName} armazenada com sucesso no container {containerName}");

        return new OkObjectResult(new
            {
            Message = "Arquivo armazenado com sucesso",
            fileUrl = blob.Uri
        });
        }
    }
}
```

Upload para o Azure Storage: Após validar o arquivo recebido, o código utiliza a SDK do Azure para criar um cliente de Blob Storage e fazer o upload do arquivo para o container correto no Azure Storage.
 



## Passo 11:  Armazenamento no Azure Blob Storage:
O arquivo é armazenado e a URL do blob é retornada, indicando que o arquivo foi carregado com sucesso.
 


 


## Passo 12:  criar uma Azure Function chamada fnPostDatabase, responsável por salvar registros no Cosmos DB. 


O processo começa com a criação de um novo projeto, onde ele configura a conexão com o Cosmos DB no arquivo de hosts, incluindo a string de conexão, o nome do banco de dados e o nome do contêiner.
 

 


fnPostDatabase: Esta função é responsável por salvar os dados de um filme no Cosmos DB.
  - Endpoint: POST /api/movie
  - Descrição: Recebe uma requisição HTTP com os dados do filme em formato JSON e os salva no Cosmos DB.
  - Arquivo: FunctionMovieDatabase.cs


## Passo 13:  configurações no local.settings.json durante o processo de criação da Azure Function. 
No arquivo de hosts será necessário configurar a Cosmos DB connection string, o nome do banco de dados e o nome do contêiner. Especificamente, ele coloca a connection string do Cosmos DB no arquivo local.settings.json para facilitar a conexão com o banco.

Essas configurações são essenciais para que a função consiga se comunicar corretamente com o Cosmos DB e fazer o post dos dados. Essas configurações não são detalhadas em termos de código exato, mas a principal informação que ele passa é que é necessário ter essas informações de configuração no arquivo para garantir que a função funcione adequadamente.

 


## Passo 14: instalar pacotes como o Newtonsoft.Json para manipulação de JSON e o Microsoft.Azure.Functions.Extensions.CosmosDB para integrar o Cosmos DB. A função será configurada para pegar dados de um filme (como título, ano, thumbnail) e salvar no banco.
 


## Passo 14: Ao codificar a função, criar um objeto MovieRequest, serializa os dados para JSON e insere no Cosmos DB. 

```yaml
namespace fnPostDatabase
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("movie")]
        [CosmosDBOutput("%DatabaseName%", "movies", Connection = "CosmosDBConnection", CreateIfNotExists = true, PartitionKey = "id")]
        public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            MovieRequest movie = null;
           
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                movie = JsonConvert.DeserializeObject<MovieRequest>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing request body" + ex.Message);
            }

            return JsonConvert.SerializeObject(movie);
        }
    }
}

```


## Passo 15: confirmar que a função está funcionando, 
Após ajustes na configuração de partição do banco e testes, a função consegue salvar filmes no banco, retornando o ID gerado e o título dos filmes corretamente, testando com dois filmes diferentes e mostrando que os dados são corretamente gravados no Cosmos DB.
 

 

 

 



## Passo 16:  Instalação de Pacotes Necessários:

Pacotes como CosmosClient e NewtonSoft.Json foram instalados para facilitar a interação com o Cosmos DB e o processamento de dados JSON.

 


## Passo 17:  Configuração da conexão: Foi configurada a string de conexão com o Cosmos DB e o nome do banco e container.

 


## Passo 18:  Criação de CosmosClient: configurar o CosmosClient como um singleton, garantindo que a instância do cliente seja reutilizada para otimizar o uso de recursos.

 


## Passo 19:  Funções para obter dados: Duas funções principais estão sendo discutidas:
  - getMovieDetail: Esta função tem como objetivo pegar detalhes de um filme específico, dado o seu ID.
  - getMovieDetail: Esta função tem como objetivo pegar detalhes de um filme específico, dado o seu ID.
  - getAllMovies: Função para listar todos os filmes, embora o foco inicial esteja no getMovieDetail.
  - getAllMovies: Função para listar todos os filmes, embora o foco inicial esteja no getMovieDetail.

fnGetMovieDetail
Esta função é responsável por recuperar os detalhes de um filme armazenado no Cosmos DB.
  - Endpoint: GET /api/detail
  - Descrição: Recebe uma requisição HTTP com o ID do filme e retorna os detalhes do filme armazenado no Cosmos DB.
  - Arquivo: FunctionMovieDetail.cs


 
```yaml

namespace fnGetMovieDetail
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly CosmosClient _cosmosClient;

        public Function1(ILogger<Function1> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("detail")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var container = _cosmosClient.GetContainer("DioFlixDB", "movies");
            var id = req.Query["id"];
            var query = new QueryDefinition($"SELECT * FROM c WHERE c.id = '{id}'");
            var result = container.GetItemQueryIterator<MovieResult>(query);
            var results = new List<MovieResult>();

            while (result.HasMoreResults)
            {
                foreach (var item in await result.ReadNextAsync())
                {
                   results.Add(item);
                }
            }
            var responseMessage = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await responseMessage.WriteAsJsonAsync(results.FirstOrDefault());

            return responseMessage;
        }
    }
}

```

## Passo 20:  Consulta ao Cosmos DB: Para a função getMovieDetail, o autor utiliza uma QueryDefinition com um parâmetro (ID do filme) para buscar um filme específico. A consulta é feita através de um query iterator que permite percorrer os resultados da consulta.

 


Respostas HTTP: O código está sendo ajustado para retornar respostas HTTP com o status correto (200 OK) e os dados do filme consultado.

 


## Passo 21:  Criação da Função getAllMovies:
Foi criada uma função chamada getAllMovies utilizando um gatilho HTTP (HTP Trigger).
 

 

A função foi configurada para se conectar ao banco de dados Cosmos DB e recuperar todos os filmes registrados, sem parâmetros de entrada. O código foi copiado de uma função anterior, com ajustes para a função getAllMovies, que itera sobre os resultados do banco de dados e os retorna como resposta HTTP.
 


Durante os testes, o desenvolvedor verificou que havia alguns problemas de configuração no endereço de API e nos parâmetros passados. Ajustes foram feitos para corrigir os endereços e o formato de alguns dados (por exemplo, para garantir que os vídeos fossem carregados corretamente).


## Passo 22:  Uso de Streamlit para Frontend:
Para o frontend, será usado o Streamlit, uma ferramenta Python para construir interfaces de usuário rapidamente. O Streamlit ajudará a criar uma interface simples para exibir os vídeos e detalhes. Foi criado um projeto HTML para consumir as APIs. No HTML, são exibidos cards com os filmes e detalhes de cada um.
 

 



