/// <reference path="../lib/knockout-2.0.0.js" />
/// <reference path="../lib/knockout-mapping.js" />

if (window.Mums == undefined)
    window.Mums = {};

if (window.Mums.Knockout == undefined)
    window.Mums.Knockout = {};

var IndexModel = function () {
    var torrentsTimer = undefined;
    var episodesTimer = undefined;
    var detailsTimer = undefined;

    this.fetchDetails = function (cb) {
        var vm = Mums.Knockout.ViewModel;
        Index.ajax('Root/GetTorrent/', { hash: vm.SelectedTorrent.Hash() }, function (response) {
            ko.mapping.fromJS(response, {}, vm.SelectedTorrent);
            cb(response);
        });
    }

    var resetTimer = function (t) {
        if (t != undefined)
            window.clearTimeout(t);
    };

    var makeTimer = function (f, timeOut) {
        timeOut = timeOut || 2000;
        return setTimeout(f, timeOut);
    };

    var setDetailsTimer = function () {
        detailsTimer = makeTimer("Index.pollSelected()", 1 * 1000);
    };

    var setTorrentsTimer = function () {
        torrentsTimer = makeTimer("Index.pollTorrents()", 3 * 1000);
    };

    var setEpisodesTimer = function () {
        episodesTimer = makeTimer("Index.pollEpisodes()", 60 * 1000);
    };

    var initModel = function (data) {
        Mums.Knockout.ViewModel = new Mums.Knockout.RootModel(data);
        var vm = Mums.Knockout.ViewModel;

        vm.ShowDetails.subscribe(function (newVal) {
            resetTimer(torrentsTimer);
            resetTimer(detailsTimer);

            if (newVal) {
                Index.fetchDetails(function (r) {
                    if (vm.ShowDetails())
                        setDetailsTimer();
                    else
                        setTorrentsTimer();
                });
            } else {
                Index.pollTorrents();
            }
        });

        ko.applyBindings(Mums.Knockout.ViewModel);
    }

    var updateRoot = function (data) {
        if (Mums.Knockout.ViewModel == undefined)
            initModel(data);
        else
            ko.mapping.fromJS(data, Mums.Knockout.ViewModel);
    };

    this.ajax = function (url, data, callback) {
        $.post(url, data, function (responseData) {
            if (responseData.Ok === false) {
                console.log(responseData);
            } else {
                callback(responseData);
            }
        }).error(function (jqXHR, errorType, excObj) {
            console.log(jqXHR);
            console.log(errorType);
            console.log(excObj);
        });
    }

    this.pollTorrents = function () {
        Index.ajax('/Root/GetTorrents/', {}, function (data) {
            updateRoot(data);
            setTorrentsTimer();
        });
    };

    this.pollEpisodes = function () {
        Index.ajax('/Root/GetEpisodes/', {}, function (data) {
            updateRoot(data);
            setEpisodesTimer();
        });
    };

    this.pollSelected = function () {
        var vm = Mums.Knockout.ViewModel;
        if (!vm.ShowDetails())
            return;

        Index.fetchDetails(function (r) {
            if (vm.ShowDetails())
                setDetailsTimer();
        });
    };

    this.init = function (torrentDetailsModel) {
        Mums.Knockout.TorrentSkeleton = torrentDetailsModel;
        Mums.Knockout.TorrentSkeleton.Name = "...";
        Index.pollTorrents();
        Index.pollEpisodes();
    }
};

window.Index = new IndexModel();
