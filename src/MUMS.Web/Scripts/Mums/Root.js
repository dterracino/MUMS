/// <reference path="../lib/jquery-vsdoc.js" />
/// <reference path="00-Canvas.js" />
/// <reference path="00-Format.js" />
/// <reference path="00-UTorrent.js" />
/// <reference path="01-Knockout.js" />

if (window.Mums == undefined)
    window.Mums = {};

if (window.Mums.Root == undefined)
    window.Mums.Root = {};


Mums.Root.Init = function () {
    $(document).ready(Mums.Root.Ready);
}

Mums.Root.Ready = function () {
    Mums.Root.InitGlobalKeys();
    Mums.Root.InitTools();
    Mums.Root.StartPolling();
}

Mums.Root.StartPolling = function () {
    Mums.Root.Ajax('/Root/GetTorrents/', {}, function (data) {
        Mums.Knockout.UpdateModel(data);
        setTimeout("Mums.Root.StartPolling()", 3000);
    });
}

Mums.Root.ShowError = function (header, message) {
    // Building the error div dynamically will prevent loading 
    // the vader.jpg until it is actually needed.
    if ($('#error').length == 0) {
        $('<div />')
            .attr('id', 'error')
            .append('<h5 />')
            .append('<div />')
            .insertAfter('#overlay')
            .append($('<a />')
                .addClass('awesome')
                .attr('href', './')
                .text('Ladda om'));
    }

    $('#error h5:first').text(header);
    $('#error div:first').html(message);
    $('#overlay, #error').fadeIn('fast');
}

Mums.Root.Ajax = function (url, data, callback) {
    $.post(url, data, function (responseData) {
        if (responseData.Ok === false) {
            console.log(responseData);
            Mums.Root.ShowError("Kunde inte hämta data", "<p><strong>Url: </strong>" + url + "</p><p><strong>ErrorMessage:</strong> " + responseData.ErrorMessage + "</p>");
        } else {
            callback(responseData);
        }
    }).error(function (jqXHR, errorType, excObj) {
        console.log(jqXHR);
        console.log(errorType);
        console.log(excObj);

        Mums.Root.ShowError("XHR error",
              "<strong>jqXHR status:</strong> " + jqXHR.status
            + "<br /><strong>errorType:</strong> " + errorType
            + "<br /><strong>excObj:</strong> " + excObj
        );
    });
}