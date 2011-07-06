/// <reference path="../lib/jquery-vsdoc.js" />
/// <reference path="00-Format.js" />
/// <reference path="00-UTorrent.js" />
/// <reference path="00-Canvas.js" />
/// <reference path="Root.js" />

if (window.Mums == undefined)
    window.Mums = {};

if (window.Mums.Knockout == undefined)
    window.Mums.Knockout = {};

Mums.Knockout.Init = function () {
    ko.bindingHandlers.renderCanvas = {
        init:   Mums.Knockout.CanvasHandler,
        update: Mums.Knockout.CanvasHandler
    };

    ko.applyBindings(Mums.Knockout.ViewModel);
}

Mums.Knockout.CanvasHandler = function (element, valueAccessor) {
    var value = ko.utils.unwrapObservable(valueAccessor()) / 100;
    Mums.Canvas.Render(element, value);
}

Mums.Knockout.ViewModel = {
    Sections: ko.observableArray()
};

Mums.Knockout.ViewModel.Finished = ko.dependentObservable(function () {
    var finished = 0;
    var sections = Mums.Knockout.ViewModel.Sections();

    for (var c = 0; c < sections.length; c++) {
        var torrents = sections[c].Torrents();
        for (var t = 0; t < torrents.length; t++) {
            if (torrents[t].Percentage() >= 100)
                finished++;
        }
    }

    return finished;
}, Mums.Knockout.ViewModel);

Mums.Knockout.UpdateTorrent = function (sectionIndex, torrentIndex, torrent) {
    var currTorrent = Mums.Knockout.ViewModel.Sections()[sectionIndex].Torrents()[torrentIndex];

    currTorrent.Hash(torrent.Hash);
    currTorrent.Name(torrent.Name);
    currTorrent.SizeInBytes(torrent.SizeInBytes);
    currTorrent.Percentage(torrent.Percentage);
    currTorrent.DownloadedInBytes(torrent.DownloadedInBytes);
    currTorrent.UploadedInBytes(torrent.UploadedInBytes);
    currTorrent.UploadSpeedInBytes(torrent.UploadSpeedInBytes);
    currTorrent.DownloadSpeedInBytes(torrent.DownloadSpeedInBytes);
    currTorrent.EstimatedTimeSeconds(torrent.EstimatedTimeSeconds);
    currTorrent.Label(torrent.Label);
    currTorrent.PeersConnected(torrent.PeersConnected);
    currTorrent.SeedsConnected(torrent.SeedsConnected);
    currTorrent.RemainingInBytes(torrent.RemainingInBytes);
    currTorrent.Status(torrent.Status);
    //    Ratio = double.Parse(input[index++]) / 1000d;
    //    EstimatedTime = TimeSpan.FromSeconds(double.Parse(input[index++]));
    //    PeersInSwarm = int.Parse(input[index++]);
    //    SeedsInSwarm = int.Parse(input[index++]);
    //    Availability = long.Parse(input[index++]);
    //    QueueOrder = int.Parse(input[index++]);
}

Mums.Knockout.UpdateSection = function (sectionIndex, torrents) {
    Mums.Knockout.PurgeTorrents(sectionIndex, newSectionData);

    var section = Mums.Knockout.ViewModel.Sections()[sectionIndex];
    var currTorrents = section.Torrents();

    for (var t = 0; t < torrents.length; t++) {
        var foundTorrent = false;
        for (var i = 0; i < section.Torrents().length; i++) {
            if (currTorrents[i].Hash == torrents[t].Hash) {
                foundTorrent = true;
                Mums.Knockout.UpdateTorrent(section, i, torrents[t]);
                break;
            }
        }

        if (!foundTorrent)
            Mums.Knockout.AddTorrent(sectionIndex, torrents[t]);
    }
}

Mums.Knockout.AddTorrent = function (sectionIndex, torrent) {
    torrent = Mums.Knockout.SetTorrentObservables(torrent);
    Mums.Knockout.ViewModel.Sections()[sectionIndex].Torrents.push(torrent);
}

Mums.Knockout.SetTorrentObservables = function (torrent) {
    torrent.Hash = ko.observable(torrent.Hash);
    torrent.Name = ko.observable(torrent.Name);
    torrent.SizeInBytes = ko.observable(torrent.SizeInBytes);
    torrent.Percentage = ko.observable(torrent.Percentage);
    torrent.DownloadedInBytes = ko.observable(torrent.DownloadedInBytes);
    torrent.UploadedInBytes = ko.observable(torrent.UploadedInBytes);
    torrent.UploadSpeedInBytes = ko.observable(torrent.UploadSpeedInBytes);
    torrent.DownloadSpeedInBytes = ko.observable(torrent.DownloadSpeedInBytes);
    torrent.EstimatedTimeSeconds = ko.observable(torrent.EstimatedTimeSeconds);
    torrent.Label = ko.observable(torrent.Label);
    torrent.PeersConnected = ko.observable(torrent.PeersConnected);
    torrent.SeedsConnected = ko.observable(torrent.SeedsConnected);
    torrent.RemainingInBytes = ko.observable(torrent.RemainingInBytes);
    torrent.Status = ko.observable(torrent.Status);

    return torrent;
}

Mums.Knockout.PurgeTorrents = function (sectionIndex, torrents) {
    var currTorrents = Mums.Knockout.ViewModel.Sections()[sectionIndex].Torrents();
    for (var i = 0; i < currTorrents.length; i++) {

        if (i >= Mums.Knockout.ViewModel.Sections()[sectionIndex].Torrents().length)
            break;

        for (var t = 0; t < torrents.length; t++) {
            if (currTorrents[i].Hash == torrents[t].Hash)
                continue;
        }

        Mums.Knockout.ViewModel.Sections()[sectionIndex].Torrents.splice(i, 1);
        i--;
    }
}

Mums.Knockout.PurgeSections = function (sections) {
    for (var i = 0; i < Mums.Knockout.ViewModel.Sections().length; i++) {
        if (i >= Mums.Knockout.ViewModel.Sections().length)
            break;

        for (var c = 0; c < sections.length; c++) {
            if (Mums.Knockout.ViewModel.Sections()[i].Id == sections[c].Id)
                continue;
        }

        Mums.Knockout.ViewModel.Sections.splice(i, 1);
        i--;
    }
}

Mums.Knockout.UpdateModel = function (sections) {
    Mums.Knockout.PurgeSections(sections);

    for (var c = 0; c < sections.length; c++) {
        var foundSection = false;

        for (var i = 0; i < Mums.Knockout.ViewModel.Sections().length; i++) {
            if (Mums.Knockout.ViewModel.Sections()[i].Id == sections[c].Id) {
                Mums.Knockout.UpdateSection(i, sections[c]);
                foundSection = true;
                break;
            }
        }

        if (!foundSection)
            Mums.Knockout.AddSection(sections[c]);
    }
}

Mums.Knockout.AddSection = function (newSection) {
    for (var t = 0; t < newSection.Torrents.length; t++)
        newSection.Torrents[t] = Mums.Knockout.SetTorrentObservables(newSection.Torrents[t]);

    newSection.Torrents = ko.observableArray(newSection.Torrents);
    newSection.Status = ko.observable(newSection.Status);
    newSection.Finished = ko.observable(newSection.Finished);

    newSection.Name = ko.dependentObservable(function () {
        return Mums.UTorrent.ParseStatus(this.Status(), this.Finished());
    }, newSection);

    Mums.Knockout.ViewModel.Sections.push(newSection);
}