<%@ Page Language="C#" Debug="true" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Testovaci uloha</title>
<script type="text/javascript">
    function setedit()
    {
        tab = document.getElementById("DataGrid11");
        if (tab != null)
        {             
             for (iti = 0; iti < tab.rows.length; iti++)
               {                
                countcells = tab.rows[iti].cells.length;
                tab.rows[iti].deleteCell(countcells - 1);//neukazuje sloupec s Id
                celldatum = tab.rows[iti].cells[3];
                if (iti > 0)
                {
                    elems = celldatum.getElementsByTagName("INPUT");
                    if (elems.length > 0)
                    {                        
                        if (elems[0].type.toUpperCase() == "TEXT")
                            if (elems[0].value.length>=10)
                                elems[0].value = elems[0].value.substr(0, 10);

                        celldatum = tab.rows[iti].cells[2];//najde editacni prvek na zaostreni
                        elems = celldatum.getElementsByTagName("INPUT");
                        if (elems.length > 0)
                            if (elems[0].type.toUpperCase() == "TEXT")
                            {
                                elems[0].focus();
                                elems[0].select();
                            }
                    }
                    else
                        if (celldatum.innerText.length>=10)
                          celldatum.innerText = celldatum.innerText.substr(0, 10);
                 }
              }            
          }

        /*elems = document.getElementsByTagName("INPUT"); //dalsi moznost,jak najit editacni prvek na zaostreni
        for (iti = 0; iti < elems.length; iti++)
        {
            if (elems[iti].type.toUpperCase() == "TEXT")
            {                
                elems[iti].focus();
                elems[iti].select();
                break;
            }
        }*/
    }
</script>
</head>
<body onload="setedit()">
    <form id="form1" runat="server">
    <h2>Testovaci uloha</h2>
    <div style="position:absolute;top:20%;">
    <asp:DataGrid ID="DataGrid11" runat ="server"/>                       
    </div>
    
    <div style="position:absolute;top:10%;left:50%">
    
    </div>
    </form>
</body>
</html>
