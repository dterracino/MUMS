/// <reference path="../lib/jquery-vsdoc.js" />
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
            return new Mums.Knockout.GetSectionModel(options);
        }
    }
};

Mums.Knockout.TorrentMapping = {
    'Torrents': {
        key: function (data) {
            return ko.utils.unwrapObservable(data.Hash);
        }
    }
};

Mums.Knockout.GetSectionModel = function (options) {
    ko.mapping.fromJS(
        options.data,
        Mums.Knockout.TorrentMapping,
        this
    );

    this.Name = ko.dependentObservable(function () {
        return Mums.UTorrent.ParseStatus(this.Status(), this.Finished());
    }, this);
};

Mums.Knockout.CanvasHandler = function (element, valueAccessor) {
    var value = ko.utils.unwrapObservable(valueAccessor()) / 100;
    Mums.Canvas.Render(element, value);
}

Mums.Knockout.Init = function (data) {
    ko.bindingHandlers.renderCanvas = {
        init: Mums.Knockout.CanvasHandler,
        update: Mums.Knockout.CanvasHandler
    };

    Mums.Knockout.ViewModel = ko.mapping.fromJS(
        data,
        Mums.Knockout.SectionMapping
    );

    Mums.Knockout.ViewModel.Finished = ko.dependentObservable(function () {
        var finished = 0;
        var sections = this.Sections();

        for (var c = 0; c < sections.length; c++) {
            var torrents = sections[c].Torrents();
            for (var t = 0; t < torrents.length; t++) {
                if (torrents[t].Percentage() >= 100)
                    finished++;
            }
        }

        return finished;
    }, Mums.Knockout.ViewModel);

    ko.applyBindings(Mums.Knockout.ViewModel);
}

Mums.Knockout.UpdateModel = function (data) {
    console.log(data);

    if (Mums.Knockout.ViewModel == undefined)
        Mums.Knockout.Init(data);

    ko.mapping.updateFromJS(Mums.Knockout.ViewModel, data);
}
