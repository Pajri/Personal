using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Personal.Utils
{
    public class ConfigUtils
    {
        public static class Path
        {
            public static readonly string FORGOTPASSWORD_FROM_ADDRESS = "PersonalConfig:Authentication:ForgotPassword:FromAddress";
            public static readonly string FORGOTPASSWORD_SUBJECT = "PersonalConfig:Authentication:ForgotPassword:Subject";
            public static readonly string FORGOTPASSWORD_BODY = "PersonalConfig:Authentication:ForgotPassword:Body";

            public static readonly string SMTP_PORT = "PersonalConfig:SMTP:Port";
            public static readonly string SMTP_HOST = "PersonalConfig:SMTP:Host";
            public static readonly string SMTP_USERNAME = "PersonalConfig:SMTP:Username";
            public static readonly string SMTP_PASSWORD = "PersonalConfig:SMTP:Password";
        }

    }
}
