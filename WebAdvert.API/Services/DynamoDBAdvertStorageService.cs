using AdvertAPI.Model;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAdvert.API.Services
{
    public class DynamoDBAdvertStorageService : IAdvertStorageService
    {
        private readonly IMapper _mapper;

        public DynamoDBAdvertStorageService(IMapper mapper)
        {
            _mapper = mapper;
        }

        private AmazonDynamoDBClient InitDynamoDBClient()
        {
            string accessKey = Environment.GetEnvironmentVariable("WebAdvertAwsAccessKey");
            string secretKey = Environment.GetEnvironmentVariable("WebAdvertAwsSecretKey");
            string region = "ap-southeast-1";
            return new AmazonDynamoDBClient(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
        }
        public async Task<string> Add(AdvertModel model)
        {
            var dynamoDbModel = _mapper.Map<AdvertDbModel>(model);
            dynamoDbModel.Id = Guid.NewGuid().ToString();
            dynamoDbModel.CreateDateTime = DateTime.UtcNow;
            dynamoDbModel.Status = AdvertStatus.Pending;
            using (var client = InitDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync(dynamoDbModel);
                }
            }
            return dynamoDbModel.Id;
        }

        public async Task Confirm(ConfirmAdvertModel model)
        {
            using (var client = InitDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    var record = await context.LoadAsync<AdvertDbModel>(model.Id);
                    if (record == null)
                    {
                        throw new KeyNotFoundException($"A record with ID {model.Id} was not found");
                    }
                    if (model.Status == AdvertStatus.Active)
                    {
                        record.Status = AdvertStatus.Active;
                        await context.SaveAsync(record);
                    }
                    else
                    {
                        await context.DeleteAsync(record);
                    }
                }
            }
        }
    }
}
