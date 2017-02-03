using System.Web.UI;
using System.Data.SqlClient;

namespace Goldtect.ASTreeViewDemo
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
                //string connStr = string.Format("Provider=SQLNCLI11;Data Source=Michael1CB7\\BC;Initial Catalog=narfdaddy1;Persist Security Info=True;User ID=sa;Password=mogilev1");

				return connStr;
			}
		}
	}
}
