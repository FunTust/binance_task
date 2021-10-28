using System;
using System.Threading.Tasks;

namespace binance_task.Interfaces
{
    public interface IMessageEmailService
    {
        Task SendMessage(string email, string subject, string message);
    }
}
