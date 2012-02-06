window.Index = {};

Index.poll = function () {
    Mums.Root.Ajax('/Root/GetTorrents/', {}, function (data) {
        Mums.Knockout.UpdateModel(data);
        setTimeout("Index.poll()", 2 * 1000);
    });
};

Index.pollEpisodes = function () {
    Mums.Root.Ajax('/Root/GetEpisodes/', {}, function (data) {
        Mums.Knockout.UpdateEpisodes(data);
        setTimeout("Index.pollEpisodes()", 60 * 1000);
    });
};

Index.init = function () {
    Index.poll();
    Index.pollEpisodes();
}