/// <reference path="../lib/jquery-vsdoc.js" />
/// <reference path="Canvas.js" />
/// <reference path="Format.js" />
/// <reference path="Knockout.js" />
/// <reference path="UTorrent.js" />

if (window.Mums == undefined)
    window.Mums = {};

if (window.Mums.Root == undefined)
    window.Mums.Root = {};


Mums.Root.Init = function () {
    $(document).ready(Mums.Root.Ready);
}

Mums.Root.Ready = function () {
    $('#txtAddFile').change(function () {
        var $wrap = $(this).closest('.addWrapper').find('.labelWrapper').show();
        $(document).one('mousedown', function () { $wrap.hide(); });
    });

    $('#txtAddUrl').focus(function () {
        var $wrap = $(this).closest('.addWrapper').find('.labelWrapper').show();
        $(document).one('mousedown', function () { $wrap.hide(); });
    });

    $('.addWrapper a').click(function (e) {
        $(this).closest('form').find(':submit').click();
        e.preventDefault();
        return false;
    });

    $('.labelWrapper').mousedown(function (e) {
        e.stopPropagation();
    });

    $('#set-label-link').mousedown(function (e) {
        if ($('#set-label-wrapper').is(':visible')) {
            $('#set-label-wrapper').hide();
        } else {
            Mums.Root.Ajax('/Root/GetTorrent/', { hash: $('#hdnHash').val() }, function (data) {
                var label = data.Label || "";
                $('#set-label-wrapper').show();
                $('#set-label-select')
                    .val(label)
                    .focus();
            });
        }

        e.preventDefault();
        e.stopPropagation();
        return false;
    });

    $('#set-label-wrapper a').click(function (e) {
        var lbl = $('#set-label-select').val();
        Mums.Root.Ajax('/Root/SetLabel/', { newLabel: lbl, hash: $('#hdnHash').val() }, function (data) {
            
        });

        Mums.Root.HideTorrentActions();
        e.preventDefault();
        e.stopPropagation();
        return false;
    });

    $('#set-label-wrapper').mousedown(function (e) {
        e.stopPropagation();
    });

    $('#torrent-actions a')
        .not('#set-label-link')
        .not('#set-label-wrapper a')
        .mousedown(function (e) {
            Mums.Root.Ajax($(this).attr('href'), { hash: $('#hdnHash').val() }, function () {
                Mums.Root.HideTorrentActions();
            });

            e.preventDefault();
            e.stopPropagation();
            return false;
        });

    $('#torrent-actions a').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        return false;
    });

    Mums.Knockout.Init();
    Mums.Root.InitTools();
    Mums.Root.StartPolling();
}

Mums.Root.InitTools = function () {
    $('#tools a').click(function (e) {
        var self = $(this);
        
        if (confirm("Är du säker?")) {
            Mums.Root.Ajax(self.attr('href'), {}, function () {
                self.fadeOut('fast') 
            });
        }

        e.preventDefault();
        return false;
    });
}

Mums.Root.StartPolling = function () {
    Mums.Root.Ajax('/Root/GetTorrents/', {}, function (data) {
        Mums.Knockout.UpdateModel(data);
        setTimeout("Mums.Root.StartPolling()", 3000);
    });
}

Mums.Root.TorrentActions = function (hash) {
    var $box = $('.percentage :hidden[value=' + hash + ']').closest('.percentage');
    var pos = $box.position();
    var left = $box.next('.title').find('.label:first').position().left;
    var top = pos.top + $box.height() - $('#torrent-actions').height() - 5;

    $('#torrent-actions')
        .css('left', left)
        .css('top', top)
        .fadeIn('fast');

    $('#hdnHash').val(hash);

    $(document).one('mousedown', function () {
        Mums.Root.HideTorrentActions();
    });

    return false;
}

Mums.Root.HideTorrentActions = function () {
    $('#torrent-actions').hide();
    $('#set-label-wrapper').hide();
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
            .append('<a />')
                .addClass('awesome')
                .attr('href', './')
                .text('Ladda om');
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