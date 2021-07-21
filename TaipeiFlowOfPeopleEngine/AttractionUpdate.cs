using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaipeiFlowOfPeopleModel;

namespace TaipeiFlowOfPeopleEngine
{
    public class AttractionUpdate : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration config;

        public AttractionUpdate(ILogger<Worker> logger, IConfiguration  configuration)
        {
            _logger = logger;
            config = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
                using(var cn =new TaipeiFlowOfPeopleContext(config))
                {
                    cn.RefreshTable<Attraction>();
                    _logger.LogInformation($"Refresh {nameof(Attraction)} at: {DateTimeOffset.Now}");
                }
                await Task.Delay(900000, stoppingToken); // 15 分鐘更新一次
            }
        }

        private string GetJsonContent(string Url)
        {
            string targetURI = Url;
            var request = System.Net.WebRequest.Create(targetURI);
            request.ContentType = "application/json; charset=utf-8";
            var response = request.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            return text;
        }
    }
}
