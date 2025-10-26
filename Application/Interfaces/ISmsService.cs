using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISmsService
    {
        // Envia um SMS. 'toPhoneNumber' deve estar no formato E.164 (+5561999999999)
        Task SendSmsAsync(string toPhoneNumber, string message);
    }
}
