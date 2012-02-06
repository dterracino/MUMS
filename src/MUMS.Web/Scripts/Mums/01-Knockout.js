/// <reference path="../lib/jquery-vsdoc.js" />
/// <reference path="../lib/knockout-1.2.1.js" />
/// <reference path="../lib/knockout-mapping.js" />
/// <reference path="00-Format.js" />
/// <reference path="00-UTorrent.js" />
/// <reference path="00-Canvas.js" />
/// <reference path="Root.js" />

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

Mums.Knockout.EpisodeModel = function (options) {
    var self = this;
    ko.mapping.fromJS(options.data, {}, self);

    self.Subtext = ko.computed(function () {
        var elapsed = Format.PrettyEta(self.SecondsSinceAdded());
        return 'tillagd för ' + elapsed + ' sedan';
    });
};

Mums.Knockout.TorrentModel = function (options) {
    var self = this;
    ko.mapping.fromJS(options.data, {}, self);

    var subtextDownload = function (byteSize, etaSeconds) {
        var size = Format.PrettySize(byteSize);
        var eta = Format.PrettyEta(etaSeconds);
        return '<span class="size">'+size + '</span>, <span class="eta">' + eta + ' kvar </span>';
    }

    self.Subtext = ko.computed(function () {
        var finished = self.Percentage() >= 100;
        var state = UTorrent.ParseStatus(self.Status(), finished);
        switch (state) {
            case UTorrent.Enums.States.Downloading:
                return subtextDownload(self.SizeInBytes(), self.EstimatedTimeSeconds());
            default:
                return 'na';
        }
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

Mums.Knockout.CanvasHandler = function (element, valueAccessor) {
    var value = ko.utils.unwrapObservable(valueAccessor()) / 100;
    Mums.Canvas.Render(element, value);
}

Mums.Knockout.Init = function (data) {
    if (!data.LatestEpisodes)
        data.LatestEpisodes = [];
    if (!data.Sections)
        data.Sections = [];

    Mums.Knockout.ViewModel = ko.mapping.fromJS(
        data,
        Mums.Knockout.SectionMapping
    );

    Mums.Knockout.ViewModel.Finished = ko.computed(function () {
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
    });

    ko.applyBindings(Mums.Knockout.ViewModel);
}

Mums.Knockout.UpdateModel = function (data) {
    if (Mums.Knockout.ViewModel == undefined)
        Mums.Knockout.Init(data);

    ko.mapping.fromJS(data, Mums.Knockout.ViewModel);
}

Mums.Knockout.UpdateEpisodes = function (data) {
    if (Mums.Knockout.ViewModel == undefined)
        Mums.Knockout.Init(data);

    ko.mapping.fromJS(data, Mums.Knockout.ViewModel);
}