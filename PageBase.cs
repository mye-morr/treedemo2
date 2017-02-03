using System.Web.UI;
using System.Data.SqlClient;

namespace ASTreeViewDemo
{
	public class PageBase : Page
	{
		protected string NorthWindConnectionString
		{
			get
			{
				//bind data from data table
				string path = Server.MapPath( "~/" ); //System.AppDomain.CurrentDomain.BaseDirectory;
                string connStr = string.Format("Provider=SQLNCLI11;Data source=184.168.194.77;Initial Catalog=narfdaddy1;User ID=narfdaddy;Password=TreeDemo1");
                //string connStr = string.Format("Provider=SQLNCLI11;Data source=kerrapp_jeffry\\MSSQLSERVER12;Initial Catalog=Northwind;User ID=upwork;Password=upwork");


				return connStr;
			}
		}
	}
}
