$(document).ready(function () {
    var codeOfLastWarehouse = 999;
    var idEditItem;
    var idDelete = 0;
    var actionType;
    var errorText = [
        "Uzupełnij poprawnie wszystkie pola.",
        "Podana nazwa już występuję.",
        "Podany kod już występuję.",
    ]
    function resetForm() {
        $("#warehouseName").val("");
        $("#warehouseCode").val(codeOfLastWarehouse + 1);
        $("textarea").val("");
    }
    function openModal() {
        $(".background-shadow").css("display", "block");
        $(".center-modal").css("display", "flex");
        $("#warehouseCode").val(codeOfLastWarehouse + 1);
    }
    function closeModal() {
        $(".background-shadow").css("display", "none");
        $(".center-modal").css("display", "none");
        $("#modal-inf").css("display", "none");
        $("#modal-edit").css("display", "none");
        $("#modal-delete").css("display", "none");
        $(".error-text").css("display", "none");
        resetForm();
    }
    function changeFormatDate(dateString) {
        var newDateFormat = dateString.replace("T", "  ");
        return newDateFormat.substr(0, 17);
    }
    $("#warehouse-menu").css("background-color", "#eee");
    $("#warehouse-menu").css("color", "#1d2127");

    $(document).on("click", ".btn-modal", function () {

        openModal();
        $("#modal-edit").css("display", "block");

        actionType = $(this).data('type');

        if (actionType == "new") {
            $("#modal-header-warehouse").css("background-color", "#47a447");
            $("#modal-title-warehouse").html("Nowy magazyn");
            $("#btn-warehouse").css("background-color", "#47a447");
            $("#btn-warehouse").html("Dodaj magazyn");
        } else if (actionType == "edit")  {
            $("#modal-header-warehouse").css("background-color", "#de8500");
            $("#modal-title-warehouse").html("Edytuj magazyn");
            $("#btn-warehouse").css("background-color", "#de8500");
            $("#btn-warehouse").html("Zapisz zmiany");

            idEditItem = $(this).data('id');

            $.ajax({
                type: "GET",
                url: "/Warehouses/" + idEditItem,
                success: function (data) {
                    $("#warehouseName").val(data[0].name);
                    $("#warehouseCode").val(data[0].code);
                    $("#warehouseDescription").val(data[0].description);
                }
            });
        }
    });
    
    $(".modal-dismiss").click(function () {
        closeModal();
        resetForm();
    });

    $(document).on("click", "#btn-warehouse", function () {
    //$("#btn-warehouse").click(function () {

        var warehouseData = JSON.stringify({
            "code": $("#warehouseCode").val(),
            "name": $("#warehouseName").val(),
            "description": $("#warehouseDescription").val(),
        });
        
        if (actionType == "new") {
            $.ajax({
                type: "POST",
                url: "/Warehouses",
                data: warehouseData,
                dataType: "json",
                contentType: "application/json",
                success: function (data) {
                    if (data == -1) {
                        closeModal();
                        drawTable();
                    } else {
                        $(".error-text").css("display", "block");
                        $(".error-text").html(errorText[data]);
                    }
                }
            });

        } else if (actionType == "edit") {

            $.ajax({
                type: "PUT",
                url: "/Warehouses/" + idEditItem,
                data: warehouseData,
                dataType: "json",
                contentType: "application/json",
                success: function (data) {
                    if (data == -1) {
                        closeModal();
                        drawTable();
                    } else {
                        $(".error-text").css("display", "block");
                        $(".error-text").html(errorText[data]);
                    }
                }
            });
           
        }

    });
    //open information about warehouse
    $(document).on("click", ".item-name", function () {

        var idInformation = $(this).data('id');
        openModal();
        $("#modal-inf").css("display", "block");

        $.ajax({
            type: "GET",
            url: "/Warehouses/" + idInformation,
            success: function (data) {
                var newDateFormat = changeFormatDate(data[0].created);
                $("#warehouse-name-information").html(data[0].name);
                $("#warehouse-created-information").html(newDateFormat);
                $("#warehouse-user-information").html(data[0].userName);
            }
        });

        $("#modal-btn-inf").click(function () {
            closeModal();
        });
    });
    //open delete warehouse modal
    $(document).on("click", ".button-table-delete", function () {

        idDelete = $(this).data('id');
        $("#modal-delete").css("display", "block");
        openModal();

        $("#button-delete").click(function () {
        $.ajax({
            type: "DELETE",
            url: "/Warehouses/" + idDelete,
            success: function () {
                drawTable();
                closeModal();
            }
        });
        })


        $("#button-cancel").click(function () {
            closeModal();
        });
    });

    //draw table with information about warehouses

    function drawTable() {
        var id = 1;
        var warehouseData = '';

        $("#warehouse-table > tbody").empty();

        $.getJSON("/Warehouses", function (data) {
            $.each(data, function (key, value) {
                warehouseData += '<tr>';
                warehouseData += '<td>' + id++ + '</td>';
                warehouseData += '<td> <a href="#" class="item-name" data-id="'+value.id_warehouse+'">' + value.name + '</a></td>';
                warehouseData += '<td>' + value.code + '</td>';
                warehouseData += '<td class="item-description">' + value.description + '</td>';
                warehouseData += '<td><button class="btn-modal" id="button-table-edit" data-type="edit" data-id="' + value.id_warehouse + '">Edycja</button> <button class="button-table-delete"data-id="' + value.id_warehouse +'">Usuń</button></td>';
                warehouseData += '</tr>';
                codeOfLastWarehouse = value.code;
            });
            $("#warehouse-table").append(warehouseData);
        });
    }

    drawTable();

});