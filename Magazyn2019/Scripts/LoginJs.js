$(document).ready(function () {


    $("#loginBtn").click(function (e) {

        e.preventDefault();

        var dataUser = JSON.stringify({
            "login_txt": $("#login").val(),
            "password_txt": $("#password").val()
        });

        $.ajax({
            url: "login",
            type: "POST",
            data: dataUser,
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                if (data == 1) {
                    window.location.href = window.location.pathname = '/HomePage';
                } else {
                    $("#login-form-error").css("display", "block");
                }
            }

        })
    });
});
