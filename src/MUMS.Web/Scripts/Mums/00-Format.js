/// <reference path="../lib/jquery-vsdoc.js" />
/// <reference path="00-Canvas.js" />
/// <reference path="00-UTorrent.js" />
/// <reference path="01-Knockout.js" />
/// <reference path="Root.js" />

if (window.Mums == undefined)
    window.Mums = {};

if (window.Mums.Format == undefined)
    window.Mums.Format = {};

Mums.Format.Percentage = function (percentage) {
    return Math.floor(percentage);
}

Mums.Format.Label = function (lbl) {
    if (lbl && lbl != "")
        return "<strong>Label:</strong> " + lbl;
    return "";
}

Mums.Format.PrettySize = function (bytes) {
    return Mums.Format.FriendlyRate(bytes, ["b", "kb", "mb", "gb"]);
}

Mums.Format.FriendlyRate = function GetFriendlyRate(rate, units, factor) {

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

Mums.Format.PrettySpeed = function (bytes) {
    return Mums.Format.FriendlyRate(bytes, ["b/s", "kb/s", "mb/s", "gb/s"]);
}

Mums.Format.PrettyEta = function (eta) {
    if (!eta || eta == undefined || eta <= 0 || eta == NaN)
        return "";

    var found = false;
    var format = "";

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
            else
                format = Math.floor(number) + suffix;
        }
    });

    return format;
}