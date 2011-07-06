/// <reference path="../lib/jquery-vsdoc.js" />
/// <reference path="00-Format.js" />
/// <reference path="00-UTorrent.js" />
/// <reference path="01-Knockout.js" />
/// <reference path="Root.js" />

if (window.Mums == undefined)
    window.Mums = {};

if (window.Mums.Canvas == undefined)
    window.Mums.Canvas = {};

Mums.Canvas.StrokeWidth = 10;
Mums.Canvas.CanvasSize = 60;
Mums.Canvas.StartAngle = Math.PI;

Mums.Canvas.StyleInProgress = "rgba(150,0,0,0.5)";
Mums.Canvas.StyleFinished = "rgba(0,70,0,0.5)";

Mums.Canvas.Render = function (canvas, percentage) {
    var ending = Mums.Canvas.StartAngle + percentage * 2 * Math.PI;
    var strokeStyle = (percentage >= 1)
        ? Mums.Canvas.StyleFinished
        : Mums.Canvas.StyleInProgress;
    
    Mums.Canvas.StrokeArcs(canvas, ending, strokeStyle);
}

Mums.Canvas.StrokeArcs = function(canvas, endingAngle, strokeStyle) {
    var context = canvas.getContext("2d");
    context.lineWidth = Mums.Canvas.StrokeWidth;

    var centerX = Mums.Canvas.CanvasSize / 2;
    var centerY = Mums.Canvas.CanvasSize / 2;
    var radius = (Mums.Canvas.CanvasSize - Mums.Canvas.StrokeWidth) / 2;
    var counterclockwise = false;

    context.beginPath();
    context.arc(centerX, centerY, radius, 0, 2 * Math.PI, counterclockwise);
    context.strokeStyle = "rgba(0,0,0,0.1)";
    context.stroke();

    context.beginPath();
    context.arc(centerX, centerY, radius, Mums.Canvas.StartAngle, endingAngle, counterclockwise);
    context.strokeStyle = strokeStyle;
    context.stroke();
}
