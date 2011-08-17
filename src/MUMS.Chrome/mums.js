$(function () {

    var sites = {
        'piratebay'     : { 
            contains    : 'http://thepiratebay.org/', 
            torrentUrl  : function() { return  $('a[href$=".torrent"]').first().attr('href'); }, 
            insert      : function(wrapper) { wrapper.insertBefore($('div.download:first'));  }
        },
        'torrentbytes'  : {
            contains    : 'http://www.torrentbytes.net/',
            getLink     : function() { return $('a[href$=".torrent"]').first(); },
            torrentUrl  : function() { return sites.torrentbytes.getLink().attr('href'); },
            insert      : function(wrapper) { wrapper.insertBefore(sites.torrentbytes.getLink().closest('table')); }
        }/*,
        'tankafetast'   : {
            contains    : 'http://www.tankafetast.com/',
            getLink     : function() { return $('a.tf2button[href^="/go/torrent"]').first(); },
            torrentUrl  : function() { return sites.tankafetast.getLink().attr('href'); },
            insert      : function(wrapper) { wrapper.css('clear','none').insertBefore(sites.tankafetast.getLink()); }
        }*/
    };

    var site = getSite();
    var url = site.torrentUrl();

    var wrapper = $('<div />')
        .addClass('mumswrapper')
        .addClass('disabled');

    site.insert(wrapper);

    var form = $('<form />')
        .attr('action', 'http://mums.chsk.se/Root/AddRemoteUrl/')
        .attr('method', 'post')
        .appendTo(wrapper)
        .submit(function (e) {
            location.href = $(this).attr('action') + "?" + $(this).serialize();
            e.preventDefault();
            return false;
        });

    var button = $('<a />')
        .addClass('mumsbutton')
        .addClass('white')
        .attr('href','#')
        .text('Ladda ner på MUMS')
        .click(function(e) {
            $(this).closest('.mumswrapper').toggleClass('disabled');
            e.preventDefault();
            return false;
        })
        .appendTo(form);
    
    var hdn = $('<input />')
        .attr('type', 'hidden')
        .attr('name', 'url')
        .val(url)
        .appendTo('form');

    var lbl = $('<label />')
        .attr('for', 'mums-label')
        .text('Välj kategori:')
        .appendTo(form);

    var sel = $('<select />')
        .attr('name', 'label')
        .attr('id', 'mums-label')
        .append(makeOpt('Filmer'))
        .append(makeOpt('Serier'))
        .append(makeOpt('Annat'))
        .appendTo(form);

   var ok = $('<a />')
        .addClass('mumsbutton')
        .addClass('red')
        .attr('href','#')
        .text('Ok')
        .click(function(e) {
            var $frm = $(this).closest('form')
            $frm.find(':submit').click();
            return false;
        })
        .appendTo(form);

    var submit = $('<input />')
        .attr('type','submit')
        .hide()
        .appendTo(form);


    function makeOpt(text) {
        return $('<option />')
                    .val(text)
                    .text(text);
    }

    function getSite() {
        var currentUrl = document.URL;
        for (var site in sites) {
            console.log(site);
            if (currentUrl.indexOf(sites[site].contains) >= 0)
                return sites[site];
        }

        return {};
    }
});
