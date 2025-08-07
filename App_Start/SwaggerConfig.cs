using System.Web.Http;
using WebActivatorEx;
using GiaimalichsuAPI;
using Swashbuckle.Application;
using System.Configuration;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace GiaimalichsuAPI
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            // Không đăng ký Swagger nếu không được bật
            var enableSwagger = ConfigurationManager.AppSettings["EnableSwagger"];
            if (enableSwagger?.ToLower() != "true")
            {
                return;
            }

            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "GiaimalichsuAPI");

                        // Thêm cấu hình JWT
                        c.ApiKey("Authorization")
                            .Description("Nhập token theo dạng: Bearer {token}")
                            .Name("Authorization")
                            .In("header");

                    })
                .EnableSwaggerUi(c =>
                    {
                        c.EnableApiKeySupport("Authorization", "header");
                    });
        }
    }
}
