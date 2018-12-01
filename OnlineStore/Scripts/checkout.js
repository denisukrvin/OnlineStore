$(document).ready(function () {

    $('#btnOpenCart').remove();

    // show the products in the cart and the checkout form
    if (localStorage['chrome']) {

        var totalPrice = 0;
        var productsCount = 0;
        var products = JSON.parse(localStorage.getItem('chrome'));      

        if (products.length !== 0) {

            $('.checkout').attr('style', 'display: block;');

            $.each(products, function (index, value) {

                $.ajax({
                    type: "post",
                    dataType: 'json',
                    url: "/ProductInfo",
                    traditional: true,
                    data: { id: value.id },
                    success: function (result) {
                        if (result.IsGet === true) {
                            $('.checkout-products-list').append('<div class="row align-items-center mb-3"> <div class="col-md-3"> <img src="' + result.ProductInfo.ImagePath + '" height="100" > </div> <div class="col-md-4">' + result.ProductInfo.Name + '</div> <div class="col-md-2">x ' + value.quantity + '</div> <div class="col-md-3">' + result.ProductInfo.Price * value.quantity + ' грн</div> </div>');

                            totalPrice += result.ProductInfo.Price * value.quantity;
                            productsCount += 1;

                            $('.products-count').text(productsCount);
                            $('.checkout-total-price').text(totalPrice + ' грн');
                        }
                        else if (result.IsGet === false) {
                            $('.checkout-empty').append('<p class="lead text-muted">Помилка при завантаженні товарів, спробуйте пізніше...</p>');
                            $('.checkout').attr('style', 'display: none;');

                            return false;
                        }
                    },
                    error: function (errormessage) {
                        alert(errormessage.responseText);
                    }
                });

            });
        }
    }
    // show the message that the cart is empty
    else {
        $('.checkout-empty').append('<p class="lead text-muted">Ваш кошик порожній :(</p>');
    }

    // event when choosing delivery Nova Poshta
    $("#delivery-second").change(function () {

        if (this.checked) {

            $('#city-select').children('option:not(:first)').remove();
            $('#warehouse-select').children('option:not(:first)').remove();          
            $('#warehouse-select').prop('disabled', 'disabled');
            $('#city-select').prop('disabled', 'disabled');
            $('#nova-poshta-info').attr('style', 'display: block;');

            $.ajax({
                type: "post",
                dataType: 'json',
                url: "/GetCities",
                traditional: true,
                success: function (result) {
                    if (result.IsGet === true) {

                        var cities = result.Cities;

                        for (var i = 0; i < cities.length; i++) {
                            $('#city-select').append('<option value=" ' + i + ' ">' + cities[i] + '</option>');
                        }

                        $('#city-select').prop('disabled', false);
                    }
                    else if (result.IsGet === false) {
                        alert('Виникла помилка при завантаженні міст!');
                    }
                },
                error: function (errormessage) {
                    alert(errormessage.responseText);
                }
            });
        }
    });

    // event when choosing the city of delivery
    $("#city-select").change(function () {

        if ($(this).val() === 'null') {

            $('#warehouse-select').prop('disabled', 'disabled');
            $('#warehouse-select').children('option:not(:first)').remove();
        }
        else {

            $('#warehouse-select').children('option:not(:first)').remove();
            $('#warehouse-select').prop('disabled', true);

            var selectedCity = $('#city-select').find(":selected").text();

            $.ajax({
                type: "post",
                dataType: 'json',
                url: "/GetWarehouses",
                traditional: true,
                data: { city: selectedCity },
                success: function (result) {
                    if (result.IsGet === true) {

                        var warehouses = result.Warehouses;

                        for (var i = 0; i < warehouses.length; i++) {
                            $('#warehouse-select').append('<option value=" ' + i + ' ">' + warehouses[i] + '</option>');
                        }

                        $('#warehouse-select').prop('disabled', false);
                    }
                    else if (result.IsGet === false) {
                        alert('Виникла помилка при завантаженні міст!');
                    }
                },
                error: function (errormessage) {
                    alert(errormessage.responseText);
                }
            });
        }
    });

    // event when choosing delivery Pickup
    $("#delivery-first").change(function () {
        if (this.checked) {
            $('#nova-poshta-info').attr('style', 'display: none;');
        }
    });  

    // checkout event
    $('#btnCheckout').click(function () {      

        $("#btnCheckout").attr('disabled', true).text('').append('<i class="fas fa-spinner fa-spin"></i>');
        $("#formCheckout :input").prop("disabled", true);
        $('#checkout-result').attr('style', 'display:none;').html('');

        if (localStorage['chrome']) {

            var userOrderData =
            {
                Name: $("#formCheckout input[name = userName]").val(),
                Phone: $("#formCheckout input[name = userPhone]").val(),
                Email: $("#formCheckout input[name = userEmail]").val(),
                PIB: $("#formCheckout input[name = PIB]").val(),
                Delivery: $('input[name=delivery]:checked', '#formCheckout').val(),
                Payment: $('input[name=payment]:checked', '#formCheckout').val(),
                DeliveryCity: $('#city-select option:selected').text(),
                DeliveryWarehouse: $('#warehouse-select option:selected').text()
            };

            var products = JSON.parse(localStorage.getItem('chrome'));
            var userDataToSend = { productsList: products, order: userOrderData };

            $.ajax({
                type: "post",
                url: "/Checkout",
                data: JSON.stringify(userDataToSend),
                contentType: "application/json;charset=utf-8",
                success: function (result) {
                    if (result.IsGet === true) {
                        localStorage.removeItem('chrome');
                        $('.checkout').attr('style', 'display: none;');
                        $('.checkout-empty').after('<div class="alert alert-success" role="alert">' + result.Message + '</div >');
                    }
                    else if (result.IsGet === false) {
                        $("#btnCheckout").attr('disabled', false).text('Замовити');
                        $("#formCheckout :input").prop("disabled", false);
                        $('#checkout-result').attr('style', 'display:block;').attr('class', 'alert alert-danger').html(result.Message);
                        window.setTimeout(function () {
                            $('#checkout-result').fadeOut("slow");
                        }, 3000);
                    }
                },
                error: function (errormessage) {
                    alert(errormessage.responseText);
                }
            });
        }
        else {

            $("#btnCheckout").attr('disabled', false).text('Замовити');
            $("#formCheckout :input").prop("disabled", false);

            $('#checkout-result').attr('style', 'display:block;').attr('class', 'alert alert-danger').html('Ваш кошик порожній!');
            window.setTimeout(function () {
                $('#checkout-result').fadeOut("slow");
            }, 2000);
        }
    });
});