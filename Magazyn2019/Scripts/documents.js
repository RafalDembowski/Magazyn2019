$(document).ready(function () {

    $("#documents-menu").css("background-color", "#eee");
    $("#documents-menu").css("color", "#1d2127");

    var moveTypeArray = [
        "",
        "Przyjęcie",
        "Wydanie",
        "Przesunięcie",
    ]
    var pdfTypeArray = [
        "",
        "PZ",
        "WZ",
        "MM",
    ]

    function changeFormatDate(dateString) {
        var newDateFormat = dateString.replace("T", "  ");
        return newDateFormat.substr(0, 10);
    }

    //draw table with information about documents
        function drawTable() {

        var id = 1;
        var documentsData = '';

        $("#document-table > tbody").empty();

        $.getJSON("/Document", function (data) {
            $.each(data, function (key, value) {
                documentsData += '<tr>';
                documentsData += '<td>' + id++ + '</td>';
                documentsData += '<td>' + changeFormatDate(value.time) + '</td>';
                documentsData += '<td>' + moveTypeArray[value.type] + '</td>';
                documentsData += '<td>' + value.name_Customer + '</td>';
                documentsData += '<td>' + value.name_WarehouseOne + '</td>';
                documentsData += '<td>' + value.name_WarehouseTwo + '</td>';
                documentsData += '<td>' + value.name_User + '</td>';
                documentsData += '<td>' + pdfTypeArray[value.type] + '/' +value.number + '</td>';
                documentsData += '<td> <a href="/pdf/' + pdfTypeArray[value.type] + '/' + value.number + '.pdf"><img src="/Content/Img/pdf.png" style="width:26px;"></a> </td>';
                documentsData += '</tr>';
            });
            $("#document-table").append(documentsData);
        });
    }

    drawTable();

});