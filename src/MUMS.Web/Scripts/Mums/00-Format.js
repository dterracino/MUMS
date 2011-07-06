/// <reference path="../lib/jquery-vsdoc.js" />
/// <reference path="00-Canvas.js" />
/// <reference path="00-UTorrent.js" />
/// <reference path="01-Knockout.js" />
/// <reference path="Root.js" />

if (window.Mums == undefined)
    window.Mums = {};

if (window.Mums.Format == undefined)
    window.Mums.Format = {};

Mums.Format.PrefixMap = {
    'gb': { range: 1073741824, precision: 1 },
    'mb': { range: 1048576, precision: 0 },
    'kb': { range: 1024, precision: 0 },
    'b' : { range: 0, precision: 0 }
};

Mums.Format.TimeMap = {
    'd' : 86400,
    't' : 3600,
    'm' : 60,
    's' : 1
};

Mums.Format.Percentage = function (percentage) {
    return Math.floor(percentage);
}

Mums.Format.Label = function (lbl) {
    if (lbl && lbl != "")
        return "<strong>Label:</strong> " + lbl;
    return "";
}

Mums.Format.PrettySize = function (bytes) {
    if (!bytes || bytes == undefined || bytes <= 0 || bytes == NaN)
        return "0b";

    var found = false;
    var format = "";

    $.each(Mums.Format.PrefixMap, function (prefix, setting) {
        if (format != "")
            return false;

        if (bytes >= setting.range) {
            var number = bytes;
            if (setting.range > 0)
                number = bytes / setting.range;

            if (setting.precision > 0)
                format = number.toFixed(setting.precision) + prefix;
            else
                format = Math.floor(number) + prefix;
        }
    });

    return format;
}

Mums.Format.PrettySpeed = function (bytes) {
    if (!bytes || bytes == undefined || bytes <= 0 || bytes == NaN)
        return "0b/s";

    var found = false;
    var format = "";
    var prevPrefix = "";

    $.each(Mums.Format.PrefixMap, function (prefix, setting) {
        if (format != "")
            return false;
        
        var range = setting.range;

        if (bytes >= range) {
            var number = bytes;
            var div = range;

            if (prevPrefix == "")
                prevPrefix = prefix;
            else
                div = (range == 0) ? 1024 : range * 1024;

            if (div > 0)
                number = bytes / div;

            format = number.toFixed(1) + prevPrefix + "/s";
        }

        prevPrefix = prefix;
    });

    return format;
}

Mums.Format.PrettyEta = function (eta) {
    if (!eta || eta == undefined || eta <= 0 || eta == NaN)
        return "";

    var found = false;
    var format = "";

    $.each(Mums.Format.TimeMap, function (suffix, range) {
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