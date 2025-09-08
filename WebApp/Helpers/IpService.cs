namespace WebApp.Helpers
{
    public class IpService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserIp()
        {
            //var testingIp = "84.16.242.160"; Localhost Test için yurt dışı ip ekleyebilirsin
            var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            // Localhost (::1 veya 127.0.0.1) ise null döndür
            if (string.IsNullOrEmpty(ip) || ip == "::1" || ip == "127.0.0.1")
                return null;
                //return testingIp;

            return ip;
        }
    }

}
