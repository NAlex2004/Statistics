function delClick(id) {
    var element = $("#item_".concat(id));

    if (confirm("Delete this sale?")) {
        $.ajax({
            type: "GET",
            url: "/Home/DeleteSale/".concat(id),
            complete: function (jqXHR, status) {
                if (status == 'success' || status == 'notmodified') {
                    var result = $.parseJSON(jqXHR.responseText);
                    if (result == "true") {
                        var sum = parseFloat($("#totalText_".concat(id)).text().replace(",", ".").replace(/\s+/g, ''));
                        var total = parseFloat($("#total").text().replace(',', '.'));
                        total = total - sum;
                        $("#total").text(total.toFixed(2));
                        element.empty();
                    }
                }
            }
        });
    };

    return false;
}