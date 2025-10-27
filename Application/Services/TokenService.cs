using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration; 
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(Usuario usuario)
        {
            // O "Handler" é o objeto que de fato cria o token
            var tokenHandler = new JwtSecurityTokenHandler();

            // A chave secreta (lida do appsettings.json)
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("Chave JWT ('Jwt:Key') não foi configurada.");
            }
            var key = Encoding.ASCII.GetBytes(jwtKey);

            // O "Payload" do token. São os dados que queremos guardar dentro dele.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()), // 'Subject' (o ID do usuário)
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Name, usuario.Nome)
            };

            // A "Receita" do token: quem emite, quem recebe, os dados,
            // a data de expiração e como ele será assinado.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.Now.AddHours(8), // Duração do token
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), // Usa a nossa chave secreta
                    SecurityAlgorithms.HmacSha256Signature // Algoritmo de assinatura
                )
            };

            // Criar e escrever o token como uma string
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GerarTokenRedefinicaoSenha(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"]; // Usamos a mesma chave secreta
            if (string.IsNullOrEmpty(jwtKey)) throw new InvalidOperationException("Chave JWT não configurada.");
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                // Claim especial para identificar o propósito deste token
                new Claim("purpose", "password_reset")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.Now.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
