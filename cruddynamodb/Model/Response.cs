namespace cruddynamodb.Model
{
    public class Response
    {
        public Response() 
        {
            codigo = 200;
            mensagem = "OK";
        }

        public int codigo { get; set; }
        public string mensagem { get; set; }
    }
}
