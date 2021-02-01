$(document).ready(function () {

    var unitArray = [
        "",
        "Sztuki",
        "Litry",
        "Kilogramy",
    ]

    $("#states-menu").css("background-color", "#eee");
    $("#states-menu").css("color", "#1d2127");
    $("#products-filter").prop("disabled", "disabled");

    function displayTable() {
        $("#inventories-table").css("display", "table");
        $("#no-items-information").css("display", "none");
    }

    function removeTable() {
        $("#inventories-table").css("display", "none");
        $("#no-items-information").css("display", "block");
    }

    //take information about products in choosen warehouse
    function getNameOfGroupsToSelect() {
        $.getJSON("Products", function (data) {
                $.each(data, function (key, value) {
                    $("#products-filter").append('<option value="' + value.id_product + '">' + value.name + '</option > ');
                });
        });
    }
 
    function getAllWarehousesToSelect() {
        $.getJSON("Warehouses", function (data) {
            $.each(data, function (key, value) {
                $('#warehouses-filter').append('<option value="' + value.id_warehouse + '">' + value.name + '</option > ');
            });
        });
    }

    function getProductsInWarehouse() {
        $("#inventories-table > tbody").empty();

        var idWarehouse = $("#warehouses-filter").val();
        var id = 1;
        var numberProductArray = 0;
        $.getJSON("Inventories/" + idWarehouse, function (data) {
            if (data != 0) {
                $.each(data, function (key, value) {

                    var inventoriesTable = '';
                    inventoriesTable += '<tr>';
                    inventoriesTable += '<td>' + id++ + '</td>';
                    inventoriesTable += '<td>' + data[numberProductArray].name + '</a></td>';
                    inventoriesTable += '<td>' + data[numberProductArray].code + '</td>';
                    inventoriesTable += '<td>' + data[numberProductArray].groupName + '</td>';
                    inventoriesTable += '<td>' + data[numberProductArray].description + '</td>';
                    inventoriesTable += '<td>' + unitArray[data[numberProductArray].unit] + '</td>';
                    inventoriesTable += '<td>' + value.amount + '</td>';
                    inventoriesTable += '</tr>';
                    numberProductArray++;
                    $("#inventories-table").append(inventoriesTable);
                    displayTable();
                });
            } else {
                removeTable();
            }
        });
    }
    //get one product
    function getOneProductInWarehouse(inventoryData) {
        //to mozna zmienic na funkcje
        $("#inventories-table > tbody").empty();
        var id = 1;
        var inventoryDataJson = JSON.stringify(inventoryData);
        $.ajax({
            type: "POST",
            url: "Inventories",
            data: inventoryDataJson,
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                if (data != 0) {
                    var inventoriesTable = '';
                    inventoriesTable += '<tr>';
                    inventoriesTable += '<td>' + id++ + '</td>';
                    inventoriesTable += '<td>' + data[0].name + '</a></td>';
                    inventoriesTable += '<td>' + data[0].code + '</td>';
                    inventoriesTable += '<td>' + data[0].groupName + '</td>';
                    inventoriesTable += '<td>' + data[0].description + '</td>';
                    inventoriesTable += '<td>' + unitArray[data[0].unit] + '</td>';
                    inventoriesTable += '<td>' + data[0].amount + '</td>';
                    inventoriesTable += '</tr>';
                    $("#inventories-table").append(inventoriesTable);
                    displayTable();
                } else {
                    removeTable();
                }
            }
        });
    }
    //chack which option is selected
    $('#warehouses-filter').on("change", function () {
        if (this.value != "") {
            $("#products-filter").prop("disabled", false);
            getProductsInWarehouse();
        }
        if (this.value == "") {
            $("#products-filter").prop("disabled", "disabled");
            $("#products-filter").val(""); 
            removeTable();
        }      
    });

    $("#products-filter").on("change", function () {
        if (this.value != null && this.value != "") {
            var inventoryData = {
                "idWarehouse": $("#warehouses-filter").val(),
                "idProduct": $("#products-filter").val(),
            };
            getOneProductInWarehouse(inventoryData);
        } else {
            getProductsInWarehouse();
        }
    });

    getAllWarehousesToSelect();
    getNameOfGroupsToSelect();
});