using System;
using System.Collections.Generic;
using System.Text;

namespace No2API.Entities.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string LogLevel { get; set; }
        public string CategoryName { get; set; }
        public string Msg { get; set; }
        public string User { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
