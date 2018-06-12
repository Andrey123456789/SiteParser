//Get all products. If domain is not empty, program reads data from it before get all data:
function GetAllProducts(domain = "") {
    console.log("GetAllProducts start");

    $.ajax({
        url: '/api/values/getproducts/?domain=' + domain,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            WriteResponse(data);
        },
        error: function (x, y, z) {
            console.log("WriteResponse failed");
            alert(x + '\n' + y + '\n' + z);
        }
    });
    console.log("GetAllProducts end");
}

// вывод полученных данных на экран
function WriteResponse(products) {
    console.log("WriteResponse start");
    var strResult = "<table class='table table_stripped'><th>Site</th><th>Product</th><th>Price</th><th>Tendency</th><th></th>";
    console.log("products" + products);
    $.each(products, function (index, product) {
        console.log("Site=" + product.Domain);
        console.log("Description=" + product.Description);
        console.log("Price=" + product.Price);
        console.log("Delta=" + product.DeltaPrice);
        console.log("Redirection link:" + $("#RedirectTo").val());
        //Adding triangle sign to describe price change:
        var delta = "" + ((product.DeltaPrice > 0) ? '<div class="green">+' + product.DeltaPrice + '</div>' : "") + ((product.DeltaPrice < 0) ? '<div class="red">' + product.DeltaPrice + '</div>' : "");
        strResult += "<tr><td>" + product.Domain + "</td><td> " + product.Description + "</td><td>" + product.Price + "</td><td>" + delta + "</td><td><a href='" + $("#RedirectTo").val() + '?id=' + product.Id + "' >Show</a></td></tr>";

    });
    strResult += "</table>";
    $("#tableBlock").html(strResult);
    console.log("WriteResponse end");
}

// вывод данных редактируемой книги в поля для редактирования
function ShowProduct(product) {
    console.log("ShowProduct start");
    console.log(product);
    if (product != null) {
        $("#productDescription").append(product.Description);
        $("#productSite").append(product.Domain);
        $("#productPrice").append(product.Price);

        var delta = "" + ((product.DeltaPrice > 0) ? '<div class="green">+' + product.DeltaPrice + '</div>' : "") + ((product.DeltaPrice < 0) ? '<div class="red">' + product.DeltaPrice + '</div>' : "");

        $("#productTendency").append(delta);

        $.each(product.ImagesBase64, function (index, image) {
            if (index == 0) {
                $("#carouselIndicators").append('<li data-target="#myCarousel" data-slide-to="0" class="active"></li>');
                $("#carouselGroup").append('<div class="item active"><img src="data:image/jpeg;base64,' + image + '" align= "right"></div>');
            }
            else {
                $("#carouselIndicators").append('<li data-target="#myCarousel" data-slide-to="' + index + '"></li>');
                $("#carouselGroup").append('<div class="item"><img src="data:image/jpeg;base64,' + image + '" align= "right"></div>');
            }
        });
    }
    else {
        alert("Product doesn't exists!");
    }
    console.log("ShowProduct end");
}

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