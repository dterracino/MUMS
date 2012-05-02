$(function () {
    var torrentModel = function (url, hash) {
        var self = this;
        self.url = $.trim(url || '');
        self.hash = $.trim(hash || '');
    }

    var sites = {
        'piratebay': {
            matches: ['http://thepiratebay.org/torrent/', 'http://thepiratebay.se/torrent/'],
            getModel: function () {
                var hash = $('#details').find('dt:contains("Info Hash:")').next('dd').text();
                var url = '';

                var magnet = $('a[href^="magnet:?"]:first');
                if (magnet.length > 0)
                    url = magnet.attr('href');
                else
                    url = $('a[href$=".torrent"]:first').attr('href');

                return new torrentModel(url, hash);
            },
            insert: function (wrapper) { wrapper.insertBefore($('div.download:first')); }
        },
        'torrentbytes': {
            matches: ['http://www.torrentbytes.net/details.php'],
            getLink: function () { return $('a[href$=".torrent"]:first'); },
            getModel: function () {
                var url = sites.torrentbytes.getLink().attr('href');
                var hash = $('#content').find('table:first').find('td:contains("Info hash"):first').next('td').text();
                return new torrentModel(url, hash);
            },
            insert: function (wrapper) { wrapper.insertBefore(sites.torrentbytes.getLink().closest('table')); }
        },
        'kickass': {
            matches: ['http://kat.ph/'],
            getModel: function () {
                var links = $('.downloadButtonGroup');
                if (links.length <= 0)
                    return new torrentModel();

                var url = '';
                var hash = '';

                var magnet = links.find('a[href^="magnet:?"]:first');
                if (magnet.length > 0) {
                    url = magnet.attr('href');
                } else {
                    url = 'http://kat.ph' + links.find('a[href^="/torrents/"]:first').attr('href');
                    hash = $('#second').find('span:last').text().substr('Torrent hash: '.length);
                }

                return new torrentModel(url, hash);
            },
            insert: function (wrapper) {
                $('.downloadButtonGroup').append(wrapper);
            }
        },
        'tankafetast': {
            matches: ['http://www.tankafetast.com/torrent/'],
            getLink: function () { return $('a[href^="/go/torrent"]:first'); },
            getModel: function () {
                var url = sites.tankafetast.getLink().attr('href');
                return new torrentModel(url, '');
            },
            insert: function (wrapper) {
                wrapper
                    .css('clear', 'none')
                    .insertAfter(sites.tankafetast.getLink()); 
            }
        }
    };

    var site = getSite();
    console.log('site', site);
    if (!site)
        return;

    var model = site.getModel();
    if (!model || !model.url || model.url == '')
        return;

    var url = model.url;

    var outer = $('<div />')
        .addClass('mums-outerwrapper');

    var wrapper = $('<div />')
        .addClass('mumswrapper')
        .addClass('disabled')
        .appendTo(outer);

    site.insert(outer);

    var button = $('<button />')
        .addClass('button nice white radius mumsbutton')
        .attr('type', 'button')
        .click(function (e) {
            $(this).closest('.mumswrapper').toggleClass('disabled');
            e.preventDefault();
            return false;
        })
        .append($('<img />').attr('src', chrome.extension.getURL('button.png')))
        .appendTo(wrapper);

    $.each(['Filmer', 'Serier', 'Annat'], function () {
        var label = this;

        var btn = $('<button />')
            .addClass('button nice radius red small')
            .attr('type', 'button')
            .append($('<span />').text(label).addClass('mumslabel'))
            .click(function (e) {
                $(this).closest('.mumswrapper').find('button').attr('disabled', 'disabled');

                $.get('http://mums.chsk.se/Root/AddRemoteUrl', { url: model.url, label: label, hash: model.hash }, function (response) {
                    wrapper.html('<p>Successfully added the torrent to <a href="http://mums.chsk.se/">http://mums.chsk.se/</a>!</p>');
                });

                return false;
            })
            .appendTo(wrapper);
    });

    function getSite() {
        var currentUrl = document.URL;
        for (var site in sites) {
            for (var m in sites[site].matches)
                if (currentUrl.indexOf(sites[site].matches[m]) >= 0)
                    return sites[site];
        }

        return {};
    }
});
