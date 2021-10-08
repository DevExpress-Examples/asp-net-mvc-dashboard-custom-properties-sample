using System.Web.Routing;
using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Mvc;
using DevExpress.DataAccess.Sql;
using System.Web.Hosting;
using DevExpress.DataAccess.Excel;
using DevExpress.DashboardCommon;
using System.Web;
using System.IO;
using System.Xml.Linq;

namespace AspMvcDashboardCustomPropertiesSample {
    public static class DashboardConfig {

        public static string  DashboardFolder {get { return HostingEnvironment.MapPath("~/App_Data/Dashboards");  } }
        public static void RegisterService(RouteCollection routes) {
            routes.MapDashboardRoute("dashboardControl", "DefaultDashboard");

            // DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(DashboardFolder);
            // DashboardConfigurator.Default.SetDashboardStorage(dashboardFileStorage);

            DashboardConfigurator.Default.SetDashboardStorage(SessionDashboardStorage.Instance); 

            DashboardConfigurator.Default.CustomExport += (s, e) => {
                ChartConstantLinesExtension.CustomExport(e);
            };
            // Uncomment this string to allow end users to create new data sources based on predefined connection strings.
            //DashboardConfigurator.Default.SetConnectionStringsProvider(new DevExpress.DataAccess.Web.ConfigFileConnectionStringsProvider());

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

            DashboardConfigurator.Default.ConfigureItemDataCalculation += (s, e) => {
                e.CalculateAllTotals = true;
            };
            // Registers an SQL data source.
            DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
            SelectQuery query = SelectQueryFluentBuilder
                .AddTable("SalesPerson")
                .SelectAllColumns()
                .Build("Sales Person");
            sqlDataSource.Queries.Add(query);
            dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());
            
            // Registers an Object data source.
            DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
            dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());
            
            // Registers an Excel data source.
            DashboardExcelDataSource excelDataSource = new DashboardExcelDataSource("Excel Data Source");
            excelDataSource.FileName = HostingEnvironment.MapPath(@"~/App_Data/Sales.xlsx");
            excelDataSource.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings("Sheet1"));
            dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml());
            
            DashboardConfigurator.Default.SetDataSourceStorage(dataSourceStorage);
            DashboardConfigurator.Default.DataLoading += DataLoading;
        }

        private static void DataLoading(object sender, DataLoadingWebEventArgs e) {
            if(e.DataSourceName == "Object Data Source") {
                e.Data = Invoices.CreateData();
            }
        }
    }
}
