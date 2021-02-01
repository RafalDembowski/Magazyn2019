$(document).ready(function () {
    var codeOfLastGroup = 2999;
    var idEditItem;
    var idDelete = 0;
    var actionType;
    var errorText = [
        "Uzupełnij poprawnie wszystkie pola.",
        "Podana nazwa już występuję.",
        "Podany kod już występuję.",
    ]
    function resetForm() {
        $("#groupName").val("");
        $("#groupCode").val(codeOfLastGroup + 1);
        $("textarea").val("");
    }
    function openModal() {
        $(".background-shadow").css("display", "block");
        $(".center-modal").css("display", "flex");
        $("#groupCode").val(codeOfLastGroup + 1);
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
    $("#group-menu").css("background-color", "#eee");
    $("#group-menu").css("color", "#1d2127");

    $(document).on("click", ".btn-modal", function () {

        openModal();
        $("#modal-edit").css("display", "block");

        actionType = $(this).data('type');

        if (actionType == "new") {
            $("#modal-header-group").css("background-color", "#47a447");
            $("#modal-title-group").html("Nowa grupa");
            $("#btn-group").css("background-color", "#47a447");
            $("#btn-group").html("Dodaj grupę");
        } else if (actionType == "edit") {
            $("#modal-header-group").css("background-color", "#de8500");
            $("#modal-title-group").html("Edytuj grupe");
            $("#btn-group").css("background-color", "#de8500");
            $("#btn-group").html("Zapisz zmiany");

            idEditItem = $(this).data('id');

            $.ajax({
                type: "GET",
                url: "Groups/" + idEditItem,
                success: function (data) {
                    $("#groupName").val(data[0].name);
                    $("#groupCode").val(data[0].code);
                    $("#groupDescription").val(data[0].description);
                }
            });
        }
    });

    $(".modal-dismiss").click(function () {
        closeModal();
        resetForm();
    });

    $(document).on("click", "#btn-group", function () {

        var groupData = JSON.stringify({
            "code": $("#groupCode").val(),
            "name": $("#groupName").val(),
            "description": $("#groupDescription").val(),
        });

        if (actionType == "new") {
            $.ajax({
                type: "POST",
                url: "Groups",
                data: groupData,
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
                url: "Groups/" + idEditItem,
                data: groupData,
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
    //open information about group
    $(document).on("click", ".item-name", function () {

        var idInformation = $(this).data('id');
        openModal();
        $("#modal-inf").css("display", "block");

        $.ajax({
            type: "GET",
            url: "Groups/" + idInformation,
            success: function (data) {
                var newDateFormat = changeFormatDate(data[0].created);
                $("#group-name-information").html(data[0].name);
                $("#group-created-information").html(newDateFormat);
                $("#group-user-information").html(data[0].userName);
            }
        });

        $("#modal-btn-inf").click(function () {
            closeModal();
        });
    });
    //open delete group modal
    $(document).on("click", ".button-table-delete", function () {

        idDelete = $(this).data('id');
        $("#modal-delete").css("display", "block");
        openModal();

        $("#button-delete").click(function () {
            $.ajax({
                type: "DELETE",
                url: "Groups/" + idDelete,
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

    //draw table with information about groups

    function drawTable() {
        var id = 1;
        var groupData = '';

        $("#group-table > tbody").empty();

        $.getJSON("Groups", function (data) {
            $.each(data, function (key, value) {
                groupData += '<tr>';
                groupData += '<td>' + id++ + '</td>';
                groupData += '<td> <a href="#" class="item-name" data-id="' + value.id_group + '">' + value.name + '</a></td>';
                groupData += '<td>' + value.code + '</td>';
                groupData += '<td class="item-description">' + value.description + '</td>';
                groupData += '<td><button class="btn-modal" id="button-table-edit" data-type="edit" data-id="' + value.id_group + '">Edycja</button> <button class="button-table-delete"data-id="' + value.id_group + '">Usuń</button></td>';
                groupData += '</tr>';
                codeOfLastGroup = value.code;
            });
            $("#group-table").append(groupData);
        });
    }

    drawTable();

});