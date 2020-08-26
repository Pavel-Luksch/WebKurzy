using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web;
using System;
using System.IO;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using WebKurzyApplication3;
using System.Text;
using System.Web.Hosting;








public partial class _Default : ViewPage
{
    NumberFormatInfo nfi = new NumberFormatInfo();     
    System.Data.DataTable dt1,dt2;
    DataView dtv1,dtv2;
    DataGrid DataGrid1,DataGrid2;
    Button Button1;    
    SqlConnection conn;

    protected override void OnUnload(EventArgs e)
    {
        try
        {
            if (conn!=null)
               conn.Close();
            base.OnUnload(e);
        }
        catch { }
    }
    public void Page_Load(object sender, EventArgs e)
    {
        nfi.NumberDecimalSeparator = ".";        
        string constr = Global.connstr;        

        //String spojeni = ConfigurationSettings.AppSettings[0]; //mozne nastaveni ve web.config
        dt1 = new DataTable();
        dt2 = new DataTable();
        conn = new SqlConnection(constr);
        conn.Open();
        
        using (var comm = conn.CreateCommand())
           {
            SelectTable(comm);
          }
        
        dt1.Columns["Id"].ReadOnly = true;        
        dtv1 = new DataView(dt1);
        dtv2 = new DataView(dt2);


        Button but = new Button();
        but.Text = "Pridat radek";
        but.ID = "BUTTON1";
        //Button1 = FindControl("BUTTON1") as Button;
        //Button1.Click += Button1_Click;
        but.Click += Button1_Click;
        

        EditCommandColumn edcol = new EditCommandColumn();
        edcol.EditText = "Oprava";
        edcol.CancelText = "Unik";
        edcol.UpdateText = "Ulozeni";
        edcol.ItemStyle.Wrap = false;
        edcol.HeaderStyle.Wrap = false;
        ButtonColumn butcol = new ButtonColumn();
        butcol.ButtonType = ButtonColumnType.LinkButton;
        butcol.Text = "Smazat";
        butcol.CommandName = "Delete";
        

        DataGrid1 = FindControl("DataGrid11") as DataGrid;
        DataGrid1.Columns.Add(edcol);
        DataGrid1.Columns.Add(butcol);
        Form.Controls.Add(but);

        DataGrid1.EditCommand +=
             new DataGridCommandEventHandler(this.ItemsGrid_Edit);
        DataGrid1.CancelCommand +=
            new DataGridCommandEventHandler(this.ItemsGrid_Cancel);
        DataGrid1.UpdateCommand +=
            new DataGridCommandEventHandler(this.ItemsGrid_Update);
        DataGrid1.ItemCommand +=
            new DataGridCommandEventHandler(this.ItemsGrid_Command);
        DataGrid1.AllowSorting = true;        
        
        
        DataGrid1.DataSource = dt1;
        DataGrid1.DataBind();


        //DataGrid2 = FindControl("DataGrid22") as DataGrid;
        DataGrid2 = new DataGrid();
        DataGrid2.Style.Value = "position:absolute;top:10%;left:50%";
        DataGrid1.Parent.Controls.Add(DataGrid2);
        DataGrid2.DataSource = dt2;
        DataGrid2.DataBind();

    }

    public void Button1_Click(object sender, EventArgs e)
    {        
        using (var comm = conn.CreateCommand())
        {            
            comm.CommandText = "INSERT INTO KURZY VALUES('','"+DbDatum(DateTime.Now)+"',0.00)";
            comm.ExecuteNonQuery();
            SelectTable(comm);            
        }
        BindGrid(); 
    }

    
   

    public void ItemsGrid_Edit(Object sender, DataGridCommandEventArgs e)
    {
                
        DataGrid1.EditItemIndex = e.Item.ItemIndex;        
        BindGrid();        
    }

    public void ItemsGrid_Cancel(Object sender, DataGridCommandEventArgs e)
    {
        
        DataGrid1.EditItemIndex = -1;
        BindGrid();

    }

    public void ItemsGrid_Update(Object sender, DataGridCommandEventArgs e)
    {
        TextBox t1=(TextBox)e.Item.Cells[2].Controls[0];
        TextBox t2 = (TextBox)e.Item.Cells[3].Controls[0];
        TextBox t3 = (TextBox)e.Item.Cells[4].Controls[0];
        string mena = null;
        string datum = null;
        string kurz = string.Empty;
        bool okdatum = false;
        try
        {
            mena = Request.Form.Get(t1.UniqueID);
            datum = Request.Form.Get(t2.UniqueID);
            datum = DbDatum(Convert.ToDateTime(datum));
            okdatum = true;
            kurz = Request.Form.Get(t3.UniqueID)+string.Empty;
            kurz = kurz.Replace(',', '.');
            kurz = Convert.ToString(Convert.ToDouble(kurz, nfi), nfi);            
        }
        catch 
        {
            kurz = "0.00";
            if (!okdatum)
               datum = DbDatum(DateTime.Now);
        }
        string idkurz = e.Item.Cells[5].Text;
        if ((mena!=null) & (datum!=null)&(!string.IsNullOrEmpty(idkurz)))
          {            
            using (var comm = conn.CreateCommand())
            {
                comm.CommandText = "UPDATE KURZY SET Mena='" + mena + "',Datum='" + datum + "',Kurz=" + kurz + " WHERE Id=" + idkurz;
                comm.ExecuteNonQuery();
                SelectTable(comm);                
            }
            BindGrid();
          }
    }

    public void ItemsGrid_Command(Object sender, DataGridCommandEventArgs e)
    {
        switch (((LinkButton)e.CommandSource).CommandName)
        {

            case "Delete":
                DeleteItem(e);
                break;
            
            default:break;

        }
    }

    void DeleteItem(DataGridCommandEventArgs e)
    {
        string idkurz = e.Item.Cells[5].Text;
        if (!string.IsNullOrEmpty(idkurz))
           using (var comm = conn.CreateCommand())
             {
               comm.CommandText = "DELETE FROM KURZY WHERE Id="+idkurz;
               comm.ExecuteNonQuery();
               SelectTable(comm);            
             }
        BindGrid();
    }
    void BindGrid()
    {
        
        DataGrid1.DataSource = dtv1;
        DataGrid1.DataBind();
        DataGrid2.DataSource = dtv2;
        DataGrid2.DataBind();

    }

    void SelectTable(SqlCommand comm)
    {
        comm.CommandText = "SELECT * FROM KURZY";
        dt1.Clear();        
        dt1.Load(comm.ExecuteReader());

        comm.CommandText = "SELECT mena,avg(kurz) as prumer,max(kurz) as maximum,min(kurz) as minimum FROM KURZY GROUP BY mena";
        dt2.Clear();
        dt2.Load(comm.ExecuteReader());
    }

    string DbDatum( DateTime dateTime)
    {
        string dnes = dateTime.Year + "-" + dateTime.Month + "." + dateTime.Day;
        if (dnes.IndexOf('.') < 7)
            dnes.Insert(5, "0");
        if (dnes.Length < 10)
            dnes.Insert(8, "0");
        dnes = dnes.Replace('.', '-');
        return dnes;
    }
}

