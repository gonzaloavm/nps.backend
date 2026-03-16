using Application.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Infrastructure.Services
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeoLocationService> _logger;

        public GeoLocationService(HttpClient httpClient, ILogger<GeoLocationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetLocationAsync(string ip, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(ip) || ip == "::1" || ip == "127.0.0.1" || ip == "Desconocida")
            {
                return "Local / Desarrollo";
            }

            try
            {
                // ip-api devuelve un JSON con city y country
                var response = await _httpClient.GetFromJsonAsync<IpApiResponse>($"http://ip-api.com/json/{ip}", ct);

                if (response?.Status == "success")
                {
                    return $"{response.City}, {response.Country}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo ubicación para IP {Ip}", ip);
            }

            return "Ubicación Desconocida";
        }

        private record IpApiResponse(string Status, string City, string Country);
    }
}
