$(document).ready(function () {

    // admin login event
    $('#btnAdminLogin').click(function (e) {
        e.preventDefault();

        var formData = $('#adminLoginForm').serialize();
        // call the admin login function
        AdminLogin(formData);
    });

    function AdminLogin(adminData) {

        $("#btnAdminLogin").attr('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Перевірка...');

        $.ajax({
            type: "post",
            url: "/Admin/Login",
            data: adminData,
            success: function (result) {
                if (result.IsLogin === true) {
                    location.reload();
                }
                else {

                    grecaptcha.reset();
                    $("#adminLoginForm")[0].reset();

                    $('#adminLoginResult').attr('style', 'display:block;').attr('class', 'alert alert-danger').html(result.Message);
                    $("#btnAdminLogin").attr('disabled', false).html('<i class="fas fa-sign-in-alt"></i> Увійти');

                    window.setTimeout(function () {
                        $('#adminLoginResult').fadeOut("slow");
                    }, 2000);
                }
            }
        });
    }
});