using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Transform;
using cruddynamodb.Model;
using Microsoft.AspNetCore.Mvc;

namespace cruddynamodb.Service
{
    public class ServiceFruta
    {
        protected AmazonDynamoDBClient client { get; set; }
        private static string nomeTabela = "Frutas";
        public ServiceFruta()
        {
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig();
            config.ServiceURL = "http://dynamodb.sa-east-1.amazonaws.com";
            client = new AmazonDynamoDBClient(config);
        }
        public async Task<List<Fruta>> Listar()
        {
            List<Fruta> lt = new List<Fruta>();
            ScanResponse response;

            response = await client.ScanAsync(nomeTabela, new List<string>());

            // Check the response.
            var responses = response.Items; // Attribute list in the response.

            foreach (var tableResponse in responses)
            {
                var ft = new Fruta()
                {
                    id = int.Parse(tableResponse["id"].N),
                    nome = tableResponse["nome"].S,
                    preco = double.Parse(tableResponse["preco"].N),
                };
                lt.Add(ft);

            }



            return lt;
        }

        public async Task<string> criarTabela()
        {


            var response = await client.CreateTableAsync(new CreateTableRequest
            {

                TableName = nomeTabela,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "id",
                        AttributeType = ScalarAttributeType.N,
                    },
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = "id",
                        KeyType = KeyType.HASH,
                    },
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5,
                },
            });

            // Wait until the table is ACTIVE and then report success.
            Console.Write("Waiting for table to become active...");

            var request = new DescribeTableRequest
            {
                TableName = response.TableDescription.TableName,
            };

            TableStatus status;

            int sleepDuration = 2000;

            do
            {
                System.Threading.Thread.Sleep(sleepDuration);

                var describeTableResponse = await client.DescribeTableAsync(request);
                status = describeTableResponse.Table.TableStatus;

                Console.Write(".");
            }
            while (status != "ACTIVE");

            return (status == TableStatus.ACTIVE).ToString();


        }

        public async Task<string> Adicionar(Fruta body)
        {
            var esiste = await FrutaPoIdAsync(body);
            if (esiste == null)
                return await Alterar(body);
            else
                return "N";
        }


        public async Task<string> Alterar(Fruta body)
        {
            var queryRequest = new PutItemRequest(nomeTabela, new Dictionary<string, AttributeValue>
            {
                new KeyValuePair<string, AttributeValue>("id", new AttributeValue()
                {
                    N =body.id.ToString()
                }),
                new KeyValuePair<string, AttributeValue>("nome", new AttributeValue(body.nome)),
                new KeyValuePair<string, AttributeValue>("preco", new AttributeValue(){
                    N =body.preco.ToString()
                })
            });

            await client.PutItemAsync(queryRequest);
            return "S";
        }

        public async Task<Dictionary<string, AttributeValue>> FrutaPoIdAsync(Fruta fruta)
        {
            var key = new Dictionary<string, AttributeValue>
            {
                ["id"] = new AttributeValue { N = fruta.id.ToString() },
            };

            var request = new GetItemRequest
            {
                Key = key,
                TableName = nomeTabela,
            };

            var response = await client.GetItemAsync(request);
            return response.Item.Count == 0? null : response.Item;
        }


        public  async Task<bool> DeleteItemAsync(
    int FrutaId)
        {
            var key = new Dictionary<string, AttributeValue>
            {
                ["id"] = new AttributeValue { N = FrutaId.ToString() },
            };

            var request = new DeleteItemRequest
            {
                TableName = nomeTabela,
                Key = key,
            };

            var response = await client.DeleteItemAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
