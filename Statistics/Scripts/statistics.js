function statisticsView(data, targetId, chartId, isAdmin) {
    var chartSelected = $('input[name="chartRadio"]:checked').val();

    var html = "<h2>Sales</h2> \n";
    if (isAdmin)
        html += "<p><a href='/Home/CreateSale'>Add New sale</a></p> \n";
    html += '<table class="table"> \n'
        + '<tbody><tr> \n'
        + '<th>Sale date</th> \n'
        + '<th>Manager</th> \n'
        + '<th>Customer</th> \n'
        + '<th>Product</th> \n'
        + '<th>Total</th> \n'
        + '<th></th> \n'
        + '</tr> \n';

    var total = 0.0;

    var chartArray = [[chartSelected, "Total"]];
    var chartMap = new Map();
    var itemKey;

    for (var i = 0; i < data.length; i++) {
        var item = data[i];
                
        if (chartSelected == 'SaleDate')
            itemKey = moment(item.SaleDate).format('DD.MM.YYYY');
        else
            itemKey = item[chartSelected].toString();

        var mapVal = 0;
        if (chartMap.has(itemKey)) 
            mapVal = chartMap.get(itemKey);
        chartMap.set(itemKey, mapVal + parseFloat(item.Total));

        total += item.Total;
        html += '<tr id="item_'.concat(item.Id).concat('">\n');        
        html += '<td>' + moment(item.SaleDate).format('DD.MM.YYYY') + '</td>\n';
        html += '<td>' + item.Manager + '</td>\n';
        html += '<td>' + item.Customer + '</td>\n';
        html += '<td>' + item.Product + '</td>\n';
        html += '<td id="totalText_' + item.Id + '">' + item.Total.toFixed(2) + '</td>\n';

        html += '<td>\n';
        html += '<input data-val="true" id="Id" name="Id" value="' + item.Id + '" type="hidden"> \n';
        if (isAdmin) {
            html += '<a data-ajax="true" data-ajax-method="GET" data-ajax-mode="replace" data-ajax-update="#item_' + item.Id + '" data-ajax-url="/Home/EditSale/' + item.Id + '" href="/Home/EditSale/' + item.Id + '">Edit</a> |';
            html += '<a href="/Home/DeleteSale/' + item.Id + '" id="delLink_' + item.Id + '" onclick="return delClick(' + item.Id + ');">Delete</a>';
        }        
        html += '</td>\n';
        html += '</tr>\n';
    }

    html += '<tr><td></td><td></td><td></td>\n';      
    html += '<td style="text-align:right; font-weight: bold">Total:</td>\n';
    html += '<td id="total" style="font-weight:bold">' + total.toFixed(2) + '</td></tr>';
    html += '</tbody></table>';
    html += '<a class="btn btn-default" href="/">Back</a>';

    $(targetId).empty;
    $(targetId).html(html);
    
    // chart  

    chartMap.forEach(function (value, key, map) {
        chartArray.push([key, value]);
    });

    google.charts.load('current', { 'packages': ['corechart'] });
    google.charts.setOnLoadCallback(function () {
        drawChart(chartArray, chartId);
    });
}

function drawChart(data, chartId) {
    var chartData = google.visualization.arrayToDataTable(data);
    var options = { 'title': 'Sales chart', 'height': 400 };
    var chart = new google.visualization.PieChart(document.getElementById(chartId));
    
    chart.draw(chartData, options);
}
 