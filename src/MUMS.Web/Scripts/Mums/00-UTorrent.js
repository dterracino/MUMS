
if (window.UTorrent == undefined)
    window.UTorrent = {};

/******************************************************************************
    Code below courtesy of "Lord Alderaan":
    http://forum.utorrent.com/viewtopic.php?id=50779
******************************************************************************/

UTorrent.Enums = {
    /**
    * Bitwise values for interpreting the status field in the torrents list
    * returned by "/gui/?list=1"
    */
    Status: {
        "STARTED": 1,
        "CHECKING": 2,
        "CHECK-START": 4,
        "CHECKED": 8,
        "ERROR": 16,
        "PAUSED": 32,
        "QUEUED": 64,
        "LOADED": 128
    },
    States: {
        Paused: 'Pausad',
        Seeding: 'Seedar',
        Downloading: 'Laddar ner',
        Checking: 'Kontrollerar',
        Error: 'Fel',
        QueuedSeed: 'Köad uppladdning',
        QueuedDownload: 'Köad nedladdning',
        Finished: 'Klar',
        Stopped: 'Stoppad'
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
UTorrent.ParseStatus = function (iStatus, bFinished) {
    if (iStatus & UTorrent.Enums.Status["STARTED"]) {
        if (iStatus & UTorrent.Enums.Status["PAUSED"]) 
            return UTorrent.Enums.States.Paused;
        else if (bFinished)
            return UTorrent.Enums.States.Seeding;

        return UTorrent.Enums.States.Downloading;
    }

    if (iStatus & UTorrent.Enums.Status["CHECKING"])
        return UTorrent.Enums.States.Checking;

    if (iStatus & UTorrent.Enums.Status["ERROR"])
        return UTorrent.Enums.States.Error;

    if (iStatus & UTorrent.Enums.Status["QUEUED"]) {
        if (bFinished)
            return UTorrent.Enums.States.QueuedSeed;

        return UTorrent.Enums.States.QueuedDownload;
    }

    if (bFinished)
        return UTorrent.Enums.States.Finished;

    return UTorrent.Enums.States.Stopped;
}