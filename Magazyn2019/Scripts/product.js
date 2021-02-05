$(document).ready(function () {

    var codeOfLastProduct = 3999;
    var idEditItem;
    var idDelete = 0;
    var actionType;
    var errorText = [
        "Uzupełnij poprawnie wszystkie pola.",
        "Podana nazwa już występuję.",
        "Podany kod już występuję.",
    ]
    var unitArray = [
        "",
        "Sztuki",
        "Litry",
        "Kilogramy",
    ]
    function resetForm() {
        $("#productName").val("");
        $("#productCode").val(codeOfLastProduct + 1);
        $("#groupType").val("");
        $("#unitType").val("");
        $("textarea").val("");
    }
    function resetOptionInSelect() {
        $('#groupType')
            .empty()
            .append('<option value=""></option')
            ;
    }
    function openModal() {
        $(".background-shadow").css("display", "block");
        $(".center-modal").css("display", "flex");
        $("#productCode").val(codeOfLastProduct + 1);
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
    $("#product-menu").css("background-color", "#eee");
    $("#product-menu").css("color", "#1d2127");

    $(document).on("click", ".btn-modal", function () {

        openModal();
        $("#modal-edit").css("display", "block");

        actionType = $(this).data('type');

        if (actionType == "new") {
            $("#modal-header-product").css("background-color", "#47a447");
            $("#modal-title-product").html("Nowy produkt");
            $("#btn-product").css("background-color", "#47a447");
            $("#btn-product").html("Dodaj produkt");
        } else if (actionType == "edit") {
            $("#modal-header-product").css("background-color", "#de8500");
            $("#modal-title-product").html("Edytuj produkt");
            $("#btn-product").css("background-color", "#de8500");
            $("#btn-product").html("Zapisz zmiany");

            idEditItem = $(this).data('id');

            $.ajax({
                type: "GET",
                url: "/Products/" + idEditItem,
                success: function (data) {
                    $("#productName").val(data[0].name);
                    $("#productCode").val(data[0].code);
                    $("#groupType").val(data[0].groupType);
                    $("#unitType").val(data[0].unitType);
                    $("#productpDescription").val(data[0].description);
                }
            });
        }
    });

    $(".modal-dismiss").click(function () {
        closeModal();
        resetForm();
    });

    $(document).on("click", "#btn-product", function () {

        var productData = JSON.stringify({
            "name": $("#productName").val(),
            "code": $("#productCode").val(),
            "groupType": $("#groupType").val(),
            "unitType": $("#unitType").val(),
            "description": $("#productDescription").val(),
        });

        alert(productData);

        if (actionType == "new") {
            $.ajax({
                type: "POST",
                url: "/Products",
                data: productData,
                dataType: "json",
                contentType: "application/json",
                success: function (data) {
                    alert(productData);
                    if (data == -1) {
                        alert(productData);
                        closeModal();
                        drawTable();
                    } else {
                        alert(productData);
                        $(".error-text").css("display", "block");
                        $(".error-text").html(errorText[data]);
                    }
                }
            });

        } else if (actionType == "edit") {

            $.ajax({
                type: "PUT",
                url: "/Products/" + idEditItem,
                data: productData,
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
    //open information about product
    $(document).on("click", ".item-name", function () {

        var idInformation = $(this).data('id');
        openModal();
        $("#modal-inf").css("display", "block");

        $.ajax({
            type: "GET",
            url: "/Products/" + idInformation,
            success: function (data) {
                var newDateFormat = changeFormatDate(data[0].created);
                $("#product-name-information").html(data[0].name);
                $("#product-created-information").html(newDateFormat);
                $("#product-user-information").html(data[0].userName);
            }
        });

        $("#modal-btn-inf").click(function () {
            closeModal();
        });
    });
    //open delete product modal
    $(document).on("click", ".button-table-delete", function () {

        idDelete = $(this).data('id');
        $("#modal-delete").css("display", "block");
        openModal();

        $("#button-delete").click(function () {
            $.ajax({
                type: "DELETE",
                url: "/Products/" + idDelete,
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
    //take information about name of groups
    function getNameOfGroups() {
        resetOptionInSelect();
        $.getJSON("/Groups", function (data) {
            $.each(data, function (key, value) {
                $('#groupType').append('<option value="' + value.id_group + '">' + value.name + '</option > ');
            });
        });
    }
    //draw table with information about products

    function drawTable() {
        var id = 1;
        var codeOfLastProduct = '';
        var productData;

        $("#product-table > tbody").empty();

        $.getJSON("/Products", function (data) {
            $.each(data, function (key, value) {
                productData += '<tr>';
                productData += '<td>' + id++ + '</td>';
                productData += '<td> <a href="#" class="item-name" data-id="' + value.id_product + '">' + value.name + '</a></td>';
                productData += '<td>' + value.code + '</td>';
                productData += '<td>' + value.group_name + '</td>';
                productData += '<td>' + unitArray[value.unit] + '</td>';
                productData += '<td class="item-description">' + value.description + '</td>';
                productData += '<td><button class="btn-modal" id="button-table-edit" data-type="edit" data-id="' + value.id_product + '">Edycja</button> <button class="button-table-delete"data-id="' + value.id_product + '">Usuń</button></td>';
                productData += '</tr>';
                codeOfLastProduct = value.code;
                codeOfLastProduct++;
            });
            $("#product-table").append(productData);
        });
    }

    drawTable();
    getNameOfGroups();

});