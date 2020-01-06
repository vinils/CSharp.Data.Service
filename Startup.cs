namespace CSharp.Data.Service
{
    using global::Data.Models;
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.AspNet.OData.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OData.Edm;
    using System;
    using System.Linq;

    public class Startup
    {
        private static IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);
            //builder.EnableLowerCamelCase();

            builder.EntitySet<Group>("Groups");
            builder.EntitySet<Data>("Datas");
            builder.EntitySet<DataDecimal>("DataDecimals");

            builder.EntitySet<LimitDecimalDenormalized>("LimitDecimalDenormalizeds")
                .EntityType
                .HasKey(e => new { e.GroupId, e.CollectionDate });

            builder
                .EntitySet<LimitStringDenormalized>("LimitStringDenormalizeds")
                .EntityType
                .HasKey(e => new { e.GroupId, e.CollectionDate });

            builder
                .EntityType<Data>()
                .HasKey(e => new { e.GroupId, e.CollectionDate })
                .Collection
                .Action("BulkInsert")
                .ReturnsCollectionFromEntitySet<Data>("Datas")
                .CollectionEntityParameter<Data>("Datas");
            
            var groupBulkInsertByName = builder
                .EntityType<Group>()
                .Collection
                .Action("BulkInsertByName")
                .ReturnsCollectionFromEntitySet<Group>("Groups");
            groupBulkInsertByName
                .EntityParameter<global::Data.Controllers.GroupNameTree>("NewGroups");
            groupBulkInsertByName
                .CollectionParameter<string>("RootPath");

            builder.EntityType<DataDecimal>()
                .Collection
                .Function("DecimalTotal")
                .Returns<string>();

            builder.EntityType<Group>()
                .Collection
                .Function("ChildsRecursively")
                //.ReturnsFromEntitySet<Group>("Group");
                .Returns<string>();

            return builder.GetEdmModel();
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllers();
            services.AddOData();
            //services.AddODataQueryFilter();

            services.AddDbContext<DataContext>(options 
                => options.UseSqlServer(
                    Configuration.GetConnectionString("DataConn"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(int.MaxValue)));
            
            services.AddMvc(option => {
                option.MaxIAsyncEnumerableBufferLimit = int.MaxValue;
                option.EnableEndpointRouting = false;
            });
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //services.AddMvc().AddJsonOptions(opt =>
            //{
            //    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //IAssemblyProvider provider = app.ApplicationServices.GetRequiredService<IAssemblyProvider>();
            //IEdmModel model = GetEdmModel(provider);

            app.UseMvc(o =>
            {
                o.Filter().Expand().Count().Expand().OrderBy().Select().MaxTop(int.MaxValue);
                o.MapODataServiceRoute("ODataRoutes", "odata", GetEdmModel(app.ApplicationServices));
            });

            app.UseWebSockets();
            app.UseHttpsRedirection();

            //app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }

    }
}
