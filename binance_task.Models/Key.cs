using Microsoft.EntityFrameworkCore;
using System;

namespace binance_task.Models
{
    public class Keys
    {
        public int Id { get; set; }
        public String API_KEY { get; set; }
        public String SECRET_KEY { get; set; }
    }
}
