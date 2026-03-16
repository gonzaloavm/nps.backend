using Application.Contracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class DeviceInfoService : IDeviceInfoService
    {
        private readonly IHttpContextAccessor _accessor;

        public DeviceInfoService(IHttpContextAccessor accessor) => _accessor = accessor;

        public string GetIpAddress() => _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";

        public string GetDeviceName()
        {
            var userAgent = _accessor.HttpContext?.Request.Headers["User-Agent"].ToString();

            var os = "Desconocido";
            var browser = "Navegador";

            if (userAgent == null)
                return $"{browser} en {os}";

            // Detección simple de OS
            if (userAgent.Contains("Windows")) os = "Windows";
            else if (userAgent.Contains("Android")) os = "Android";
            else if (userAgent.Contains("iPhone")) os = "iPhone/iOS";
            else if (userAgent.Contains("Macintosh")) os = "macOS";
            else if (userAgent.Contains("Linux")) os = "Linux";

            // Detección simple de Navegador
            if (userAgent.Contains("Edg/")) browser = "Edge";
            else if (userAgent.Contains("Chrome")) browser = "Chrome";
            else if (userAgent.Contains("Firefox")) browser = "Firefox";
            else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) browser = "Safari";

            return $"{browser} en {os}";
        }
    }
}
