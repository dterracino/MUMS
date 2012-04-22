/// <reference path="../lib/knockout-2.0.0.js" />
/// <reference path="../lib/knockout-mapping.js" />

if (window.Mums == undefined)
    window.Mums = {};

if (window.Mums.Knockout == undefined)
    window.Mums.Knockout = {};

Mums.Knockout.SectionMapping = {
    'Sections': {
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        },
        create: function (options) {
            return new Mums.Knockout.SectionModel(options);
        }
    },
    'LatestEpisodes': {
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        },
        create: function (options) {
            return new Mums.Knockout.EpisodeModel(options);
        }
    },
    'SelectedTorrent': {
        key: function (data) {
            return ko.utils.unwrapObservable(data.Hash);
        },
        create: function (options) {
            return new Mums.Knockout.SelectedTorrentModel(options);
        }
    }
};

Mums.Knockout.TorrentMapping = {
    'Torrents': {
        key: function (data) {
            return ko.utils.unwrapObservable(data.Hash);
        },
        create: function (options) {
            return new Mums.Knockout.TorrentModel(options);
        }
    }
};

Mums.Knockout.SelectedTorrentModel = function (options) {
    var self = this;
    ko.mapping.fromJS(options.data, {
        'Files': {
            key: function (d) {
                return ko.utils.unwrapObservable(d.Path);
            }
        }
    }, self);

    self.action = function (data, e) {
        var $lnk = $(e.target);
        var url = $lnk.attr('href');
        Index.ajax(url, { hash: self.Hash() }, function (response) {
            console.log(response);
        });
    }

    self.setLabel = function (data, e) {
        var $lnk = $(e.target);
        var url = $lnk.attr('href');
        var label = $lnk.text();

        Index.ajax(url, { newLabel: label, hash: self.Hash() }, function (response) {
            console.log(response);
        });
    }

    self.remove = function (data, e) {
        var $lnk = $(e.target);
        var url = $lnk.attr('href');

        Index.ajax(url, { hash: self.Hash() }, function (response) {
            console.log(response);
            Mums.Knockout.ViewModel.CloseDetails();
        });
    }

    self.reset = function () {
        ko.mapping.fromJS(Mums.Knockout.TorrentSkeleton, {}, self);
    }
}

Mums.Knockout.EpisodeModel = function (options) {
    var self = this;
    ko.mapping.fromJS(options.data, {}, self);

    self.Subtext = ko.computed(function () {
        var seconds = self.SecondsSinceAdded();
        if (!seconds || seconds < 0)
            return '...';
        else if (seconds == 0)
            return 'Jämt nu!';

        var elapsed = Format.PrettyEta(seconds);
        return elapsed + ' sedan';
    });
};

Mums.Knockout.TorrentModel = function (options) {
    var self = this;
    ko.mapping.fromJS(options.data, {}, self);

    self.State = ko.computed(function () {
        var finished = self.Percentage() >= 100;
        return UTorrent.ParseStatus(self.Status(), finished);
    });
}

Mums.Knockout.SectionModel = function (options) {
    var self = this;
    ko.mapping.fromJS(
        options.data,
        Mums.Knockout.TorrentMapping,
        self
    );

    self.Name = ko.computed(function () {
        return UTorrent.ParseStatus(self.Status(), self.Finished());
    });
};

Mums.Knockout.RootModel = function (data) {
    var self = this;
    var detailsTimer = undefined;

    if (!data.LatestEpisodes)
        data.LatestEpisodes = [];
    if (!data.Sections)
        data.Sections = [];
    if (!data.DownloadSpeedInBytes)
        data.DownloadSpeedInBytes = 0;
    if (!data.UploadSpeedInBytes)
        data.UploadSpeedInBytes = 0;

    data.SelectedTorrent = Mums.Knockout.TorrentSkeleton;
    data.ShowDetails = false;

    ko.mapping.fromJS(data, Mums.Knockout.SectionMapping, self);

    self.ResetSelectedTorrent = function () {
        self.SelectedTorrent.reset();
    };

    self.TorrentClicked = function (t) {
        self.ResetSelectedTorrent();
        self.SelectedTorrent.Hash(t.Hash());
        self.ShowDetails(true);
        return true;
    };

    self.CloseDetails = function () {
        console.log('showDetails');
        self.ResetSelectedTorrent();
        self.ShowDetails(false);
        return true;
    };
}