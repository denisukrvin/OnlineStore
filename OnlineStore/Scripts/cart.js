$(document).ready(function () {

    // add to cart event
    $('#btnAddToCart').click(function (e) {
        e.preventDefault();

        var productId = $(this).attr('data-id');
        var productQuantity = $('.product-quantity').text();

        // call the add to cart function
        AddToCart(productId, productQuantity);      
    });

    function AddToCart(productId, productQuantity) {

        var productData = { id: productId, quantity: productQuantity };

        if (localStorage['chrome']) {

            var exist = false;
            var products = JSON.parse(localStorage.getItem('chrome'));

            // check if there is current product in the cart
            $.each(products, function (index, value) {

                if (value.id === productId) {

                    exist = true;

                    $('#alertAddToCart').attr('style', 'display: block;').attr('class', 'alert alert-danger').html('Товар уже в кошику!');
                    window.setTimeout(function () {
                        $('#alertAddToCart').fadeOut("slow");
                    }, 2000);
                 
                    return false;
                }
            });

            if (!exist) {

                products.push(productData);                                 
                localStorage.setItem('chrome', JSON.stringify(products));                 

                $('#alertAddToCart').attr('style', 'display: block;').attr('class', 'alert alert-success').html('Товар успішно доданий в кошик!');
                window.setTimeout(function () {
                    $('#alertAddToCart').fadeOut("slow");
                }, 2000);
            }     
        }
        else {

            var productsList = [];                                              
            productsList.push(productData);                                 
            localStorage.setItem('chrome', JSON.stringify(productsList));

            $('#alertAddToCart').attr('style', 'display: block;').attr('class', 'alert alert-success').html('Товар успішно доданий в кошик!');
            window.setTimeout(function () {
                $('#alertAddToCart').fadeOut("slow");
            }, 2000);

        }
    }

    // cart open event
    $('#btnOpenCart').click(function (e) {
        e.preventDefault();

        //call the cart open function
        OpenCart();       
    });

    function OpenCart() {

        $('.cart-info').empty();

        if (localStorage['chrome']) {

            var products = JSON.parse(localStorage.getItem('chrome'));
           
            if (products.length !== 0) {

                $('.cart-info').append('<div class="container-fluid cart-products">');

                // send the id of the products and get its information
                $.each(products, function (index, value) {

                    $.ajax({
                        type: "post",
                        dataType: 'json',
                        url: "/ProductInfo",
                        traditional: true,
                        data: { id: value.id },
                        success: function (result) {
                            if (result.IsGet === true) {
                                $('.cart-products').append('<div class="row align-items-center mb-3" data-id="' + result.ProductInfo.Id + '"> <div class="col-3"> <img src="' + result.ProductInfo.ImagePath + '" height="100" > </div> <div class="col-5">' + result.ProductInfo.Name + '</div> <div class="col-1">x ' + value.quantity + '</div> <div class="col-2">' + result.ProductInfo.Price * value.quantity + ' грн</div> <div class="col-1"><button type="button" class="btn btn-link remove-cart-item"><i class="fas fa-times"></i></button></div> </div>');
                            }
                            else if (result.IsGet === false) {
                                $('.cart-info').append('<p>Виникла помилка при завантаженні товару, спробуйте пізніше...</p>');
                                $('#btnGoToCheckout').attr('style', 'display: none;');

                                return false;
                            }
                        },
                        error: function (errormessage) {
                            alert(errormessage.responseText);
                        }
                    });
                });

                $('#btnGoToCheckout').attr('style', 'display: block;');
            }
        }      
        else {

            $('.cart-info').append('<p>Ваш кошик порожній <br> Але це ніколи не пізно виправити :)</p>');
            $('#btnGoToCheckout').attr('style', 'display: none;');
        }
    }

    // remove cart product event
    $(document).on('click', '.remove-cart-item', function () {

        var productRow = $(this).parents().get(1);
        var productId = $(productRow).attr('data-id');

        // call the function of removing product from the cart
        RemoveCartProduct(productId, productRow);       
    });

    function RemoveCartProduct(productId, productRow) {
       
        // hide the product row from cart
        $(productRow).hide('slow', function () { $(productId).remove(); }); 

        if (localStorage['chrome']) {

            var products = JSON.parse(localStorage.getItem('chrome'));           

            // remove the product
            products = $.grep(products, function (value) {

                if (value.id === productId) {
                    return false;
                } else {
                    return true;
                }
            });

            // update local storage
            localStorage.setItem('chrome', JSON.stringify(products));

            // check or this product was last
            if (products.length === 0) {                

                localStorage.removeItem('chrome');

                $('.cart-info').append('<p>Ваш кошик порожній <br> Але це ніколи не пізно виправити :)</p>');
                $('#btnGoToCheckout').attr('style', 'display: none;');
            }
        }
    }
    
    // event of reducing the amount of product
    $(document).on('click', '.minus', function () {

        ChangeProductQuantity('minus');
    });

    // event of increasing the amount of product
    $(document).on('click', '.plus', function () {

        ChangeProductQuantity('plus');
    });

    function ChangeProductQuantity(operation) {

        var quantity = $('.product-quantity').text();

        if (operation === 'minus') {

            if (quantity > 1) {

                quantity--;
                $('.product-quantity').text(quantity);
            }
        }
        else if (operation === 'plus') {

            if (quantity < 9) {

                quantity++;
                $('.product-quantity').text(quantity);
            }
        }
    }

});