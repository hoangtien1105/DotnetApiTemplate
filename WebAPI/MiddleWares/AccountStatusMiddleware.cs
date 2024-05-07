namespace WebAPI.MiddleWares
{
    public class AccountStatusMiddleware : IMiddleware
    {
        private readonly ILogger<AccountStatusMiddleware> logger;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AccountStatusMiddleware(ILogger<AccountStatusMiddleware> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);
        }
    }
}
