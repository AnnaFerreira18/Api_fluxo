using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Twilio; 
using Twilio.Rest.Api.V2010.Account; 
using Twilio.Types; 
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Twilio.Types;
using Twilio;

namespace Application.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;

            _accountSid = _configuration["Twilio:AccountSid"];
            _authToken = _configuration["Twilio:AuthToken"];
            _fromPhoneNumber = _configuration["Twilio:FromPhoneNumber"];

            if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken) || string.IsNullOrEmpty(_fromPhoneNumber))
            {

                Console.WriteLine("ERRO: Credenciais do Twilio não configuradas corretamente.");
            }
            else
            {
                TwilioClient.Init(_accountSid, _authToken);
            }
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            if (string.IsNullOrEmpty(_accountSid))
            {
                Console.WriteLine($"Não foi possível enviar SMS para {toPhoneNumber}: Credenciais Twilio em falta.");
                return; 
            }

            try
            {
                // O número 'to' DEVE estar no formato 
                var messageOptions = new CreateMessageOptions(new PhoneNumber(toPhoneNumber))
                {
                    From = new PhoneNumber(_fromPhoneNumber),
                    Body = message
                };

                // Enviar a mensagem usando o SDK
                var messageResponse = await MessageResource.CreateAsync(messageOptions);

                Console.WriteLine($"SMS enviado para {toPhoneNumber}. SID: {messageResponse.Sid}, Status: {messageResponse.Status}");

                if (messageResponse.Status == MessageResource.StatusEnum.Failed ||
                    messageResponse.Status == MessageResource.StatusEnum.Undelivered)
                {
                    Console.WriteLine($"Falha ao enviar SMS para {toPhoneNumber}. Erro: {messageResponse.ErrorMessage} (Code: {messageResponse.ErrorCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar SMS para {toPhoneNumber}: {ex.Message}");
            }
        }
    }
}
