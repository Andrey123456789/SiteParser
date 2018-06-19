//Get all products. If domain is not empty, program reads data from it before get all data
function GetAllProducts(pdp) {
    $('#parseDomain').prop('disabled', true);
    console.log("GetAllProducts start");
    console.log(pdp);
    var spdp = JSON.stringify(pdp);
    console.log("spdp=" + spdp);
    $.ajax({
        url: '/api/values/getproducts',
        cache: false,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: spdp,
        dataType: 'json',
        success: function (data) {
            $('#parseDomain').prop('disabled', false);
            $('#domainInput').val("");
            WriteResponse(data);
        },
        error: function (x, y, z) {
            $('#parseDomain').prop('disabled', false);
            $('#domainInput').val("");
            console.log("WriteResponse failed");
            alert(x + '\n' + y + '\n' + z);
        }
    });
    console.log("GetAllProducts end");
}

//Writing a response to page
function WriteResponse(products) {
    console.log("WriteResponse start");
    var strResult = "<table class='table table_stripped'><th>Site</th><th>Product</th><th>Price</th><th>Tendency</th><th></th>";
    console.log("products" + products);
    $.each(products, function (index, product) {
        console.log("Site=" + product.Domain);
        console.log("Description=" + product.Description);
        console.log("Price=" + product.Price);
        console.log("Delta=" + product.DeltaPrice);
        console.log("Currency=" + product.Currency.Short);
        console.log("Redirection link:" + $("#RedirectTo").val());
        //Adding triangle sign to describe price change:
        var delta = "" + ((product.DeltaPrice > 0) ? '<div class="green">+' + product.DeltaPrice + '</div>' : "") + ((product.DeltaPrice < 0) ? '<div class="red">' + product.DeltaPrice + '</div>' : "");
        strResult += "<tr><td>" + product.Domain + "</td><td> " + product.Description + "</td><td>" + product.Price +" " +product.Currency.Short + "</td><td>" + delta + "</td><td><a href='" + $("#RedirectTo").val() + '?id=' + product.Id + "' >Show</a></td></tr>";

    });
    strResult += "</table>";
    $("#tableBlock").html(strResult);
    console.log("WriteResponse end");
}

//Show information about single product
function ShowProduct(product) {
    console.log("ShowProduct start");
    console.log(product);
    if (product != null) {
        $("#productDescription").append(product.Description);
        $("#productSite").append(product.Domain);
        $("#productPrice").append(product.Price + " " + product.Currency.Short);

        var delta = product.DeltaPrice;

        if (delta < 0) {
            $("#productTendency").addClass("red");
        }

        if (delta > 0) {
            $("#productTendency").addClass("green");
            delta = "+" + delta;
        }

        $("#productTendency").append(delta);

        $.each(product.ImagesBase64, function (index, image) {
            //if (index == 0) {
            //    //$("#carouselIndicators").append('<li data-target="#myCarousel" data-slide-to="0" class="active"></li>');
            //    $("#carouselGroup").append('<div class="item active" align="center"><img src="data:image/jpeg;base64,' + image + '"></div>');
            //}
            //else {
                //$("#carouselIndicators").append('<li data-target="#myCarousel" data-slide-to="' + index + '"></li>');
            $("#carouselGroup").append('<div class="item imageDiv" align="center"><img class="imageBorder" src="data:image/' + image + '"></div>');
            $("#carouselIndicators").append('<li data-target="#myCarousel" data-slide-to="' + index + '"></li>');
            //}
        });

        $('.item').first().addClass('active');
        $('.carousel-indicators > li').first().addClass('active');
        $('.carousel').carousel();
    }
    else {
        alert("Product doesn't exists!");
    }
    console.log("ShowProduct end");
}

//Get data for single product
function GetProduct(id) {
    console.log("GetProduct start");
    $.ajax({
        url: '/api/values/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            ShowProduct(data);
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
    console.log("GetProduct end");
}