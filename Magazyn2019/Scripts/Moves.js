$(document).ready(function () {

    var url = window.location.pathname;
    var typeMovement = url.substring(url.lastIndexOf('/') + 1);

    function setCorrectTextToMove() {

        if (typeMovement == 2) {
            $('#name-of-movement').text('Nowe wydanie');
            $('#btn-movement').text('nowe wydanie');
        }
        if (typeMovement == 3) {
            $('#first-select').find("label[for =warehouse-one]").text("Magazyn wyjściowy");
            $('#second-select').find("label[for =warehouse-second]").text("Magazyn wejściowy");
            $('#second-select').find("select").attr("id", "warehouses-filter-two");


            $('#name-of-movement').text('Nowe przesunięcie');
            $('#btn-movement').text('nowe przesunięcie');
        }
    }

    var productAmount = 0;
    var numberRow = 1; 
    var movementData = null;
    var errorTrue = 1; 

    var unitArray = [
        "",
        "Sztuki",
        "Litry",
        "Kilogramy",
    ]
    $("#states-menu").css("background-color", "#eee");
    $("#states-menu").css("color", "#1d2127");

    function openModal() {
        $(".background-shadow").css("display", "block");
        $(".center-modal").css("display", "flex");
        $(".modal").css("display", "block");
    }
    function closeModal() {
        $(".background-shadow").css("display", "none");
        $(".center-modal").css("display", "none");
        $(".modal").css("display", "none");
    }

    $("#btn-close").click(function () {
        closeModal();
        location.reload();
    });

    $(".modal-dismiss").click(function () {
        closeModal();
        location.reload();
    });

    //take information about products in choosen warehouse
    function getNameOfGroupsToSelect() {

        var idWarehouse = $("#warehouses-filter").val();

        $.getJSON("/AllProductsForWarehouse/" + idWarehouse, function (data) {
            $.each(data, function (key, value) {
                $("#products-filter-movement").append('<option value="' + value.id_product + '">(' + value.code + ') ' + value.name + '</option > ');
            });
        });
    }
    function getAllActiveProductstoSelect() {
        $.getJSON("/Products" , function (data) {
            $.each(data, function (key, value) {
                $("#products-filter-movement").append('<option value="' + value.id_product + '">(' + value.code + ') ' + value.name + '</option > ');
            });
        });
    }
    function getAllWarehousesToSelect() {
        $.getJSON("/Warehouses", function (data) {
            $.each(data, function (key, value) {
                $('#warehouses-filter').append('<option value="' + value.id_warehouse + '">' + value.name + '</option > ');
                if (typeMovement == 3) {
                    $('#warehouses-filter-two').append('<option value="' + value.id_warehouse + '">' + value.name + '</option > ');
                }
            });
        });
    }

    function getAllCustomersToSelect() {
        $.getJSON("/Customers", function (data) {
            $.each(data, function (key, value) {
                $('#customers-filter').append('<option value="' + value.id_customer + '">' + value.name + '</option > ');
            });
        });
    }

    function checkProductAmount(productDataProbne) {


        var i = 1;
        productAmount = 0;
        isMovePossibility = 0;
        productData = JSON.parse(productDataProbne);

        $.getJSON("/Inventories/" + productData.warehouse, function (data) {


            if (productData.product != data.id_product && typeMovement != 1) {
                $(".error-text-second").css("display", "block");
                errorTrue = 1;
            }
           
            $.each(data, function (key, value) {
                if (value.id_product == productData.product) {

                    productAmount = value.amount;
                        if(productData.amount > 0){
                            $(".error-text-fourth").css("display", "none");
                            errorTrue = 0;
                        }
                        if (productData.amount <= 0) {
                        $(".error-text-fourth").css("display", "block");
                        errorTrue = 1;
                        }
                        else {
                            if (typeMovement != 1) {
                                if (productData.amount > productAmount) {
                                    $(".error-text-second").css("display", "block");
                                    errorTrue = 1;
                                }
                                else {
                                    $(".error-text-second").css("display", "none");
                                    errorTrue = 0;
                                }
                            }
                    }
                }
            });

        });
 
    }

    function optionValidation() {

        errorTrue = 0;

        var date = $("#date-picker").val();
        var warehousesFilter = $("#warehouses-filter").val();
        var customersFilter = $("#customers-filter").val();
        var spinner = $("#spinner").val();
        var productsFilterMovement = $("#products-filter-movement").val();


        var optionArray = [
            date,
            warehousesFilter,
            customersFilter,
            spinner,
            productsFilterMovement,
        ]

        optionArray.forEach(isOptionNull);
    } 

    function isOptionNull(item) {
        if (!Boolean(item)) {
            errorTrue = 1;
            $(".error-text").css("display", "block");
        }
    }

    //typeof $(idItem).value === 'undefined'
    function displayProductToMove(productData) {

        var productObject = JSON.parse(productData);
        var tableMoveData = '';

        if (productObject.amount > 0) {
            $.getJSON("/ProductsWithNoActive/" + productObject.product, function (data) {
                $("#container-table").css("display", "block");
                if ($('#moves-table').find('tr#' + productObject.product).find('td:eq(1)').html() === data[0].name) {
                    $('#moves-table').find('tr#' + productObject.product).find('td:eq(5)').html(productObject.amount);
                } else {
                    tableMoveData += '<tr id="' + productObject.product + '">';
                    tableMoveData += '<td>' + numberRow++ + '</td>';
                    tableMoveData += '<td>' + data[0].name + '</a></td>';
                    tableMoveData += '<td>' + data[0].code + '</td>';
                    tableMoveData += '<td>' + unitArray[data[0].unit] + '</td>';
                    tableMoveData += '<td>' + productAmount + '</td>';
                    tableMoveData += '<td>' + productObject.amount + '</td>';
                    tableMoveData += '<td><button class="button-table-delete"data-id="' + productObject.product + '">Usuń</button></td>';
                    tableMoveData += '</tr>';
                    $("#moves-table").append(tableMoveData);
                }
            });
        }

    }
    function checkIfWarehousesAreDifferent(typeMove) {

        var warehousesFilterOne = $("#warehouses-filter").val();
        var warehousesFilterTwo = $("#warehouses-filter-two").val();

        if (typeMove == 3 && warehousesFilterOne === warehousesFilterTwo) {
            $(".error-text-third").css("display", "block");
            errorTrue = 1;
        }
        else {
            $(".error-text-third").css("display", "none");
            errorTrue = 0;
        }
    }
    //adding new product to the table
    $("#btn-movement-add").click(function () {
        if ($('#products-filter-movement').is(':enabled'))
        {

        //disabled all filter 
        optionValidation();
        checkIfWarehousesAreDifferent(typeMovement);

        if (errorTrue == 0) {

            $(".error-text").css("display", "none");
            $("#date-picker").prop("disabled", "disabled");
            $("#warehouses-filter").prop("disabled", "disabled");
            $("#warehouses-filter-two").prop("disabled", "disabled");
            $("#customers-filter").prop("disabled", "disabled");


            var productData = JSON.stringify({
                "warehouse": $("#warehouses-filter").val(),
                "product": $("#products-filter-movement").val(),
                "amount": $("#spinner").val(),
            })

            $.when(checkProductAmount(productData)).done(function () {
                displayProductToMove(productData);
            });
        }

        }


    });

    //delete row from table
    $(document).on("click", ".button-table-delete", function () {


        $removeRow = $(this).data('id');
        $('#' + $removeRow).remove();
        numberRow--;

        checkIfMoveIsPossible();

        //refactor id of product
        $('#moves-table').find('tbody').find('tr').each(function (index) {
            var firstTdElement = $(this).find('td')[0];
            var $firstTDObject = $(firstTdElement);
            $firstTDObject.text(index + 1);
        });


        if (numberRow == 1) {
            $("#container-table").css("display", "none");
        }

        


    });
    //check if move is possible 
    function checkIfMoveIsPossible() {

        var amountInWarehouse;
        var outAmount;
        var notEnoughProduct = false;

            $('#moves-table').find('tbody').find('tr').each(function () {
                outAmount = $(this).find('td:eq(5)').html();
                amountInWarehouse = $(this).find('td:eq(4)').html();


                if (typeMovement == 3 && outAmount > amountInWarehouse) {
                    notEnoughProduct = true;
                } 

            });
        
        if (notEnoughProduct == false) {
                $(".error-text-second").css("display", "none");
            
        }
    }

    //add new move
    $("#btn-movement").click(function () {

        var i = 0;

            movementData = {
                "date": $("#date-picker").val(),
                "warehouseOne": $("#warehouses-filter").val(),
                "customer": $("#customers-filter").val(),
                "type": typeMovement,
                "products": [],
            };
            $('#moves-table').find('tbody').find('tr').each(function () {
                var productName = $(this).find('td:eq(1)').html();
                var amount = $(this).find('td:eq(5)').html();
                var idProduct = $(this).attr('id');

                movementData['products'].push({ "productId": idProduct, "productName": productName, "amount": amount });
            });


        if (typeMovement == 3) {

            var warehouseTwo = $("#warehouses-filter-two").val();
            movementData['customer'] = warehouseTwo;
            alert(JSON.stringify(movementData));
        }
        
        var movementDataJson = JSON.stringify(movementData);

        if (Array.isArray(movementData.products) && movementData.products.length === 0)
        {
            errorTrue = 1;
            $(".error-text").css("display", "block");
        }


        if (errorTrue == 0) {
            $(".error-text").css("display", "none");
            $.ajax({
                type: "POST",
                url: "/Moves",
                data: movementDataJson,
                dataType: "json",
                contentType: "application/json",
                success: function (data) {
                    setInformationAboutMoveToModal();
                    openModal();
                }
            })
            movementData = null;
        }else {
            $(".error-text").css("display", "block");
        }
    });


    function setInformationAboutMoveToModal() {
        var customerName = $("#warehouses-filter").text();
        var warehouseName = $("#customers-filter").text();
        $("#customer-name").text(customerName);
        $("#warehouse-name").text(warehouseName);
    }
    //chack which option is selected
    $('#warehouses-filter').on("change", function () {
        $("#products-filter-movement option").remove();
        if (typeMovement == 1) {
            getAllActiveProductstoSelect();
        }
        if (typeMovement == 2) {
            getNameOfGroupsToSelect();
        }
        if (typeMovement == 3) {
            getNameOfGroupsToSelect();
        }
        checkIfSelectsAreChoosen();

    });
    $('#date-picker').on("change", function () {
        checkIfSelectsAreChoosen();
    });
    $('#customers-filter').on("change", function () {
        checkIfSelectsAreChoosen();
    });
    //check if all select are choosen before add product
    function checkIfSelectsAreChoosen() {
        var date = $("#date-picker").val();
        var warehousesFilter = $("#warehouses-filter").val();
        if (typeMovement == 1 || typeMovement == 2) {

            var customersFilter = $("#customers-filter").val()

            if (date != "" && warehousesFilter != "" && customersFilter != "") {
                $("#products-filter-movement").prop("disabled", false);
            }
            if (date == "" && warehousesFilter == "" && customersFilter == "" || date == "" || warehousesFilter == "" || customersFilter == "") {
                $("#products-filter-movement").prop("disabled", "disabled");
            }
        }
        if (typeMovement == 3) {

            var warehousesFilterTwo = $("#warehouses-filter-two").val()

            if (date != "" && warehousesFilter != "" && warehousesFilterTwo != "") {
                $("#products-filter-movement").prop("disabled", false);
            }
            if (date == "" && warehousesFilter == "" && warehousesFilterTwo == "" || date == "" || warehousesFilter == "" || warehousesFilterTwo == "") {
                $("#products-filter-movement").prop("disabled", "disabled");
            }
        }

    }

    //script for spinner
    $(function () {
        var spinner = $("#spinner").spinner({
            min: 1
        });
    });

    //script for datepicker
    $(function () {
        $("#date-picker").datepicker();
    });

    setCorrectTextToMove();
    getAllWarehousesToSelect();
    //getNameOfGroupsToSelect();
    checkIfSelectsAreChoosen();
    getAllCustomersToSelect();


});
