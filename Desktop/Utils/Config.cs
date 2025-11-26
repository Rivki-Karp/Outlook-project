using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Utils
{
    public static class Config
    {
        public static string ApiBaseUrl = "https://localhost:7094/api/jobs";
        public static int PollingIntervalMs = 5000; // בדיקה כל 5 שניות
    }
}
