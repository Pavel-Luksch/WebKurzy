using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WebKurzyApplication3
{
    public class Global : HttpApplication
    {
        NumberFormatInfo nfi = new NumberFormatInfo();
        public static string connstr = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=SSPI;DATABASE=TEST";
        void Application_Start(object sender, EventArgs e)
        {
            // Kód, který je spuštěn při spuštění aplikace
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // -------- po zalozeni databaze a tabulky mozno zakomentovat --------------
            using (var conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=SSPI"))
            {
                try
                {
                    conn.Open();
                    using (var comm = conn.CreateCommand())
                    {
                        comm.CommandText = "CREATE DATABASE TEST";                        
                        try
                        {
                            comm.ExecuteNonQuery();
                        }
                        catch (Exception expo)
                        {
                            string hlaska = expo.Message;
                        }
                    }
                }
                catch 
                { 
                }
            }
            using (var conn = new SqlConnection(connstr))
               {
                 try
                  {
                     conn.Open();
                  }
                  catch {}
                bool novatab = true;
                  using (var comm = conn.CreateCommand())
                    {
                       comm.CommandText = "DROP TABLE KURZY";
                        try
                         {
                           //comm.ExecuteNonQuery(); //v pripade,ze chci tabulku znovu vytvorit
                         }
                        catch (Exception expo)
                        {
                          string hlaska = expo.Message;
                        }

                    
                    //comm.CommandText = "CREATE TABLE KURZY ([Mena][NVARCHAR](5) ,[Datum] [NVARCHAR](10) ,[Kurz] NUMERIC(9, 3) NULL,[Id][BIGINT] IDENTITY (1, 1) NOT NULL)"; //datum jako string
                    comm.CommandText = "CREATE TABLE KURZY ([Mena][NVARCHAR](5) ,[Datum] [DATE] ,[Kurz] NUMERIC(9, 3) NULL,[Id][BIGINT] IDENTITY (1, 1) NOT NULL)"; //datum jako datum
                    try
                        {
                           comm.ExecuteNonQuery();
                        }
                        catch (Exception expo)
                          {
                            string hlaska = expo.Message;
                            novatab = false;
                          }
                 }  
               if (novatab)
                 ImportExcelu1(conn);
            }
            
            
        }
       

        public void ImportExcelu1(SqlConnection conn)
        {
            string namefile = Server.MapPath("~/") + "KURZY.XLSX";
            ApplicationClass excelapp = new ApplicationClass();            
            excelapp.Visible = false;
            excelapp.ThousandsSeparator = ".";
            excelapp.DecimalSeparator = ".";            
            excelapp.UseSystemSeparators = false;
            excelapp.DisplayAlerts = false;
            excelapp.Workbooks.Open(namefile);
            Worksheet wbk1 = (Worksheet)excelapp.Workbooks[1].Worksheets[1];
            int RangeRowCount = wbk1.UsedRange.Rows.Count;
            StringBuilder SBInsert = new StringBuilder();
            nfi.NumberDecimalSeparator = ".";

            for (int irow = 2; irow <= RangeRowCount; irow++)
            {                
                try
                {
                    string mena = (string)((Range)wbk1.Cells[irow, 1]).Text;
                    ((Range)wbk1.Cells[irow, 2]).NumberFormat = "yyyy-mm-dd";
                    string datstr = (string)((Range)wbk1.Cells[irow, 2]).Text;
                    DateTime datum = System.Convert.ToDateTime((string)((Range)wbk1.Cells[irow, 2]).Text);
                    double kurz = System.Convert.ToDouble(((string)((Range)wbk1.Cells[irow, 3]).Text), nfi);
                    SBInsert.Append("('" + mena + "','" + datstr + "'," + System.Convert.ToString(kurz, nfi) + ')');
                }
                catch 
                {                    
                }                
            }

            excelapp.Workbooks[1].Close();
            wbk1 = null;
            excelapp.Quit();
            excelapp = null;

            using (var comm = conn.CreateCommand())
            {
                try
                {
                    comm.CommandText = "DELETE FROM KURZY";
                    comm.ExecuteNonQuery();
                }
                catch { }
                comm.CommandText = "INSERT INTO KURZY VALUES " + SBInsert.ToString().Replace(")(", "),(");
                var poms = comm.CommandText;
                SBInsert.Clear();
                namefile = namefile + ".TXT";
                //File.WriteAllText(namefile, poms); zapis pro kontrolu prikazu
                comm.ExecuteNonQuery();
            }

        }
    }
}