using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace cruddynamodb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AWSPingController : Controller
    {
        //lista as tabelas para vert se a conexao está ok
        [HttpGet()]
        public async Task<string> Get()
        {
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig();
            config.ServiceURL = "http://dynamodb.sa-east-1.amazonaws.com";
            var dynamoDbClient = new AmazonDynamoDBClient(config);
            Console.WriteLine($"Hello Amazon Dynamo DB! Following are some of your tables:");
            Console.WriteLine();

            var response = await dynamoDbClient.ListTablesAsync(
                new ListTablesRequest()
                {
                    Limit = 5
                });
            StringBuilder sb = new StringBuilder();
            foreach (var table in response.TableNames)
            {
                sb.AppendLine($"\tTable: {table}");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
