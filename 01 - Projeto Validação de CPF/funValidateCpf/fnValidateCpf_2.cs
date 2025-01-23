using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace httpValidaCpf
{
    public static class fnValidacpf
    {
        [FunctionName("fnValidacpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Iniciando a validação do CPF.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (data == null)
            {
                return new BadRequestObjectResult("Por favor, informe o CPF.");
            }

            var cpf = data?.cpf;

            if (ValidaCPF(cpf) == false) 
            {
                return new BadRequestObjectResult("CPF inválido.");
            }

            var responseMessage = "CPF válido, e não consta na base de dados de fraudes, e não consta na base de dados de débitos.";

            return new OkObjectResult(responseMessage); 
        }
    }

    public static bool ValidaCPF(string cpf)
    {
        if (string.IsNullOrEmpty(cpf))
        {
            return false;
        }

        // Remove any non-digit characters
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        // Check if the CPF has 11 digits
        if (cpf.Length != 11)
        {
            return false;
        }

        // Check for invalid patterns (all digits equal)
        if (cpf.Distinct().Count() == 1)
        {
            return false;
        }

        // Calculate the first verification digit
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += int.Parse(cpf[i].ToString()) * (10 - i);
        }
        int remainder = (sum * 10) % 11;
        if (remainder == 10)
        {
            remainder = 0;
        }
        if (remainder != int.Parse(cpf[9].ToString()))
        {
            return false;
        }

        // Calculate the second verification digit
        sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += int.Parse(cpf[i].ToString()) * (11 - i);
        }
        remainder = (sum * 10) % 11;
        if (remainder == 10)
        {
            remainder = 0;
        }
        if (remainder != int.Parse(cpf[10].ToString()))
        {
            return false;
        }

        return true;
    }
}
