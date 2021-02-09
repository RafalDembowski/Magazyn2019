$(document).ready(function () {
    var codeOfLastCustomer = 1999;
    var idEditItem;
    var idDelete = 0;
    var actionType;
    var errorText = [
        "Uzupełnij poprawnie wszystkie pola.",
        "Podana nazwa już występuję.",
        "Podany kod już występuję.",
    ]
    var typeOfCustomer = [
        "",
        "Dostawca",
        "Odbiorca",
        "Dostawca / Odbiorca",
    ]
    
    function resetForm() {
        $("#customerName").val("");
        $("#customerCode").val(codeOfLastCustomer + 1);
        $("#customerStreet").val("");
        $("#customerZipCode").val("");
        $("#customerCity").val("");
        $("select").val("");
    }
    
    function openModal() {
        $(".background-shadow").css("display", "block");
        $(".center-modal").css("display", "flex");
        $("#customerCode").val(codeOfLastCustomer + 1);
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
    function displayTable() {
        $("#customer-table").css("display", "table");
        $("#no-items-information").css("display", "none");
    }
    function removeTable() {
        $("#customer-table").css("display", "none");
        $("#no-items-information").css("display", "block");
    }
    $("#customer-menu").css("background-color", "#eee");
    $("#customer-menu").css("color", "#1d2127");

    $(document).on("click", ".btn-modal", function () {

        openModal();
        $("#modal-edit").css("display", "block");

        actionType = $(this).data('type');

        if (actionType == "new") {
            $("#modal-header-customer").css("background-color", "#47a447");
            $("#modal-title-customer").html("Nowy kontrahent");
            $("#btn-customer").css("background-color", "#47a447");
            $("#btn-customer").html("Dodaj kontrahenta");
        } else if (actionType == "edit") {
            $("#modal-header-customer").css("background-color", "#de8500");
            $("#modal-title-customer").html("Edytuj kontrahenta");
            $("#btn-customer").css("background-color", "#de8500");
            $("#btn-customer").html("Zapisz zmiany");

            idEditItem = $(this).data('id');

            
            $.ajax({
                type: "GET",
                url: "/Customers/" + idEditItem,
                success: function (data) {
                    $("#customerName").val(data[0].name);
                    $("#customerCode").val(data[0].code);
                    $("#customerStreet").val(data[0].street);
                    $("#customerZipCode").val(data[0].zipCode);
                    $("#customerCity").val(data[0].city);
                    $("select").val(data[0].type);
                }
            });
            
        }
    });

    $(".modal-dismiss").click(function () {
        closeModal();
        resetForm();
    });
    
    $(document).on("click", "#btn-customer", function () {

        var customerData = JSON.stringify({
            "code": $("#customerCode").val(),
            "name": $("#customerName").val(),
            "street": $("#customerStreet").val(),
            "zipCode": $("#customerZipCode").val(),
            "city": $("#customerCity").val(),
            "type": $("#customerType").val(),
        });

        if (actionType == "new") {
            $.ajax({
                type: "POST",
                url: "/Customers",
                data: customerData,
                dataType: "json",
                contentType: "application/json",
                success: function (data) {
                    if (data == -1) {
                        closeModal();
                        drawTable();
                    }
                    if (data == 3) {
                        location.reload();
                    }
                    if (data >= 0 && data < 3) {
                        $(".error-text").css("display", "block");
                        $(".error-text").html(errorText[data]);
                    }
                }
            });

        } else if (actionType == "edit") {

            $.ajax({
                type: "PUT",
                url: "/Customers/" + idEditItem,
                data: customerData,
                dataType: "json",
                contentType: "application/json",
                success: function (data) {
                    if (data == -1) {
                        closeModal();
                        drawTable();
                    }
                    if (data == 3) {
                        location.reload();
                    }
                    if (data >= 0 && data < 3) {
                        $(".error-text").css("display", "block");
                        $(".error-text").html(errorText[data]);
                    }
                }
            });

        }

    });
    
    //open information about customer
    $(document).on("click", ".item-name", function () {

        var idInformation = $(this).data('id');
        openModal();
        $("#modal-inf").css("display", "block");

        $.ajax({
            type: "GET",
            url: "/Customers/" + idInformation,
            success: function (data) {
                var newDateFormat = changeFormatDate(data[0].created);
                $("#customer-name-information").html(data[0].name);
                $("#customer-created-information").html(newDateFormat);
                $("#customer-user-information").html(data[0].userName);
            }
        });

        $("#modal-btn-inf").click(function () {
            closeModal();
        });
    });
    //open delete customer modal
    $(document).on("click", ".button-table-delete", function () {

        idDelete = $(this).data('id');
        alert(idDelete);
        $("#modal-delete").css("display", "block");
        openModal();

        $("#button-delete").click(function () {
            $.ajax({
                type: "DELETE",
                url: "/Customers/" + idDelete,
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

    //draw table with information about customers

    function drawTable() {
        var id = 1;
        var customerData = '';

        $("#customer-table > tbody").empty();

        $.getJSON("/Customers", function (data) {
            if (data != 0) {
                $.each(data, function (key, value) {
                    customerData += '<tr>';
                    customerData += '<td>' + id++ + '</td>';
                    customerData += '<td> <a href="#" class="item-name" data-id="' + value.id_customer + '">' + value.name + '</a></td>';
                    customerData += '<td>' + value.street + '</br>' + value.zipCode + ' ' + value.city + '</td>';
                    customerData += '<td>' + value.code + '</td>';
                    customerData += '<td>' + typeOfCustomer[value.type] + '</td>';
                    customerData += '<td><button class="btn-modal" id="button-table-edit" data-type="edit" data-id="' + value.id_customer + '">Edycja</button> <button class="button-table-delete"data-id="' + value.id_customer + '">Usuń</button></td>';
                    customerData += '</tr>';
                    codeOfLastCustomer = value.code;
                });
                $("#customer-table").append(customerData);
                displayTable();
            } else {
                removeTable();
            }

        });
    }

    drawTable();

    
});