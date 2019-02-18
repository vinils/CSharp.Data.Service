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
            builder.EntitySet<Exam>("Exams");
            builder.EntitySet<ExamDecimal>("ExamDecimals");
            builder.EntitySet<LimitDecimalDenormalized>("LimitDecimalDenormalizeds");
            builder.EntitySet<LimitStringDenormalized>("LimitStringDenormalizeds");
            builder
                .EntityType<Exam>()
                .Collection
                .Action("BulkInsert")
                .ReturnsCollectionFromEntitySet<Exam>("Exams")
                .CollectionEntityParameter<Exam>("Exams");
            builder
                .EntityType<Group>()
                .Collection
                .Action("BulkInsertByName")
                .ReturnsCollectionFromEntitySet<Group>("Groups")
                .EntityParameter<Controllers.GroupNameTree>("Groups");

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
