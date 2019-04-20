namespace Data
{
    using Data.Models;
    using Microsoft.AspNet.OData.Batch;
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.AspNet.OData.Extensions;
    using Microsoft.OData.Edm;
    using System;
    using System.Linq;
    using System.Web.Http;

    public static class WebApiConfig
    {
        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Group>("Groups");
            builder.EntitySet<Data>("Datas");
            builder.EntitySet<DataDecimal>("DataDecimals");
            builder.EntitySet<LimitDecimalDenormalized>("LimitDecimalDenormalizeds");
            builder.EntitySet<LimitStringDenormalized>("LimitStringDenormalizeds");
            builder
                .EntityType<Data>()
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
                .EntityParameter<Controllers.GroupNameTree>("NewGroups");
            groupBulkInsertByName
                .CollectionParameter<string>("RootPath");

            return builder.GetEdmModel();
        }

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors(new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*") { SupportsCredentials = true });

            //// Web API routes
            //config.MapHttpAttributeRoutes();

            var odataBatchHandler = new UnbufferedODataBatchHandler(GlobalConfiguration.DefaultServer);
            odataBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 1_000_000;
            odataBatchHandler.MessageQuotas.MaxPartsPerBatch = 1_000_000;
            odataBatchHandler.MessageQuotas.MaxReceivedMessageSize = Int64.MaxValue;

            config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();
            config.MapODataServiceRoute(
                routeName: "ODataRouteV4",
                routePrefix: "odata/v4",
                  batchHandler: odataBatchHandler,
                  model: GetEdmModel());
        }
    }
}
