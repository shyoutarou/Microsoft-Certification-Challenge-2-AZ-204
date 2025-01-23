using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Linq;

namespace httpValidateCpf
{
    public static class fnValidateCpf
    {
        [FunctionName("fnValidateCpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Starting validation CPF.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (data is null)
            {
                return new BadRequestObjectResult("Please, inform your Cpf.");
            }

            string cpf = data?.cpf;

            if(ValidateCpf(cpf) == false){
                return new BadRequestObjectResult("Cpf is invalid.");
            }

            return new OkObjectResult("Cpf is valid.");
        }

        public static bool ValidateCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return false;
            }

            // Remove non-numeric characters
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            // Validate length
            if (cpf.Length != 11) return false;

            // Check for invalid CPFs
            string[] invalidCpfs = new string[]
            {
            "00000000000", "11111111111", "22222222222", "33333333333",
            "44444444444", "55555555555", "66666666666", "77777777777",
            "88888888888", "99999999999"
            };
            if (invalidCpfs.Contains(cpf)) return false;

            // Calculate verification digits
            int[] multipliers1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multipliers2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multipliers1[i];

            int remainder = sum % 11;
            if (remainder < 2) remainder = 0;
            else remainder = 11 - remainder;

            string digit = remainder.ToString();
            tempCpf += digit;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multipliers2[i];

            remainder = sum % 11;
            if (remainder < 2) remainder = 0;
            else remainder = 11 - remainder;

            digit += remainder.ToString();

            // Validate CPF
            return cpf.EndsWith(digit);
        }
    }
}
