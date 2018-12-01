$(document).ready(function () {

    // send feedback event
    $('#btnSendFeedback').click(function (e) {
        e.preventDefault();

        var formData = $('#formFeedback').serialize();

        // call the send feedback function
        SendFeedback(formData);
    });

    function SendFeedback(userData) {

        $("#btnSendFeedback").attr('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Перевірка...');

        $.ajax({
            type: "post",
            url: "/SendFeedback",
            data: userData,
            success: function (result) {
                if (result.IsSend === true) {

                    $('#formFeedback').attr('style', 'display: none;');
                    $('#formFeedback').after('<div class="alert alert-success" role="alert">' + result.Message + '</div >');
                }
                else {

                    grecaptcha.reset();                    

                    $('#sendFeedbackResult').attr('style', 'display:block;').attr('class', 'alert alert-danger').html(result.Message);
                    $("#btnSendFeedback").attr('disabled', false).html('Відправити');

                    window.setTimeout(function () {
                        $('#sendFeedbackResult').fadeOut("slow");
                    }, 2000);
                }
            }
        });
    }
});