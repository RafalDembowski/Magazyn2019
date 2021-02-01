using Magazyn2019.Models;
using OpenHtmlToPdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Magazyn2019
{
    public class PdfDocument
    {
        private string[] typeOfProduct =
        { 
        "",
        "Sztuki",
        "Litry",
        "Kilogramy"
        };

        private string[] textArray = new string[4];
        private string logoPdfPath = HttpContext.Current.Server.MapPath("~/Content/Img/logo_pdf.png");

        public byte[] preparePdf(Move move)
        {
            string itemToTable;
            itemToTable = setItemToTable(move);
            textArray = prepereTextToDocument(move);

            string htmlToPdf =
                        "<html>" +
                        "<head> <meta charset='UTF-8'>" +
                        "</head>" +
                        "<style>" +
                        "@font - face {" +
                        "font - family: 'OpenSans';" +
                        "src: url(../fonts / OpenSans-Regular.ttf);" +
                        "font - weight: normal;" +
                        "font - style: normal;" +
                        "}" +
                        ".OpenSans {" +
                        "font-family: 'Open Sans';" +
                        "}" +
                        ".pdf - body{" +
                        "background: none! important;" +
                        "margin: 50px;" +
                        "}" +
                        ".pdf-row" +
                        "{" +
                        "margin-top:15px;" +
                        "}" +
                        ".pdf-row-information{" +
                        "margin-top: 15px;" +
                        "padding: 15px;" +
                        "width: 300px;" +
                        "border: solid 1px black;" +
                        "float: left;" +
                        "}" +
                        ".move-info{" +
                        "float: right;" +
                        "}" +
                        "table {" +
                        "width: 100%;" +
                        "}" +
                        "table, th, td {" +
                        "border: solid 1px #DDD;" +
                        "border-collapse: collapse;" +
                        "text-align: center;" +
                        "color: #777;" +
                        "font-weight: 500;" +
                        "}" +
                        "th, td {" +
                        "padding: 1rem;" +
                        "}" +
                        "thead {" +
                        "background-color: #f6f6f6;" +
                        "}" +
                        ".pdf-body h2 {" +
                        "text-align: center;" +
                        "width: 100%;" +
                        "float: left;" +
                        "}" +
                        ".pdf-signature{" +
                        "margin-top: 35px;" +
                        "float: right;" +
                        "}" +
                        "</style>" +
                        "<body class='pdf-body'>" +
                        "<img src = '" + logoPdfPath + "' height='80'>" +
                        "<div class='pdf-row'>" +
                        textArray[3] +
                        "<div class='move-info'>" +
                        "<p class ='OpenSans' >"+ textArray[1] +" <b>" + move.time.ToString("dd/MM/yyyy") + "</b></p>" +
                        "<p class ='OpenSans' >Magazyn: <b>" + move.WarehouseOne.name + "</b></p>" +
                        "<p class ='OpenSans' >Kod: " + move.WarehouseOne.code + "</p>" +
                        "</div>" +
                        "</div>" +
                        "<div>" +
                        "<h2 class ='OpenSans'> "+ textArray[2] +" "+ move.number +" / "+ move.time.ToString("yyyy") + "r </h2>" +
                        "</div>" +
                        "<div>" +
                        "<table>" +
                        "<thead>" +
                        "<tr>" +
                        "<th></th>" +
                        "<th class ='OpenSans'> Nazwa </ th >" +
                        "<th class ='OpenSans' > Ilość </ th >" +
                        "<th class ='OpenSans' > Jednostka </ th >" +
                        "<th class ='OpenSans'> Kod produktu </ th >" +
                        "</tr>" +
                        "</thead>";

            string endHtml =
                        "</table>" +
                        "</div>" +
                        "<div class='pdf-signature'>" +
                        "<p class ='OpenSans'>Dokument wystawił:</p>" +
                        "<p class ='OpenSans'><b>"+ move.User.fullName +"</b></p>" +
                        "</div>" +
                        "</body>" +
                        "</html>";

            htmlToPdf += itemToTable + endHtml;

            var movePdf = Pdf
	                  .From(htmlToPdf)
	                  .Content();

            return movePdf;
        }
        
        public void savePDF(byte[] pdf, Move move)
        {
            string path="";
            switch (move.type)
            {
            case 1:
              path = HttpContext.Current.Server.MapPath("~/pdf/PZ/" + move.number + ".pdf");
              break;
            case 2:
              path = HttpContext.Current.Server.MapPath("~/pdf/WZ/" + move.number + ".pdf");
              break;
            case  3:
              path = HttpContext.Current.Server.MapPath("~/pdf/MM/" + move.number + ".pdf");
              break;
            }
            try
            {
                File.WriteAllBytes(path, pdf);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
        private string setItemToTable(Move move)
        {
            string itemInTable = "<tbody>";
            int amountItem = 1; 

            foreach (Inventory item in move.Inventories)
            {
                itemInTable +=
                    "<tr>" +
                    "<td>" + amountItem + "</td>" +
                    "<td>" + item.Product.name + "</td>" +
                    "<td >" +item.amount + "</td>" +
                    "<td>" + typeOfProduct[item.Product.unit] + "</td>" +
                    "<td>" + item.Product.code + "</td>" +
                    "</tr>";

                amountItem++;
            }
            itemInTable += "</tbody>";

            return itemInTable;
        }
        private string[] prepereTextToDocument(Move move)
        {
            switch (move.type)
            {
                case 1:
                    textArray[0] = "Dostawca:";
                    textArray[1] = "Data przyjęcia:";
                    textArray[2] = "DOKUMENT PRZYJĘCIA nr PZ";
                    textArray[3] = "<div class='pdf-row-information'>" +
                                   "<p class ='OpenSans' ><b>" + textArray[0] + "</b></p>" +
                                   "<p class ='OpenSans' >" + move.Customer.name + "</p>" +
                                   "<p class ='OpenSans' >" + move.Customer.street + "</p>" +
                                   "<p class ='OpenSans' >" + move.Customer.city + " " + move.Customer.code + "</p>" +
                                   "</div>";
                    break;
                case 2:
                    textArray[0] = "Odbiorca:";
                    textArray[1] = "Data wydania:";
                    textArray[2] = "DOKUMENT WYDANIA nr WZ";
                    textArray[3] = "<div class='pdf-row-information'>" +
                                   "<p class ='OpenSans' ><b>" + textArray[0] + "</b></p>" +
                                   "<p class ='OpenSans' >" + move.Customer.name + "</p>" +
                                   "<p class ='OpenSans' >" + move.Customer.street + "</p>" +
                                   "<p class ='OpenSans' >" + move.Customer.city + " " + move.Customer.code + "</p>" +
                                   "</div>";
                    break;
                case 3:
                    textArray[0] = "";
                    textArray[1] = "Data wydania:";
                    textArray[2] = "DOKUMENT PRZESUNIĘCIA nr WZ";
                    textArray[3] = "<div class='pdf-row-information'>" +
                                   "<p class ='OpenSans' ><b>Wejście</b></p>" +
                                   "<p class ='OpenSans' >Magazyn: <b>" + move.WarehouseTwo.name + "</b></p>" +
                                   "<p class ='OpenSans' >Kod: " + move.WarehouseTwo.code + "</p>" +
                                   "</div>";
                    break;
            }
            return textArray;
        }

    }
}