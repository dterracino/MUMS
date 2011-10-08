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

Mums.Root.RemoveTorrent = function (hash) {
    var $next = Mums.Root.GetNextWrapper(hash);
    if ($next.length == 0)
        $next = Mums.Root.GetPrevWrapper(hash);

    Mums.Root.Ajax($('#removeTorrentLink').attr('href'), { hash: hash }, function () {
        Mums.Root.HideTorrentActions();
    });

    $(':hidden[name=Hash][value=' + hash + ']')
        .closest('.torrent-wrapper')
        .fadeOut();

    Mums.Root.SetCurrWrapper($next);
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

    $('#removeTorrentLink').mousedown(function (e) {
        var hash = $('#hdnHash').val();
        Mums.Root.RemoveTorrent(hash);
        e.preventDefault();
        e.stopPropagation();
        return false;
    });

    $('#torrent-actions a')
        .not('#set-label-link')
        .not('#set-label-wrapper a')
        .not('#removeTorrentLink')
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

    Mums.Root.InitGlobalKeys();
    Mums.Root.InitTools();
    Mums.Root.StartPolling();
}

Mums.Root.GetCurrWrapper = function (hash) {
    return $(':hidden[name=Hash][value=' + hash + ']')
        .closest('.torrent-wrapper');
}

Mums.Root.GetNextWrapper = function (hash) {
    var $curr = Mums.Root.GetCurrWrapper(hash);
    var $next = $curr.next('.torrent-wrapper');

    if ($next)
        return $next;

    return $curr.closest('.section-wrapper')
        .next('.section-wrapper')
        .find('.torrent-wrapper:first');
}

Mums.Root.GetPrevWrapper = function (hash) {
    var $curr = Mums.Root.GetCurrWrapper(hash);
    var $prev = $curr.prev('.torrent-wrapper');

    if ($prev)
        return $prev;

    return $curr.closest('.section-wrapper')
        .prev('.section-wrapper')
        .find('.torrent-wrapper:last');
}

Mums.Root.SetCurrWrapper = function ($wrap) {
    var hdn = $wrap.find(':hidden[name=Hash]');
    if (hdn.length == 1) {
        Mums.Root.HideTorrentActions();
        Mums.Knockout.ViewModel.SelectedHash(hdn.val());
    }
}

Mums.Root.InitGlobalKeys = function () {
    $(document).keydown(function (e) {
        switch (e.keyCode) {
            case 13: // enter
                Mums.Root.TorrentActions(Mums.Knockout.ViewModel.SelectedHash());
                return false;
            case 27: // escape
                Mums.Root.HideTorrentActions();
                return false;
            case 38: // up
                var $prev = Mums.Root.GetPrevWrapper(Mums.Knockout.ViewModel.SelectedHash());
                if ($prev) {
                    Mums.Root.SetCurrWrapper($prev);
                    return false;
                }
                break;
            case 40: // down
                var $next = Mums.Root.GetNextWrapper(Mums.Knockout.ViewModel.SelectedHash());
                if ($next) {
                    Mums.Root.SetCurrWrapper($next);
                    return false;
                }
                break;
            case 46: // delete
                Mums.Root.RemoveTorrent(Mums.Knockout.ViewModel.SelectedHash());
                break;
        }
    });
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

    Mums.Knockout.ViewModel.SelectedHash(hash);

    $('#torrent-actions')
        .css('left', left)
        .css('top', top)
        .fadeIn('fast');

    $('#hdnHash').val(hash);

    $(document).one('mousedown', function () {
        Mums.Root.HideTorrentActions();
    });

    $('#torrent-actions').find('a:first').focus();

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