$(document).ready(function () {

    // add comment event
    $('#btnAddComment').click(function (e) {
        e.preventDefault();

        var formData = $('#formComment').serialize();

        // call the add comment function
        AddComment(formData);
    });

    function AddComment(commentData) {

        $("#btnAddComment").attr('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Перевірка...');

        $.ajax({
            type: "post",
            url: "/AddComment",
            data: commentData,
            success: function (result) {
                if (result.IsAdd === true) {
                    location.reload();
                }
                else {

                    grecaptcha.reset();
                    $("#formComment")[0].reset();

                    $('#addCommentResult').attr('style', 'display:block;').attr('class', 'alert alert-danger').html(result.Message);
                    $("#btnAddComment").attr('disabled', false).html('Додати');

                    window.setTimeout(function () {
                        $('#addCommentResult').fadeOut("slow");
                    }, 2000);
                }
            }
        });
    }
});