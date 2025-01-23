# Criando um Microsserviço Serverless para Validação de CPF

## DESCRIÇÃO
Este projeto tem como objetivo desenvolver um microsserviço eficiente, escalável e econômico para validação de CPFs, utilizando arquitetura serverless. A aplicação será construída com base em princípios modernos de computação em nuvem, garantindo alta disponibilidade, baixo custo operacional e facilidade de manutenção.

## Instrutor
**Henrique Eduardo Souza**  
Microsoft MVP, Microsoft  
[LinkedIn](https://www.linkedin.com/in/hsouzaeduardo/?locale=pt_BR)

## Ferramentas Necessárias

- **Visual Studio Code ou Visual Studio 22**: Utilizado para criar a API base, que será publicada no Azure.  
  [Visual Studio Code](https://code.visualstudio.com/)  
  [Visual Studio](https://visualstudio.microsoft.com/pt-br/)
-  [**DotNet 8 SDK**](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
-  [**Azure CLI**](https://learn.microsoft.com/pt-br/cli/azure/install-azure-cli)
-  [**Azure Function Core Tools**](https://learn.microsoft.com/pt-br/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp)



## Passo 1: Criar um Novo Recurso no Azure
1.	Acesse o portal do Azure:
2.	No menu lateral esquerdo, clique em “Create a resource” e procure por “Function App”.
3.	Clique em “Create” e preencha os seguintes detalhes:
  - Subscription: Selecione sua assinatura do Azure.
  - Resource Group: Crie um novo grupo de recursos ou selecione um existente.
  - Function App name: Escolha um nome único para sua Function App.
  - Publish: Selecione “Code”.
  - Runtime stack: Selecione ".NET" para um projeto C#..
  - Version: Escolha a versão do .NET que deseja usar. Por exemplo, .NET 8 (ou outra versão disponível).
  - Region: Selecione a região mais próxima de você.
4.	Clique em “Next: Hosting” e configure o armazenamento:
  - Storage Account: Crie uma nova conta de armazenamento ou selecione uma existente.
5.	Clique em “Next: Monitoring” e desative o Application Insights se não for necessário.
6.	Clique em “Review + create” e, em seguida, “Create” para finalizar a criação do recurso.


O arquivo host.json é utilizado quando a Azure Function é configurada e implantada, mas ele não é um arquivo de código que você altera durante a execução da função em si. Ele serve para configurar comportamentos globais do ambiente de execução da Azure Function, como, por exemplo:
  - Versão do runtime.
  - Configurações de logging.
  - Definir configurações de escalabilidade.
  - Entre outras configurações específicas da execução da função.

```yaml
host.json
{
    "version": "2.0",
    "logging": {
        "applicationInsights": {
            "samplingSettings": {
                "isEnabled": true,
                "excludedTypes": "Request"
            },
            "enableLiveMetricsFilters": true
        }
    }
}
```

## Passo 2: Configurando o Projeto Azure Functions Localmente

Os comandos a seguir ajudam a criar e configurar rapidamente um novo projeto de Azure Functions baseado no runtime .NET, com um template adequado para começar a desenvolver funções para a plataforma Azure. Abra o terminal e execute os seguintes comandos:
  - Preparar o ambiente de desenvolvimento com Visual Studio Code, Azure CLI, e Azure Function Core Tools.
  - Criar uma Azure Function usando a extensão Azure Functions no VS Code e como configurar o ambiente de execução (.NET 8).

dotnet new -i Microsoft.Azure.Functions.ProjectTemplate

  - Este comando instala o template de projeto para Azure Functions no seu ambiente de desenvolvimento local.
  - O parâmetro -i é utilizado para instalar o template que será utilizado na criação de novos projetos de Azure Functions.
  - O template Microsoft.Azure.Functions.ProjectTemplate contém as configurações iniciais necessárias para iniciar um projeto de Azure Functions.

dotnet new func --name ValidadorCpfFunction --worker-runtime dotnet

  - Este comando cria um novo projeto de Azure Functions utilizando o template instalado anteriormente.
  - O parâmetro --name define o nome do projeto ou da função. Neste caso, o nome da função será ValidadorCpfFunction.
  - O parâmetro --worker-runtime dotnet especifica que o projeto será baseado no runtime .NET para Azure Functions. Ou seja, a função será escrita em C# ou outras linguagens que usem o ambiente de execução .NET.

cd ValidadorCpfFunction

  - Este comando altera o diretório atual para a pasta do projeto recém-criado.
  - O nome ValidadorCpfFunction é o nome do projeto, e após executar esse comando, você estará no diretório onde o código da função foi gerado e onde poderá editar e configurar o projeto conforme necessário.



## Passo 3: Código para a função de validação de CPF:
O código inicializa uma função HTTP para validar o CPF, utilizando parâmetros no corpo da requisição.
•	Criar uma função que verifica se o CPF é válido, implementando a lógica de validação e retornando respostas apropriadas para CPF válido ou inválido.

```yaml
using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public static class Function1
{
    [FunctionName("ValidadorCpf")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = new StreamReader(req.Body).ReadToEnd();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        string cpf = data?.cpf;

        if (string.IsNullOrEmpty(cpf) || cpf.Length != 11 || !Regex.IsMatch(cpf, @"^\d{11}$"))
        {
            return new BadRequestObjectResult("CPF inválido");
        }

        int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCpf, digito;
        int soma, resto;

        tempCpf = cpf.Substring(0, 9);
        soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = digito + resto.ToString();
        bool isValid = cpf.EndsWith(digito);

        return new OkObjectResult(isValid ? "CPF Válido" : "CPF Inválido");
    }
}
```

A função é testada localmente usando o Postman, para corrigir alguns erros de implementação, como a forma de passar o CPF na requisição.



## Passo 4: Publicar a função

Para publicar (ou implantar) uma Azure Function no Azure, diretamente de um ambiente de desenvolvimento local.  
  - Publicar a function na Azure usando Web Deploy. 
  - Configuração do runtime para garantir que o código seja executado corretamente na nuvem. 
Execute os seguintes comandos no terminal para fazer o deploy do projeto:

az login
az account set --subscription "<seu-id-de-assinatura>"
func azure functionapp publish ValidadorCpf

  - func: Esse é o comando principal da Azure Functions Core Tools, que fornece várias funcionalidades para trabalhar com Azure Functions localmente e na nuvem.
  - azure: Indica que a operação será realizada no contexto do Azure.
  - functionapp: Refere-se ao serviço do Azure Functions que irá hospedar a função.
  - publish: Esse subcomando é utilizado para publicar a função para a nuvem, ou seja, enviar o código do seu projeto para o Azure, tornando a função disponível e executável na plataforma.
  - ValidadorCpf: Esse é o nome do Function App no Azure. A função será publicada para esse Function App específico.

Esse comando é bastante útil no processo de CI/CD ou no desenvolvimento local de funções que precisam ser disponibilizadas na nuvem rapidamente.
  - O Function App com o nome ValidadorCpf deve já existir no Azure ou você precisará criá-lo antes de publicar.
  - O usuário precisa estar autenticado no Azure através da CLI ou da ferramenta, e o ambiente deve estar configurado corretamente para permitir a publicação.
  - O Azure Functions Core Tools realiza o processo de implantação da sua aplicação de função para o Azure.
  - A ferramenta compila o código da função e o envia para o Function App no Azure, criando a infraestrutura necessária para que a função seja executada na nuvem.
  - Após a publicação, a função estará disponível no Azure Functions para ser executada, conforme os triggers definidos (como HTTP, timer, filas, etc.).


## Passo 5: Use a URL gerada para fazer requisições.

Após a publicação, configurar uma autenticação básica para proteger a função na nuvem, usando uma chave de acesso. Ao final, a function está funcionando corretamente, validando CPFs com sucesso tanto localmente quanto na Azure.


Exemplo de Resposta
```yaml
Requisição
{
  "cpf": "12345678909"
}

Resposta
{
  "cpf": "12345678909",
  "is_valid": true
}
```

## Passo 6: Limpar recursos
Excluir o grupo de recursos e todos os recursos contidos nele para evitar custos adicionais:

az group delete --name "<seu- functions-group >"


## Contribuição  
Sinta-se à vontade para contribuir com melhorias neste projeto. Envie um pull request ou abra uma issue para discussão.


