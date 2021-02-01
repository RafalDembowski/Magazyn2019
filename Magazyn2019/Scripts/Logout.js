$(document).ready(function () {


    $("#logoutBtn").click(function () {

        $.ajax({
            url: "logout",
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                location.reload();
            }

        })
    });
});
