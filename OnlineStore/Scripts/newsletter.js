$(document).ready(function () {

    // subscribe to newsletter event
    $('#btn-newsletter').click(function (e) {
        e.preventDefault();

        console.log('Нажали на кнопку подписки на новости');

        var userData = $('#email-newsletter').val();

        console.log('Получили данные с инпута: ' + userData);

        // call the subscribe to newsletter function
        SubscribeToNewsletter(userData);
    });

    function SubscribeToNewsletter(userEmail) {

        $("#btn-newsletter").attr('disabled', true).html('<i class="fas fa-spinner fa-spin"></i>');

        $.ajax({
            type: "post",
            url: "/SubscribeToNewsletter",
            data: { userEmail: userEmail },
            success: function (result) {
                if (result.IsSubscribe === true) {

                    $('#result-newsletter').attr('style', 'display: block; ').attr('class', 'text-success').html(result.Message);
                    $("#btn-newsletter").attr('disabled', false).html('<i class="fas fa-envelope"></i>');

                    window.setTimeout(function () {
                        $('#result-newsletter').fadeOut("slow");
                    }, 3000);
                }
                else {                   

                    $('#result-newsletter').attr('style', 'display: block; ').attr('class', 'text-danger').html(result.Message);
                    $("#btn-newsletter").attr('disabled', false).html('<i class="fas fa-envelope"></i>');

                    window.setTimeout(function () {
                        $('#result-newsletter').fadeOut("slow");
                    }, 3000);
                }
            }
        });
    }
});