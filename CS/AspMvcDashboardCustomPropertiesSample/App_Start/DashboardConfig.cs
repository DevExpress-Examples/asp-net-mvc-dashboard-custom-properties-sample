using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Mvc;
using DevExpress.DataAccess.Sql;
using System.Web.Hosting;
using System.Web.Routing;

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

            DashboardConfigurator.Default.SetDataSourceStorage(dataSourceStorage);
        }
    }
}
