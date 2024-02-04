using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Amazon.Runtime.Internal.Transform;
using cruddynamodb.Model;
using cruddynamodb.Service;

namespace cruddynamodb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AWSFrutasController : ControllerBase
    {
        private static ServiceFruta service = new ServiceFruta();

        [Route("criartabela")]
        [HttpGet()]
        public async Task<Response> Get()
        {
            var resp = new Response() 
            {
                mensagem = await service.criarTabela(),
            };
            return resp;
        }

        [HttpGet()]
        public async Task<List<Fruta>> Getitems()
        {
            return await service.Listar();
        }

        [Route("{id}")]
        [HttpGet()]
        public async Task<object> GetitemByid(int id)
        {
            var massa = new Fruta()
            {
                id = id
            };
            return await service.FrutaPoIdAsync(massa);
        }

        [HttpPost()]
        public async Task<Response>  Post(Fruta body)
        {
            var resp = new Response()
            {
                codigo = 201,
                mensagem = await service.Adicionar(body),
            };
            return resp;
        }


        [HttpPut()]
        public async Task<Response> Put(Fruta body)
        {
            var resp = new Response()
            {
                mensagem = await service.Alterar(body),
            };
            return resp;
        }

        [Route("{id}")]
        [HttpDelete()]
        public async Task<Response> Delete(int id)
        {

            var resp = new Response()
            {
                mensagem = (await service.DeleteItemAsync(id)).ToString(),
            };
            resp.mensagem = resp.mensagem.ToLower() == "true" ? "S": "N";
            return resp;
        }



    }
}
