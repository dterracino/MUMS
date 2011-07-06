/// <reference path="../lib/jquery-vsdoc.js" />
/// <reference path="00-Format.js" />
/// <reference path="00-Canvas.js" />
/// <reference path="01-Knockout.js" />
/// <reference path="Root.js" />

if (window.Mums == undefined)
    window.Mums = {};

if (window.Mums.UTorrent == undefined)
    window.Mums.UTorrent = {};

/******************************************************************************
    Code below courtesy of "Lord Alderaan":
    http://forum.utorrent.com/viewtopic.php?id=50779
******************************************************************************/

Mums.UTorrent.Enums = {
    /**
    * Bitwise values for interpreting the status field in the torrents list
    * returned by "/gui/?list=1"
    */
    "Status" : {
        "STARTED": 1,
        "CHECKING": 2,
        "CHECK-START": 4,
        "CHECKED": 8,
        "ERROR": 16,
        "PAUSED": 32,
        "QUEUED": 64,
        "LOADED": 128
    }
};

/**
* Converts a bitwise torrent status value into a string.
*
* @param iStatus   The bitwise torrent status value
* @param bFinished A boolean value indicating the torrent finished state
*
* @return The string representation of the torrent status.
*/
Mums.UTorrent.ParseStatus = function (iStatus, bFinished) {
    if (iStatus & Mums.UTorrent.Enums.Status["STARTED"]) {
        var sForced = ((!(iStatus & Mums.UTorrent.Enums.Status["QUEUED"])) ? ' [F]' : '');

        if (iStatus & Mums.UTorrent.Enums.Status["PAUSED"]) return 'Paused';
        else if (bFinished) return 'Seeding' + sForced;
        return 'Downloading' + sForced;
    }

    if (iStatus & Mums.UTorrent.Enums.Status["CHECKING"])
        return 'Checking';

    if (iStatus & Mums.UTorrent.Enums.Status["ERROR"])
        return 'Error';

    if (iStatus & Mums.UTorrent.Enums.Status["QUEUED"]) {
        if (bFinished) return 'Queued Seed';
        return 'Queued Download';
    }

    if (bFinished) return 'Finished';
    return 'Stopped';
}