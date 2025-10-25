using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Application.Interfaces;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Ler as configurações
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderName = _configuration["EmailSettings:SenderName"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"]; // Senha de App

            // Validar configurações
            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
            {
                Console.WriteLine("ERRO: Configurações de email incompletas no appsettings.json.");
                // Poderia lançar uma exceção aqui para impedir a continuação
                return; // Ou apenas logar e não enviar
            }

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(toEmail));

            // --- CONFIGURAÇÃO PARA GMAIL (OU SMTP REAL) ---
            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true; // Gmail requer SSL/TLS
                client.UseDefaultCredentials = false; // Precisamos de fornecer credenciais
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword); // Usa o Username e a Senha de App
                client.DeliveryMethod = SmtpDeliveryMethod.Network; // Método de envio padrão
                                                                    // Para alguns provedores, pode ser necessário configurar Timeouts, etc.
                                                                    // client.Timeout = 20000; // Ex: 20 segundos
                                                                    // --- FIM DA CONFIGURAÇÃO ---

                try
                {
                    await client.SendMailAsync(message);
                    Console.WriteLine($"Email REAL enviado com sucesso para {toEmail}");
                }
                catch (SmtpException smtpEx) // Captura erros específicos de SMTP
                {
                    Console.WriteLine($"Erro SMTP ao enviar email para {toEmail}: {smtpEx.StatusCode} - {smtpEx.Message}");
                    // Log detalhado do erro
                    // throw; // Relançar pode ser apropriado dependendo da sua estratégia de erro
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro GERAL ao enviar email para {toEmail}: {ex.Message}");
                    // throw;
                }
            }
        }
    }
}
