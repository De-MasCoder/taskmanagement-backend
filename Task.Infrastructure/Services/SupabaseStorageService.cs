using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Services
{
    public class SupabaseStorageService : IFileStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _bucket;
        private readonly string _supabaseUrl;

        public SupabaseStorageService(IConfiguration config)
        {
            _supabaseUrl = config["Supabase:Url"];
            _bucket = config["Supabase:Bucket"];
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"{_supabaseUrl}/storage/v1/")
            };

            _httpClient.DefaultRequestHeaders.Add("apikey", config["Supabase:ApiKey"]);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", config["Supabase:ApiKey"]);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType,CancellationToken cancellationToken, string folder = "")
        {
            var path = $"{folder}/{fileName}".Trim('/');
            var url = $"object/{_bucket}/{path}";

            using var content = new StreamContent(fileStream);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Supabase upload failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");

            return $"{_supabaseUrl}/storage/v1/object/public/{_bucket}/{path}";
        }
    }
}
