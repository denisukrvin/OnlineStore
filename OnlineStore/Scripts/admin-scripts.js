$(document).ready(function () {

    // create newsletter event
    $('#btnCreateNewsletter').click(function (e) {
        e.preventDefault();

        var formData = $('#formCreateNewsletter').serialize();

        // call the send feedback function
        CreateNewsletter(formData);
    });

    function CreateNewsletter(messageData) {

        $("#btnCreateNewsletter").attr('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Зачекайте...');

        $.ajax({
            type: "post",
            url: "/Admin/CreateNewsletter",
            data: messageData,
            success: function (result) {
                if (result.IsCreate === true) {

                    $("#btnCreateNewsletter").attr('disabled', false).html('Створити');
                    $('#createNewsletterResult').attr('style', 'display:block;').attr('class', 'alert alert-success').html(result.Message);

                    window.setTimeout(function () {
                        $('#createNewsletterResult').fadeOut("slow");
                    }, 3000);
                }
                else {

                    $('#createNewsletterResult').attr('style', 'display:block;').attr('class', 'alert alert-danger').html(result.Message);
                    $("#btnCreateNewsletter").attr('disabled', false).html('Створити');

                    window.setTimeout(function () {
                        $('#createNewsletterResult').fadeOut("slow");
                    }, 3000);
                }
            }
        });
    }
});