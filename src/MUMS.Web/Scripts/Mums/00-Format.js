/// <reference path="../lib/jquery-vsdoc.js" />
/// <reference path="00-Canvas.js" />
/// <reference path="00-UTorrent.js" />
/// <reference path="01-Knockout.js" />
/// <reference path="Root.js" />

if (window.Format == undefined)
    window.Format = {};

Format.Percentage = function (percentage) {
    return Math.floor(percentage);
}

Format.Label = function (lbl) {
    if (lbl && lbl != "")
        return "<strong>Label:</strong> " + lbl;
    return "";
}

Format.PrettySize = function (bytes) {
    return Format.FriendlyRate(bytes, ["b", "kb", "mb", "gb"]);
}

Format.FriendlyRate = function (rate, units, factor) {

    units = units || ["b", "kb", "Mb", "Gb"];
    factor = factor || 1024;

    for (var i = 0; i < units.length; i++) {
        var u = units[i];

        if (rate < 10) {
            if (i == 0)
                return Math.round(rate) + u;
            else
                return Math.round(rate * 10) / 10 + u;
        }
        else if (rate < factor) {
            return Math.round(rate) + u;
        }

        rate = rate / factor;
    }

    return "0" + units[0];
}

Format.PrettySpeed = function (bytes) {
    return Format.FriendlyRate(bytes, ["b/s", "kb/s", "mb/s", "gb/s"]);
}

Format.PrettyEta = function (eta) {
    if (!eta || eta == undefined || eta <= 0 || eta == NaN)
        return "";

    var found = false;
    var format = "";
    var singularMap = {
        'd': 'dag',
        't': 'timme',
        'm': 'minut',
        's': 'sekund'
    };
    var pluralMap = {
        'd': 'dagar',
        't': 'timmar',
        'm': 'minuter',
        's': 'sekunder'
    };

    var timeMap = {
        'd': 86400,
        't': 3600,
        'm': 60,
        's': 1
    };


    $.each(timeMap, function (suffix, range) {
        if (format != "")
            return false;

        if (eta >= range) {
            var number = eta;
            if (range > 0)
                number = eta / range;

            if (number >= 100)
                format = "";
            else {
                number = Math.floor(number);
                if (number == 1)
                    suffix = singularMap[suffix];
                else
                    suffix = pluralMap[suffix];

                format = number + ' ' + suffix;
            }
        }
    });

    return format;
}