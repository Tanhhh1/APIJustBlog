using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Auth.DTO
{
    public class VerifyOtpDTO
    {
        public string Username { get; set; } = "";
        public string Otp { get; set; } = "";
    }

}
