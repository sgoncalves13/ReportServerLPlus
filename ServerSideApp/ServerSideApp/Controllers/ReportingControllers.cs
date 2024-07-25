using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.QueryBuilder.Native.Services;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using Microsoft.AspNetCore.Mvc;

namespace ServerSideApp.Controllers
{
    public class CustomReportDesignerController : ReportDesignerController
    {
        public CustomReportDesignerController(IReportDesignerMvcControllerService controllerService) : base(controllerService)
        {
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetDesignerModel(
            [FromForm] string reportName,
            [FromServices] IReportDesignerModelBuilder reportDesignerModelBuilder)
        {
            var dataSources = new Dictionary<string, object>();
            var ds = new SqlDataSource("HERMES_ProfitPlusADM_Connection");

            // Create a SQL query to access the Products data table.
            //SelectQuery query = SelectQueryFluentBuilder.AddTable("Products").SelectAllColumnsFromTable().Build("Products");
            SelectQuery query = SelectQueryFluentBuilder
            .AddTable("factura_factura")
            .SelectColumns(
                "factura_factura.id",
                "factura_factura.cliente_id",
                "factura_factura.observacion",
                "factura_factura.codigo",
                "factura_factura.anulado",
                "factura_factura.monto_impuesto",
                "factura_factura.monto_precio_total",
                "factura_renglon.id AS factura_renglon_id",
                "factura_renglon.factura_id",
                "factura_renglon.secuencia",
                "factura_renglon.art_id",
                "factura_renglon.cantidad",
                "factura_renglon.unidadmedida_id",
                "factura_renglon.precio_unitario",
                "factura_renglon.descuento",
                "factura_renglon.recargo",
                "factura_renglon.impuesto",
                "factura_renglon.precio_total",
                "art_art.descripcion AS Desc_Art",
                "cliente_cliente.descripcion AS cliente_cliente_descripcion"
            )
            .Join("factura_renglon", "factura_factura.id", "factura_renglon.factura_id")
            .Join("art_art", "factura_renglon.art_id", "art_art.id")
            .Join("cliente_cliente", "factura_factura.cliente_id", "cliente_cliente.id")
            .Build("factura_factura_with_joins");
            ds.Queries.Add(query);
            ds.RebuildResultSchema();
            dataSources.Add("ProfitADM", ds);

            reportName = string.IsNullOrEmpty(reportName) ? "TestExportReport" : reportName;
            var designerModel = await reportDesignerModelBuilder
                .Report(reportName)
                .DataSources(dataSources)
                .BuildModelAsync();

            return DesignerModel(designerModel);

        }
    }

    public class CustomQueryBuilderController : QueryBuilderController
    {
        public CustomQueryBuilderController(IQueryBuilderMvcControllerService controllerService) : base(controllerService)
        {
        }
    }

    public class CustomWebDocumentViewerController : WebDocumentViewerController
    {
        public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService) : base(controllerService)
        {
        }
    }
}